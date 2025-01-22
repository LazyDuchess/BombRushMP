using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Reptile;
using BombRushMP.PluginCommon;

namespace BombRushMP.CrewBoom
{
    public class CharacterHandle : IDisposable
    {
        public GameObject CharacterPrefab { get; private set; } = null;
        public int References { get; private set; } = 0;
        public AudioLibrary AudioLibrary { get; private set; }
        private AssetBundle _bundle = null;
        public Guid GUID = Guid.Empty;

        public CharacterHandle(Guid guid, string bundle)
        {
            GUID = guid;
            _bundle = AssetBundle.LoadFromFile(bundle);
            var gos = _bundle.LoadAllAssets<GameObject>();
            foreach(var go in gos)
            {
                var charDef = go.GetComponent(CrewBoomTypes.CharacterDefinitionType);
                if (charDef != null)
                {
                    CharacterPrefab = go;
                    AudioLibrary = AudioLibraryUtils.CreateFromCrewBoomCharacterDefinition(charDef);
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
