using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientLobbySetReady : Packet
    {
        public override Packets PacketId => Packets.ClientLobbySetReady;
        public bool Ready = false;

        public ClientLobbySetReady()
        {

        }

        public ClientLobbySetReady(bool ready)
        {
            Ready = ready;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Ready);
        }

        public override void Read(BinaryReader reader)
        {
            Ready = reader.ReadBoolean();
        }
    }
}
