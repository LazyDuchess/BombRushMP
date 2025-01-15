using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerSetChibi : Packet
    {
        public override Packets PacketId => Packets.ServerSetChibi;
        public bool Set = false;

        public ServerSetChibi(bool set)
        {
            Set = set;
        }

        public ServerSetChibi()
        {

        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Set);
        }

        public override void Read(BinaryReader reader)
        {
            Set = reader.ReadBoolean();
        }
    }
}
