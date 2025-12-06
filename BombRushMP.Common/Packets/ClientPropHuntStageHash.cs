using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientPropHuntStageHash : Packet
    {
        public override Packets PacketId => Packets.ClientPropHuntStageHash;
        public int StageHash = 0;

        public ClientPropHuntStageHash()
        {

        }

        public ClientPropHuntStageHash(int stageHash)
        {
            StageHash = stageHash;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(StageHash);
        }

        public override void Read(BinaryReader reader)
        {
            StageHash = reader.ReadInt32();
        }
    }
}
