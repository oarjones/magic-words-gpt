// GameController.cs
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private BoardConfig boardConfig;
    [SerializeField] private GameView gameView;

    private IGameMode gameMode;
    private GameModel gameModel;
    private IBackendService backendService;
    private IDictionaryService dictionaryService;
    private InputManager inputManager;
    private MatchmakingController matchmakingController;

    public void Initialize(IBackendService backendService, IDictionaryService dictionaryService, IGameMode gameMode, InputManager inputManager, MatchmakingController matchmakingController)
    {
        this.backendService = backendService;
        this.dictionaryService = dictionaryService;
        this.gameMode = gameMode;
        this.inputManager = inputManager;
        this.matchmakingController = matchmakingController;

        // Example of initializing a new game
        StartCoroutine(InitializeGame());
    }
    private IEnumerator InitializeGame()
    {
        gameModel = gameMode.CreateGame(gameConfig, boardConfig, backendService, dictionaryService);
        gameView.InitializeBoard(gameModel.board); // Inicializa el tablero a través de la vista
        gameView.UpdateTimer(gameModel.remainingTime);

        // Set the game state to Playing after initialization
        gameModel.gameState = GameState.Playing;

        // Start listening for game updates if in PvP mode
        if (gameConfig.selectedGameMode == GameMode.PvP)
        {
            backendService.ListenForGameUpdates(gameModel.gameSessionId, OnGameUpdated, OnError);
        }

        yield return null;
    }

    private void OnGameUpdated(GameModel updatedGameModel)
    {
        // Update the local game model with the data received from Firebase
        gameModel = updatedGameModel;

        // Update the view based on the new game state
        gameView.UpdateScore(gameModel.player1.id, gameModel.player1.score);
        gameView.UpdateScore(gameModel.player2.id, gameModel.player2.score);
        gameView.UpdateWord(gameModel.player1.id, gameModel.player1.currentWord);
        gameView.UpdateWord(gameModel.player2.id, gameModel.player2.currentWord);
        gameView.UpdateTimer(gameModel.remainingTime);

        // Check if the game has ended
        if (gameModel.gameState == GameState.Ended)
        {
            gameView.ShowGameEndScreen(gameModel.player1.score > gameModel.player2.score ? gameModel.player1.id : gameModel.player2.id);
        }
    }

    private void OnError(string errorMessage)
    {
        // Handle errors, such as failing to listen for updates
        Debug.LogError("Error listening for game updates: " + errorMessage);
    }

    private void Update()
    {
        if (gameModel.gameState == GameState.Playing)
        {
            gameModel.remainingTime -= Time.deltaTime;
            gameView.UpdateTimer(gameModel.remainingTime);

            if (gameModel.remainingTime <= 0f)
            {
                gameModel.gameState = GameState.Ended;
                // Determine the winner based on score
                string winnerId = gameModel.player1.score > gameModel.player2.score ? gameModel.player1.id : gameModel.player2.id;
                if (gameModel.player1.score == gameModel.player2.score)
                {
                    winnerId = null; // It's a tie
                }
                gameView.ShowGameEndScreen(winnerId);
            }
        }
    }

    public void ValidateWord(string playerId)
    {
        Player player = playerId == gameModel.player1.id ? gameModel.player1 : gameModel.player2;
        string languageCode = "es-ES"; // Example language code, should be configurable

        if (dictionaryService.IsValidWord(player.currentWord, languageCode))
        {
            // Update player score
            player.score += player.currentWord.Length;

            // Check if the player has reached a goal cell
            if (player.currentCell.isObjective)
            {
                gameModel.gameState = GameState.Ended;
                gameView.ShowGameEndScreen(player.id); // Show the end game screen with the current player as the winner
                return; // Exit the method early since the game has ended
            }

            // Reset the current word to the last selected cell's letter
            player.currentWord = player.currentCell.letter;

            // Update the player data in the backend
            backendService.UpdatePlayer(player, (success) =>
            {
                if (!success)
                {
                    Debug.LogError("Failed to update player data.");
                }
            });

            // Update the view
            gameView.UpdateScore(playerId, player.score);
            gameView.UpdateWord(playerId, player.currentWord);
        }
        else
        {
            // Reset the current word to the last selected cell's letter
            player.currentWord = player.currentCell.letter;

            // Update the view
            gameView.UpdateWord(playerId, player.currentWord);

            // Optionally, provide feedback that the word is invalid
            // This could be a visual indicator or a sound
        }
    }

    public void HandleCellSelection(Cell selectedCell)
    {
        // Determine the current player based on some logic
        // In a non-turn-based system, we could just check which player is making the selection
        // Assuming we have a way to identify the current player (e.g., from input)
        Player currentPlayer = DetermineCurrentPlayer();

        // Check if the selected cell is adjacent to the current cell and not occupied
        if (currentPlayer.currentCell != null &&
            gameModel.board.GetNeighboringCells(currentPlayer.currentCell).Contains(selectedCell) &&
            !selectedCell.isOccupied)
        {
            // Update the player's current cell and word
            currentPlayer.currentCell = selectedCell;
            currentPlayer.currentWord += selectedCell.letter;

            // Mark the cell as occupied
            selectedCell.isOccupied = true;

            // Update the view
            gameView.UpdateWord(currentPlayer.id, currentPlayer.currentWord);

            // Optionally, update the player's position in the backend
            backendService.UpdatePlayer(currentPlayer, (success) =>
            {
                if (!success)
                {
                    Debug.LogError("Failed to update player data.");
                }
            });
        }
    }

    private Player DetermineCurrentPlayer()
    {
        // Simple logic to get the player corresponding to the input
        // This needs to be adapted to however you determine the current player
        return gameModel.player1; // Or gameModel.player2 based on input or other logic
    }

    //... Resto del GameController
}