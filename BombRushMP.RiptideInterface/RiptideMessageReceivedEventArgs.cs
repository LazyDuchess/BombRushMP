using BombRushMP.Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.RiptideInterface
{
    public class RiptideMessageReceivedEventArgs : IMessageReceivedEventArgs
    {
        public INetConnection FromConnection => _fromConnection;
        public ushort MessageId => RiptideMessageEvent.MessageId;
        public IMessage Message => _message;
        public Riptide.MessageReceivedEventArgs RiptideMessageEvent;
        private RiptideMessage _message;
        private RiptideConnection _fromConnection;
        public RiptideMessageReceivedEventArgs(Riptide.MessageReceivedEventArgs message)
        {
            RiptideMessageEvent = message;
            _message = new RiptideMessage(RiptideMessageEvent.Message);
            _fromConnection = new RiptideConnection(RiptideMessageEvent.FromConnection);
        }
    }
}
