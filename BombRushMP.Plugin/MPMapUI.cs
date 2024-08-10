using Reptile;
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
    public class MPMapUI : MonoBehaviour
    {
        private TextMeshProUGUI _personCountLabel;
        private TextMeshProUGUI _notificationCountLabel;
        private ClientController _clientController;

        private void Awake()
        {
            var mapController = Mapcontroller.Instance;
            _clientController = ClientController.Instance;
            var mapRawImage = transform.Find("Map Texture").GetComponent<RawImage>();
            _personCountLabel = transform.Find("People Count").GetComponent<TextMeshProUGUI>();
            _notificationCountLabel = transform.Find("Notification Count").GetComponent<TextMeshProUGUI>();
            mapRawImage.texture = mapController.m_Camera.targetTexture;
        }

        private void Update()
        {
            if (!_clientController.Connected)
                _personCountLabel.text = "Offline";
            else
                _personCountLabel.text = _clientController.Players.Count.ToString();
            _notificationCountLabel.text = _clientController.ClientLobbyManager.LobbiesInvited.Count.ToString();
        }
    }
}
