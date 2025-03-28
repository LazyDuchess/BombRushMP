﻿using BombRushMP.Common;
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
        public bool InGame => CurrentGamemode != null && CurrentGamemode.InGame;

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

        public Gamemode GetOrCreateGamemode()
        {
            if (CurrentGamemode != null) return CurrentGamemode;
            return GamemodeFactory.GetGamemode(LobbyState.Gamemode);
        }
    }
}
