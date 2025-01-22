using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.CrewBoom
{
    public class CharacterHandle
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
                var charDef = go.GetComponent(CrewBoomStreamer.CharacterDefinitionType);
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
            UpdateDeletion();
        }

        private void UpdateDeletion()
        {
            if (References > 0) return;
            CrewBoomStreamer.CharacterHandleByGUID.Remove(_guid);
            _bundle.Unload(true);
        }
    }
}
