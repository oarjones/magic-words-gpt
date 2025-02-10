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
    //private FirebaseDatabase databaseRef;
    //private string currentGameId;
    //private bool isMasterPlayer;
    //private IBackendService backendService;
    private BoardGenerator boardGenerator;
    public User userData = null;
    private System.Action<GameModel> onGameStarted;
    private BoardConfig boardConfig;
    private GameConfig gameConfig;
    private GameModel gameModel = default(GameModel);
    

    public void InitializeGame(GameConfig gameConfig, BoardConfig boardConfig, IBackendService backendService,
        IDictionaryService dictionaryService, BoardGenerator boardGenerator, System.Action<GameModel> onGameStarted)
    {
        this.gameConfig = gameConfig;
        this.boardConfig = boardConfig;
        this.onGameStarted = onGameStarted;

        addToGameWaitRoom(FirebaseInitializer.auth.CurrentUser);
    }

    public void addToGameWaitRoom(Firebase.Auth.FirebaseUser user)
    {
        try
        {
            if (FirebaseInitializer.dbRef != null)
            {
                PlayerWaitRoom playerWaitRoom = new PlayerWaitRoom()
                {
                    createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    langCode = PlayerPrefs.GetString("LANG"),
                    level = PlayerPrefs.GetInt("LEVEL"),
                    userName = FirebaseAuth.DefaultInstance.CurrentUser.DisplayName
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
                gameModel = new GameModel();
                gameModel.gameId = args.Snapshot.Key;
                gameModel.data = gameData;

                GamePlayerData playerInfo = gameData.playersInfo.Where(d => d.Key == FirebaseInitializer.auth.CurrentUser.UserId).FirstOrDefault().Value;
                GamePlayerData opponentInfo = gameData.playersInfo.Where(d => d.Key != FirebaseInitializer.auth.CurrentUser.UserId).FirstOrDefault().Value;

                //Si es el jugador MASTER, se encargará de generar y guardar el tablero
                if (playerInfo != null && playerInfo.master)
                {

                    Board board = CreateBoard(boardConfig);
                    gameModel.data.gameBoard = board;

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


                    string jsonGameBoard = Newtonsoft.Json.JsonConvert.SerializeObject(board);
                    FirebaseInitializer.dbRef.GetReference($"games/{gameModel.gameId}/gameBoard").SetRawJsonValueAsync(jsonGameBoard).ContinueWithOnMainThread(task =>
                    {

                        var gamesRef = FirebaseInitializer.dbRef.GetReference("games");
                        gamesRef.ChildAdded -= HandleGameAdded;

                        //A la espera de que el jugador esclavo cargue el tablero
                        gamesRef = FirebaseInitializer.dbRef.GetReference($"games/{gameModel.gameId}/gameBoard");
                        gamesRef.ValueChanged += HandleOpnnentLoadGameboard;
                    });
                }
                //Si es el player SLAVE, esperará a que se genere el tablero 
                else
                {
                    var gamesRef = FirebaseInitializer.dbRef.GetReference($"games/{gameModel.gameId}/gameBoard");
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
            var gameBoard = Newtonsoft.Json.JsonConvert.DeserializeObject<Board>(jsonGameBoard);
            var gameId = args.Snapshot.Reference.Parent.Key;
            if (gameBoard != null && gameModel.gameId == gameId)
            {

                //Cambiamos el status a GameBoardCompleted
                FirebaseInitializer.dbRef.GetReference($"games/{gameModel.gameId}").GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    var gameJson = task.Result.GetRawJsonValue();
                    var game = JsonConvert.DeserializeObject<GameModel>(gameJson);
                    if (game.data.status == GameStatus.GameBoardCompleted)
                    {
                        //Eliminamos el trigger
                        var gamesRef = FirebaseInitializer.dbRef.GetReference($"games/{gameModel.gameId}/gameBoard");
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
            var gameBoard = Newtonsoft.Json.JsonConvert.DeserializeObject<Board>(jsonGameBoard);

            var gameId = args.Snapshot.Reference.Parent.Key;

            if (gameBoard != null && gameModel.gameId == gameId)
            {
                gameModel.data.gameBoard = gameBoard;

                //Cambiamos el status a GameBoardCompleted
                FirebaseInitializer.dbRef.GetReference($"games/{gameModel.gameId}/status").SetValueAsync((int)GameStatus.GameBoardCompleted).ContinueWithOnMainThread(task =>
                {
                    //Eliminamos el trigger
                    var gamesRef = FirebaseInitializer.dbRef.GetReference($"games/{gameModel.gameId}/gameBoard");
                    gamesRef.ValueChanged -= HandleGameBoardAdded;

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