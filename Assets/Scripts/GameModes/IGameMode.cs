public interface IGameMode
{
        
    void InitializeGame(GameConfig gameConfig, BoardConfig boardConfig, 
        IBackendService backendService, IDictionaryService dictionaryService, BoardGenerator boardGenerator, System.Action<GameModel> onGameStarted);
}