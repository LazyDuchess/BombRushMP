using CommonAPI.Phone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Phone
{
    public class AppMultiplayerAcceptInvite : CustomApp
    {
        private static uint LobbyId = 0;
        private static Action OnAccept;
        private static Action OnDecline;
        public override bool Available => false;

        public static void Initialize()
        {
            PhoneAPI.RegisterApp<AppMultiplayerAcceptInvite>("accept invite");
        }

        public static void Show(uint lobbyId, Reptile.Phone.Phone phone, Action onAccept = null, Action onDecline = null)
        {
            LobbyId = lobbyId;
            OnAccept = onAccept;
            OnDecline = onDecline;
            phone.OpenApp(typeof(AppMultiplayerAcceptInvite));
        }

        public override void OnAppInit()
        {
            base.OnAppInit();
            CreateIconlessTitleBar("Accept Invite?", 70f);
            ScrollView = PhoneScrollView.Create(this);
        }

        public override void OnAppEnable()
        {
            base.OnAppEnable();
            PopulateButtons();
        }

        private void PopulateButtons()
        {
            ScrollView.RemoveAllButtons();

            var button = PhoneUIUtility.CreateSimpleButton("Yes");
            button.OnConfirm += () =>
            {
                Accept();
                MyPhone.CloseCurrentApp();
                OnAccept?.Invoke();
                OnAccept = null;
                OnDecline = null;
            };
            ScrollView.AddButton(button);

            button = PhoneUIUtility.CreateSimpleButton("No");
            button.OnConfirm += () =>
            {
                Decline();
                MyPhone.CloseCurrentApp();
                OnDecline?.Invoke();
                OnAccept = null;
                OnDecline = null;
            };
            ScrollView.AddButton(button);
        }

        private void Accept()
        {
            ClientController.Instance.ClientLobbyManager.JoinLobby(LobbyId);
            LobbyId = 0;
        }

        private void Decline()
        {
            ClientController.Instance.ClientLobbyManager.DeclineInvite(LobbyId);
            LobbyId = 0;
        }
    }
}
