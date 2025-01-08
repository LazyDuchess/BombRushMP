﻿using BombRushMP.Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.RiptideInterface
{
    public static class RiptideUtils
    {
        public static IMessageReceivedEventArgs RiptideMessageReceivedToInterface(Riptide.MessageReceivedEventArgs ripargs)
        {
            var args = new RiptideMessageReceivedEventArgs(ripargs);
            return args;
        }

        public static Riptide.MessageSendMode SendModeToRiptide(IMessage.SendModes sendMode)
        {
            if (sendMode == IMessage.SendModes.Unreliable) return Riptide.MessageSendMode.Unreliable;
            return Riptide.MessageSendMode.Reliable;
        }
    }
}
