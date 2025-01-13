using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace BombRushMP.Common.Packets
{
    public class ClientGraffitiRaceData : Packet
    {
        public const int MaxGraffitiSpotsPerPacket = 5;
        public override Packets PacketId => Packets.ClientGraffitiRaceData;
        public Vector3 SpawnPosition = Vector3.Zero;
        public Quaternion SpawnRotation = Quaternion.Identity;
        public List<int> GraffitiSpots = new();
        public bool FinalPacket = true;

        public ClientGraffitiRaceData()
        {

        }

        public ClientGraffitiRaceData(Vector3 spawnPosition, Quaternion spawnRotation, List<int> graffitiSpots, bool final)
        {
            SpawnPosition = spawnPosition;
            SpawnRotation = spawnRotation;
            GraffitiSpots = graffitiSpots;
            FinalPacket = final;
        }

        public override void Read(BinaryReader reader)
        {
            FinalPacket = reader.ReadBoolean();

            var spx = reader.ReadSingle();
            var spy = reader.ReadSingle();
            var spz = reader.ReadSingle();
            SpawnPosition = new Vector3(spx, spy, spz);

            SpawnRotation = Quaternion.Normalize(Compression.ReadCompressedQuaternion(reader));

            var gSpotCount = reader.ReadUInt16();
            for(var i = 0; i < gSpotCount; i++)
            {
                var gSpotUID = reader.ReadInt32();
                GraffitiSpots.Add(gSpotUID);
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(FinalPacket);

            writer.Write(SpawnPosition.X);
            writer.Write(SpawnPosition.Y);
            writer.Write(SpawnPosition.Z);

            Compression.WriteCompressedQuaternion(SpawnRotation, writer);

            writer.Write((short)GraffitiSpots.Count);

            foreach(var grafSpot in GraffitiSpots)
            {
                writer.Write(grafSpot);
            }
        }
    }
}