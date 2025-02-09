using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

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
    private BoardGenerator boardGenerator;
    private BoardController boardController;

    private GameStatus currentState = GameStatus.Pending;

    public void Initialize(IBackendService backendService, IDictionaryService dictionaryService,
        InputManager inputManager, GameConfig gameConfig, BoardConfig boardConfig,
        GameModel gameModel, BoardController boardController, GameObject gameView, IGameMode gameMode)
    {
        this.backendService = backendService;
        this.dictionaryService = dictionaryService;
        this.inputManager = inputManager;
        this.gameConfig = gameConfig;
        this.boardConfig = boardConfig;
        this.gameModel = gameModel;
        this.boardController = boardController;
        this.gameView = gameView.GetComponent<GameView>();
        this.gameMode = gameMode;

        // Initialize the board through BoardController
        boardController.Initialize(inputManager, gameModel);
        this.gameView.InitializeBoard(gameModel.data.gameBoard); // Now GameModel is available for the View
        //this.gameView.UpdateTimer(gameModel.remainingTime);

        UpdateGameState(GameStatus.Loading);
    }
    private IEnumerator InitializeGame()
    {
        // Set the game state to Playing after initialization
        gameModel.data.status = GameStatus.Playing;

        // Start listening for game updates if in PvP mode
        if (gameConfig.selectedGameMode == GameMode.PvP)
        {
            backendService.ListenForGameUpdates(gameModel.gameId, OnGameUpdated, OnError);
        }

        yield return null;
    }



    public void UpdateGameState(GameStatus newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case GameStatus.Loading:
                HandleStartingState();
                break;

            case GameStatus.WaitingForPlayers:
                HandleWaitingForPlayersState();
                break;

            case GameStatus.Playing:
                HandlePlayingState();
                break;

            case GameStatus.GameOver:
                HandleGameOverState();
                break;
        }
    }


    private void HandleStartingState()
    {
        // Lógica de inicialización
        Debug.Log("Game State: Starting");
        gameView.InitializeView(); 
    }

    private void HandleWaitingForPlayersState()
    {
        Debug.Log("Game State: WaitingForPlayers");
    }
    private void HandlePlayingState()
    {
        Debug.Log("Game State: Playing");
    }
    private void HandleGameOverState()
    {
        Debug.Log("Game State: GameOver");
    }




    private void OnGameUpdated(GameModel updatedGameModel)
    {
        // Update the local game model with the data received from Firebase
        gameModel = updatedGameModel;

        // Update the view based on the new game state
        //gameView.UpdateScore(gameModel.player1.id, gameModel.player1.score);
        //gameView.UpdateScore(gameModel.player2.id, gameModel.player2.score);
        //gameView.UpdateWord(gameModel.player1.id, gameModel.player1.currentWord);
        //gameView.UpdateWord(gameModel.player2.id, gameModel.player2.currentWord);
        //gameView.UpdateTimer(gameModel.remainingTime);

        //// Check if the game has ended
        //if (gameModel.gameState == GameStatus.GameOver)
        //{
        //    gameView.ShowGameEndScreen(gameModel.player1.score > gameModel.player2.score ? gameModel.player1.id : gameModel.player2.id);
        //}
    }

    private void OnError(string errorMessage)
    {
        // Handle errors, such as failing to listen for updates
        Debug.LogError("Error listening for game updates: " + errorMessage);
    }

    private void Update()
    {
        //if (gameModel.gameState == GameStatus.Playing)
        //{
        //    gameModel.remainingTime -= Time.deltaTime;
        //    gameView.UpdateTimer(gameModel.remainingTime);

        //    if (gameModel.remainingTime <= 0f)
        //    {
        //        gameModel.gameState = GameStatus.GameOver;
        //        // Determine the winner based on score
        //        string winnerId = gameModel.player1.score > gameModel.player2.score ? gameModel.player1.id : gameModel.player2.id;
        //        if (gameModel.player1.score == gameModel.player2.score)
        //        {
        //            winnerId = null; // It's a tie
        //        }
        //        gameView.ShowGameEndScreen(winnerId);
        //    }
        //}
    }

    public void ValidateWord(string playerId)
    {
        
        string languageCode = "es-ES"; // Example language code, should be configurable

        //if (dictionaryService.IsValidWord(player.currentWord, languageCode))
        //{
        //    // Update player score
        //    player.score += player.currentWord.Length;

        //    // Check if the player has reached a goal cell
        //    if (player.currentCell.isObjective)
        //    {
        //        gameModel.gameState = GameStatus.Ended;
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
        //Player currentPlayer = DetermineCurrentPlayer();

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

    //private Player DetermineCurrentPlayer()
    //{
    //    // Simple logic to get the player corresponding to the input
    //    // This needs to be adapted to however you determine the current player
    //    return gameModel.player1; // Or gameModel.player2 based on input or other logic
    //}
}