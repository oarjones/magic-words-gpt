public interface IGameMode
{
    GameModel CreateGame(GameConfig gameConfig, BoardConfig boardConfig, IBackendService backendService, IDictionaryService dictionaryService);
}