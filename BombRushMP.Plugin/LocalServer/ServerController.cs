using BombRushMP.Common.Networking;
using BombRushMP.Plugin.OfflineInterface;
using BombRushMP.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.LocalServer
{
    public class ServerController
    {
        public static ServerController Instance { get; private set; }
        public BRCServer Server;
        private int _port;
        private INetworkingInterface NetworkingInterface => NetworkingEnvironment.NetworkingInterface;

        public ServerController(int port, float tickRate, ushort maxPlayers, bool offline)
        {
            Instance = this;
            var db = new LocalServerDatabase(offline);
            Server = new BRCServer(port, maxPlayers, tickRate, db);
            var serverThread = new Thread(Update);
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        private void Update()
        {
            while (true)
            {
                Server.Update();
            }
        }
    }
}
