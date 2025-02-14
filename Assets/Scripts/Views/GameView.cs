using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    [Header("Player Info")]
    public Image player1Icon;
    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player1ScoreText;

    public Image player2Icon;
    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI player2ScoreText;

    [Header("Game Board")]
    public GameObject boardContainer; // Contenedor para el tablero

    [Header("Controls")]
    public Button validateButton;
    public Button powerUpButton;

    [Header("Timer")]
    public TextMeshProUGUI timerText;

    private GameModelNotifier modelNotifier;

    // Se invoca desde el GameController al inicializar la vista.
    public void Initialize(GameModelNotifier notifier)
    {
        modelNotifier = notifier;
        // Suscribirse al evento para actualizar la UI cuando cambie el modelo.
        modelNotifier.OnModelChanged += OnModelChanged;
        // Inicializa la UI con los datos actuales
        RefreshUI();
    }

    // Método para configurar la UI al entrar en GameSetup
    public void SetupUI()
    {
        // Supongamos que la configuración de los jugadores proviene de ScriptableObjects
        // Por el momento, valores de ejemplo:
        player1NameText.text = "Player 1";
        player2NameText.text = "Player 2";
        player1ScoreText.text = "0";
        player2ScoreText.text = "0";
        timerText.text = FormatTime(modelNotifier.Model.data.createdAt); // O usar remainingTime si se ha definido

        // Configurar botones
        validateButton.onClick.AddListener(OnValidateButtonClicked);
        powerUpButton.onClick.AddListener(OnPowerUpButtonClicked);

        // Se podría instanciar o renderizar el tablero en boardContainer, según la lógica de la escena.
    }

    private void OnModelChanged(GameModel model)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (modelNotifier == null || modelNotifier.Model == null)
            return;

        // Actualización de scores (si los hubiera en el GamePlayerData o en otro sitio)
        // Aquí se actualizan los textos del score y el temporizador.
        // Ejemplo:
        // player1ScoreText.text = modelNotifier.Model.data.playersInfo["player1"].score.ToString();
        // player2ScoreText.text = modelNotifier.Model.data.playersInfo["player2"].score.ToString();

        // Actualizar temporizador:
        // Suponiendo que disponemos de remainingTime en algún lado, se actualiza el timer:
        // timerText.text = FormatTime(modelNotifier.Model.data.createdAt);
    }

    private string FormatTime(double timeInSeconds)
    {
        int minutes = Mathf.FloorToInt((float)timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt((float)timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnValidateButtonClicked()
    {
        Debug.Log("Botón de validar pulsado.");
        // Se comunica con el GameController para procesar la validación.
    }

    private void OnPowerUpButtonClicked()
    {
        Debug.Log("Botón de power-up pulsado.");
        // Se activa el power-up correspondiente.
    }

    private void OnDestroy()
    {
        if (modelNotifier != null)
            modelNotifier.OnModelChanged -= OnModelChanged;
    }
}
