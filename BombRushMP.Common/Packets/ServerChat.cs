using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerChat : Packet
    {
        public override Packets PacketId => Packets.ServerChat;
        public string Author = "";
        public string Message = "";
        public ChatMessageTypes MessageType = ChatMessageTypes.Chat;

        public ServerChat()
        {

        }

        public ServerChat(string author, string message, ChatMessageTypes messageType = ChatMessageTypes.Chat)
        {
            Author = author;
            Message = message;
            MessageType = messageType;
        }

        public ServerChat(string message, ChatMessageTypes messageType = ChatMessageTypes.System)
        {
            Message = message;
            MessageType = messageType;
        }

        public override void Read(BinaryReader reader)
        {
            Author = reader.ReadString();
            Message = reader.ReadString();
            MessageType = (ChatMessageTypes)reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Author);
            writer.Write(Message);
            writer.Write((int)MessageType);
        }
    }
}
