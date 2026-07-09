using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common;

namespace BombRushMP.Plugin
{
    public class RagdollEvent
    {
        public enum Events
        {
            Start,
            Stop
        }
        public Events Event = Events.Start;
        public RagdollState State = null;
        public float Force = 0f;
        public Vector3 ForcePosition = Vector3.zero;
        public Vector3 FixedForce = Vector3.zero;

        public RagdollEvent()
        {

        }

        public RagdollEvent(Events ev){
            Event = ev;
        }

        public RagdollEvent(Events ev, RagdollState state, float force, Vector3 forcePosition, Vector3 fixedForce)
        {
            Event = ev;
            State = state;
            Force = force;
            ForcePosition = forcePosition;
            FixedForce = fixedForce;
        }

        public void Read(BinaryReader reader)
        {
            var version = reader.ReadByte();
            Event = (Events)reader.ReadInt32();
            var hasState = reader.ReadBoolean();
            if (hasState)
            {
                State = new RagdollState();
                State.Read(reader);
            }
            if (Event == Events.Start)
            {
                Force = reader.ReadSingle();
                ForcePosition.x = reader.ReadSingle();
                ForcePosition.y = reader.ReadSingle();
                ForcePosition.z = reader.ReadSingle();
                FixedForce.x = reader.ReadSingle();
                FixedForce.y = reader.ReadSingle();
                FixedForce.z = reader.ReadSingle();
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)0);
            writer.Write((int)Event);
            writer.Write(State != null);
            if (State != null)
            {
                State.Write(writer);
            }
            if (Event == Events.Start)
            {
                writer.Write(Force);
                writer.Write(ForcePosition.x);
                writer.Write(ForcePosition.y);
                writer.Write(ForcePosition.z);
                writer.Write(FixedForce.x);
                writer.Write(FixedForce.y);
                writer.Write(FixedForce.z);
            }
        }
    }
}
