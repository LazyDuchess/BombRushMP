using BombRushMP.Common;
using CommonAPI;
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
        public static int LoadedCharacters => CharacterHandleByGUID.Count;
        public static Shader CharacterShader { get; private set; }
        internal static FieldInfo CharacterDefinitionIdField;
        internal static Dictionary<string, CharacterHandle> CharacterHandleByGUID = new();
        private static Dictionary<string, string> BundlePathByGUID = new();
        private static List<string> Directories = new();

        public static void Initialize()
        {
            CharacterDefinitionIdField = CrewBoomTypes.CharacterDefinitionType.GetField("Id");
        }

        public static void AddDirectory(string directory)
        {
            Directories.Add(directory);
        }

        public static void Reload()
        {
            CharacterShader = AssetAPI.GetShader(AssetAPI.ShaderNames.AmbientCharacter);
            foreach(var ch in CharacterHandleByGUID)
            {
                ch.Value.Dispose();
            }
            CharacterHandleByGUID.Clear();
            BundlePathByGUID.Clear();
            foreach(var directory in Directories)
            {
                LoadFromDirectory(directory);
            }
        }

        public static void LoadFromDirectory(string directory)
        {
            var cbbFiles = Directory.GetFiles(directory, "*.cbb", SearchOption.AllDirectories);
            foreach (var file in cbbFiles)
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
                    foreach (var go in gos)
                    {
                        var charDef = go.GetComponent(CrewBoomTypes.CharacterDefinitionType);
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
        public static void Tick()
        {
            var charHandles = new Dictionary<string, CharacterHandle>(CharacterHandleByGUID);
            foreach(var handle in charHandles)
            {
                if (handle.Value.References <= 0)
                {
                    handle.Value.Dispose();
                    CharacterHandleByGUID.Remove(handle.Key);
                }
            }
        }

        private static void Register(string file, string guid)
        {
            BundlePathByGUID[guid] = file;
        }
    }
}
