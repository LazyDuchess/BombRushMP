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

        private float _setupTime = 9999f;
        private float _matchTime = 9999f;
        private float _pingInterval = 99999f;
        private int _stageHash = 0;

        public PropHunt() : base()
        {
            TeamBased = true;
        }

        public override void Tick(float deltaTime)
        {
            switch (_state)
            {
                case States.Setup:
                    if (_stateTimer >= _setupTime)
                    {
                        ServerLobbyManager.SendPacketToLobby(new ServerPropHuntBegin(), Common.Networking.IMessage.SendModes.Reliable, Lobby.LobbyState.Id, Common.Networking.NetChannels.Gamemodes);
                        SetState(States.Main);
                    }
                    break;

                case States.Main:
                    if (_stateTimer >= _matchTime)
                    {
                        ServerLobbyManager.EndGame(Lobby.LobbyState.Id, false);
                        SetState(States.Finished);
                    }
                    break;
            }
            _stateTimer += deltaTime;
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
                    }
                    break;
            }
        }

        private void SetState(States newState)
        {
            if (_state == newState) return;
            _state = newState;
            _stateTimer = 0f;
        }
    }
}
