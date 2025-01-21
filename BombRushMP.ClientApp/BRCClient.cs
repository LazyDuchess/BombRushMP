using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common;
using BombRushMP.Common.Networking;
using BombRushMP.Common.Packets;

namespace BombRushMP.ClientApp
{
    public class BRCClient : IDisposable
    {
        private const string BotName = "T-800";
        private const string CrewName = "Bots";
        private INetClient _client;
        private ServerClientStates _clientStates;
        private ServerLobbies _lobbies;
        private ushort _localId = 0;
        private Tasks _task = Tasks.CreateLobbyAndInvitePlayer;
        private INetworkingInterface NetworkingInterface => NetworkingEnvironment.NetworkingInterface;

        private int _taskStep = 0;

        private Random _random = new Random();

        public enum Tasks
        {
            CreateLobbyAndInvitePlayer,
            JoinPlayersLobby,
            SendChatMessage,
            CreateLobbyInvitePlayerAndStartGame
        }

        public BRCClient(string address, int port, Tasks task)
        {
            NetworkingInterface.MaxPayloadSize = Constants.MaxPayloadSize;
            PacketFactory.Initialize();
            _task = task;
            _client = NetworkingInterface.CreateClient();
            _client.Connect(address, port);
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
                    CreateLobbyAndInvitePlayer_Update(false);
                    break;

                case Tasks.JoinPlayersLobby:
                    JoinPlayersLobby_Update();
                    break;

                case Tasks.SendChatMessage:
                    SendChatMessage_Update();
                    break;

                case Tasks.CreateLobbyInvitePlayerAndStartGame:
                    CreateLobbyAndInvitePlayer_Update(true);
                    break;
            }
            var scorePacket = new ClientGamemodeScore((float)(_random.NextDouble() * 10000000f));
            SendPacket(scorePacket);
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

        private void CreateLobbyAndInvitePlayer_Update(bool startGame)
        {
            if (_localId == 0)
                return;
            if (_lobbies == null)
                return;
            var myLobby = GetMyLobby();
            var player = GetPlayer();
            switch (_taskStep)
            {
                case 0:
                    SendPacket(new ClientLobbyCreate(GamemodeIDs.ScoreBattle, new byte[] {}));
                    Log("Creating lobby.");
                    _taskStep++;
                    break;
                case 1:
                    if (myLobby == 0)
                        return;
                    if (player == 0)
                        return;
                    SendPacket(new ClientLobbySetGamemode(GamemodeIDs.ScoreBattle, new byte[] { }));
                    SendPacket(new ClientLobbySetChallenge(true));
                    SendPacket(new ClientLobbyInvite(player));
                    Log("Inviting player to lobby and waiting for them to join.");
                    _taskStep++;
                    break;
                case 2:
                    if (myLobby == 0)
                        return;
                    if (player == 0)
                        return;
                    if (GetPlayersLobby(player) == myLobby)
                    Log("Player joined.");
                    _taskStep++;
                    break;
                case 3:
                    if (!startGame)
                    {
                        Log("Finished.");
                        _taskStep++;
                    }
                    else
                    {
                        Log("Waiting for input to start game.");
                        _taskStep++;
                        Console.ReadLine();
                    }
                    break;
                case 4:
                    if (myLobby == 0)
                        return;
                    if (startGame)
                    {
                        Log("Starting game!");
                        SendPacket(new ClientLobbyStart());
                        _taskStep++;
                        Log("Finished.");
                        return;
                    }
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
                {
                    return player.Key;
                }
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

        float degreesToRadians(float degrees) => degrees * ((float)Math.PI / 180f);

        private void SendDummyClientState()
        {
            var cs = new ClientState();
            cs.Name = BotName;
            cs.CrewName = CrewName;
            cs.Stage = 50851934;
            SendPacket(cs);

            var vs = new ClientVisualState();
            vs.CurrentAnimation = 1252092624;
            vs.Position = new Vector3(2.7161f, -0.91f, -2.046f);
            vs.Rotation = Quaternion.CreateFromYawPitchRoll(degreesToRadians(279.1345f),0f, 0f);
            SendPacket(vs);
        }

        public void SendPacket(Packet packet)
        {
           var  message = PacketFactory.MessageFromPacket(packet, IMessage.SendModes.Reliable, NetChannels.Default);
           _client.Send(message);
        }
    }
}
