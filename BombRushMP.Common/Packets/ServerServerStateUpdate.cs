using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerServerStateUpdate : Packet
    {
        public override Packets PacketId => Packets.ServerServerStateUpdate;
        public ServerState State;

        public ServerServerStateUpdate(ServerState state)
        {
            State = state;
        }

        public ServerServerStateUpdate()
        {

        }

        public override void Read(BinaryReader reader)
        {
            State = new ServerState();
            State.Read(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            State.Write(writer);
        }
    }
}
