using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Plugin.Gamemodes;
using Reptile;
using TMPro;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class PropHuntUI : MonoBehaviour
    {
        internal static PropHuntUI Instance;

        private TextMeshProUGUI _aimLabel;
        private TextMeshProUGUI _aimGlyph;

        private TextMeshProUGUI _fireLabel;
        private TextMeshProUGUI _fireGlyph;

        private TextMeshProUGUI _turnLabel;

        private TextMeshProUGUI _freezeLabel;
        private TextMeshProUGUI _freezeGlyph;

        private TextMeshProUGUI _unfreezeLabel;

        private TextMeshProUGUI _lockedControlsLabel;

        private void Update()
        {
            _aimLabel.gameObject.SetActive(false);
            _aimGlyph.gameObject.SetActive(false);

            _fireLabel.gameObject.SetActive(false);
            _fireGlyph.gameObject.SetActive(false);

            _turnLabel.gameObject.SetActive(false);

            _freezeLabel.gameObject.SetActive(false);
            _freezeGlyph.gameObject.SetActive(false);

            _unfreezeLabel.gameObject.SetActive(false);

            _lockedControlsLabel.gameObject.SetActive(false);

            if (Core.Instance.BaseModule.IsInGamePaused) return;

            var player = WorldHandler.instance.GetCurrentPlayer();

            if (player.phone.state != Reptile.Phone.Phone.PhoneState.OFF && player.phone.state != Reptile.Phone.Phone.PhoneState.SHUTTINGDOWN)
                return;

            if (SpectatorUI.Instance != null && SpectatorUI.Instance.gameObject.activeSelf) return;

            var propHuntPlayer = PropHuntPlayer.GetLocal();

            if (propHuntPlayer == null) return;

            if (propHuntPlayer.LockedTimer > 0f)
            {
                _lockedControlsLabel.gameObject.SetActive(true);
                _lockedControlsLabel.text = $"Locked: {TimerUI.GetTimeString(propHuntPlayer.LockedTimer)}";
                return;
            }

            var team = PropDisguiseController.Instance.LocalPropHuntTeam;

            _aimLabel.gameObject.SetActive(true);
            _aimGlyph.gameObject.SetActive(true);

            if (propHuntPlayer.Aiming)
            {
                _fireGlyph.gameObject.SetActive(true);

                if (team == PropHuntTeams.Hunters)
                    _fireLabel.gameObject.SetActive(true);
                else
                    _turnLabel.gameObject.SetActive(true);
            }

            if (team == PropHuntTeams.Props)
            {
                _freezeGlyph.gameObject.SetActive(true);

                if (propHuntPlayer.Frozen)
                    _unfreezeLabel.gameObject.SetActive(true);
                else
                    _freezeLabel.gameObject.SetActive(true);
            }
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

        internal static void InitializeUI(GameplayUI gameplayUI)
        {
            if (Instance != null)
                return;
            Instance = new GameObject("Prop Hunt UI Root").AddComponent<PropHuntUI>();
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

            _aimLabel = MakeLabel(referenceText, "Aim");
            _aimLabel.text = "Aim";
            _aimLabel.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _aimLabel.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _aimLabel.rectTransform.pivot = new Vector2(0f, 1f);
            _aimLabel.rectTransform.anchoredPosition = new Vector2(labelLeft + glyphOffset, labelBegin + labelSeparation);
            _aimLabel.rectTransform.SetParent(rectParent, false);

            _aimGlyph = MakeGlyph(referenceText, 10);
            _aimGlyph.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _aimGlyph.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _aimGlyph.rectTransform.pivot = new Vector2(0f, 1f);
            _aimGlyph.rectTransform.anchoredPosition = new Vector2(labelLeft, labelBegin + labelSeparation);
            _aimGlyph.rectTransform.SetParent(rectParent, false);


            _fireLabel = MakeLabel(referenceText, "Fire");
            _fireLabel.text = "Fire";
            _fireLabel.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _fireLabel.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _fireLabel.rectTransform.pivot = new Vector2(0f, 1f);
            _fireLabel.rectTransform.anchoredPosition = new Vector2(labelLeft + glyphOffset, labelBegin + (labelSeparation * 2));
            _fireLabel.rectTransform.SetParent(rectParent, false);

            _fireGlyph = MakeGlyph(referenceText, 11);
            _fireGlyph.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _fireGlyph.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _fireGlyph.rectTransform.pivot = new Vector2(0f, 1f);
            _fireGlyph.rectTransform.anchoredPosition = new Vector2(labelLeft, labelBegin + (labelSeparation * 2));
            _fireGlyph.rectTransform.SetParent(rectParent, false);

            _turnLabel = MakeLabel(referenceText, "Turn");
            _turnLabel.text = "Turn into Prop";
            _turnLabel.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _turnLabel.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _turnLabel.rectTransform.pivot = new Vector2(0f, 1f);
            _turnLabel.rectTransform.anchoredPosition = new Vector2(labelLeft + glyphOffset, labelBegin + (labelSeparation * 2));
            _turnLabel.rectTransform.SetParent(rectParent, false);

            _freezeLabel = MakeLabel(referenceText, "Freeze");
            _freezeLabel.text = "Freeze";
            _freezeLabel.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _freezeLabel.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _freezeLabel.rectTransform.pivot = new Vector2(0f, 1f);
            _freezeLabel.rectTransform.anchoredPosition = new Vector2(labelLeft + glyphOffset, labelBegin + (labelSeparation * 3));
            _freezeLabel.rectTransform.SetParent(rectParent, false);

            _freezeGlyph = MakeGlyph(referenceText, 18);
            _freezeGlyph.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _freezeGlyph.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _freezeGlyph.rectTransform.pivot = new Vector2(0f, 1f);
            _freezeGlyph.rectTransform.anchoredPosition = new Vector2(labelLeft, labelBegin + (labelSeparation * 3));
            _freezeGlyph.rectTransform.SetParent(rectParent, false);

            _unfreezeLabel = MakeLabel(referenceText, "Unfreeze");
            _unfreezeLabel.text = "Unfreeze";
            _unfreezeLabel.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _unfreezeLabel.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _unfreezeLabel.rectTransform.pivot = new Vector2(0f, 1f);
            _unfreezeLabel.rectTransform.anchoredPosition = new Vector2(labelLeft + glyphOffset, labelBegin + (labelSeparation * 3));
            _unfreezeLabel.rectTransform.SetParent(rectParent, false);

            _lockedControlsLabel = MakeLabel(referenceText, "Locked");
            _lockedControlsLabel.text = "Locked";
            _lockedControlsLabel.rectTransform.anchorMin = new Vector2(0.0f, 0f);
            _lockedControlsLabel.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            _lockedControlsLabel.rectTransform.pivot = new Vector2(0f, 1f);
            _lockedControlsLabel.rectTransform.anchoredPosition = new Vector2(labelLeft + glyphOffset, labelBegin + (labelSeparation * 4));
            _lockedControlsLabel.rectTransform.SetParent(rectParent, false);
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