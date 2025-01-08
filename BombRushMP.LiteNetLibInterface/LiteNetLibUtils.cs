using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.LiteNetLibInterface
{
    public static class LiteNetLibUtils
    {
        public static ushort PeerIdToGameId(int peerId)
        {
            return (ushort)(peerId + 1);
        }

        public static int GameIdToPeerId(ushort gameId)
        {
            return gameId - 1;
        }
    }
}
