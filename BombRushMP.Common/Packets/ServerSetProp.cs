using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerSetProp : Packet
    {
        public override Packets PacketId => Packets.ServerSetProp;

        public bool Disguised = false;
        public int PropId = 0;

        public ServerSetProp(bool disguised, int propId)
        {
            Disguised = disguised;
            PropId = propId;
        }

        public override void Read(BinaryReader reader)
        {
            Disguised = reader.ReadBoolean();
            PropId = reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Disguised);
            writer.Write(PropId);
        }
    }
}
