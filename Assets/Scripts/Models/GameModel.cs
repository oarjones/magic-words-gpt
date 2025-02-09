// GameModel.cs
using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using System.Collections.Generic;

[System.Serializable]
public class GameModel
{
    public string gameId { get; set; }
    public GameData data { get; set; }
}

public class GameData
{
    public GameStatus status { get; set; } = GameStatus.Pending;
    public GameType type { get; set; } = GameType.CatchLetter;
    public string langCode { get; set; }
    public double createdAt { get; set; }
    public Dictionary<string, GamePlayerData> playersInfo { get; set; }
    public Board gameBoard { get; set; }
}

public class GamePlayerData
{
    public string userName { get; set; }
    public int level { get; set; }
    public bool master { get; set; }
    public bool gameBoardLoaded { get; set; }
}