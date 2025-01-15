using BombRushMP.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Server.Gamemodes
{
    public abstract class Gamemode
    {
        public Lobby Lobby;
        protected int CountdownTime = 10;
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

        public virtual void OnEnd(bool cancelled)
        {

        }

        public virtual void Tick(float deltaTime)
        {

        }

        public virtual void OnPacketFromLobbyReceived(Packets packetId, Packet packet, ushort playerId)
        {
            if (packetId == Packets.ClientGamemodeCountdown && playerId == Lobby.LobbyState.HostId)
            {
                CountdownTime = (packet as ClientGamemodeCountdown).CountdownSeconds;
            }
        }
    }
}
