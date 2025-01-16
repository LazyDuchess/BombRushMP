using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientLobbySetAllowTeamSwitching : Packet
    {
        public override Packets PacketId => Packets.ClientLobbySetAllowTeamSwitching;
        public bool Set = true;

        public ClientLobbySetAllowTeamSwitching()
        {

        }

        public ClientLobbySetAllowTeamSwitching(bool set)
        {
            Set = set;
        }

        public override void Read(BinaryReader reader)
        {
            Set = reader.ReadBoolean();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Set);
        }
    }
}
