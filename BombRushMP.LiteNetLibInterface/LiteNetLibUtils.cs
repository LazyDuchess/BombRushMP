using BombRushMP.Common.Networking;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BombRushMP.Common.Networking.IMessage;

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

        public static DeliveryMethod SendModeToDeliveryMethod(IMessage.SendModes sendMode)
        {
            switch (sendMode)
            {
                case IMessage.SendModes.Unreliable:
                    return DeliveryMethod.Unreliable;
                case IMessage.SendModes.Reliable:
                    return DeliveryMethod.ReliableOrdered;
                case IMessage.SendModes.ReliableUnordered:
                    return DeliveryMethod.ReliableUnordered;
            }
            return DeliveryMethod.ReliableOrdered;
        }

        public static IMessage.SendModes DeliveryMethodToSendMode(DeliveryMethod deliveryMethod)
        {
            switch (deliveryMethod)
            {
                case DeliveryMethod.Unreliable:
                    return IMessage.SendModes.Unreliable;
                case DeliveryMethod.ReliableOrdered:
                    return IMessage.SendModes.Reliable;
                case DeliveryMethod.ReliableUnordered:
                    return IMessage.SendModes.ReliableUnordered;
            }
            return IMessage.SendModes.Reliable;
        }
    }
}
