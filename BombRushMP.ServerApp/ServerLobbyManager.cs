﻿using BombRushMP.Common;
using BombRushMP.Common.Packets;
using BombRushMP.ServerApp.Gamemodes;
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
        private List<Action> _queuedActions = new();

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

        public void StartGame(uint lobbyId)
        {
            var lobby = Lobbies[lobbyId];
            lobby.CurrentGamemode = GamemodeFactory.GetGamemode(lobby.LobbyState.Gamemode);
            lobby.CurrentGamemode.Lobby = lobby;
            lobby.LobbyState.InGame = true;
            foreach(var player in lobby.LobbyState.Players)
            {
                player.Value.Score = 0;
                player.Value.Ready = false;
            }
            ClearAllInvitesForLobby(lobbyId);
            SendLobbiesToStage(lobby.LobbyState.Stage);
            SendPacketToLobby(new ServerLobbyStart(), MessageSendMode.Reliable, lobbyId);
            lobby.CurrentGamemode.OnStart();
        }

        public void EndGame(uint lobbyId, bool cancelled)
        {
            var lobby = Lobbies[lobbyId];
            lobby.LobbyState.InGame = false;
            var gamemode = lobby.CurrentGamemode;
            lobby.CurrentGamemode = null;
            SendLobbiesToStage(lobby.LobbyState.Stage);
            SendPacketToLobby(new ServerLobbyEnd(cancelled), MessageSendMode.Reliable, lobbyId);
            gamemode.OnEnd(cancelled);
        }

        public void SetPlayerScore(ushort clientId, float score)
        {
            var existingLobby = GetLobbyPlayerIsIn(clientId);
            existingLobby.LobbyState.Players[clientId].Score = score;
            SendLobbiesToStage(existingLobby.LobbyState.Stage);
        }

        public Lobby GetLobbyPlayerIsIn(ushort clientId)
        {
            foreach(var lobby in Lobbies)
            {
                if (lobby.Value.LobbyState.Players.Keys.Contains(clientId))
                    return lobby.Value;
            }
            return null;
        }

        public void DeleteLobby(uint lobbyId)
        {
            if (Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                if (lobby.CurrentGamemode != null)
                    EndGame(lobbyId, true);
                Lobbies.Remove(lobbyId);
                _uidProvider.FreeUID(lobbyId);
                ServerLogger.Log($"Deleted Lobby with UID {lobbyId}");
                _server.SendPacketToStage(new ServerLobbyDeleted(lobby.LobbyState.Id), MessageSendMode.Reliable, lobby.LobbyState.Stage);
            }
        }

        public void AddPlayer(uint lobbyId, ushort clientId)
        {
            if (Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                lobby.LobbyState.Players[clientId] = new LobbyPlayer(lobbyId, clientId);
                UninvitePlayer(lobbyId, clientId);
                QueueStageUpdate(lobby.LobbyState.Stage);
            }
        }

        public void RemovePlayer(uint lobbyId, ushort clientId)
        {
            if (Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                lobby.LobbyState.Players.Remove(clientId);
                if (lobby.LobbyState.HostId == clientId)
                    DeleteLobby(lobbyId);
                QueueStageUpdate(lobby.LobbyState.Stage);
            }
        }

        public bool InvitePlayer(uint lobbyId, ushort inviteeId)
        {
            if (Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                if (lobby.LobbyState.Players.ContainsKey(inviteeId)) return false;
                if (lobby.LobbyState.InGame) return false;
                if (!lobby.LobbyState.InvitedPlayers.ContainsKey(inviteeId))
                {
                    lobby.LobbyState.InvitedPlayers[inviteeId] = DateTime.UtcNow;
                    QueueStageUpdate(lobby.LobbyState.Stage);
                    return true;
                }
            }
            return false;
        }

        public void UninvitePlayer(uint lobbyId, ushort inviteeId)
        {
            if (Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                if (lobby.LobbyState.InvitedPlayers.ContainsKey(inviteeId))
                {
                    lobby.LobbyState.InvitedPlayers.Remove(inviteeId);
                    QueueStageUpdate(lobby.LobbyState.Stage);
                }
            }
        }

        public void ClearAllInvitesForLobby(uint lobbyId)
        {
            if (Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                lobby.LobbyState.InvitedPlayers.Clear();
                QueueStageUpdate(lobby.LobbyState.Stage);
            }
        }

        public void ClearAllInvitesForPlayer(ushort playerId)
        {
            foreach (var lobby in Lobbies)
            {
                if (lobby.Value.LobbyState.InvitedPlayers.ContainsKey(playerId))
                {
                    lobby.Value.LobbyState.InvitedPlayers.Remove(playerId);
                    QueueStageUpdate(lobby.Value.LobbyState.Stage);
                }
            }
        }

        private void OnTick(float deltaTime)
        {
            foreach (var stage in _queuedStageUpdates)
                SendLobbiesToStage(stage);
            _queuedStageUpdates.Clear();
            foreach (var action in _queuedActions)
                action();
            _queuedActions.Clear();
            foreach (var lobby in Lobbies)
                lobby.Value.Tick(deltaTime);
        }

        private void OnClientHandshook(Connection client)
        {
            SendLobbiesToClient(client);
        }

        private void OnClientDisconnected(Connection client)
        {
            ClearAllInvitesForPlayer(client.Id);
            var lobby = GetLobbyPlayerIsIn(client.Id);
            if (lobby != null)
            {
                RemovePlayer(lobby.LobbyState.Id, client.Id);
            }
        }

        private void OnPacketReceived(Connection client, Packets packetId, Packet packet)
        {
            var playerId = client.Id;
            var player = _server.Players[playerId];
            if (player.ClientState == null) return;

            var existingLobby = GetLobbyPlayerIsIn(client.Id);
            if (existingLobby != null && existingLobby.CurrentGamemode != null)
                existingLobby.CurrentGamemode.OnPacketFromLobbyReceived(packetId, packet, playerId);

            switch (packetId)
            {
                case Packets.ClientLobbySetGamemode:
                    {
                        if (existingLobby == null)
                            break;

                        if (existingLobby.LobbyState.HostId != playerId)
                            break;

                        if (existingLobby.LobbyState.InGame)
                            break;

                        if (existingLobby.CurrentGamemode != null)
                            break;

                        existingLobby.LobbyState.Gamemode = ((ClientLobbySetGamemode)packet).Gamemode;
                        QueueStageUpdate(existingLobby.LobbyState.Stage);
                    }
                    break;

                case Packets.ClientLobbyCreate:
                    {
                        if (existingLobby != null)
                            RemovePlayer(existingLobby.LobbyState.Id, client.Id);

                        var lobbyState = new LobbyState(player.ClientState.Stage, _uidProvider.RequestUID(), client.Id);
                        var lobby = new Lobby(lobbyState);
                        Lobbies[lobby.LobbyState.Id] = lobby;
                        AddPlayer(lobby.LobbyState.Id, client.Id);
                        ServerLogger.Log($"Created Lobby with UID {lobby.LobbyState.Id} with host {player.ClientState.Name}");
                        QueueAction(() => { _server.SendPacketToClient(new ServerLobbyCreateResponse(), MessageSendMode.Reliable, client); });
                    }
                    break;

                case Packets.ClientLobbyJoin:
                    {
                        var lobbyPacket = (ClientLobbyJoin)packet;

                        if (!Lobbies.TryGetValue(lobbyPacket.LobbyId, out var lobby))
                            break;

                        if (lobby.LobbyState.InGame) 
                            break;

                        if (existingLobby == lobby)
                            break;

                        if (existingLobby != null)
                            RemovePlayer(existingLobby.LobbyState.Id, playerId);

                        AddPlayer(lobbyPacket.LobbyId, playerId);
                        ServerLogger.Log($"{_server.Players[playerId].ClientState.Name} joined lobby UID {lobby.LobbyState.Id}. Now at {lobby.LobbyState.Players.Count} players. Hosted by {_server.Players[lobby.LobbyState.HostId].ClientState.Name}.");
                    }
                    break;

                case Packets.ClientLobbyLeave:
                    {
                        if (existingLobby != null)
                            RemovePlayer(existingLobby.LobbyState.Id, playerId);
                    }
                    break;

                case Packets.ClientLobbyStart:
                    {
                        if (existingLobby != null && existingLobby.LobbyState.HostId == playerId && existingLobby.LobbyState.InGame == false)
                            StartGame(existingLobby.LobbyState.Id);
                    }
                    break;

                case Packets.ClientLobbyEnd:
                    {
                        if (existingLobby != null && existingLobby.LobbyState.HostId == playerId && existingLobby.LobbyState.InGame == true)
                            EndGame(existingLobby.LobbyState.Id, true);
                    }
                    break;

                case Packets.ClientLobbySetReady:
                    {
                        if (existingLobby != null)
                        {
                            existingLobby.LobbyState.Players[playerId].Ready = ((ClientLobbySetReady)packet).Ready;
                            QueueStageUpdate(existingLobby.LobbyState.Stage);
                        }
                    }
                    break;

                case Packets.ClientLobbyInvite:
                    {
                        if (existingLobby != null && existingLobby.LobbyState.HostId == playerId)
                        {
                            var invitePacket = (ClientLobbyInvite)packet;
                            if (InvitePlayer(existingLobby.LobbyState.Id, invitePacket.InviteeId))
                                _server.SendPacketToClient(new ServerLobbyInvite(invitePacket.InviteeId, playerId, existingLobby.LobbyState.Id), MessageSendMode.Reliable, _server.Players[invitePacket.InviteeId].Client);
                        }
                    }
                    break;

                case Packets.ClientLobbyDeclineInvite:
                    {
                        var declinePacket = (ClientLobbyDeclineInvite)packet;
                        UninvitePlayer(declinePacket.LobbyId, playerId);
                    }
                    break;

                case Packets.ClientLobbyDeclineAllInvites:
                    {
                        ClearAllInvitesForPlayer(playerId);
                    }
                    break;

                case Packets.ClientLobbyKick:
                    {
                        if (existingLobby != null && existingLobby.LobbyState.HostId == playerId)
                        {
                            var kickPacket = (ClientLobbyKick)packet;
                            var playerToKick = kickPacket.PlayerId;
                            if (existingLobby.LobbyState.Players.ContainsKey(playerToKick))
                            {
                                RemovePlayer(existingLobby.LobbyState.Id, playerToKick);
                            }
                        }
                    }
                    break;
            }
        }

        public void SendPacketToLobby(Packet packet, MessageSendMode sendMode, uint lobbyId)
        {
            if (Lobbies.TryGetValue(lobbyId, out var lobby))
            {
                foreach (var player in lobby.LobbyState.Players)
                {
                    _server.SendPacketToClient(packet, sendMode, _server.Players[player.Key].Client);
                }
            }
        }

        private void SendLobbiesToClient(Connection client)
        {
            var player = _server.Players[client.Id];

            if (_queuedStageUpdates.Contains(player.ClientState.Stage)) return;

            var lobbies = CreateServerLobbyStatesPacket(player.ClientState.Stage);
            _server.SendPacketToClient(lobbies, MessageSendMode.Reliable, client);
        }

        private void QueueStageUpdate(int stage)
        {
            _queuedStageUpdates.Add(stage);
        }

        private void QueueAction(Action action)
        {
            _queuedActions.Add(action);
        }

        private void SendLobbiesToStage(int stage)
        {
            var lobbies = CreateServerLobbyStatesPacket(stage);
            _server.SendPacketToStage(lobbies, MessageSendMode.Reliable, stage);
        }

        private List<LobbyState> GetLobbyStatesForStage(int stage)
        {
            var lobbyStates = new List<LobbyState>();
            foreach (var lobby in Lobbies)
            {
                if (lobby.Value.LobbyState.Stage != stage) continue;
                lobbyStates.Add(lobby.Value.LobbyState);
            }
            return lobbyStates;
        }

        private Packet CreateServerLobbyStatesPacket(int stage)
        {
            
            return new ServerLobbies(GetLobbyStatesForStage(stage));
        }
    }
}
