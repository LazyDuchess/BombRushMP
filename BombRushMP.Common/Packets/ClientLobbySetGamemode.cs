using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientLobbySetGamemode : Packet
    {
        public override Packets PacketId => Packets.ClientLobbySetGamemode;
        public GamemodeIDs Gamemode = GamemodeIDs.ScoreBattle;
        public byte[] GamemodeSettings = [];

        public ClientLobbySetGamemode()
        {

        }

        public ClientLobbySetGamemode(GamemodeIDs gamemode, byte[] gamemodeSettings)
        {
            Gamemode = gamemode;
            GamemodeSettings = gamemodeSettings;
        }

        public override void Read(BinaryReader reader)
        {
            Gamemode = (GamemodeIDs)reader.ReadInt32();
            var settingsLength = reader.ReadInt32();
            GamemodeSettings = reader.ReadBytes(settingsLength);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write((int)Gamemode);
            writer.Write(GamemodeSettings.Length);
            writer.Write(GamemodeSettings);
        }
    }
}
