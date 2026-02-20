using BombRushMP.Common.Packets;
using BombRushMP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common.Networking;

namespace BombRushMP.Server.Gamemodes
{
    public class ScoreBattle : Gamemode
    {
        public enum States
        {
            Countdown,
            Main,
            Finished
        }
        private States _state = States.Countdown;
        private float _stateTimer = 0f;
        private DateTime _startTime = DateTime.UtcNow;
        public bool ComboBased = false;
        private HashSet<ushort> _clientsFinishedCombo = new HashSet<ushort>();
        private float _timeElapsed = 0f;
        private float _timeLeft = 1f;
        private int _durationMinutes = 3;

        public ScoreBattle() : base()
        {

        }

        public override void Tick(float deltaTime)
        {
            switch (_state)
            {
                case States.Countdown:
                    if (_stateTimer > CountdownTime)
                    {
                        ServerLobbyManager.SendPacketToLobby(new ServerGamemodeBegin(), IMessage.SendModes.Reliable, Lobby.LobbyState.Id, NetChannels.Gamemodes);
                        _state = States.Main;
                        _startTime = DateTime.UtcNow;
                    }
                    break;

                case States.Main:
                    _timeElapsed = (float)(DateTime.UtcNow - _startTime).TotalSeconds;
                    var durationInSecs = _durationMinutes * 60f;
                    _timeLeft = durationInSecs - _timeElapsed;
                    if (!ComboBased)
                    {
                        if (_timeLeft <= 0f)
                        {
                            var winner = GetHighestScoringPlayer();
                            if (winner != 0)
                                ServerLobbyManager.AddPlayerWin(winner);
                            ServerLobbyManager.EndGame(Lobby.LobbyState.Id, false);
                            _state = States.Finished;
                        }
                    }
                    else
                    {
                        if (EveryoneFinishedComboing() && _timeLeft <= 0f)
                        {
                            var winner = GetHighestScoringPlayer();
                            if (winner != 0)
                                ServerLobbyManager.AddPlayerWin(winner);
                            ServerLobbyManager.EndGame(Lobby.LobbyState.Id, false);
                            _state = States.Finished;
                        }
                    }
                    break;
            }
            _stateTimer += deltaTime;
        }

        private ushort GetHighestScoringPlayer()
        {
            ushort lastPly = 0;
            var lastScore = 0f;
            foreach(var ply in Lobby.LobbyState.Players)
            {
                if (lastPly == 0)
                {
                    lastPly = ply.Key;
                    lastScore = ply.Value.Score;
                }
                else if (ply.Value.Score > lastScore)
                {
                    lastPly = ply.Key;
                    lastScore = ply.Value.Score;
                }
            }
            return lastPly;
        }

        private bool EveryoneFinishedComboing()
        {
            foreach(var player in Lobby.LobbyState.Players)
            {
                if (!_clientsFinishedCombo.Contains(player.Key))
                    return false;
            }
            return true;
        }

        public override void OnPacketFromLobbyReceived(Packets packetId, Packet packet, ushort playerId)
        {
            base.OnPacketFromLobbyReceived(packetId, packet, playerId);
            switch (packetId)
            {
                case Packets.ClientScoreBattleLength:
                    if (playerId == Lobby.LobbyState.HostId)
                    {
                        _durationMinutes = (packet as ClientScoreBattleLength).Minutes;
                    }
                    break;
            }
            if (_state == States.Main)
            {
                switch (packetId)
                {
                    case Packets.ClientGameModeScore:
                        if (_clientsFinishedCombo.Contains(playerId))
                            break;
                        var scorePacket = (ClientGamemodeScore)packet;
                        ServerLobbyManager.SetPlayerScore(playerId, scorePacket.Score);
                        break;

                    case Packets.ClientComboOver:
                        if (ComboBased)
                        {
                            if (!_clientsFinishedCombo.Contains(playerId))
                            {
                                ServerLobbyManager.SendPacketToLobby(new ServerChat(TMPFilter.CloseAllTags(BRCServer.Instance.Players[playerId].ClientState.Name), "{0} landed their combo!", BRCServer.Instance.Players[playerId].ClientState.ShowBadges ? BRCServer.Instance.Players[playerId].ClientState.User.Badges : null, ChatMessageTypes.System), IMessage.SendModes.ReliableUnordered, Lobby.LobbyState.Id, NetChannels.Gamemodes);
                                _clientsFinishedCombo.Add(playerId);
                                ServerLobbyManager.SetPlayerScore(playerId, ((ClientComboOver)packet).Score);
                            }
                        }
                        break;
                }
            }
        }
    }
}
