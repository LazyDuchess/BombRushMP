﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class PlayerGraffitiFinisher : PlayerPacket
    {
        public override Packets PacketId => Packets.PlayerGraffitiFinisher;
        public int GraffitiSize = 0;

        public PlayerGraffitiFinisher()
        {

        }

        public PlayerGraffitiFinisher(int graffitiSize)
        {
            GraffitiSize = graffitiSize;
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            GraffitiSize = reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(GraffitiSize);
        }
    }
}
