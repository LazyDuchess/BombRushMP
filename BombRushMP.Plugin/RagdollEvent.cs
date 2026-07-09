using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public RagdollEvent()
        {

        }

        public RagdollEvent(Events ev){
            Event = ev;
        }

        public RagdollEvent(Events ev, RagdollState state)
        {
            Event = ev;
            State = state;
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
        }
    }
}
