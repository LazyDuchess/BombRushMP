﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientComboOver : Packet
    {
        public override Packets PacketId => Packets.ClientComboOver;
    }
}
