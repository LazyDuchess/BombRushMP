using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerPropHuntSpawn : Packet
    {
        public override Packets PacketId => Packets.ServerPropHuntSpawn;
        public Vector3 Position;
        public Quaternion Rotation;

        public ServerPropHuntSpawn()
        {

        }

        public ServerPropHuntSpawn(Vector3 pos, Quaternion rot)
        {
            Position = pos;
            Rotation = rot;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Position.Z);

            writer.Write(Rotation.X);
            writer.Write(Rotation.Y);
            writer.Write(Rotation.Z);
            writer.Write(Rotation.W);
        }

        public override void Read(BinaryReader reader)
        {
            var px = reader.ReadSingle();
            var py = reader.ReadSingle();
            var pz = reader.ReadSingle();

            var rx = reader.ReadSingle();
            var ry = reader.ReadSingle();
            var rz = reader.ReadSingle();
            var rw = reader.ReadSingle();

            Position = new Vector3(px, py, pz);
            Rotation = new Quaternion(rx, ry, rz, rw);
        }
    }
}
