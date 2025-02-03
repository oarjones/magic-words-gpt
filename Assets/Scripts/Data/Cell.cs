[System.Serializable]
public class Cell
{
    public int x;
    public int y;
    public string letter;
    public bool isOccupied; //Add this property to track if a cell is occupied by a player.
    public bool isObjective; // Add this property to mark a cell as an objective.

    public Cell(int x, int y, string letter)
    {
        this.x = x;
        this.y = y;
        this.letter = letter;
        this.isOccupied = false;
        this.isObjective = false;
    }
}