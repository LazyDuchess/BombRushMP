using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.ClientApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var address = "127.0.0.1:34085";
            var client = new BRCClient(address);
            while (true)
            {
                client.Update();
            }
        }
    }
}
