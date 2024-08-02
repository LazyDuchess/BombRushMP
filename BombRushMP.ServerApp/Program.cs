﻿using BombRushMP.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.ServerApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            PacketFactory.Initialize();
            var port = (ushort)41585;
            var server = new BRCServer(port, 65534);
            while(true)
            {
                server.Update();
            }
        }
    }
}
