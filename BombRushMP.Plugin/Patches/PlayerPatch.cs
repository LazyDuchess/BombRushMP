using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common;
using BombRushMP.Common.Packets;
using BombRushMP.Plugin.Gamemodes;
using HarmonyLib;
using Reptile;
using Riptide;
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
            var clientController = ClientController.Instance;
            if (clientController == null) return true;
            if (clientController.Connected && !__instance.isAI)
            {
                if (__instance.inGraffitiGame) return true;
                var packet = new PlayerAnimation(newAnim, forceOverwrite, instant, atTime);
                // Doesn't really need to be reliable no?
                clientController.SendPacket(packet, MessageSendMode.Reliable);
            }
            if (!PlayAnimPatchEnabled) return true;
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.FixedUpdateAbilities))]
        private static bool FixedUpdateAbilities_Prefix(Player __instance)
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
            clientController.SendPacket(packet, MessageSendMode.Reliable);
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
            clientController.SendPacket(packet, MessageSendMode.Reliable);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.OnTriggerStay))]
        private static bool OnTriggerStay_Prefix(Player __instance)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
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
        private static void SetCharacter_Postfix(Player __instance)
        {
            var playerComponent = PlayerComponent.Get(__instance);
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
            clientController.SendGenericEvent(GenericEvents.Spray, MessageSendMode.Reliable);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.CompletelyStop))]
        private static void CompletelyStop_Postfix(Player __instance)
        {
            if (__instance.isAI) return;
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            clientController.SendGenericEvent(GenericEvents.Teleport, MessageSendMode.Reliable);
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

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.StartGraffitiMode))]
        private static void StartGraffitiMode_Postfix(Player __instance, GraffitiSpot graffitiSpot)
        {
            if (!__instance.inGraffitiGame) return;
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            clientController.CurrentGraffitiGame = GameObject.FindFirstObjectByType<GraffitiGame>();
            if (!clientController.Connected) return;
            clientController.SendGenericEvent(GenericEvents.Teleport, MessageSendMode.Reliable);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.EndGraffitiMode))]
        private static void EndGraffitiMode_Postfix(Player __instance, GraffitiSpot graffitiSpot)
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            clientController.CurrentGraffitiGame = null;
            if (!clientController.Connected) return;
            clientController.SendGenericEvent(GenericEvents.Teleport, MessageSendMode.Reliable);
            clientController.SendGenericEvent(GenericEvents.GraffitiGameOver, MessageSendMode.Reliable);
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

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.LandCombo))]
        private static void LandCombo_Prefix(Player __instance)
        {
            if (!__instance.IsComboing()) return;
            var proSkater = ProSkaterPlayer.Get(__instance);
            if (proSkater != null)
                proSkater.OnEndCombo();
            if (WorldHandler.instance.currentEncounter != null && WorldHandler.instance.currentEncounter is ProxyEncounter)
                __instance.ClearMultipliersDone();
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
