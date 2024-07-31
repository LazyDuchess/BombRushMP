using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var address = "localhost";
            var port = 34085;
            var client = new Client(address, port);
            while (true)
            {
                //hello
            }
        }
    }
}
