using Assets.Scripts.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Firebase.Auth;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

public class PvAlgorithmGameMode : IGameMode
{
    private BoardGenerator boardGenerator;
    private FirebaseUser user;
    public void InitializeGame(GameConfig gameConfig, BoardConfig boardConfig, IBackendService backendService, 
        IDictionaryService dictionaryService, BoardGenerator boardGenerator, System.Action<GameModel> onGameStarted)
    {

        user = FirebaseAuth.DefaultInstance.CurrentUser;

        this.boardGenerator = boardGenerator;

        // Create a new board with the specified configuration
        Board board = CreateBoard(boardConfig);
        

        // Create and return a new GameModel
        var gameModel = new GameModel();
        gameModel.gameId = Guid.NewGuid().ToString();
        gameModel.data = new GameData();
        gameModel.data.gameBoard = board;
        gameModel.data.playersInfo = new System.Collections.Generic.Dictionary<string, GamePlayerData>
        {
            { user.UserId, new GamePlayerData { userName = user.DisplayName, level = 1, master = true, gameBoardLoaded = true } }
        };

        gameModel.data.playersInfo = new System.Collections.Generic.Dictionary<string, GamePlayerData>
        {
            { Guid.NewGuid().ToString(), new GamePlayerData { userName = "Superalgorithm", level = 1, master = false, gameBoardLoaded = true } }
        };
        gameModel.data.type = GameType.CatchLetter;
        gameModel.data.status = GameStatus.GameBoardCompleted;
        gameModel.data.langCode = "es";
        gameModel.data.createdAt = DateTime.Now.Ticks;
        


        onGameStarted?.Invoke(gameModel); // Notifica que el juego está listo
        
    }

    private Board CreateBoard(BoardConfig boardConfig)
    {
        return boardGenerator.GenerateBoard(boardConfig.mapSize, GameMode.PvA);
    }

    private CellView GetInitialCellForPlayer(Board board, BoardConfig boardConfig, bool isPlayer1)
    {
        //TODO: implement logic to get the initial cell for the player
        return board.cells.First();
    }

    
}