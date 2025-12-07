using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientPropHuntShoot : Packet
    {
        public override Packets PacketId => Packets.ClientPropHuntShoot;
        public ushort Attacker = 0;
        public ushort Target = 0;

        public ClientPropHuntShoot()
        {

        }

        public ClientPropHuntShoot(ushort target)
        {
            Target = target;
        }

        public override void Read(BinaryReader reader)
        {
            Attacker = reader.ReadUInt16();
            Target = reader.ReadUInt16();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Attacker);
            writer.Write(Target);
        }
    }
}
