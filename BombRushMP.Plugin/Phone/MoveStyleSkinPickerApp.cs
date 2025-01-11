using BombRushMP.Common;
using CommonAPI.Phone;
using Mono.Cecil;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using static UnityEngine.UIElements.UIR.BestFitAllocator;

namespace BombRushMP.Plugin.Phone
{
    public class MoveStyleSkinPickerApp : CustomApp
    {
        public override bool Available => false;
        public static void Initialize()
        {
            PhoneAPI.RegisterApp<MoveStyleSkinPickerApp>("movestyle skin picker");
        }

        public override void OnAppInit()
        {
            base.OnAppInit();
            CreateIconlessTitleBar("Movestyle Skin");
            ScrollView = PhoneScrollView.Create(this);
        }

        public void PopulateButtons(MoveStyle moveStyle)
        {
            ScrollView.RemoveAllButtons();

            var styleSwitchMenu = Core.Instance.UIManager.styleSwitchUI;
            StyleSwitchMenu.SortSkinUnlockables(styleSwitchMenu.skateboardUnlockables);
            StyleSwitchMenu.SortSkinUnlockables(styleSwitchMenu.bmxUnlockables);
            StyleSwitchMenu.SortSkinUnlockables(styleSwitchMenu.rollerSkatesUnlockables);

            MoveStyleSkin[] unlockables = null;
            var moveStyleLocalization = string.Empty;

            switch (moveStyle)
            {
                case MoveStyle.INLINE:
                    unlockables = styleSwitchMenu.rollerSkatesUnlockables;
                    moveStyleLocalization = "U_MOVESTYLE_SKATES{0}";
                    break;
                case MoveStyle.SKATEBOARD:
                    unlockables = styleSwitchMenu.skateboardUnlockables;
                    moveStyleLocalization = "U_MOVESTYLE_DECK{0}";
                    break;
                case MoveStyle.BMX:
                    unlockables = styleSwitchMenu.bmxUnlockables;
                    moveStyleLocalization = "U_MOVESTYLE_BIKE{0}";
                    break;
            }

            for(var i = 0; i < unlockables.Length; i++)
            {
                var unlock = unlockables[i];
                var button = PhoneUIUtility.CreateSimpleButton(Core.Instance.Localizer.GetSkinText(string.Format(moveStyleLocalization, unlock.skinIndex + 1)));
                button.OnConfirm += () =>
                {
                    SetVanillaMoveStyleSkin(unlock.skinIndex);
                    MyPhone.CloseCurrentApp();
                };
                ScrollView.AddButton(button);
            }

            var mpUnlockManager = MPUnlockManager.Instance;

            var saveData = MPSaveData.Instance;

            foreach (var mpUnlockable in mpUnlockManager.UnlockByID)
            {
                var valid = false;
                if (mpUnlockable.Key == Animator.StringToHash(SpecialPlayerUtils.SpecialPlayerUnlockID) && !saveData.ShouldDisplayGoonieBoard()) continue;
                if (mpUnlockable.Value is not MPMoveStyleSkin) continue;
                if (mpUnlockable.Value is MPSkateboardSkin && moveStyle == MoveStyle.SKATEBOARD) valid = true;
                if (!valid) continue;
                var button = PhoneUIUtility.CreateSimpleButton((mpUnlockable.Value as MPMoveStyleSkin).Title);
                button.OnConfirm += () =>
                {
                    SetCustomMoveStyleSkin(mpUnlockable.Value as MPMoveStyleSkin);
                    MyPhone.CloseCurrentApp();
                };
                ScrollView.AddButton(button);
            }
        }

        private void SetCustomMoveStyleSkin(MPMoveStyleSkin skin)
        {
            var saveManager = Core.Instance.SaveManager;
            var player = WorldHandler.instance.GetCurrentPlayer();
            saveManager.CurrentSaveSlot.GetCharacterProgress(player.character).moveStyleSkin = 0;
            MPSaveData.Instance.GetCharacterData(player.character).MPMoveStyleSkin = skin.Identifier;
            skin.ApplyToPlayer(player);
            saveManager.SaveCurrentSaveSlot();
        }

        private void SetVanillaMoveStyleSkin(int skin)
        {
            var saveManager = Core.Instance.SaveManager;
            var player = WorldHandler.instance.GetCurrentPlayer();
            saveManager.CurrentSaveSlot.GetCharacterProgress(player.character).moveStyleSkin = skin;
            MPSaveData.Instance.GetCharacterData(player.character).MPMoveStyleSkin = -1;
            var playerComponent = PlayerComponent.Get(player);
            playerComponent.ApplyMoveStyleSkin(saveManager.CurrentSaveSlot.GetCharacterProgress(player.character).moveStyleSkin);
            saveManager.SaveCurrentSaveSlot();
        }
    }
}
