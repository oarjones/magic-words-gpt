using System.Collections.Generic;
using MagicWords.Core;
using MagicWords.Core.Utilities; // Si quieres disparar eventos desde aquí
using UnityEngine; // Solo si necesitas Debug.Log, sino lo quitas

namespace MagicWords.Features.Board
{
    /// <summary>
    /// Representa la lógica interna y datos de una celda del tablero.
    /// Usa un enum con flags (CellState) y un OwnerId para saber quién la posee.
    /// </summary>
    public class CellModel : BaseModel
    {
        // Posición de la celda (por si fuera útil)
        public int Row { get; private set; }
        public int Col { get; private set; }


        public int Level { get; private set; }
        public int TileNumber { get; private set; }
        public string Name { get; private set; }

        // El estado combinable (bloqueado, oculto, congelado, seleccionado, etc.)
        public CellState State { get; private set; } = CellState.None;

        /// <summary>
        /// Identifica quién es el "dueño" actual de la celda.
        /// - En modo offline, puede ser "AI_Easy", "AI_Hard", etc.
        /// - En modo online, será el UID de Firebase.
        /// Si está vacío (""), no hay dueño.
        /// </summary>
        public string OwnerId { get; private set; } = string.Empty;

        // Letra que muestra la celda
        public string CurrentLetter { get; private set; } = string.Empty;

        // Control de tiempo (si la celda usa contador)
        public bool UseTimeCounter { get; private set; }
        public float TimeCounter { get; private set; }

        // Celdas adyacentes, si necesitas conectividad (hexagonal, etc.)
        public List<CellModel> AdjacentCells { get; private set; } = new List<CellModel>();

        public CellModel(int level, int tileNumber, string name, Vector3 position)
        {
            Level= level;
            TileNumber= tileNumber;
            Name= name;
            Position = position;
        }


        public Vector3 Position { get; private set; }

        public void SetPosition(float x, float y)
        {
            Position = new Vector3(x, y, 0);
        }
        public override void Initialize()
        {
            base.Initialize();
            State = CellState.None;
            OwnerId = string.Empty;
            CurrentLetter = string.Empty;
            UseTimeCounter = false;
            TimeCounter = 0f;
            AdjacentCells.Clear();
        }

        #region Owner / Selection

        /// <summary>
        /// Asigna el ID del propietario de la celda.
        /// Útil para offline (IA) y online (jugador remoto).
        /// </summary>
        public void SetOwnerId(string newOwnerId)
        {
            OwnerId = newOwnerId;
            // Si quieres disparar un evento global cada vez que cambie de Owner, podrías hacerlo aquí.
        }

        /// <summary>
        /// Comprueba si la celda tiene dueño (cualquier jugador).
        /// </summary>
        public bool HasOwner()
        {
            return !string.IsNullOrEmpty(OwnerId);
        }

        /// <summary>
        /// Devuelve true si la celda pertenece al jugador local (comparando su ID).
        /// </summary>
        public bool IsOwnedByPlayer(string localPlayerId)
        {
            return OwnerId == localPlayerId && !string.IsNullOrEmpty(localPlayerId);
        }

        /// <summary>
        /// Devuelve true si la celda pertenece a otro jugador (IA, oponente online, etc.).
        /// </summary>
        public bool IsOwnedBySomeoneElse(string localPlayerId)
        {
            return !string.IsNullOrEmpty(OwnerId) && OwnerId != localPlayerId;
        }

        #endregion

        #region State Management (Flags)

        public void AddState(CellState stateToAdd)
        {
            State |= stateToAdd;
            // EventBus.RaiseCellStateChanged(this); // si deseas dispararlo inmediatamente
        }

        public void RemoveState(CellState stateToRemove)
        {
            State &= ~stateToRemove;
            // EventBus.RaiseCellStateChanged(this);
        }

        public bool HasState(CellState stateToCheck)
        {
            return State.HasFlag(stateToCheck);
        }

        /// <summary>
        /// Reemplaza por completo el estado, si fuera necesario.
        /// </summary>
        public void SetExactState(CellState newState)
        {
            State = newState;
            // EventBus.RaiseCellStateChanged(this);
        }

        #endregion

        #region Logic / Methods

        /// <summary>
        /// Define la letra que contiene la celda.
        /// </summary>
        public void SetLetter(string letter)
        {
            CurrentLetter = letter;
        }

        /// <summary>
        /// Activa un contador de tiempo en la celda.
        /// </summary>
        public void ActivateTimeCounter(float duration)
        {
            UseTimeCounter = true;
            TimeCounter = duration;
        }

        /// <summary>
        /// Desactiva el contador de tiempo.
        /// </summary>
        public void DeactivateTimeCounter()
        {
            UseTimeCounter = false;
            TimeCounter = 0f;
        }

        /// <summary>
        /// Actualiza el contador en cada frame. 
        /// Llamarlo desde un manager si lo usas.
        /// </summary>
        public void UpdateTimer(float deltaTime)
        {
            if (UseTimeCounter && TimeCounter > 0)
            {
                TimeCounter -= deltaTime;
                if (TimeCounter < 0) TimeCounter = 0;
            }
        }

        /// <summary>
        /// Comprueba si el jugador local puede seleccionar esta celda.
        /// </summary>
        public bool CanSelectByPlayer(string localPlayerId)
        {
            // 1) No puede estar bloqueada, oculta o congelada
            if (HasState(CellState.Blocked) || HasState(CellState.Hidden) || HasState(CellState.Frozen))
                return false;

            // 2) Si ya pertenece a otro (distinto del local), no se puede seleccionar
            if (IsOwnedBySomeoneElse(localPlayerId))
                return false;

            // 3) Si ya es del jugador local, la lógica depende de tu mecánica:
            //    Por ejemplo, sólo permitir re-seleccionarla si es la última en la cadena.
            if (IsOwnedByPlayer(localPlayerId) && !HasState(CellState.LastInWordChain))
                return false;

            // Si nada impide, se puede seleccionar
            return true;
        }

        /// <summary>
        /// Selecciona la celda para el jugador local, aplicando flags y OwnerId.
        /// </summary>
        public void SelectByPlayer(string localPlayerId, bool isLastInChain)
        {
            SetOwnerId(localPlayerId);
            AddState(CellState.Selected);

            if (isLastInChain) AddState(CellState.LastInWordChain);
            else RemoveState(CellState.LastInWordChain);

            // Aquí podrías invocar tu evento global si quieres
            // EventBus.RaiseCellStateChanged(this);
        }

        /// <summary>
        /// Deselecciona la celda, limpiando Owner y flags de selección.
        /// </summary>
        public void DeselectCell()
        {
            SetOwnerId(string.Empty);
            RemoveState(CellState.Selected);
            RemoveState(CellState.LastInWordChain);
            // EventBus.RaiseCellStateChanged(this);
        }

        #endregion

        #region Adjacency

        public void AddAdjacentCell(CellModel neighbor)
        {
            if (neighbor != null && !AdjacentCells.Contains(neighbor))
            {
                AdjacentCells.Add(neighbor);
            }
        }

        #endregion
    }
}
