﻿using Riptide;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Riptide.Utils;
using Riptide.Transports.Udp;
using BombRushMP.Common.Packets;
using BombRushMP.Common;
using System.Diagnostics;
using Riptide.Transports;
using System.Security.Policy;

namespace BombRushMP.ServerApp
{
    public class BRCServer : IDisposable
    {
        public static BRCServer Instance { get; private set; }
        public ServerLobbyManager ServerLobbyManager;
        public Action<Connection> ClientHandshook;
        public Action<Connection> ClientDisconnected;
        public Action<Connection, Packets, Packet> PacketReceived;
        public Action<float> OnTick;
        public Dictionary<ushort, Player> Players = new();
        private Server _server;
        private Stopwatch _tickStopWatch;
        private HashSet<int> _activeStages;
        private float _playerCountTickTimer = 0f;

        public BRCServer(ushort port, ushort maxPlayers)
        {
            Instance = this;
            Message.MaxPayloadSize = Constants.MaxPayloadSize;
            ServerLobbyManager = new();
            _tickStopWatch = new Stopwatch();
            _tickStopWatch.Start();
            _server = new Server();
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
                SendPacket(new ServerPlayerCount(playerCountDictionary), MessageSendMode.Unreliable);
            }
        }

        private void TickStage(int stage)
        {
            var clientVisualStates = CreateClientVisualStatesPacket(stage);
            SendPacketToStage(clientVisualStates, MessageSendMode.Unreliable, stage);
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

        public void SendPacket(Packet packet, MessageSendMode sendMode, ushort[] except = null)
        {
            var message = PacketFactory.MessageFromPacket(packet, sendMode);
            foreach (var player in Players)
            {
                if (player.Value.ClientState == null) continue;
                if (except != null && except.Contains(player.Key)) continue;
                player.Value.Client.Send(message);
            }
        }

        public void SendPacketToStage(Packet packet, MessageSendMode sendMode, int stage, ushort[] except = null)
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

        public void SendPacketToClient(Packet packet, MessageSendMode sendMode, Connection client)
        {
            var message = PacketFactory.MessageFromPacket(packet, sendMode);
            client.Send(message);
        }

        private void OnPacketReceived(Connection client, Packets packetId, Packet packet)
        {
            switch (packetId)
            {
                case Packets.ClientState:
                    {
                        var clientState = (ClientState)packet;
                        var oldClientState = Players[client.Id].ClientState;
                        Players[client.Id].ClientState = clientState;
                        if (oldClientState != null)
                        {
                            var clientStateUpdatePacket = new ServerClientStates();
                            clientStateUpdatePacket.ClientStates[client.Id] = clientState;
                            SendPacketToStage(clientStateUpdatePacket, MessageSendMode.Reliable, oldClientState.Stage);
                            return;
                        }
                        if (clientState.ProtocolVersion != Constants.ProtocolVersion)
                        {
                            ServerLogger.Log($"Rejecting player from {client} (ID: {client.Id}) because of protocol version mismatch (Server: {Constants.ProtocolVersion}, Client: {clientState.ProtocolVersion}).");
                            _server.DisconnectClient(client);
                            return;
                        }
                        ServerLogger.Log($"Player from {client} (ID: {client.Id}) connected as {clientState.Name} in stage {clientState.Stage}. Protocol Version: {clientState.ProtocolVersion}");
                        SendPacketToClient(new ServerConnectionResponse() { LocalClientId = client.Id }, MessageSendMode.Reliable, client);
                        var clientStates = CreateClientStatesPacket(clientState.Stage);
                        SendPacketToStage(clientStates, MessageSendMode.Reliable, clientState.Stage);
                        ClientHandshook?.Invoke(client);
                    }
                    break;

                case Packets.ClientVisualState:
                    {
                        var clientVisualState = (ClientVisualState)packet;
                        Players[client.Id].ClientVisualState = clientVisualState;
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
                            SendPacketToStage(playerPacket, MessageSendMode.Reliable, Players[client.Id].ClientState.Stage);
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
                _server.DisconnectClient(e.FromConnection);
            }
        }

        private void OnClientConnected(object sender, ServerConnectedEventArgs e)
        {
            ServerLogger.Log($"Client connected from {e.Client}. ID: {e.Client.Id}.");
            var player = new Player();
            player.Client = e.Client;
            player.Server = this;
            Players[e.Client.Id] = player;
        }

        private void OnClientDisconnected(object sender, ServerDisconnectedEventArgs e)
        {
            ServerLogger.Log($"Client disconnected from {e.Client}. ID: {e.Client.Id}.");
            ClientDisconnected?.Invoke(e.Client);
            ClientState clientState = null;
            if (Players.TryGetValue(e.Client.Id, out var result))
            {
                Players.Remove(e.Client.Id);
                clientState = result.ClientState;
            }
            if (clientState != null)
            {
                var clientStates = CreateClientStatesPacket(clientState.Stage);
                SendPacketToStage(clientStates, MessageSendMode.Reliable, clientState.Stage);
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
