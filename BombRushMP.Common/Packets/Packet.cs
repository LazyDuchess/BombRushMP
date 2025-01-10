using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public abstract class Packet
    {
        public abstract Packets PacketId { get; }
        public virtual void Read(BinaryReader reader)
        {

        }

        public virtual void Write(BinaryWriter writer)
        {

        }
    }
}
