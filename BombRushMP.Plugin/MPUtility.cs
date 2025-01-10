using BombRushMP.Common;
using BombRushMP.Common.Packets;
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
        public static string GetPlayerDisplayName(string name)
        {
            if (name.Length >= MPSettings.MaxNameLength)
                name = name.Substring(0, MPSettings.MaxNameLength);
            name = TMPFilter.CloseAllTags(name);
            if (MPSettings.Instance.FilterProfanity)
            {
                if (ProfanityFilter.TMPContainsProfanity(name))
                    name = ProfanityFilter.CensoredName;
            }
            return name;
        }

        public static string GetPlayerDisplayName(ClientState clientState)
        {
            var name = clientState.Name;
            //name = GetPlayerDisplayName(name);
            var user = clientState.User;
            if (user.Badge != -1)
            {
                name = $"<sprite={user.Badge}> {name}";
            }
            return name;
        }

        public static void PlaceCurrentPlayer(Vector3 position, Quaternion rotation)
        {
            PlacePlayer(WorldHandler.instance.GetCurrentPlayer(), position, rotation);
        }

        public static void PlacePlayer(Player player, Vector3 position, Quaternion rotation)
        {
            player.CompletelyStop();
            WorldHandler.instance.PlacePlayerAt(player, position, rotation, true);
            player.CompletelyStop();
            player.SetPosAndRotHard(position, rotation);
        }
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
            Core.OnCoreUpdatePaused -= player.OnCoreUpdatePaused;
            Core.OnCoreUpdateUnPaused -= player.OnCoreUpdateUnPaused;
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
