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
            NetworkingEnvironment.UseNativeSocketsIfAvailable = serverSettings.UseNativeSockets;
            NetworkingEnvironment.NetworkingInterface = NetworkInterfaceFactory.GetNetworkInterface(serverSettings.NetworkInterface);
            NetworkingEnvironment.LogEventHandler += (log) =>
            {
                ServerLogger.Log($"[{nameof(NetworkingEnvironment)}] {log}");
            };
            PacketFactory.Initialize();
            var port = (ushort)41585;
            var db = new ServerAppDatabase();
            var server = new BRCServer(port, 65534, 1f/serverSettings.TicksPerSecond, db);
            server.LogMessages = serverSettings.LogChats;
            server.AllowNameChanges = serverSettings.AllowNameChanges;
            server.ChatCooldown = serverSettings.ChatCooldown;
            while(true)
            {
                server.Update();
            }
        }
    }
}
