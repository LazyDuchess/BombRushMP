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
using System.Security.Cryptography;

namespace BombRushMP.Server
{
    public class BRCServer : IDisposable
    {
        public static BRCServer Instance { get; private set; }
        public ServerLobbyManager ServerLobbyManager;
        public static Action<INetConnection> ClientHandshook;
        public static Action<INetConnection> ClientDisconnected;
        public static Action<INetConnection, Packets, Packet> PacketReceived;
        public static Action<float> OnTick;
        public Dictionary<ushort, Player> Players = new();
        public INetServer Server;
        private float _tickRate = Constants.DefaultNetworkingTickRate;
        private Stopwatch _tickStopWatch;
        private HashSet<int> _activeStages;
        private float _playerCountTickTimer = 0f;
        private INetworkingInterface NetworkingInterface => NetworkingEnvironment.NetworkingInterface;
        private IServerDatabase _database;
        public bool LogMessages = false;

        public BRCServer(int port, ushort maxPlayers, float tickRate, IServerDatabase database)
        {
            Instance = this;
            _database = database;
            _tickRate = tickRate;
            NetworkingInterface.MaxPayloadSize = Constants.MaxPayloadSize;
            ServerLobbyManager = new ServerLobbyManager();
            _tickStopWatch = new Stopwatch();
            _tickStopWatch.Start();
            Server = NetworkingInterface.CreateServer();
            Server.TimeoutTime = 10000;
            Server.ClientConnected += OnClientConnected;
            Server.ClientDisconnected += OnClientDisconnected;
            Server.MessageReceived += OnMessageReceived;
            Server.Start((ushort)port, maxPlayers);
            ServerLogger.Log($"Starting server on port {port} with max players {maxPlayers}, using Network Interface {NetworkingInterface}");
        }

        public void DisconnectClient(ushort id)
        {
            Server.DisconnectClient(id);
        }

        public void Update()
        {
            var deltaTime = _tickStopWatch.Elapsed.TotalSeconds;
            if (deltaTime >= _tickRate)
            {
                Tick((float)deltaTime);
                _tickStopWatch.Restart();
            }
        }

