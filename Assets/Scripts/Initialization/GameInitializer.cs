using System;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private BoardConfig boardConfig;
    [SerializeField] private DictionaryConfig dictionaryConfig;
    [SerializeField] private GameObject mainMenuView;
    [SerializeField] private GameObject gameView;
    private FirebaseController firebaseController;
    private DictionaryController dictionaryController;
    private GameController gameController;
    private BoardController boardController;
    private MatchmakingController matchmakingController;
    private InputManager inputManager;
    private BoardGenerator boardGenerator;

    private void Awake()
    {
        // Initialize input manager
        inputManager = gameObject.AddComponent<InputManager>();

        // Initialize BoardGenerator
        boardGenerator = gameObject.AddComponent<BoardGenerator>();
        boardGenerator.boardConfig = boardConfig;

        mainMenuView.gameObject.SetActive(true);
        gameView.gameObject.SetActive(false);
    }

    public void StartPvPGame()
    {
        OnGameModeSelected(GameMode.PvP);
    }

    public void StartPvEGame()
    {
        OnGameModeSelected(GameMode.PvE);
    }

    private void OnGameModeSelected(GameMode mode)
    {
        GameModeManager.Instance.SetGameMode(mode); // Guarda el modo de juego seleccionado
        gameConfig.selectedGameMode = mode;
        mainMenuView.gameObject.SetActive(false);
        gameView.gameObject.SetActive(true);
        InitializeGame();
    }

    private void InitializeGame()
    {
        // Initialize Firebase
        firebaseController = gameObject.AddComponent<FirebaseController>();
        firebaseController.Initialize();

        // Initialize Dictionary
        dictionaryController = gameObject.AddComponent<DictionaryController>();
        dictionaryController.Initialize(dictionaryConfig);

        // Initialize Matchmaking
        matchmakingController = gameObject.AddComponent<MatchmakingController>();
        matchmakingController.Initialize(firebaseController, gameConfig);

        // Initialize Game
        gameController = gameObject.AddComponent<GameController>();
        boardController = gameObject.AddComponent<BoardController>();

        // Create GameMode and GameModel before initializing controllers
        IGameMode gameMode = CreateGameMode(GameModeManager.Instance.SelectedGameMode, firebaseController);
        GameModel gameModel = gameMode.CreateGame(gameConfig, boardConfig, firebaseController, dictionaryController);

        // Initialize controllers after creating GameModel
        gameController.Initialize(firebaseController, dictionaryController, boardGenerator, inputManager, matchmakingController, gameConfig, boardConfig, gameModel, boardController, gameView);
        boardController.Initialize(inputManager, boardGenerator, gameModel);

        // If PvP, start matchmaking
        if (GameModeManager.Instance.SelectedGameMode == GameMode.PvP)
        {
            matchmakingController.FindMatch();
        }
    }

    private IGameMode CreateGameMode(GameMode gameMode, IBackendService backendService)
    {
        string playerId = string.Empty;

        switch (gameMode)
        {
            case GameMode.PvP:
                // Assuming you have a way to get the opponent ID, e.g., from matchmaking
                playerId = GetPlayerId(); // Implement this to get the current player's ID
                string opponentId = GetOpponentId(); // Implement this to get the opponent's ID
                return new PvPGameMode(playerId, opponentId, backendService);
            case GameMode.PvE:
                playerId = GetPlayerId();
                return new PvAlgorithmGameMode(playerId);
            default:
                throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
        }
    }

    private string GetPlayerId()
    {
        // Return a unique identifier for the player
        // This could be a Firebase User ID, PlayerPrefs value, or any other unique identifier
        return SystemInfo.deviceUniqueIdentifier; // Example using the device's unique identifier
    }

    private string GetOpponentId()
    {
        // If you have a matchmaking system, this is where you would retrieve the opponent's ID
        // For this example, we'll return a placeholder
        return "opponent-placeholder-id";
    }
}