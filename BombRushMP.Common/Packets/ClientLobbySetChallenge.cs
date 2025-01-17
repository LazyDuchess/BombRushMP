using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientLobbySetChallenge : Packet
    {
        public override Packets PacketId => Packets.ClientLobbySetChallenge;
        public bool Set = true;

        public ClientLobbySetChallenge()
        {

        }

        public ClientLobbySetChallenge(bool set)
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
