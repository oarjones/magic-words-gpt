using System.Collections.Generic;

[System.Serializable]
public class Board
{
    public Cell[,] cells;

    public Board(Cell[,] cells)
    {
        this.cells = cells;
    }

    // Add method to get neighboring cells here, very important for gameplay
    public List<Cell> GetNeighboringCells(Cell cell)
    {
        // Logic to find and return neighboring cells
        List<Cell> neighbors = new List<Cell>();
        // ...
        return neighbors;
    }
}