        private void Tick(float deltaTime)
        {
            Server.Update();
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
            Server.Stop();
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

        private void SendEliteNagChat(int stage)
        {
            SendPacketToStage(new ServerChat(SpecialPlayerUtils.SpecialPlayerNag), IMessage.SendModes.Reliable, stage);
        }

        private void ProcessCommand(string message, Player player)
        {
            var args = message.Split(' ');
            var cmd = args[0].Substring(1, args[0].Length - 1);
            switch (cmd)
            {
                case "nag":
                    if (player.ClientState.User.HasTag(SpecialPlayerUtils.SpecialPlayerTag))
                    {
                        SendEliteNagChat(player.ClientState.Stage);
                    }
                    break;

                case "banaddress":
                    if (player.ClientState.User.UserKind == UserKinds.Mod || player.ClientState.User.UserKind == UserKinds.Admin)
                    {
                        if (args.Length > 1)
                            BanPlayerByAddress(args[1]);
                    }
                    break;

                case "banid":
                    if (player.ClientState.User.UserKind == UserKinds.Mod || player.ClientState.User.UserKind == UserKinds.Admin)
                    {
                        if (args.Length > 1)
                        {
                            if (ushort.TryParse(args[1], out var result))
                                BanPlayerById(result);
                        }
                    }
                    break;

                case "getids":
                    if (player.ClientState.User.UserKind == UserKinds.Mod || player.ClientState.User.UserKind == UserKinds.Admin)
                    {
                        var idString = "";
                        foreach(var playa in Players)
                        {
                            if (playa.Value.ClientState.Stage == player.ClientState.Stage)
                            {
                                idString += $"{playa.Key} - {TMPFilter.CloseAllTags(playa.Value.ClientState.Name)}\n";
                            }
                        }
                        SendPacketToClient(new ServerChat(idString), IMessage.SendModes.Reliable, player.Client);
                    }
                    break;

                case "getaddresses":
                    if (player.ClientState.User.UserKind == UserKinds.Mod || player.ClientState.User.UserKind == UserKinds.Admin)
                    {
                        var idString = "";
                        foreach (var playa in Players)
                        {
                            if (playa.Value.ClientState.Stage == player.ClientState.Stage)
                            {
                                idString += $"{playa.Value.Client.Address} - {TMPFilter.CloseAllTags(playa.Value.ClientState.Name)} ({playa.Key})\n";
                            }
                        }
                        SendPacketToClient(new ServerChat(idString), IMessage.SendModes.Reliable, player.Client);
                    }
                    break;

                case "help":
                    if (player.ClientState.User.UserKind == UserKinds.Mod || player.ClientState.User.UserKind == UserKinds.Admin)
                    {
                        var helpStr = $"Available commands:\nbanaddress (ip)\nbanid (id)\ngetids\ngetaddresses\nhelp";
                        SendPacketToClient(new ServerChat(helpStr), IMessage.SendModes.Reliable, player.Client);
                    }
                    break;
            }
        }

        private void OnPacketReceived(INetConnection client, Packets packetId, Packet packet)
        {
            if (_database.BannedUsers.IsBanned(client.Address))
            {
                Server.DisconnectClient(client.Id);
                return;
            }
            switch (packetId)
            {
                case Packets.ClientAuth:
                case Packets.ClientState:
                    {
                        var clientAuth = packet as ClientAuth;
                        var clientState = packet as ClientState;
                        if (clientAuth != null)
                        {
                            clientState = clientAuth.State;
                        }
                        if (clientState.ProtocolVersion != Constants.ProtocolVersion)
                        {
                            ServerLogger.Log($"Rejecting player from {client.Address} (ID: {client.Id}) because of protocol version mismatch (Server: {Constants.ProtocolVersion}, Client: {clientState.ProtocolVersion}).");
                            Server.DisconnectClient(client);
                            return;
                        }
                        var oldClientState = Players[client.Id].ClientState;
                        if (clientAuth != null)
                        {
                            var user = _database.AuthKeys.GetUser(clientAuth.AuthKey);
                            clientState.User = user;
                        }
                        else if (oldClientState != null)
                        {
                            clientState.User = oldClientState.User;
                        }
                        if (clientState.User.HasTag(SpecialPlayerUtils.SpecialPlayerTag))
                        {
                            clientState.Name = SpecialPlayerUtils.SpecialPlayerName;
                        }
                        else
                        {
                            if (clientState.SpecialSkin == SpecialSkins.SpecialPlayer)
                            {
                                clientState.SpecialSkin = SpecialSkins.None;
                            }
                        }
                        Players[client.Id].ClientState = clientState;
                        if (oldClientState != null)
                        {
                            var clientStateUpdatePacket = new ServerClientStates();
                            clientStateUpdatePacket.ClientStates[client.Id] = clientState;
                            SendPacketToStage(clientStateUpdatePacket, IMessage.SendModes.Reliable, oldClientState.Stage);
                            return;
                        }
                        ServerLogger.Log($"Player from {client.Address} (ID: {client.Id}) connected as {clientState.Name} in stage {clientState.Stage}. Protocol Version: {clientState.ProtocolVersion}");
                        SendPacketToClient(new ServerConnectionResponse() { LocalClientId = client.Id, TickRate = _tickRate, User = clientState.User }, IMessage.SendModes.Reliable, client);
                        var clientStates = CreateClientStatesPacket(clientState.Stage);
                        SendPacketToStage(clientStates, IMessage.SendModes.Reliable, clientState.Stage);

                        SendPacketToStage(new ServerChat(
                            TMPFilter.CloseAllTags(clientState.Name), ServerConstants.JoinMessage, clientState.User.Badge, ChatMessageTypes.PlayerJoinedOrLeft),
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
                                    TMPFilter.CloseAllTags(clientState.Name), ServerConstants.AFKMessage, clientState.User.Badge, ChatMessageTypes.PlayerAFK),
                                    IMessage.SendModes.Reliable, clientState.Stage);
                            }
                            else
                            {
                                SendPacketToStage(new ServerChat(
                                    TMPFilter.CloseAllTags(clientState.Name), ServerConstants.LeaveAFKMessage, clientState.User.Badge, ChatMessageTypes.PlayerAFK),
                                    IMessage.SendModes.Reliable, clientState.Stage);
                            }
                        }
                    }
                    break;

