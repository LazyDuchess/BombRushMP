using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace BombRushMP.Common.Packets
{
    public class ClientGraffitiRaceGSpots : Packet
    {
        public override Packets PacketId => Packets.ClientGraffitiRaceGSpots;
        public const int MaxGraffitiSpotsPerPacket = 10;
        public List<int> GraffitiSpots = new();
        public bool FinalPacket = true;

        public ClientGraffitiRaceGSpots()
        {

        }

        public ClientGraffitiRaceGSpots(List<int> graffitiSpots, bool final)
        {
            GraffitiSpots = graffitiSpots;
            FinalPacket = final;
        }

        public override void Read(BinaryReader reader)
        {
            FinalPacket = reader.ReadBoolean();
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
            writer.Write((short)GraffitiSpots.Count);

            foreach(var grafSpot in GraffitiSpots)
            {
                writer.Write(grafSpot);
            }
        }
    }
}