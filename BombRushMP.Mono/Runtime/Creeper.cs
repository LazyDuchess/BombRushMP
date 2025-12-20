using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if PLUGIN
using Reptile;
#endif

namespace BombRushMP.Mono.Runtime
{
    public class Creeper : MonoBehaviour
    {
        public float AnimationSpeed = 5f;
        public float AnimationDegrees = 30f;

        public float Knockback = 1f;
        public float KnockbackUp = 1f;
        public float KnockbackTime = 0.5f;

        private float _knockbackTimer = 0f;

        public AudioSource PrimeAudioSource;

        public Material FlashMaterial;
        public float DetectionRange = 10f;
        public float AttackRange = 0.8f;
        public float KillPlaneY = 0f;

        public Renderer MovementArea;

        public AudioClip[] ExplosionSFX;
        public GameObject ExplosionEffect;

        public float ExplosionTime = 1.5f;
        public int ExplosionDamage = 3;
        public float ExplosionRadius = 2f;
        public float PrimeRadius = 3f;

        public float TurnSpeed = 200f;
        public float TopSpeed = 5f;
        public float Acceleration = 2f;
        public float PrimeGrace = 0.8f;
        public float PrimeGraceRadius = 4f;

        public float GrowTime = 1.2f;
        public float GrowSize = 0.3f;

        public float FlashInterval = 0.1f;
        public float FlashTime = 0.05f;

        public float HeadHeight = 1.8f;

        public float WanderUpdateTime = 1.5f;

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
            Gizmos.DrawSphere(transform.position, ExplosionRadius);

            Gizmos.color = new Color(0f, 0f, 1f, 1.0f);
            Gizmos.DrawWireSphere(transform.position, DetectionRange);

            Gizmos.color = new Color(1f, 1f, 0f, 1.0f);
            Gizmos.DrawWireSphere(transform.position + (HeadHeight * transform.up), 0.5f);
        }

#if PLUGIN
        public enum States
        {
            None,
            LookAround,
            Wander,
            Attack
        }

        private Bounds _movementBounds;
        private Rigidbody _rb;
        private Renderer _renderer;
        private Material _originalMaterial;
        private Player _player = null;
        private States _currentState = States.None;
        private float _stateTimer = 0f;
        private bool _primed = false;
        private float _primeTimer = 0f;
        private float _flashTimer = 0f;

        private Transform _frontLLeg;
        private Transform _frontRLeg;
        private Transform _backLLeg;
        private Transform _backRLeg;

        private Vector3 _movementVector = Vector3.zero;

        private static States[] WanderStates = { States.Wander, States.LookAround };
        private static List<Creeper> Creepers = new();

        private void Awake()
        {
            if (MovementArea != null)
                _movementBounds = MovementArea.bounds;
            else
                _movementBounds = new Bounds(transform.position, new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity));
            _rb = GetComponent<Rigidbody>();
            _renderer = GetComponentInChildren<Renderer>();
            _originalMaterial = _renderer.sharedMaterial;

            var root = transform.FindRecursive("root");

            _frontLLeg = root.Find("footf.L");
            _frontRLeg = root.Find("footf.R");

            _backLLeg = root.Find("footb.L");
            _backRLeg = root.Find("footb.R");

