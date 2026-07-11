using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin
{
    public class PreferencesPacket
    {
        public const string Id = "ACN-PREFERENCESPACKET";
        public bool PvP = false;
        public ulong PacketId = 0;

        public static void InitializeStatic()
        {
            ClientController.RegisterCustomPacketHandler(Id, (player, data) =>
            {
                using var ms = new MemoryStream(data);
                using var reader = new BinaryReader(ms);
                var prefs = new PreferencesPacket();
                prefs.Read(reader);
                var clientController = ClientController.Instance;
                if (clientController.Players.TryGetValue(player, out var mpPlayer))
                {
                    if (mpPlayer.LatestPreferences != null)
                    {
                        if (prefs.PacketId > mpPlayer.LatestPreferences.PacketId)
                        {
                            mpPlayer.LatestPreferences = prefs;
                        }
                    }
                    else
                    {
                        mpPlayer.LatestPreferences = prefs;
                    }
                }
            });
        }

        public PreferencesPacket()
        {

        }

        public PreferencesPacket(bool pvp, ulong packetid)
        {
            PvP = pvp;
            PacketId = packetid;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)0);
            writer.Write(PvP);
            writer.Write(PacketId);
        }

        public void Read(BinaryReader reader)
        {
            var version = reader.ReadByte();
            PvP = reader.ReadBoolean();
            PacketId = reader.ReadUInt64();
        }
    }
}
