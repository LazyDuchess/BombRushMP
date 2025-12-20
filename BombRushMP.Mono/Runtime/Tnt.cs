#if PLUGIN
using Reptile;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Mono.Runtime
{
    public class Tnt : MonoBehaviour
    {
        public GameObject ExplosionEffect;
        public Material FlashMaterial;
        public float ExplosionTime = 1.0f;
        public float ExplosionRadius = 2f;
        public int ExplosionDamage = 2;
        public float GrowTime = 0.5f;
        public float GrowSize = 0.3f;
        public float FlashInterval = 0.1f;
        public float FlashTime = 0.05f;

        public AudioClip[] ExplosionSounds;

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
            Gizmos.DrawSphere(transform.position, ExplosionRadius);
        }

#if PLUGIN
        private Material _originalMaterial;
        private Renderer _renderer;

        private float _flashTimer = 0f;
        private float _timer = 0f;

        private Rigidbody _rb;

        private AudioSource _audioSource;

        public void ApplyForce(Vector3 force)
        {
            _rb.AddForce(force, ForceMode.VelocityChange);
        }

        private void Awake()
        {
            _renderer = GetComponentInChildren<Renderer>();
            _originalMaterial = _renderer.sharedMaterial;
            _rb = GetComponent<Rigidbody>();
            _audioSource = GetComponentInChildren<AudioSource>();
        }

        private void Update()
        {
            if (_flashTimer >= FlashInterval + FlashTime)
            {
                _flashTimer = 0f;
                _renderer.sharedMaterial = _originalMaterial;
            }
            else if (_flashTimer >= FlashInterval)
            {
                _renderer.sharedMaterial = FlashMaterial;
            }

            var growAmount = ((Mathf.Clamp(_timer, GrowTime, ExplosionTime) - GrowTime) / (ExplosionTime - GrowTime)) * GrowSize;
            _renderer.transform.localScale = Vector3.one + new Vector3(growAmount, growAmount, growAmount);

            if (_timer >= ExplosionTime)
            {
                Explode();
            }

            _flashTimer += Time.deltaTime;
            _timer += Time.deltaTime;
        }

        private void Explode()
        {
            var explosionGFX = Instantiate(ExplosionEffect);
            explosionGFX.transform.position = transform.position;
            var explosionClip = ExplosionSounds[UnityEngine.Random.Range(0, ExplosionSounds.Length)];
            OneShotAudio.Create(explosionClip, transform.position, 1.0f, 10f, 15f);
            var colls = Physics.OverlapSphere(transform.position, ExplosionRadius, (1 << Layers.Player), QueryTriggerInteraction.Ignore);
            foreach(var coll in colls)
            {
                var player = coll.GetComponentInParent<Player>();
                if (player == null) continue;
                if (player.isAI) continue;
                player.GetHit(ExplosionDamage, (transform.position - player.transform.position).normalized, KnockbackAbility.KnockbackType.BIG);
                GameplayCamera.StartScreenShake(ScreenShakeType.HEAVY, 0.3f, false);
            }
            _timer = 0f;
            Destroy(gameObject);
        }
#endif
    }
}
