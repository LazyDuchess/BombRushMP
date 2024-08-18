using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BombRushMP.Common;

namespace BombRushMP.Plugin
{
    public class PlayerComponent : MonoBehaviour
    {
        public SpecialSkins SpecialSkin = SpecialSkins.None;
        public int SpecialSkinVariant = -1;
        public SkinnedMeshRenderer MainRenderer = null;
        public static PlayerComponent Get(Player player)
        {
            return player.GetComponent<PlayerComponent>();
        }
    }
}
