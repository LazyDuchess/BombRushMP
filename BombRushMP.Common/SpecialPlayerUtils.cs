using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common
{
    public static class SpecialPlayerUtils
    {
        public const string SpecialPlayerName = "Freesoul Elite";
        public const string SpecialPlayerCrewName = "Freesoul";
        public const SpecialSkins SpecialPlayerSkin = SpecialSkins.SpecialPlayer;
        public const string SpecialPlayerTag = "elite";
        public const string SpecialPlayerUnlockName = "Goonie";
        public const string SpecialPlayerUnlockID = "goonieskateboard";

        public const string SpecialPlayerUnlockNotification = $"<color=yellow>You've unlocked the {SpecialPlayerUnlockName} skateboard skin for beating a {SpecialPlayerName}!</color>";
        public const string SpecialPlayerNag = $"<color=yellow>Beat a {SpecialPlayerName} in any gamemode to unlock a skateboard skin.</color>";

        public const string SpecialPlayerJoinMessage = "<color=yellow>A {0} has entered the stage!</color>";
        public const string SpecialPlayerLeaveMessage = "<color=yellow>A {0} has left!</color>";
        public const string SpecialPlayerInviteMessage = "<color=yellow>A {0} is challenging you to a {1}.</color>";
    }
}
