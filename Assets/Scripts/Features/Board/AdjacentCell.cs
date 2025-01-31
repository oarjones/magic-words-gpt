using MagicWords.Features.Board;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicWords.Features.Board
{
    /// <summary>
    /// Estructura de celdas vecinas para celdas hexagonales
    /// </summary>
    public struct AdjacentCell
    {
        public CellModel neighbor_TOP;
        public CellModel neighbor_BOTTOM;
        public CellModel neighbor_LEFTUP;
        public CellModel neighbor_LEFTDOWN;
        public CellModel neighbor_RIGHTUP;
        public CellModel neighbor_RIGHTDOWN;

        public System.Collections.Generic.List<CellModel> ToList()
        {
            return new System.Collections.Generic.List<CellModel> { neighbor_TOP, neighbor_BOTTOM, neighbor_LEFTUP, neighbor_LEFTDOWN, neighbor_RIGHTUP, neighbor_RIGHTDOWN };
        }

        internal void Clear()
        {
            neighbor_TOP = default(CellModel);
            neighbor_BOTTOM = default(CellModel);
            neighbor_LEFTUP = default(CellModel);
            neighbor_LEFTDOWN = default(CellModel);
            neighbor_RIGHTUP = default(CellModel);
            neighbor_RIGHTDOWN = default(CellModel);
    }
    }
}
