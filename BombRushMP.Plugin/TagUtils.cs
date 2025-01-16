using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin
{
    public static class TagUtils
    {
        public static GraffitiArt GetRandomGraffitiArt(GraffitiSpot spot, Player player)
        {
            if (spot.size == GraffitiSize.S)
                return spot.GraffitiArtInfo.FindByCharacter(player.character);
            var grafs = spot.GraffitiArtInfo.graffitiArt.Where((art) =>
            {
                return art.graffitiSize == spot.size;
            }).ToList();
            return grafs[UnityEngine.Random.Range(0, grafs.Count)];
        }
    }
}
