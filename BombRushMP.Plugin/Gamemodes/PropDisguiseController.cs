using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Reptile;
using BombRushMP.Common;

namespace BombRushMP.Plugin.Gamemodes
{
    public class PropDisguiseController : MonoBehaviour
    {
        public static PropDisguiseController Instance { get; private set; }
        public Dictionary<int, GameObject> PropByIndex = new();
        public Dictionary<GameObject, int> IndexByProp = new();
        public Dictionary<int, float> SizeByIndex = new();
        public bool FrozenProps = false;
        public bool InPropHunt = false;
        public bool InSetupPhase = false;
        public PropHuntTeams LocalPropHuntTeam = PropHuntTeams.None;
        public int StageHash { get; private set; }
        private GameObject _currentlyOutlinedGameObject = null;
        private GameObject _outlineGameObject = null;

        public float PropVerticalSpeed;
        public float PropHorizontalSpeed;
        public int PropDamage;
        public float PropMinimumSize;
        public float HunterReticleSize;
        public int HunterPelletAmount;

        private string _hSpeedTag = "ph_hspeed=";
        private string _vSpeedTag = "ph_vspeed=";
        private string _damageTag = "ph_damage=";
        private string _sizeTag = "ph_size=";
        private string _reticleTag = "ph_reticle=";
        private string _pelletTag = "ph_pellet=";
        private string _versionIdentifier = "VERSION2";

        public void LoadTestSettings()
        {
            PropVerticalSpeed = 10f;
            PropHorizontalSpeed = 10f;
            PropDamage = 2;
            PropMinimumSize = 0.1f;
            HunterReticleSize = 0.5f;
            HunterPelletAmount = 8;

            foreach(var tag in ClientController.Instance.ServerState.Tags)
            {

                if (tag.StartsWith(_hSpeedTag))
                    PropHorizontalSpeed = float.Parse(tag.Substring(_hSpeedTag.Length));

                if (tag.StartsWith(_vSpeedTag))
                    PropVerticalSpeed = float.Parse(tag.Substring(_vSpeedTag.Length));

                if (tag.StartsWith(_damageTag))
                    PropDamage = int.Parse(tag.Substring(_damageTag.Length));

                if (tag.StartsWith(_sizeTag))
                    PropMinimumSize = float.Parse(tag.Substring(_sizeTag.Length));

                if (tag.StartsWith(_reticleTag))
                    HunterReticleSize = float.Parse(tag.Substring(_reticleTag.Length));

                if (tag.StartsWith(_pelletTag))
                    HunterPelletAmount = int.Parse(tag.Substring(_pelletTag.Length));
            }
        }

        public void OutlineGameObject(GameObject obj)
        {
            if (obj == _currentlyOutlinedGameObject) return;
            if (_outlineGameObject != null)
            {
                Destroy(_outlineGameObject);
                _outlineGameObject = null;
            }
            _currentlyOutlinedGameObject = obj;
            if (obj == null) return;

            _outlineGameObject = Instantiate(obj);
            

            var colliders = _outlineGameObject.GetComponentsInChildren<Collider>();
            var rbs = _outlineGameObject.GetComponentsInChildren<Rigidbody>();
            var renderers = _outlineGameObject.GetComponentsInChildren<Renderer>();
            var filters = _outlineGameObject.GetComponentsInChildren<MeshFilter>();
            var streetLife = _outlineGameObject.GetComponentInChildren<StreetLife>();
            var anim = _outlineGameObject.GetComponentInChildren<Animator>();

            if (streetLife != null && anim != null)
            {
                anim.Play(Animator.StringToHash(streetLife.idleAnimation.ToString()), 0);
            }

            if (anim != null)
                Destroy(anim);

            foreach(var collider in colliders)
            {
                Destroy(collider);
            }

            foreach(var rb in rbs)
            {
                Destroy(rb);
            }

            foreach(var renderer in renderers)
            {
                renderer.sharedMaterial = MPAssets.Instance.AimOutlineMaterial;
            }

            _outlineGameObject.transform.position = obj.transform.position;
            _outlineGameObject.transform.rotation = obj.transform.rotation;
            _outlineGameObject.transform.localScale = obj.transform.localScale;
        }

