using BombRushMP.Common;
using BombRushMP.Common.Packets;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.ServerApp
{
    public class LobbyManager : IDisposable
    {
        private BRCServer _server = null;
        private UIDProvider _uidProvider = new();
        public LobbyManager()
        {
            _server = BRCServer.Instance;
            _server.PacketReceived += OnPacketReceived;
            _server.OnTick += OnTick;
        }

        public void Dispose()
        {
            _server.PacketReceived -= OnPacketReceived;
            _server.OnTick -= OnTick;
        }

        private void OnTick(float deltaTime)
        {

        }

        private void OnPacketReceived(Connection client, Packets packetId, Packet packet)
        {
            var playerId = client.Id;
            var player = _server.Players[playerId];
            switch (packetId)
            {
                case Packets.ClientLobbyCreate:
                    var lobby = new Lobby(_uidProvider.RequestUID(), client.Id);
                    Log($"Created Lobby with UID {lobby.Id} with host {player.ClientState.Name}");
                    break;
            }
        }

        private void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] {message}");
        }
    }
}
