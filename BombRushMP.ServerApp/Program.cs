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
            var port = (ushort)34085;
            var server = new BRCServer(port, 65534);
            while(true)
            {
                server.Update();
            }
        }
    }
}
