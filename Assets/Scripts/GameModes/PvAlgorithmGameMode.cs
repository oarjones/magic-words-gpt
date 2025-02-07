using Assets.Scripts.Models;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

public class PvAlgorithmGameMode : IGameMode
{
    private BoardGenerator boardGenerator;
    private string playerId;

    public PvAlgorithmGameMode(string playerId)
    {
        this.playerId = playerId;
    }

    public GameModel CreateGame(GameConfig gameConfig, BoardConfig boardConfig, IBackendService backendService, 
        IDictionaryService dictionaryService, BoardGenerator boardGenerator)
    {
        this.boardGenerator = boardGenerator;

        // Create a new board with the specified configuration
        Board board = CreateBoard(boardConfig);

        // Initialize player
        Player player1 = new Player(playerId, "Player 1", GetInitialCellForPlayer(board, boardConfig, true));

        // Initialize algorithm as a player
        Player player2 = new Player("algorithm", "Algorithm", GetInitialCellForPlayer(board, boardConfig, false));

        // Create and return a new GameModel
        return new GameModel(board, player1, player2, gameConfig);
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