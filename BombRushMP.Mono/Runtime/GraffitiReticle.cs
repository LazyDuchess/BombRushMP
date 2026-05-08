using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BombRushMP.Mono.Runtime
{
    public class GraffitiReticle : MonoBehaviour
    {
        private void Awake()
        {
            var rawImage = GetComponent<RawImage>();
            var grafReticle = Theme.CurrentTheme.GraffitiReticle;
            if (grafReticle != null)
                rawImage.texture = grafReticle;
        }
    }
}
