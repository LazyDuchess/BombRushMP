using Riptide;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public static class PacketFactory
    {
        public static Message MessageFromPacket(Packet packet, MessageSendMode sendMode)
        {
            var message = Message.Create(sendMode, packet.PacketId);
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            packet.Write(writer);
            message.Add(ms.ToArray());
            return message;
        }

        public static Packet PacketFromMessage(Packets packetId, Message message)
        {
            var bytes = message.GetBytes();
            using var ms = new MemoryStream(bytes);
            using var reader = new BinaryReader(ms);
            Packet packet = null;
            switch (packetId)
            {
                case Packets.ClientState:
                    packet = new ClientState();
                    break;
                case Packets.ServerConnectionResponse:
                    packet = new ServerConnectionResponse();
                    break;
                case Packets.ServerClientStates:
                    packet = new ServerClientStates();
                    break;
                case Packets.ClientVisualState:
                    packet = new ClientVisualState();
                    break;
                case Packets.ServerClientVisualStates:
                    packet = new ServerClientVisualStates();
                    break;
                case Packets.PlayerAnimation:
                    packet = new PlayerAnimation();
                    break;
                case Packets.PlayerVoice:
                    packet = new PlayerVoice();
                    break;
                case Packets.PlayerSpray:
                    packet = new PlayerSpray();
                    break;
                case Packets.PlayerTeleport:
                    packet = new PlayerTeleport();
                    break;
            }
            if (packet != null)
                packet.Read(reader);
            return packet;
        }
    }
}
