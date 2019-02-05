using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LongDark
{
    public class Config
    {
        public int MaxTemperature = 5;
        public int MinTemperature = -10;
        public int ChimatlonTemperature = -1;
        public int AlterSpeed = 1;
        public int FrostBurnDuration = 10;
        public int PoisonedDuration = 10;
        public int BleedingDuration = 30;
        public int ChilledDuration = 20;
        public int ConfusedDuration = 20;
        public string ColderMessage = "Getting Cold";
        public string WarmerMessage = "Feeling Warm";
    }
}
