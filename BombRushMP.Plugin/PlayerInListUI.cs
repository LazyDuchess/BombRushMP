using BombRushMP.Common;
using BombRushMP.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BombRushMP.Plugin
{
    public class PlayerInListUI : MonoBehaviour
    {
        private TextMeshProUGUI _label;
        private Button _spectateButton;
        private Button _banButton;
        private MPPlayer _player;
        private GameObject _afkIndicator;
        private void Awake()
        {
            _label = transform.Find("Name").GetComponent<TextMeshProUGUI>();
            _label.spriteAsset = MPAssets.Instance.Sprites;
            _spectateButton = transform.Find("Spectate Button").GetComponent<Button>();
            _banButton = transform.Find("Ban Button").GetComponent<Button>();
            _spectateButton.onClick.AddListener(TrySpectate);
            _banButton.onClick.AddListener(TryBan);
            _afkIndicator = transform.Find("AFK").gameObject;
        }

        private void Update()
        {
            if (_player != null && _player.ClientVisualState != null)
            {
                _afkIndicator.SetActive(_player.ClientVisualState.AFK);
            }
        }

        public void SetPlayer(MPPlayer player)
        {
            var clientController = ClientController.Instance;
            _player = player;
            var nameText = MPUtility.GetPlayerDisplayName(player.ClientState);
            var user = clientController.GetLocalUser();
            if (user.IsModerator)
                nameText = $"[{player.ClientId}] {nameText}";
            _label.text = nameText;
            var localId = clientController.LocalID;

            _banButton.gameObject.SetActive(false);
            _spectateButton.gameObject.SetActive(false);

            if (localId != player.ClientId)
            {
                _spectateButton.gameObject.SetActive(true);
                if (user.IsModerator)
                    _banButton.gameObject.SetActive(true);
            }
        }

        private void TrySpectate()
        {
            if (_player == null) return;
            SpectatorController.StartSpectating();
            SpectatorController.Instance.SpectatePlayer(_player);
        }

        private void TryBan()
        {
            if (_player == null) return;
            ClientController.Instance.SendPacket(new ClientChat($"{Constants.CommandChar}banid {_player.ClientId}"), Common.Networking.IMessage.SendModes.ReliableUnordered, Common.Networking.NetChannels.Chat);
        }

        public static PlayerInListUI Create(GameObject gameObject)
        {
            var go = Instantiate(gameObject);
            go.SetActive(true);
            return go.AddComponent<PlayerInListUI>();
        }
    }
}
