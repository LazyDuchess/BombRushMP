using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public class MessageReceivedEventArgs
    {
        public readonly ushort MessageId;
        public readonly IMessage Message;
        public readonly INetConnection FromConnection;

        public MessageReceivedEventArgs(ushort messageId, IMessage message, INetConnection fromConnection)
        {
            MessageId = messageId;
            Message = message;
            FromConnection = fromConnection;
        }
    }
}
