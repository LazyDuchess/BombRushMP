using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin
{
    public class Emojis
    {
        public int[] AprilBadges = [
            38,
            47,
            34,
            35,
            25,
            54,
            55,
            56,
            57,
            58,
            59,
            60,
            61,
            62,
            2
            ];

        public Dictionary<string, int> Sprites = new()
        {
            { ":car:", 25 },
            { ":akkowot:", 24 },
            { ":krehs:", 26 },
            { ":ben:", 38 },
            { ":skull:", 34 },
            { ":pensive:", 46 },
            { ":joy:", 47 },
            { ":trans:", 48 },
            { ":pride:", 49 },
            { ":heart:", 50 },
        };
    }
}
