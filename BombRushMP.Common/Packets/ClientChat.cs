using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientChat : Packet
    {
        public override Packets PacketId => Packets.ClientChat;
        public string Message = "";

        public ClientChat()
        {

        }

        public ClientChat(string message)
        {
            Message = message;
        }

        public override void Read(BinaryReader reader)
        {
            Message = reader.ReadString();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Message);
        }
    }
}
