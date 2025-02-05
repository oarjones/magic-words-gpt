using System.Collections.Generic;

[System.Serializable]
public class Player
{
    public string id;
    public string name;
    public int score;
    public CellView currentCell;
    public string currentWord;
    public List<PowerUpType> powerUps;

    public Player(string id, string name, CellView initialCell)
    {
        this.id = id;
        this.name = name;
        this.score = 0;
        this.currentCell = initialCell;
        this.currentWord = initialCell.cellModel.CurrentLetter;
        this.powerUps = new List<PowerUpType>();
    }
}