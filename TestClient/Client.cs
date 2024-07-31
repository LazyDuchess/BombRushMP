using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    public class Client : IDisposable
    {
        private TcpClient _tcpClient;
        public Client(string address, int port)
        {
            _tcpClient = new TcpClient(address, port);
            var stream = _tcpClient.GetStream();
            var msg = System.Text.Encoding.ASCII.GetBytes("Hallo!");
            stream.Write(msg, 0, msg.Length);
        }

        public void Stop()
        {
            _tcpClient.Close();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
