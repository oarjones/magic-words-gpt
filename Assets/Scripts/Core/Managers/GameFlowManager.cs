using UnityEngine;

namespace MagicWords.Core.Managers
{
    /// <summary>
    /// Gestiona el flujo general del juego (carga de escenas, inicio de partida, etc.).
    /// </summary>
    public class GameFlowManager : MonoBehaviour
    {
        private void Awake()
        {
            // Por ejemplo, asegurarnos de que este objeto no se destruya al cambiar de escena
            DontDestroyOnLoad(gameObject);

            // Inicializaciones generales
        }

        private void Start()
        {
            // Podrías cargar la escena principal o mostrar el menú inicial
            // SceneManager.LoadScene("MainMenuScene");
        }
    }
}
