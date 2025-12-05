using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common;
using UnityEngine;

namespace BombRushMP.Plugin.Gamemodes
{
    public static class GamemodeFactory
    {
        private static Team[] _defaultTeams =
        {
            new Team("Red Team", Color.red),
            new Team("Blue Team", Color.blue),
            new Team("Yellow Team", Color.yellow),
            new Team("Green Team", Color.green)
        };

        private static Team[] _propHuntTeams =
        {
            new Team("Hunters", Color.red),
            new Team("Props", Color.green)
        };

        private static Dictionary<GamemodeIDs, Type> Gamemodes = new()
        {
            {GamemodeIDs.ScoreBattle, typeof(ScoreBattle) },
            {GamemodeIDs.GraffitiRace, typeof(GraffitiRace) },
            {GamemodeIDs.TeamGraffitiRace, typeof(TeamGraffitiRace) },
            {GamemodeIDs.ProSkaterScoreBattle, typeof(ProSkaterScoreBattle) },
            {GamemodeIDs.TeamScoreBattle, typeof(TeamScoreBattle) },
            {GamemodeIDs.TeamProSkaterScoreBattle, typeof(TeamProSkaterScoreBattle) },
            {GamemodeIDs.PropHunt, typeof(PropHunt) }
        };

        private static Dictionary<GamemodeIDs, string> GamemodeNames = new()
        {
            {GamemodeIDs.ScoreBattle, "Score Battle" },
            {GamemodeIDs.GraffitiRace, "Graffiti Race" },
            {GamemodeIDs.TeamGraffitiRace, "Crew Graffiti Race" },
            {GamemodeIDs.ProSkaterScoreBattle, "Pro Skater Score Battle" },
            {GamemodeIDs.TeamScoreBattle, "Crew Score Battle" },
            {GamemodeIDs.TeamProSkaterScoreBattle, "Crew Pro Skater Score Battle" },
            {GamemodeIDs.PropHunt, "Prop Hunt" }
        };

        public static bool IsExclusive(GamemodeIDs gamemodeId)
        {
            if (gamemodeId == GamemodeIDs.PropHunt)
                return true;
            return false;
        }

        public static Team[] GetTeams(GamemodeIDs gamemodeId)
        {
            if (gamemodeId == GamemodeIDs.PropHunt)
                return _propHuntTeams;
            return _defaultTeams;
        }

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
