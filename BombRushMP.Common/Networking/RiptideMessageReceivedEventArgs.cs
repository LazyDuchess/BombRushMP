using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public class RiptideMessageReceivedEventArgs : IMessageReceivedEventArgs
    {
        public ushort MessageId => RiptideMessageEvent.MessageId;
        public IMessage Message => _message;
        public Riptide.MessageReceivedEventArgs RiptideMessageEvent;
        private RiptideMessage _message;
        public RiptideMessageReceivedEventArgs(Riptide.MessageReceivedEventArgs message)
        {
            RiptideMessageEvent = message;
            _message = new RiptideMessage(RiptideMessageEvent.Message);
        }
    }
}
