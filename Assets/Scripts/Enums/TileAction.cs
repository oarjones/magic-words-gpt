using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enums
{
    public enum TileAction
    {
        None = 0,
        Select = 1,
        Unselect = 2,
        ChangeLetter = 3,
        UpdateLetter = 4,
        SetObjective = 5,
        FreezeTile = 6,
        FreezePlayer = 7
    }
}
