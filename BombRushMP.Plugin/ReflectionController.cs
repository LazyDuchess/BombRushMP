using CommonAPI;
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
        public static ReflectionController Instance { get; private set; }
        public ReflectionProbe Probe { get; private set; }
        public Transform Anchor;
        private const int MediumQualityReflectionResolution = 32;
        private static Vector3 PlaceholderLocation = new Vector3(50000f, 50000f, 50000f);
        private static GameObject Prefab;

        public static void Initialize()
        {
            Prefab = MPAssets.Instance.Bundle.LoadAsset<GameObject>("Camera Reflection Probe");
            StageAPI.OnStagePreInitialization += OnStagePreInitialization;
        }

        private static void OnStagePreInitialization(Stage newStage, Stage previousStage)
        {
            var refController = new GameObject("Reflection Controller");
            refController.AddComponent<ReflectionController>();
        }

        private void Awake()
        {
            Instance = this;
            var reflectionQuality = MPSettings.Instance.ReflectionQuality;
            var anchorGo = new GameObject("Reflection Anchor");
            anchorGo.transform.SetPositionAndRotation(PlaceholderLocation, Quaternion.identity);
            Anchor = anchorGo.transform;

            if (reflectionQuality != ReflectionQualities.Off)
            {
                Probe = Instantiate(Prefab).GetComponent<ReflectionProbe>();

                if (reflectionQuality == ReflectionQualities.Low)
                {
                    Probe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
                    StartCoroutine(RenderLowReflection());
                }

                if (reflectionQuality == ReflectionQualities.Medium)
                {
                    Probe.resolution = MediumQualityReflectionResolution;
                    Probe.timeSlicingMode = UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.IndividualFaces;
                }
            }
        }

        private IEnumerator RenderLowReflection()
        {
            yield return new WaitForSecondsRealtime(1f);
            Probe.transform.position = WorldHandler.instance.CurrentCameraPosition;
            Probe.RenderProbe();
        }

        private void Update()
        {
            if (Probe == null) return;
            Probe.center = PlaceholderLocation;
            Probe.transform.position = WorldHandler.instance.CurrentCameraPosition;
            Anchor.transform.position = Probe.transform.position + Probe.center;
        }
    }
}
