
public enum GameStatus
{
    WaitingForOpponent,   // Lobby: Emparejamiento o espera (PvP) o preparación (PvA)
    BoardGenerating,      // Se está generando el tablero
    BoardLoaded,          // Ambos jugadores han recibido y validado el tablero
    GameSetup,            // Inicialización de la vista: posicionamiento inicial, carga de UI, etc.
    GameStart,            // Estado intermedio (ready check, cuenta regresiva)
    Playing,              // Fase activa de juego
    GameOver              // Finalización del juego
}