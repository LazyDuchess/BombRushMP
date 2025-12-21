using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if PLUGIN
using Reptile;
#endif

namespace BombRushMP.Mono.Runtime
{
    public class NPCVisibilityController : MonoBehaviour
    {
        public GameObject NPC;
        public int AssociatedCharacterIndex = -1;

#if PLUGIN
        private Characters AssociatedCharacter => (Characters)AssociatedCharacterIndex;
        private void Update()
        {
            var shouldBeVisible = true;
            var player = WorldHandler.instance.GetCurrentPlayer();
            if (player != null && player.character == AssociatedCharacter)
            {
                shouldBeVisible = false;
            }
            else if (WorldHandler.instance.currentEncounter != null)
            {
                shouldBeVisible = false;
            }

            if (NPC.activeSelf != shouldBeVisible)
            {
                NPC.SetActive(shouldBeVisible);
            }
        }
#endif
    }
}
