using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class LobbyPlayerUI : MonoBehaviour
    {
        private TextMeshProUGUI _playerName;
        private ClientController _clientController;
        private void Awake()
        {
            _playerName = transform.Find("Label").GetComponent<TextMeshProUGUI>();
            _clientController = ClientController.Instance;
        }

        public void SetPlayer(ushort playerId)
        {
            _playerName.text = _clientController.Players[playerId].ClientState.Name;
        }

        public static LobbyPlayerUI Create(GameObject reference)
        {
            var go = Instantiate(reference);
            go.SetActive(true);
            var playerui = go.AddComponent<LobbyPlayerUI>();
            return playerui;
        }
    }
}
