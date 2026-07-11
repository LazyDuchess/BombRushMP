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
    public class StatusTextUI : MonoBehaviour
    {
        public const int DefaultPriority = 0;
        public const float DefaultTime = 1f;
        public static StatusTextUI Instance { get; private set; }
        private TextMeshProUGUI _statusLabel;
        private bool Displaying => _currentTimer > 0f;
        private int _currentPriority = -1;
        private float _currentTimer = 0f;

        private void Awake()
        {
            Instance = this;
            Init();
            _statusLabel.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_statusLabel.gameObject.activeSelf != Displaying)
                _statusLabel.gameObject.SetActive(Displaying);
            _currentTimer -= Time.deltaTime;
            if (_currentTimer < 0f)
                _currentTimer = 0f;
        }

        public void TryShowStatus(int priority = DefaultPriority, string status = "", float time = DefaultTime)
        {
            if (priority >= _currentPriority || _currentTimer <= 0f)
            {
                ForceShowStatus(priority, status, time);
            }
        }

        public void ForceShowStatus(int priority = DefaultPriority, string status = "", float time = DefaultTime)
        {
            _currentTimer = time;
            _currentPriority = priority;
            _statusLabel.text = status;
        }

        static TextMeshProUGUI MakeLabel(TextMeshProUGUI reference, string name)
        {
            var font = reference.font;
            var fontSize = reference.fontSize;
            var fontMaterial = reference.fontMaterial;

            var labelObj = new GameObject(name);
            var newLabel = labelObj.AddComponent<TextMeshProUGUI>();
            newLabel.font = font;
            newLabel.fontSize = fontSize;
            newLabel.fontMaterial = fontMaterial;
            newLabel.alignment = TextAlignmentOptions.MidlineLeft;
            newLabel.fontStyle = FontStyles.Bold;
            newLabel.outlineWidth = 0.2f;

            return newLabel;
        }

        void Init()
        {
            var rectParent = Instance.gameObject.AddComponent<RectTransform>();
            rectParent.anchorMin = Vector2.zero;
            rectParent.anchorMax = Vector2.one;

            var uiManager = Core.Instance.UIManager;
            var labels = uiManager.danceAbilityUI.GetComponentsInChildren<TextMeshProUGUI>(true);
            TextMeshProUGUI referenceText = null;
            foreach (var label in labels)
            {
                if (label.transform.gameObject.name == "DanceSelectConfirmText")
                    referenceText = label;
            }

            if (referenceText == null)
                return;

            _statusLabel = MakeLabel(referenceText, "StatusLabel");
            _statusLabel.text = "";
            _statusLabel.rectTransform.anchorMin = new Vector2(0.5f, 1.0f);
            _statusLabel.rectTransform.anchorMax = new Vector2(0.5f, 1.0f);
            _statusLabel.rectTransform.pivot = new Vector2(0f, 1f);
            _statusLabel.rectTransform.anchoredPosition = new Vector2(-100f, -100f);
            _statusLabel.rectTransform.sizeDelta = new Vector2(500f, 500f);
            _statusLabel.horizontalAlignment = HorizontalAlignmentOptions.Center;
            _statusLabel.verticalAlignment = VerticalAlignmentOptions.Top;
            _statusLabel.rectTransform.SetParent(rectParent, false);
        }

        public static void Create()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            var go = new GameObject("ACN Status Text");
            go.transform.SetParent(Core.Instance.UIManager.gameplay.transform.parent.GetComponent<RectTransform>(), false);
            go.AddComponent<StatusTextUI>();
        }
    }
}