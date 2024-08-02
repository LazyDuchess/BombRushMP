using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class PlayerSpray : PlayerPacket
    {
        public override Packets PacketId => Packets.PlayerSpray;
    }
}
