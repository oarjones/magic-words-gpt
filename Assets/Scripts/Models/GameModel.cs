// GameModel.cs
[System.Serializable]
public class GameModel
{
    public GameStatus gameState;
    public Board board;
    public Player player1;
    public Player player2;
    public string currentPlayerTurnId; // Might be useful for turn management, even without strict turns
    public float remainingTime;
    public string gameSessionId; // Add a unique identifier for the game session

    public GameModel(Board board, Player player, Player opponent, GameConfig gameConfig)
    {
        this.gameState = GameStatus.Pending;
        this.board = board;
        this.player1 = player;
        this.player2 = opponent;
        this.currentPlayerTurnId = player1.id; // Or randomly choose who starts
        this.remainingTime = gameConfig.gameDuration;
        this.gameSessionId = System.Guid.NewGuid().ToString();
    }
}