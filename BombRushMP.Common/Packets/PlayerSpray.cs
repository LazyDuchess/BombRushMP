using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class PlayerSpray : Packet
    {
        public override Packets PacketId => Packets.PlayerSpray;
        private const byte Version = 0;
        public ushort ClientId = 0;
        public override void Write(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(ClientId);
        }

        public override void Read(BinaryReader reader)
        {
            var version = reader.ReadByte();
            ClientId = reader.ReadUInt16();
        }
    }
}
