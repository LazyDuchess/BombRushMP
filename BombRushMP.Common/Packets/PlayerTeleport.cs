﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class PlayerTeleport : PlayerPacket
    {
        public override Packets PacketId => Packets.PlayerTeleport;
    }
}
