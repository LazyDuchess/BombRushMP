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
        public MinimapOverrideModes MinimapOverrideMode = MinimapOverrideModes.None;
        public Lobby Lobby;
        public bool InGame { get; private set; }
        protected ClientController ClientController;
        protected ClientLobbyManager ClientLobbyManager;

        public Gamemode()
        {
            ClientController = ClientController.Instance;
            ClientLobbyManager = ClientController.ClientLobbyManager;
        }

        public virtual void OnStart()
        {
            InGame = true;
        }

        public virtual void OnEnd(bool cancelled)
        {
            InGame = false;
        }

        public virtual void OnUpdate_InGame()
        {

        }

        public virtual void OnTick_InGame()
        {

        }

        public virtual void OnPacketReceived_InGame(Packets packetId, Packet packet)
        {

        }
    }
}
