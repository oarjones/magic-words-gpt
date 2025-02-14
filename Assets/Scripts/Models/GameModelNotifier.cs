using System;

public class GameModelNotifier
{
    public GameModel Model { get; private set; }

    // Evento que se disparará cada vez que se actualice el modelo.
    public event Action<GameModel> OnModelChanged;

    public GameModelNotifier(GameModel model)
    {
        Model = model;
    }

    // Métodos de actualización centralizados que actualizan el modelo y notifican a los observadores.
    public void UpdateStatus(GameStatus newStatus)
    {
        if (Model.data.status != newStatus)
        {
            Model.data.status = newStatus;
            NotifyModelChanged();
        }
    }

    public void UpdateGameBoard(Board board)
    {
        Model.data.gameBoard = board;
        NotifyModelChanged();
    }

    public void UpdatePlayerScore(string playerId, int newScore)
    {
        // Supongamos que GamePlayerData tiene algún campo para score; si no, se puede agregar o realizar otra actualización.
        // Aquí se realizaría la actualización necesaria.
        NotifyModelChanged();
    }

    // Método para notificar cambios
    private void NotifyModelChanged()
    {
        OnModelChanged?.Invoke(Model);
    }
}
