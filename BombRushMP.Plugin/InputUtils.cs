﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin
{
    public static class InputUtils
    {
        public static bool Override = false;
        public static bool InputBlocked
        {
            get
            {
                if (Override) return false;
                var chat = ChatUI.Instance;
                var playerList = PlayerListUI.Instance;
                if (playerList == null) return false;
                if (chat == null) return false;
                return chat.State == ChatUI.States.Focused || playerList.Displaying;
            }
        }
    }
}
