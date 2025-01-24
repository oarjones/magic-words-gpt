using UnityEngine;

namespace MagicWords.Core.Managers
{
    /// <summary>
    /// Se encarga de mostrar, ocultar y administrar las pantallas o paneles UI.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        // Métodos para abrir/cerrar pantallas:
        // public void ShowPanel(GameObject panel);
        // public void HidePanel(GameObject panel);
        // etc.
    }
}
