using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if PLUGIN
using Reptile;
#endif
using UnityEngine;

namespace BombRushMP.Mono.Runtime
{
    public class CutsceneCharacterMesh : MonoBehaviour
    {
        public int CharacterIndex = -1;
        public bool CanBlink = false;

#if PLUGIN
        private SkinnedMeshRenderer _renderer;
        private int _prevOutfit = -1;
        public Characters Character => (Characters)CharacterIndex;
        private float _blinkTimer = 0f;
        private float _blendShape = 0f;

        private void Start()
        {
            _renderer = GetComponent<SkinnedMeshRenderer>();
            UpdateSkin();
        }

        private void ResetBlinkTimer()
        {
            _blinkTimer = UnityEngine.Random.Range(2, 4);
        }

        private void Update()
        {
            UpdateSkin();
            if (CanBlink)
            {
                _blinkTimer -= Time.deltaTime;
                if (_blinkTimer <= 0f)
                {
                    StartCoroutine(Blink());
                }
            }
        }

        private IEnumerator Blink()
        {
            ResetBlinkTimer();
            CloseEyes();
            yield return new WaitForSeconds(0.1f);
            OpenEyes();
        }

        private void CloseEyes()
        {
            _blendShape = 100f;
        }

        private void OpenEyes()
        {
            _blendShape = 0f;
        }

        private void LateUpdate()
        {
            if (CanBlink)
                _renderer.SetBlendShapeWeight(0, _blendShape);
        }

        private void UpdateSkin()
        {
            if (CharacterIndex == -1) return;
            var outfit = Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(Character).outfit;
            if (outfit == _prevOutfit) return;
            _prevOutfit = outfit;
            var charConstructor = Core.Instance.BaseModule.StageManager.characterConstructor;
            var mat = charConstructor.CreateCharacterMaterial(Character, outfit);
            _renderer.sharedMaterial = mat;
        }
#endif
    }
}
