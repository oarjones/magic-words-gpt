using MagicWords.Core;

namespace MagicWords.Features.Board
{
    /// <summary>
    /// Controlador que conecta el CellModel y el CellView. 
    /// Responde a eventos (clic) y orquesta la lógica de selección.
    /// </summary>
    public class CellController : BaseController
    {
        private CellModel cellModel => (CellModel)model;
        private CellView cellView => (CellView)view;

        // Identificador del jugador local. Se podría inyectar o traer de un GameManager.
        private string localPlayerId;

        public CellController(CellModel model, CellView view, string localPlayerId)
        {
            SetModel(model);
            SetView(view);
            this.localPlayerId = localPlayerId;

            Initialize();
            cellView.SetController(this);

            // Primer refresco de la vista
            UpdateView();
        }

        public override void Initialize()
        {
            base.Initialize();
            // Si necesitas inicializar algo más
        }

        /// <summary>
        /// Llamado por la vista cuando se hace clic en la celda (OnMouseDown).
        /// </summary>
        public void OnCellClicked()
        {
            // Preguntar al modelo si se puede seleccionar
            if (cellModel.CanSelectByPlayer(localPlayerId))
            {
                bool isAlreadyOwned = cellModel.IsOwnedByPlayer(localPlayerId);
                bool isLast = cellModel.HasState(CellState.LastInWordChain);

                if (!isAlreadyOwned)
                {
                    // Seleccionar la celda para el jugador local.
                    // Suponemos que es la última en la cadena por defecto, o lo parametrizamos.
                    cellModel.SelectByPlayer(localPlayerId, true);
                }
                else
                {
                    // Si ya es del jugador local y es la última, la deseleccionamos
                    if (isLast)
                    {
                        cellModel.DeselectCell();
                    }
                }

                // Actualizar la vista
                UpdateView();
            }
            else
            {
                // Feedback: "No se puede seleccionar"
                // Podrías mostrar un sonido, un popup, etc.
            }
        }

        /// <summary>
        /// Refresca la vista con el estado actual del modelo.
        /// </summary>
        private void UpdateView()
        {
            cellView.UpdateVisual(cellModel.State,
                                  cellModel.CurrentLetter,
                                  cellModel.OwnerId,
                                  localPlayerId);
        }

        /// <summary>
        /// Si quieres actualizar el timer cada frame desde un Manager, llamas a esto.
        /// </summary>
        public void UpdateTimer(float deltaTime)
        {
            cellModel.UpdateTimer(deltaTime);
            // Si el tiempo influye en la apariencia, refresca:
            UpdateView();
        }
    }
}
