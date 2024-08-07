using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using TMPro;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class SpectatorUI : MonoBehaviour
    {
        internal static SpectatorUI Instance;
        private TextMeshProUGUI _nextLabel;
        private TextMeshProUGUI _previousLabel;
        private TextMeshProUGUI _backLabel;

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

        static TextMeshProUGUI MakeGlyph(TextMeshProUGUI reference, int actionID)
        {
            var label = MakeLabel(reference, "");
            var glyph = label.gameObject.AddComponent<UIButtonGlyphComponent>();
            glyph.inputActionID = actionID;
            glyph.localizedGlyphTextComponent = label;
            glyph.localizedTextComponent = label;
            glyph.enabled = true;
            return label;
        }

        internal static void InitializeUI(GameplayUI gameplayUI)
        {
            if (Instance != null)
                return;
            Instance = new GameObject("Spectator UI Root").AddComponent<SpectatorUI>();
            Instance.transform.SetParent(gameplayUI.transform.parent.GetComponent<RectTransform>(), false);
            Instance.Init();
            Instance.Deactivate();
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

            var labelLeft = 100f;
            var labelSeparation = -50f;
            var labelBegin = 50f;
            var glyphOffset = 75f;

            _previousLabel = MakeLabel(referenceText, "PreviousPlayer");
            _previousLabel.text = "Previous Player";
            _previousLabel.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _previousLabel.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _previousLabel.rectTransform.pivot = new Vector2(0f, 1f);
            _previousLabel.rectTransform.anchoredPosition = new Vector2(labelLeft + glyphOffset, labelBegin + labelSeparation);
            _previousLabel.rectTransform.SetParent(rectParent, false);

            var glyph = MakeGlyph(referenceText, 57);
            glyph.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            glyph.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            glyph.rectTransform.pivot = new Vector2(0f, 1f);
            glyph.rectTransform.anchoredPosition = new Vector2(labelLeft, labelBegin + labelSeparation);
            glyph.rectTransform.SetParent(rectParent, false);


            _nextLabel = MakeLabel(referenceText, "NextPlayer");
            _nextLabel.text = "Next Player";
            _nextLabel.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _nextLabel.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _nextLabel.rectTransform.pivot = new Vector2(0f, 1f);
            _nextLabel.rectTransform.anchoredPosition = new Vector2(labelLeft + glyphOffset, labelBegin + (labelSeparation * 2));
            _nextLabel.rectTransform.SetParent(rectParent, false);

            glyph = MakeGlyph(referenceText, 29);
            glyph.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            glyph.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            glyph.rectTransform.pivot = new Vector2(0f, 1f);
            glyph.rectTransform.anchoredPosition = new Vector2(labelLeft, labelBegin + (labelSeparation * 2));
            glyph.rectTransform.SetParent(rectParent, false);

            _backLabel = MakeLabel(referenceText, "BackLabel");
            _backLabel.text = "Back";
            _backLabel.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _backLabel.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _backLabel.rectTransform.pivot = new Vector2(0f, 1f);
            _backLabel.rectTransform.anchoredPosition = new Vector2(labelLeft + glyphOffset, labelBegin + (labelSeparation * 3));
            _backLabel.rectTransform.SetParent(rectParent, false);

            glyph = MakeGlyph(referenceText, 3);
            glyph.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            glyph.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            glyph.rectTransform.pivot = new Vector2(0f, 1f);
            glyph.rectTransform.anchoredPosition = new Vector2(labelLeft, labelBegin + (labelSeparation * 3));
            glyph.rectTransform.SetParent(rectParent, false);
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        internal static void DestroyUI()
        {
            if (Instance != null)
                GameObject.Destroy(Instance);
            Instance = null;
        }
    }
}