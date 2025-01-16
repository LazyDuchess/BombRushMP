using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common;

namespace BombRushMP.Plugin.Gamemodes
{
    public static class GamemodeFactory
    {
        private static Dictionary<GamemodeIDs, Type> Gamemodes = new()
        {
            {GamemodeIDs.ScoreBattle, typeof(ScoreBattle) },
            {GamemodeIDs.GraffitiRace, typeof(GraffitiRace) },
            {GamemodeIDs.TeamGraffitiRace, typeof(TeamGraffitiRace) },
            {GamemodeIDs.ProSkaterScoreBattle, typeof(ProSkaterScoreBattle) },
            {GamemodeIDs.TeamScoreBattle, typeof(TeamScoreBattle) },
            {GamemodeIDs.TeamProSkaterScoreBattle, typeof(TeamProSkaterScoreBattle) }
        };

        private static Dictionary<GamemodeIDs, string> GamemodeNames = new()
        {
            {GamemodeIDs.ScoreBattle, "Score Battle" },
            {GamemodeIDs.GraffitiRace, "Graffiti Race" },
            {GamemodeIDs.TeamGraffitiRace, "Team Graffiti Race" },
            {GamemodeIDs.ProSkaterScoreBattle, "Pro Skater Score Battle" },
            {GamemodeIDs.TeamScoreBattle, "Team Score Battle" },
            {GamemodeIDs.TeamProSkaterScoreBattle, "Team Pro Skater Score Battle" }
        };

        public static Gamemode GetGamemode(GamemodeIDs gameModeID)
        {
            var instance = Activator.CreateInstance(Gamemodes[gameModeID]) as Gamemode;
            return instance;
        }

        public static GamemodeSettings GetGamemodeSettings(GamemodeIDs gameModeID)
        {
            var gm = GetGamemode(gameModeID);
            return gm.GetDefaultSettings();
        }

        public static GamemodeSettings ParseGamemodeSettings(GamemodeIDs gameModeID, byte[] serializedSettings)
        {
            var gm = GetGamemode(gameModeID);
            var sets = gm.GetDefaultSettings();
            if (serializedSettings.Length > 0)
            {
                using (var ms = new MemoryStream(serializedSettings))
                {
                    using (var reader = new BinaryReader(ms))
                    {
                        sets.Read(reader);
                    }
                }
            }
            return sets;
        }

        public static string GetGamemodeName(GamemodeIDs gameModeID)
        {
            return GamemodeNames[gameModeID];
        }
    }
}
