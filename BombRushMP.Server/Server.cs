using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BombRushMP.Server
{
    public class Server
    {
        private TcpListener _tcpServer;
        public Server(int port)
        {
            _tcpServer = new TcpListener(IPAddress.Any, port);
            Log($"Starting TCP Server on port {port}");
            _tcpServer.Start();
        }
        
        public void Update()
        {
            TCPWaitForClients();
        }

        public void Stop()
        {
            if (_tcpServer != null)
                _tcpServer.Stop();
        }

        private void TCPWaitForClients()
        {
            var client = _tcpServer.AcceptTcpClient();
            Log($"Accepting client connection from address {(client.Client.RemoteEndPoint as IPEndPoint).Address}");
            ThreadPool.QueueUserWorkItem(TCPProcessClient, client);
        }

        private void TCPProcessClient(object obj)
        {
            var client = (TcpClient)obj;
        }

        private void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
