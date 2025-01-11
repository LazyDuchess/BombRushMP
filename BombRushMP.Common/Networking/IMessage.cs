using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public interface IMessage
    {
        public enum SendModes
        {
            Unreliable,
            Reliable,
            ReliableUnordered
        }

        public byte[] GetBytes();

        public IMessage Add(byte[] data);
    }
}
