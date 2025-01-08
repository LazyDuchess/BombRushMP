using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BombRushMP.Common.Packets;
using BombRushMP.Common;
using BombRushMP.Common.Networking;
using System.Diagnostics;
using System.Security.Policy;

namespace BombRushMP.ServerApp
{
    public class BRCServer : IDisposable
    {
        public static BRCServer Instance { get; private set; }
        public ServerLobbyManager ServerLobbyManager;
        public Action<INetConnection> ClientHandshook;
        public Action<INetConnection> ClientDisconnected;
        public Action<INetConnection, Packets, Packet> PacketReceived;
        public Action<float> OnTick;
        public Dictionary<ushort, Player> Players = new();
        private INetServer _server;
        private Stopwatch _tickStopWatch;
        private HashSet<int> _activeStages;
        private float _playerCountTickTimer = 0f;
        private INetworkingInterface NetworkingInterface => NetworkingEnvironment.NetworkingInterface;

        public BRCServer(ushort port, ushort maxPlayers)
        {
            Instance = this;
            NetworkingInterface.MaxPayloadSize = Constants.MaxPayloadSize;
            ServerLobbyManager = new();
            _tickStopWatch = new Stopwatch();
            _tickStopWatch.Start();
            _server = NetworkingInterface.CreateServer();
            _server.TimeoutTime = 10000;
            _server.ClientConnected += OnClientConnected;
            _server.ClientDisconnected += OnClientDisconnected;
            _server.MessageReceived += OnMessageReceived;
            _server.Start(port, maxPlayers);
            ServerLogger.Log($"Starting server on port {port} with max players {maxPlayers}");
        }

        public void DisconnectClient(ushort id)
        {
            _server.DisconnectClient(id);
        }

        public void Update()
        {
            var deltaTime = _tickStopWatch.Elapsed.TotalSeconds;
            if (deltaTime >= Constants.NetworkingTickRate)
            {
                Tick((float)_tickStopWatch.Elapsed.TotalSeconds);
                _tickStopWatch.Restart();
            }
        }

        private void Tick(float deltaTime)
        {
            _server.Update();
            foreach(var player in Players)
            {
                player.Value.Tick(deltaTime);
            }
            _activeStages = GetActiveStages();
            foreach(var stage in _activeStages)
            {
                TickStage(stage);
            }
            TickPlayerCount(deltaTime);
            OnTick?.Invoke(deltaTime);
        }

        private void TickPlayerCount(float deltaTime)
        {
            _playerCountTickTimer += deltaTime;
            if (_playerCountTickTimer >= ServerConstants.PlayerCountTickRate)
            {
                _playerCountTickTimer = 0f;
                var playerCountDictionary = new Dictionary<int, int>();
                foreach(var player in Players)
                {
                    if (player.Value.ClientState == null) continue;

                    if (playerCountDictionary.TryGetValue(player.Value.ClientState.Stage, out var playerCount))
                        playerCountDictionary[player.Value.ClientState.Stage] = playerCount + 1;
                    else
                        playerCountDictionary[player.Value.ClientState.Stage] = 1;
                }
                SendPacket(new ServerPlayerCount(playerCountDictionary), IMessage.SendModes.Unreliable);
            }
        }

        private void TickStage(int stage)
        {
            var clientVisualStates = CreateClientVisualStatesPacket(stage);
            SendPacketToStage(clientVisualStates, IMessage.SendModes.Unreliable, stage);
        }

        private HashSet<int> GetActiveStages()
        {
            var stages = new HashSet<int>();
            foreach (var player in Players)
            {
                if (player.Value.ClientState == null) continue;
                stages.Add(player.Value.ClientState.Stage);
            }
            return stages;
        }

        public void Dispose()
        {
            _server.Stop();
        }

        public void SendPacket(Packet packet, IMessage.SendModes sendMode, ushort[] except = null)
        {
            var message = PacketFactory.MessageFromPacket(packet, sendMode);
            foreach (var player in Players)
            {
                if (player.Value.ClientState == null) continue;
                if (except != null && except.Contains(player.Key)) continue;
                player.Value.Client.Send(message);
            }
        }

