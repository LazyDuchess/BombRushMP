using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BombRushMP.Mono.Runtime
{
    public class ThemeImage : MonoBehaviour
    {
        public ImageKind Kind = ImageKind.LobbyNameBackground;
        public enum ImageKind
        {
            LobbyNameBackground,
            LobbyPlayerNameBackground,
            MapOutline,
            MapBackground,
            MapTexture,
            MapIcon,
            NotificationBackground,
            NotificationIcon,
            ChatScrollView,
            ChatScrollBar,
            ChatForeground,
        }

#if PLUGIN
        private void Awake()
        {
            var img = GetComponent<Image>();
            var rawimg = GetComponent<RawImage>();
            var col = Color.white;
            var theme = Theme.CurrentTheme;
            switch (Kind)
            {
                case ImageKind.LobbyNameBackground:
                    col = theme.LobbyNameBackgroundColor;
                    break;

                case ImageKind.LobbyPlayerNameBackground:
                    col = theme.LobbyPlayerBackgroundColor;
                    break;

                case ImageKind.MapOutline:
                    col = theme.MapOutlineColor;
                    break;

                case ImageKind.MapBackground:
                    col = theme.MapBackgroundColor;
                    break;

                case ImageKind.MapTexture:
                    col = theme.MapContentsColor;
                    break;

                case ImageKind.MapIcon:
                    col = theme.MapIconsColor;
                    break;

                case ImageKind.NotificationBackground:
                    col = theme.NotificationBackgroundColor;
                    break;

                case ImageKind.NotificationIcon:
                    col = theme.NotificationIconColor;
                    break;

                case ImageKind.ChatScrollView:
                    col = theme.ChatScrollViewColor;
                    break;

                case ImageKind.ChatScrollBar:
                    col = theme.ChatScrollBarColor;
                    break;

                case ImageKind.ChatForeground:
                    col = theme.ChatForegroundColor;
                    break;
            }

            if (img != null)
                img.color = col;
            if (rawimg != null)
                rawimg.color = col;
        }
#endif
    }
}
