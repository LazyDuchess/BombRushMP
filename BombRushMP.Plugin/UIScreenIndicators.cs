using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class UIScreenIndicators : MonoBehaviour
    {
        private float _minDistance = 5f;
        private float _maxDistance = 20f;
        private float _distanceSizeMultiplier = 0.5f;
        private Vector2 _originalSize;
        private UIManager _uiManager;
        private GameObject _indicator;
        private Transform _canvasTf;
        private Dictionary<Transform, RectTransform> _indicators = new();
        private bool _visible = true;

        private void Awake()
        {
            _uiManager = Core.Instance.UIManager;
            _canvasTf = transform.Find("Canvas");
            _indicator = _canvasTf.Find("Indicator").gameObject;
            _originalSize = _indicator.RectTransform().sizeDelta;
            _indicator.SetActive(false);
        }

        private void LateUpdate()
        {
            if (!_uiManager.gameplay.gameplayScreen.gameObject.activeInHierarchy)
            {
                _canvasTf.gameObject.SetActive(false);
                return;
            }
            _canvasTf.gameObject.SetActive(true);
            var cam = GameplayCamera.instance.cam;
            foreach(var indicator in _indicators)
            {
                var tf = indicator.Key;
                var rect = indicator.Value;
                if (!IsInFront(cam.transform.position, cam.transform.forward, indicator.Key.position))
                {
                    rect.gameObject.SetActive(false);
                    continue;
                }
                rect.gameObject.SetActive(true);
                var screenPos = cam.WorldToViewportPoint(tf.position);
                rect.anchorMax = screenPos;
                rect.anchorMin = screenPos;
                var dist = Vector3.Distance(cam.transform.position, tf.position);
                dist -= _minDistance;
                dist /= _maxDistance;
                dist = Mathf.Clamp(dist, 0f, 1f);
                rect.sizeDelta = Vector2.Lerp(_originalSize, _originalSize * _distanceSizeMultiplier, dist);
            }
        }

        private bool IsInFront(Vector3 cameraPos, Vector3 cameraForward, Vector3 targetPos)
        {
            var diff = targetPos - cameraPos;
            return Vector3.Dot(cameraForward, diff) > 0;
        }

        public void AddIndicator(Transform target)
        {
            var indicator = Instantiate(_indicator.gameObject);
            indicator.transform.SetParent(_canvasTf, false);
            indicator.gameObject.SetActive(true);
            _indicators[target] = indicator.RectTransform();
        }

        public void RemoveIndicator(Transform target)
        {
            if (_indicators.TryGetValue(target, out var result))
            {
                _indicators.Remove(target);
                Destroy(result.gameObject);
            }
        }

        public static UIScreenIndicators Create()
        {
            var mpAssets = MPAssets.Instance;
            var prefab = mpAssets.Bundle.LoadAsset<GameObject>("Screen Indicators UI");
            var indicatorUI = Instantiate(prefab);
            indicatorUI.transform.SetParent(Core.Instance.UIManager.transform, false);
            return indicatorUI.AddComponent<UIScreenIndicators>();
        }
    }
}