        public void SendPacketToStage(Packet packet, IMessage.SendModes sendMode, int stage, ushort[] except = null)
        {
            var message = PacketFactory.MessageFromPacket(packet, sendMode);
            foreach (var player in Players)
            {
                if (player.Value.ClientState == null) continue;
                if (player.Value.ClientState.Stage != stage) continue;
                if (except != null && except.Contains(player.Key)) continue;
                player.Value.Client.Send(message);
            }
        }

        public void SendPacketToClient(Packet packet, IMessage.SendModes sendMode, INetConnection client)
        {
            var message = PacketFactory.MessageFromPacket(packet, sendMode);
            client.Send(message);
        }

        private void OnPacketReceived(INetConnection client, Packets packetId, Packet packet)
        {
            switch (packetId)
            {
                case Packets.ClientState:
                    {
                        var clientState = (ClientState)packet;
                        if (clientState.ProtocolVersion != Constants.ProtocolVersion)
                        {
                            ServerLogger.Log($"Rejecting player from {client} (ID: {client.Id}) because of protocol version mismatch (Server: {Constants.ProtocolVersion}, Client: {clientState.ProtocolVersion}).");
                            _server.DisconnectClient(client);
                            return;
                        }
                        var oldClientState = Players[client.Id].ClientState;
                        Players[client.Id].ClientState = clientState;
                        if (oldClientState != null)
                        {
                            var clientStateUpdatePacket = new ServerClientStates();
                            clientStateUpdatePacket.ClientStates[client.Id] = clientState;
                            SendPacketToStage(clientStateUpdatePacket, IMessage.SendModes.Reliable, oldClientState.Stage);
                            return;
                        }
                        ServerLogger.Log($"Player from {client} (ID: {client.Id}) connected as {clientState.Name} in stage {clientState.Stage}. Protocol Version: {clientState.ProtocolVersion}");
                        SendPacketToClient(new ServerConnectionResponse() { LocalClientId = client.Id }, IMessage.SendModes.Reliable, client);
                        var clientStates = CreateClientStatesPacket(clientState.Stage);
                        SendPacketToStage(clientStates, IMessage.SendModes.Reliable, clientState.Stage);

                        SendPacketToStage(new ServerChat(
                            string.Format(ServerConstants.JoinMessage, TMPFilter.CloseAllTags(clientState.Name)), ChatMessageTypes.PlayerJoinedOrLeft),
                            IMessage.SendModes.Reliable, clientState.Stage);

                        ClientHandshook?.Invoke(client);
                    }
                    break;

                case Packets.ClientVisualState:
                    {
                        var clientState = Players[client.Id].ClientState;
                        var clientVisualState = (ClientVisualState)packet;
                        var oldVisualState = Players[client.Id].ClientVisualState;
                        Players[client.Id].ClientVisualState = clientVisualState;

                        if (oldVisualState != null && oldVisualState.AFK != clientVisualState.AFK)
                        {
                            if (clientVisualState.AFK)
                            {
                                SendPacketToStage(new ServerChat(
                                    string.Format(ServerConstants.AFKMessage, TMPFilter.CloseAllTags(clientState.Name)), ChatMessageTypes.PlayerAFK),
                                    IMessage.SendModes.Reliable, clientState.Stage);
                            }
                            else
                            {
                                SendPacketToStage(new ServerChat(
                                    string.Format(ServerConstants.LeaveAFKMessage, TMPFilter.CloseAllTags(clientState.Name)), ChatMessageTypes.PlayerAFK),
                                    IMessage.SendModes.Reliable, clientState.Stage);
                            }
                        }
                    }
                    break;

                case Packets.ClientChat:
                    {
                        var chatPacket = (ClientChat)packet;
                        if (!TMPFilter.IsValidChatMessage(chatPacket.Message)) return;
                        var player = Players[client.Id];
                        if (player.ClientState == null) return;
                        var serverChatPacket = new ServerChat(player.ClientState.Name, chatPacket.Message, ChatMessageTypes.Chat);
                        SendPacketToStage(serverChatPacket, IMessage.SendModes.Reliable, player.ClientState.Stage);
                    }
                    break;

                default:
                    {
                        if (packet is PlayerPacket)
                        {
                            var playerPacket = packet as PlayerPacket;
                            playerPacket.ClientId = client.Id;
                            var player = Players[client.Id];
                            if (player.ClientState == null) return;
                            // Exclude sender - will break things if you're trying to display the networked local player clientside.
                            // SendPacketToStage(playerPacket, MessageSendMode.Reliable, _players[client.Id].ClientState.Stage, [client.Id]);
                            SendPacketToStage(playerPacket, IMessage.SendModes.Reliable, Players[client.Id].ClientState.Stage);
                        }
                    }
                    break;
            }
        }

