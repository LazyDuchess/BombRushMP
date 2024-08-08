using BombRushMP.Common;
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
        public Lobby(LobbyState lobbyState)
        {
            LobbyState = lobbyState;
        }
    }
}
