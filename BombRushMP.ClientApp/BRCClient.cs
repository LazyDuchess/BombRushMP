using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Riptide;
using Riptide.Utils;

namespace BombRushMP.ClientApp
{
    public class BRCClient : IDisposable
    {
        private Client _client;
        public BRCClient(string address)
        {
            _client = new Client();
            _client.Connect(address);
        }

        public void Update()
        {
            _client.Update();
        }

        public void Stop()
        {
            _client.Disconnect();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
