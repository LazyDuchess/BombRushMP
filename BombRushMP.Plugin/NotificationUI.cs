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
        private TextMeshProUGUI _gamemodeLabel;
        private TextMeshProUGUI _hostLabel;
        private TextMeshProUGUI _playerCountLabel;
        private TextMeshProUGUI _glyph;
        private float _defaultX = 0f;
        private float _xSize = 0f;
        private Queue<Notification> _notificationQueue = new();

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
        }

        private void SnapClosed()
        {
            var tf = transform.localPosition;
            tf.x = _defaultX - _xSize;
            transform.localPosition = tf;
        }

        public class Notification
        {
            public string PlayerName = "";
            public string LobbyName = "";
            public int LobbyPlayers = 0;

            public Notification(string playerName, string lobbyName, int lobbyPlayers)
            {
                PlayerName = playerName;
                LobbyName = lobbyName;
                LobbyPlayers = lobbyPlayers;
            }
        }
    }
}
