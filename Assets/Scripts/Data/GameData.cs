using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{

    public class Game
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
        public GameBoard gameBoard { get; set; } = new GameBoard();
    }

    public class GamePlayerData
    {
        public string userName { get; set; }
        public int level { get; set; }
        public bool master { get; set; }
        public bool gameBoardLoaded { get; set; }
    }

    public class BoardTile
    {
        public PosVector posVector { get; set; }
        public string name { get; set; }
        public int level { get; set; }
        public int tileNumber { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public bool isObjectiveTile { get; set; }
        public string ownerId { get; set; }
        public string letter { get; set; }
        public TileAction action { get; set; } = TileAction.None;
        public GameTileState tileState { get; set; } = GameTileState.Unselected;
        //public int index { get; set; }
    }

    

    

    public class PosVector
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class GameBoard
    {
        public string gameId { get; set; }
        public List<BoardTile> boardTiles { get; set; }
    }



}
