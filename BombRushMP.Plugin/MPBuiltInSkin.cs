using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class MPBuiltInSkin : MPMoveStyleSkin
    {
        public const int InlineBuiltinId = 6969420;
        public const string InlineTransformName = "builtin_inlines";
        public override MoveStyle MoveStyle => _moveStyle;
        private MoveStyle _moveStyle;

        public MPBuiltInSkin(MoveStyle moveStyle, int id) : base("Signature", "Use the character's unique gear.", InlineBuiltinId, true, null, null)
        {
            _moveStyle = moveStyle;
        }

        public override void ApplyToPlayer(Player player)
        {
            base.ApplyToPlayer(player);
            player.characterVisual.moveStyleProps.skateL.GetComponent<MeshFilter>().sharedMesh = null;
            player.characterVisual.moveStyleProps.skateR.GetComponent<MeshFilter>().sharedMesh = null;
        }
    }
}
