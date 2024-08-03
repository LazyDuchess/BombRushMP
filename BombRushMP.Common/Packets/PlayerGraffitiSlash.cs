using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.IO;
using System.Linq.Expressions;

namespace BombRushMP.Common.Packets
{
    public class PlayerGraffitiSlash : PlayerPacket
    {
        public override Packets PacketId => Packets.PlayerGraffitiSlash;
        private const byte Version = 0;
        public Vector3 Direction = Vector3.Zero;

        public PlayerGraffitiSlash()
        {

        }

        public PlayerGraffitiSlash(Vector3 direction)
        {
            Direction = direction;
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            var version = reader.ReadByte();
            var dx = reader.ReadSingle();
            var dy = reader.ReadSingle();
            var dz = reader.ReadSingle();
            Direction = new Vector3(dx, dy, dz);
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(Version);
            writer.Write(Direction.X);
            writer.Write(Direction.Y);
            writer.Write(Direction.Z);
        }
    }
}
