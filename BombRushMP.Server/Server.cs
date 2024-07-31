using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BombRushMP.Server
{
    public class Server : IDisposable
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
            _tcpServer.Stop();
        }

        public void Dispose()
        {
            Stop();
        }

        private void TCPWaitForClients()
        {
            var client = _tcpServer.AcceptTcpClient();
            Log($"Accepting client connection from address {(client.Client.RemoteEndPoint as IPEndPoint).Address}");
            ThreadPool.QueueUserWorkItem(TCPProcessClient, client);
        }

        private void TCPProcessClient(object obj)
        {
            var connectionOpen = true;
            var client = (TcpClient)obj;
            var stream = client.GetStream();
            while (connectionOpen)
            {
                var bytes = new byte[client.ReceiveBufferSize];
                try
                {
                    stream.Read(bytes, 0, client.ReceiveBufferSize);
                }
                catch (Exception)
                {
                    connectionOpen = false;
                }
                if (connectionOpen)
                {
                    string msg = Encoding.ASCII.GetString(bytes); //the message incoming
                    Log(msg);
                }
            }
            Log($"Client connection from address {(client.Client.RemoteEndPoint as IPEndPoint).Address} was closed.");
            client.Close();
        }

        private void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
