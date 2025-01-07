using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class NotificationUI : MonoBehaviour
    {
        public bool HasNotificationUp => _state != States.Closed;
        private float _speed = 750f;
        private float _stayTime = 4f;
        private TextMeshProUGUI _gamemodeLabel;
        private TextMeshProUGUI _hostLabel;
        private TextMeshProUGUI _playerCountLabel;
        private TextMeshProUGUI _glyph;
        private float _defaultX = 0f;
        private float _xSize = 0f;
        private Queue<Notification> _notificationQueue = new();
        private States _state = States.Closed;
        private float _stateTimer = 0f;
        public bool Active = true;

        private void Awake()
        {
            _gamemodeLabel = transform.Find("Gamemode").GetComponent<TextMeshProUGUI>();
            _hostLabel = transform.Find("Host").GetComponent<TextMeshProUGUI>();
            _playerCountLabel = transform.Find("People Count").GetComponent<TextMeshProUGUI>();
            _glyph = transform.Find("Glyph").GetComponent<TextMeshProUGUI>();

            _xSize = transform.RectTransform().sizeDelta.x;
            _defaultX = transform.localPosition.x;

            UIUtility.MakeGlyph(_glyph, 21);
            SnapClosed();
            Active = gameObject.activeSelf;
        }

        private void OnDisable()
        {
            Active = false;
            SnapClosed();
        }

        private void OnEnable()
        {
            Active = true;
            SnapClosed();
        }

        private void Update()
        {
            _stateTimer += Time.deltaTime;
            UpdateAnimation();
            if (_state == States.Open && _stateTimer >= _stayTime)
                SetState(States.Closing);
            if (_state == States.Closed && CanOpenAtThisTime())
                OpenNextNotificationIfPossible();
            if (!CanOpenAtThisTime())
                SnapClosed();
        }

        public void QueueInviteNotification(Lobby lobby)
        {
            var clientController = ClientController.Instance;
            var hostPlayer = clientController.Players[lobby.LobbyState.HostId].ClientState.Name;
            var notif = new Notification(hostPlayer, clientController.ClientLobbyManager.GetLobbyName(lobby.LobbyState.Id), lobby.LobbyState.Players.Count, lobby.LobbyState.Id);
            _notificationQueue.Enqueue(notif);
            Core.Instance.AudioManager.PlaySfxUI(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_RingTone);
        }

        public void RemoveNotificationForLobby(uint lobby)
        {
            _notificationQueue = new Queue<Notification>(_notificationQueue.Where(x => x.Lobby != lobby));
        }

        public void RemoveAllNotifications()
        {
            _notificationQueue.Clear();
        }

        public bool CanQueueAtThisTime()
        {
            var mpSettings = MPSettings.Instance;
            if (!mpSettings.ShowNotifications) return false;
            return true;
        }

        public bool CanOpenAtThisTime()
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return false;
            var uiManager = Core.Instance.UIManager;
            if (!CanQueueAtThisTime()) return false;
            var player = WorldHandler.instance.GetCurrentPlayer();
            if (player.phone.state != Reptile.Phone.Phone.PhoneState.OFF) return false;
            if (!uiManager.gameplay.gameplayScreen.gameObject.activeSelf) return false;
            var currentLobby = clientController.ClientLobbyManager.CurrentLobby;
            if (currentLobby != null && currentLobby.InGame) return false;
            return true;
        }

        private void OpenNextNotificationIfPossible()
        {
            if (_notificationQueue.Count == 0) return;
            var nextNotif = _notificationQueue.Dequeue();
            SetNotification(nextNotif);
            SetState(States.Opening);
        }

        private void SetNotification(Notification notification)
        {
            _gamemodeLabel.text = notification.LobbyName;
            _hostLabel.text = notification.PlayerName;
            _playerCountLabel.text = notification.LobbyPlayers.ToString();
        }

        private void UpdateAnimation()
        {
            switch (_state)
            {
                case States.Closed:
                    SnapClosed();
                    break;
                case States.Open:
                    SnapOpen();
                    break;
                case States.Opening:
                    {
                        var tf = transform.localPosition;
                        tf.x += _speed * Time.deltaTime;
                        transform.localPosition = tf;
                        if (tf.x >= _defaultX)
                            SetState(States.Open);
                    }
                    break;
                case States.Closing:
                    {
                        var tf = transform.localPosition;
                        tf.x -= _speed * Time.deltaTime;
                        transform.localPosition = tf;
                        if (tf.x <= _defaultX - _xSize)
                            SetState(States.Closed);
                    }
                    break;
            }
        }

        private void SetState(States state)
        {
            if (state != _state)
                _stateTimer = 0f;
            _state = state;
        }

        private void SnapOpen()
        {
            SetState(States.Open);
            var tf = transform.localPosition;
            tf.x = _defaultX;
            transform.localPosition = tf;
        }

        private void SnapClosed()
        {
            SetState(States.Closed);
            var tf = transform.localPosition;
            tf.x = _defaultX - _xSize;
            transform.localPosition = tf;
        }

        private enum States
        {
            Closed,
            Opening,
            Open,
            Closing
        }

        private class Notification
        {
            public string PlayerName = "";
            public string LobbyName = "";
            public int LobbyPlayers = 0;
            public uint Lobby = 0;

            public Notification(string playerName, string lobbyName, int lobbyPlayers, uint associatedLobby = 0)
            {
                PlayerName = playerName;
                LobbyName = lobbyName;
                LobbyPlayers = lobbyPlayers;
                Lobby = associatedLobby;
            }
        }
    }
}
