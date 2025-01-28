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
using Newtonsoft.Json;

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
        public Action RestartAction;
        public Dictionary<ushort, Player> Players = new();
        public INetServer Server;
        private float _tickRate = Constants.DefaultNetworkingTickRate;
        private Stopwatch _tickStopWatch;
        private HashSet<int> _activeStages;
        private float _playerCountTickTimer = 0f;
        private INetworkingInterface NetworkingInterface => NetworkingEnvironment.NetworkingInterface;
        private IServerDatabase _database;
        public bool LogMessagesToFile = false;
        public bool LogMessages = true;
        public bool AllowNameChanges = true;
        public float ChatCooldown = 0.5f;
        public IMessage.SendModes ClientAnimationSendMode = IMessage.SendModes.ReliableUnordered;

        public ServerState ServerState = new();

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
                    if (player.Value.Invisible) continue;

                    if (playerCountDictionary.TryGetValue(player.Value.ClientState.Stage, out var playerCount))
                        playerCountDictionary[player.Value.ClientState.Stage] = playerCount + 1;
                    else
                        playerCountDictionary[player.Value.ClientState.Stage] = 1;
                }
                SendPacket(new ServerPlayerCount(playerCountDictionary), IMessage.SendModes.Unreliable, NetChannels.Default);
            }
        }

        private void TickStage(int stage)
        {
            var clientVisualStates = CreateClientVisualStatesPacket(stage);
            foreach (var visualState in clientVisualStates)
            {
                SendPacketToStage(visualState, IMessage.SendModes.Unreliable, stage, NetChannels.VisualUpdates);
            }
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

        public void SendPacket(Packet packet, IMessage.SendModes sendMode, NetChannels channel, ushort[] except = null)
        {
            var message = PacketFactory.MessageFromPacket(packet, sendMode, channel);
            foreach (var player in Players)
            {
                if (player.Value.ClientState == null) continue;
                if (except != null && except.Contains(player.Key)) continue;
                player.Value.Client.Send(message);
            }
        }

        public void SendPacketToStage(Packet packet, IMessage.SendModes sendMode, int stage, NetChannels channel, ushort[] except = null)
        {
            var message = PacketFactory.MessageFromPacket(packet, sendMode, channel);
            foreach (var player in Players)
            {
                if (player.Value.ClientState == null) continue;
                if (player.Value.ClientState.Stage != stage) continue;
                if (except != null && except.Contains(player.Key)) continue;
                player.Value.Client.Send(message);
            }
        }

        public void SendPacketToClient(Packet packet, IMessage.SendModes sendMode, INetConnection client, NetChannels channel)
        {
            var message = PacketFactory.MessageFromPacket(packet, sendMode, channel);
            client.Send(message);
        }

        private void SendEliteNagChat(int stage)
        {
            SendPacketToStage(new ServerChat(SpecialPlayerUtils.SpecialPlayerNag), IMessage.SendModes.ReliableUnordered, stage, NetChannels.Chat);
        }

        public void SendMessageToPlayer(string message, Player player)
        {
            SendPacketToClient(new ServerChat(message), IMessage.SendModes.ReliableUnordered, player.Client, NetChannels.Chat);
        }

        private void ReloadAllUsers()
        {
            foreach(var player in Players)
            {
                player.Value.ClientState.User = _database.AuthKeys.GetUser(player.Value.Auth.AuthKey);
            }
            var activeStages = GetActiveStages();
            foreach(var activeStage in activeStages)
            {
                var clientStates = CreateClientStatesPacket(activeStage);
                SendPacketToStage(clientStates, IMessage.SendModes.Reliable, activeStage, NetChannels.ClientAndLobbyUpdates);
            }
        }

        private void ProcessCommand(string message, Player player)
        {
            var args = message.Split(' ');
            var cmd = args[0].Substring(1, args[0].Length - 1);
            switch (cmd)
            {
                case "getservertags":
                    if (player.ClientState.User.IsModerator)
                    {
                        var tagtxt = "Current server tags:\n";
                        foreach(var tag in ServerState.Tags)
                        {
                            tagtxt += $"{tag}\n";
                        }
                        SendPacketToClient(new ServerChat(tagtxt), IMessage.SendModes.ReliableUnordered, player.Client, NetChannels.Chat);
                    }
                    break;
                case "removeservertag":
                    if (player.ClientState.User.IsModerator)
                    {
                        if (args.Length > 1)
                        {
                            ServerState.Tags.Remove(args[1]);
                            SendPacket(new ServerServerStateUpdate(ServerState), IMessage.SendModes.Reliable, NetChannels.ClientAndLobbyUpdates);
                        }
                    }
                    break;
                case "setservertag":
                    if (player.ClientState.User.IsModerator)
                    {
                        if (args.Length > 1)
                        {
                            ServerState.Tags.Add(args[1]);
                            SendPacket(new ServerServerStateUpdate(ServerState), IMessage.SendModes.Reliable, NetChannels.ClientAndLobbyUpdates);
                        }
                    }
                    break;
                case "makeseankingston":
                    if (player.ClientState.User.IsModerator)
                    {
                        if (args.Length > 1)
                        {
                            if (ushort.TryParse(args[1], out var result))
                            {
                                if (Players.TryGetValue(result, out var playa))
                                {
                                    SendPacketToClient(new ServerSetSpecialSkin(SpecialSkins.SeanKingston), IMessage.SendModes.ReliableUnordered, playa.Client, NetChannels.Default);
                                }
                            }
                        }
                    }
                    break;
                case "makechibi":
                    if (player.ClientState.User.IsModerator)
                    {
                        if (args.Length > 1)
                        {
                            if (ushort.TryParse(args[1], out var result))
                            {
                                if (Players.TryGetValue(result, out var playa))
                                {
                                    SendPacketToClient(new ServerSetChibi(true), IMessage.SendModes.ReliableUnordered, playa.Client, NetChannels.Default);
                                }
                            }
                        }
                    }
                    break;

                case "nag":
                    if (player.ClientState.User.HasTag(SpecialPlayerUtils.SpecialPlayerTag))
                    {
                        SendEliteNagChat(player.ClientState.Stage);
                    }
                    break;

                case "banaddress":
                    if (player.ClientState.User.IsModerator)
                    {
                        var parsedArgs = CommandUtility.ParseArgs(message, 2);
                        if (string.IsNullOrWhiteSpace(parsedArgs[0])) break;
                        if (string.IsNullOrWhiteSpace(parsedArgs[1])) parsedArgs[1] = "None";
                        if (BanPlayerByAddress(parsedArgs[0], parsedArgs[1]))
                            SendMessageToPlayer("Player has been banned.", player);
                    }
                    break;

                case "banid":
                    if (player.ClientState.User.IsModerator)
                    {
                        var parsedArgs = CommandUtility.ParseArgs(message, 2);
                        if (string.IsNullOrWhiteSpace(parsedArgs[0])) break;
                        if (string.IsNullOrWhiteSpace(parsedArgs[1])) parsedArgs[1] = "None";
                        if (ushort.TryParse(parsedArgs[0], out var result))
                        {
                            if (BanPlayerById(result, parsedArgs[1]))
                                SendMessageToPlayer("Player has been banned.", player);
                        }
                    }
                    break;

                case "unban":
                    if (player.ClientState.User.IsModerator)
                    {
                        if (args.Length > 1)
                        {
                            if (Unban(args[1]))
                                SendMessageToPlayer("Player has been unbanned.", player);
                        }
                    }
                    break;

                case "getids":
                    if (player.ClientState.User.IsModerator)
                    {
                        var idString = "";
                        foreach(var playa in Players)
                        {
                            if (playa.Value.ClientState.Stage == player.ClientState.Stage)
                            {
                                idString += $"{playa.Key} - {TMPFilter.CloseAllTags(playa.Value.ClientState.Name)}\n";
                            }
                        }
                        SendMessageToPlayer(idString, player);
                    }
                    break;

                case "getaddresses":
                    if (player.ClientState.User.IsModerator)
                    {
                        var idString = "";
                        foreach (var playa in Players)
                        {
                            if (playa.Value.ClientState.Stage == player.ClientState.Stage)
                            {
                                idString += $"{playa.Value.Client.Address} - {TMPFilter.CloseAllTags(playa.Value.ClientState.Name)} ({playa.Key})\n";
                            }
                        }
                        SendMessageToPlayer(idString, player);
                    }
                    break;

                case "help":
                    var cmdChar = Constants.CommandChar;
                    var helpStr = "\nAvailable commands:\n";
                    helpStr += $"{cmdChar}hide - Hide chat\n{cmdChar}show - Show chat\n";
                    if (player.ClientState.User.IsModerator)
                    {
                        helpStr += $"{cmdChar}banlist - Downloads the ban list from the server\n{cmdChar}banaddress (ip) (reason) - Bans player by IP\n{cmdChar}banid (id) (reason) - Bans player by ID\n{cmdChar}unban (ip) - Unbans player by IP\n{cmdChar}getids - Gets IDs of players in current stage\n{cmdChar}getaddresses - Gets IP addresses of players in current stage\n{cmdChar}help\n{cmdChar}stats - Shows global player and lobby stats\n{cmdChar}makechibi (id)\n{cmdChar}makeseankingston (id)\n{cmdChar}setservertag (tag)\n{cmdChar}removeservertag (tag)\n{cmdChar}getservertags\n";
                    }
                    if (player.ClientState.User.IsAdmin)
                    {
                        helpStr += $"{cmdChar}reload - Reloads server auth keys and banned users\n{cmdChar}restart - Restarts the server\n{cmdChar}say (announcement for stage)\n{cmdChar}sayall (global announcement)\n";
                    }
                    SendMessageToPlayer(helpStr, player);
                    break;

                case "say":
                    if (player.ClientState.User.IsAdmin)
                    {
                        SendPacketToStage(new ServerChat(message.Substring(5)), IMessage.SendModes.ReliableUnordered, player.ClientState.Stage, NetChannels.Chat);
                    }
                    break;

                case "sayall":
                    if (player.ClientState.User.IsAdmin)
                    {
                        SendPacket(new ServerChat(message.Substring(8)), IMessage.SendModes.ReliableUnordered, NetChannels.Chat);
                    }
                    break;

                case "stats":
                    if (player.ClientState.User.IsModerator)
                    {
                        var playerCount = Players.Count;
                        var lobbyCount = ServerLobbyManager.Lobbies.Count;
                        var lobbiesInGame = 0;
                        foreach(var lobby in ServerLobbyManager.Lobbies)
                        {
                            if (lobby.Value.LobbyState.InGame)
                                lobbiesInGame++;
                        }
                        SendMessageToPlayer($"Players: {playerCount}\nLobbies: {lobbyCount}\nLobbies in game: {lobbiesInGame}", player);
                    }
                    break;

                case "reload":
                    if (player.ClientState.User.IsAdmin)
                    {
                        SendMessageToPlayer("Reloading server database.", player);
                        _database.Load();
                        ReloadAllUsers();
                    }
                    break;

                case "restart":
                    if (player.ClientState.User.IsAdmin)
                    {
                        SendMessageToPlayer("Restarting server.", player);
                        RestartAction?.Invoke();
                    }
                    break;

                case "banlist":
                    if (player.ClientState.User.IsModerator)
                    {
                        SendPacketToClient(new ServerBanList(JsonConvert.SerializeObject(_database.BannedUsers, Formatting.Indented)), IMessage.SendModes.ReliableUnordered, player.Client, NetChannels.Default);
                    }
                    break;
            }
        }

        private void OnPacketReceived(INetConnection client, Packets packetId, Packet packet)
        {
            var player = Players[client.Id];
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
                            // don't re-auth mid connection.
                            if (player.Auth != null)
                            {
                                clientAuth = null;
                            }
                            else
                            {
                                player.Auth = clientAuth;
                            }
                        }

                        if (clientState.Name.Length > Constants.MaxNameLength)
                            clientState.Name = clientState.Name.Substring(0, Constants.MaxNameLength);

                        if (clientState.CrewName.Length > Constants.MaxCrewNameLength)
                            clientState.CrewName = clientState.CrewName.Substring(0, Constants.MaxCrewNameLength);

                        var oldClientState = player.ClientState;
                        if (oldClientState != null)
                        {
                            if (!AllowNameChanges && oldClientState.User.UserKind == UserKinds.Player)
                            {
                                clientState.Name = oldClientState.Name;
                            }
                            clientState.Stage = oldClientState.Stage;
                        }
                        if (clientAuth != null)
                        {
                            var user = _database.AuthKeys.GetUser(clientAuth.AuthKey);
                            clientState.User = user;
                            if (user.CanLurk)
                            {
                                player.Invisible = clientAuth.Invisible;
                            }
                        }
                        else if (oldClientState != null)
                        {
                            clientState.User = oldClientState.User;
                        }
                        if (clientState.User.HasTag(SpecialPlayerUtils.SpecialPlayerTag))
                        {
                            clientState.Name = SpecialPlayerUtils.SpecialPlayerName;
                            clientState.CrewName = SpecialPlayerUtils.SpecialPlayerCrewName;
                        }
                        else
                        {
                            if (clientState.SpecialSkin == SpecialSkins.SpecialPlayer)
                            {
                                clientState.SpecialSkin = SpecialSkins.None;
                            }
                        }
                        player.ClientState = clientState;

                        var updateClientState = CreatePlayerClientState(player);
                        if (updateClientState != null)
                            SendPacketToStage(updateClientState, IMessage.SendModes.Reliable, clientState.Stage, NetChannels.ClientAndLobbyUpdates);

                        if (oldClientState != null)
                            return;

                        ServerLogger.Log($"Player from {client.Address} (ID: {client.Id}) connected as {clientState.Name} in stage {clientState.Stage}.");
                        SendPacketToClient(new ServerConnectionResponse() { LocalClientId = client.Id, TickRate = _tickRate, ClientAnimationSendMode = ClientAnimationSendMode, User = clientState.User, ServerState = ServerState }, IMessage.SendModes.Reliable, client, NetChannels.Default);

                        var currentClientStates = CreateClientStatesPacket(clientState.Stage);
                        SendPacketToClient(currentClientStates, IMessage.SendModes.Reliable, client, NetChannels.ClientAndLobbyUpdates);

                        var joinMessage = ServerConstants.JoinMessage;

                        if (clientState.User.HasTag(SpecialPlayerUtils.SpecialPlayerTag))
                            joinMessage = SpecialPlayerUtils.SpecialPlayerJoinMessage;

                        if (!player.Invisible)
                        {
                            SendPacketToStage(new ServerChat(
                                clientState.Name, joinMessage, clientState.User.Badges, ChatMessageTypes.PlayerJoinedOrLeft),
                                IMessage.SendModes.ReliableUnordered, clientState.Stage, NetChannels.Chat);
                        }

                        ClientHandshook?.Invoke(client);
                    }
                    break;

                case Packets.ClientVisualState:
                    {
                        var clientState = player.ClientState;
                        var clientVisualState = (ClientVisualState)packet;
                        var oldVisualState = player.ClientVisualState;
                        player.ClientVisualState = clientVisualState;

                        if (oldVisualState != null && oldVisualState.AFK != clientVisualState.AFK && !player.Invisible)
                        {
                            if (clientVisualState.AFK)
                            {
                                SendPacketToStage(new ServerChat(
                                    clientState.Name, ServerConstants.AFKMessage, clientState.User.Badges, ChatMessageTypes.PlayerAFK),
                                    IMessage.SendModes.ReliableUnordered, clientState.Stage, NetChannels.Chat);
                            }
                            else
                            {
                                SendPacketToStage(new ServerChat(
                                    clientState.Name, ServerConstants.LeaveAFKMessage, clientState.User.Badges, ChatMessageTypes.PlayerAFK),
                                    IMessage.SendModes.ReliableUnordered, clientState.Stage, NetChannels.Chat);
                            }
                        }
                    }
                    break;

                case Packets.ClientChat:
                    {
                        var chatPacket = (ClientChat)packet;
                        var chatMessage = chatPacket.Message;
                        chatMessage = TMPFilter.Sanitize(chatMessage);
                        if (chatMessage.Length > Constants.MaxMessageLength)
                            chatMessage = chatMessage.Substring(0, Constants.MaxMessageLength);
                        var logText = $"{player.ClientState.Name}/{TMPFilter.RemoveAllTags(player.ClientState.Name)} ({player.Client.Address}): {chatMessage}";
                        if (LogMessages)
                        {
                            ServerLogger.Log($"[Stage: {player.ClientState.Stage}] {logText}");
                        }
                        if (LogMessagesToFile)
                        {
                            _database.LogChatMessage($"[{DateTime.Now.ToShortTimeString()}] {logText}", player.ClientState.Stage);
                        }
                        if (!TMPFilter.IsValidChatMessage(chatMessage)) return;
                        var lastChatFromThisPlayer = player.LastChatTime;
                        var now = DateTime.UtcNow;
                        var elapsed = now - lastChatFromThisPlayer;
                        if (elapsed.TotalSeconds <= ChatCooldown)
                            break;
                        player.LastChatTime = now;
                        if (chatPacket.Message[0] == Constants.CommandChar)
                        {
                            ProcessCommand(chatMessage, player);
                        }
                        else
                        {
                            if (player.ClientState == null) return;
                            var serverChatPacket = new ServerChat(player.ClientState.Name, chatMessage, player.ClientState.User.Badges, ChatMessageTypes.Chat);
                            SendPacketToStage(serverChatPacket, IMessage.SendModes.ReliableUnordered, player.ClientState.Stage, NetChannels.Chat);
                        }
                    }
                    break;

                default:
                    {
                        if (packet is PlayerPacket)
                        {
                            var playerPacket = packet as PlayerPacket;
                            playerPacket.ClientId = client.Id;
                            if (player.ClientState == null) return;
                            // Exclude sender - will break things if you're trying to display the networked local player clientside.
                            // SendPacketToStage(playerPacket, MessageSendMode.Reliable, _players[client.Id].ClientState.Stage, [client.Id]);
                            var chan = NetChannels.Default;
                            var reliable = IMessage.SendModes.ReliableUnordered;
                            if (packet is PlayerAnimation)
                            {
                                chan = NetChannels.Animation;
                                reliable = PlayerAnimation.ServerSendMode;
                            }
                            SendPacketToStage(playerPacket, reliable, Players[client.Id].ClientState.Stage, chan);
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

        public bool BanPlayerById(ushort id, string reason = "None")
        {
            if (Players.TryGetValue(id, out var result))
            {
                return BanPlayerByAddress(result.Client.Address);
            }
            return false;
        }

        public bool Unban(string address)
        {
            _database.Load();
            if (!_database.BannedUsers.IsBanned(address)) return false;
            _database.BannedUsers.Unban(address);
            ServerLogger.Log($"Unbanned IP {address}");
            _database.Save();
            return true;
        }

        public bool BanPlayerByAddress(string address, string reason = "None")
        {
            _database.Load();
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
            var log = $"Banned IP {address}, player name: {playerName}, reason: {reason}";
            ServerLogger.Log(log);
            if (playerId != 0)
                Server.DisconnectClient(playerId);
            _database.Save();
            return true;
        }

        private void OnClientConnected(object sender, ServerConnectedEventArgs e)
        {
            if (!_database.BannedUsers.IsBanned(e.Client.Address))
                ServerLogger.Log($"Client connected from {e.Client.Address}. ID: {e.Client.Id}. Players: {Players.Count + 1}");
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

        private void OnClientDisconnected(object sender, ServerDisconnectedEventArgs e)
        {
            if (!_database.BannedUsers.IsBanned(e.Client.Address))
            {
                ServerLogger.Log($"Client disconnected from {e.Client.Address}. ID: {e.Client.Id}. Reason: {e.Reason}. Players: {Players.Count - 1}");
            }
            ClientState clientState = null;
            if (Players.TryGetValue(e.Client.Id, out var player))
            {
                ClientDisconnected?.Invoke(e.Client);
                Players.Remove(e.Client.Id);
                clientState = player.ClientState;
            }
            if (clientState != null)
            {
                SendPacketToStage(new ServerClientDisconnected(e.Client.Id), IMessage.SendModes.Reliable, clientState.Stage, NetChannels.ClientAndLobbyUpdates);

                var user = clientState.User;
                var leaveMessage = ServerConstants.LeaveMessage;

                if (user.HasTag(SpecialPlayerUtils.SpecialPlayerTag))
                    leaveMessage = SpecialPlayerUtils.SpecialPlayerLeaveMessage;

                if (!player.Invisible)
                {
                    SendPacketToStage(new ServerChat(
                        clientState.Name, leaveMessage, clientState.User.Badges, ChatMessageTypes.PlayerJoinedOrLeft),
                        IMessage.SendModes.ReliableUnordered, clientState.Stage, NetChannels.Chat);
                }
            }
        }

        private ServerClientStates CreatePlayerClientState(Player player)
        {
            if (player.Invisible) return null;
            if (player.ClientState == null) return null;
            var packet = new ServerClientStates();
            packet.Full = false;
            packet.ClientStates[player.Client.Id] = player.ClientState;
            return packet;
        }

        private ServerClientStates CreateClientStatesPacket(int stage)
        {
            var packet = new ServerClientStates();
            packet.Full = true;
            foreach(var player in Players)
            {
                if (player.Value.Invisible) continue;
                if (player.Value.ClientState == null) continue;
                if (player.Value.ClientState.Stage != stage) continue;
                packet.ClientStates[player.Key] = player.Value.ClientState;
            }
            return packet;
        }

        private const int MaxVisualUpdates = 5;

        private List<ServerClientVisualStates> CreateClientVisualStatesPacket(int stage)
        {
            var packetList = new List<ServerClientVisualStates>();
            packetList.Shuffle();
            var currentNumber = 0;
            var currentPacket = new ServerClientVisualStates();
            foreach (var player in Players)
            {
                if (currentNumber >= MaxVisualUpdates)
                {
                    packetList.Add(currentPacket);
                    currentPacket = new ServerClientVisualStates();
                }
                if (player.Value.Invisible) continue;
                if (player.Value.ClientState == null) continue;
                if (player.Value.ClientState.Stage != stage) continue;
                if (player.Value.ClientVisualState == null) continue;
                currentPacket.ClientVisualStates[player.Key] = player.Value.ClientVisualState;
                currentNumber++;
            }
            packetList.Add(currentPacket);
            return packetList;
        }
    }
}
