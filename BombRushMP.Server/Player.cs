using BombRushMP.Common;
using BombRushMP.Common.Networking;
using BombRushMP.Common.Packets;
using BombRushMP.Server.Gamemodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Server
{
    public class Player
    {
        private const float SecondsToKickPlayerWithoutClientState = 1f;
        public BRCServer Server = null;
        public ClientState ClientState = null;
        public ClientVisualState ClientVisualState = null;
        public float SecondsWithoutSendingClientState = 0f;
        public INetConnection Client;
        public DateTime LastChatTime = DateTime.UtcNow;

        public void Tick(float deltaTime)
        {
            if (ClientState == null)
            {
                SecondsWithoutSendingClientState += deltaTime;
                if (SecondsWithoutSendingClientState > SecondsToKickPlayerWithoutClientState)
                {
                    ServerLogger.Log($"Rejecting player from {Client} (ID: {Client.Id}) because they took longer than {SecondsToKickPlayerWithoutClientState} seconds to send ClientState.");
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
