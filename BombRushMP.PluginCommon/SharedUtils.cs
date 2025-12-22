using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using BombRushMP.Common;

namespace BombRushMP.PluginCommon
{
    public static class SharedUtils
    {
        public static Action<Characters> SetLocalCharacter;
        public static Action<SpecialSkins> SetLocalSpecialSkin;

    }
}
