using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientPropHuntSettings : Packet
    {
        public override Packets PacketId => Packets.ClientPropHuntSettings;

        public float SetupLength = 0f;
        public float MatchLength = 0f;
        public float PingInterval = 0f;
        public int StageHash = 0;

        public ClientPropHuntSettings()
        {

        }

        public ClientPropHuntSettings(float setupLength, float matchLength, float pingInterval, int stageHash)
        {
            SetupLength = setupLength;
            MatchLength = matchLength;
            PingInterval = pingInterval;
            StageHash = stageHash;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(SetupLength);
            writer.Write(MatchLength);
            writer.Write(PingInterval);
            writer.Write(StageHash);
        }

        public override void Read(BinaryReader reader)
        {
            SetupLength = reader.ReadSingle();
            MatchLength = reader.ReadSingle();
            PingInterval = reader.ReadSingle();
            StageHash = reader.ReadInt32();
        }
    }
}
