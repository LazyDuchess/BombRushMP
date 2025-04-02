using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerTeleportPlayer : Packet
    {
        public override Packets PacketId => Packets.ServerTeleportPlayer;

        public Vector3 Position;
        public Quaternion Rotation;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Position.Z);
            Compression.WriteCompressedQuaternion(Rotation, writer);
        }

        public override void Read(BinaryReader reader)
        {
            var px = reader.ReadSingle();
            var py = reader.ReadSingle();
            var pz = reader.ReadSingle();
            Position = new Vector3(px, py, pz);
            Rotation = Compression.ReadCompressedQuaternion(reader);
        }
    }
}
