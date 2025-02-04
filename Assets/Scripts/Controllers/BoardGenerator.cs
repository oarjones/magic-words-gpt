using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public BoardConfig boardConfig; // Asigna el ScriptableObject BoardConfig en el Inspector

    public Board GenerateBoard()
    {
        int width = boardConfig.boardWidth;
        int height = boardConfig.boardHeight;
        Cell[,] cells = new Cell[width, height];
        List<CellView> cellViews = new List<CellView>(); // To associate Cell data with CellView instances

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate the position based on hexagonal grid logic
                float xPos = x * boardConfig.cellSpacing * 0.866f; // 0.866f is approximately sqrt(3)/2
                float yPos = y * boardConfig.cellSpacing;

                // Offset for odd rows
                if (y % 2 != 0)
                {
                    xPos += boardConfig.cellSpacing * 0.866f / 2;
                }

                // Create the cell data
                string letter = GetRandomLetter(); // You'll need to implement this
                Cell cell = new Cell(x, y, letter);
                cells[x, y] = cell;

                // Instantiate the cell prefab
                GameObject cellGO = Instantiate(boardConfig.cellPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity, transform);
                cellGO.name = $"Cell_{x}_{y}";
                CellView cellView = cellGO.GetComponent<CellView>();
                cellView.Initialize(cell);
                cellViews.Add(cellView); // Add the CellView to the list
            }
        }

        // Now that all cells are created, set up neighbors
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y].neighbors = GetNeighboringCells(cells, x, y);
            }
        }

        return new Board(cells);
    }

    private string GetRandomLetter()
    {
        // Logic to return a random letter, you can use your own logic here
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return letters[UnityEngine.Random.Range(0, letters.Length)].ToString();
    }

    public List<Cell> GetNeighboringCells(Cell[,] cells, int x, int y)
    {
        List<Cell> neighbors = new List<Cell>();
        int width = cells.GetLength(0);
        int height = cells.GetLength(1);

        // Define the relative positions of neighbors in a hexagonal grid
        int[,] neighborOffsets = {
            { 0, -1 }, { 1, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { -1, -1 }
        };

        // Adjust neighbor offsets for odd rows
        if (y % 2 != 0)
        {
            neighborOffsets[1, 0] = 0;
            neighborOffsets[1, 1] = -1;
            neighborOffsets[5, 0] = 0;
            neighborOffsets[5, 1] = -1;
        }

        for (int i = 0; i < 6; i++)
        {
            int nx = x + neighborOffsets[i, 0];
            int ny = y + neighborOffsets[i, 1];

            // Check if the neighbor is within the board boundaries
            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                neighbors.Add(cells[nx, ny]);
            }
        }

        return neighbors;
    }
}