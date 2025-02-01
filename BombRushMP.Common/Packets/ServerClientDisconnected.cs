using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerClientDisconnected : Packet
    {
        public override Packets PacketId => Packets.ServerClientDisconnected;
        public ushort ClientId;

        public ServerClientDisconnected(ushort clientId)
        {
            ClientId = clientId;
        }

        public ServerClientDisconnected()
        {

        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(ClientId);
        }

        public override void Read(BinaryReader reader)
        {
            ClientId = reader.ReadUInt16();
        }
    }
}
