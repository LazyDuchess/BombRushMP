using BombRushMP.Plugin.Patches;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public static class MPUtility
    {
        public static Player CreateMultiplayerPlayer()
        {
            var clientController = ClientController.Instance;
            var worldHandler = WorldHandler.instance;
            Player player = UnityEngine.Object.Instantiate(worldHandler.playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>();
            player.tf.SetAsFirstSibling();
            player.motor._rigidbody.isKinematic = true;
            worldHandler.RegisterPlayer(player);
            worldHandler.InitPlayer(player, Characters.metalHead, 0, PlayerType.NONE, MoveStyle.SKATEBOARD, Crew.PLAYERS);
            clientController.PlayerRegistry.Add(player);
            return player;
        }

        public static bool IsMultiplayerPlayer(Player player)
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return false;
            return clientController.PlayerRegistry.Contains(player);
        }

        public static void PlayAnimationOnMultiplayerPlayer(Player player, int newAnim, bool forceOverwrite = false, bool instant = false, float atTime = -1)
        {
            PlayerPatch.PlayAnimPatchEnabled = false;
            try
            {
                player.PlayAnim(newAnim, forceOverwrite, instant, atTime);
            }
            finally
            {
                PlayerPatch.PlayAnimPatchEnabled = true;
            }
        }
    }
}
