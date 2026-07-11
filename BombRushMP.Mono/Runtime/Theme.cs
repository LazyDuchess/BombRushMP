using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Mono.Runtime
{
    public class Theme
    {
        public static Theme CurrentTheme;

        public Color LobbyNameBackgroundColor { get; private set; }
        public Color LobbyNameColor { get; private set; }

        public Color LobbyPlayerBackgroundColor { get; private set; }
        public Color LobbyPlayerNameColor { get; private set; }
        public Color LobbyPlayerScoreColor { get; private set; }

        public Color LobbySettingsColor { get; private set; }

        public Color TimerColor { get; private set; }

        public Color MapOutlineColor { get; private set; }
        public Color MapBackgroundColor { get; private set; }
        public Color MapContentsColor { get; private set; }
        public Color MapIconsColor { get; private set; }
        public Color MapTextColor { get; private set; }

        public Color ChatTextColor { get; private set; }
        public Color ChatScrollViewColor { get; private set; }
        public Color ChatScrollBarColor { get; private set; }
        public Color ChatForegroundColor { get; private set; }


        public Color NotificationBackgroundColor { get; private set; }
        public Color NotificationTextColor { get; private set; }
        public Color NotificationIconColor { get; private set; }

        public Color GraffitiSwirlColor { get; private set; }

        public Color RaceGraffitiSwirlColor { get; private set; }

        public Color MapPlayerColor { get; private set; }
        public Color MapRivalColor { get; private set; }
        public Color MapFriendlyColor { get; private set; }
        public Color MapPvPColor { get; private set; }

        public float RaceReticleSize { get; private set; }

        public Texture2D GraffitiReticle;

        public float LobbyPlayerTeamBackgroundAlpha { get; private set; }

        public void ParseConfig(ThemeConfig config)
        {
            LobbyNameBackgroundColor = ColorFromHex(config.LobbyNameBackgroundColor);
            LobbyNameColor = ColorFromHex(config.LobbyNameColor);

            LobbyPlayerBackgroundColor = ColorFromHex(config.LobbyPlayerBackgroundColor);
            LobbyPlayerNameColor = ColorFromHex(config.LobbyPlayerNameColor);
            LobbyPlayerScoreColor = ColorFromHex(config.LobbyPlayerScoreColor);

            LobbySettingsColor = ColorFromHex(config.LobbySettingsColor);

            TimerColor = ColorFromHex(config.TimerColor);

            MapOutlineColor = ColorFromHex(config.MapOutlineColor);
            MapBackgroundColor = ColorFromHex(config.MapBackgroundColor);
            MapContentsColor = ColorFromHex(config.MapContentsColor);
            MapIconsColor = ColorFromHex(config.MapIconsColor);
            MapTextColor = ColorFromHex(config.MapTextColor);

            ChatTextColor = ColorFromHex(config.ChatTextColor);
            ChatScrollViewColor = ColorFromHex(config.ChatScrollViewColor);
            ChatScrollBarColor = ColorFromHex(config.ChatScrollBarColor);
            ChatForegroundColor = ColorFromHex(config.ChatForegroundColor);

            NotificationBackgroundColor = ColorFromHex(config.NotificationBackgroundColor);
            NotificationTextColor = ColorFromHex(config.NotificationTextColor);
            NotificationIconColor = ColorFromHex(config.NotificationIconColor);

            GraffitiSwirlColor = ColorFromHex(config.GraffitiSwirlColor);
            RaceGraffitiSwirlColor = ColorFromHex(config.RaceGraffitiSwirlColor);

            MapPlayerColor = ColorFromHex(config.MapPlayerColor);
            MapRivalColor = ColorFromHex(config.MapRivalColor);
            MapFriendlyColor = ColorFromHex(config.MapFriendlyColor);
            MapPvPColor = ColorFromHex(config.MapPvPColor);

            RaceReticleSize = config.RaceReticleSize;

            LobbyPlayerTeamBackgroundAlpha = config.LobbyPlayerTeamBackgroundAlpha;
        }

        private Color ColorFromHex(string hex)
        {
            if (hex[0] != '#')
                throw new ArgumentException("Color code does not begin with #", "hex");
            var substr = hex.Substring(1);
            var isrgba = false;
            if (substr.Length == 8)
                isrgba = true;
            else if (substr.Length != 6)
                throw new ArgumentException("Color code is not 6 (RGB) or 8 (RGBA) characters long", "hex");
            var rComponent = int.Parse(substr.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
            var gComponent = int.Parse(substr.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
            var bComponent = int.Parse(substr.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
            var aComponent = 1f;
            if (isrgba)
            {
                aComponent = int.Parse(substr.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) / 255f;
            }
            return new Color(rComponent, gComponent, bComponent, aComponent);
        }
    }
}
