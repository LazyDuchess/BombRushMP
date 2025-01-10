using BombRushMP.Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.OfflineInterface
{
    public class OfflineConnection : INetConnection
    {
        public bool CanQualityDisconnect { get; set; }

        public ushort Id => _server ? OfflineInterface.ServerId : OfflineInterface.ClientId;
        private bool _server = false;
        public string Address => "Local";

        public OfflineConnection(bool server)
        {
            _server = server;
        }

        public void Send(IMessage message)
        {
            if (_server)
            {
                lock (OfflineInterface.ClientToServerMessages)
                {
                    OfflineInterface.ClientToServerMessages.Add(message as OfflineMessage);
                }
            }
            else
            {
                lock (OfflineInterface.ServerToClientMessages)
                {
                    OfflineInterface.ServerToClientMessages.Add(message as OfflineMessage);
                }
            }
        }

        public override string ToString()
        {
            return _server ? "Server" : "Client";
        }
    }
}
