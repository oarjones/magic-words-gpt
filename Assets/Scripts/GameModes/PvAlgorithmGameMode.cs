using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Firebase.Auth;
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

        // Initialize player
        Player player = new Player(user.UserId, user.DisplayName, GetInitialCellForPlayer(board, boardConfig, true));

        // Initialize algorithm as a player
        Player opponent = new Player("algorithm", "Algorithm", GetInitialCellForPlayer(board, boardConfig, false));

        // Create and return a new GameModel
        var gameModel = new GameModel(board, player, opponent, gameConfig);

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