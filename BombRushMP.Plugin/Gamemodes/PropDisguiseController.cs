using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Reptile;

namespace BombRushMP.Plugin.Gamemodes
{
    public class PropDisguiseController : MonoBehaviour
    {
        public static PropDisguiseController Instance { get; private set; }
        public Dictionary<int, GameObject> PropByIndex = new();
        public Dictionary<GameObject, int> IndexByProp = new();
        public bool FrozenProps = false;
        public bool InPropHunt = false;
        public bool InSetupPhase = false;
        public PropHuntTeams LocalPropHuntTeam = PropHuntTeams.None;
        private GameObject _currentlyOutlinedGameObject = null;
        private GameObject _outlineGameObject = null;

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
        }

        public void UnfreezeProps()
        {
            if (!FrozenProps) return;
            FrozenProps = false;
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
            }
        }
    }
}
