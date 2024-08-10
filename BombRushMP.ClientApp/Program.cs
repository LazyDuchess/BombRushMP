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
            Console.WriteLine("Please, type in the ID of a task you'd like the client to perform:");
            var taskValues = Enum.GetValues(typeof(BRCClient.Tasks));
            foreach(var taskValue in taskValues)
            {
                Console.WriteLine($"ID {(int)taskValue}: {(BRCClient.Tasks)taskValue}");
            }
            var taskID = int.Parse(Console.ReadLine());
            var address = "127.0.0.1:41585";
            var client = new BRCClient(address, (BRCClient.Tasks)taskID);
            while (true)
            {
                client.Update();
            }
        }
    }
}
