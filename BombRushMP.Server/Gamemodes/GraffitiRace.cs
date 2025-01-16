using BombRushMP.Common.Networking;
using BombRushMP.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Server.Gamemodes
{
    public class GraffitiRace : Gamemode
    {
        public enum States
        {
            Countdown,
            Main,
            Finished
        }
        private States _state = States.Countdown;
        private float _countdownTimer = 0f;
        private float _maxScore = 0;
        private Dictionary<byte, HashSet<int>> _graffitisCompletedByTeam = new();
        public GraffitiRace() : base()
        {

        }

        public override void OnEnd(bool cancelled)
        {
            base.OnEnd(cancelled);
            _maxScore = 0;
        }

        public override void OnStart()
        {
            base.OnStart();
            _maxScore = 0;
            _graffitisCompletedByTeam = new();
        }

        private bool SetTagCompleted(byte team, int tag)
        {
            HashSet<int> teamSet;
            if (_graffitisCompletedByTeam.TryGetValue(team, out var result))
            {
                teamSet = result;
            }
            else
            {
                teamSet = new HashSet<int>();
                _graffitisCompletedByTeam[team] = teamSet;
            }
            if (teamSet.Contains(tag)) return false;
            teamSet.Add(tag);
            return true;
        }

        public override void Tick(float deltaTime)
        {
            switch (_state)
            {
                case States.Countdown:
                    if (_countdownTimer > CountdownTime)
                    {
                        ServerLobbyManager.SendPacketToLobby(new ServerGamemodeBegin(), IMessage.SendModes.Reliable, Lobby.LobbyState.Id, NetChannels.Gamemodes);
                        _state = States.Main;
                    }
                    break;
            }
            _countdownTimer += deltaTime;
        }

        public override void OnPacketFromLobbyReceived(Packets packetId, Packet packet, ushort playerId)
        {
            base.OnPacketFromLobbyReceived(packetId, packet, playerId);
            switch (packetId)
            {
                case Packets.ClientGraffitiRaceStart:
                    {
                        if (playerId == Lobby.LobbyState.HostId)
                        {
                            ServerLobbyManager.SendPacketToLobby(packet, IMessage.SendModes.Reliable, Lobby.LobbyState.Id, NetChannels.Gamemodes);
                        }
                    }
                    break;

                case Packets.ClientGraffitiRaceGSpots:
                    {
                        if (playerId == Lobby.LobbyState.HostId)
                        {
                            var racePacket = (ClientGraffitiRaceGSpots)packet;
                            ServerLobbyManager.SendPacketToLobby(racePacket, IMessage.SendModes.Reliable, Lobby.LobbyState.Id, NetChannels.Gamemodes);
                            _maxScore += racePacket.GraffitiSpots.Count;
                        }
                    }
                    break;

                case Packets.ClientGameModeScore:
                    if (TeamBased) break;
                    if (_state == States.Main)
                    {
                        var scorePacket = (ClientGamemodeScore)packet;
                        ServerLobbyManager.SetPlayerScore(playerId, scorePacket.Score);
                        if (scorePacket.Score >= _maxScore)
                        {
                            ServerLobbyManager.EndGame(Lobby.LobbyState.Id, false);
                            _state = States.Finished;
                        }
                    }
                    break;

                case Packets.ClientTeamGraffRaceScore:
                    if (!TeamBased) break;
                    {
                        var lobbyPlayer = Lobby.LobbyState.Players[playerId];
                        if (_state == States.Main)
                        {
                            var tagPacket = (ClientTeamGraffRaceScore)packet;
                            if (SetTagCompleted(lobbyPlayer.Team, tagPacket.TagHash))
                            {
                                ServerLobbyManager.SetPlayerScore(playerId, lobbyPlayer.Score + 1);
                                var teamScore = Lobby.LobbyState.GetScoreForTeam(Lobby.LobbyState.Players[playerId].Team);
                                if (teamScore >= _maxScore)
                                {
                                    ServerLobbyManager.EndGame(Lobby.LobbyState.Id, false);
                                    _state = States.Finished;
                                }
                                var otherTeamPlayers = Lobby.LobbyState.Players.Where((x) => x.Value.Team == lobbyPlayer.Team && x.Value.Id != lobbyPlayer.Id);
                                var serverPacket = new ServerTeamGraffRaceScore(playerId, tagPacket.TagHash);
                                foreach(var play in otherTeamPlayers)
                                {
                                    Server.SendPacketToClient(serverPacket, IMessage.SendModes.Reliable, Server.Players[play.Key].Client, NetChannels.Gamemodes);
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }   
}
