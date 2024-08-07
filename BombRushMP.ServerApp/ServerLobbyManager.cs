using BombRushMP.Common;
using BombRushMP.Common.Packets;
using Riptide;
using Riptide.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.ServerApp
{
    public class ServerLobbyManager : IDisposable
    {
        public Dictionary<uint, Lobby> Lobbies = new();
        private BRCServer _server = null;
        private UIDProvider _uidProvider = new();
        private HashSet<int> _queuedStageUpdates = new();

        public ServerLobbyManager()
        {
            _server = BRCServer.Instance;
            _server.PacketReceived += OnPacketReceived;
            _server.OnTick += OnTick;
            _server.ClientDisconnected += OnClientDisconnected;
            _server.ClientHandshook += OnClientHandshook;
        }

        public void Dispose()
        {
            _server.PacketReceived -= OnPacketReceived;
            _server.OnTick -= OnTick;
            _server.ClientDisconnected -= OnClientDisconnected;
            _server.ClientHandshook -= OnClientHandshook;
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
                ServerLogger.Log($"Deleted Lobby with UID {lobbyId}");
                QueueStageUpdate(lobby.Stage);
            }
        }

        public void AddPlayer(uint lobbyId, ushort clientId)
        {
            if (Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                lobby.Players.Add(clientId);
                QueueStageUpdate(lobby.Stage);
            }
        }

        public void RemovePlayer(uint lobbyId, ushort clientId)
        {
            if (Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                lobby.Players.Remove(clientId);
                if (lobby.HostId == clientId)
                    DeleteLobby(lobbyId);
                QueueStageUpdate(lobby.Stage);
            }
        }

        private void OnTick(float deltaTime)
        {
            foreach (var stage in _queuedStageUpdates)
                SendLobbiesToStage(stage);
            _queuedStageUpdates.Clear();
        }

        private void OnClientHandshook(Connection client)
        {
            SendLobbiesToClient(client);
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
            if (player.ClientState == null) return;
            switch (packetId)
            {
                case Packets.ClientLobbyCreate:
                    var existingLobby = GetLobbyPlayerIsIn(client.Id);

                    if (existingLobby != null)
                        RemovePlayer(existingLobby.Id, client.Id);

                    var lobby = new Lobby(player.ClientState.Stage, _uidProvider.RequestUID(), client.Id);
                    Lobbies[lobby.Id] = lobby;
                    AddPlayer(lobby.Id, client.Id);
                    ServerLogger.Log($"Created Lobby with UID {lobby.Id} with host {player.ClientState.Name}");
                    break;
            }
        }

        private void SendLobbiesToClient(Connection client)
        {
            var player = _server.Players[client.Id];

            if (_queuedStageUpdates.Contains(player.ClientState.Stage)) return;

            var lobbies = CreateServerLobbiesPacket(player.ClientState.Stage);
            _server.SendPacketToClient(lobbies, MessageSendMode.Reliable, client);
        }

        private void QueueStageUpdate(int stage)
        {
            _queuedStageUpdates.Add(stage);
        }

        private void SendLobbiesToStage(int stage)
        {
            var lobbies = CreateServerLobbiesPacket(stage);
            _server.SendPacketToStage(lobbies, MessageSendMode.Reliable, stage);
        }

        private List<Lobby> GetLobbiesForStage(int stage)
        {
            return Lobbies.Values.Where(x => x.Stage == stage).ToList();
        }

        private Packet CreateServerLobbiesPacket(int stage)
        {
            return new ServerLobbies(GetLobbiesForStage(stage));
        }
    }
}
