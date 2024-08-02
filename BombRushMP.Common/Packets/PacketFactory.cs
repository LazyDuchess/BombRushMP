using Riptide;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public static class PacketFactory
    {

        private static Dictionary<Packets, Type> _packetTypeById = new();

        public static void Initialize()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            foreach(var type in types)
            {
                if (typeof(Packet).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    var instance = Activator.CreateInstance(type) as Packet;
                    _packetTypeById[instance.PacketId] = type;
                }
            }
        }

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
            if (_packetTypeById.TryGetValue(packetId, out var type))
            {
                packet = Activator.CreateInstance(type) as Packet;
            }
            if (packet != null)
                packet.Read(reader);
            return packet;
        }
    }
}
