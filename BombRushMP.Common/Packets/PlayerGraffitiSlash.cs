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
            Direction = Compression.ReadCompressedVector3Normal(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            Compression.WriteCompressedVector3Normal(Direction, writer);
        }
    }
}
