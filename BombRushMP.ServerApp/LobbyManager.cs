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
        public Dictionary<uint, Lobby> Lobbies = new();
        private BRCServer _server = null;
        private UIDProvider _uidProvider = new();
        public LobbyManager()
        {
            _server = BRCServer.Instance;
            _server.PacketReceived += OnPacketReceived;
            _server.OnTick += OnTick;
            _server.ClientDisconnected += OnClientDisconnected;
        }

        public Lobby GetLobbyPlayerIsIn(ushort clientId)
        {
            foreach(var lobby in Lobbies)
            {
                if (lobby.Value.Players.Contains(clientId))
                    return lobby.Value;
            }
            return null;
        }

        public void DeleteLobby(uint lobbyId)
        {
            if (Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                Lobbies.Remove(lobbyId);
                _uidProvider.FreeUID(lobbyId);
                Log($"Deleted Lobby with UID {lobbyId}");
            }
        }

        public void AddPlayer(uint lobbyId, ushort clientId)
        {
            if (Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                lobby.Players.Add(clientId);
            }
        }

        public void RemovePlayer(uint lobbyId, ushort clientId)
        {
            if (Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                lobby.Players.Remove(clientId);
                if (lobby.HostId == clientId)
                    DeleteLobby(lobbyId);
            }
        }

        public void Dispose()
        {
            _server.PacketReceived -= OnPacketReceived;
            _server.OnTick -= OnTick;
            _server.ClientDisconnected -= OnClientDisconnected;
        }

        private void OnTick(float deltaTime)
        {

        }

        private void OnClientDisconnected(Connection client)
        {
            var lobby = GetLobbyPlayerIsIn(client.Id);
            if (lobby != null)
            {
                RemovePlayer(lobby.Id, client.Id);
            }
        }

        private void OnPacketReceived(Connection client, Packets packetId, Packet packet)
        {
            var playerId = client.Id;
            var player = _server.Players[playerId];
            switch (packetId)
            {
                case Packets.ClientLobbyCreate:
                    var existingLobby = GetLobbyPlayerIsIn(client.Id);

                    if (existingLobby != null)
                        RemovePlayer(existingLobby.Id, client.Id);

                    var lobby = new Lobby(_uidProvider.RequestUID(), client.Id);
                    Lobbies[lobby.Id] = lobby;
                    AddPlayer(lobby.Id, client.Id);
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
