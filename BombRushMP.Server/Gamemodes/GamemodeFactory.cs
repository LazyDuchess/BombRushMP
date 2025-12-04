using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common;

namespace BombRushMP.Server.Gamemodes
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
            {GamemodeIDs.TeamProSkaterScoreBattle, typeof(TeamProSkaterScoreBattle) },
            {GamemodeIDs.PropHunt, typeof(PropHunt) }
        };

        public static Gamemode GetGamemode(GamemodeIDs gameModeID)
        {
            var instance = Activator.CreateInstance(Gamemodes[gameModeID]) as Gamemode;
            return instance;
        }
    }
}
