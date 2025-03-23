using BombRushMP.Common;
using Reptile;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class StatsUI : MonoBehaviour
    {
        public static StatsUI Instance { get; private set; }
        public bool Displaying = false;
        private GameObject _canvas;
        private TextMeshProUGUI _statsLabel;
        private const string TokenPrefix = "$";
        private const float MinHeight = -1100;
        private const float MaxHeight = 2945;
        private const float AutoScrollSpeed = 60f;
        private const float ManualScrollSpeed = 400f;
        private const float AutoScrollTime = 3f;
        private const int UpButton = 21;
        private const int DownButton = 56;
        private const int CloseButton = 3;
        private const string Music = "MusicTrack_InThePocket";
        private float _autoScrollTimer = 0f;
        private string _originalText;
        private bool _wasMusicPlaying = false;
        private AudioSource _musicAudioSource;

        private void Awake()
        {
            Instance = this;
            _canvas = transform.Find("Canvas").gameObject;
            _musicAudioSource = transform.Find("Stats Music").GetComponent<AudioSource>();
            _musicAudioSource.outputAudioMixerGroup = Core.Instance.AudioManager.mixerGroups[4];
            _musicAudioSource.clip = Core.Instance.Assets.LoadAssetFromBundle<MusicTrack>("coreassets", Music).AudioClip;
            _statsLabel = _canvas.transform.Find("Stats").GetComponent<TextMeshProUGUI>();
            _statsLabel.spriteAsset = MPAssets.Instance.Sprites;
            _originalText = _statsLabel.text;
            var controls = _canvas.transform.Find("Controls");
            var closeGlyph = controls.Find("Close Glyph").GetComponent<TextMeshProUGUI>();
            var downGlyph = controls.Find("Down Glyph").GetComponent<TextMeshProUGUI>();
            var upGlyph = controls.Find("Up Glyph").GetComponent<TextMeshProUGUI>();
            UIUtility.MakeGlyph(closeGlyph, CloseButton);
            UIUtility.MakeGlyph(downGlyph, DownButton);
            UIUtility.MakeGlyph(upGlyph, UpButton);
            _canvas.SetActive(false);
        }

        private void Update()
        {
            if (Displaying)
            {
                if (_autoScrollTimer <= 0f)
                    _statsLabel.rectTransform.localPosition += new Vector3(0f, AutoScrollSpeed * Core.dt, 0f);

                var gameInput = Core.Instance.GameInput;
                var upHeld = gameInput.GetButtonHeld(UpButton);
                var downHeld = gameInput.GetButtonHeld(DownButton);
                var closeNew = gameInput.GetButtonNew(CloseButton);

                if (upHeld || downHeld)
                    _autoScrollTimer = AutoScrollTime;

                if (downHeld)
                    _statsLabel.rectTransform.localPosition += new Vector3(0f, ManualScrollSpeed * Core.dt, 0f);

                if (upHeld)
                    _statsLabel.rectTransform.localPosition -= new Vector3(0f, ManualScrollSpeed * Core.dt, 0f);

                if (closeNew)
                    Deactivate();

                _autoScrollTimer -= Core.dt;
                if (_autoScrollTimer < 0f)
                    _autoScrollTimer = 0f;
            }
            var currentScroll = _statsLabel.rectTransform.anchoredPosition.y;
            currentScroll = Mathf.Clamp(currentScroll, MinHeight, MaxHeight);
            _statsLabel.rectTransform.anchoredPosition = new Vector2(0f, currentScroll);
        }

        public void Activate()
        {
            var musicPlayer = Core.Instance.AudioManager.MusicPlayer;
            _wasMusicPlaying = musicPlayer.IsPlaying;
            musicPlayer.Pause();
            _musicAudioSource.Play();
            MPUtility.CloseMenusAndSpectator();
            var player = WorldHandler.instance.GetCurrentPlayer();
            player.ui.TurnOn(false);
            player.phone.AllowPhone(false);
            Displaying = true;
            _canvas.SetActive(true);
            InitializeStats();
            _statsLabel.rectTransform.anchoredPosition = new Vector2(0f, MinHeight);
            _autoScrollTimer = 0f;
            var gameInput = Core.Instance.GameInput;
            gameInput.DisableAllControllerMaps(0);
            gameInput.EnableControllerMaps(BaseModule.IN_GAME_INPUT_MAPS);
            gameInput.EnableControllerMaps(BaseModule.MENU_INPUT_MAPS);
            player.userInputEnabled = false;
        }

        public void Deactivate()
        {
            _musicAudioSource.Stop();
            var musicPlayer = Core.Instance.AudioManager.MusicPlayer;
            if (_wasMusicPlaying)
                musicPlayer.Play();
            var player = WorldHandler.instance.GetCurrentPlayer();
            player.userInputEnabled = true;
            player.ui.TurnOn(true);
            player.phone.AllowPhone(true);
            Displaying = false;
            _canvas.SetActive(false);
            var gameInput = Core.Instance.GameInput;
            gameInput.DisableAllControllerMaps(0);
            gameInput.EnableControllerMaps(BaseModule.IN_GAME_INPUT_MAPS, 0);
        }

        private void ReplaceStatToken(string statToken, string text)
        {
            ReplaceToken(statToken, $"<color=yellow>{text}</color>");
        }

        private void ReplaceToken(string token, string text)
        {
            _statsLabel.text = _statsLabel.text.Replace($"{TokenPrefix}{token}", text);
        }

        private void InitializeStats()
        {
            _statsLabel.text = _originalText;
            var stats = MPSaveData.Instance.Stats;
            var playerName = MPUtility.GetPlayerDisplayName(MPSettings.Instance.PlayerName);

            ReplaceToken("s_name", playerName);

            ReplaceStatToken("s_elitesbeaten", stats.ElitesBeaten.ToString());
            ReplaceStatToken("s_elites", stats.ElitesPlayedAgainst.ToString());

            ReplaceStatToken("s_asleeptime", FormatSecondsToTime(stats.TimeSpentAsleep));
            ReplaceStatToken("s_asleep", stats.TimesFallenAsleep.ToString());

            ReplaceStatToken("s_spectating", FormatSecondsToTime(stats.TimeSpentSpectating));

            ReplaceStatToken("s_playershit", stats.PlayersHit.ToString());
            ReplaceStatToken("s_timeshit", stats.TimesHit.ToString());

            ReplaceStatToken("s_deaths", stats.Deaths.ToString());

            ReplaceStatToken("s_hello", stats.TimesSaidHello.ToString());

            var gamemodesEnum = Enum.GetValues(typeof(GamemodeIDs));
            for(var i=gamemodesEnum.Length-1;i>=0;i--)
            {
                var gamemode = (GamemodeIDs)gamemodesEnum.GetValue(i);
                var played = 0U;
                var playedAlone = 0U;
                var wins = 0U;

                if (stats.GamemodesPlayed.TryGetValue(gamemode, out var result))
                    played = result;
                if (stats.GamemodesPlayedAlone.TryGetValue(gamemode, out result))
                    playedAlone = result;
                if (stats.GamemodesWon.TryGetValue(gamemode, out result))
                    wins = result;

                ReplaceStatToken($"s_played{(int)gamemode}", played.ToString());
                ReplaceStatToken($"s_alone{(int)gamemode}", playedAlone.ToString());
                ReplaceStatToken($"s_won{(int)gamemode}", wins.ToString());
            }
        }

        private static string FormatSecondsToTime(double totalSeconds)
        {
            int hours = (int)(totalSeconds / 3600);
            int minutes = (int)((totalSeconds % 3600) / 60);
            int seconds = (int)(totalSeconds % 60);

            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }


        public static void Create()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            var mpAssets = MPAssets.Instance;
            var prefab = mpAssets.Bundle.LoadAsset<GameObject>("Stats UI");
            var statsUi = Instantiate(prefab);
            statsUi.transform.SetParent(Core.Instance.UIManager.transform, false);
            statsUi.AddComponent<StatsUI>();
        }
    }
}
