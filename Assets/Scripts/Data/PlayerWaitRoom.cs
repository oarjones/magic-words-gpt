using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Data
{
    public class PlayerWaitRoom
    {
        public string userName { get; set; }
        public int level { get; set; } = 0;
        public string langCode { get; set; } = "es-ES";
        public double createdAt { get; set; }
        public string status { get; set; } = "waiting";
    }
}
