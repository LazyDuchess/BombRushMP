using BombRushMP.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Gamemodes
{
    public class Gamemode
    {
        public Lobby Lobby;
        protected ClientController ClientController;
        protected ClientLobbyManager ClientLobbyManager;

        public Gamemode()
        {
            ClientController = ClientController.Instance;
            ClientLobbyManager = ClientController.ClientLobbyManager;
        }

        public virtual void OnStart()
        {

        }

        public virtual void OnEnd(bool cancelled)
        {

        }

        public virtual void OnUpdate()
        {

        }

        public virtual void OnTick()
        {

        }

        public virtual void OnPacketReceived(Packets packetId, Packet packet)
        {

        }
    }
}
