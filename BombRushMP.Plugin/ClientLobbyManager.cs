using BombRushMP.Common;
using BombRushMP.Common.Packets;
using BombRushMP.ServerApp.Gamemodes;
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

        public void OnUpdate()
        {
            if (CurrentLobby != null && CurrentLobby.LobbyState.InGame && CurrentLobby.CurrentGamemode != null)
            {
                CurrentLobby.CurrentGamemode.OnUpdate();
            }
        }

        public void OnTick()
        {
            if (CurrentLobby != null && CurrentLobby.LobbyState.InGame && CurrentLobby.CurrentGamemode != null)
            {
                CurrentLobby.CurrentGamemode.OnTick();
            }
        }

        public string GetLobbyName(uint lobbyId)
        {
            var lobby = Lobbies[lobbyId];
            return GamemodeFactory.GetGamemodeName(lobby.LobbyState.Gamemode);
        }

        public void Dispose()
        {
            _clientController.PacketReceived -= OnPacketReceived;
            _clientController.ServerDisconnect -= OnDisconnect;
            if (CurrentLobby != null && CurrentLobby.CurrentGamemode != null)
                CurrentLobby.CurrentGamemode.OnEnd(true);
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

        public void StartGame()
        {
            _clientController.SendPacket(new ClientLobbyStart(), MessageSendMode.Reliable);
        }

        public void EndGame()
        {
            _clientController.SendPacket(new ClientLobbyEnd(), MessageSendMode.Reliable);
        }

        private void OnPacketReceived(Packets packetId, Packet packet)
        {
            if (CurrentLobby != null && CurrentLobby.CurrentGamemode != null)
                CurrentLobby.CurrentGamemode.OnPacketReceived(packetId, packet);
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

                case Packets.ServerLobbyStart:
                    {
                        OnStartGame();
                    }
                    break;

                case Packets.ServerLobbyEnd:
                    {
                        var endPacket = (ServerLobbyEnd)packet;
                        OnEndGame(endPacket.Cancelled);
                    }
                    break;
            }
        }

        private void OnDisconnect()
        {
            Lobbies.Clear();
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

        private void OnEndGame(bool cancelled)
        {
            if (CurrentLobby.CurrentGamemode != null)
            {
                CurrentLobby.CurrentGamemode.OnEnd(cancelled);
            }
            CurrentLobby.CurrentGamemode = null;
            CurrentLobby.LobbyState.InGame = false;
        }

        private void OnStartGame()
        {
            if (CurrentLobby.CurrentGamemode != null)
            {
                CurrentLobby.CurrentGamemode.OnEnd(true);
            }
            CurrentLobby.CurrentGamemode = GamemodeFactory.GetGamemode(GamemodeIDs.ScoreBattle);
            CurrentLobby.CurrentGamemode.Lobby = CurrentLobby;
            CurrentLobby.LobbyState.InGame = true;
            CurrentLobby.CurrentGamemode.OnStart();
        }

        private void OnLobbyDeleted(Lobby lobby)
        {
            if (lobby == CurrentLobby)
            {
                CurrentLobby = null;
                if (lobby.CurrentGamemode != null)
                {
                    lobby.CurrentGamemode.OnEnd(true);
                }
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
            {
                if (oldLobby != null)
                {
                    if (oldLobby.CurrentGamemode != null)
                    {
                        oldLobby.CurrentGamemode.OnEnd(true);
                        oldLobby.CurrentGamemode = null;
                    }
                }
                LobbyChanged?.Invoke();
            }
            LobbiesUpdated?.Invoke();
        }
    }
}
