﻿using BombRushMP.Common.Networking;
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
                    if (_state == States.Main)
                    {
                        var scorePacket = (ClientGamemodeScore)packet;
                        ServerLobbyManager.SetPlayerScore(playerId, scorePacket.Score);
                        if (TeamBased)
                        {
                            var teamScore = Lobby.LobbyState.GetScoreForTeam(Lobby.LobbyState.Players[playerId].Team);
                            if (teamScore >= _maxScore)
                            {
                                ServerLobbyManager.EndGame(Lobby.LobbyState.Id, false);
                                _state = States.Finished;
                            }
                        }
                        else
                        {
                            if (scorePacket.Score >= _maxScore)
                            {
                                ServerLobbyManager.EndGame(Lobby.LobbyState.Id, false);
                                _state = States.Finished;
                            }
                        }
                    }
                    break;
            }
        }
    }   
}
