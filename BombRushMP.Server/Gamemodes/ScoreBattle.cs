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

        public ScoreBattle() : base()
        {

        }

        public override void Tick(float deltaTime)
        {
            switch (_state)
            {
                case States.Countdown:
                    if (_stateTimer > 3f)
                    {
                        ServerLobbyManager.SendPacketToLobby(new ServerGamemodeBegin(), IMessage.SendModes.Reliable, Lobby.LobbyState.Id);
                        _state = States.Main;
                        _startTime = DateTime.UtcNow;
                    }
                    break;

                case States.Main:
                    _timeElapsed = (float)(DateTime.UtcNow - _startTime).TotalSeconds;
                    _timeLeft = Constants.ScoreBattleDuration - _timeElapsed;
                    if (!ComboBased)
                    {
                        if (_timeLeft <= 0f)
                        {
                            ServerLobbyManager.EndGame(Lobby.LobbyState.Id, false);
                            _state = States.Finished;
                        }
                    }
                    else
                    {
                        if (EveryoneFinishedComboing() && _timeLeft <= 0f)
                        {
                            ServerLobbyManager.EndGame(Lobby.LobbyState.Id, false);
                            _state = States.Finished;
                        }
                    }
                    break;
            }
            _stateTimer += deltaTime;
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
                                ServerLobbyManager.SendPacketToLobby(new ServerChat(TMPFilter.CloseAllTags(BRCServer.Instance.Players[playerId].ClientState.Name), "{0} landed their combo!", BRCServer.Instance.Players[playerId].ClientState.User.Badge, ChatMessageTypes.System), IMessage.SendModes.Reliable, Lobby.LobbyState.Id);
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
