using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class PlayerGraffitiFinisher : PlayerPacket
    {
        public override Packets PacketId => Packets.PlayerGraffitiFinisher;
    }
}
