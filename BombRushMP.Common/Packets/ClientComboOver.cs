using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientComboOver : Packet
    {
        public override Packets PacketId => Packets.ClientComboOver;
        public float Score = 0f;
        public ClientComboOver()
        {

        }

        public ClientComboOver(float score)
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
