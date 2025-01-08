using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public interface IMessageReceivedEventArgs
    {
        public ushort MessageId { get; }
        public IMessage Message { get; }
    }
}
