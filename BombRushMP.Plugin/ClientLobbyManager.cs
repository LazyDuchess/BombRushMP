﻿using BombRushMP.Common;
using BombRushMP.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using BombRushMP.Plugin.Phone;
using BombRushMP.Common.Networking;
using BombRushMP.Plugin.Gamemodes;
using System.IO;

namespace BombRushMP.Plugin
{
    public class ClientLobbyManager : IDisposable
    {
        public static Action LobbiesUpdated;
        public static Action LobbyChanged;
        public Lobby CurrentLobby { get; private set; }
        public Dictionary<uint, Lobby> Lobbies = new();
        public List<uint> LobbiesInvited = new();
        private ClientController _clientController;
        private WorldHandler _worldHandler;

        public ClientLobbyManager()
        {
            _clientController = ClientController.Instance;
            ClientController.PacketReceived += OnPacketReceived;
            ClientController.ServerDisconnect += OnDisconnect;
            _worldHandler = WorldHandler.instance;
            LobbyChanged += HandleEncounter;
        }

        public void Dispose()
        {
            ClientController.PacketReceived -= OnPacketReceived;
            ClientController.ServerDisconnect -= OnDisconnect;
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
            _clientController.SendPacket(new ClientLobbyCreate(), IMessage.SendModes.ReliableUnordered);
        }

        public void JoinLobby(uint lobbyId)
        {
            if (!CanJoinLobby()) return;
            _clientController.SendPacket(new ClientLobbyJoin(lobbyId), IMessage.SendModes.ReliableUnordered);
            NotificationController.Instance.RemoveNotificationForLobby(lobbyId);
        }

        public void LeaveLobby()
        {
            _clientController.SendPacket(new ClientLobbyLeave(), IMessage.SendModes.ReliableUnordered);
        }

        public void StartGame()
        {
            _clientController.SendPacket(new ClientLobbyStart(), IMessage.SendModes.ReliableUnordered);
        }

        public void EndGame()
        {
            _clientController.SendPacket(new ClientLobbyEnd(), IMessage.SendModes.ReliableUnordered);
        }

        public void SetGamemode(GamemodeIDs gamemode, GamemodeSettings settings)
        {
            byte[] data;
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    settings.Write(writer);
                    data = ms.ToArray();
                }
            }
            _clientController.SendPacket(new ClientLobbySetGamemode(gamemode, data), IMessage.SendModes.ReliableUnordered);
        }

        public void InvitePlayer(ushort playerId)
        {
            _clientController.SendPacket(new ClientLobbyInvite(playerId), IMessage.SendModes.ReliableUnordered);
        }

        public void DeclineInvite(uint lobbyId)
        {
            _clientController.SendPacket(new ClientLobbyDeclineInvite(lobbyId), IMessage.SendModes.ReliableUnordered);
            NotificationController.Instance.RemoveNotificationForLobby(lobbyId);
        }

        public void DeclineAllInvites()
        {
            _clientController.SendPacket(new ClientLobbyDeclineAllInvites(), IMessage.SendModes.ReliableUnordered);
            NotificationController.Instance.RemoveAllNotifications();
        }

        public void KickPlayer(ushort playerId)
        {
            _clientController.SendPacket(new ClientLobbyKick(playerId), IMessage.SendModes.ReliableUnordered);
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
