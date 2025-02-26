﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common;
using BombRushMP.Common.Networking;
using LiteNetLib;

namespace BombRushMP.LiteNetLibInterface
{
    public class LiteNetLibInterface : INetworkingInterface
    {
        public static string Key = $"ACN-{Constants.ProtocolVersion}";
        public int MaxPayloadSize
        {
            get
            {
                return 1400;
            }
            set
            {

            }
        }

        public INetClient CreateClient()
        {
            return new LiteNetLibClient();
        }

        public IMessage CreateMessage(IMessage.SendModes sendMode, NetChannels channel, Enum packetId)
        {
            return new LiteNetLibMessage(sendMode, channel, Convert.ToUInt16(packetId));
        }

        public INetServer CreateServer()
        {
            return new LiteNetLibServer();
        }

        public override string ToString()
        {
            return "LiteNetLib";
        }
    }
}
