using BombRushMP.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin
{
    public class RagdollState
    {
        public List<Quaternion> LimbRotations = new();

        public RagdollState()
        {

        }

        public RagdollState(List<Quaternion> limbRotations)
        {
            LimbRotations = limbRotations;
        }

        public void Read(BinaryReader reader)
        {
            LimbRotations.Clear();
            var version = reader.ReadByte();
            var limbAmount = (int)reader.ReadByte();
            for(var i = 0; i < limbAmount; i++)
            {
                LimbRotations.Add(Compression.ReadCompressedQuaternion(reader));
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)0);
            writer.Write((byte)LimbRotations.Count);
            foreach(var rot in LimbRotations)
            {
                Compression.WriteCompressedQuaternion(rot, writer);
            }
        }
    }
}
