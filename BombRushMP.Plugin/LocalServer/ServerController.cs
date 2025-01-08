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
        private int _port;
        private BRCServer _server;
        private INetworkingInterface NetworkingInterface => NetworkingEnvironment.NetworkingInterface;

        public ServerController(int port, float tickRate, ushort maxPlayers, bool local)
        {
            if (local)
                maxPlayers = 1;
            _server = new BRCServer(port, maxPlayers, tickRate, local);
            var serverThread = new Thread(Update);
            serverThread.IsBackground = true;
            serverThread.Start();
        }

        private void Update()
        {
            while (true)
            {
                _server.Update();
            }
        }
    }
}
