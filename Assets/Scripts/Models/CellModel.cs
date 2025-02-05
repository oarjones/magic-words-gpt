using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Assets.Scripts.Models
{
    public class CellModel : Cell
    {
        public CellModel(Cell cell) : base(cell.Level, cell.TileNumber, cell.Name, cell.Position)
        {
        }


        #region Owner / Selection

        /// <summary>
        /// Asigna el ID del propietario de la celda.
        /// Útil para offline (IA) y online (jugador remoto).
        /// </summary>
        public void SetOwnerId(string newOwnerId)
        {
            base.OwnerId = newOwnerId;
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

    }
}
