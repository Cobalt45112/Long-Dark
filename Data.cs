using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LongDark
{
    class Data
    {
        class LDPlayer
        {
            public string name = null;
            public float temperature = 0;
            public LDPlayer(string str, float f)
            {
                name = str;
                temperature = f;
            }
        }
        int MaxTemperature, MinTemperature;
        Dictionary<string, LDPlayer> LDPlayers = new Dictionary<string, LDPlayer>();
        public Data(int x, int y)
        {
            MinTemperature = x;
            MaxTemperature = y;
        }
        public float GetTemperature(string name)
        {
            if (LDPlayers.ContainsKey(name))
            {
                return LDPlayers[name].temperature;
            }
            return 0;
        }

        public void AlterTemperature(string name, float f)
        {
            if (LDPlayers.ContainsKey(name))
            {
                if (f >= 0 && LDPlayers[name].temperature + f < MaxTemperature)
                    LDPlayers[name].temperature += f;
                if (f < 0 && LDPlayers[name].temperature + f > MinTemperature)
                    LDPlayers[name].temperature += f;
            }
            else
            {
                LDPlayer p = new LDPlayer(name, 1);
                LDPlayers[name] = p;
                AlterTemperature(name, f);
            }
        }
        public void SetTemperature(string name, float f)
        {
            if (LDPlayers.ContainsKey(name))
            {
                LDPlayers[name].temperature = f;
            }
            else
            {
                LDPlayer p = new LDPlayer(name, 1);
                LDPlayers[name] = p;
                SetTemperature(name, f);
            }
        }
    }
}
