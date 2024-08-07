using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class PlayerGenericEvent : PlayerPacket
    {
        public override Packets PacketId => Packets.PlayerGenericEvent;
        public GenericEvents Event;

        public PlayerGenericEvent()
        {

        }

        public PlayerGenericEvent(GenericEvents ev)
        {
            Event = ev;
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            Event = (GenericEvents)reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write((int)Event);
        }
    }
}
