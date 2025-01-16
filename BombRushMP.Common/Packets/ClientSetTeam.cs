using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientSetTeam : Packet
    {
        public override Packets PacketId => Packets.ClientSetTeam;
        public byte Team = 0;

        public ClientSetTeam()
        {

        }

        public ClientSetTeam(byte team)
        {
            Team = team;
        }

        public override void Read(BinaryReader reader)
        {
            Team = reader.ReadByte();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Team);
        }
    }
}
