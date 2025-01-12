using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerBanList : Packet
    {
        public override Packets PacketId => Packets.ServerBanList;
        public string JsonData = "";

        public ServerBanList()
        {

        }

        public ServerBanList(string banList)
        {
            JsonData = banList;
        }

        public override void Read(BinaryReader reader)
        {
            JsonData = reader.ReadString();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(JsonData);
        }
    }
}
