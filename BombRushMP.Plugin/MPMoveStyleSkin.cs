﻿using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public abstract class MPMoveStyleSkin : MPUnlockable
    {
        public string Title = "";
        public string HowToUnlock = string.Empty;
        public Texture Texture = null;
        public Material[] Materials = null;
        public abstract MoveStyle MoveStyle { get; }

        public MPMoveStyleSkin(string title, string howToUnlock, int id, bool unlockedByDefault, Texture texture, Material[] materials) : base(id, unlockedByDefault)
        {
            Title = title;
            Texture = texture;
            HowToUnlock = howToUnlock;
            Materials = materials;
        }

        public virtual void ApplyToPlayer(Player player)
        {
            var playerComp = PlayerComponent.Get(player);
            playerComp.MovestyleSkin = this;
            playerComp.ApplyMoveStyleSkin(0);
            if (Materials != null)
            {
                playerComp.ApplyMoveStyleMaterial(Materials);
            }
            else if (Texture != null)
            {
                switch (MoveStyle)
                {
                    case MoveStyle.INLINE:
                        {
                            var mats = MoveStyleLoader.GetMoveStyleMaterials(player, MoveStyle.INLINE);
                            foreach (var mat in mats)
                            {
                                mat.mainTexture = Texture;
                            }
                        }
                        break;
                    case MoveStyle.BMX:
                        {
                            var mats = MoveStyleLoader.GetMoveStyleMaterials(player, MoveStyle.BMX);
                            foreach (var mat in mats)
                            {
                                mat.mainTexture = Texture;
                            }
                        }
                        break;
                    case MoveStyle.SKATEBOARD:
                        {
                            var mats = MoveStyleLoader.GetMoveStyleMaterials(player, MoveStyle.SKATEBOARD);
                            foreach (var mat in mats)
                            {
                                mat.mainTexture = Texture;
                            }
                        }
                        break;
                }
            }
        }
    }
}
