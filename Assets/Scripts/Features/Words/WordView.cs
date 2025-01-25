using UnityEngine;
using UnityEngine.UI;
using MagicWords.Core;
using MagicWords.UI;
using TMPro;

namespace MagicWords.Features.Words
{
    /// <summary>
    /// Vista para el sistema de palabras. Maneja la UI (ej. un campo de texto, botón, etc.).
    /// </summary>
    public class WordView : BaseView
    {
        [SerializeField] private Text wordText;
        [SerializeField] private Button validateButton;
        [SerializeField] private InputField inputField;

        // El controlador se asignará dinámicamente
        private WordController wordController;

        private void Awake()
        {
            // Asignar el listener al botón
            validateButton.onClick.AddListener(OnValidateButtonClick);
        }

        public void SetController(WordController controller)
        {
            this.wordController = controller;
        }

        private void OnValidateButtonClick()
        {
            wordController?.OnValidateButtonPressed(inputField.text);
        }

        /// <summary>
        /// Muestra la palabra actual (para debug o feedback en pantalla).
        /// </summary>
        public void UpdateWordLabel(string newWord)
        {
            if (wordText != null)
                wordText.text = newWord;
        }
    }
}
