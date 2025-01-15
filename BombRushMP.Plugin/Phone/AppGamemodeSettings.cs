using BombRushMP.Common;
using BombRushMP.Plugin.Gamemodes;
using CommonAPI.Phone;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Phone
{
    public class AppGamemodeSettings : CustomApp
    {
        public override bool Available => false;
        private GamemodeIDs _currentGamemode = (GamemodeIDs)(-1);
        private Dictionary<int, SimplePhoneButton> _buttonBySettingID = new();

        public static void Initialize()
        {
            PhoneAPI.RegisterApp<AppGamemodeSettings>("gamemode settings");
        }

        public override void OnAppInit()
        {
            base.OnAppInit();
            CreateIconlessTitleBar("Gamemode Settings");
            ScrollView = PhoneScrollView.Create(this);
        }

        public override void OnAppEnable()
        {
            base.OnAppEnable();
            UpdateGamemode();
        }

        public override void OnAppUpdate()
        {
            base.OnAppUpdate();
            var currentGamemode = GetCurrentGamemode();
            if (currentGamemode != _currentGamemode)
                UpdateGamemode();
            UpdateLabels();
        }

        private void UpdateGamemode()
        {
            _currentGamemode = GetCurrentGamemode();
            _buttonBySettingID.Clear();
            ScrollView.RemoveAllButtons();
            var currentSettings = GetCurrentSettings();
            if (currentSettings == null) {
                _currentGamemode = (GamemodeIDs)(-1);
            }
            
            var resetButton = PhoneUIUtility.CreateSimpleButton("Reset");
            resetButton.OnConfirm += () =>
            {
                var defaults = GetDefaultSettings();
                MPSaveData.Instance.GamemodeSettings[_currentGamemode] = defaults.ToSaved();
                Core.Instance.SaveManager.SaveCurrentSaveSlot();
                SendSettings(defaults);
            };
            ScrollView.AddButton(resetButton);
            foreach(var setting in currentSettings.SettingByID)
            {
                var button = PhoneUIUtility.CreateSimpleButton(setting.Value.ToString());
                _buttonBySettingID[setting.Key] = button;
                button.OnConfirm += () =>
                {
                    currentSettings.SettingByID[setting.Key].Next();
                    MPSaveData.Instance.GamemodeSettings[_currentGamemode] = currentSettings.ToSaved();
                    Core.Instance.SaveManager.SaveCurrentSaveSlot();
                    SendSettings(currentSettings);
                    UpdateLabels();
                };
                ScrollView.AddButton(button);
            }
        }

        private void UpdateLabels()
        {
            var currentSettings = GetCurrentSettings();
            foreach (var setting in currentSettings.SettingByID)
            {
                if (_buttonBySettingID.TryGetValue(setting.Key, out var result))
                {
                    result.Label.text = setting.Value.ToString();
                }
            }
        }

        private GamemodeIDs GetCurrentGamemode()
        {
            var currentLobby = ClientController.Instance.ClientLobbyManager.CurrentLobby;
            if (currentLobby == null) return (GamemodeIDs)(-1);
            return currentLobby.LobbyState.Gamemode;
        }

        private GamemodeSettings GetCurrentSettings()
        {
            var currentLobby = ClientController.Instance.ClientLobbyManager.CurrentLobby;
            if (currentLobby == null) return null;
            var currentGamemode = GetCurrentGamemode();
            return GamemodeFactory.ParseGamemodeSettings(currentGamemode, currentLobby.LobbyState.GamemodeSettings);
        }

        private GamemodeSettings GetDefaultSettings()
        {
            var currentLobby = ClientController.Instance.ClientLobbyManager.CurrentLobby;
            if (currentLobby == null) return null;
            var currentGamemode = GetCurrentGamemode();
            return GamemodeFactory.GetGamemodeSettings(currentGamemode);
        }

        private void SendSettings(GamemodeSettings settings)
        {
            ClientController.Instance.ClientLobbyManager.SetGamemode(GetCurrentGamemode(), settings);
        }
    }
}
