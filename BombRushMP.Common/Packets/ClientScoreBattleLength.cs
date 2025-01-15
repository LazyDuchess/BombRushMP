using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientScoreBattleLength : Packet
    {
        public override Packets PacketId => Packets.ClientScoreBattleLength;
        public byte Minutes = 3;
        
        public ClientScoreBattleLength(byte minutes)
        {
            Minutes = minutes;
        }

        public ClientScoreBattleLength()
        {

        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Minutes);
        }

        public override void Read(BinaryReader reader)
        {
            Minutes = reader.ReadByte();
        }
    }
}
