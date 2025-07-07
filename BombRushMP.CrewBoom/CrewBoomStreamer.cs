using BombRushMP.Common;
using CommonAPI;
using HarmonyLib;
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
        public static bool AlreadyLoadedThisSession = false;
        public static int LoadedCharacters => CharacterHandleByGUID.Count;
        public static Shader CharacterShader { get; private set; }
        internal static FieldInfo CharacterDefinitionIdField;
        internal static Dictionary<Guid, CharacterHandle> CharacterHandleByGUID = new();
        private static Dictionary<Guid, string> BundlePathByGUID = new();
        private static List<string> Directories = new();
        private static HashSet<string> CrewBoomFilenames = new();

        public static void Initialize()
        {
            CharacterDefinitionIdField = CrewBoomTypes.CharacterDefinitionType.GetField("Id");
        }

        public static void LoadCrewBoomConfigFolder(string configFolder)
        {
            var cbFiles = Directory.GetFiles(configFolder, "*.cbb", SearchOption.TopDirectoryOnly).ToList();
            cbFiles.AddRange(Directory.GetFiles(Path.Combine(configFolder, "no_cypher"), "*.cbb", SearchOption.TopDirectoryOnly));
            foreach(var cbFile in cbFiles)
            {
                CrewBoomFilenames.Add(Path.GetFileNameWithoutExtension(cbFile));
            }
        }

        public static void AddDirectory(string directory)
        {
            Directories.Add(directory);
            Directory.CreateDirectory(directory);
        }

        public static void ReloadResources()
        {
            CharacterShader = AssetAPI.GetShader(AssetAPI.ShaderNames.AmbientCharacter);
        }

        public static void ReloadCharacters()
        {
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
            AlreadyLoadedThisSession = true;
        }

        public static void LoadFromDirectory(string directory)
        {
            var cbbFiles = Directory.GetFiles(directory, "*.cbb", SearchOption.AllDirectories);
            foreach (var file in cbbFiles)
            {
                var errString = $"Shit! Error trying to load streamed character {Path.GetFileName(file)}";
                var txtFile = Path.ChangeExtension(file, ".txt");
                if (CrewBoomFilenames.Contains(Path.GetFileNameWithoutExtension(file)))
                {
                    Debug.LogError(errString);
                    Debug.LogError("A CBB with the same name already exists in your regular CrewBoom folder.");
                    break;
                }
                if (File.Exists(txtFile))
                {
                    var guid = Guid.Parse(File.ReadAllText(txtFile));
                    if (HasCharacter(guid))
                    {
                        Debug.LogError(errString);
                        Debug.LogError($"GUID ({guid.ToString()}) is already taken by another streamed CrewBoom character.");
                        break;
                    }
                    if (CrewBoomSupport.GetCharacterForGuid(guid, Reptile.Characters.NONE) != Reptile.Characters.NONE)
                    {
                        Debug.LogError(errString);
                        Debug.LogError($"GUID ({guid.ToString()}) is already taken by a CrewBoom character.");
                        break;
                    }
                    Register(guid, file);
                }
                else
                {
                    AssetBundle bundle = null;
                    try
                    {
                        bundle = AssetBundle.LoadFromFile(file);
                        var gos = bundle.LoadAllAssets<GameObject>();
                        foreach (var go in gos)
                        {
                            var charDef = go.GetComponent(CrewBoomTypes.CharacterDefinitionType);
                            if (charDef != null)
                            {
                                var id = Guid.Parse(CharacterDefinitionIdField.GetValue(charDef) as string);
                                if (HasCharacter(id))
                                {
                                    Debug.LogError(errString);
                                    Debug.LogError($"GUID ({id.ToString()}) is already taken by another streamed CrewBoom character.");
                                    break;
                                }
                                if (CrewBoomSupport.GetCharacterForGuid(id, Reptile.Characters.NONE) != Reptile.Characters.NONE)
                                {
                                    Debug.LogError(errString);
                                    Debug.LogError($"GUID ({id.ToString()}) is already taken by a CrewBoom character.");
                                    break;
                                }
                                Register(id, file);
                                File.WriteAllText(txtFile, id.ToString());
                                break;
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        Debug.LogError(errString);
                        Debug.LogException(e);
                    }
                    finally
                    {
                        if (bundle != null)
                            bundle.Unload(true);
                    }
                }
            }
        }

        public static bool HasCharacter(Guid guid)
        {
            return BundlePathByGUID.ContainsKey(guid);
        }

        public static CharacterHandle RequestCharacter(Guid guid, bool isAsync)
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
                    var newHandle = new CharacterHandle(guid);
                    if (isAsync)
                        newHandle.LoadAsync(path);
                    else
                        newHandle.Load(path);
                    newHandle.AddReference();
                    CharacterHandleByGUID[guid] = newHandle;
                    return newHandle;
                }
            }
            return null;
        }
        public static void Tick()
        {
            var charHandles = new Dictionary<Guid, CharacterHandle>(CharacterHandleByGUID);
            foreach(var handle in charHandles)
            {
                if (handle.Value.References <= 0 && handle.Value.Finished)
                {
                    handle.Value.Dispose();
                    CharacterHandleByGUID.Remove(handle.Key);
                }
            }
        }

        private static void Register(Guid guid, string file)
        {
            BundlePathByGUID[guid] = file;
        }
    }
}
