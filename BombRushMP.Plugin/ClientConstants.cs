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
        public static int GrindDirectionHash = Animator.StringToHash("grindDirection");
    }
}
