using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common
{
    public class CustomPacketBannedMods
    {
        public const string PacketId = "ACN-BANNEDMODS";
        public const string JoinedLobbyPacketId = "ACN-BANNEDMODS-JOINEDLOBBY";
        public string[] BannedMods;

        public CustomPacketBannedMods(string[] bannedMods)
        {
            BannedMods = bannedMods;
        }

        public CustomPacketBannedMods()
        {

        }

        public byte[] Serialize()
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            writer.Write(BannedMods.Length);
            foreach(var bannedMod in BannedMods)
            {
                writer.Write(bannedMod);
            }
            return ms.ToArray();
        }

        public void Deserialize(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var reader = new BinaryReader(ms);
            int length = reader.ReadInt32();
            BannedMods = new string[length];
            for(int i = 0; i < length; i++)
            {
                BannedMods[i] = reader.ReadString();
            }
        }
    }
}
