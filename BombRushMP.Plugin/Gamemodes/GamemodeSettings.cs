using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Gamemodes
{
    public class GamemodeSettings
    {
        public Dictionary<int, GamemodeSetting> SettingByID = new();
        public string GetDisplayString(bool host, bool inGame)
        {
            var str = "";
            foreach (var setting in SettingByID.Values)
            {
                str += $"{setting.ToString()}\n";
            }
            if (!inGame) {
                if (!string.IsNullOrEmpty(str))
                    str += "\n";
                if (host)
                {
                    str += @"Use the Lobby menu in the Multiplayer phone app to start the game or change the mode.";
                }
                else
                {
                    str += @"Use the Lobby menu in the Multiplayer phone app to toggle Ready.";
                }
            }
            return str;
        }
    }
}
