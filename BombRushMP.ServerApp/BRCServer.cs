using Riptide;
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
        private Dictionary<ushort, Player> _players = new();
        private Server _server;
        private Stopwatch _tickStopWatch;
        private float _tickTimer = 0f;
        public BRCServer(ushort port, ushort maxPlayers)
        {
            _tickStopWatch = new Stopwatch();
            _tickStopWatch.Start();
            _server = new Server();
            _server.ClientConnected += OnClientConnected;
            _server.ClientDisconnected += OnClientDisconnected;
            _server.MessageReceived += OnMessageReceived;
            _server.Start(port, maxPlayers);
            Log($"Starting server on port {port} with max players {maxPlayers}");
        }

        public void DisconnectClient(ushort id)
        {
            _server.DisconnectClient(id);
        }

        public void Update()
        {
            _tickStopWatch.Stop();
            var deltaTime = (float)_tickStopWatch.Elapsed.TotalSeconds;
            _tickTimer += deltaTime;
            _tickStopWatch.Restart();
            if (_tickTimer >= Constants.NetworkingTickRate)
            {
                Tick(_tickTimer);
                _tickTimer = 0f;
            }
        }

        private void Tick(float deltaTime)
        {
            _server.Update();
            foreach(var player in _players)
            {
                player.Value.Tick(deltaTime);
            }
            var stages = GetActiveStages();
            foreach(var stage in stages)
            {
                TickStage(stage);
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
            foreach (var player in _players)
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

        public void SendPacketToStage(Packet packet, MessageSendMode sendMode, int stage)
        {
            var message = PacketFactory.MessageFromPacket(packet, sendMode);
            foreach (var player in _players)
            {
                if (player.Value.ClientState == null) continue;
                if (player.Value.ClientState.Stage != stage) continue;
                player.Value.Client.Send(message);
            }
        }

        public void SendPacketToClient(Packet packet, MessageSendMode sendMode, Connection client)
        {
            var message = PacketFactory.MessageFromPacket(packet, sendMode);
            client.Send(message);
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var packetId = (Packets)e.MessageId;
            var packet = PacketFactory.PacketFromMessage(packetId, e.Message);
            if (packet == null) return;
            if (!_players.TryGetValue(e.FromConnection.Id, out var result)) return;
            switch (packetId)
            {
                case Packets.ClientState:
                    {
                        var clientState = (ClientState)packet;
                        var oldClientState = _players[e.FromConnection.Id].ClientState;
                        if (oldClientState != null)
                        {
                            var clientStateUpdatePacket = new ServerClientStates();
                            clientStateUpdatePacket.ClientStates[e.FromConnection.Id] = clientState;
                            SendPacketToStage(clientStateUpdatePacket, MessageSendMode.Reliable,oldClientState.Stage);
                            return;
                        }
                        if (clientState.ProtocolVersion != Constants.ProtocolVersion)
                        {
                            Log($"Rejecting player from {e.FromConnection} (ID: {e.FromConnection.Id}) because of protocol version mismatch (Server: {Constants.ProtocolVersion}, Client: {clientState.ProtocolVersion}).");
                            _server.DisconnectClient(e.FromConnection);
                            return;
                        }
                        _players[e.FromConnection.Id].ClientState = clientState;
                        Log($"Player from {e.FromConnection} (ID: {e.FromConnection.Id}) connected as {clientState.Name} in stage {clientState.Stage}. Protocol Version: {clientState.ProtocolVersion}");
                        SendPacketToClient(new ServerConnectionResponse() { LocalClientId = e.FromConnection.Id }, MessageSendMode.Reliable, e.FromConnection);
                        var clientStates = CreateClientStatesPacket(clientState.Stage);
                        SendPacketToStage(clientStates, MessageSendMode.Reliable, clientState.Stage);
                    }
                    break;

                case Packets.ClientVisualState:
                    {
                        var clientVisualState = (ClientVisualState)packet;
                        _players[e.FromConnection.Id].ClientVisualState = clientVisualState;
                    }
                    break;

                case Packets.PlayerAnimation:
                    {
                        var playerAnimation = (PlayerAnimation)packet;
                        playerAnimation.ClientId = e.FromConnection.Id;
                        var player = _players[e.FromConnection.Id];
                        if (player.ClientState == null) return;
                        SendPacketToStage(packet, MessageSendMode.Reliable, _players[e.FromConnection.Id].ClientState.Stage);
                    }
                    break;

                case Packets.PlayerVoice:
                    {
                        var playerVoice = (PlayerVoice)packet;
                        playerVoice.ClientId = e.FromConnection.Id;
                        var player = _players[e.FromConnection.Id];
                        if (player.ClientState == null) return;
                        SendPacketToStage(packet, MessageSendMode.Reliable, _players[e.FromConnection.Id].ClientState.Stage);
                    }
                    break;
            }
        }

        private void OnClientConnected(object sender, ServerConnectedEventArgs e)
        {
            Log($"Client connected from {e.Client}. ID: {e.Client.Id}.");
            var player = new Player();
            player.Client = e.Client;
            player.Server = this;
            _players[e.Client.Id] = player;
        }

        private void OnClientDisconnected(object sender, ServerDisconnectedEventArgs e)
        {
            Log($"Client disconnected from {e.Client}. ID: {e.Client.Id}.");
            ClientState clientState = null;
            if (_players.TryGetValue(e.Client.Id, out var result))
            {
                _players.Remove(e.Client.Id);
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
            foreach(var player in _players)
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
            foreach(var player in _players)
            {
                if (player.Value.ClientState == null) continue;
                if (player.Value.ClientState.Stage != stage) continue;
                if (player.Value.ClientVisualState == null) continue;
                packet.ClientVisualStates[player.Key] = player.Value.ClientVisualState;
            }
            return packet;
        }

        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
