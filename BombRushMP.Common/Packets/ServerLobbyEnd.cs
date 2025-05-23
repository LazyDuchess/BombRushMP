﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerLobbyEnd : Packet
    {
        public override Packets PacketId => Packets.ServerLobbyEnd;
        public bool Cancelled = false;
        public ServerLobbyEnd()
        {

        }

        public ServerLobbyEnd(bool cancelled)
        {
           Cancelled = cancelled;
        }

        public override void Read(BinaryReader reader)
        {
            Cancelled = reader.ReadBoolean();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Cancelled);
        }
    }
}
