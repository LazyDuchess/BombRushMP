using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common;
using BombRushMP.Common.Networking;
using BombRushMP.Common.Packets;
using BombRushMP.Plugin.Gamemodes;
using HarmonyLib;
using Reptile;
using UnityEngine;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal static class PlayerPatch
    {
        internal static bool PlayAnimPatchEnabled = true;

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.PlayAnim))]
        private static bool PlayAnim_Prefix(Player __instance, int newAnim, bool forceOverwrite, bool instant, float atTime)
        {
            if (!__instance.isAI && newAnim == __instance.idleFidget1Hash && PlayerComponent.Get(__instance).AFK) return false;
            var clientController = ClientController.Instance;
            if (clientController == null) return true;
            if (clientController.Connected && !__instance.isAI)
            {
                if (__instance.inGraffitiGame) return true;
                var packet = new PlayerAnimation(newAnim, forceOverwrite, instant, atTime);
                clientController.SendPacket(packet, PlayerAnimation.ClientSendMode, NetChannels.Animation);
            }
            if (!PlayAnimPatchEnabled) return true;
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.FixedUpdatePlayer))]
        private static bool FixedUpdatePlayer_Prefix(Player __instance)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.PlayVoice))]
        private static void PlayVoice_Prefix(Player __instance, AudioClipID audioClipID, VoicePriority voicePriority)
        {
            if (__instance.isAI) return;
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            var packet = new PlayerVoice();
            packet.AudioClipId = (int)audioClipID;
            packet.VoicePriority = (int)voicePriority;
            clientController.SendPacket(packet, IMessage.SendModes.ReliableUnordered, NetChannels.VisualUpdates);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.Jump))]
        private static void Jump_Prefix(Player __instance)
        {
            if (__instance.isAI) return;
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            var packet = new PlayerVoice();
            packet.AudioClipId = (int)AudioClipID.VoiceJump;
            packet.VoicePriority = (int)VoicePriority.MOVEMENT;
            clientController.SendPacket(packet, IMessage.SendModes.ReliableUnordered, NetChannels.VisualUpdates);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.OnTriggerStay))]
        private static bool OnTriggerStay_Prefix(Player __instance, Collider other)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            if (other.gameObject.layer == Layers.EnemyHitbox && __instance.ability != __instance.knockbackAbility)
            {
                if (!PvPUtils.CanIGetHit()) return true;
                var otherPlayer = other.gameObject.GetComponentInParent<Player>();
                if (otherPlayer == null) return true;
                if (PvPUtils.CanReptilePlayerPvP(otherPlayer))
                {
                    var canDamage = PvPUtils.CanIGetDamaged();
                    __instance.GetHit(canDamage ? 1 : 0, (otherPlayer.transform.position - __instance.transform.position).normalized, KnockbackAbility.KnockbackType.FAR);
                    MPSaveData.Instance.Stats.TimesHit++;
                    Core.Instance.SaveManager.SaveCurrentSaveSlot();
                    var mpPlayer = MPUtility.GetMuliplayerPlayer(otherPlayer);
                    ClientController.Instance.SendPacket(new ClientHitByPlayer(mpPlayer.ClientId), IMessage.SendModes.ReliableUnordered, NetChannels.Default);
                }
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.InitHitboxes))]
        private static void InitHitboxes_Postfix(Player __instance)
        {
            if (__instance.isAI)
                MPUtility.PlayerHitboxesToEnemy(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.CheckWallrun))]
        private static bool CheckWallrun_Prefix(Player __instance)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.CheckVert))]
        private static bool CheckVert_Prefix(Player __instance, ref bool __result)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance))
            {
                __result = false;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.StartHitpause))]
        private static bool StartHitpause_Prefix(Player __instance)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.SetOutfit))]
        private static bool SetOutfit_Prefix(Player __instance, int setOutfit)
        {
            var playerComponent = PlayerComponent.Get(__instance);
            if (playerComponent.SpecialSkin == SpecialSkins.None) return true;
            if (!__instance.isAI)
            {
                Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(__instance.character).outfit = setOutfit;
            }
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.SetOutfit))]
        private static void SetOutfit_Postfix(Player __instance)
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            if (__instance.isAI) return;
            clientController.SendClientState();
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.SetCharacter))]
        private static void SetCharacter_Postfix(Characters setChar, Player __instance)
        {
            if (!__instance.isAI)
            {
                var saveData = MPSaveData.Instance.GetCharacterData(setChar);
                if (saveData.MPMoveStyleSkin != -1)
                {
                    var mpUnlockManager = MPUnlockManager.Instance;
                    if (mpUnlockManager.UnlockByID.ContainsKey(saveData.MPMoveStyleSkin))
                    {
                        var skin = mpUnlockManager.UnlockByID[saveData.MPMoveStyleSkin] as MPMoveStyleSkin;
                        if (skin != null)
                            skin.ApplyToPlayer(__instance);
                    }
                }
            }
            var playerComponent = PlayerComponent.Get(__instance);
            playerComponent.Chibi = false;
            playerComponent.SpecialSkin = SpecialSkins.None;
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            if (__instance.isAI) return;
            clientController.SendClientState();
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.OrientVisualToSurface))]
        private static bool OrientVisualToSurface_Prefix(Player __instance)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.UpdateVisualRot))]
        private static bool UpdateVisualRot_Prefix(Player __instance)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.SetInputs))]
        private static void SetInputs_Prefix(Player __instance, ref UserInputHandler.InputBuffer inputBuffer)
        {
            if (__instance.isAI) return;
            var proSkater = ProSkaterPlayer.Get(__instance);
            if (proSkater == null) return;
            if (proSkater.IsOnManual())
                inputBuffer.moveAxisY = 0f;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.SetInputs))]
        private static void SetInputs_Postfix(Player __instance)
        {
            if (!__instance.isAI) return;
            var mpPlayer = MPUtility.GetMuliplayerPlayer(__instance);
            if (mpPlayer == null) return;
            __instance.sprayButtonHeld = mpPlayer.ClientVisualState.SprayCanHeld;
            if (!mpPlayer.ClientVisualState.SprayCanHeld && __instance.spraycanState != Player.SpraycanState.SPRAY)
                __instance.SetSpraycanState(Player.SpraycanState.NONE);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.SetSpraycanState))]
        private static void SetSpraycanState_Prefix(Player __instance, Player.SpraycanState state)
        {
            if (__instance.isAI) return;
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            if (state != Player.SpraycanState.SPRAY) return;
            clientController.SendGenericEvent(GenericEvents.Spray, IMessage.SendModes.ReliableUnordered);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.CompletelyStop))]
        private static void CompletelyStop_Postfix(Player __instance)
        {
            if (__instance.isAI) return;
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            clientController.SendGenericEvent(GenericEvents.Teleport, IMessage.SendModes.ReliableUnordered);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.OnTriggerEnter))]
        private static bool OnTriggerEnter_Prefix(Player __instance)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.LateUpdateAnimation))]
        private static bool LateUpdateAnimation_Prefix(Player __instance)
        {
            if (!MPUtility.IsMultiplayerPlayer(__instance)) return true;
            if (__instance.characterVisual.canBlink)
            {
                Player.UpdateBlinkAnimation(__instance.characterVisual.mainRenderer, __instance.characterMesh, ref __instance.blinkTimer, ref __instance.blink, ref __instance.blinkDuration, Core.dt);
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.StartGraffitiMode))]
        private static bool StartGraffitiMode_Prefix(Player __instance, GraffitiSpot graffitiSpot)
        {
            if (__instance.IsBusyWithSequence() || __instance.IsDead() || __instance.ability == __instance.danceAbility || __instance.isAI)
            {
                return true;
            }
            var clientController = ClientController.Instance;
            if (clientController == null) return true;
            var lobbyManager = clientController.ClientLobbyManager;
            if (lobbyManager == null) return true;
            var lobby = lobbyManager.CurrentLobby;
            if (lobby == null || lobby.CurrentGamemode == null || !lobby.InGame) return true;
            var grafRace = lobby.CurrentGamemode as GraffitiRace;
            if (grafRace == null) return true;
            if (!grafRace.QuickGraffitiEnabled) return true;
            if (grafRace.IsRaceGraffitiSpot(graffitiSpot))
            {
                grafRace.AddScore(graffitiSpot);
                grafRace.MarkGraffitiSpotDone(graffitiSpot);
            }
            Player.TrickType trickType = Player.TrickType.GRAFFITI_S;
            if (graffitiSpot.size == GraffitiSize.M)
            {
                trickType = Player.TrickType.GRAFFITI_M;
            }
            if (graffitiSpot.size == GraffitiSize.L)
            {
                trickType = Player.TrickType.GRAFFITI_L;
            }
            if (graffitiSpot.size == GraffitiSize.XL)
            {
                trickType = Player.TrickType.GRAFFITI_XL;
            }
            var art = TagUtils.GetRandomGraffitiArt(graffitiSpot, __instance);
            __instance.DoTrick(trickType, art.title, 0);
            graffitiSpot.Paint(Crew.PLAYERS, art, null);
            __instance.PlayVoice(AudioClipID.VoiceBoostTrick, VoicePriority.BOOST_TRICK, true);
            __instance.audioManager.PlaySfxGameplay(SfxCollectionID.GraffitiSfx, AudioClipID.graffitiComplete, 0f);
            __instance.RemoveGraffitiSlash();
            __instance.CreateGraffitiFinishEffect(graffitiSpot.transform, graffitiSpot.size);
            clientController.SendPacket(new PlayerGraffitiFinisher((byte)graffitiSpot.size), IMessage.SendModes.ReliableUnordered, NetChannels.VisualUpdates);
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.StartGraffitiMode))]
        private static void StartGraffitiMode_Postfix(Player __instance, GraffitiSpot graffitiSpot)
        {
            if (!__instance.inGraffitiGame) return;
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            clientController.CurrentGraffitiGame = GameObject.FindFirstObjectByType<GraffitiGame>();
            if (!clientController.Connected) return;
            clientController.SendGenericEvent(GenericEvents.Teleport, IMessage.SendModes.ReliableUnordered);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.EndGraffitiMode))]
        private static void EndGraffitiMode_Postfix(Player __instance, GraffitiSpot graffitiSpot)
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            clientController.CurrentGraffitiGame = null;
            if (!clientController.Connected) return;
            clientController.SendGenericEvent(GenericEvents.Teleport, IMessage.SendModes.ReliableUnordered);
            clientController.SendGenericEvent(GenericEvents.GraffitiGameOver, IMessage.SendModes.ReliableUnordered);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.UpdateBoostpack))]
        private static bool UpdateBoostpack_Prefix(Player __instance)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.LateUpdateUIDisplay))]
        private static bool LateUpdateUIDisplay_Prefix(Player __instance)
        {
            if (__instance.isAI) return true;
            if (ClientController.Instance == null) return true;
            var currentLobby = ClientController.Instance.ClientLobbyManager.CurrentLobby;
            if (currentLobby == null) return true;
            if (!currentLobby.InGame) return true;
            if (currentLobby.CurrentGamemode is GraffitiRace)
            {
                __instance.GrafSpotVisualizationStop();
                __instance.StoryObjectVisualizationStop();
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.OnLanded))]
        private static void OnLanded_Postfix(Player __instance)
        {
            if (__instance.isAI) return;
            ClientController.Instance.SendGenericEvent(GenericEvents.Land, IMessage.SendModes.ReliableUnordered);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.LandCombo))]
        private static void LandCombo_Postfix(Player __instance)
        {
            if (!__instance.IsComboing())
            {
                var proSkater = ProSkaterPlayer.Get(__instance);
                if (proSkater != null)
                    proSkater.OnEndCombo();
                if (WorldHandler.instance.currentEncounter != null && WorldHandler.instance.currentEncounter is ProxyEncounter)
                    __instance.ClearMultipliersDone();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.DropCombo))]
        private static bool DropCombo_Prefix(Player __instance)
        {
            if (!__instance.IsComboing()) return true;
            var proSkater = ProSkaterPlayer.Get(__instance);
            if (proSkater != null)
            {
                proSkater.OnEndCombo();
                __instance.baseScore = 0f;
                __instance.scoreMultiplier = 0f;
                __instance.lastScore = 0f;
                __instance.lastCornered = 0;
                __instance.ClearMultipliersDone();
                __instance.comboTimeOutTimer = 1f;
                __instance.currentTrickType = Player.TrickType.NONE;
                __instance.RefreshAirTricks();
                __instance.tricksInCombo = 0;
                __instance.RefreshAllDegrade();
                GameplayUI gameplayUI = __instance.ui;
                if (gameplayUI != null)
                {
                    gameplayUI.SetTrickingChargeBarActive(false);
                }
                WorldHandler.instance.AllowRedoGrafspots();
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.Awake))]
        private static void Awake_Prefix(Player __instance)
        {
            __instance.gameObject.AddComponent<PlayerComponent>();
        }
    }
}
