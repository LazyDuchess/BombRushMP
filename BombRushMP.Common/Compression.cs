using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common
{
    public static class Compression
    {
        public static sbyte CompressNormal(float normalValue)
        {
            return (sbyte)(normalValue * sbyte.MaxValue);
        }

        public static float DecompressNormal(sbyte compressedValue)
        {
            return (float)compressedValue / sbyte.MaxValue;
        }
    }
}
