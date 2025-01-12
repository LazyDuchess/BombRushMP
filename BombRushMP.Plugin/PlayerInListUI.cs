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
        private void Awake()
        {
            _label = transform.Find("Name").GetComponent<TextMeshProUGUI>();
            _spectateButton = transform.Find("Spectate Button").GetComponent<Button>();
            _banButton = transform.Find("Ban Button").GetComponent<Button>();
            _spectateButton.onClick.AddListener(TrySpectate);
            _banButton.onClick.AddListener(TryBan);
        }

        public void SetPlayer(MPPlayer player)
        {
            _player = player;
            _label.text = MPUtility.GetPlayerDisplayName(player.ClientState);
            var clientController = ClientController.Instance;

            var user = clientController.GetLocalUser();
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
    }
}
