using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    /// <summary>
    /// From server to client, tells client what their client ID is on connect.
    /// </summary>
    public class ServerConnectionResponse : Packet
    {
        public override Packets PacketId => Packets.ServerConnectionResponse;
        public ushort LocalClientId = 0;

        public override void Read(BinaryReader reader)
        {
            LocalClientId = reader.ReadUInt16();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(LocalClientId);
        }
    }
}
