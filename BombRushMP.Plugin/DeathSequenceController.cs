using BombRushMP.NetRadio;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class DeathSequenceController : MonoBehaviour
    {
        public static DeathSequenceController Instance { get; private set; }
        private Player _player;
        private Camera _cam;
        private float _timer = 0f;
        private const float SpinSpeed = 4f;
        private const float MoveSpeed = 0.1f;
        private const float HeadOffset = 0.4f;
        private bool _wasMusicPlaying = false;
        private AudioSource _musicSource;

        private void Awake()
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
            var assets = MPAssets.Instance;
            _musicSource.clip = assets.DeathMusic[UnityEngine.Random.RandomRangeInt(0, assets.DeathMusic.Length)];
            _musicSource.spatialBlend = 0f;
            _musicSource.loop = false;
            _musicSource.volume = Core.Instance.audioManager.MusicVolume;
            _musicSource.Play();
            Instance = this;
            var worldHandler = WorldHandler.instance;
            _player = worldHandler.GetCurrentPlayer();
            _cam = worldHandler.CurrentCamera;
            if (NetRadioSupport.Installed)
            {
                NetRadioSupport.Paused = true;
            }
            var musicPlayer = Core.Instance.AudioManager.MusicPlayer;
            _wasMusicPlaying = musicPlayer.IsPlaying;
            musicPlayer.Pause();
        }

        private void LateUpdate()
        {
            _cam.transform.position = _player.headTf.position + (HeadOffset * Vector3.up) + (_timer * MoveSpeed * Vector3.up);
            var camRot = Quaternion.LookRotation(Vector3.down, _player.transform.forward) * Quaternion.Euler(0f, 0f, _timer * SpinSpeed);
            _cam.transform.rotation = camRot;
            _timer += Time.deltaTime;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void End()
        {
            if (NetRadioSupport.Installed)
            {
                NetRadioSupport.Paused = false;
            }
            var musicPlayer = Core.Instance.AudioManager.MusicPlayer;
            if (_wasMusicPlaying)
                musicPlayer.Play();
            Destroy(gameObject);
        }

        public static void Create()
        {
            var go = new GameObject("Death Sequence Controller");
            go.AddComponent<DeathSequenceController>();
        }
    }
}
