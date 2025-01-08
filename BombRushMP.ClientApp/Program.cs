using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.NetworkInterfaceProvider;

namespace BombRushMP.ClientApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            NetworkInterfaceFactory.InitializeNetworkInterface(NetworkInterfaces.LiteNetLib);
            Console.WriteLine("Please, type in the ID of a task you'd like the client to perform:");
            var taskValues = Enum.GetValues(typeof(BRCClient.Tasks));
            foreach(var taskValue in taskValues)
            {
                Console.WriteLine($"ID {(int)taskValue}: {(BRCClient.Tasks)taskValue}");
            }
            var taskID = int.Parse(Console.ReadLine());
            var client = new BRCClient("localhost", 41585, (BRCClient.Tasks)taskID);
            while (true)
            {
                client.Update();
            }
        }
    }
}
