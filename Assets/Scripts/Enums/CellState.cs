using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enums
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
        /// Indica si la celda está en estado "seleccionada".
        /// Para saber por quién, usamos OwnerId en el modelo.
        /// </summary>
        Selected = 1 << 3,

        /// <summary>
        /// Marca esta celda como la última en la cadena de la palabra,
        /// de cara a permitir su deselección u otras reglas.
        /// </summary>
        LastInWordChain = 1 << 4,

        // Puedes añadir más si tu mecánica lo requiere...
    }
}
