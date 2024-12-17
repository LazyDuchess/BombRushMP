using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common;

namespace BombRushMP.ServerApp.Gamemodes
{
    public static class GamemodeFactory
    {
        private static Dictionary<GamemodeIDs, Type> Gamemodes = new()
        {
            {GamemodeIDs.ScoreBattle, typeof(ScoreBattle) },
            {GamemodeIDs.GraffitiRace, typeof(GraffitiRace) },
            {GamemodeIDs.ProSkaterScoreBattle, typeof(ProSkaterScoreBattle) }
        };

        public static Gamemode GetGamemode(GamemodeIDs gameModeID)
        {
            var instance = Activator.CreateInstance(Gamemodes[gameModeID]) as Gamemode;
            return instance;
        }
    }
}
