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
    public class DebugUI : MonoBehaviour
    {
        internal static DebugUI Instance;
        private TextMeshProUGUI _curAnimLabel;

        private void Awake()
        {
            Instance = this;
            Init();
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

        static TextMeshProUGUI MakeGlyph(TextMeshProUGUI reference, int actionID)
        {
            var label = MakeLabel(reference, "");
            label.gameObject.SetActive(false);
            var glyph = label.gameObject.AddComponent<UIButtonGlyphComponent>();
            glyph.inputActionID = actionID;
            glyph.localizedGlyphTextComponent = label;
            glyph.localizedTextComponent = label;
            glyph.enabled = true;
            label.gameObject.SetActive(true);
            return label;
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

            _curAnimLabel = MakeLabel(referenceText, "CurAnim");
            _curAnimLabel.text = "CurAnim";
            _curAnimLabel.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _curAnimLabel.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _curAnimLabel.rectTransform.pivot = new Vector2(0f, 1f);
            _curAnimLabel.rectTransform.anchoredPosition = new Vector2(labelLeft + glyphOffset, labelBegin + labelSeparation);
            _curAnimLabel.rectTransform.SetParent(rectParent, false);
        }

        private void Update()
        {
            if (_curAnimLabel == null) return;
            var worldHandler = WorldHandler.instance;
            if (worldHandler == null) return;
            var player = worldHandler.GetCurrentPlayer();
            if (player == null) return;
            _curAnimLabel.text = $"CurAnim: {player.curAnim}\n";
            _curAnimLabel.text += $"Players: {RenderStats.Players}\n";
            _curAnimLabel.text += $"Players Rendered: {RenderStats.PlayersRendered}\n";
            _curAnimLabel.text += $"Players Culled: {RenderStats.PlayersCulled}\n";
        }

        public static void Create()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            var go = new GameObject("Debug UI");
            go.transform.SetParent(Core.Instance.UIManager.gameplay.transform.parent.GetComponent<RectTransform>(), false);
            go.AddComponent<DebugUI>();
        }
    }
}