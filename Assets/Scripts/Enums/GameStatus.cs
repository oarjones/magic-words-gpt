
public enum GameStatus
{
    WaitingForOpponent,   // Lobby: Emparejamiento o espera (PvP) o preparaci�n (PvA)
    BoardGenerating,      // Se est� generando el tablero
    BoardLoaded,          // Ambos jugadores han recibido y validado el tablero
    GameSetup,            // Inicializaci�n de la vista: posicionamiento inicial, carga de UI, etc.
    GameStart,            // Estado intermedio (ready check, cuenta regresiva)
    Playing,              // Fase activa de juego
    GameOver              // Finalizaci�n del juego
}