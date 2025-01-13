using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientGraffitiRaceStart : Packet
    {
        public override Packets PacketId => Packets.ClientGraffitiRaceStart;
        public Vector3 SpawnPosition = Vector3.Zero;
        public Quaternion SpawnRotation = Quaternion.Identity;

        public ClientGraffitiRaceStart(Vector3 position, Quaternion rotation)
        {
            SpawnPosition = position;
            SpawnRotation = rotation;
        }

        public ClientGraffitiRaceStart()
        {

        }

        public override void Read(BinaryReader reader)
        {
            var px = reader.ReadSingle();
            var py = reader.ReadSingle();
            var pz = reader.ReadSingle();
            var spawnRotation = Quaternion.Normalize(Compression.ReadCompressedQuaternion(reader));
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(SpawnPosition.X);
            writer.Write(SpawnPosition.Y);
            writer.Write(SpawnPosition.Z);
            Compression.WriteCompressedQuaternion(SpawnRotation, writer);
        }
    }
}
