using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerGamemodeBegin : Packet
    {
        public override Packets PacketId => Packets.ServerGamemodeBegin;
        public DateTime StartTime = DateTime.UtcNow;

        public ServerGamemodeBegin()
        {

        }

        public ServerGamemodeBegin(DateTime startTime)
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
