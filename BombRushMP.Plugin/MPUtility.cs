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
        public static Player CreateMultiplayerPlayer(Characters character, int outfit, MPPlayer multiplayerPlayer)
        {
            var clientController = ClientController.Instance;
            var worldHandler = WorldHandler.instance;
            Player player = UnityEngine.Object.Instantiate(worldHandler.playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>();
            player.tf.SetAsFirstSibling();
            player.motor._rigidbody.isKinematic = true;
            player.motor._rigidbody.useGravity = false;
            worldHandler.RegisterPlayer(player);
            worldHandler.InitPlayer(player, character, outfit, PlayerType.NONE, MoveStyle.SKATEBOARD, Crew.PLAYERS);
            clientController.MultiplayerPlayerByPlayer[player] = multiplayerPlayer;
            return player;
        }

        public static bool IsMultiplayerPlayer(Player player)
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return false;
            return clientController.MultiplayerPlayerByPlayer.ContainsKey(player);
        }

        public static MPPlayer GetMuliplayerPlayer(Player player)
        {
            if (!IsMultiplayerPlayer(player)) return null;
            var clientController = ClientController.Instance;
            return clientController.MultiplayerPlayerByPlayer[player];
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
