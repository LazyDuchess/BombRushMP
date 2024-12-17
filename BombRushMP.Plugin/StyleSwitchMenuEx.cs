using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Reptile.BasicCop;
using UnityEngine.Playables;

namespace BombRushMP.Plugin
{
    public class StyleSwitchMenuEx : MonoBehaviour
    {
        public Material[] OriginalMaterials = null;

        public static StyleSwitchMenuEx Get(StyleSwitchMenu menu)
        {
            var retValue = menu.GetComponent<StyleSwitchMenuEx>();
            if (retValue == null)
                retValue = menu.gameObject.AddComponent<StyleSwitchMenuEx>();
            return retValue;
        }
    }
}
