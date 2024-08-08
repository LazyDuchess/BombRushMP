using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientGamemodeScore : Packet
    {
        public override Packets PacketId => Packets.ClientGameModeScore;
        public float Score = 0;

        public ClientGamemodeScore()
        {

        }

        public ClientGamemodeScore(float score)
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
