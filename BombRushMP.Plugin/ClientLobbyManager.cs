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
    public class ClientLobbyManager : IDisposable
    {
        public Lobby CurrentLobby
        {
            get
            {
                foreach(var lobby in Lobbies)
                {
                    if (lobby.Value.Players.Contains(_clientController.LocalID))
                        return lobby.Value;
                }
                return null;
            }
        }
        public Dictionary<uint, Lobby> Lobbies = new();
        public Action LobbiesUpdated;
        private ClientController _clientController;

        public ClientLobbyManager()
        {
            _clientController = ClientController.Instance;
            _clientController.PacketReceived += OnPacketReceived;
            _clientController.ServerDisconnect += OnDisconnect;
        }

        public void Dispose()
        {
            _clientController.PacketReceived -= OnPacketReceived;
            _clientController.ServerDisconnect -= OnDisconnect;
        }

        public void CreateLobby()
        {
            _clientController.SendPacket(new ClientLobbyCreate(), MessageSendMode.Reliable);
        }

        private void OnPacketReceived(Packets packetId, Packet packet)
        {
            switch (packetId)
            {
                case Packets.ServerLobbies:
                    var lobbies = (ServerLobbies)packet;
                    Lobbies = new();
                    foreach(var lobby in lobbies.Lobbies)
                    {
                        Lobbies[lobby.Id] = lobby;
                    }
                    Log($"Received {lobbies.Lobbies.Count} lobbies from server.");
                    LobbiesUpdated?.Invoke();
                    break;
            }
        }

        private void OnDisconnect()
        {
            Lobbies = new();
        }

        private void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] {message}");
        }
    }
}
