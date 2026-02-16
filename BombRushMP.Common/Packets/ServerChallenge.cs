using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerChallenge : Packet
    {
        public override Packets PacketId => Packets.ServerChallenge;
        public string Challenge = "";

        public ServerChallenge()
        {

        }

        public ServerChallenge(string challenge)
        {
            Challenge = challenge;
        }

        public override void Read(BinaryReader reader)
        {
            Challenge = reader.ReadString();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Challenge);
        }
    }
}
