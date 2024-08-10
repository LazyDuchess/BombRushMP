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

        private void Awake()
        {
            _gamemodeLabel = transform.Find("Gamemode").GetComponent<TextMeshProUGUI>();
            _hostLabel = transform.Find("Host").GetComponent<TextMeshProUGUI>();
            _playerCountLabel = transform.Find("People Count").GetComponent<TextMeshProUGUI>();
            _glyph = transform.Find("Glyph").GetComponent<TextMeshProUGUI>();
            UIUtility.MakeGlyph(_glyph, 21);
        }
    }
}
