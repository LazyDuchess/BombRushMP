using BombRushMP.Common;
using BombRushMP.Common.Packets;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin
{
    public class LobbyManager : IDisposable
    {
        public Lobby CurrentLobby
        {
            get
            {
                if (CurrentLobbyId == 0)
                    return null;
                if (!Lobbies.TryGetValue(CurrentLobbyId, out var lobby))
                    return null;
                return lobby;
            }
        }
        public ulong CurrentLobbyId = 0;
        public Dictionary<ulong, Lobby> Lobbies = new();
        private ClientController _clientController;

        public LobbyManager()
        {
            _clientController = ClientController.Instance;
            _clientController.PacketReceived += OnPacketReceived;
        }

        public void CreateLobby()
        {
            _clientController.SendPacket(new ClientLobbyCreate(), MessageSendMode.Reliable);
        }

        private void OnPacketReceived(Connection client, Packets packetId, Packet packet)
        {

        }

        public void Dispose()
        {
            _clientController.PacketReceived -= OnPacketReceived;
        }
    }
}
