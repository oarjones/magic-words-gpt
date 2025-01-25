using System;

namespace MagicWords.Features.Board
{
    /// <summary>
    /// Estados combinables de una celda usando flags,
    /// de modo que puedan coexistir varios a la vez.
    /// </summary>
    [Flags]
    public enum CellState
    {
        None = 0,
        Blocked = 1 << 0,
        Hidden = 1 << 1,
        Frozen = 1 << 2,

        /// <summary>
        /// Indica si la celda est� en estado "seleccionada".
        /// Para saber por qui�n, usamos OwnerId en el modelo.
        /// </summary>
        Selected = 1 << 3,

        /// <summary>
        /// Marca esta celda como la �ltima en la cadena de la palabra,
        /// de cara a permitir su deselecci�n u otras reglas.
        /// </summary>
        LastInWordChain = 1 << 4,

        // Puedes a�adir m�s si tu mec�nica lo requiere...
    }
}
