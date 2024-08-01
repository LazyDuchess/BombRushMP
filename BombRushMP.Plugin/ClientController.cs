using BombRushMP.Common;
using BombRushMP.Common.Packets;
using Reptile;
using Riptide;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BombRushMP.Plugin
{
    public class ClientController : MonoBehaviour
    {
        public static ClientController Instance { get; private set; }
        public HashSet<Player> PlayerRegistry = new();
        public Dictionary<ushort, MPPlayer> Players = new();
        /// <summary>
        /// Whether the client is connected to the server and performed the initial handshake.
        /// </summary>
        public bool Connected => _client != null && _client.IsConnected && _handShook;
        public ushort LocalID = 0;
        public string Address = "";
        public string Username = "Goofiest Gooner";
        private Client _client;
        private float _tickTimer = 0f;
        private bool _handShook = false;

        public void Connect()
        {
            Log($"Connecting to {Address}");
            _client = new Client();
            _client.Connect(Address);
            _client.Connected += OnConnected;
            _client.Disconnected += OnDisconnect;
            _client.ConnectionFailed += OnConnectionFailed;
            _client.MessageReceived += OnMessageReceived;
            _client.ClientDisconnected += OnClientDisconnected;
        }

        public void Disconnect()
        {
            foreach(var player in Players)
            {
                player.Value.Delete();
            }
            Players.Clear();
            _handShook = false;
            LocalID = 0;
            if (_client != null)
            {
                _client.Connected -= OnConnected;
                _client.Disconnected -= OnDisconnect;
                _client.ConnectionFailed -= OnConnectionFailed;
                _client.MessageReceived -= OnMessageReceived;
                _client.ClientDisconnected -= OnClientDisconnected;
                _client.Disconnect();
            }
            _client = null;
        }

        public void SendPacket(Packet packet, MessageSendMode sendMode)
        {
            var message = PacketFactory.MessageFromPacket(packet, sendMode);
            _client.Send(message);
        }

        public static ClientController Create(string address)
        {
            var clientControllerGO = new GameObject("Client Controller");
            var clientController = clientControllerGO.AddComponent<ClientController>();
            clientController.Address = address;
            clientController.Connect();
            Instance = clientController;
            return clientController;
        }

        private ClientVisualState CreateVisualStatePacket(Player player)
        {
            var packet = new ClientVisualState();
            packet.SetUnityPosition(player.gameObject.transform.position);
            packet.SetUnityRotation(player.gameObject.transform.rotation);
            return packet;
        }

        private void SendVisualState()
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            if (player == null) return;
            var packet = CreateVisualStatePacket(player);
            SendPacket(packet, MessageSendMode.Unreliable);
        }

        private void Tick()
        {
            _client.Update();
            if (Connected)
            {
                SendVisualState();
            }
        }

        private void Update()
        {
            _tickTimer += Time.deltaTime;
            if (_tickTimer >= Constants.NetworkingTickRate)
            {
                Tick();
                _tickTimer = 0f;
            }
            foreach(var player in Players)
            {
                player.Value.FrameUpdate();
            }
        }

        private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            if (Players.TryGetValue(e.Id, out var player))
            {
                player.Delete();
                Players.Remove(e.Id);
            }
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var packetId = (Packets)e.MessageId;
            var packet = PacketFactory.PacketFromMessage(packetId, e.Message);
            if (packet == null) return;
            switch (packetId)
            {
                case Packets.ServerConnectionResponse:
                    {
                        var connectionResponse = (ServerConnectionResponse)packet;
                        LocalID = connectionResponse.LocalClientId;
                        _handShook = true;
                        Log($"Received server handshake - our local ID is {connectionResponse.LocalClientId}.");
                    }
                    break;

                case Packets.ServerClientStates:
                    {
                        var clientStates = (ServerClientStates)packet;
                        foreach (var clientState in clientStates.ClientStates)
                        {
                            if (!Players.TryGetValue(clientState.Key, out var player))
                            {
                                player = new MPPlayer();
                                Players[clientState.Key] = player;
                            }
                            player.ClientState = clientState.Value;
                            player.ClientId = clientState.Key;
                        }
                    }
                    break;

                case Packets.ServerClientVisualStates:
                    {
                        var clientVisualStates = (ServerClientVisualStates)packet;
                        foreach (var clientVisualState in clientVisualStates.ClientVisualStates)
                        {
                            if (!Players.TryGetValue(clientVisualState.Key, out var player)) continue;
                            player.ClientVisualState = clientVisualState.Value;
                        }
                    }
                    break;

                case Packets.PlayerAnimation:
                    {
                        var playerAnimation = (PlayerAnimation)packet;
                        if (Players.TryGetValue(playerAnimation.ClientId, out var player))
                        {
                            if (player.Player != null)
                                MPUtility.PlayAnimationOnMultiplayerPlayer(player.Player, playerAnimation.NewAnim, playerAnimation.ForceOverwrite, playerAnimation.Instant, playerAnimation.AtTime);
                        }
                    }
                    break;
            }
        }

        private void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Log($"Failed to connect to server. Reason: {e.Reason}");
            Log("Will attempt to re-connect");
            Disconnect();
            Connect();
        }

        private void OnDisconnect(object sender, DisconnectedEventArgs e)
        {
            Log($"Disconnected! Reason: {e.Reason}");
            Log("Will attempt to re-connect");
            Disconnect();
            Connect();
        }

        private void OnConnected(object sender, EventArgs e)
        {
            Log("Connected!");
            var statePacket = new ClientState()
            {
                Name = Username,
                Stage = (int)Reptile.Utility.SceneNameToStage(SceneManager.GetActiveScene().name),
                ProtocolVersion = Constants.ProtocolVersion
            };
            SendPacket(statePacket, MessageSendMode.Reliable);
        }

        private void Log(string message)
        {
            Debug.Log(message);
        }

        private void OnDestroy()
        {
            Disconnect();
        }
    }
}
