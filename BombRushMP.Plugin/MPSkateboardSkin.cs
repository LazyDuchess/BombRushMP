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

        public MPSkateboardSkin(string title, string howToUnlock, int id, bool unlockedByDefault, Texture texture, Mesh mesh = null) : base(title, howToUnlock, id, unlockedByDefault, texture, null)
        {
            SkateboardMesh = mesh;
        }

        public MPSkateboardSkin(string title, string howToUnlock, int id, bool unlockedByDefault, Material material, Mesh mesh = null) : base(title, howToUnlock, id, unlockedByDefault, null, [material])
        {
            SkateboardMesh = mesh;
        }

        public MPSkateboardSkin(string title, string howToUnlock, int id, bool unlockedByDefault, Material[] materials, Mesh mesh = null) : base(title, howToUnlock, id, unlockedByDefault, null, materials)
        {
            SkateboardMesh = mesh;
        }

        public override void ApplyToPlayer(Player player)
        {
            base.ApplyToPlayer(player);
            var mesh = SkateboardMesh;
            if (mesh == null)
                mesh = player.MoveStylePropsPrefabs.skateboard.GetComponent<MeshFilter>().sharedMesh;
            player.characterVisual.moveStyleProps.skateboard.GetComponent<MeshFilter>().sharedMesh = mesh;
            var refController = ReflectionController.Instance;
            if (refController != null && refController.Anchor != null)
            {
                player.characterVisual.moveStyleProps.skateboard.GetComponent<MeshRenderer>().probeAnchor = refController.Anchor;
            }
        }
    }
}
