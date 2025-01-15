using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientGamemodeCountdown : Packet
    {
        public override Packets PacketId => Packets.ClientGamemodeCountdown;
        public ushort CountdownSeconds = 3;

        public ClientGamemodeCountdown()
        {

        }

        public ClientGamemodeCountdown(ushort seconds)
        {
            CountdownSeconds = seconds;
        }

        public override void Read(BinaryReader reader)
        {
            CountdownSeconds = reader.ReadUInt16();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(CountdownSeconds);
        }
    }
}
