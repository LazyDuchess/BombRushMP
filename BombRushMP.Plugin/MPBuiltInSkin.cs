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

        public const int BikeBuiltinId = 6767420;
        public const string BikeBuiltinTransformName = "builtin_bmx";

        public const int SkateBuiltinId = 5318008;
        public const string SkateTransformName = "builtin_skate";
        public override MoveStyle MoveStyle => _moveStyle;
        private MoveStyle _moveStyle;

        public MPBuiltInSkin(MoveStyle moveStyle, int id) : base("Signature", "Use the character's unique gear.", InlineBuiltinId, true, null, null)
        {
            _moveStyle = moveStyle;
        }

        public override void ApplyToPlayer(Player player)
        {
            base.ApplyToPlayer(player);

            switch (_moveStyle)
            {
                case MoveStyle.INLINE:
                    player.characterVisual.moveStyleProps.skateL.GetComponent<MeshFilter>().sharedMesh = null;
                    player.characterVisual.moveStyleProps.skateR.GetComponent<MeshFilter>().sharedMesh = null;
                    break;

                case MoveStyle.SKATEBOARD:
                    player.characterVisual.moveStyleProps.skateboard.GetComponent<MeshFilter>().sharedMesh = null;
                    break;

                case MoveStyle.BMX:
                    player.characterVisual.moveStyleProps.bmxFrame.GetComponent<MeshFilter>().sharedMesh = null;
                    player.characterVisual.moveStyleProps.bmxGear.GetComponent<MeshFilter>().sharedMesh = null;
                    player.characterVisual.moveStyleProps.bmxHandlebars.GetComponent<MeshFilter>().sharedMesh = null;
                    player.characterVisual.moveStyleProps.bmxPedalL.GetComponent<MeshFilter>().sharedMesh = null;
                    player.characterVisual.moveStyleProps.bmxPedalR.GetComponent<MeshFilter>().sharedMesh = null;
                    player.characterVisual.moveStyleProps.bmxWheelF.GetComponent<MeshFilter>().sharedMesh = null;
                    player.characterVisual.moveStyleProps.bmxWheelR.GetComponent<MeshFilter>().sharedMesh = null;
                    break;
            }
        }
    }
}
