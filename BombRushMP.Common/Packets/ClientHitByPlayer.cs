using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientHitByPlayer : PlayerPacket
    {
        public override Packets PacketId => Packets.ClientHitByPlayer;
        public ushort Attacker = 0;

        public ClientHitByPlayer()
        {

        }

        public ClientHitByPlayer(ushort attacker)
        {
            Attacker = attacker;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Attacker);
        }

        public override void Read(BinaryReader reader)
        {
            Attacker = reader.ReadUInt16();
        }
    }
}
