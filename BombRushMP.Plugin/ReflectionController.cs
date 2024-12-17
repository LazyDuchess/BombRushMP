using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class ReflectionController : MonoBehaviour
    {
        private static GameObject Prefab;
        private Transform _probeInstance;

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
            _probeInstance = Instantiate(Prefab).transform;
        }

        private void Update()
        {
            _probeInstance.transform.position = WorldHandler.instance.CurrentCameraPosition;
        }
    }
}
