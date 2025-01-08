using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.NetworkInterfaceProvider;
using System.IO;
using Newtonsoft.Json;
using BombRushMP.Common.Networking;

namespace BombRushMP.ClientApp
{
    internal class Program
    {
        private const string ClientSettingsPath = "client.json";
        static void Main(string[] args)
        {
            ClientSettings clientSettings = null;
            if (!File.Exists(ClientSettingsPath))
            {
                clientSettings = new ClientSettings();
            }
            else
            {
                clientSettings = JsonConvert.DeserializeObject<ClientSettings>(File.ReadAllText(ClientSettingsPath));
            }
            File.WriteAllText(ClientSettingsPath, JsonConvert.SerializeObject(clientSettings, Formatting.Indented));
            var netInterface = NetworkInterfaces.LiteNetLib;
            if (Enum.TryParse<NetworkInterfaces>(clientSettings.NetworkInterface, out var result))
                netInterface = result;
            NetworkInterfaceFactory.InitializeNetworkInterface(netInterface);
            NetworkingEnvironment.LogEventHandler += (log) =>
            {
                Console.WriteLine($"[{nameof(NetworkingEnvironment)}] {log}");
            };
            Console.WriteLine("Please, type in the ID of a task you'd like the client to perform:");
            var taskValues = Enum.GetValues(typeof(BRCClient.Tasks));
            foreach(var taskValue in taskValues)
            {
                Console.WriteLine($"ID {(int)taskValue}: {(BRCClient.Tasks)taskValue}");
            }
            var taskID = int.Parse(Console.ReadLine());
            var client = new BRCClient(clientSettings.Address, clientSettings.Port, (BRCClient.Tasks)taskID);
            while (true)
            {
                client.Update();
            }
        }
    }
}
