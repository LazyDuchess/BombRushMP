using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class RagdollLaunchPacket
    {
        public Vector3 Point = Vector3.zero;
        public float Force = 0f;
        public float UpForce = 0f;

        public RagdollLaunchPacket()
        {

        }

        public RagdollLaunchPacket(Vector3 point, float force, float upForce)
        {
            Point = point;
            Force = force;
            UpForce = upForce;
        }

        public void Read(BinaryReader reader)
        {
            var version = reader.ReadByte();
            Point.x = reader.ReadSingle();
            Point.y = reader.ReadSingle();
            Point.z = reader.ReadSingle();
            Force = reader.ReadSingle();
            UpForce = reader.ReadSingle();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)0);
            writer.Write(Point.x);
            writer.Write(Point.y);
            writer.Write(Point.z);
            writer.Write(Force);
            writer.Write(UpForce);
        }
    }
}
