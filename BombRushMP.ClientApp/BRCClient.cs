using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common;
using BombRushMP.Common.Packets;
using Riptide;
using Riptide.Utils;

namespace BombRushMP.ClientApp
{
    public class BRCClient : IDisposable
    {
        private const string BotName = "BRCMP BOT";
        private Client _client;
        private ServerClientStates _clientStates;
        private ServerLobbies _lobbies;
        private ushort _localId = 0;
        private Tasks _task = Tasks.CreateLobbyAndInvitePlayer;

        private int _taskStep = 0;

        public enum Tasks
        {
            CreateLobbyAndInvitePlayer,
            JoinPlayersLobby,
            SendChatMessage
        }

        public BRCClient(string address, Tasks task)
        {
            Message.MaxPayloadSize = Constants.MaxPayloadSize;
            PacketFactory.Initialize();
            _task = task;
            _client = new Client();
            _client.Connect(address);
            _client.Connected += OnConnected;
            _client.MessageReceived += OnMessage;
        }

        private void OnConnected(object sender, EventArgs e)
        {
            SendDummyClientState();
        }

        public void Update()
        {
            _client.Update();
            switch (_task)
            {
                case Tasks.CreateLobbyAndInvitePlayer:
                    CreateLobbyAndInvitePlayer_Update();
                    break;

                case Tasks.JoinPlayersLobby:
                    JoinPlayersLobby_Update();
                    break;

                case Tasks.SendChatMessage:
                    SendChatMessage_Update();
                    break;
            }
        }

        private void SendChatMessage_Update()
        {
            if (_localId == 0)
                return;
            switch (_taskStep)
            {
                case 0:
                    Log("Sending chat message.");
                    SendPacket(new ClientChat("Hello, world!"));
                    _taskStep++;
                    break;
            }
        }

        private void JoinPlayersLobby_Update()
        {
            if (_localId == 0)
                return;
            if (_lobbies == null)
                return;
            switch (_taskStep)
            {
                case 0:
                    var player = GetPlayer();
                    if (player == 0) return;
                    var lobby = GetPlayersLobby(player);
                    if (lobby == 0) return;
                    SendPacket(new ClientLobbyJoin(lobby));
                    Log("Joining lobby");
                    _taskStep++;
                    break;
                case 1:
                    Log("Finished.");
                    _taskStep++;
                    break;
            }
        }

        private void CreateLobbyAndInvitePlayer_Update()
        {
            if (_localId == 0)
                return;
            switch (_taskStep)
            {
                case 0:
                    SendPacket(new ClientLobbyCreate());
                    Log("Creating lobby.");
                    _taskStep++;
                    break;
                case 1:
                    var myLobby = GetMyLobby();
                    if (myLobby == 0)
                        return;
                    var player = GetPlayer();
                    if (player == 0)
                        return;
                    SendPacket(new ClientLobbyInvite(player));
                    Log("Inviting player to lobby.");
                    _taskStep++;
                    break;
                case 2:
                    Log("Finished.");
                    _taskStep++;
                    break;
            }
        }

        private void Log(string message)
        {
            Console.WriteLine(message);
        }

        private uint GetPlayersLobby(ushort playerId)
        {
            foreach (var lobby in _lobbies.Lobbies)
            {
                if (lobby.HostId != _localId)
                    return lobby.Id;
            }
            return 0;
        }

        private ushort GetPlayer()
        {
            foreach(var player in _clientStates.ClientStates)
            {
                if (player.Value.Name != BotName)
                    return player.Key;
            }
            return 0;
        }

        private uint GetMyLobby()
        {
            foreach(var lobby in _lobbies.Lobbies)
            {
                if (lobby.HostId == _localId)
                    return lobby.Id;
            }
            return 0;
        }

        public void Stop()
        {
            _client.Disconnect();
        }

        public void Dispose()
        {
            Stop();
        }

        private void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            var packetId = (Packets)e.MessageId;
            var packet = PacketFactory.PacketFromMessage(packetId, e.Message);
            try
            {
                
                switch (packetId)
                {
                    case Packets.ServerClientStates:
                        _clientStates = (ServerClientStates)packet;
                        break;

                    case Packets.ServerConnectionResponse:
                        _localId = ((ServerConnectionResponse)packet).LocalClientId;
                        break;

                    case Packets.ServerLobbies:
                        _lobbies = (ServerLobbies)packet;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling message {packetId}: {ex}");
            }
        }

        public void JoinPlayerLobby()
        {
            foreach(var lobby in _lobbies.Lobbies)
            {
                if (lobby.HostId != _localId)
                {
                    SendPacket(new ClientLobbyJoin(lobby.Id));
                    return;
                }
            }
        }

        private void SendDummyClientState()
        {
            var cs = new ClientState();
            cs.Name = BotName;
            cs.Stage = 5;
            SendPacket(cs);

            var vs = new ClientVisualState();
            SendPacket(vs);
        }

        public void SendPacket(Packet packet)
        {
           var  message = PacketFactory.MessageFromPacket(packet, MessageSendMode.Reliable);
            _client.Send(message);
        }
    }
}
