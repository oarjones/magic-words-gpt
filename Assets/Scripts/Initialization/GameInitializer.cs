using Assets.Scripts.Managers;
using Firebase.Auth;
using System;
using System.ComponentModel.Design;
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

        // Initialize Game
        gameController = gameObject.AddComponent<GameController>();
        boardController = gameObject.AddComponent<BoardController>();

        // Create GameMode and GameModel before initializing controllers
        IGameMode gameMode = CreateGameMode(GameModeManager.Instance.SelectedGameMode);

        //Inicialización de IGameMode
        gameMode.Initialize(gameConfig, boardConfig, firebaseController, dictionaryController, boardGenerator);

        // Inicializar el GameController, quien se encargará de gestionar el flujo y llamar a IGameMode
        gameController.Initialize(firebaseController, dictionaryController, inputManager, gameConfig, boardConfig,
            boardController, gameView, gameMode);


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

    
    private IGameMode CreateGameMode(GameMode gameMode)
    {
        string playerId = string.Empty;

        switch (gameMode)
        {
            case GameMode.PvP:
                return new PvPGameMode();
            case GameMode.PvA:
                return new PvAlgorithmGameMode();
            default:
                throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
        }
    }


}