using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientVisualState : Packet
    {
        public override Packets PacketId => Packets.ClientVisualState;
        private const byte Version = 0;
        public Vector3 Position;
        public Vector3 Velocity;
        public Quaternion Rotation;
        public Quaternion VisualRotation;
        public bool MoveStyleEquipped;
        public int MoveStyle;

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Version);

            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Position.Z);

            writer.Write(Rotation.X);
            writer.Write(Rotation.Y);
            writer.Write(Rotation.Z);
            writer.Write(Rotation.W);

            writer.Write(VisualRotation.X);
            writer.Write(VisualRotation.Y);
            writer.Write(VisualRotation.Z);
            writer.Write(VisualRotation.W);

            writer.Write(Velocity.X);
            writer.Write(Velocity.Y);
            writer.Write(Velocity.Z);

            writer.Write(MoveStyleEquipped);
            writer.Write(MoveStyle);
        }

        public override void Read(BinaryReader reader)
        {
            var version = reader.ReadByte();

            var posX = reader.ReadSingle();
            var posY = reader.ReadSingle();
            var posZ = reader.ReadSingle();

            var rotX = reader.ReadSingle();
            var rotY = reader.ReadSingle();
            var rotZ = reader.ReadSingle();
            var rotW = reader.ReadSingle();

            var visualRotX = reader.ReadSingle();
            var visualRotY = reader.ReadSingle();
            var visualRotZ = reader.ReadSingle();
            var visualRotW = reader.ReadSingle();

            var velX = reader.ReadSingle();
            var velY = reader.ReadSingle();
            var velZ = reader.ReadSingle();

            MoveStyleEquipped = reader.ReadBoolean();
            MoveStyle = reader.ReadInt32();

            Position = new Vector3(posX, posY, posZ);
            Rotation = new Quaternion(rotX, rotY, rotZ, rotW);
            Velocity = new Vector3(velX, velY, velZ);
            VisualRotation = new Quaternion(visualRotX, visualRotY, visualRotZ, visualRotW);
        }
    }
}
