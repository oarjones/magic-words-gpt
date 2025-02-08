using Assets.Scripts.Managers;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private BoardConfig boardConfig;
    [SerializeField] private DictionaryConfig dictionaryConfig;
    [SerializeField] private GameObject gameView;
    [SerializeField] private BoardGenerator boardGenerator;
    [SerializeField] private InputManager inputManager;

    private FirebaseController firebaseController;
    private DictionaryController dictionaryController;
    private GameController gameController;
    private BoardController boardController;
    private MatchmakingController matchmakingController;
    private GameMode mode = GameMode.PvA;



    private void OnEnable()
    {
        // Recupera el GameMode de los parámetros
        string gameModeString = PlayerPrefs.GetString("GameMode", "PvA"); // Valor por defecto: PvA
        mode = (GameMode)Enum.Parse(typeof(GameMode), gameModeString);

        // Si el modo de juego es PvP, se espera a que Firebase se inicialice antes de continuar
        if (mode == GameMode.PvP)
        {
            FirebaseInitializer firebaseInitializer = FindFirstObjectByType<FirebaseInitializer>();
            if (firebaseInitializer != null)
            {
                firebaseInitializer.OnFirebaseInitialized += FirebaseInitialized;
            }
        }
        else
        {
            FirebaseInitialized();
        }
    }

    private void OnDestroy()
    {
        if (mode == GameMode.PvP)
        {
            FirebaseInitializer firebaseInitializer = FindFirstObjectByType<FirebaseInitializer>();
            if (firebaseInitializer != null)
            {
                firebaseInitializer.OnFirebaseInitialized -= FirebaseInitialized;
            }
        }
    }

    private void FirebaseInitialized()
    {
        GetTestUser();

        // Se establece y guarda el modo de juego seleccionado
        GameModeManager.Instance.SetGameMode(mode);
        gameConfig.selectedGameMode = GameModeManager.Instance.SelectedGameMode;


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
        GameModel gameModel = gameMode.CreateGame(gameConfig, boardConfig, firebaseController, dictionaryController, boardGenerator);

        // Initialize controllers after creating GameModel
        gameController.Initialize(firebaseController, dictionaryController, inputManager, matchmakingController, gameConfig, boardConfig, gameModel, boardController, gameView);
    }

    private void GetTestUser()
    {
        Firebase.Auth.FirebaseAuth auth = FirebaseInitializer.auth;
        AuthFirebaseManager authFirebaseManager = FindFirstObjectByType<AuthFirebaseManager>();

#if UNITY_EDITOR
        if (auth.CurrentUser == null || auth.CurrentUser.Email != "oarjones@gmail.com")
        {
            if (auth.CurrentUser != null)
            {
                auth.SignOut();
            }

            authFirebaseManager.OnSignInWithEmailAndPasswordAsync("oarjones@gmail.com", "Am1lcarbarca");
        }
#else
            
        if (auth.CurrentUser == null || auth.CurrentUser.Email != "manuelp@gmail.com")
        {
            if (auth.CurrentUser != null)
            {
                auth.SignOut();
            }

            authFirebaseManager.OnSignInWithEmailAndPasswordAsync("manuelp@gmail.com", "Am1lcarbarca");
        }
#endif
    }

    private void InitializeGame()
    {
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
            case GameMode.PvA:
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