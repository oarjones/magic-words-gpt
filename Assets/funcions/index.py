import firebase_functions_v1.database_fn as database_fn
import firebase_admin
from firebase_admin import credentials, db
import datetime
from firebase_functions_v1 import options

# Inicialización de Firebase (ajusta según sea necesario, ver comentarios anteriores)
if not firebase_admin._apps:
    firebase_admin.initialize_app()
options.set_global_options(region=options.SupportedRegion.EUROPE_WEST_1)

@database_fn.on_value_created(reference="/gameWaitRoom/{userId}")
def join_players(event: database_fn.DatabaseEvent) -> None:
    """
    Se activa cuando un jugador se une a la sala de espera.
    Intenta emparejar al jugador con otro jugador en espera.
    Si lo encuentra, crea una nueva partida.
    Usa transacciones para evitar condiciones de carrera.
    """
    user_id = event.params["userId"]
    first_player_data = event.data

    if first_player_data is None:
        print(f"No data for user {user_id} in gameWaitRoom.")
        return

    try:
        def try_to_match(transaction):
            wait_room_ref = db.reference("/gameWaitRoom")
            wait_room = transaction.get(wait_room_ref)
            second_player_id = None
            second_player_data = None

            if wait_room:
                for other_user_id, other_user_data in wait_room.items():
                    if (other_user_id != user_id and
                        other_user_data.get("langCode") == first_player_data.get("langCode")): #Comprobaciones
                        # TODO: Añadir comprobación de nivel (level) aquí si es necesario
                        second_player_id = other_user_id
                        second_player_data = other_user_data
                        break  # Sale del bucle for

            if second_player_id:
                # Crea la nueva partida
                new_game_ref = db.reference("/games").push()
                game_id = new_game_ref.key

                new_game = {
                    "status": 2,  # "Created" (según tu lógica original)
                    "type": 2,
                    "langCode": first_player_data["langCode"],
                    "createdAt": datetime.datetime.now().timestamp(),
                    "playersInfo": {
                        user_id: {
                            "userName": first_player_data["userName"],
                            "level": first_player_data["level"],
                            "master": True,
                            "gameBoardLoaded": False  # Inicialmente False
                        },
                        second_player_id: {
                            "userName": second_player_data["userName"],
                            "level": second_player_data["level"],
                            "master": False,
                            "gameBoardLoaded": False  # Inicialmente False
                        }
                    },
                    "gameBoard": {}  # Se rellena después por el jugador maestro
                }

                transaction.set(new_game_ref, new_game)  # Crea la partida
                transaction.delete(db.reference(f"/gameWaitRoom/{user_id}"))      # Se elimina a sí mismo
                transaction.delete(db.reference(f"/gameWaitRoom/{second_player_id}"))  # Elimina al oponente

                print(f"Partida creada: {game_id} ({user_id} vs {second_player_id})")

            else:
                print(f"No se encontró oponente para {user_id}")


        db.reference("/").transaction(try_to_match) #Transacción en la raíz

    except Exception as e:
        print(f"Error en join_players (usuario {user_id}): {e}")

@database_fn.on_value_created(reference="/games/{gameId}/playersInfo/{userId}/gameBoardLoaded")
def check_players_ready(event: database_fn.DatabaseEvent) -> None:
    """
    Se activa cuando un jugador indica que ha cargado el tablero.
    Comprueba si ambos jugadores están listos y, si es así, cambia el estado del juego a "playing" (3).
    """
    game_id = event.params["gameId"]
    user_id = event.params["userId"]

    try:
        game_ref = db.reference(f"/games/{game_id}")

        def check_if_ready(transaction):
            game_data = transaction.get(game_ref)

            if game_data is None:
                print(f"Error: No se encontró la partida con ID: {game_id}")
                return #No hace nada

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
                transaction.update(game_ref, {"status": 3})  # Cambia a "playing" (3)
                print(f"Partida {game_id}: ¡Todos los jugadores listos!")

        db.reference("/").transaction(check_if_ready) #Transacción en la raíz

    except Exception as e:
        print(f"Error en check_players_ready (partida {game_id}, jugador {user_id}): {e}")