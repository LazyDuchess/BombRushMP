using BombRushMP.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.CrewBoom
{
    public static class CrewBoomStreamer
    {
        internal static Type CharacterDefinitionType;
        internal static FieldInfo CharacterDefinitionIdField;
        internal static Dictionary<string, CharacterHandle> CharacterHandleByGUID = new();
        private static Dictionary<string, string> BundlePathByGUID = new();

        public static CharacterHandle RequestCharacter(string guid)
        {
            if (CharacterHandleByGUID.TryGetValue(guid, out var handle))
            {
                handle.AddReference();
                return handle;
            }
            else
            {
                if (BundlePathByGUID.TryGetValue(guid, out var path))
                {
                    var newHandle = new CharacterHandle(guid, path);
                    newHandle.AddReference();
                    CharacterHandleByGUID[guid] = newHandle;
                    return newHandle;
                }
            }
            return null;
        }

        public static void Initialize(string cbbDirectory)
        {
            CharacterDefinitionType = ReflectionUtility.GetTypeByName("CrewBoomMono.CharacterDefinition");
            CharacterDefinitionIdField = CharacterDefinitionType.GetField("Id");
            var cbbFiles = Directory.GetFiles(cbbDirectory, "*.cbb", SearchOption.AllDirectories);
            foreach(var file in cbbFiles)
            {
                var txtFile = Path.ChangeExtension(file, ".txt");
                if (File.Exists(txtFile))
                {
                    Register(file, File.ReadAllText(txtFile));
                }
                else
                {
                    var bundle = AssetBundle.LoadFromFile(file);
                    var gos = bundle.LoadAllAssets<GameObject>();
                    foreach(var go in gos)
                    {
                        var charDef = go.GetComponent(CharacterDefinitionType);
                        if (charDef != null)
                        {
                            var id = CharacterDefinitionIdField.GetValue(charDef) as string;
                            Register(file, id);
                            File.WriteAllText(txtFile, id);
                            break;
                        }
                    }
                    bundle.Unload(true);
                }
            }
        }

        private static void Register(string file, string guid)
        {
            BundlePathByGUID[guid] = file;
        }
    }
}
