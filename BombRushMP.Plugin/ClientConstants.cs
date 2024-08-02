using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public static class ClientConstants
    {
        public const float PlayerInterpolation = 12f;
        public const float PlayerGraffitiDistance = 1f;
        public const float PlayerGraffitiDownDistance = 1f;
        public static int GrindDirectionHash = Animator.StringToHash("grindDirection");
        public static int PhoneDirectionXHash = Animator.StringToHash("phoneDirectionX");
        public static int PhoneDirectionYHash = Animator.StringToHash("phoneDirectionY");
        public static int TurnDirection1Hash = Animator.StringToHash("turnDirectionX");
        public static int TurnDirection2Hash = Animator.StringToHash("turnDirectionX2");
        public static int TurnDirection3Hash = Animator.StringToHash("turnDirectionX3");
        public static int TurnDirectionSkateboardHash = Animator.StringToHash("turnDirectionSkateboard");
    }
}
