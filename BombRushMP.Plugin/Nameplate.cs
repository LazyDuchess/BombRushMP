using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class Nameplate : MonoBehaviour
    {
        public TextMeshPro Label;

        public static Nameplate Create()
        {
            var go = new GameObject("Nameplate Container");
            var namePlate = go.AddComponent<Nameplate>();
            namePlate.Label = go.AddComponent<TextMeshPro>();
            namePlate.Label.fontSize = 2f;
            namePlate.Label.sortingOrder = 1;
            namePlate.Label.horizontalAlignment = HorizontalAlignmentOptions.Center;
            namePlate.Label.verticalAlignment = VerticalAlignmentOptions.Middle;
            return namePlate;
        }

        void OnWillRenderObject()
        {
            transform.rotation = Camera.current.transform.rotation;
        }
    }
}
