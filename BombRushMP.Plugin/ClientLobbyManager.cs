using BombRushMP.Common;
using BombRushMP.Common.Packets;
using BombRushMP.ServerApp.Gamemodes;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using BombRushMP.Plugin.Phone;

namespace BombRushMP.Plugin
{
    public class ClientLobbyManager : IDisposable
    {
        public Lobby CurrentLobby { get; private set; }
        public Dictionary<uint, Lobby> Lobbies = new();
        public Action LobbiesUpdated;
        public Action LobbyChanged;
        public List<uint> LobbiesInvited = new();
        private ClientController _clientController;
        private WorldHandler _worldHandler;

        public ClientLobbyManager()
        {
            _clientController = ClientController.Instance;
            _clientController.PacketReceived += OnPacketReceived;
            _clientController.ServerDisconnect += OnDisconnect;
            _worldHandler = WorldHandler.instance;
            LobbyChanged += HandleEncounter;
        }

        public void Dispose()
        {
            _clientController.PacketReceived -= OnPacketReceived;
            _clientController.ServerDisconnect -= OnDisconnect;
            if (CurrentLobby != null && CurrentLobby.InGame)
                CurrentLobby.CurrentGamemode.OnEnd(true);
            if (_worldHandler.currentEncounter != null && _worldHandler.currentEncounter is ProxyEncounter)
                _worldHandler.currentEncounter.SetEncounterState(Encounter.EncounterState.CLOSED);
            LobbyChanged -= HandleEncounter;
        }

        private void HandleEncounter()
        {
            if (CurrentLobby == null && _worldHandler.currentEncounter != null && _worldHandler.currentEncounter is ProxyEncounter)
            {
                _worldHandler.currentEncounter.SetEncounterState(Encounter.EncounterState.CLOSED);
            }
            else if (CurrentLobby != null && _worldHandler.currentEncounter == null)
            {
                ProxyEncounter.Instance.ActivateEncounter();
            }
        }

        public void OnUpdate()
        {
            if (CurrentLobby != null && CurrentLobby.InGame)
            {
                CurrentLobby.CurrentGamemode.OnUpdate_InGame();
            }
        }

        public bool CanJoinLobby()
        {
            if (_worldHandler.currentEncounter != null && _worldHandler.currentEncounter is not ProxyEncounter)
                return false;
            return true;
        }

        public void OnTick()
        {
            if (CurrentLobby != null && CurrentLobby.InGame)
            {
                CurrentLobby.CurrentGamemode.OnTick_InGame();
            }
        }

        public string GetLobbyName(uint lobbyId)
        {
            var lobby = Lobbies[lobbyId];
            return GamemodeFactory.GetGamemodeName(lobby.LobbyState.Gamemode);
        }

        public void CreateLobby()
        {
            if (!CanJoinLobby()) return;
            _clientController.SendPacket(new ClientLobbyCreate(), MessageSendMode.Reliable);
        }

        public void JoinLobby(uint lobbyId)
        {
            if (!CanJoinLobby()) return;
            _clientController.SendPacket(new ClientLobbyJoin(lobbyId), MessageSendMode.Reliable);
            NotificationController.Instance.RemoveNotificationForLobby(lobbyId);
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

        public void SetGamemode(GamemodeIDs gamemode)
        {
            _clientController.SendPacket(new ClientLobbySetGamemode(gamemode), MessageSendMode.Reliable);
        }

        public void InvitePlayer(ushort playerId)
        {
            _clientController.SendPacket(new ClientLobbyInvite(playerId), MessageSendMode.Reliable);
        }

        public void DeclineInvite(uint lobbyId)
        {
            _clientController.SendPacket(new ClientLobbyDeclineInvite(lobbyId), MessageSendMode.Reliable);
            NotificationController.Instance.RemoveNotificationForLobby(lobbyId);
        }

        public void DeclineAllInvites()
        {
            _clientController.SendPacket(new ClientLobbyDeclineAllInvites(), MessageSendMode.Reliable);
            NotificationController.Instance.RemoveAllNotifications();
        }

        public void KickPlayer(ushort playerId)
        {
            _clientController.SendPacket(new ClientLobbyKick(playerId), MessageSendMode.Reliable);
        }

        private void OnPacketReceived(Packets packetId, Packet packet)
        {
            if (CurrentLobby != null && CurrentLobby.InGame)
                CurrentLobby.CurrentGamemode.OnPacketReceived_InGame(packetId, packet);
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
            if (CurrentLobby.InGame)
            {
                CurrentLobby.CurrentGamemode.OnEnd(cancelled);
            }
            CurrentLobby.CurrentGamemode = null;
            CurrentLobby.LobbyState.InGame = false;
        }

        private void OnStartGame()
        {
            if (CurrentLobby.InGame)
            {
                CurrentLobby.CurrentGamemode.OnEnd(true);
            }
            CurrentLobby.CurrentGamemode = GamemodeFactory.GetGamemode(CurrentLobby.LobbyState.Gamemode);
            CurrentLobby.CurrentGamemode.Lobby = CurrentLobby;
            CurrentLobby.LobbyState.InGame = true;
            CurrentLobby.CurrentGamemode.OnStart();
            var player = _worldHandler.GetCurrentPlayer();
            PhoneUtility.BackToHomescreen(player.phone);
            player.phone.TurnOff();
        }

        private void OnLobbyDeleted(Lobby lobby)
        {
            if (lobby == CurrentLobby)
            {
                CurrentLobby = null;
                if (lobby.InGame)
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
            LobbiesInvited.Clear();
            foreach(var lobby in Lobbies)
            {
                if (lobby.Value == CurrentLobby) continue;
                if (lobby.Value.LobbyState.InvitedPlayers.Keys.Contains(_clientController.LocalID))
                    LobbiesInvited.Add(lobby.Key);
            }
            if (oldLobby != CurrentLobby)
            {
                if (oldLobby != null)
                {
                    if (oldLobby.InGame)
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
