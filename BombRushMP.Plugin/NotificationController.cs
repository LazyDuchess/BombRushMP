using BombRushMP.Common.Packets;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class NotificationController : MonoBehaviour
    {
        public static NotificationController Instance { get; private set; }
        private NotificationUI _notificationUI;
        private UIManager _uiManager;

        private void Awake()
        {
            Instance = this;
            var notifUIGameObject = transform.Find("Canvas").Find("Notification").gameObject;
            _notificationUI = notifUIGameObject.AddComponent<NotificationUI>();
            ClientController.PacketReceived += OnPacketReceived;
        }

        private void OnDestroy()
        {
            ClientController.PacketReceived += OnPacketReceived;
        }

        public void RemoveNotificationForLobby(uint lobby)
        {
            _notificationUI.RemoveNotificationForLobby(lobby);
        }

        public void RemoveAllNotifications()
        {
            _notificationUI.RemoveAllNotifications();
        }

        private void OnPacketReceived(Packets packetId, Packet packet)
        {
            var clientController = ClientController.Instance;
            var lobbyManager = clientController.ClientLobbyManager;
            if (packetId == Packets.ServerLobbyInvite)
            {
                var invitePacket = (ServerLobbyInvite)packet;
                var inviter = invitePacket.InviterId;
                var invitee = invitePacket.InviteeId;
                var lobby = invitePacket.LobbyId;
                if (invitee != clientController.LocalID) return;
                if (_notificationUI.CanQueueAtThisTime())
                {
                    _notificationUI.QueueInviteNotification(lobbyManager.Lobbies[lobby]);
                }
            }
        }

        public static void Create()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            var mpAssets = MPAssets.Instance;
            var prefab = mpAssets.Bundle.LoadAsset<GameObject>("Notification UI");
            var notifUI = Instantiate(prefab);
            notifUI.transform.SetParent(Core.Instance.UIManager.transform, false);
            notifUI.AddComponent<NotificationController>();
        }
    }
}
