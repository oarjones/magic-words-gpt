using System.Collections.Generic;

[System.Serializable]
public class Board
{
    public List<CellView> cells;

    public Board(List<CellView> cells)
    {
        this.cells = cells;
    }
    
}

