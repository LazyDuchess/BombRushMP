using BombRushMP.Common;
using BombRushMP.Common.Packets;
using BombRushMP.MapStation;
using BombRushMP.Plugin;
using BombRushMP.Plugin.Gamemodes;
using BombRushMP.Plugin.Phone;
using CommonAPI;
using CommonAPI.Phone;
using System.IO;
using UnityEngine;

public class AppMultiplayer : CustomApp
{
    private PhoneButton _createLobbyButton;
    private PhoneButton _lobbyButton;
    private bool _waitingForLobbyCreateResponse = false;
    public static void Initialize()
    {
        var txtFile = File.ReadAllBytes(Path.Combine(MPSettings.Instance.Directory, "acn_icon.png"));
        var texture = new Texture2D(1, 1);
        texture.LoadImage(txtFile);
        texture.wrapMode = TextureWrapMode.Clamp;
        var icon = TextureUtility.CreateSpriteFromTexture(texture);
        PhoneAPI.RegisterApp<AppMultiplayer>("multiplayer", icon);
    }

    public override void OnAppInit()
    {
        base.OnAppInit();
        CreateIconlessTitleBar("Multiplayer", 70f);
        ScrollView = PhoneScrollView.Create(this);
        PopulateButtons();
    }

    public override void OnAppUpdate()
    {
        base.OnAppUpdate();
        var clientController = ClientController.Instance;
        if (!clientController.Connected)
        {
            _waitingForLobbyCreateResponse = false;
            ClientController.PacketReceived -= OnPacketReceived_LobbyCreation;
        }
        var lobbyManager = clientController.ClientLobbyManager;
        if (lobbyManager.CurrentLobby != null)
        {
            if (_lobbyButton == null)
                MakeLobbyButton();
            if (_createLobbyButton != null)
            {
                ScrollView.RemoveButton(_createLobbyButton);
                _createLobbyButton = null;
            }
        }
        else
        {
            if (_createLobbyButton == null)
                MakeCreateLobbyButton();
            if (_lobbyButton != null)
            {
                ScrollView.RemoveButton(_lobbyButton);
                _lobbyButton = null;
            }
        }
    }

    private void MakeCreateLobbyButton()
    {
        var lobbyManager = ClientController.Instance.ClientLobbyManager;
        _createLobbyButton = PhoneUIUtility.CreateSimpleButton("Create Lobby");
        _createLobbyButton.OnConfirm += () =>
        {
            if (!_waitingForLobbyCreateResponse)
            {
                var scoreBattleSettings = GamemodeFactory.GetGamemodeSettings(GamemodeIDs.ScoreBattle);
                var savedScoreBattleSettings = MPSaveData.Instance.GetSavedSettings(GamemodeIDs.ScoreBattle);
                if (savedScoreBattleSettings != null)
                    scoreBattleSettings.ApplySaved(savedScoreBattleSettings);
                lobbyManager.CreateLobby(GamemodeIDs.ScoreBattle, scoreBattleSettings);
                _waitingForLobbyCreateResponse = true;
                ClientController.PacketReceived += OnPacketReceived_LobbyCreation;
            }
        };
        ScrollView.InsertButton(0, _createLobbyButton);
    }

    private void OnPacketReceived_LobbyCreation(Packets packetId, Packet packet)
    {
        if (packetId == Packets.ServerLobbyCreateResponse)
        {
            _waitingForLobbyCreateResponse = false;
            ClientController.PacketReceived -= OnPacketReceived_LobbyCreation;
            MyPhone.OpenApp(typeof(AppMultiplayerCurrentLobby));
        }
    }

    private void MakeLobbyButton()
    {
        _lobbyButton = PhoneUIUtility.CreateSimpleButton("Lobby");
        _lobbyButton.OnConfirm += () =>
        {
            MyPhone.OpenApp(typeof(AppMultiplayerCurrentLobby));
        };
        ScrollView.InsertButton(0, _lobbyButton);
    }

    private void PopulateButtons()
    {
        ScrollView.RemoveAllButtons();

        var button = PhoneUIUtility.CreateSimpleButton("Invites");
        button.OnConfirm += () =>
        {
            MyPhone.OpenApp(typeof(AppMultiplayerInvites));
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Stages");
        button.OnConfirm += () =>
        {
            if (MapStationSupport.Stages.Count > 0)
                MyPhone.OpenApp(typeof(AppMultiplayerStages));
            else
                MyPhone.OpenApp(typeof(AppMultiplayerBaseStages));
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Spectate");
        button.OnConfirm += () =>
        {
            SpectatorController.StartSpectating(false);
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Go AFK");
        button.OnConfirm += () =>
        {
            PlayerComponent.Get(MyPhone.player).ForceAFK();
            MyPhone.TurnOff(true);
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Stop AFK");
        button.OnConfirm += () =>
        {
            PlayerComponent.Get(MyPhone.player).StopAFK();
            MyPhone.TurnOff(true);
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Movestyle");
        button.OnConfirm += () =>
        {
            MyPhone.OpenApp(typeof(MoveStylePickerApp));
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Stats");
        button.OnConfirm += () =>
        {
            var spec = SpectatorController.Instance;
            if (spec != null && spec.Forced) return;
            var statsUi = StatsUI.Instance;
            if (!statsUi.Displaying)
            {
                MyPhone.TurnOff(false);
                statsUi.Activate();
            }
        };
        ScrollView.AddButton(button);

#if DEBUG
        button = PhoneUIUtility.CreateSimpleButton("Debug");
        button.OnConfirm += () =>
        {
            MyPhone.OpenApp(typeof(AppMultiplayerDebug));
        };
        ScrollView.AddButton(button);
#endif
    }
}
