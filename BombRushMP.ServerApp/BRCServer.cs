using Riptide;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Riptide.Utils;
using Riptide.Transports.Udp;

namespace BombRushMP.ServerApp
{
    public class BRCServer : IDisposable
    {
        private Server _server;
        public BRCServer(ushort port, ushort maxPlayers)
        {
            _server = new Server();
            _server.ClientConnected += OnClientConnected;
            _server.ClientDisconnected += OnClientDisconnected;
            _server.Start(port, maxPlayers);
            Log($"Starting server on port {port} with max players {maxPlayers}");
        }

        public void Update()
        {
            _server.Update();
        }

        public void Dispose()
        {
            _server.Stop();
        }

        private void OnClientConnected(object sender, ServerConnectedEventArgs e)
        {
            Log($"Client connected {e.Client}");
        }

        private void OnClientDisconnected(object sender, ServerDisconnectedEventArgs e)
        {
            Log($"Client disconnected {e.Client.Id}");
        }

        private void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
