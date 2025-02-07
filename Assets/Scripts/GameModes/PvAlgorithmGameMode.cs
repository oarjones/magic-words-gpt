using Assets.Scripts.Models;
using System.Diagnostics.CodeAnalysis;

public class PvAlgorithmGameMode : IGameMode
{
    private string playerId;

    public PvAlgorithmGameMode(string playerId)
    {
        this.playerId = playerId;
    }

    public GameModel CreateGame(GameConfig gameConfig, BoardConfig boardConfig, IBackendService backendService, IDictionaryService dictionaryService)
    {
        // Create a new board with the specified configuration
        Board board = CreateBoard(boardConfig, gameConfig);

        // Initialize player
        Player player1 = new Player(playerId, "Player 1", GetInitialCellForPlayer(board, boardConfig, true));

        // Initialize algorithm as a player
        Player player2 = new Player("algorithm", "Algorithm", GetInitialCellForPlayer(board, boardConfig, false));

        // Create and return a new GameModel
        return new GameModel(board, player1, player2, gameConfig);
    }

    private Board CreateBoard(BoardConfig boardConfig, GameConfig gameConfig)
    {
        return new BoardGenerator().GenerateBoard(boardConfig.mapSize, gameConfig.selectedGameMode);
    }

    private CellView GetInitialCellForPlayer(Board board, BoardConfig boardConfig, bool isPlayer1)
    {
        //TODO: implement logic to get the initial cell for the player
        return null;
    }

    private string GetRandomLetter()
    {
        // Logic to return a random letter, you can use your own logic here
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return letters[UnityEngine.Random.Range(0, letters.Length)].ToString();
    }
}