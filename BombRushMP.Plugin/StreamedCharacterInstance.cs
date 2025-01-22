using BombRushMP.CrewBoom;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BombRushMP.PluginCommon;

namespace BombRushMP.Plugin
{
    public class StreamedCharacterInstance
    {
        public CharacterHandle Handle { get; private set; }
        public int Outfit = 0;
        private CharacterVisual _visual;
        private object _charDef;

        public StreamedCharacterInstance(CharacterHandle handle)
        {
            Handle = handle;
            Handle.AddReference();
        }

        public void SetVisual(CharacterVisual visual)
        {
            _visual = visual;
            _charDef = visual.GetComponentInChildren(CrewBoomTypes.CharacterDefinitionType);
        }

        public void ApplyOutfit(int outfitIndex)
        {
            Outfit = outfitIndex;
            var fits = CrewBoomTypes.CharacterDefinitionOutfitsField.GetValue(_charDef) as object[];
            var fit = fits[outfitIndex];
            var materialContainers = CrewBoomTypes.CharacterOutfitMaterialContainersField.GetValue(fit) as object[];
            var renderers = CrewBoomTypes.CharacterDefinitionRenderersField.GetValue(_charDef) as SkinnedMeshRenderer[];
            SkinnedMeshRenderer mainRenderer = null;
            for(var i = 0; i < renderers.Length; i++)
            {
                var renderer = renderers[i];
                var enabledRenderers = CrewBoomTypes.CharacterOutfitEnabledRenderersField.GetValue(fit) as bool[];
                var rendererEnabled = enabledRenderers[i];
                renderer.gameObject.SetActive(rendererEnabled);
                if (rendererEnabled)
                {
                    if (mainRenderer == null)
                        mainRenderer = renderer;
                    var mats = CrewBoomTypes.CharacterOutfitRendererMaterialsField.GetValue(materialContainers[i]) as Material[];
                    var useShader = CrewBoomTypes.CharacterOutfitRendererUseShaderForMaterialField.GetValue(materialContainers[i]) as bool[];
                    for(var n=0;n<mats.Length;n++)
                    {
                        if (useShader[n])
                            mats[n].shader = CrewBoomStreamer.CharacterShader;
                    }
                    renderer.sharedMaterials = mats;
                }
            }
            _visual.mainRenderer = mainRenderer;
            _visual.canBlink = (bool)CrewBoomTypes.CharacterDefinitionCanBlinkField.GetValue(_charDef);
        }
    }
}
