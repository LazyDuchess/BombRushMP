using BombRushMP.Plugin;
using CommonAPI.Phone;
using Reptile;
using UnityEngine;

public class AppMultiplayerDebug : CustomApp
{
    public override bool Available => false;
    public static void Initialize()
    {
        PhoneAPI.RegisterApp<AppMultiplayerDebug>("multiplayer debug");
    }

    public override void OnAppInit()
    {
        base.OnAppInit();
        CreateIconlessTitleBar("Debug");
        ScrollView = PhoneScrollView.Create(this);
        PopulateButtons();
    }

    private void PopulateButtons()
    {
        ScrollView.RemoveAllButtons();

        var button = PhoneUIUtility.CreateSimpleButton("Join Lobby");
        button.OnConfirm += () =>
        {
            MyPhone.OpenApp(typeof(AppJoinLobbyDebug));
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Cop Skin Test");
        button.OnConfirm += () =>
        {
            var playerSkinPrefab = MPAssets.Instance.Bundle.LoadAsset<GameObject>("PlayerFemaleCopPrefab");
            var visuals = new GameObject("Player Female Cop Visuals");
            Instantiate(playerSkinPrefab);
            playerSkinPrefab.transform.SetParent(visuals.transform, false);
            
            var player = WorldHandler.instance.GetCurrentPlayer();

            var setChar = player.character;
            player.character = setChar;
            player.charTestCycleNum = (int)setChar;
            if (player.visualTf != null)
                Destroy(player.visualTf.gameObject);

            var visual = Instantiate(visuals).AddComponent<CharacterVisual>();
            visual.Init(player.character, player.animatorController, true, player.motor.groundDetection.groundLimit);
            visual.gameObject.SetActive(true);
            player.characterVisual = visual;
            player.characterMesh = player.characterVisual.mainRenderer.sharedMesh;
            player.characterVisual.transform.SetParent(player.transform.GetChild(0), false);
            player.characterVisual.transform.localPosition = Vector3.zero;
            player.characterVisual.transform.rotation = Quaternion.LookRotation(player.transform.forward);
            player.characterVisual.anim.gameObject.AddComponent<AnimationEventRelay>().Init();
            player.visualTf = player.characterVisual.transform;
            player.headTf = player.visualTf.FindRecursive("head");
            player.phoneDirBone = player.visualTf.FindRecursive("phoneDirection");
            player.heightToHead = (player.headTf.position - player.visualTf.position).y;
            player.isGirl = true;
            player.anim = player.characterVisual.anim;
            if (player.curAnim != 0)
            {
                var anim = player.curAnim;
                player.curAnim = 0;
                player.PlayAnim(anim, false, false, -1f);
            }
            player.characterVisual.InitVFX(player.VFXPrefabs);
            player.characterVisual.InitMoveStyleProps(player.MoveStylePropsPrefabs);
            player.characterConstructor.SetMoveStyleSkinsForCharacter(player, player.character);
            if (player.characterVisual.hasEffects)
            {
                player.boostpackTrail = player.characterVisual.VFX.boostpackTrail.GetComponent<TrailRenderer>();
                player.boostpackTrailDefaultWidth = player.boostpackTrail.startWidth;
                player.boostpackTrailDefaultTime = player.boostpackTrail.time;
                player.spraypaintParticles = player.characterVisual.VFX.spraypaint.GetComponent<ParticleSystem>();
                player.characterVisual.VFX.spraypaint.transform.localScale = Vector3.one * 0.5f;
                player.SetDustEmission(0);
                player.ringParticles = player.characterVisual.VFX.ring.GetComponent<ParticleSystem>();
                player.SetRingEmission(0);
            }
            player.SetMoveStyle(MoveStyle.ON_FOOT, true, true, null);
            player.InitVisual();
        };
        ScrollView.AddButton(button);
    }
}
