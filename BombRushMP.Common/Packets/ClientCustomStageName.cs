using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BombRushMP.Common.Packets
{
    public class ClientCustomStageName : Packet
    {
        public override Packets PacketId => Packets.ClientCustomStageName;
        public string Name = "";
        public ClientCustomStageName()
        {

        }

        public ClientCustomStageName(string name)
        {
            Name = name;
        }

        public override void Read(BinaryReader reader)
        {
            Name = reader.ReadString();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Name);
        }
    }
}
