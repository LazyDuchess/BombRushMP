using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public interface INetConnection
    {
        public bool CanQualityDisconnect { get; set; }
        public ushort Id { get; }
        public void Send(IMessage message);
    }
}
