public class PvAlgorithmGameMode : IGameMode
{
    private string playerId;

    public PvAlgorithmGameMode(string playerId)
    {
        this.playerId = playerId;
    }

    public GameModel CreateGame(GameConfig gameConfig, BoardConfig boardConfig, IBackendService backendService, IDictionaryService dictionaryService)
    {
        // Initialize player
        Player player1 = new Player(playerId, "Player 1", GetInitialCellForPlayer(boardConfig, true));

        // Initialize algorithm as a player
        Player player2 = new Player("algorithm", "Algorithm", GetInitialCellForPlayer(boardConfig, false));

        // Create a new board with the specified configuration
        Board board = CreateBoard(boardConfig);

        // Create and return a new GameModel
        return new GameModel(board, player1, player2, gameConfig);
    }

    private Board CreateBoard(BoardConfig boardConfig)
    {
        // Generate a new board based on boardConfig
        Cell[,] cells = new Cell[boardConfig.boardWidth, boardConfig.boardHeight];
        for (int x = 0; x < boardConfig.boardWidth; x++)
        {
            for (int y = 0; y < boardConfig.boardHeight; y++)
            {
                // Example letter, replace with your logic to assign letters
                string letter = GetRandomLetter();
                cells[x, y] = new Cell(x, y, letter);
            }
        }
        return new Board(cells);
    }

    private Cell GetInitialCellForPlayer(BoardConfig boardConfig, bool isPlayer1)
    {
        // Determine initial cell based on player number and board configuration
        // This is a simple example, you might want to make it more sophisticated
        int x = isPlayer1 ? 0 : boardConfig.boardWidth - 1;
        int y = isPlayer1 ? 0 : boardConfig.boardHeight - 1;
        return new Cell(x, y, GetRandomLetter());
    }

    private string GetRandomLetter()
    {
        // Logic to return a random letter, you can use your own logic here
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return letters[UnityEngine.Random.Range(0, letters.Length)].ToString();
    }
}