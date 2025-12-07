using BombRushMP.Common;
using BombRushMP.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Server.Gamemodes
{
    public class PropHunt : Gamemode
    {
        public enum States
        {
            Setup,
            Main,
            Finished
        }
        private States _state = States.Setup;
        private float _stateTimer = 0f;
        private float _pingTimer = 0f;

        private float _setupTime = 9999f;
        private float _matchTime = 9999f;
        private float _pingInterval = 99999f;
        private int _stageHash = 0;
        private bool _validatedHashes = false;
        private bool _becomeHunterOnKill = false;
        private bool _respawnOnKill = false;
        private float _respawnTime = 5f;

        private Dictionary<ushort, int> _stageHashes = new();
        private HashSet<ushort> _deadPlayers = new();

        public PropHunt() : base()
        {
            TeamBased = true;
        }

        public override void OnStart()
        {
            var host = Server.Players[Lobby.LobbyState.HostId];
            var spawnPacket = new ServerPropHuntSpawn(host.ClientVisualState.Position, host.ClientVisualState.Rotation);
            ServerLobbyManager.SendPacketToLobby(spawnPacket, Common.Networking.IMessage.SendModes.Reliable, Lobby.LobbyState.Id, Common.Networking.NetChannels.Gamemodes);
        }

        public override void Tick(float deltaTime)
        {
            switch (_state)
            {
                case States.Setup:
                    if (_stateTimer >= 5f && !_validatedHashes)
                    {
                        ValidateStageHashes();
                    }
                    if (_stateTimer >= _setupTime)
                    {
                        SetState(States.Main);
                    }
                    //CheckForSetupCancellation();
                    break;

                case States.Main:
                    if (_pingTimer >= _pingInterval)
                    {
                        _pingTimer = 0f;
                        ServerLobbyManager.SendPacketToLobby(new ServerPropHuntPing(), Common.Networking.IMessage.SendModes.ReliableUnordered, Lobby.LobbyState.Id, Common.Networking.NetChannels.Gamemodes);
                    }
                    if (_stateTimer >= _matchTime)
                    {
                        ServerLobbyManager.EndGame(Lobby.LobbyState.Id, false);
                        SetState(States.Finished);
                    }
                    _pingTimer += deltaTime;
                    //CheckWinConditions();
                    UpdatePlayerRespawn(deltaTime);
                    break;
            }
            _stateTimer += deltaTime;
        }

        private void CheckWinConditions()
        {
            var hunters = 0;
            var props = 0;
            foreach (var player in Lobby.LobbyState.Players)
            {
                if (_deadPlayers.Contains(player.Key)) continue;
                if (player.Value.Team == 0)
                {
                    hunters++;
                }
                else if (player.Value.Team == 1)
                {
                    props++;
                }
            }
            if (props == 0)
            {
                ServerLobbyManager.EndGame(Lobby.LobbyState.Id, false);
                SendSystemMessage("Hunters win!");
            }
            else if (hunters == 0)
            {
                ServerLobbyManager.EndGame(Lobby.LobbyState.Id, false);
                SendSystemMessage("Props win!");
            }
        }

        private void ValidateStageHashes()
        {
            foreach(var player in Lobby.LobbyState.Players)
            {
                if (player.Key == Lobby.LobbyState.HostId) continue;
                if (_stageHashes.TryGetValue(player.Key, out var hash))
                {
                    if (hash != _stageHash)
                    {
                        SendPlayerMessage(player.Key, "{0} has been removed due to a stage mismatch.");
                        ServerLobbyManager.RemovePlayer(Lobby.LobbyState.Id, player.Key);
                    }
                }
                else
                {
                    SendPlayerMessage(player.Key, "{0} timed out of sending stage info.");
                    ServerLobbyManager.RemovePlayer(Lobby.LobbyState.Id, player.Key);
                }
            }
            _validatedHashes = true;
        }

        private void SendSystemMessage(string txt)
        {
            ServerLobbyManager.SendPacketToLobby(new ServerChat($"<color=yellow>{txt}</color>", Common.ChatMessageTypes.System), Common.Networking.IMessage.SendModes.ReliableUnordered, Lobby.LobbyState.Id, Common.Networking.NetChannels.Chat);
        }

        private void SendPlayerMessage(ushort playerId, string txt)
        {
            var playerName = Server.Players[playerId].ClientState.Name;
            var chat = new ServerChat(TMPFilter.CloseAllTags(playerName), $"<color=yellow>{txt}</color>", Server.Players[playerId].ClientState.User.Badges, Common.ChatMessageTypes.System);
            ServerLobbyManager.SendPacketToLobby(chat, Common.Networking.IMessage.SendModes.ReliableUnordered, Lobby.LobbyState.Id, Common.Networking.NetChannels.Chat);
        }

        private void CheckForSetupCancellation()
        {
            var hunters = 0;
            var props = 0;
            foreach(var player in Lobby.LobbyState.Players)
            {
                if (player.Value.Team == 0)
                    hunters++;
                else if (player.Value.Team == 1)
                    props++;
            }
            if (hunters == 0 || props == 0)
            {
                ServerLobbyManager.EndGame(Lobby.LobbyState.Id, true);
                SendSystemMessage("Game cancelled due to lack of players.");
            }
        }

        private List<Tuple<float, ushort>> _playersToRespawn = new();

        void UpdatePlayerRespawn(float deltaTime)
        {
            var newPlayersToRespawn = new List<Tuple<float, ushort>>();
            foreach(var player in _playersToRespawn)
            {
                var newTime = player.Item1 - deltaTime;
                if (newTime <= 0f)
                {
                    Server.SendPacketToClient(new ServerPropHuntRespawn(), Common.Networking.IMessage.SendModes.Reliable, Server.Players[player.Item2].Client, Common.Networking.NetChannels.Gamemodes);
                }
                else
                {
                    newPlayersToRespawn.Add(new(newTime, player.Item2));
                }
            }
            _playersToRespawn = newPlayersToRespawn;
        }

        public override void OnPacketFromLobbyReceived(Packets packetId, Packet packet, ushort playerId)
        {
            switch (packetId)
            {
                case Packets.ClientPropHuntSettings:
                    if (playerId == Lobby.LobbyState.HostId)
                    {
                        var settingsPacket = packet as ClientPropHuntSettings;
                        _setupTime = settingsPacket.SetupLength;
                        _matchTime = settingsPacket.MatchLength;
                        _pingInterval = settingsPacket.PingInterval;
                        _stageHash = settingsPacket.StageHash;
                        _becomeHunterOnKill = settingsPacket.PropsBecomeHuntersOnDeath;
                        _respawnOnKill = settingsPacket.HuntersRespawnOnDeath;
                        _respawnTime = settingsPacket.RespawnTime;
                    }
                    break;

                case Packets.ClientPropHuntStageHash:
                    {
                        var hashPacket = packet as ClientPropHuntStageHash;
                        _stageHashes[playerId] = hashPacket.StageHash;
                    }
                    break;

                case Packets.ClientPropHuntDeath:
                    {
                        var team = Lobby.LobbyState.Players[playerId].Team;
                        if (team == 0)
                        {
                            SendPlayerMessage(playerId, "{0} has perished.");
                            if (_respawnOnKill)
                            {
                                _playersToRespawn.Add(new(_respawnTime, playerId));
                            }
                            else
                            {
                                _deadPlayers.Add(playerId);
                            }
                        }
                        else
                        {
                            SendPlayerMessage(playerId, "{0} was hunted!");
                            if (_becomeHunterOnKill)
                            {
                                Lobby.LobbyState.Players[playerId].Team = 0;
                                ServerLobbyManager.QueueStageUpdate(Lobby.LobbyState.Stage);
                                _playersToRespawn.Add(new(_respawnTime, playerId));
                            }
                            else
                            {
                                _deadPlayers.Add(playerId);
                            }
                        }
                    }
                    break;

                case Packets.ClientPropHuntShoot:
                    {
                        var pack = packet as ClientPropHuntShoot;
                        pack.Attacker = playerId;
                        Server.SendPacketToClient(pack, Common.Networking.IMessage.SendModes.Reliable, Server.Players[pack.Target].Client, Common.Networking.NetChannels.Gamemodes);
                    }
                    break;
            }
        }

        public void SetState(States newState)
        {
            if (_state == newState) return;
            if (newState == States.Main)
                ServerLobbyManager.SendPacketToLobby(new ServerPropHuntBegin(), Common.Networking.IMessage.SendModes.Reliable, Lobby.LobbyState.Id, Common.Networking.NetChannels.Gamemodes);
            _state = newState;
            _stateTimer = 0f;
        }
    }
}
