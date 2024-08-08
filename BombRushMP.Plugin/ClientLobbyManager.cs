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
        public Action LobbyChanged;
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
                case Packets.ServerLobbyDeleted:
                    {
                        var serverpacket = (ServerLobbyDeleted)packet;
                        if (Lobbies.TryGetValue(serverpacket.LobbyUID, out var lobby))
                        {
                            Lobbies.Remove(serverpacket.LobbyUID);
                            OnLobbyDeleted(lobby);
                        }
                    }
                    break;

                case Packets.ServerLobbies:
                    {
                        var lobbies = (ServerLobbies)packet;
                        foreach (var lobbyState in lobbies.Lobbies)
                        {
                            if (!Lobbies.TryGetValue(lobbyState.Id, out var lobby))
                            {
                                lobby = new Lobby();
                                Lobbies[lobbyState.Id] = lobby;
                            }
                            lobby.LobbyState = lobbyState;
                        }
                    }
                    OnLobbiesUpdated();
                    break;
            }
        }

        private void OnDisconnect()
        {
            Lobbies = new();
            OnLobbiesUpdated();
        }

        private void UpdateCurrentLobby()
        {
            CurrentLobby = null;
            foreach (var lobby in Lobbies)
            {
                if (lobby.Value.LobbyState.Players.Keys.Contains(_clientController.LocalID))
                    CurrentLobby = lobby.Value;
            }
        }

        private void OnLobbyDeleted(Lobby lobby)
        {
            if (lobby == CurrentLobby)
            {
                CurrentLobby = null;
                LobbyChanged?.Invoke();
            }
            UpdateCurrentLobby();
            LobbiesUpdated?.Invoke();
        }

        private void OnLobbiesUpdated()
        {
            var oldLobby = CurrentLobby;
            UpdateCurrentLobby();
            if (oldLobby != CurrentLobby)
                LobbyChanged?.Invoke();
            LobbiesUpdated?.Invoke();
        }
    }
}
