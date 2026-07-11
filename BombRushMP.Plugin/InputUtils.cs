using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Mono.Runtime;
using BombRushMP.PluginCommon;

namespace BombRushMP.Plugin
{
    public static class InputUtils
    {
        public static bool InputBlocked
        {
            get
            {
                if (SharedUtils.OverrideInput) return false;
                var chat = ChatUI.Instance;
                var playerList = PlayerListUI.Instance;
                var textInput = TextInput.Instance;
                if (playerList == null) return false;
                if (chat == null) return false;
                if (textInput == null) return false;
                return chat.State == ChatUI.States.Focused || playerList.Displaying || textInput.Open;
            }
        }
    }
}