                case Packets.ClientChat:
                    {
                        var chatPacket = (ClientChat)packet;
                        var player = Players[client.Id];
                        if (LogMessages)
                        {
                            var logText = $"[{DateTime.Now.ToShortTimeString()}] {player.ClientState.Name} ({player.Client.Address}): {chatPacket.Message}";
                            _database.LogChatMessage(logText, player.ClientState.Stage);
                        }
                        if (!TMPFilter.IsValidChatMessage(chatPacket.Message)) return;
                        if (chatPacket.Message[0] == '/')
                        {
                            ProcessCommand(chatPacket.Message, player);
                        }
                        else
                        {
                            if (player.ClientState == null) return;
                            var serverChatPacket = new ServerChat(player.ClientState.Name, chatPacket.Message, player.ClientState.User.Badge, ChatMessageTypes.Chat);
                            SendPacketToStage(serverChatPacket, IMessage.SendModes.Reliable, player.ClientState.Stage);
                        }
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

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
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
                Server.DisconnectClient(e.FromConnection);
            }
        }

        public void BanPlayerById(ushort id, string reason = "None")
        {
            if (Players.TryGetValue(id, out var result))
            {
                if (_database.BannedUsers.IsBanned(result.Client.Address)) return;
                BanPlayerByAddress(result.Client.Address);
            }
        }

        public void BanPlayerByAddress(string address, string reason = "None")
        {
            if (_database.BannedUsers.IsBanned(address)) return;
            var playerName = "None";
            ushort playerId = 0;
            foreach(var player in Players)
            {
                if (player.Value.Client.Address == address)
                {
                    playerName = player.Value.ClientState.Name;
                    playerId = player.Key;
                    break;
                }
            }
            _database.BannedUsers.Ban(address, playerName, reason);
            ServerLogger.Log($"Banned IP {address}, player name: {playerName}, reason: {reason}");
            if (playerId != 0)
                Server.DisconnectClient(playerId);
            _database.Save();
        }

        private void OnClientConnected(object sender, ServerConnectedEventArgs e)
        {
            if (!_database.BannedUsers.IsBanned(e.Client.Address))
                ServerLogger.Log($"Client connected from {e.Client.Address}. ID: {e.Client.Id}.");
            var player = new Player();
            player.Client = e.Client;
            player.Server = this;
            Players[e.Client.Id] = player;
            e.Client.CanQualityDisconnect = false;
            if (_database.BannedUsers.IsBanned(e.Client.Address))
            {
                Server.DisconnectClient(e.Client.Id);
                return;
            }
        }

        private string GetAddressWithoutPort(string address)
        {
            return address.Split(':')[0];
        }

        private void OnClientDisconnected(object sender, ServerDisconnectedEventArgs e)
        {
            if (!_database.BannedUsers.IsBanned(e.Client.Address))
            {
                ServerLogger.Log($"Client disconnected from {e.Client.Address}. ID: {e.Client.Id}. Reason: {e.Reason}");
            }
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
                    TMPFilter.CloseAllTags(clientState.Name), ServerConstants.LeaveMessage, clientState.User.Badge, ChatMessageTypes.PlayerJoinedOrLeft),
                    IMessage.SendModes.Reliable, clientState.Stage);
            }
        }

        private ServerClientStates CreateClientStatesPacket(int stage)
        {
            var packet = new ServerClientStates();
            packet.Full = true;
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
