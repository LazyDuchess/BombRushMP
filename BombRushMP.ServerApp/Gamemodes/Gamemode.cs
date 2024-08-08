using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.ServerApp.Gamemodes
{
    public abstract class Gamemode
    {
        public Lobby Lobby;
        protected BRCServer Server;
        protected ServerLobbyManager ServerLobbyManager;

        public Gamemode()
        {
            Server = BRCServer.Instance;
            ServerLobbyManager = Server.ServerLobbyManager;
        }

        public virtual void OnStart()
        {

        }

        public virtual void OnEnd()
        {

        }

        public virtual void Tick()
        {

        }
    }
}
