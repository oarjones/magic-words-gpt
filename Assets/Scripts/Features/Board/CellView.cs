using UnityEngine;
using UnityEngine.UI;    // O TextMeshPro si prefieres
using MagicWords.UI;

namespace MagicWords.Features.Board
{
    /// <summary>
    /// Vista que muestra la celda en pantalla: sprite, texto, efectos. 
    /// Maneja la detección de clicks y notifica al controlador.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class CellView : BaseView
    {
        [Header("Visual References")]
        [SerializeField] private SpriteRenderer cellSpriteRenderer;
        [SerializeField] private Text letterText; // O TextMeshProUGUI
        [SerializeField] private GameObject blockedEffect;
        [SerializeField] private GameObject freezeEffect;
        [SerializeField] private GameObject selectedHalo;
        [SerializeField] private GameObject hiddenVisual;

        private CellController cellController;

        public void SetController(CellController controller)
        {
            cellController = controller;
        }

        /// <summary>
        /// Actualiza la apariencia según el estado y dueño.
        /// </summary>
        public void UpdateVisual(CellState state, string letter, string ownerId, string localPlayerId)
        {
            // Texto/letter
            if (letterText != null)
                letterText.text = letter;

            // Bloqueo
            if (blockedEffect != null)
                blockedEffect.SetActive(state.HasFlag(CellState.Blocked));

            // Congelado
            if (freezeEffect != null)
                freezeEffect.SetActive(state.HasFlag(CellState.Frozen));

            // Seleccionada
            bool isSelected = state.HasFlag(CellState.Selected);
            if (selectedHalo != null)
                selectedHalo.SetActive(isSelected);

            // Oculta
            if (hiddenVisual != null)
                hiddenVisual.SetActive(state.HasFlag(CellState.Hidden));

            // Ejemplo: podrías cambiar el color del sprite si la celda es del jugador local u oponente
            if (cellSpriteRenderer != null)
            {
                if (!isSelected)
                {
                    cellSpriteRenderer.color = Color.white; // default
                }
                else
                {
                    if (ownerId == localPlayerId)
                    {
                        cellSpriteRenderer.color = Color.green; // color de selección local
                    }
                    else
                    {
                        cellSpriteRenderer.color = Color.red;   // color de selección oponente/bot
                    }
                }
            }
        }

        private void OnMouseDown()
        {
            cellController?.OnCellClicked();
        }
    }
}
