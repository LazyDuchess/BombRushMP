using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public class RiptideMessage : IMessage
    {
        public Riptide.Message InternalRiptideMessage { get; private set; }
        public RiptideMessage(Riptide.Message sourceMessage)
        {
            InternalRiptideMessage = sourceMessage;
        }

        public byte[] GetBytes()
        {
            return InternalRiptideMessage.GetBytes();
        }

        public IMessage Add(byte[] data)
        {
            InternalRiptideMessage.Add(data);
            return this;
        }
    }
}
