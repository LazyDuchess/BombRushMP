using BombRushMP.Common;
using CommonAPI.Phone;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Phone
{
    public class MoveStylePickerApp : CustomApp
    {
        public override bool Available => false;
        public static void Initialize()
        {
            PhoneAPI.RegisterApp<MoveStylePickerApp>("movestyle");
        }

        public override void OnAppInit()
        {
            base.OnAppInit();
            CreateIconlessTitleBar("Movestyle");
            ScrollView = PhoneScrollView.Create(this);
            PopulateButtons();
        }

        private void PopulateButtons()
        {
            ScrollView.RemoveAllButtons();

            var button = PhoneUIUtility.CreateSimpleButton("Skateboard");
            button.OnConfirm += () =>
            {
                SetMoveStyle(MoveStyle.SKATEBOARD);
                MyPhone.CloseCurrentApp();
                MyPhone.OpenApp(typeof(MoveStyleSkinPickerApp));
                (MyPhone.m_CurrentApp as MoveStyleSkinPickerApp).PopulateButtons(MoveStyle.SKATEBOARD);
            };
            ScrollView.AddButton(button);

            button = PhoneUIUtility.CreateSimpleButton("Inline");
            button.OnConfirm += () =>
            {
                SetMoveStyle(MoveStyle.INLINE);
                MyPhone.CloseCurrentApp();
                MyPhone.OpenApp(typeof(MoveStyleSkinPickerApp));
                (MyPhone.m_CurrentApp as MoveStyleSkinPickerApp).PopulateButtons(MoveStyle.INLINE);
            };
            ScrollView.AddButton(button);

            button = PhoneUIUtility.CreateSimpleButton("BMX");
            button.OnConfirm += () =>
            {
                SetMoveStyle(MoveStyle.BMX);
                MyPhone.CloseCurrentApp();
                MyPhone.OpenApp(typeof(MoveStyleSkinPickerApp));
                (MyPhone.m_CurrentApp as MoveStyleSkinPickerApp).PopulateButtons(MoveStyle.BMX);
            };
            ScrollView.AddButton(button);
        }

        private void SetMoveStyle(MoveStyle moveStyle)
        {
            var saveManager = Core.Instance.SaveManager;
            var player = WorldHandler.instance.GetCurrentPlayer();
            saveManager.CurrentSaveSlot.GetCharacterProgress(player.character).moveStyle = moveStyle;
            MPSaveData.Instance.GetCharacterData(player.character).MPMoveStyleSkin = -1;
            player.SetCurrentMoveStyleEquipped(moveStyle, true, true);
            player.SwitchToEquippedMovestyle(true, false, true, true);
            var playerComponent = PlayerComponent.Get(player);
            playerComponent.ApplyMoveStyleSkin(saveManager.CurrentSaveSlot.GetCharacterProgress(player.character).moveStyleSkin);
            saveManager.SaveCurrentSaveSlot();
        }
    }
}
