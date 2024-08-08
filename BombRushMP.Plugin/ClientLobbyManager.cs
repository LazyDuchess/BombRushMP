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
        public Lobby CurrentLobby { get; private set; }
        public Dictionary<uint, Lobby> Lobbies = new();
        public Action LobbiesUpdated;
        private ClientController _clientController;

        public ClientLobbyManager()
        {
            _clientController = ClientController.Instance;
            _clientController.PacketReceived += OnPacketReceived;
            _clientController.ServerDisconnect += OnDisconnect;
        }

        public string GetLobbyName(uint lobbyId)
        {
            var lobby = Lobbies[lobbyId];
            return $"Freeroam";
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

        public void JoinLobby(uint lobbyId)
        {
            _clientController.SendPacket(new ClientLobbyJoin(lobbyId), MessageSendMode.Reliable);
        }

        public void LeaveLobby()
        {
            _clientController.SendPacket(new ClientLobbyLeave(), MessageSendMode.Reliable);
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
                    ClientLogger.Log($"Received {lobbies.Lobbies.Count} lobbies from server.");
                    OnLobbiesUpdated();
                    break;
            }
        }

        private void OnDisconnect()
        {
            Lobbies = new();
            OnLobbiesUpdated();
        }

        private void OnLobbiesUpdated()
        {
            CurrentLobby = null;
            foreach (var lobby in Lobbies)
            {
                if (lobby.Value.Players.Keys.Contains(_clientController.LocalID))
                    CurrentLobby = lobby.Value;
            }
            LobbiesUpdated?.Invoke();
        }
    }
}
