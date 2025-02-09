using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{
    public class UserStatistics
    {
        public decimal avscorepergame { get; set; } = 0;
        public decimal avtimepergame { get; set; } = 0;
        public decimal avwongames { get; set; } = 0;
        public List<GameStatistics> gamestatistics { get; set; } = new List<GameStatistics>();

    }

    public class GameStatistics
    {
        public Dictionary<string, GameStatisticsData> gamestatisticsData { get; set; } = new Dictionary<string, GameStatisticsData>();
    }

    public class GameStatisticsData
    {
        public GameReult result { get; set; } = GameReult.unfinished;
        public double startedat { get; set; }
        public double finishedat { get; set; } = -1;
        public decimal avcomplexityratio { get; set; } = 0;
        public decimal avlettersinwords { get; set; } = 0;
        public string opponentId { get; set; }
        public List<string> words { get; set; } = new List<string>();

    }
}
