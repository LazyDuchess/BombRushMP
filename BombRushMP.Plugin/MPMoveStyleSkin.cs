using Reptile;
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
        public abstract MoveStyle MoveStyle { get; }

        public MPMoveStyleSkin(string title, string howToUnlock, string id, bool unlockedByDefault, Texture texture) : base(id, unlockedByDefault)
        {
            Title = title;
            Texture = texture;
            HowToUnlock = howToUnlock;
        }

        public virtual void ApplyToPlayer(Player player)
        {
            switch (player.moveStyle)
            {
                case MoveStyle.INLINE:
                    player.characterVisual.moveStyleProps.skateL.GetComponent<Renderer>().sharedMaterial.mainTexture = Texture;
                    player.characterVisual.moveStyleProps.skateR.GetComponent<Renderer>().sharedMaterial.mainTexture = Texture;
                    break;
                case MoveStyle.BMX:
                    player.characterVisual.moveStyleProps.bmxPedalL.GetComponent<Renderer>().sharedMaterial.mainTexture = Texture;
                    player.characterVisual.moveStyleProps.bmxPedalR.GetComponent<Renderer>().sharedMaterial.mainTexture = Texture;
                    player.characterVisual.moveStyleProps.bmxFrame.GetComponent<Renderer>().sharedMaterial.mainTexture = Texture;
                    player.characterVisual.moveStyleProps.bmxHandlebars.GetComponent<Renderer>().sharedMaterial.mainTexture = Texture;
                    player.characterVisual.moveStyleProps.bmxWheelF.GetComponent<Renderer>().sharedMaterial.mainTexture = Texture;
                    player.characterVisual.moveStyleProps.bmxWheelR.GetComponent<Renderer>().sharedMaterial.mainTexture = Texture;
                    break;
                case MoveStyle.SKATEBOARD:
                    player.characterVisual.moveStyleProps.skateboard.GetComponent<Renderer>().sharedMaterial.mainTexture = Texture;
                    break;
            }
        }
    }
}
