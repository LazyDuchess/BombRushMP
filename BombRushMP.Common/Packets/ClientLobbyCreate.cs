using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientLobbyCreate : Packet
    {
        public override Packets PacketId => Packets.ClientLobbyCreate;
        public GamemodeIDs GamemodeID;
        public byte[] Settings;

        public ClientLobbyCreate()
        {

        }

        public ClientLobbyCreate(GamemodeIDs gamemode, byte[] settings)
        {
            GamemodeID = gamemode;
            Settings = settings;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write((int)GamemodeID);
            writer.Write(Settings.Length);
            writer.Write(Settings);
        }

        public override void Read(BinaryReader reader)
        {
            GamemodeID = (GamemodeIDs)reader.ReadInt32();
            var settingsLength = reader.ReadInt32();
            Settings = reader.ReadBytes(settingsLength);
        }
    }
}
