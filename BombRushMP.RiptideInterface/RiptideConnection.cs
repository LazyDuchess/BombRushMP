using BombRushMP.Common.Networking;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.RiptideInterface
{
    public class RiptideConnection : INetConnection
    {
        public bool CanQualityDisconnect
        {
            get
            {
                return _riptideConnection.CanQualityDisconnect;
            }

            set
            {
                _riptideConnection.CanQualityDisconnect = value;
            }
        }
        public ushort Id => _riptideConnection.Id;
        private Riptide.Connection _riptideConnection;
        public string Address => _riptideConnection.ToString().Split(':')[0];
        public RiptideConnection(Riptide.Connection connection)
        {
            _riptideConnection = connection;
        }
        public void Send(IMessage message)
        {
            _riptideConnection.Send((message as RiptideMessage).InternalRiptideMessage);
        }

        public override string ToString()
        {
            return _riptideConnection.ToString();
        }
    }
}
