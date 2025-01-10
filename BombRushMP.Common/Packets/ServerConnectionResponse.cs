﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    /// <summary>
    /// From server to client, tells client what their client ID is on connect.
    /// </summary>
    public class ServerConnectionResponse : Packet
    {
        public override Packets PacketId => Packets.ServerConnectionResponse;
        public ushort LocalClientId = 0;
        public float TickRate = Constants.DefaultNetworkingTickRate;
        public AuthUser User;

        public override void Read(BinaryReader reader)
        {
            LocalClientId = reader.ReadUInt16();
            TickRate = reader.ReadSingle();
            User = new AuthUser();
            User.Read(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(LocalClientId);
            writer.Write(TickRate);
            User.Write(writer);
        }
    }
}
