using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientScoreBattleScore : Packet
    {
        public override Packets PacketId => Packets.ClientScoreBattleScore;
        public float Score = 0;

        public ClientScoreBattleScore()
        {

        }

        public ClientScoreBattleScore(float score)
        {
            Score = score;
        }

        public override void Read(BinaryReader reader)
        {
            Score = reader.ReadSingle();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Score);
        }
    }
}
