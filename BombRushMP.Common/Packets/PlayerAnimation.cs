using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class PlayerAnimation : Packet
    {
        public override Packets PacketId => Packets.PlayerAnimation;
        private const byte Version = 0;
        public ushort ClientId;
        public int NewAnim;
        public bool ForceOverwrite;
        public bool Instant;
        public float AtTime;
        public override void Write(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(ClientId);
            writer.Write(NewAnim);
            writer.Write(ForceOverwrite);
            writer.Write(Instant);
            writer.Write(AtTime);
        }

        public override void Read(BinaryReader reader)
        {
            var version = reader.ReadByte();
            ClientId = reader.ReadUInt16();
            NewAnim = reader.ReadInt32();
            ForceOverwrite = reader.ReadBoolean();
            Instant = reader.ReadBoolean();
            AtTime = reader.ReadSingle();
        }
    }
}
