using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerSetSpecialSkin : Packet
    {
        public override Packets PacketId => Packets.ServerSetSpecialSkin;
        public SpecialSkins SpecialSkin = SpecialSkins.None;

        public ServerSetSpecialSkin(SpecialSkins specialSkin)
        {
            SpecialSkin = specialSkin;
        }

        public ServerSetSpecialSkin()
        {

        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write((int)SpecialSkin);
        }

        public override void Read(BinaryReader reader)
        {
            SpecialSkin = (SpecialSkins)reader.ReadInt32();
        }
    }
}
