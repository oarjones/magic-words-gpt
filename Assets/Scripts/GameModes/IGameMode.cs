//public interface IGameMode
//{

//    void InitializeGame(GameConfig gameConfig, BoardConfig boardConfig, IBackendService backendService, 
//        IDictionaryService dictionaryService, BoardGenerator boardGenerator, System.Action<GameModel> onGameStarted);
//}

using System;

public interface IGameMode
{

    /// <summary>
    /// Inicializa el modo de juego con los parámetros necesarios.
    /// </summary>
    /// <param name="gameConfig"></param>
    /// <param name="boardConfig"></param>
    /// <param name="backendService"></param>
    /// <param name="dictionaryService"></param>
    /// <param name="boardGenerator"></param>
    void Initialize(GameConfig gameConfig, BoardConfig boardConfig, IBackendService backendService,
        IDictionaryService dictionaryService, BoardGenerator boardGenerator);

    /// <summary>
    /// Comprueba la conexión a Internet.
    /// </summary>
    /// <param name="onConnectionChecked"></param>
    void CheckConnection(Action<bool> onConnectionChecked);

    /// <summary>
    /// Inicia el proceso de emparejamiento y espera a que se encuentre un oponente.
    /// El callback se invoca cuando se detecta que se ha emparejado (por ejemplo, tras registrar al jugador en Firebase y detectar la entrada de un segundo jugador).
    /// </summary>
    void StartWaitingForOpponent(Action onOpponentFound);

    /// <summary>
    /// Inicia la generación del tablero.
    /// Este método se encarga de que, en caso de que el jugador sea MASTER, se genere y se suba el tablero a Firebase.
    /// El callback recibe el GameModel actualizado con la información del tablero generado.
    /// </summary>
    void GenerateBoard(Action<GameModel> onBoardGenerated);

    /// <summary>
    /// Se suscribe a los eventos necesarios para detectar la carga y sincronización del tablero.
    /// Cuando se detecta que el tablero se ha cargado (ya sea para el jugador MASTER o SLAVE), se invoca el callback con el GameModel actualizado.
    /// </summary>
    void WaitForBoardLoad(Action<GameModel> onBoardLoaded);
}