using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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

        public static void WriteCompressedQuaternion(Quaternion quat, BinaryWriter writer)
        {
            writer.Write(CompressNormal(quat.X));
            writer.Write(CompressNormal(quat.Y));
            writer.Write(CompressNormal(quat.Z));
            writer.Write(CompressNormal(quat.W));
        }

        public static Quaternion ReadCompressedQuaternion(BinaryReader reader)
        {
            var x = DecompressNormal(reader.ReadSByte());
            var y = DecompressNormal(reader.ReadSByte());
            var z = DecompressNormal(reader.ReadSByte());
            var w = DecompressNormal(reader.ReadSByte());
            return new Quaternion(x, y, z, w);
        }

        public static void WriteCompressedVector3Normal(Vector3 vec, BinaryWriter writer)
        {
            writer.Write(CompressNormal(vec.X));
            writer.Write(CompressNormal(vec.Y));
            writer.Write(CompressNormal(vec.Z));
        }

        public static Vector3 ReadCompressedVector3Normal(BinaryReader reader)
        {
            var x = DecompressNormal(reader.ReadSByte());
            var y = DecompressNormal(reader.ReadSByte());
            var z = DecompressNormal(reader.ReadSByte());
            return new Vector3(x, y, z);
        }
    }
}
