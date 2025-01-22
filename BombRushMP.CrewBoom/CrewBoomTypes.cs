using BombRushMP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.CrewBoom
{
    public static class CrewBoomTypes
    {
        public static Type CharacterDefinitionType;
        public static Type CharacterOutfitType;
        public static Type CharacterOutfitRendererType;
        public static FieldInfo CharacterDefinitionOutfitsField;
        public static FieldInfo CharacterDefinitionRenderersField;
        public static FieldInfo CharacterDefinitionCanBlinkField;
        public static FieldInfo CharacterOutfitEnabledRenderersField;
        public static FieldInfo CharacterOutfitMaterialContainersField;
        public static FieldInfo CharacterOutfitRendererMaterialsField;
        public static FieldInfo CharacterOutfitRendererUseShaderForMaterialField;

        public static FieldInfo CharacterDefinitionVoiceDieField;
        public static FieldInfo CharacterDefinitionVoiceDieFallField;
        public static FieldInfo CharacterDefinitionVoiceTalkField;
        public static FieldInfo CharacterDefinitionVoiceBoostTrickField;
        public static FieldInfo CharacterDefinitionVoiceComboField;
        public static FieldInfo CharacterDefinitionVoiceGetHitField;
        public static FieldInfo CharacterDefinitionVoiceJumpField;

        public static void Initialize()
        {
            CharacterDefinitionType = ReflectionUtility.GetTypeByName("CrewBoomMono.CharacterDefinition");
            CharacterOutfitType = ReflectionUtility.GetTypeByName("CrewBoomMono.CharacterOutfit");
            CharacterOutfitRendererType = ReflectionUtility.GetTypeByName("CrewBoomMono.CharacterOutfitRenderer");
            CharacterDefinitionOutfitsField = CharacterDefinitionType.GetField("Outfits");
            CharacterDefinitionRenderersField = CharacterDefinitionType.GetField("Renderers");
            CharacterDefinitionCanBlinkField = CharacterDefinitionType.GetField("CanBlink");
            CharacterOutfitEnabledRenderersField = CharacterOutfitType.GetField("EnabledRenderers");
            CharacterOutfitMaterialContainersField = CharacterOutfitType.GetField("MaterialContainers");
            CharacterOutfitRendererMaterialsField = CharacterOutfitRendererType.GetField("Materials");
            CharacterOutfitRendererUseShaderForMaterialField = CharacterOutfitRendererType.GetField("UseShaderForMaterial");

            CharacterDefinitionVoiceDieField = CharacterDefinitionType.GetField("VoiceDie");
            CharacterDefinitionVoiceDieFallField = CharacterDefinitionType.GetField("VoiceDieFall");
            CharacterDefinitionVoiceTalkField = CharacterDefinitionType.GetField("VoiceTalk");
            CharacterDefinitionVoiceBoostTrickField = CharacterDefinitionType.GetField("VoiceBoostTrick");
            CharacterDefinitionVoiceComboField = CharacterDefinitionType.GetField("VoiceCombo");
            CharacterDefinitionVoiceGetHitField = CharacterDefinitionType.GetField("VoiceGetHit");
            CharacterDefinitionVoiceJumpField = CharacterDefinitionType.GetField("VoiceJump");
        }
    }
}
