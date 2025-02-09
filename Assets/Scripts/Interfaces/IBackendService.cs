public interface IBackendService
{
    void Authenticate(System.Action<bool> callback);
    // ... other backend operations
    void FindOpponent(GameMode gameMode, System.Action<string> onOpponentFound, System.Action<string> onError);
    void CancelMatchmaking();
    void StartGameSession(string playerId, string opponentId, System.Action<string> onGameSessionStarted, System.Action<string> onError);
    void ListenForGameUpdates(string gameSessionId, System.Action<GameModel> onGameUpdated, System.Action<string> onError);
}