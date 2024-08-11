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
    public static class UIUtility
    {
        private static bool _initialized = false;
        private static TextMeshProUGUI _referenceLabel;
        public static void Initialize()
        {
            _initialized = true;
            var uiManager = Core.Instance.UIManager;
            var labels = uiManager.danceAbilityUI.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var label in labels)
            {
                if (label.transform.gameObject.name == "DanceSelectConfirmText")
                {
                    _referenceLabel = label;
                    break;
                }
            }

        }

        public static void MakeGlyph(TextMeshProUGUI text, int actionID)
        {
            if (!_initialized)
                Initialize();
            text.gameObject.SetActive(false);
            text.font = _referenceLabel.font;
            text.fontMaterial = _referenceLabel.fontMaterial;
            var glyph = text.gameObject.AddComponent<UIButtonGlyphComponent>();
            glyph.inputActionID = actionID;
            glyph.localizedGlyphTextComponent = text;
            glyph.localizedTextComponent = text;
            glyph.enabled = true;
            text.gameObject.SetActive(true);
        }
    }
}
