using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace LongDark
{
    [ApiVersion(2, 1)]
    public class LongDark : TerrariaPlugin
    {
        public override string Name => "Long Dark";
        public override string Author => "Cobalt";
        public override string Description => "Advanced difficulty";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        Data data;
        DateTime lastCheck;
        Config config;
        static string configPath = "tshock\\LongDark.json";
        const int cozyFire = 87, burning = 67, heart_lamp = 89, blessing = 165, tipsy = 25, well_fed = 26, warmth = 124, inferno = 116, on_fire = 24, imbue_cursed_flames = 73, imbue_fire = 74;
        public LongDark(Main game) : base(game)
        {
        }

        public override void Initialize()
        {
            ReadConfig(configPath, new Config(), ref config);
            data = new Data(config.MinTemperature, config.MaxTemperature);
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
            GetDataHandlers.PlayerDamage += OnPlayerDamaged;
            GetDataHandlers.KillMe += OnKillMe;
            Commands.ChatCommands.Add(new Command("longdark.use", OnCommand, "ldinfo") { HelpText = "Show buffs' duration" });
            lastCheck = DateTime.UtcNow;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
                TShockAPI.GetDataHandlers.PlayerDamage -= OnPlayerDamaged;
            }
            base.Dispose(disposing);
        }

        public static void ReadConfig<ConfigType>(string path, ConfigType defaultConfig, ref ConfigType config)
        {
            if (!File.Exists(path))
            {
                config = defaultConfig;
                File.WriteAllText(path, JsonConvert.SerializeObject((object)config));
            }
            else
            {
                config = JsonConvert.DeserializeObject<ConfigType>(File.ReadAllText(path));
                File.WriteAllText(path, JsonConvert.SerializeObject((object)config));
            }
        }

        private void OnKillMe(object sender, GetDataHandlers.KillMeEventArgs args)
        {
            data.SetTemperature(TShock.Players[args.PlayerId].Name, 1);
        }

        private void OnPlayerDamaged(object sender, GetDataHandlers.PlayerDamageEventArgs args)
        {
            TSPlayer player = TShock.Players[args.ID];
            player.SetBuff(44, 60 * config.FrostBurnDuration);
            player.SetBuff(20, 60 * config.PoisonedDuration);
            player.SetBuff(30, 60 * config.BleedingDuration);
            player.SetBuff(46, 60 * config.ChilledDuration);
            player.SetBuff(31, 60 * config.ConfusedDuration);
        }

        private void OnUpdate(EventArgs args)
        {
            if ((DateTime.UtcNow - lastCheck).TotalSeconds >= 3)
            {
                SecondsUpdate();
                lastCheck = DateTime.UtcNow;
            }
        }

        private void OnCommand(CommandArgs args)
        {
            args.Player.SendInfoMessage("FrostBurnDuration: " + config.FrostBurnDuration + "s");
            args.Player.SendInfoMessage("PoisonedDuration: " + config.PoisonedDuration + "s");
            args.Player.SendInfoMessage("BleedingBiteDuration: " + config.BleedingDuration + "s");
            args.Player.SendInfoMessage("ChilledDuration: " + config.ChilledDuration + "s");
            args.Player.SendInfoMessage("ConfusedDuration: " + config.ConfusedDuration + "s");
        }

        private bool Warm(int index)
        {
            int length = TShock.Players[index].TPlayer.buffType.Length;
            for (int i = 0; i < length; i++)
            {
                int type = TShock.Players[index].TPlayer.buffType[i];
                if (type == cozyFire || type == burning || type == heart_lamp || type == blessing || type == tipsy || type == well_fed || type == warmth || type == inferno || type == on_fire || type == imbue_cursed_flames || type == imbue_fire)
                    return true;
            }
            return false;
        }

        private void UpdateStatus(int index, bool w)
        {
            string msg = "";
            for (int i = 1; i <= 20; i++)
            {
                msg += "\n";
            }
            msg += "Temperature: " + data.GetTemperature(TShock.Players[index].Name).ToString("0.#") + "℃\n";
            if (w)
                msg += config.WarmerMessage;
            else
                msg += config.ColderMessage;
            for (int i = 1; i <= 20; i++)
            {
                msg += "\n";
            }
            TShock.Players[index].SendData(PacketTypes.Status, msg);
        }

        private void SecondsUpdate()
        {
            int length = TShock.Players.Length;
            for (int i = 0; i < length; i++)
            {
                TSPlayer p = TShock.Players[i];
                if (p == null)
                    continue;
                string name = p.Name;
                if (Warm(i))
                {
                    data.AlterTemperature(name, 0.1f * config.AlterSpeed);
                    UpdateStatus(i, true);
                }
                else
                {
                    data.AlterTemperature(name, -0.1f * config.AlterSpeed);
                    UpdateStatus(i, false);
                }
                float f = data.GetTemperature(name);
                if (f < 0 && config.ChilledDuration > 0)
                {
                    TShock.Players[i].SetBuff(46, 60 * config.ChilledDuration);
                }
                if (f < config.ChimatlonTemperature)
                {
                    TShock.Players[i].DamagePlayer((int)(-f + TShock.Players[i].TPlayer.statDefense * 0.77));
                }
            }
        }
    }
}