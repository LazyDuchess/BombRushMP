using BombRushMP.Common.Networking;
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

        public ServerController(int port, float tickRate, ushort maxPlayers)
        {
            Instance = this;
            Server = new BRCServer(port, maxPlayers, tickRate);
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
