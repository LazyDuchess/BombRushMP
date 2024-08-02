using Riptide;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public abstract class PlayerPacket : Packet
    {
        public ushort ClientId;
        public override void Read(BinaryReader reader)
        {
            ClientId = reader.ReadUInt16();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(ClientId);
        }
    }
}
