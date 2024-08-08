﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common;
using BombRushMP.Plugin.Gamemodes;

namespace BombRushMP.ServerApp.Gamemodes
{
    public static class GamemodeFactory
    {
        private static Dictionary<GamemodeIDs, Type> Gamemodes = new()
        {
            {GamemodeIDs.ScoreBattle, typeof(ScoreBattle) }
        };

        private static Dictionary<GamemodeIDs, string> GamemodeNames = new()
        {
            {GamemodeIDs.ScoreBattle, "Score Battle" }
        };

        public static Gamemode GetGamemode(GamemodeIDs gameModeID)
        {
            var instance = Activator.CreateInstance(Gamemodes[gameModeID]) as Gamemode;
            return instance;
        }

        public static string GetGamemodeName(GamemodeIDs gameModeID)
        {
            return GamemodeNames[gameModeID];
        }
    }
}
