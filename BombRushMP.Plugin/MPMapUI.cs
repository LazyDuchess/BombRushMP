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
        private GameObject _invisPersonIcon;
        private TextMeshProUGUI _invisPersonCountLabel;
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
            _invisPersonIcon = transform.Find("Invis Person Icon").gameObject;
            _invisPersonCountLabel = transform.Find("Invis People Count").GetComponent<TextMeshProUGUI>();
            mapRawImage.texture = mapController.m_Camera.targetTexture;
        }

        private void Update()
        {
            if (!_clientController.Connected)
            {
                _personCountLabel.text = "Offline";
                if (_invisPersonIcon.activeSelf)
                {
                    _invisPersonIcon.SetActive(false);
                    _invisPersonCountLabel.gameObject.SetActive(false);
                }
            }
            else
            {
                _personCountLabel.text = GetVisiblePlayerCount().ToString();
                if (_clientController.Connected && _clientController.GetLocalUser().IsModerator)
                {
                    if (!_invisPersonIcon.activeSelf)
                    {
                        _invisPersonIcon.SetActive(true);
                        _invisPersonCountLabel.gameObject.SetActive(true);
                    }
                    _invisPersonCountLabel.text = GetInvisiblePlayerCount().ToString();
                }
            }
            _notificationCountLabel.text = _clientController.ClientLobbyManager.LobbiesInvited.Count.ToString();
        }

        private int GetVisiblePlayerCount()
        {
            var cnt = 0;
            foreach(var ply in _clientController.Players)
            {
                if (ply.Value.ClientState != null)
                {
                    if (!ply.Value.ClientState.ServerInvisible)
                        cnt++;
                }
            }
            return cnt;
        }

        private int GetInvisiblePlayerCount()
        {
            var cnt = 0;
            foreach (var ply in _clientController.Players)
            {
                if (ply.Value.ClientState != null)
                {
                    if (ply.Value.ClientState.ServerInvisible)
                        cnt++;
                }
            }
            return cnt;
        }
    }
}
