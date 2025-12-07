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
        public bool PropsBecomeHuntersOnDeath = false;
        public bool HuntersRespawnOnDeath = false;
        public float RespawnTime = 0f;

        public ClientPropHuntSettings()
        {

        }

        public ClientPropHuntSettings(float setupLength, float matchLength, float pingInterval, int stageHash, bool propsBecomeHuntersOnDeath, bool huntersRespawnOnDeath, float respawnTime)
        {
            SetupLength = setupLength;
            MatchLength = matchLength;
            PingInterval = pingInterval;
            StageHash = stageHash;
            PropsBecomeHuntersOnDeath = propsBecomeHuntersOnDeath;
            HuntersRespawnOnDeath = huntersRespawnOnDeath;
            RespawnTime = respawnTime;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(SetupLength);
            writer.Write(MatchLength);
            writer.Write(PingInterval);
            writer.Write(StageHash);
            writer.Write(PropsBecomeHuntersOnDeath);
            writer.Write(HuntersRespawnOnDeath);
            writer.Write(RespawnTime);
        }

        public override void Read(BinaryReader reader)
        {
            SetupLength = reader.ReadSingle();
            MatchLength = reader.ReadSingle();
            PingInterval = reader.ReadSingle();
            StageHash = reader.ReadInt32();
            PropsBecomeHuntersOnDeath = reader.ReadBoolean();
            HuntersRespawnOnDeath = reader.ReadBoolean();
            RespawnTime = reader.ReadSingle();
        }
    }
}
