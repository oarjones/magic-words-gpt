using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;

namespace Assets.Scripts.Data
{
    /// <summary>
    /// Estructura de celdas vecinas para celdas hexagonales
    /// </summary>
    public struct AdjacentCell
    {
        public Cell neighbor_TOP;
        public Cell neighbor_BOTTOM;
        public Cell neighbor_LEFTUP;
        public Cell neighbor_LEFTDOWN;
        public Cell neighbor_RIGHTUP;
        public Cell neighbor_RIGHTDOWN;

        public System.Collections.Generic.List<Cell> ToList()
        {
            return new System.Collections.Generic.List<Cell> { neighbor_TOP, neighbor_BOTTOM, neighbor_LEFTUP, neighbor_LEFTDOWN, neighbor_RIGHTUP, neighbor_RIGHTDOWN };
        }

        internal void Clear()
        {
            neighbor_TOP = default(Cell);
            neighbor_BOTTOM = default(Cell);
            neighbor_LEFTUP = default(Cell);
            neighbor_LEFTDOWN = default(Cell);
            neighbor_RIGHTUP = default(Cell);
            neighbor_RIGHTDOWN = default(Cell);
        }
    }
}
