using CommonAPI;
using CommonAPI.Phone;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace BombRushMP.Plugin.Phone
{
    public class PlayerPickerApp : CustomApp
    {
        public override bool Available => false;
        protected bool AllowEveryone = true;
        private Dictionary<ushort, PhoneButton> _phoneButtonByPlayerId = new();
        private bool _wasConnected = true;

        public override void OnAppInit()
        {
            base.OnAppInit();
            ScrollView = PhoneScrollView.Create(this);
        }

        public override void OnAppEnable()
        {
            base.OnAppEnable();
            UnSubToEvents();
            SubToEvents();
            _wasConnected = true;
            PopulateButtons();
        }

        public override void OnAppDisable()
        {
            base.OnAppDisable();
            UnSubToEvents();
        }

        public override void OnAppUpdate()
        {
            base.OnAppUpdate();
            var clientController = ClientController.Instance;
            if (clientController.Connected != _wasConnected)
            {
                _wasConnected = clientController.Connected;
                if (!_wasConnected)
                    OnDisconnected();
            }
        }

        private void OnPlayerDisconnected(ushort playerId)
        {
            if (_phoneButtonByPlayerId.TryGetValue(playerId, out var button))
            {
                ScrollView.RemoveButton(button);
            }
        }

        private void PopulateButtons()
        {
            _phoneButtonByPlayerId.Clear();
            ScrollView.RemoveAllButtons();
            var clientController = ClientController.Instance;
            SimplePhoneButton everyoneButton = null;
            if (AllowEveryone)
            {
                everyoneButton = PhoneUIUtility.CreateSimpleButton("Everyone");
                ScrollView.AddButton(everyoneButton);
            }

            var players = clientController.Players.OrderBy(x => x.Value.ClientState.Name).ToDictionary(x => x.Key, x => x.Value);

            foreach (var player in players)
            {
                if (player.Value.ClientId == clientController.LocalID) continue;
                if (!PlayerFilter(player.Value.ClientId)) continue;
                var button = PhoneUIUtility.CreateSimpleButton(player.Value.ClientState.Name);
                button.OnConfirm += () =>
                {
                    PlayerChosen([ player.Value.ClientId ]);
                };
                ScrollView.AddButton(button);
                _phoneButtonByPlayerId[player.Value.ClientId] = button;
            }

            if (AllowEveryone)
            {
                everyoneButton.OnConfirm += () =>
                {
                    PlayerChosen(_phoneButtonByPlayerId.Keys.ToArray());
                };
            }
        }

        public virtual void PlayerChosen(ushort[] playerIds)
        {
            MyPhone.CloseCurrentApp();
        }

        public virtual bool PlayerFilter(ushort playerId)
        {
            return true;
        }

        public virtual void OnDisconnected()
        {
            MyPhone.CloseCurrentApp();
        }

        private void SubToEvents()
        {
            ClientController.Instance.PlayerDisconnected += OnPlayerDisconnected;
        }
        
        private void UnSubToEvents()
        {
            ClientController.Instance.PlayerDisconnected -= OnPlayerDisconnected;
        }
    }
}