        public static void Create()
        {
            var go = new GameObject("Prop Disguise Controller");
            go.AddComponent<PropDisguiseController>();
        }

        private float _oldLodBias = 0f;

        public void FreezeProps()
        {
            if (FrozenProps) return;
            FrozenProps = true;
            WorldHandler.instance.SceneObjectsRegister.stageChunks.ForEach(chunk =>
            {
                var junkBehaviour = chunk.junkBehaviour;
                junkBehaviour.kickedJunkIndex = 0;
                junkBehaviour.nonupdatingJunkIndex = 0;
                foreach(var junk in junkBehaviour.totalJunk)
                {
                    JunkBehaviour.RestoreSingle(junkBehaviour, junk);
                }
            });

            var junkStageHandlers = FindObjectsOfType<JunkStageHandler>();
            foreach(var junkStageHandler in junkStageHandlers)
            {
                var junkBehaviour = junkStageHandler.junkBehaviour;
                junkBehaviour.kickedJunkIndex = 0;
                junkBehaviour.nonupdatingJunkIndex = 0;
                foreach (var junk in junkBehaviour.totalJunk)
                {
                    JunkBehaviour.RestoreSingle(junkBehaviour, junk);
                }
            }

            var breakables = FindObjectsOfType<BreakableObject>(true);
            foreach(var breakable in breakables)
            {
                breakable.gameObject.SetActive(false);
            }

            _oldLodBias = QualitySettings.lodBias;
            QualitySettings.lodBias *= 3f;
        }

        public void UnfreezeProps()
        {
            if (!FrozenProps) return;
            FrozenProps = false;
            QualitySettings.lodBias *= _oldLodBias;
        }

        private void Awake()
        {
            Instance = this;
            CacheProps();
        }

        private void CacheProps()
        {
            var junk = FindObjectsOfType<Junk>();
            var streetlife = FindObjectsOfType<StreetLife>();

            foreach (var j in junk)
            {
                var stageChunk = j.GetComponentInParent<StageChunk>();
                var junkStageHandler = j.GetComponentInParent<JunkStageHandler>();
                if (stageChunk == null && junkStageHandler == null) continue;
                var id = MPUtility.GenerateGameObjectID(j.gameObject);
                PropByIndex[id] = j.gameObject;
                IndexByProp[j.gameObject] = id;
                var renderer = j.GetComponentInChildren<Renderer>();
                if (renderer == null)
                {
                    SizeByIndex[id] = 0f;
                }
                else
                {
                    SizeByIndex[id] = Mathf.Min(renderer.bounds.size.x, renderer.bounds.size.y, renderer.bounds.size.z);
                }
            }

            foreach (var ped in streetlife)
            {
                var stageChunk = ped.GetComponentInParent<StageChunk>();
                if (stageChunk == null) continue;
                var moveAlong = ped.GetComponent<MoveAlongPoints>();
                if (moveAlong != null) continue;
                var id = MPUtility.GenerateGameObjectID(ped.gameObject);
                PropByIndex[id] = ped.gameObject;
                IndexByProp[ped.gameObject] = id;
                var renderer = ped.GetComponentInChildren<Renderer>();
                if (renderer == null)
                {
                    SizeByIndex[id] = 0f;
                }
                else
                {
                    SizeByIndex[id] = Mathf.Min(renderer.bounds.size.x, renderer.bounds.size.y, renderer.bounds.size.z);
                }
            }

            var stageHashStr = "";
            foreach(var index in PropByIndex.Keys)
            {
                stageHashStr += index.ToString() + "|";
            }
            stageHashStr += _versionIdentifier;

            StageHash = Compression.HashString(stageHashStr);
        }
    }
}
