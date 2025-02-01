using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common
{
    public class ServerState
    {
        public HashSet<string> Tags = new();

        public void Read(BinaryReader reader)
        {
            var tagCount = reader.ReadInt32();
            for(var i = 0; i < tagCount; i++)
            {
                Tags.Add(reader.ReadString());
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Tags.Count);
            foreach(var tag in Tags)
            {
                writer.Write(tag);
            }
        }
    }
}
