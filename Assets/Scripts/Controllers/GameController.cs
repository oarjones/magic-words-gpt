// GameController.cs
using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private GameConfig gameConfig;
    private BoardConfig boardConfig;
    private GameView gameView;
    private IGameMode gameMode;
    private IBackendService backendService;
    private IDictionaryService dictionaryService;
    private InputManager inputManager;
    private BoardGenerator boardGenerator;
    private BoardController boardController;

    // Estado extendido del juego
    private GameStatus currentState = GameStatus.WaitingForOpponent;

    // Control de suscripciones
    private bool isSubscribedToGameUpdates = false;

    // Evento interno para notificaciones de estado (placeholder para futuro EventBus)
    public event Action<GameStatus> OnStateChanged;

    // Modelo y su notificador
    public GameModel gameModel;
    public GameModelNotifier modelNotifier;


    public void Initialize(IBackendService backendService, IDictionaryService dictionaryService,
    InputManager inputManager, GameConfig gameConfig, BoardConfig boardConfig, BoardController boardController,
    GameView gameView, IGameMode gameMode)
    {
        // Asignaci�n de dependencias
        this.backendService = backendService;
        this.dictionaryService = dictionaryService;
        this.inputManager = inputManager;
        this.gameConfig = gameConfig;
        this.boardConfig = boardConfig;
        this.boardController = boardController;
        this.gameView = gameView.GetComponent<GameView>();
        this.gameMode = gameMode;


        // Inicializamos el notificador del modelo
        modelNotifier = new GameModelNotifier(gameModel);

        // La GameView se suscribe al notificador para actualizarse autom�ticamente
        gameView.Initialize(modelNotifier);

        // Supongamos que el GameController es un MonoBehaviour, se puede iniciar el chequeo continuo solo para PvP:
        if (gameConfig.selectedGameMode == GameMode.PvP)
        {
            gameMode.CheckConnection((connected) =>
            {
                if (!connected)
                {
                    Debug.LogError("No hay conexi�n al iniciar el modo PvP.");
                    // Podr�as optar por no continuar o notificar al usuario.
                }
                else
                {
                    Debug.Log("Conexi�n estable al iniciar el modo PvP.");
                }
            });

            // Inicia la comprobaci�n continua de conexi�n:
            ((PvPGameMode)gameMode).StartContinuousConnectionCheck(this, () =>
            {
                // Si se pierde la conexi�n durante el juego, finaliza la partida.
                EndGameDueToConnectionLoss();
            });
        }


        // Se inicia la cadena de estados:
        // 1. Emparejamiento
        gameMode.StartWaitingForOpponent(() =>
        {
            // Al emparejarse, se pasa a la generaci�n del tablero
            ChangeGameState(GameStatus.BoardGenerating);

            // 2. Generaci�n del tablero
            gameMode.GenerateBoard((generatedModel) =>
            {
                // Se actualiza el modelo y se pasa a la carga del tablero
                this.gameModel = generatedModel;
                ChangeGameState(GameStatus.BoardLoaded);

                // 3. Carga/sincronizaci�n del tablero
                gameMode.WaitForBoardLoad((loadedModel) =>
                {
                    this.gameModel = loadedModel;
                    // Una vez cargado, se contin�a con la inicializaci�n de la vista
                    ChangeGameState(GameStatus.GameSetup);
                });
            });
        });

        // Se inicia el flujo en estado WaitingForOpponent
        ChangeGameState(GameStatus.WaitingForOpponent);
    }


    private void EndGameDueToConnectionLoss()
    {
        Debug.Log("Finalizando partida debido a p�rdida de conexi�n.");
        // Aqu� puedes cambiar el estado, notificar al backend y llevar al usuario a la pantalla de Game Over.
        EndGame();
    }


    /// <summary>
    /// Cambia de estado y notifica el cambio a otros componentes.
    /// </summary>
    private void ChangeGameState(GameStatus newState)
    {
        currentState = newState;
        Debug.Log($"Cambio de estado a: {currentState}");
        OnStateChanged?.Invoke(currentState);

        // Llamada al m�todo correspondiente seg�n el estado
        switch (currentState)
        {
            case GameStatus.WaitingForOpponent:
                HandleWaitingForOpponent();
                break;
            case GameStatus.BoardGenerating:
                HandleBoardGenerating();
                break;
            case GameStatus.BoardLoaded:
                HandleBoardLoaded();
                break;
            case GameStatus.GameSetup:
                HandleGameSetup();
                break;
            case GameStatus.GameStart:
                HandleGameStart();
                break;
            case GameStatus.Playing:
                HandlePlaying();
                break;
            case GameStatus.GameOver:
                HandleGameOver();
                break;
            default:
                Debug.LogWarning("Estado desconocido");
                break;
        }
    }

    #region Estado: WaitingForOpponent
    private void HandleWaitingForOpponent()
    {
        Debug.Log("Estado: WaitingForOpponent");
        // En este estado se espera a que se empareje al jugador (o en PvA se configura el algoritmo).
        // Se podr�a invocar un LobbyManager o similar. Por simplicidad, asumimos que cuando se
        // asigna el oponente o se determina que el tablero debe generarse, se pasa al siguiente estado.
        // Por ejemplo, en PvP, el GameMode notificar� cuando se encuentre un oponente.
        // En este ejemplo, se simula la transici�n directa:
        ChangeGameState(GameStatus.BoardGenerating);
    }
    #endregion

    #region Estado: BoardGenerating
    private void HandleBoardGenerating()
    {
        Debug.Log("Estado: BoardGenerating");
        // En este estado se supone que el GameMode (o un BoardManager) genera el tablero.
        // Se llama a un m�todo para inicializar el tablero, que a su vez actualizar� el GameModel.
        try
        {
            // Inicializar el controlador del tablero y generar la vista
            boardController.Initialize(inputManager, gameModel);
            if (gameModel.data.gameBoard == null)
            {
                Debug.LogError("El tablero generado es nulo.");
                return;
            }
            // Notificar que el tablero ya est� generado y cargarlo en la vista
            ChangeGameState(GameStatus.BoardLoaded);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Excepci�n en HandleBoardGenerating: {ex.Message}");
        }
    }
    #endregion

    #region Estado: BoardLoaded
    private void HandleBoardLoaded()
    {
        Debug.Log("Estado: BoardLoaded");
        // Se verifica que ambos jugadores tienen el mismo tablero (sincronizaci�n).
        // Este ejemplo asume que el GameMode se ha encargado de sincronizar el GameModel.
        // Se actualiza la vista.
        try
        {
            if (gameModel.data.gameBoard == null)
            {
                Debug.LogError("El GameModel no tiene un tablero cargado.");
                return;
            }
            
            // Una vez que se ha cargado el tablero, se pasa a la inicializaci�n de la vista de juego.
            ChangeGameState(GameStatus.GameSetup);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Excepci�n en HandleBoardLoaded: {ex.Message}");
        }
    }
    #endregion

    #region Estado: GameSetup
    private void HandleGameSetup()
    {
        Debug.Log("Estado: GameSetup");
        try
        {
            Debug.Log("Estado: GameSetup");
            // Llamamos a la GameView para configurar la UI
            gameView.SetupUI();
            ChangeGameState(GameStatus.GameStart);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Excepci�n en HandleGameSetup: {ex.Message}");
        }
    }

    // Stub: Posiciona al jugador en la celda inicial.
    private void PositionPlayerAtInitialCell()
    {
        // TODO: Implementar la l�gica para determinar y posicionar la celda inicial del jugador.
        Debug.Log("Posicionando jugador en la celda inicial (stub).");
    }

    // Stub: Inicializa elementos de UI (score, iconos, botones, power-ups).
    private void InitializeUIElements()
    {
        // TODO: Implementar la configuraci�n de UI.
        Debug.Log("Inicializando elementos de UI (stub).");
    }
    #endregion

    #region Estado: GameStart
    private void HandleGameStart()
    {
        Debug.Log("Estado: GameStart");
        StartCoroutine(ReadyCountdown(3));
    }

    private IEnumerator ReadyCountdown(int seconds)
    {
        int count = seconds;
        while (count > 0)
        {
            Debug.Log($"El juego iniciar� en {count}...");
            yield return new WaitForSeconds(1);
            count--;
        }
        // Al finalizar la cuenta regresiva, se pasa al estado Playing.
        ChangeGameState(GameStatus.Playing);
    }
    #endregion

    #region Estado: Playing
    private void HandlePlaying()
    {
        Debug.Log("Estado: Playing");
        modelNotifier.UpdateStatus(GameStatus.Playing);
    }
    #endregion

    #region Estado: GameOver
    private void HandleGameOver()
    {
        Debug.Log("Estado: GameOver");
        // Se desactivan entradas, se muestran resultados y se finaliza la partida.
        // Se puede implementar una transici�n a una pantalla de resultados o un reinicio.
        StopGameUpdates();
        // Ejemplo: Notificar resultados y limpiar la sesi�n.
        Debug.Log("El juego ha finalizado. Mostrando resultados...");
    }
    #endregion

    #region Actualizaci�n y Suscripci�n de Juego
    /// <summary>
    /// Suscribe al backend para recibir actualizaciones en el GameModel.
    /// </summary>
    private void SubscribeToGameUpdates()
    {
        try
        {
            backendService.ListenForGameUpdates(gameModel.gameId, OnGameUpdated, OnError);
            isSubscribedToGameUpdates = true;
            Debug.Log("Suscripci�n a actualizaciones de juego establecida.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error al suscribirse a las actualizaciones del juego: {ex.Message}");
        }
    }

    private void StopGameUpdates()
    {
        // L�gica para cancelar la suscripci�n al backend, si es necesario.
        // Por ejemplo: backendService.UnsubscribeFromGameUpdates(gameModel.gameId);
        isSubscribedToGameUpdates = false;
    }

    /// <summary>
    /// Callback que actualiza el GameModel a partir de los datos recibidos.
    /// </summary>
    private void OnGameUpdated(GameModel updatedGameModel)
    {
        try
        {
            if (updatedGameModel == null)
            {
                Debug.LogError("El modelo de juego actualizado es nulo en OnGameUpdated.");
                return;
            }
            gameModel = updatedGameModel;
            UpdateViewWithGameModel();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Excepci�n en OnGameUpdated: {ex.Message}");
        }
    }

    /// <summary>
    /// Actualiza la vista con la informaci�n contenida en el GameModel.
    /// </summary>
    private void UpdateViewWithGameModel()
    {
        try
        {
            // Actualizar puntuaciones, temporizadores, etc.
            Debug.Log("Actualizaci�n de la vista con el GameModel.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Excepci�n en UpdateViewWithGameModel: {ex.Message}");
        }
    }

    /// <summary>
    /// Callback de error para el servicio de backend.
    /// </summary>
    private void OnError(string errorMessage)
    {
        Debug.LogError($"Error en la escucha de actualizaciones del juego: {errorMessage}");
    }
    #endregion

    #region Manejo de Input y Acciones
    /// <summary>
    /// Maneja la selecci�n de una celda por parte del jugador.
    /// </summary>
    public void HandleCellSelection(Cell selectedCell)
    {
        try
        {
            // Validar la selecci�n y actualizar el estado del juego.
            Debug.Log("Celda seleccionada procesada.");
            // Aqu� se podr�a llamar a un m�todo espec�fico que valide la adyacencia,
            // actualice el GameModel y notifique al backend.
        }
        catch (Exception ex)
        {
            Debug.LogError($"Excepci�n en HandleCellSelection: {ex.Message}");
        }
    }
    #endregion

    // M�todo p�blico para finalizar el juego (se puede llamar desde el backend o internamente).
    public void EndGame()
    {
        ChangeGameState(GameStatus.GameOver);
    }
}
