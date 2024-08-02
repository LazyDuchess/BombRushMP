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
        public Dictionary<Player, MPPlayer> MultiplayerPlayerByPlayer = new();
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
            packet.MoveStyleEquipped = player.usingEquippedMovestyle;
            packet.MoveStyle = (int)player.moveStyleEquipped;
            packet.SetUnityPosition(player.gameObject.transform.position);
            packet.SetUnityVisualPosition(player.visualTf.localPosition);
            packet.SetUnityRotation(player.gameObject.transform.rotation);
            packet.SetUnityVisualRotation(player.visualTf.localRotation);
            packet.SetUnityVeolcity(player.motor._rigidbody.velocity);
            packet.GrindDirection = player.anim.GetFloat(ClientConstants.GrindDirectionHash);
            packet.SprayCanHeld = player.spraycanState == Player.SpraycanState.START || player.spraycanState == Player.SpraycanState.SHAKE;
            packet.PhoneHeld = player.characterVisual.phoneActive;
            packet.PhoneDirectionX = player.anim.GetFloat(ClientConstants.PhoneDirectionXHash);
            packet.PhoneDirectionY = player.anim.GetFloat(ClientConstants.PhoneDirectionYHash);
            packet.TurnDirection1 = player.anim.GetFloat(ClientConstants.TurnDirection1Hash);
            packet.TurnDirection2 = player.anim.GetFloat(ClientConstants.TurnDirection2Hash);
            packet.TurnDirection3 = player.anim.GetFloat(ClientConstants.TurnDirection3Hash);
            packet.TurnDirectionSkateboard = player.anim.GetFloat(ClientConstants.TurnDirectionSkateboardHash);
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

                case Packets.PlayerVoice:
                    {
                        var playerVoice = (PlayerVoice)packet;
                        if (Players.TryGetValue(playerVoice.ClientId, out var player))
                        {
                            if (player.Player != null)
                                player.Player.PlayVoice((AudioClipID)playerVoice.AudioClipId, (VoicePriority)playerVoice.VoicePriority, true);
                        }
                    }
                    break;

                case Packets.PlayerSpray:
                    {
                        var playerSpray = (PlayerSpray)packet;
                        if (Players.TryGetValue(playerSpray.ClientId, out var player))
                        {
                            if (player.Player != null)
                                player.Player.SetSpraycanState(Player.SpraycanState.SPRAY);
                        }
                    }
                    break;

                case Packets.PlayerTeleport:
                    {
                        var playerTp = (PlayerTeleport)packet;
                        if (Players.TryGetValue(playerTp.ClientId, out var player))
                        {
                            player.Teleporting = true;
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

        public void SendClientState()
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            var statePacket = new ClientState()
            {
                Name = Username,
                Character = (int)player.character,
                Outfit = Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(player.character).outfit,
                Stage = (int)Reptile.Utility.SceneNameToStage(SceneManager.GetActiveScene().name),
                ProtocolVersion = Constants.ProtocolVersion
            };
            SendPacket(statePacket, MessageSendMode.Reliable);
        }

        private void OnConnected(object sender, EventArgs e)
        {
            Log("Connected!");
            SendClientState();
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
