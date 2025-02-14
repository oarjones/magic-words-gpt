// PvPGameMode.cs
using Assets.Scripts.Data;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PvPGameMode : IGameMode
{
    private BoardGenerator boardGenerator;
    private BoardConfig boardConfig;
    private GameConfig gameConfig;
    private GameModel gameModel = default(GameModel);
    private DatabaseReference gamesRef;
    private bool isGameAddedSubscribed = false;

    /* Comprobación de conexión */
    private bool isConnected = true;
    private float disconnectGraceTime = 30.0f;  // Tiempo de cortesía en segundos
    private float disconnectTimer = 0f;
    private Coroutine connectionCheckerCoroutine;


    public void Initialize(GameConfig gameConfig, BoardConfig boardConfig, IBackendService backendService,
        IDictionaryService dictionaryService, BoardGenerator boardGenerator)
    {
        this.boardGenerator = boardGenerator;
        this.gameConfig = gameConfig;
        this.boardConfig = boardConfig;
    }


    

    /// <summary>
    /// Este método se suscribe a .info/connected y llama al callback con el estado actual.
    /// </summary>
    public void CheckConnection(Action<bool> callback)
    {
        try
        {
            DatabaseReference connectedRef = FirebaseInitializer.dbRef.GetReference(".info/connected");
            // Suscribirse al evento ValueChanged para monitorizar la conexión.
            connectedRef.ValueChanged += OnConnectionValueChanged;
            // Se llama al callback con el estado actual.
            callback?.Invoke(isConnected);
        }
        catch (Exception ex)
        {
            Debug.LogError("Excepción en CheckConnection: " + ex.Message);
            callback?.Invoke(false);
        }
    }

    /// <summary>
    /// Evento que se lanza al cambiar el estado de la conexión.
    /// </summary>
    private void OnConnectionValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("Firebase error en CheckConnection: " + args.DatabaseError.Message);
            return;
        }

        if (args.Snapshot != null && args.Snapshot.Value != null)
        {
            bool currentStatus = Convert.ToBoolean(args.Snapshot.Value);
            if (currentStatus)
            {
                isConnected = true;
                disconnectTimer = 0f;
            }
            else
            {
                isConnected = false;
            }
            Debug.Log("Estado de conexión (PvP): " + isConnected);
        }
    }

    /// <summary>
    /// Inicia una comprobación continua de la conexión durante la partida.
    /// Si se pierde la conexión durante el tiempo de cortesía, se invoca el callback onConnectionLost.
    /// </summary>
    public void StartContinuousConnectionCheck(MonoBehaviour host, Action onConnectionLost)
    {
        if (connectionCheckerCoroutine != null)
        {
            host.StopCoroutine(connectionCheckerCoroutine);
        }
        connectionCheckerCoroutine = host.StartCoroutine(ConnectionCheckerCoroutine(onConnectionLost));
    }


    

    private IEnumerator ConnectionCheckerCoroutine(Action onConnectionLost)
    {
        // Intervalo entre comprobaciones (en segundos)
        float checkInterval = 6f;

        while (true)
        {            

            if (!isConnected)
            {
                disconnectTimer += Time.deltaTime;
                if (disconnectTimer >= disconnectGraceTime)
                {
                    Debug.LogWarning("Conexión perdida por más de " + disconnectGraceTime + " segundos. Finalizando partida.");
                    onConnectionLost?.Invoke();
                    yield break;
                }
            }
            else
            {
                disconnectTimer = 0f;
            }

            // Espera el intervalo definido antes de realizar la comprobación
            yield return new WaitForSeconds(checkInterval);
        }
    }

    // Recuerda desuscribirte del evento OnConnectionValueChanged cuando finalice la partida.
    public void StopContinuousConnectionCheck()
    {
        DatabaseReference connectedRef = FirebaseInitializer.dbRef.GetReference(".info/connected");
        connectedRef.ValueChanged -= OnConnectionValueChanged;
    }


    /// <summary>
    /// Fase 1: Emparejamiento. Se registra al jugador en Firebase y se espera la aparición de un oponente.
    /// Cuando se detecta un juego en el que participa el jugador, se invoca el callback onOpponentFound.
    /// </summary>
    public void StartWaitingForOpponent(Action onOpponentFound)
    {
        try
        {
            // Validación de Firebase
            if (FirebaseInitializer.auth?.CurrentUser == null)
            {
                Debug.LogError("FirebaseInitializer.auth.CurrentUser es nulo. No se puede emparejar.");
                return;
            }
            // Registrar al jugador en la sala de espera
            PlayerWaitRoom playerWaitRoom = new PlayerWaitRoom()
            {
                createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                langCode = PlayerPrefs.GetString("LANG"),
                level = PlayerPrefs.GetInt("LEVEL"),
                userName = "", // Se puede modificar para incluir el username
                status = "waiting"
            };

            string jsonPlayerWaitRoom = JsonConvert.SerializeObject(playerWaitRoom);
            FirebaseInitializer.dbRef.RootReference.Child("gameWaitRoom")
                .Child(FirebaseInitializer.auth.CurrentUser.UserId)
                .SetRawJsonValueAsync(jsonPlayerWaitRoom)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.LogError($"Error al agregar a la sala de espera: {task.Exception}");
                        return;
                    }
                    // Suscribir a la detección de la creación de juegos
                    gamesRef = FirebaseInitializer.dbRef.GetReference("games");
                    if (!isGameAddedSubscribed)
                    {
                        gamesRef.ChildAdded += (sender, args) =>
                        {
                            if (args.DatabaseError != null)
                            {
                                Debug.LogError($"Firebase error en StartWaitingForOpponent: {args.DatabaseError.Message}");
                                return;
                            }
                            // Se verifica que el juego incluye al jugador actual
                            var jsonGame = args.Snapshot.GetRawJsonValue();
                            if (string.IsNullOrEmpty(jsonGame))
                                return;

                            GameData gameData = JsonConvert.DeserializeObject<GameData>(jsonGame);
                            if (gameData != null && gameData.playersInfo != null &&
                                gameData.playersInfo.ContainsKey(FirebaseInitializer.auth.CurrentUser.UserId))
                            {
                                // Se crea el GameModel a partir del snapshot
                                gameModel = new GameModel
                                {
                                    gameId = args.Snapshot.Key,
                                    data = gameData
                                };
                                // Se invoca el callback para notificar que se ha emparejado
                                onOpponentFound?.Invoke();
                            }
                        };
                        isGameAddedSubscribed = true;
                    }
                });
        }
        catch (Exception ex)
        {
            Debug.LogError($"Excepción en StartWaitingForOpponent: {ex.Message}");
        }
    }

    /// <summary>
    /// Fase 2: Generación del tablero.
    /// Si el jugador es MASTER, se genera el tablero y se sube a Firebase.
    /// Se invoca el callback onBoardGenerated con el GameModel actualizado.
    /// </summary>
    public void GenerateBoard(Action<GameModel> onBoardGenerated)
    {
        try
        {
            // Guardar configuraciones locales
            // Se asume que gameConfig, boardConfig y boardGenerator ya han sido asignados externamente
            if (gameConfig == null || boardConfig == null)
            {
                Debug.LogError("gameConfig o boardConfig son nulos en GenerateBoard.");
                return;
            }

            // Se determina si el jugador es MASTER
            GamePlayerData playerInfo = gameModel.data.playersInfo[FirebaseInitializer.auth.CurrentUser.UserId];
            if (playerInfo.master)
            {
                // Generar el tablero y actualizar el GameModel
                Board board = boardGenerator.GenerateBoard(boardConfig.mapSize, GameMode.PvP);
                gameModel.data.gameBoard = board;

                // Se pueden invocar métodos stub para asignar la celda y palabra inicial
                AssignInitialCellToPlayer(FirebaseInitializer.auth.CurrentUser.UserId, board);
                AssignInitialWordToPlayer(FirebaseInitializer.auth.CurrentUser.UserId, board);

                // Subir el tablero a Firebase
                string jsonGameBoard = JsonConvert.SerializeObject(board);
                FirebaseInitializer.dbRef.GetReference($"games/{gameModel.gameId}/gameBoard")
                    .SetRawJsonValueAsync(jsonGameBoard)
                    .ContinueWithOnMainThread(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Debug.LogError($"Error al guardar el tablero: {task.Exception}");
                            return;
                        }
                        // Se desuscribe el ChildAdded para evitar redundancias
                        if (isGameAddedSubscribed)
                        {
                            gamesRef.ChildAdded -= null;
                            isGameAddedSubscribed = false;
                        }
                        // Invocar callback indicando que el tablero se generó correctamente
                        onBoardGenerated?.Invoke(gameModel);
                    });
            }
            else
            {
                // Si no es MASTER, se notifica inmediatamente (o se puede esperar a la siguiente fase)
                onBoardGenerated?.Invoke(gameModel);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Excepción en GenerateBoard: {ex.Message}");
        }
    }

    /// <summary>
    /// Fase 3: Carga y sincronización del tablero.
    /// Se suscribe a los eventos de Firebase para detectar cuando el tablero se ha cargado para el jugador SLAVE
    /// o cuando el MASTER detecta que el oponente ha cargado el tablero.
    /// El callback onBoardLoaded se invoca con el GameModel actualizado.
    /// </summary>
    public void WaitForBoardLoad(Action<GameModel> onBoardLoaded)
    {
        try
        {
            // Se obtiene la referencia al tablero en Firebase
            var boardRef = FirebaseInitializer.dbRef.GetReference($"games/{gameModel.gameId}/gameBoard");

            // Se decide el comportamiento según si el jugador es MASTER o SLAVE
            GamePlayerData playerInfo = gameModel.data.playersInfo[FirebaseInitializer.auth.CurrentUser.UserId];
            if (playerInfo.master)
            {
                // Para el MASTER, se espera a que el oponente cargue el tablero y cambie el estado del juego
                boardRef.ValueChanged += (sender, args) =>
                {
                    if (args.DatabaseError != null)
                    {
                        Debug.LogError($"Firebase error en WaitForBoardLoad (MASTER): {args.DatabaseError.Message}");
                        return;
                    }
                    var jsonGameBoard = args.Snapshot.GetRawJsonValue();
                    if (string.IsNullOrEmpty(jsonGameBoard))
                        return;

                    // Se lee y se actualiza el tablero en el GameModel
                    Board board = JsonConvert.DeserializeObject<Board>(jsonGameBoard);
                    var gameId = args.Snapshot.Reference.Parent.Key;
                    if (board != null && gameModel.gameId == gameId)
                    {
                        // Aquí se podría validar que el oponente ha cargado el tablero.
                        // Por ejemplo, verificando el estado del juego en Firebase.
                        FirebaseInitializer.dbRef.GetReference($"games/{gameModel.gameId}")
                            .GetValueAsync().ContinueWithOnMainThread(task =>
                            {
                                if (task.IsFaulted)
                                {
                                    Debug.LogError($"Error al obtener el juego: {task.Exception}");
                                    return;
                                }
                                var gameJson = task.Result.GetRawJsonValue();
                                GameModel updatedGame = JsonConvert.DeserializeObject<GameModel>(gameJson);
                                if (updatedGame.data.status == GameStatus.BoardLoaded)
                                {
                                    // Se desuscribe el evento para evitar invocaciones múltiples
                                    boardRef.ValueChanged -= null;
                                    onBoardLoaded?.Invoke(updatedGame);
                                }
                            });
                    }
                };
            }
            else
            {
                // Para el SLAVE, se espera a que el tablero se agregue y se sincronice
                boardRef.ValueChanged += (sender, args) =>
                {
                    if (args.DatabaseError != null)
                    {
                        Debug.LogError($"Firebase error en WaitForBoardLoad (SLAVE): {args.DatabaseError.Message}");
                        return;
                    }
                    var jsonGameBoard = args.Snapshot.GetRawJsonValue();
                    if (string.IsNullOrEmpty(jsonGameBoard))
                        return;

                    Board board = JsonConvert.DeserializeObject<Board>(jsonGameBoard);
                    var gameId = args.Snapshot.Reference.Parent.Key;
                    if (board != null && gameModel.gameId == gameId)
                    {
                        gameModel.data.gameBoard = board;
                        FirebaseInitializer.dbRef.GetReference($"games/{gameModel.gameId}/status")
                            .SetValueAsync((int)GameStatus.BoardLoaded)
                            .ContinueWithOnMainThread(task =>
                            {
                                if (task.IsFaulted)
                                {
                                    Debug.LogError($"Error al actualizar el status: {task.Exception}");
                                    return;
                                }
                                boardRef.ValueChanged -= null;
                                onBoardLoaded?.Invoke(gameModel);
                            });
                    }
                };
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Excepción en WaitForBoardLoad: {ex.Message}");
        }
    }

    // Stubs para asignar celda y palabra inicial (sin implementación actual)
    private void AssignInitialCellToPlayer(string playerId, Board board)
    {
        Debug.Log($"Asignar celda inicial para el jugador {playerId} (stub).");
    }
    private void AssignInitialWordToPlayer(string playerId, Board board)
    {
        Debug.Log($"Asignar palabra inicial para el jugador {playerId} (stub).");
    }

    // Métodos adicionales para inyección de dependencias
    public void SetBoardGenerator(BoardGenerator generator)
    {
        this.boardGenerator = generator;
    }

    public void SetConfigurations(GameConfig gameConfig, BoardConfig boardConfig)
    {
        this.gameConfig = gameConfig;
        this.boardConfig = boardConfig;
    }
}
