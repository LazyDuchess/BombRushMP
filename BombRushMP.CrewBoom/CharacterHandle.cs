using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Reptile;

namespace BombRushMP.CrewBoom
{
    public class CharacterHandle : IDisposable
    {
        public GameObject CharacterPrefab { get; private set; } = null;
        public int References { get; private set; } = 0;
        private AssetBundle _bundle = null;
        private string _guid = string.Empty;

        public CharacterHandle(string guid, string bundle)
        {
            _guid = guid;
            _bundle = AssetBundle.LoadFromFile(bundle);
            var gos = _bundle.LoadAllAssets<GameObject>();
            foreach(var go in gos)
            {
                var charDef = go.GetComponent(CrewBoomTypes.CharacterDefinitionType);
                if (charDef != null)
                {
                    CharacterPrefab = go;
                    break;
                }
            }
        }

        public void AddReference()
        {
            References++;
        }

        public void Release()
        {
            References--;
        }

        public void Dispose()
        {
            if (_bundle != null)
            {
                _bundle.Unload(true);
                _bundle = null;
            }
        }

        public CharacterVisual ConstructVisual()
        {
            var prefabInstance = GameObject.Instantiate(CharacterPrefab);
            var visual = new GameObject($"{CharacterPrefab.name} Visual");
            prefabInstance.transform.SetParent(visual.transform, false);
            prefabInstance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            var visualComp = visual.AddComponent<CharacterVisual>();
            return visualComp;
        }
    }
}