        private void OnMessageReceived(object sender, IMessageReceivedEventArgs e)
        {
            try
            {
                var packetId = (Packets)e.MessageId;
                var packet = PacketFactory.PacketFromMessage(packetId, e.Message);
                if (packet == null) return;
                if (!Players.TryGetValue(e.FromConnection.Id, out var result)) return;
                PacketReceived?.Invoke(e.FromConnection, packetId, packet);
                OnPacketReceived(e.FromConnection, packetId, packet);
            }
            catch(Exception exc)
            {
                ServerLogger.Log($"Dropped client from {e.FromConnection} (ID: {e.FromConnection.Id}) because they sent a faulty packet. Exception:\n{exc}");
                _server.DisconnectClient(e.FromConnection);
            }
        }

        private void OnClientConnected(object sender, IServerConnectedEventArgs e)
        {
            ServerLogger.Log($"Client connected from {e.Client}. ID: {e.Client.Id}.");
            var player = new Player();
            player.Client = e.Client;
            player.Server = this;
            Players[e.Client.Id] = player;
            e.Client.CanQualityDisconnect = false;
        }

        private string GetAddressWithoutPort(string address)
        {
            return address.Split(":")[0];
        }

        private void OnClientDisconnected(object sender, IServerDisconnectedEventArgs e)
        {
            ServerLogger.Log($"Client disconnected from {e.Client}. ID: {e.Client.Id}. Reason: {e.Reason}");
            ClientState clientState = null;
            if (Players.TryGetValue(e.Client.Id, out var result))
            {
                ClientDisconnected?.Invoke(e.Client);
                Players.Remove(e.Client.Id);
                clientState = result.ClientState;
            }
            if (clientState != null)
            {
                var clientStates = CreateClientStatesPacket(clientState.Stage);
                SendPacketToStage(clientStates, IMessage.SendModes.Reliable, clientState.Stage);

                SendPacketToStage(new ServerChat(
                    string.Format(ServerConstants.LeaveMessage, TMPFilter.CloseAllTags(clientState.Name)), ChatMessageTypes.PlayerJoinedOrLeft),
                    IMessage.SendModes.Reliable, clientState.Stage);
            }
        }

        private ServerClientStates CreateClientStatesPacket(int stage)
        {
            var packet = new ServerClientStates();
            foreach(var player in Players)
            {
                if (player.Value.ClientState == null) continue;
                if (player.Value.ClientState.Stage != stage) continue;
                packet.ClientStates[player.Key] = player.Value.ClientState;
            }
            return packet;
        }

        private ServerClientVisualStates CreateClientVisualStatesPacket(int stage)
        {
            var packet = new ServerClientVisualStates();
            foreach(var player in Players)
            {
                if (player.Value.ClientState == null) continue;
                if (player.Value.ClientState.Stage != stage) continue;
                if (player.Value.ClientVisualState == null) continue;
                packet.ClientVisualStates[player.Key] = player.Value.ClientVisualState;
            }
            return packet;
        }
    }
}
