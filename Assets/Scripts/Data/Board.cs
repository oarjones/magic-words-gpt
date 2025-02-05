using System.Collections.Generic;

[System.Serializable]
public class Board
{
    public List<Cell> cells;

    public Board(List<Cell> cells)
    {
        this.cells = cells;
    }
    
}