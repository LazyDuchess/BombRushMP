using BombRushMP.Common;
using BombRushMP.ServerApp.Gamemodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.ServerApp
{
    public class Lobby
    {
        public LobbyState LobbyState = null;
        public Gamemode CurrentGamemode = null;

        public Lobby(LobbyState lobbyState)
        {
            LobbyState = lobbyState;
        }

        public void Tick(float deltaTime)
        {
            if (CurrentGamemode != null && LobbyState.InGame)
                CurrentGamemode.Tick(deltaTime);
        }
    }
}
