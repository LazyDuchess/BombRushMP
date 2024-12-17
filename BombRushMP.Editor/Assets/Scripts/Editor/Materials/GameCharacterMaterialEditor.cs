using UnityEngine;
using UnityEditor;

// Custom Inspector for the Game Character material.
public class GameCharacterMaterialEditor : ShaderGUI
{
    // Draw the inspector.
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        EditorGUILayout.HelpBox("The shader from the game will be used for this material.", MessageType.Info);
        foreach (var property in properties)
        {
            if ((property.flags & MaterialProperty.PropFlags.HideInInspector) == MaterialProperty.PropFlags.HideInInspector)
                continue;
            if (property.name == "_MainTex" || property.name == "_Emission")
            {
                EditorGUILayout.BeginVertical("GroupBox");
            }
            materialEditor.ShaderProperty(property, property.displayName);
            if (property.name == "_MainTex" || property.name == "_Emission")
            {
                EditorGUILayout.EndVertical();
            }
        }
        materialEditor.RenderQueueField();
    }
}