using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace BombRushMP.Common.Packets
{
    public class ClientGraffitiRaceData : Packet
    {
        public const int MaxGraffitiSpotsPerPacket = 10;
        public override Packets PacketId => Packets.ClientGraffitiRaceData;
        public Vector3 SpawnPosition = Vector3.Zero;
        public Quaternion SpawnRotation = Quaternion.Identity;
        public List<string> GraffitiSpots = new();
        public bool FinalPacket = true;

        public ClientGraffitiRaceData()
        {

        }

        public ClientGraffitiRaceData(Vector3 spawnPosition, Quaternion spawnRotation, List<string> graffitiSpots, bool final)
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

            var srx = reader.ReadSingle();
            var sry = reader.ReadSingle();
            var srz = reader.ReadSingle();
            var srw = reader.ReadSingle();
            SpawnRotation = new Quaternion(srx, sry, srz, srw);

            var gSpotCount = reader.ReadInt32();
            for(var i = 0; i < gSpotCount; i++)
            {
                var gSpotUID = reader.ReadString();
                GraffitiSpots.Add(gSpotUID);
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(FinalPacket);

            writer.Write(SpawnPosition.X);
            writer.Write(SpawnPosition.Y);
            writer.Write(SpawnPosition.Z);

            writer.Write(SpawnRotation.X);
            writer.Write(SpawnRotation.Y);
            writer.Write(SpawnRotation.Z);
            writer.Write(SpawnRotation.W);

            writer.Write(GraffitiSpots.Count);

            foreach(var grafSpot in GraffitiSpots)
            {
                writer.Write(grafSpot);
            }
        }
    }
}