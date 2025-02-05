// PvPGameMode.cs
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PvPGameMode : IGameMode
{
    private string playerId;
    private string opponentId;
    private IBackendService backendService;

    public PvPGameMode(string playerId, string opponentId, IBackendService backendService)
    {
        this.playerId = playerId;
        this.opponentId = opponentId;
        this.backendService = backendService;
    }

    public GameModel CreateGame(GameConfig gameConfig, BoardConfig boardConfig, IBackendService backendService, IDictionaryService dictionaryService)
    {
        // Initialize players from backend data
        Task<Player> player1Task = GetPlayerWithTask(backendService, playerId);
        Task<Player> player2Task = GetPlayerWithTask(backendService, opponentId);

        // Use Task.WhenAll to wait for both tasks to complete
        Task.WhenAll(player1Task, player2Task).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to retrieve player data: " + task.Exception);
                return null;
            }

            Player player1 = player1Task.Result;
            Player player2 = player2Task.Result;

            // If player data is null, initialize them with default values
            if (player1 == null)
            {
                player1 = new Player(playerId, "Player 1", GetInitialCellForPlayer(boardConfig, true));
            }
            if (player2 == null)
            {
                player2 = new Player(opponentId, "Player 2", GetInitialCellForPlayer(boardConfig, false));
            }

            // Create a new board with the specified configuration
            Board board = CreateBoard(boardConfig);

            // Create and return a new GameModel
            return new GameModel(board, player1, player2, gameConfig);
        }, TaskScheduler.FromCurrentSynchronizationContext()); // Ensure ContinueWith runs on the main thread

        return null; // Return null or a placeholder value immediately
    }

    // Método asíncrono para obtener los datos del jugador con una tarea (Task)
    private async Task<Player> GetPlayerWithTask(IBackendService backendService, string playerId)
    {
        // Usa una tarea para envolver la llamada a GetPlayerData
        return await Task.Run(() =>
        {
            Player result = null;

            // Usa un AutoResetEvent para esperar a que se complete la callback
            var resetEvent = new AutoResetEvent(false);

            backendService.GetPlayerData(playerId, (player) =>
            {
                result = player;
                resetEvent.Set(); // Señala que la callback ha terminado
            });

            resetEvent.WaitOne(); // Espera a que la callback se complete
            return result;
        });
    }

    private Board CreateBoard(BoardConfig boardConfig)
    {
        //// Generate a new board based on boardConfig
        //Cell[,] cells = new Cell[boardConfig.boardWidth, boardConfig.boardHeight];
        //for (int x = 0; x < boardConfig.boardWidth; x++)
        //{
        //    for (int y = 0; y < boardConfig.boardHeight; y++)
        //    {
        //        // Example letter, replace with your logic to assign letters
        //        string letter = GetRandomLetter();
        //        cells[x, y] = new Cell(x, y, letter);
        //    }
        //}
        //return new Board(cells);

        return new BoardGenerator().GenerateBoard(boardConfig.mapSize);
    }

    private CellView GetInitialCellForPlayer(BoardConfig boardConfig, bool isPlayer1)
    {
        return null;
    }

    private string GetRandomLetter()
    {
        // Logic to return a random letter, you can use your own logic here
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return letters[UnityEngine.Random.Range(0, letters.Length)].ToString();
    }
}