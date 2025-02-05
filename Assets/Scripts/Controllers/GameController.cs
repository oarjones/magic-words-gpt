using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private GameConfig gameConfig;
    private BoardConfig boardConfig;
    private GameView gameView;
    private IGameMode gameMode;
    private GameModel gameModel;
    private IBackendService backendService;
    private IDictionaryService dictionaryService;
    private InputManager inputManager;
    private MatchmakingController matchmakingController;
    private BoardGenerator boardGenerator;
    private BoardController boardController;
    

    public void Initialize(IBackendService backendService, IDictionaryService dictionaryService, BoardGenerator boardGenerator, 
        InputManager inputManager, MatchmakingController matchmakingController, GameConfig gameConfig, BoardConfig boardConfig, 
        GameModel gameModel, BoardController boardController, GameObject gameView)
    {
        this.backendService = backendService;
        this.dictionaryService = dictionaryService;
        this.boardGenerator = boardGenerator;
        this.inputManager = inputManager;
        this.matchmakingController = matchmakingController;
        this.gameConfig = gameConfig;
        this.boardConfig = boardConfig;
        this.gameModel = gameModel;
        this.boardController = boardController;
        this.gameView = gameView.GetComponent<GameView>();

        // Initialize the board through BoardController
        boardController.Initialize(inputManager, boardGenerator, gameModel);
        boardController.InitializeBoard();
        this.gameView.InitializeBoard(gameModel.board); // Now GameModel is available for the View
        this.gameView.UpdateTimer(gameModel.remainingTime);

        StartCoroutine(InitializeGame());
    }
    private IEnumerator InitializeGame()
    {
        // Set the game state to Playing after initialization
        gameModel.gameState = GameState.Playing;

        // Start listening for game updates if in PvP mode
        if (gameConfig.selectedGameMode == GameMode.PvP)
        {
            backendService.ListenForGameUpdates(gameModel.gameSessionId, OnGameUpdated, OnError);
        }

        yield return null;
    }

    private IGameMode CreateGameMode(GameMode gameMode, IBackendService backendService)
    {
        string playerId = string.Empty;

        switch (gameMode)
        {
            case GameMode.PvP:
                // Assuming you have a way to get the opponent ID, e.g., from matchmaking
                playerId = GetPlayerId(); // Implement this to get the current player's ID
                string opponentId = GetOpponentId(); // Implement this to get the opponent's ID
                return new PvPGameMode(playerId, opponentId, backendService);
            case GameMode.PvE:
                playerId = GetPlayerId();
                return new PvAlgorithmGameMode(playerId);
            default:
                throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
        }
    }

    private string GetPlayerId()
    {
        // Return a unique identifier for the player
        // This could be a Firebase User ID, PlayerPrefs value, or any other unique identifier
        return SystemInfo.deviceUniqueIdentifier; // Example using the device's unique identifier
    }

    private string GetOpponentId()
    {
        // If you have a matchmaking system, this is where you would retrieve the opponent's ID
        // For this example, we'll return a placeholder
        return "opponent-placeholder-id";
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

        //if (dictionaryService.IsValidWord(player.currentWord, languageCode))
        //{
        //    // Update player score
        //    player.score += player.currentWord.Length;

        //    // Check if the player has reached a goal cell
        //    if (player.currentCell.isObjective)
        //    {
        //        gameModel.gameState = GameState.Ended;
        //        gameView.ShowGameEndScreen(player.id); // Show the end game screen with the current player as the winner
        //        return; // Exit the method early since the game has ended
        //    }

        //    // Reset the current word to the last selected cell's letter
        //    player.currentWord = player.currentCell.letter;

        //    // Update the player data in the backend
        //    backendService.UpdatePlayer(player, (success) =>
        //    {
        //        if (!success)
        //        {
        //            Debug.LogError("Failed to update player data.");
        //        }
        //    });

        //    // Update the view
        //    gameView.UpdateScore(playerId, player.score);
        //    gameView.UpdateWord(playerId, player.currentWord);
        //}
        //else
        //{
        //    // Reset the current word to the last selected cell's letter
        //    player.currentWord = player.currentCell.letter;

        //    // Update the view
        //    gameView.UpdateWord(playerId, player.currentWord);

        //    // Optionally, provide feedback that the word is invalid
        //    // This could be a visual indicator or a sound
        //}
    }

    public void HandleCellSelection(Cell selectedCell)
    {
        // Determine the current player based on some logic
        // In a non-turn-based system, we could just check which player is making the selection
        // Assuming we have a way to identify the current player (e.g., from input)
        Player currentPlayer = DetermineCurrentPlayer();

        // Check if the selected cell is adjacent to the current cell and not occupied
        //if (currentPlayer.currentCell != null &&
        //    gameModel.board.GetNeighboringCells(currentPlayer.currentCell).Contains(selectedCell) &&
        //    !selectedCell.isOccupied)
        //{
        //    // Update the player's current cell and word
        //    currentPlayer.currentCell = selectedCell;
        //    currentPlayer.currentWord += selectedCell.letter;

        //    // Mark the cell as occupied
        //    selectedCell.isOccupied = true;

        //    // Update the view
        //    gameView.UpdateWord(currentPlayer.id, currentPlayer.currentWord);

        //    // Optionally, update the player's position in the backend
        //    backendService.UpdatePlayer(currentPlayer, (success) =>
        //    {
        //        if (!success)
        //        {
        //            Debug.LogError("Failed to update player data.");
        //        }
        //    });
        //}
    }

    private Player DetermineCurrentPlayer()
    {
        // Simple logic to get the player corresponding to the input
        // This needs to be adapted to however you determine the current player
        return gameModel.player1; // Or gameModel.player2 based on input or other logic
    }
}