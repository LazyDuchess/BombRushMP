using BombRushMP.Common.Packets;
using Riptide;
using Riptide.Transports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.ServerApp
{
    public class Player
    {
        private const float SecondsToKickPlayerWithoutClientState = 1f;
        public BRCServer Server = null;
        public ClientState ClientState = null;
        public ClientVisualState ClientVisualState = null;
        public float SecondsWithoutSendingClientState = 0f;
        public Connection Client;

        public void Tick(float deltaTime)
        {
            if (ClientState == null)
            {
                SecondsWithoutSendingClientState += deltaTime;
                if (SecondsWithoutSendingClientState > SecondsToKickPlayerWithoutClientState)
                {
                    Server.Log($"Rejecting player from {Client} (ID: {Client.Id}) because they took longer than {SecondsToKickPlayerWithoutClientState} seconds to send ClientState.");
                    Server.DisconnectClient(Client.Id);
                    return;
                }
            }
            else
            {
                SecondsWithoutSendingClientState = 0f;
            }
        }
    }
}
