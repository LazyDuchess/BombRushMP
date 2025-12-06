using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class XHairUI : MonoBehaviour
    {
        public static XHairUI Instance { get; private set; }
        public enum Modes
        {
            Off,
            On,
            Cross
        }
        public Modes CurrentMode = Modes.Off;
        public float HitMarkerTime = 0f;
        private GameObject _xhair;
        private GameObject _cross;
        private GameObject _hitMarker;

        private void Awake()
        {
            Instance = this;
            var canvas = transform.Find("Canvas");
            _xhair = canvas.Find("xhair").gameObject;
            _cross = canvas.Find("cross").gameObject;
            _hitMarker = canvas.Find("hitmarker").gameObject;
            UpdateSprites();
        }

        private void Update()
        {
            UpdateSprites();
            HitMarkerTime -= Time.deltaTime;
        }

        private void UpdateSprites()
        {
            if (HitMarkerTime > 0f)
                _hitMarker.SetActive(true);
            else
                _hitMarker.SetActive(false);

            _xhair.SetActive(false);
            _cross.SetActive(false);

            switch (CurrentMode)
            {
                case Modes.On:
                    _xhair.SetActive(true);
                    break;

                case Modes.Cross:
                    _cross.SetActive(true);
                    break;
            }
        }

        public static XHairUI Create()
        {
            var mpAssets = MPAssets.Instance;
            var prefab = mpAssets.Bundle.LoadAsset<GameObject>("XHair UI");
            var xHairUI = Instantiate(prefab);
            xHairUI.transform.SetParent(Core.Instance.UIManager.transform, false);
            return xHairUI.AddComponent<XHairUI>();
        }
    }
}
