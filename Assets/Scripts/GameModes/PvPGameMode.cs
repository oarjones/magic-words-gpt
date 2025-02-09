// PvPGameMode.cs
using Assets.Scripts.Data;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using static Unity.VisualScripting.Icons;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PvPGameMode : IGameMode
{
    private FirebaseDatabase databaseRef;
    private string currentGameId;
    private bool isMasterPlayer;
    private IBackendService backendService;
    private BoardGenerator boardGenerator;
    //private FirebaseUser user;
    public User userData = null;
    public static Game GameData { get; set; }
    System.Action<GameModel> onGameStarted;
    BoardConfig boardConfig;
    GameConfig gameConfig;
    GameBoard gameBoardPvP = new GameBoard();
    GameModel gameModel = default(GameModel);

    public async void InitializeGame(GameConfig gameConfig, BoardConfig boardConfig, IBackendService backendService,
        IDictionaryService dictionaryService, BoardGenerator boardGenerator, System.Action<GameModel> onGameStarted)
    {
        this.gameConfig = gameConfig;
        this.boardConfig = boardConfig;
        this.onGameStarted = onGameStarted;
        GetUserData();
        addToGameWaitRoom(FirebaseInitializer.auth.CurrentUser);
    }


    private void GetUserData()
    {
        Firebase.Auth.FirebaseAuth auth = FirebaseInitializer.auth;

        if (auth.CurrentUser != null)
        {
            FirebaseInitializer.dbRef.RootReference.Child("users").Child(auth.CurrentUser.UserId).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    var currentUser = task.Result;

                    if (currentUser != null && currentUser.Value != null)
                    {
                        var currentUserJson = currentUser.GetRawJsonValue();
                        userData = JsonConvert.DeserializeObject<User>(currentUserJson);
                    }
                }
                else
                {
                    // Manejar error
                    Debug.LogError("Error al obtener datos de Firebase: " + task.Exception);
                }
            });

        }
    }

    public void addToGameWaitRoom(Firebase.Auth.FirebaseUser user)
    {
        try
        {
            if (FirebaseInitializer.dbRef != null)
            {
                PlayerWaitRoom playerWaitRoom = null;


                FirebaseInitializer.dbRef.GetReference($"users/{user.UserId}/username").GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted)
                    {
                        Debug.Log($"[addToGameWaitRoom] Error retrieving user data: {task.Exception.Message}");
                    }
                    else if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;

                        playerWaitRoom = new PlayerWaitRoom()
                        {
                            createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                            langCode = PlayerPrefs.GetString("LANG"),
                            level = PlayerPrefs.GetInt("LEVEL"),
                            userName = task.Result.Value.ToString()
                        };


                        //Se añade una nueva entrada
                        string jsonPlayerWaitRoom = Newtonsoft.Json.JsonConvert.SerializeObject(playerWaitRoom);
                        FirebaseInitializer.dbRef.RootReference.Child("gameWaitRoom").Child(user.UserId).SetRawJsonValueAsync(jsonPlayerWaitRoom).ContinueWithOnMainThread(task =>
                        {
                            //Cuando se una otro jugador se generará automaticamente una partida
                            var gamesRef = FirebaseInitializer.dbRef.GetReference("games");
                            gamesRef.ChildAdded += HandleGameAdded;

                        });

                    }
                });

            }
            else
            {
                throw new Exception("No se ha inicializado FirebaseDatabase dbRef!");
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

    }


    

    public void HandleGameAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        //var game = args.Snapshot;
        var jsonGame = args.Snapshot.GetRawJsonValue();

        try
        {
            GameData gameData = Newtonsoft.Json.JsonConvert.DeserializeObject<GameData>(jsonGame);

            //Si el juego me incluye como player...
            if (gameData != null && gameData.playersInfo.Where(d => d.Key == FirebaseInitializer.auth.CurrentUser.UserId).Any())
            {
                GameData = new Game();
                GameData.gameId = args.Snapshot.Key;
                GameData.data = gameData;

                GamePlayerData playerInfo = gameData.playersInfo.Where(d => d.Key == FirebaseInitializer.auth.CurrentUser.UserId).FirstOrDefault().Value;

                var otherPlayerId = GameData.data.playersInfo.Where(d => d.Key != FirebaseInitializer.auth.CurrentUser.UserId).FirstOrDefault().Key;

                //Si es el jugador MASTER, se encargará de generar y guardar el tablero
                if (playerInfo != null && playerInfo.master)
                {

                    Board board = CreateBoard(boardConfig);

                    // Initialize player
                    Player player = new Player(FirebaseInitializer.auth.CurrentUser.UserId, FirebaseInitializer.auth.CurrentUser.DisplayName, null);

                    // Initialize algorithm as a player
                    Player opponent = new Player(otherPlayerId, "", null);

                    // Create and return a new GameModel
                    gameModel = new GameModel(board, player, opponent, gameConfig);

                    gameBoardPvP = new GameBoard();
                    gameBoardPvP.gameId = GameData.gameId;
                    gameBoardPvP.boardTiles = new List<BoardTile>();

                    foreach(var cell in board.cells.Select(c => c.cellModel))
                    {
                        gameBoardPvP.boardTiles.Add(new BoardTile()
                        {
                            posVector = new PosVector() { x = cell.Position.x, y = cell.Position.y, z = cell.Position.z },
                            name = cell.Name,
                            level = cell.Level,
                            tileNumber = cell.TileNumber,
                            x = cell.X,
                            y = cell.Y,
                            isObjectiveTile = false,
                            ownerId = cell.OwnerId,
                            letter = cell.CurrentLetter,
                            action = Assets.Scripts.Enums.TileAction.None,
                            tileState = Assets.Scripts.Enums.GameTileState.Unselected
                        });
                    }

                    ////TODO: Añadir palabras iniciales a los 2 jugadores
                    /*
                    var playerInitalWord = GetRandomWord();
                    var opponentInitalWord = GetRandomWord();

                    List<BoardTile> tiles = new List<BoardTile>();

                    short i = 0;
                    BoardTile currentTile = null;

                    //Player
                    foreach (char letter in playerInitalWord.ToUpper())
                    {
                        if (i == 0)
                        {
                            currentTile = gameBoard.boardTiles.Where(c => !string.IsNullOrEmpty(c.playerInitial) && c.playerInitial == FirebaseInitializer.auth.CurrentUser.UserId).FirstOrDefault(); //map.CurrentPlayerTile;
                        }
                        else
                        {
                            var neighborns = GetNeighbors(currentTile, gameBoard.boardTiles);

                            var cellCount = neighborns.Where(c => c != null && !tiles.Contains(c)).Count();
                            int randomCellIndex = UnityEngine.Random.Range(0, (cellCount - 1));

                            currentTile = neighborns.Where(c => c != null && !tiles.Contains(c)).ToList()[randomCellIndex];

                        }

                        currentTile.letter = DictionaryUtilities.RemoveDiacritics(letter.ToString()).Trim();
                        tiles.Add(currentTile);

                        i++;
                    }

                    //Opponent
                    i = 0;
                    foreach (char letter in opponentInitalWord.ToUpper())
                    {
                        if (i == 0)
                        {
                            currentTile = gameBoard.boardTiles.Where(c => !string.IsNullOrEmpty(c.playerInitial) && c.playerInitial != FirebaseInitializer.auth.CurrentUser.UserId).FirstOrDefault(); //map.CurrentPlayerTile;
                        }
                        else
                        {
                            var neighborns = GetNeighbors(currentTile, gameBoard.boardTiles);

                            var cellCount = neighborns.Where(c => c != null && !tiles.Contains(c)).Count();
                            int randomCellIndex = UnityEngine.Random.Range(0, (cellCount - 1));

                            currentTile = neighborns.Where(c => c != null && !tiles.Contains(c)).ToList()[randomCellIndex];
                        }

                        currentTile.letter = DictionaryUtilities.RemoveDiacritics(letter.ToString()).Trim();
                        tiles.Add(currentTile);

                        i++;
                    }
                    */


                    GameData.data.gameBoard = gameBoardPvP;

                    string jsonGameBoard = Newtonsoft.Json.JsonConvert.SerializeObject(gameBoardPvP);
                    FirebaseInitializer.dbRef.GetReference($"games/{GameData.gameId}/gameBoard").SetRawJsonValueAsync(jsonGameBoard).ContinueWithOnMainThread(task =>
                    {

                        var gamesRef = FirebaseInitializer.dbRef.GetReference("games");
                        gamesRef.ChildAdded -= HandleGameAdded;

                        //TODO:Debermnos quedar a la espera de que el jugador esclavo cargue el tablero
                        gamesRef = FirebaseInitializer.dbRef.GetReference($"games/{GameData.gameId}/gameBoard");
                        
                        gamesRef.ValueChanged += HandleOpnnentLoadGameboard;
                    });
                }
                //Si es el player SLAVE, esperará a que se genere el tablero 
                else
                {
                    var gamesRef = FirebaseInitializer.dbRef.GetReference($"games/{GameData.gameId}/gameBoard");
                    gamesRef.ValueChanged += HandleGameBoardAdded;
                }

            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw;
        }

    }


    void HandleOpnnentLoadGameboard(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        var jsonGameBoard = args.Snapshot.GetRawJsonValue();

        try
        {
            var gameBoard = Newtonsoft.Json.JsonConvert.DeserializeObject<GameBoard>(jsonGameBoard);
            var gameId = args.Snapshot.Reference.Parent.Key;
            if (gameBoard != null && GameData.gameId == gameId )
            {
                
                //Cambiamos el status a GameBoardCompleted
                FirebaseInitializer.dbRef.GetReference($"games/{GameData.gameId}").GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    var gameJson = task.Result.GetRawJsonValue();
                    var game = JsonConvert.DeserializeObject<Game>(gameJson);
                    if(game.data.status == GameStatus.GameBoardCompleted)
                    {
                        //Eliminamos el trigger
                        var gamesRef = FirebaseInitializer.dbRef.GetReference($"games/{GameData.gameId}/gameBoard");
                        gamesRef.ValueChanged -= HandleOpnnentLoadGameboard;

                        onGameStarted?.Invoke(gameModel); // Notifica que el juego está listo
                    }
                    
                });
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw;
        }
    }

    void HandleGameBoardAdded(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }


        var jsonGameBoard = args.Snapshot.GetRawJsonValue();

        try
        {
            var gameBoard = Newtonsoft.Json.JsonConvert.DeserializeObject<GameBoard>(jsonGameBoard);

            var gameId = args.Snapshot.Reference.Parent.Key;

            if (gameBoard != null && GameData.gameId == gameId)
            {
                GameData.data.gameBoard = gameBoard;
                gameBoardPvP = gameBoard;

                //Cambiamos el status a GameBoardCompleted
                FirebaseInitializer.dbRef.GetReference($"games/{GameData.gameId}/status").SetValueAsync((int)GameStatus.GameBoardCompleted).ContinueWithOnMainThread(task =>
                {
                    //Eliminamos el trigger
                    var gamesRef = FirebaseInitializer.dbRef.GetReference($"games/{GameData.gameId}/gameBoard");
                    gamesRef.ValueChanged -= HandleGameBoardAdded;

                    var otherPlayerId = GameData.data.playersInfo.Where(d => d.Key != FirebaseInitializer.auth.CurrentUser.UserId).FirstOrDefault().Key;

                    // Initialize player
                    Player player = new Player(FirebaseInitializer.auth.CurrentUser.UserId, FirebaseInitializer.auth.CurrentUser.DisplayName, null);

                    // Initialize algorithm as a player
                    Player opponent = new Player(otherPlayerId, "", null);

                    Board board = boardGenerator.GenerateBoard(gameBoardPvP, gameConfig.selectedGameMode);

                    // Create and return a new GameModel
                    gameModel = new GameModel(board, player, opponent, gameConfig);

                    onGameStarted?.Invoke(gameModel); // Notifica que el juego está listo

                });

            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw;
        }



    }


    private Board CreateBoard(BoardConfig boardConfig)
    {
        return boardGenerator.GenerateBoard(boardConfig.mapSize, GameMode.PvP);
    }


}