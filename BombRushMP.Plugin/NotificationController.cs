using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class NotificationController : MonoBehaviour
    {
        public static NotificationController Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            var notifUIGameObject = transform.Find("Canvas").Find("Notification").gameObject;
            notifUIGameObject.AddComponent<NotificationUI>();
        }

        public static void Create()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            var mpAssets = MPAssets.Instance;
            var prefab = mpAssets.Bundle.LoadAsset<GameObject>("Notification UI");
            var notifUI = Instantiate(prefab);
            notifUI.transform.SetParent(Core.Instance.UIManager.transform, false);
            notifUI.AddComponent<NotificationController>();
        }
    }
}
