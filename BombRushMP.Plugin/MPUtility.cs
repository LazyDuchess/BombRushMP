using BepInEx;
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
        public static string ParseMessageEmojis(string message)
        {
            var emojis = MPAssets.Instance.Emojis;
            foreach(var emoji in emojis.Sprites)
            {
                message = message.Replace(emoji.Key, $"<sprite={emoji.Value}>");
            }
            return message;
        }

        public static string GetPlayerDisplayName(string name)
        {
            name = TMPFilter.Sanitize(name);
            name = TMPFilter.FilterTags(name, MPSettings.Instance.ChatCriteria);
            name = TMPFilter.CloseAllTags(name);
            if (MPSettings.Instance.FilterProfanity)
            {
                if (ProfanityFilter.TMPContainsProfanity(name))
                    name = ProfanityFilter.CensoredName;
            }
            name = name.Trim();
            if (TMPFilter.RemoveAllTags(name).IsNullOrWhiteSpace())
                name = MPSettings.DefaultName;
            return name;
        }

        public static string GetPlayerDisplayName(ClientState clientState)
        {
            var name = clientState.Name;
            name = GetPlayerDisplayName(name);
            var user = clientState.User;
            foreach(var badge in user.Badges)
            {
                name = $"<sprite={badge}> {name}";
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

        public static void PlayerHitboxesToEnemy(Player player)
        {
            var allTransforms = player.GetComponentsInChildren<Transform>(true);
            foreach (var transform in allTransforms)
            {
                if (transform.gameObject.layer == Layers.PlayerHitbox)
                    transform.gameObject.layer = Layers.EnemyHitbox;
            }
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
            PlayerHitboxesToEnemy(player);
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

        public static void CloseMenusForGameStateUpdate()
        {
            CloseMenus();
            Core.Instance.UIManager.HidePauseMenuInstant();
        }

        public static void CloseMenus()
        {
            var spectatorController = SpectatorController.Instance;
            if (spectatorController != null)
                spectatorController.EndSpectating();
            var playerList = PlayerListUI.Instance;
            if (playerList != null)
                playerList.Displaying = false;
        }

        public static bool AnyMenusOpen()
        {
            var spectatorController = SpectatorController.Instance;
            if (spectatorController != null)
                return true;
            var playerList = PlayerListUI.Instance;
            if (playerList.Displaying)
                return true;
            return false;
        }
    }
}
