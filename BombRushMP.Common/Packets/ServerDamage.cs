using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerDamage : Packet
    {
        public override Packets PacketId => Packets.ServerDamage;

        public int Damage = 0;

        public ServerDamage()
        {

        }
        public ServerDamage(int dmg)
        {
            Damage = dmg;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Damage);
        }

        public override void Read(BinaryReader reader)
        {
            Damage = reader.ReadInt32();
        }
    }
}
