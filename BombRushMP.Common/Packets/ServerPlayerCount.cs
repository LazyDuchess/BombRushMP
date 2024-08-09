using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerPlayerCount : Packet
    {
        public override Packets PacketId => Packets.ServerPlayerCount;
        public Dictionary<int, int> PlayerCountByStage = new();

        public ServerPlayerCount()
        {

        }

        public ServerPlayerCount(Dictionary<int, int> playerCountByStage)
        {
            PlayerCountByStage = playerCountByStage;
        }

        public override void Read(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            for(var i = 0; i < count; i++)
            {
                var stageId = reader.ReadInt32();
                var playerCount = reader.ReadInt32();
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(PlayerCountByStage.Count);
            foreach(var stage in PlayerCountByStage)
            {
                writer.Write(stage.Key);
                writer.Write(stage.Value);
            }
        }
    }
}
