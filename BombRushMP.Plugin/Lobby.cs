using BombRushMP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Plugin.Gamemodes;

namespace BombRushMP.Plugin
{
    public class Lobby
    {
        public LobbyState LobbyState = null;
        public Gamemode CurrentGamemode = null;

        public LobbyPlayer GetHighestScoringPlayer()
        {
            LobbyPlayer lastPlayer = null;
            foreach(var player in LobbyState.Players)
            {
                if (lastPlayer == null || player.Value.Score > lastPlayer.Score)
                    lastPlayer = player.Value;
            }
            return lastPlayer;
        }
    }
}
