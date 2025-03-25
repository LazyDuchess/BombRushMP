using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin.Gamemodes
{
    public class GamemodeSettings
    {
        public Dictionary<int, GamemodeSetting> SettingByID = new();

        public SavedGamemodeSettings ToSaved()
        {
            var settings = new SavedGamemodeSettings();
            foreach(var setting in SettingByID)
            {
                settings.ValueByID[setting.Key] = setting.Value.Value;
            }
            return settings;
        }

        public void ApplySaved(SavedGamemodeSettings savedSettings)
        {
            foreach(var setting in SettingByID)
            {
                if (savedSettings.ValueByID.TryGetValue(setting.Key, out var result))
                {
                    var val = result;
                    if (val > setting.Value.MaxValue || val < setting.Value.MinValue)
                        val = setting.Value.Value;
                    setting.Value.Value = val;
                }
            }
        }

        public string GetDisplayString(bool host, bool inGame)
        {
            var str = "";
            foreach (var setting in SettingByID.Values)
            {
                if (setting.OnCheckVisibility != null && !setting.OnCheckVisibility()) continue;
                str += $"{setting.ToString()}\n";
            }
            if (!inGame) {
                if (!string.IsNullOrEmpty(str))
                    str += "\n";
                if (host)
                {
                    str += @"Use the Lobby menu in the Multiplayer phone app to start the game, change the mode or switch teams.";
                }
                else
                {
                    str += @"Use the Lobby menu in the Multiplayer phone app to toggle Ready or switch teams.";
                }
            }
            return str;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(SettingByID.Count);
            foreach(var setting in SettingByID)
            {
                writer.Write(setting.Key);
                setting.Value.Write(writer);
            }
        }

        public void Read(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            for(var i = 0; i < count; i++)
            {
                var key = reader.ReadInt32();
                if (SettingByID.TryGetValue(key, out var setting))
                {
                    setting.Read(reader);
                }
                else
                    return;
            }
        }
    }
}
