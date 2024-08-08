using BombRushMP.Common.Packets;
using BombRushMP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.ServerApp.Gamemodes
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
                        ServerLobbyManager.SendPacketToLobby(new ServerGamemodeBegin(), Riptide.MessageSendMode.Reliable, Lobby.LobbyState.Id);
                        _state = States.Main;
                        _startTime = DateTime.UtcNow;
                    }
                    break;

                case States.Main:
                    var timeElapsed = (float)(DateTime.UtcNow - _startTime).TotalSeconds;
                    var timeLeft = Constants.ScoreBattleDuration - timeElapsed;
                    if (timeLeft <= 0f)
                    {
                        ServerLobbyManager.EndGame(Lobby.LobbyState.Id, false);
                        _state = States.Finished;
                    }
                    break;
            }
            _stateTimer += deltaTime;
        }

        public override void OnPacketFromLobbyReceived(Packets packetId, Packet packet, ushort playerId)
        {
            if (_state == States.Main)
            {
                switch (packetId)
                {
                    case Packets.ClientGameModeScore:
                        var scorePacket = (ClientGamemodeScore)packet;
                        ServerLobbyManager.SetPlayerScore(playerId, scorePacket.Score);
                        break;
                }
            }
        }
    }
}
