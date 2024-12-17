using Reptile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class ReflectionController : MonoBehaviour
    {
        private const int MediumQualityReflectionResolution = 32;
        private static GameObject Prefab;
        private ReflectionProbe _probeInstance;

        public static void Initialize()
        {
            Prefab = MPAssets.Instance.Bundle.LoadAsset<GameObject>("Camera Reflection Probe");
            StageManager.OnStagePostInitialization += StageManager_OnStagePostInitialization;
        }

        private static void StageManager_OnStagePostInitialization()
        {
            var refController = new GameObject("Reflection Controller");
            refController.AddComponent<ReflectionController>();
        }

        private void Awake()
        {
            _probeInstance = Instantiate(Prefab).GetComponent<ReflectionProbe>();
            var reflectionQuality = MPSettings.Instance.ReflectionQuality;

            if (reflectionQuality == ReflectionQualities.Low)
            {
                _probeInstance.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
                _probeInstance.RenderProbe();
            }

            if (reflectionQuality == ReflectionQualities.Medium)
            {
                _probeInstance.resolution = MediumQualityReflectionResolution;
                _probeInstance.timeSlicingMode = UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.IndividualFaces;
            }
        }

        private void Update()
        {
            _probeInstance.transform.position = WorldHandler.instance.CurrentCameraPosition;
        }

        private IEnumerator BakeReflections()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            
        }
    }
}
