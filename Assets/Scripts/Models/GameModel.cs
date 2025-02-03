// GameModel.cs
[System.Serializable]
public class GameModel
{
    public GameState gameState;
    public Board board;
    public Player player1;
    public Player player2;
    public string currentPlayerTurnId; // Might be useful for turn management, even without strict turns
    public float remainingTime;
    public string gameSessionId; // Add a unique identifier for the game session

    public GameModel(Board board, Player player1, Player player2, GameConfig gameConfig)
    {
        this.gameState = GameState.Loading;
        this.board = board;
        this.player1 = player1;
        this.player2 = player2;
        this.currentPlayerTurnId = player1.id; // Or randomly choose who starts
        this.remainingTime = gameConfig.gameDuration;
        this.gameSessionId = System.Guid.NewGuid().ToString();
    }
}