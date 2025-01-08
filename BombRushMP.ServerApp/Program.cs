using BombRushMP.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.NetworkInterfaceProvider;
using Newtonsoft.Json;
using System.IO;
using BombRushMP.Common.Networking;
using BombRushMP.Server;

namespace BombRushMP.ServerApp
{
    internal class Program
    {
        private const string ServerSettingsPath = "server.json";
        static void Main(string[] args)
        {
            ServerSettings serverSettings = null;
            if (!File.Exists(ServerSettingsPath))
            {
                serverSettings = new ServerSettings();
            }
            else
            {
                serverSettings = JsonConvert.DeserializeObject<ServerSettings>(File.ReadAllText(ServerSettingsPath));
            }
            File.WriteAllText(ServerSettingsPath, JsonConvert.SerializeObject(serverSettings, Formatting.Indented));
            var netInterface = NetworkInterfaces.LiteNetLib;
            if (Enum.TryParse<NetworkInterfaces>(serverSettings.NetworkInterface, out var result))
                netInterface = result;
            NetworkingEnvironment.NetworkingInterface = NetworkInterfaceFactory.GetNetworkInterface(netInterface);
            NetworkingEnvironment.LogEventHandler += (log) =>
            {
                ServerLogger.Log($"[{nameof(NetworkingEnvironment)}] {log}");
            };
            PacketFactory.Initialize();
            var port = (ushort)41585;
            var server = new BRCServer(port, 65534, 1f/serverSettings.TicksPerSecond);
            while(true)
            {
                server.Update();
            }
        }
    }
}
