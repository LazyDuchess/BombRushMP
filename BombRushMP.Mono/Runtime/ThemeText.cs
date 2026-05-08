using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if PLUGIN
using TMPro;
#endif

namespace BombRushMP.Mono.Runtime
{
    public class ThemeText : MonoBehaviour
    {
        public TextKind Kind = TextKind.LobbyName;
        public enum TextKind
        {
            LobbyName,
            LobbyPlayerName,
            LobbyPlayerScore,
            LobbySettings,
            Timer,
            MapText,
            ChatText,
            NotificationText
        }

#if PLUGIN
        private void Awake()
        {
            var text = GetComponent<TextMeshProUGUI>();
            var col = Color.white;
            var theme = Theme.CurrentTheme;
            switch (Kind)
            {
                case TextKind.LobbyName:
                    col = theme.LobbyNameColor;
                    break;

                case TextKind.LobbyPlayerName:
                    col = theme.LobbyPlayerNameColor;
                    break;

                case TextKind.LobbyPlayerScore:
                    col = theme.LobbyPlayerScoreColor;
                    break;

                case TextKind.LobbySettings:
                    col = theme.LobbySettingsColor;
                    break;

                case TextKind.Timer:
                    col = theme.TimerColor;
                    break;

                case TextKind.MapText:
                    col = theme.MapTextColor;
                    break;

                case TextKind.ChatText:
                    col = theme.ChatTextColor;
                    break;

                case TextKind.NotificationText:
                    col = theme.NotificationTextColor;
                    break;
            }
            text.color = col;
        }
#endif
    }
}
