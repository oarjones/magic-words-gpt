import firebase_functions.db_fn as database_fn
import firebase_admin
from firebase_admin import credentials, db
import datetime
from firebase_functions import options

# Inicialización de Firebase
if not firebase_admin._apps:
    firebase_admin.initialize_app()
options.set_global_options(region=options.SupportedRegion.EUROPE_WEST1)

@database_fn.on_value_created(reference="/gameWaitRoom/{userId}")
def join_players(event: database_fn.Event) -> None:
    user_id = event.params["userId"]
    user_data = event.data

    print(f"[INFO] Evento recibido para user_id: {user_id}. Datos: {user_data}")

    # Si el jugador se desconecta, no hacemos nada
    if user_data is None:
        print(f"[INFO] Jugador {user_id} se desconectó de gameWaitRoom (o fue emparejado).")
        return

    # Verificamos que los campos esenciales existan y no sean None
    required_fields = ["langCode", "userName", "level", "status"]
    for field in required_fields:
        if user_data.get(field) is None:
            print(f"[ERROR] El campo '{field}' del jugador {user_id} es None. Datos recibidos: {user_data}")
            return

    # Si el jugador ya fue emparejado (status = "matched"), no hacemos nada.
    if user_data.get("status") == "matched":
        print(f"[INFO] Jugador {user_id} ya está emparejado (estado 'matched').")
        return

    # Comprobación inicial para evitar duplicados
    existing_player_ref = db.reference(f"/gameWaitRoom/{user_id}")
    existing_player_data = existing_player_ref.get()
    print(f"[DEBUG] Datos existentes para {user_id}: {existing_player_data}")
    if existing_player_data is not None and existing_player_data.get("status") != "waiting":
        print(f"[WARN] Jugador {user_id} ya existe en gameWaitRoom con un estado diferente.")
        return

    # Función de transacción que trabaja sobre /gameWaitRoom
    def try_to_match(wait_room):
        print("[TRANSACTION] Iniciando transacción sobre /gameWaitRoom")
        if wait_room is None:
            wait_room = {}
            print("[TRANSACTION] wait_room estaba vacío, inicializando como dict.")

        # Si el jugador no está en wait_room aún, lo añadimos con todos sus datos
        if user_id not in wait_room:
            wait_room[user_id] = user_data
            print(f"[TRANSACTION] Agregado jugador {user_id} a wait_room con datos: {user_data}")
        else:
            print(f"[TRANSACTION] Jugador {user_id} ya existe en wait_room. Datos actuales: {wait_room[user_id]}")

        second_player_id = None
        # Buscamos un jugador distinto en estado "waiting" y con el mismo langCode
        for other_user_id, other_user_data in wait_room.items():
            if other_user_id != user_id:
                # Log extra para ver los datos del otro jugador
                print(f"[TRANSACTION] Revisando jugador {other_user_id}: {other_user_data}")
                if (other_user_data.get("status") == "waiting" and 
                    other_user_data.get("langCode") == user_data.get("langCode")):
                    second_player_id = other_user_id
                    print(f"[TRANSACTION] Encontrado oponente {other_user_id} para jugador {user_id}")
                    break

        if second_player_id:
            # Verificamos que los campos esenciales del segundo jugador no sean None
            for field in required_fields:
                if wait_room[second_player_id].get(field) is None:
                    print(f"[ERROR] El campo '{field}' del jugador {second_player_id} es None. Datos: {wait_room[second_player_id]}")
                    # En este caso, no se puede hacer match, se devuelve el estado sin emparejar
                    wait_room[user_id]["status"] = "waiting"
                    return wait_room

            # Emparejamos a ambos jugadores: se actualiza el estado a "matched"
            wait_room[user_id]["status"] = "matched"
            wait_room[second_player_id]["status"] = "matched"
            # Se añade un flag interno para indicar el match
            wait_room["_match"] = { "first": user_id, "second": second_player_id }
            print(f"[TRANSACTION] Jugadores {user_id} y {second_player_id} marcados como 'matched'")
        else:
            # Si no se encontró oponente, aseguramos que el jugador quede en "waiting"
            wait_room[user_id]["status"] = "waiting"
            print(f"[TRANSACTION] No se encontró oponente para {user_id}. Estado establecido a 'waiting'.")

        print(f"[TRANSACTION] Estado final de wait_room en la transacción: {wait_room}")
        return wait_room

    try:
        print(f"[INFO] Ejecutando transacción para el jugador {user_id} sobre /gameWaitRoom")
        # Ejecutamos la transacción sobre /gameWaitRoom
        updated_wait_room = db.reference("/gameWaitRoom").transaction(try_to_match)
        print(f"[INFO] Transacción completada. Estado actualizado: {updated_wait_room}")

        # Si se produjo un match, se procede a crear la partida
        if updated_wait_room and "_match" in updated_wait_room:
            match_info = updated_wait_room["_match"]
            first_player_id = match_info["first"]
            second_player_id = match_info["second"]

            print(f"[INFO] Match confirmado entre {first_player_id} y {second_player_id}")

            # Obtenemos los datos completos de ambos jugadores desde updated_wait_room
            first_player_data = updated_wait_room.get(first_player_id)
            second_player_data = updated_wait_room.get(second_player_id)
            print(f"[DEBUG] Datos del primer jugador: {first_player_data}")
            print(f"[DEBUG] Datos del segundo jugador: {second_player_data}")

            # Verificamos que los campos esenciales de ambos jugadores sean válidos
            for pid, pdata in [(first_player_id, first_player_data), (second_player_id, second_player_data)]:
                for field in required_fields:
                    if pdata.get(field) is None:
                        print(f"[ERROR] El campo '{field}' del jugador {pid} es None. Datos: {pdata}")
                        print(f"[ERROR] Abortando la creación de la partida.")
                        return

            new_game_ref = db.reference("/games").push()
            game_id = new_game_ref.key
            new_game = {
                "status": 2,
                "type": 2,
                "langCode": first_player_data["langCode"],
                "createdAt": datetime.datetime.now().timestamp(),
                "playersInfo": {
                    first_player_id: {
                        "userName": first_player_data["userName"],
                        "level": first_player_data["level"],
                        "master": True,
                        "gameBoardLoaded": False
                    },
                    second_player_id: {
                        "userName": second_player_data["userName"],
                        "level": second_player_data["level"],
                        "master": False,
                        "gameBoardLoaded": False
                    }
                },
                "gameBoard": {}
            }
            print(f"[INFO] Creando partida con datos: {new_game}")
            new_game_ref.set(new_game)
            print(f"[INFO] Partida creada: {game_id} ({first_player_id} vs {second_player_id})")

            # Opcional: eliminar ambos jugadores de la sala de espera
            db.reference(f"/gameWaitRoom/{first_player_id}").delete()
            db.reference(f"/gameWaitRoom/{second_player_id}").delete()
            print(f"[INFO] Jugadores {first_player_id} y {second_player_id} eliminados de gameWaitRoom")
        else:
            print(f"[INFO] No se realizó un match para el jugador {user_id} en esta transacción.")
    except Exception as e:
        print(f"[ERROR] Error en join_players (usuario {user_id}): {e}")



@database_fn.on_value_created(reference="/games/{gameId}/playersInfo/{userId}/gameBoardLoaded")
def check_players_ready(event: database_fn.Event) -> None:
    game_id = event.params["gameId"]
    user_id = event.params["userId"]

    try:
        game_ref = db.reference(f"/games/{game_id}")

        def check_if_ready(transaction):
            game_data = transaction.get(game_ref)

            if game_data is None:
                print(f"Error: No se encontró la partida con ID: {game_id}")
                return

            players_info = game_data.get("playersInfo")
            if players_info is None:
                print(f"Error: No playersInfo in game {game_id}")
                return

            all_ready = True
            for player_id, player_data in players_info.items():
                if not player_data.get("gameBoardLoaded"):
                    all_ready = False
                    break

            if all_ready:
                transaction.update(game_ref, {"status": 3})
                print(f"Partida {game_id}: ¡Todos los jugadores listos!")

        db.reference("/").transaction(check_if_ready)

    except Exception as e:
        print(f"Error en check_players_ready (partida {game_id}, jugador {user_id}): {e}")