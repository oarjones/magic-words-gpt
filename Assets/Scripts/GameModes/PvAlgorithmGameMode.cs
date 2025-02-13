using Assets.Scripts.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Firebase.Auth;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

public class PvAlgorithmGameMode : IGameMode
{
    private BoardGenerator boardGenerator;
    private BoardConfig boardConfig;
    private GameConfig gameConfig;
    private GameModel gameModel = new GameModel();


    public void Initialize(GameConfig gameConfig, BoardConfig boardConfig, IBackendService backendService,
        IDictionaryService dictionaryService, BoardGenerator boardGenerator)
    {
        this.boardGenerator = boardGenerator;
        this.gameConfig = gameConfig;
        this.boardConfig = boardConfig;
    }

    public void GenerateBoard(Action<GameModel> onBoardGenerated)
    {
        // Create a new board with the specified configuration
        Board board = CreateBoard(boardConfig);

        
        gameModel.data.gameBoard = board;

        onBoardGenerated?.Invoke(gameModel);
    }

    public void StartWaitingForOpponent(Action onOpponentFound)
    {
        gameModel.data.playersInfo = new System.Collections.Generic.Dictionary<string, GamePlayerData>
        {
            { Guid.NewGuid().ToString(), new GamePlayerData { userName = PlayerPrefs.GetString("username") ?? "test user", level= PlayerPrefs.GetInt("level"), master = true, gameBoardLoaded = true } }
        };

        gameModel.data.playersInfo = new System.Collections.Generic.Dictionary<string, GamePlayerData>
        {
            { Guid.NewGuid().ToString(), new GamePlayerData { userName = "Superalgorithm", level = 1, master = false, gameBoardLoaded = true } }
        };

        onOpponentFound?.Invoke();
    }

    public void WaitForBoardLoad(Action<GameModel> onBoardLoaded)
    {
        gameModel.gameId = Guid.NewGuid().ToString();        
        gameModel.data.type = GameType.CatchLetter;
        gameModel.data.status = GameStatus.BoardLoaded;
        gameModel.data.langCode = "es";
        gameModel.data.createdAt = DateTime.Now.Ticks;

        onBoardLoaded?.Invoke(gameModel);
    }

    private Board CreateBoard(BoardConfig boardConfig)
    {
        return boardGenerator.GenerateBoard(boardConfig.mapSize, GameMode.PvA);
    }
        
}