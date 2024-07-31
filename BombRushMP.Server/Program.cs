using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var port = 34085;
            var server = new Server(port);
            while (true)
            {
                server.Update();
            }
        }
    }
}
