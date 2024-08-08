using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerScoreBattleBegin : Packet
    {
        public override Packets PacketId => Packets.ServerScoreBattleBegin;
        public DateTime StartTime = DateTime.UtcNow;

        public ServerScoreBattleBegin()
        {

        }

        public ServerScoreBattleBegin(DateTime startTime)
        {
            StartTime = startTime;
        }

        public override void Read(BinaryReader reader)
        {
            StartTime = DateTime.FromBinary(reader.ReadInt64());
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(StartTime.ToBinary());
        }
    }
}