            Creepers.Add(this);
        }

        private void UpdateAnimation()
        {
            var flatVelocity = _rb.velocity;
            flatVelocity.y = 0f;

            var flatSpeed = flatVelocity.magnitude;

            var animMultiplier = Mathf.Min(1f, flatSpeed / TopSpeed);

            var sine = Mathf.Sin(Time.timeSinceLevelLoad * AnimationSpeed) * AnimationDegrees;

            _frontLLeg.transform.localRotation = Quaternion.Euler(147.323f + (sine * animMultiplier), 0f, 0f);
            _frontRLeg.transform.localRotation = Quaternion.Euler(147.323f - (sine * animMultiplier), 0f, 0f);

            _backRLeg.transform.localRotation = Quaternion.Euler(-146.254f + (sine * animMultiplier), 0f, 0f);
            _backLLeg.transform.localRotation = Quaternion.Euler(-146.254f - (sine * animMultiplier), 0f, 0f);
        }

        private void UpdateDetection()
        {
            _player = null;
            var overlaps = Physics.OverlapSphere(transform.position + (transform.up * HeadHeight), DetectionRange, (1 << Layers.Player));
            foreach(var overlap in overlaps)
            {
                var player = overlap.GetComponentInParent<Player>();
                if (player == null) continue;
                if (player.isAI) continue;
                if (player.IsDead()) continue;
                if (CalculateVisibility(player.transform.position + (player.transform.up * 0.25f)) ||
                    CalculateVisibility(player.transform.position + (player.transform.up * 0.8f)) ||
                    CalculateVisibility(player.transform.position + (player.transform.up * 1.3f)))
                {
                    _player = player;
                    break;
                }
            }
        }

        private void PickRandomWanderState()
        {
            SetState(WanderStates[UnityEngine.Random.Range(0, WanderStates.Length)], true);
        }

        private void CalculateState()
        {
            if (_player != null)
            {
                SetState(States.Attack);
                return;
            }
            else if (_currentState == States.None || _currentState == States.Attack || _stateTimer >= WanderUpdateTime)
            {
                PickRandomWanderState();
            }
        }

        private void SetState(States newState, bool forceResetTimer = false)
        {
            if (forceResetTimer)
                _stateTimer = 0f;
            if (newState == _currentState) return;
            _currentState = newState;
            _stateTimer = 0f;
        }

        private void AttackStateUpdate()
        {
            var diffToPlayer = (_player.transform.position - transform.position).normalized;
            var headingToPlayer = diffToPlayer.normalized;

            var distToPlayer = Vector3.Distance(transform.position, _player.transform.position);

            if (distToPlayer <= PrimeRadius && !_primed)
                Prime();

            if (_primed && distToPlayer > PrimeGraceRadius && _primeTimer <= PrimeGrace)
                UnPrime();

            var targetPoint = _player.transform.position;

            if (!_movementBounds.Contains(targetPoint))
            {
                targetPoint = _movementBounds.ClosestPoint(_player.transform.position);
            }

            targetPoint.y = transform.position.y;

            var distToPoint = Vector3.Distance(transform.position, targetPoint);
            var headingToPoint = (targetPoint - transform.position).normalized;

            if (distToPoint <= AttackRange)
                return;

            _movementVector = headingToPoint;
            _movementVector.y = 0f;
            _movementVector = _movementVector.normalized;
        }

        private void MovementUpdate()
        {
            if (_knockbackTimer > 0f)
                return;
            var flatVelocity = _rb.velocity;
            var yVelocity = flatVelocity.y;
            flatVelocity.y = 0f;
            var flatVelocityMagnitude = flatVelocity.magnitude;

            var movementVector = _movementVector;

            if (_movementVector.magnitude <= 0f)
            {
                movementVector = flatVelocity.normalized;
                if (flatVelocityMagnitude > 0f)
                {
                    flatVelocityMagnitude = Mathf.Max(0f, flatVelocityMagnitude - Acceleration * Time.deltaTime);
                }
            }
            else
            {
                flatVelocityMagnitude = Mathf.Min(TopSpeed, flatVelocityMagnitude + Acceleration * Time.deltaTime);
            }
            var newVel = flatVelocityMagnitude * movementVector;
            newVel.y = yVelocity;
            _rb.velocity = newVel;
        }

        private void LookUpdate()
        {
            var targetLook = Vector3.zero;
            if (_currentState == States.Attack && _player != null)
            {
                var dirToPlayer = (_player.transform.position - _rb.position).normalized;
                dirToPlayer.y = 0f;
                targetLook = dirToPlayer.normalized;
            }

            var yawDelta = Vector3.SignedAngle(_rb.rotation * Vector3.forward, targetLook, Vector3.up);
            _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0f, Mathf.Clamp(yawDelta, -TurnSpeed * Time.deltaTime, TurnSpeed * Time.deltaTime), 0f));
        }

        private void StateUpdate()
        {
            _movementVector = Vector3.zero;
            switch (_currentState)
            {
                case States.Attack:
                    AttackStateUpdate();
                    break;
            }
            _stateTimer += Time.deltaTime;
        }

        private bool CalculateVisibility(Vector3 to)
        {
            var origin = transform.position + (HeadHeight * transform.up);
            var dist = Vector3.Distance(origin, to);
            var dir = (to - origin).normalized;

            var ray = new Ray(origin, dir);

            return !Physics.Raycast(ray, dist, (1 << Layers.Default), QueryTriggerInteraction.Ignore);
        }

        private void CalculateCreeperCollision()
        {
            foreach(var creeper in Creepers)
            {
                if (creeper == this) continue;
                var otherPos = creeper.transform.position;
                otherPos.y = transform.position.y;
                var dist = Vector3.Distance(otherPos, transform.position);
                if (dist <= 1f)
                {
                    var headingToOther = (otherPos - transform.position).normalized;
                    transform.position -= headingToOther * (1f - dist);
                }
            }
        }

        private void FixedUpdate()
        {
            UpdateDetection();
            CalculateState();
            StateUpdate();
            MovementUpdate();
            LookUpdate();
            CalculateCreeperCollision();
            if (transform.localPosition.y <= KillPlaneY)
                Kill();
            _knockbackTimer -= Time.deltaTime;
            if (_knockbackTimer < 0f)
                _knockbackTimer = 0f;
        }

        private void OnTriggerStay(Collider other)
        {
            if (_knockbackTimer > 0f) return;
            if (other.gameObject.layer != Layers.PlayerHitbox) return;
            var player = other.gameObject.GetComponentInParent<Player>();
            if (player == null) return;
            if (player.isAI) return;
            if (player.IsDead()) return;
            GetKnockBack(player.transform.position);
        }

        private void GetKnockBack(Vector3 from)
        {
            if (_knockbackTimer > 0f)
                return;
            var head = (from - transform.position).normalized;
            head.y = 0f;
            head = head.normalized;
            _rb.velocity = -head * Knockback;
            _rb.velocity += Vector3.up * KnockbackUp;
            _knockbackTimer = KnockbackTime;
        }

        private void Prime()
        {
            if (_primed) return;
            _primed = true;
            PrimeAudioSource.Play();
        }

        private void UnPrime()
        {
            if (!_primed) return;
            _primed = false;
            _flashTimer = 0f;
            _primeTimer = 0f;
            _renderer.sharedMaterial = _originalMaterial;
            _renderer.transform.localScale = Vector3.one;
        }

        private void UpdatePrime()
        {
            if (!_primed) return;

            if (_flashTimer >= FlashInterval + FlashTime)
            {
                _flashTimer = 0f;
                _renderer.sharedMaterial = _originalMaterial;
            }
            else if (_flashTimer >= FlashInterval)
            {
                _renderer.sharedMaterial = FlashMaterial;
            }

            var growAmount = ((Mathf.Clamp(_primeTimer, GrowTime, ExplosionTime) - GrowTime) / (ExplosionTime - GrowTime)) * GrowSize;
            _renderer.transform.localScale = Vector3.one + new Vector3(growAmount, growAmount, growAmount);

            if (_primeTimer >= ExplosionTime)
            {
                Explode();
            }

            _primeTimer += Time.deltaTime;
            _flashTimer += Time.deltaTime;
        }

        private void Explode()
        {
            var explosionGFX = Instantiate(ExplosionEffect);
            explosionGFX.transform.position = transform.position;
            var explosionClip = ExplosionSFX[UnityEngine.Random.Range(0, ExplosionSFX.Length)];
            OneShotAudio.Create(explosionClip, transform.position, 1.0f, 10f, 15f);
            var colls = Physics.OverlapSphere(transform.position, ExplosionRadius, (1 << Layers.Player), QueryTriggerInteraction.Ignore);
            foreach (var coll in colls)
            {
                var player = coll.GetComponentInParent<Player>();
                if (player == null) continue;
                if (player.isAI) continue;
                player.GetHit(ExplosionDamage, (transform.position - player.transform.position).normalized, KnockbackAbility.KnockbackType.BIG);
                GameplayCamera.StartScreenShake(ScreenShakeType.HEAVY, 0.3f, false);
            }
            _primeTimer = 0f;
            Kill();
        }

        private void LateUpdate()
        {
            UpdatePrime();
            UpdateAnimation();
        }

        private void Kill()
        {
            Creepers.Remove(this);
            Destroy(gameObject);
        }
#endif
    }
}
