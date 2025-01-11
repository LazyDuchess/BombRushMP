using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common.Networking;

namespace BombRushMP.Common.Packets
{
    public static class PacketFactory
    {
        private static INetworkingInterface NetworkingInterface => NetworkingEnvironment.NetworkingInterface;
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

        public static IMessage MessageFromPacket(Packet packet, IMessage.SendModes sendMode, NetChannels channel)
        {
            var message = NetworkingInterface.CreateMessage(sendMode, channel, packet.PacketId);
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            packet.Write(writer);
            message.Add(ms.ToArray());
            return message;
        }

        public static Packet PacketFromMessage(Packets packetId, IMessage message)
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
