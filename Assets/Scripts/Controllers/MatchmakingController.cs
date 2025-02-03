using UnityEngine;

public class MatchmakingController : MonoBehaviour
{
    private IBackendService backendService;
    private GameConfig gameConfig;

    public void Initialize(IBackendService backendService, GameConfig gameConfig)
    {
        this.backendService = backendService;
        this.gameConfig = gameConfig;
    }

    public void FindMatch()
    {
        backendService.FindOpponent(gameConfig.selectedGameMode, OnOpponentFound, OnError);
    }

    private void OnOpponentFound(string opponentId)
    {
        // Handle the found opponent, e.g., start a game session
        // Notify the GameController or a similar manager to start the game with the found opponent
    }

    private void OnError(string error)
    {
        // Handle the error, e.g., display a message to the user
        Debug.LogError("Matchmaking error: " + error);
    }

    public void CancelMatchmaking()
    {
        backendService.CancelMatchmaking();
    }
}