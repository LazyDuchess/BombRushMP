using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientTeamGraffRaceScore : Packet
    {
        public override Packets PacketId => Packets.ClientTeamGraffRaceScore;
        public int TagHash;

        public ClientTeamGraffRaceScore()
        {

        }

        public ClientTeamGraffRaceScore(int tagHash)
        {
            TagHash = tagHash;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(TagHash);
        }

        public override void Read(BinaryReader reader)
        {
            TagHash = reader.ReadInt32();
        }
    }
}
