using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class MPSkateboardSkin : MPMoveStyleSkin
    {
        public Mesh SkateboardMesh = null;
        public override MoveStyle MoveStyle => MoveStyle.SKATEBOARD;

        public MPSkateboardSkin(string title, string howToUnlock, string id, bool unlockedByDefault, Texture texture, Mesh mesh = null) : base(title, howToUnlock, id, unlockedByDefault, texture)
        {
            SkateboardMesh = mesh;
        }

        public override void ApplyToPlayer(Player player)
        {
            base.ApplyToPlayer(player);
            if (player.moveStyle != MoveStyle.SKATEBOARD) return;
            var mesh = SkateboardMesh;
            if (mesh == null)
                mesh = player.MoveStylePropsPrefabs.skateboard.GetComponent<MeshFilter>().sharedMesh;
            player.characterVisual.moveStyleProps.skateboard.GetComponent<MeshFilter>().sharedMesh = mesh;
        }
    }
}
