using BombRushMP.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin
{
    public class RagdollState
    {
        public List<Quaternion> LimbRotations = new();
        public ulong PacketId = 0;

        public RagdollState()
        {

        }

        public RagdollState(List<Quaternion> limbRotations, ulong packetId)
        {
            LimbRotations = limbRotations;
            PacketId = packetId;
        }

        public void Read(BinaryReader reader)
        {
            LimbRotations.Clear();
            var version = reader.ReadByte();
            PacketId = reader.ReadUInt64();
            var limbAmount = (int)reader.ReadByte();
            for(var i = 0; i < limbAmount; i++)
            {
                LimbRotations.Add(Compression.ReadCompressedQuaternion(reader).ToUnityQuaternion());
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)0);
            writer.Write(PacketId);
            writer.Write((byte)LimbRotations.Count);
            foreach(var rot in LimbRotations)
            {
                Compression.WriteCompressedQuaternion(rot.ToSystemQuaternion(), writer);
            }
        }
    }
}
