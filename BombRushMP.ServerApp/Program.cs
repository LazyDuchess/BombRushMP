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
using System.Diagnostics;
using BombRushMP.Common;

namespace BombRushMP.ServerApp
{
    internal class Program
    {
        private const string ServerSettingsPath = "server.json";
        private const string ServerTagsPath = "tags.txt";
        private static ServerSettings ServerSettings = null;
        private static BRCServer Server = null;

        static void Restart()
        {
            ServerLogger.Log("Shutting down server!");
            Environment.Exit(0);
        }

        static void OnSave()
        {
            ServerSettings.MOTD = Server.MOTD;
            ServerSettings.AlwaysShowMOTD = Server.AlwaysShowMOTD;
            ServerSettings.AllowNameChanges = Server.AllowNameChanges;
            File.WriteAllText(ServerSettingsPath, JsonConvert.SerializeObject(ServerSettings, Formatting.Indented));
        }

        static void Main(string[] args)
        {
            if (!File.Exists(ServerSettingsPath))
            {
                ServerSettings = new ServerSettings();
            }
            else
            {
                ServerSettings = JsonConvert.DeserializeObject<ServerSettings>(File.ReadAllText(ServerSettingsPath));
            }
            File.WriteAllText(ServerSettingsPath, JsonConvert.SerializeObject(ServerSettings, Formatting.Indented));
            NetworkingEnvironment.UseNativeSocketsIfAvailable = ServerSettings.UseNativeSockets;
            NetworkingEnvironment.NetworkingInterface = NetworkInterfaceFactory.GetNetworkInterface(ServerSettings.NetworkInterface);
            NetworkingEnvironment.LogEventHandler += (log) =>
            {
                ServerLogger.Log($"[{nameof(NetworkingEnvironment)}] {log}");
            };
            PacketFactory.Initialize();
            var db = new ServerAppDatabase(ServerSettings.WebServer, ServerSettings.DatabaseConnectionString);
            var ticks = 1f / ServerSettings.TicksPerSecond;
            Server = new BRCServer(ServerSettings.Port, ServerSettings.MaxPlayers, ticks, db, ServerSettings.CustomPackets);
            PlayerAnimation.ServerSendMode = ServerSettings.ServerAnimationSendMode;
            Server.ClientAnimationSendMode = ServerSettings.ClientAnimationSendMode;
            Server.LogMessagesToFile = ServerSettings.LogChatsToFiles;
            Server.LogMessages = ServerSettings.LogChats;
            Server.AllowNameChanges = ServerSettings.AllowNameChanges;
            Server.ChatCooldown = ServerSettings.ChatCooldown;
            Server.RestartAction = Restart;
            Server.MOTD = ServerSettings.MOTD;
            Server.AlwaysShowMOTD = ServerSettings.AlwaysShowMOTD;
            if (File.Exists(ServerTagsPath))
            {
                var tags = File.ReadAllLines(ServerTagsPath);
                foreach(var tag in tags)
                {
                    Server.ServerState.Tags.Add(tag);
                }
            }
            if (ServerSettings.WebServer)
            {
                var webServer = new WebServer(Server);
                webServer.ServerManagedAction += OnSave;
                webServer.Start(ServerSettings.WebServerFrontendURL, ServerSettings.DiscordClientId, ServerSettings.DiscordClientSecret, ServerSettings.DiscordCallback, ServerSettings.DatabaseConnectionString);
            }
            while(true)
            {
                Server.Update();
            }
        }
    }
}
