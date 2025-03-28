using BepInEx;
using BombRushMP.Common;
using BombRushMP.Common.Packets;
using BombRushMP.Plugin.Patches;
using Reptile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public static string GetTeamName(LobbyState lobbyState, Team team, byte teamId)
        {
            var teamPlayers = lobbyState.Players.Where((x) => x.Value.Team == teamId);
            var setCrewName = false;
            var lastCrewName = "";
            foreach(var teamPlayer in teamPlayers)
            {
                var crewName = ClientController.Instance.Players[teamPlayer.Key].ClientState.CrewName;
                if (!setCrewName)
                {
                    setCrewName = true;
                    lastCrewName = crewName;
                }
                else
                {
                    if (crewName != lastCrewName)
                        return "";
                }
            }
            return lastCrewName;
        }

        public static string GetCrewDisplayName(string name)
        {
            name = TMPFilter.Sanitize(name);
            name = TMPFilter.FilterTags(name, MPSettings.Instance.ChatCriteria);
            name = TMPFilter.CloseAllTags(name);
            if (ProfanityFilter.TMPContainsProfanity(name))
            {
                if (MPSettings.Instance.FilterProfanity)
                    name = "";
                else
                    name += ProfanityFilter.FilteredIndicator;
            }
            name = name.Trim();
            if (TMPFilter.RemoveAllTags(name).IsNullOrWhiteSpace())
                name = "";
            return name;
        }

        public static string GetPlayerDisplayName(string name)
        {
            name = TMPFilter.Sanitize(name);
            name = TMPFilter.FilterTags(name, MPSettings.Instance.ChatCriteria);
            name = TMPFilter.CloseAllTags(name);
            if (ProfanityFilter.TMPContainsProfanity(name))
            {
                if (MPSettings.Instance.FilterProfanity)
                    name = ProfanityFilter.CensoredName;
                else
                    name += ProfanityFilter.FilteredIndicator;
            }
            name = name.Trim();
            if (TMPFilter.RemoveAllTags(name).IsNullOrWhiteSpace())
                name = MPSettings.DefaultName;
            return name;
        }

        public static string GetPlayerDisplayNameWithoutTags(ClientState clientState)
        {
            var name = clientState.Name;
            name = TMPFilter.RemoveAllTags(GetPlayerDisplayName(name));
            var user = clientState.User;
            if (AprilClient.GetAprilEventEnabled())
            {
                name = $"<sprite={AprilClient.GetBadgeForName(clientState.Name)}> {name}";
            }
            foreach (var badge in user.Badges)
            {
                name = $"<sprite={badge}> {name}";
            }
            return name;
        }

        public static string GetPlayerDisplayName(ClientState clientState)
        {
            var name = clientState.Name;
            name = GetPlayerDisplayName(name);
            var user = clientState.User;
            if (AprilClient.GetAprilEventEnabled())
            {
                name = $"<sprite={AprilClient.GetBadgeForName(clientState.Name)}> {name}";
            }
            foreach (var badge in user.Badges)
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
            if (!MPSettings.Instance.PlayerDopplerEnabled)
            {
                var audios = player.GetComponentsInChildren<AudioSource>(true);
                foreach(var audio in audios)
                {
                    audio.dopplerLevel = 0f;
                }
            }
            PlayerComponent.Get(player).SkinLoaded += multiplayerPlayer.OnSkinLoaded;
            return player;
        }

        public static string RemoveCrewTag(string text)
        {
            var rich = new Regex(@"\[[^\]]*\]");
            text = rich.Replace(text, string.Empty);
            return text;
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

        public static void SetUpPlayerForGameStateUpdate()
        {
            CloseMenusAndSpectator();
            var uiManager = Core.Instance.UIManager;
            if (uiManager.menuNavigationController.IsMenuPartOfMenuStack(uiManager.pauseMenu))
            {
                uiManager.pauseMenu.ResumeCurrentGame();
            }
            Core.Instance.UIManager.PopAllMenusInstant();
            var grafGame = GameObject.FindObjectOfType<GraffitiGame>();
            if (grafGame != null)
                grafGame.CancelGame();
            var player = WorldHandler.instance.GetCurrentPlayer();
            if (player.IsDead())
                Revive();
            var tps = GameObject.FindObjectsOfType<Teleport>();
            foreach(var tp in tps)
            {
                if (tp.teleportRoutine != null)
                {
                    tp.StopAllCoroutines();
                    tp.teleportRoutine = null;
                }
            }
            Core.Instance.UIManager.effects.fullScreenFade.gameObject.SetActive(false);
        }

        public static void CloseMenusAndSpectator()
        {
            var spectatorController = SpectatorController.Instance;
            if (spectatorController != null)
                spectatorController.EndSpectating();
            var playerList = PlayerListUI.Instance;
            if (playerList != null)
                playerList.Displaying = false;
            var statsUi = StatsUI.Instance;
            if (statsUi != null && statsUi.Displaying)
                statsUi.Deactivate();
        }

        public static bool AnyMenusOpen()
        {
            var playerList = PlayerListUI.Instance;
            if (playerList.Displaying)
                return true;
            var statsUi = StatsUI.Instance;
            if (statsUi.Displaying)
                return true;
            return false;
        }

        public static void Revive()
        {
            var dieMenu = Core.instance.UIManager.dieMenu;
            var player = WorldHandler.instance.GetCurrentPlayer();
            player.Revive();
            player.cam.cam.clearFlags = CameraClearFlags.Skybox;
            player.cam.cam.cullingMask = WorldHandler.GetGameplayCameraCullingMask();
            player.CompletelyStop();
            dieMenu.baseModule.UnPauseGame(PauseType.GameOver);
            dieMenu.StopAllCoroutines();
            dieMenu.uIManager.PopAllMenusInstant();
            player.ui.TurnOn(true);
            var currentStageProgress = Core.Instance.SaveManager.CurrentSaveSlot.GetCurrentStageProgress();
            PlaceCurrentPlayer(currentStageProgress.respawnPos, Quaternion.Euler(currentStageProgress.respawnRot));
        }

        public static PublicToilet GetCurrentToilet()
        {
            var sequenceHandler = SequenceHandler.instance;

            if (sequenceHandler.IsInSequence())
            {
                var sequence = sequenceHandler.sequence;
                if (sequence != null && sequence.state == UnityEngine.Playables.PlayState.Playing)
                {
                    var toilet = sequence.GetComponentInParent<PublicToilet>(true);
                    if (toilet != null)
                    {
                        return toilet;
                    }
                }
            }

            return null;
        }
    }
}
