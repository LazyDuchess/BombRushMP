﻿using BombRushMP.Common.Packets;
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

namespace BombRushMP.ServerApp
{
    internal class Program
    {
        private const string ServerSettingsPath = "server.json";
        private const string ServerTagsPath = "tags.txt";

        static void Restart()
        {
            ServerLogger.Log("Shutting down server!");
            Environment.Exit(0);
        }

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
            var db = new ServerAppDatabase();
            var server = new BRCServer(serverSettings.Port, serverSettings.MaxPlayers, 1f/serverSettings.TicksPerSecond, db);
            PlayerAnimation.ServerSendMode = serverSettings.ServerAnimationSendMode;
            server.ClientAnimationSendMode = serverSettings.ClientAnimationSendMode;
            server.LogMessagesToFile = serverSettings.LogChatsToFiles;
            server.LogMessages = serverSettings.LogChats;
            server.AllowNameChanges = serverSettings.AllowNameChanges;
            server.ChatCooldown = serverSettings.ChatCooldown;
            server.RestartAction = Restart;
            server.MOTD = serverSettings.MOTD;
            server.AlwaysShowMOTD = serverSettings.AlwaysShowMOTD;
            if (File.Exists(ServerTagsPath))
            {
                var tags = File.ReadAllLines(ServerTagsPath);
                foreach(var tag in tags)
                {
                    server.ServerState.Tags.Add(tag);
                }
            }
            while(true)
            {
                server.Update();
            }
        }
    }
}
