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
        public Dictionary<int, GameObject> Props = new();
        public bool FrozenProps = false;

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
                Props[MPUtility.GenerateGameObjectID(j.gameObject)] = j.gameObject;
            }

            foreach (var ped in streetlife)
            {
                var stageChunk = ped.GetComponentInParent<StageChunk>();
                if (stageChunk == null) continue;
                var moveAlong = ped.GetComponent<MoveAlongPoints>();
                if (moveAlong != null) continue;
                Props[MPUtility.GenerateGameObjectID(ped.gameObject)] = ped.gameObject;
            }
        }
    }
}
