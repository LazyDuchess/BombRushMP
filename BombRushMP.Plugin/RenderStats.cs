using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin
{
    public static class RenderStats
    {
        public static int Players = 0;
        public static int PlayersCulled = 0;
        public static int PlayersRendered = 0;

        public static void Reset()
        {
            Players = 0;
            PlayersCulled = 0;
            PlayersRendered = 0;
        }
    }
}
