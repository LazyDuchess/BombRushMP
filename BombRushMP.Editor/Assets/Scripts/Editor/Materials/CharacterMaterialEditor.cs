using UnityEngine;
using UnityEditor;

// Custom Inspector for the BRC Character material.
public class CharacterMaterialEditor : ShaderGUI
{
    private enum Transparency
    {
        Opaque,
        Cutout
    }

    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);
        ValidateNewShader(material);
    }

    // Validate some properties after changing values in the inspector.
    public override void ValidateMaterial(Material material)
    {
        base.ValidateMaterial(material);
        var materials = new Material[] { material };
        var properties = MaterialEditor.GetMaterialProperties(materials);
        var transparencyProperty = ShaderGUI.FindProperty("_Transparency", properties);
        ValidateTransparency(properties, material, (Transparency)transparencyProperty.floatValue);
    }

    private void ValidateNewShader(Material material)
    {
        var materials = new Material[] { material };
        var properties = MaterialEditor.GetMaterialProperties(materials);
        var transparencyProperty = ShaderGUI.FindProperty("_Transparency", properties);
        ValidateTransparency(properties, material, (Transparency)transparencyProperty.floatValue);
        ValidateRenderQueue(properties, material, (Transparency)transparencyProperty.floatValue);
    }

    // Draw the inspector.
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        var transparencyProperty = ShaderGUI.FindProperty("_Transparency", properties);

        var cutoutProperty = ShaderGUI.FindProperty("_CutOut", properties);

        var material = materialEditor.target as Material;

        var transparencyOptions = new string[]
        {
            "Opaque",
            "Cutout"
        };

        var transparencyMode = (Transparency)transparencyProperty.floatValue;

        // If the transparency mode is changed automatically adjust the Render Queue.
        EditorGUI.BeginChangeCheck();
        {
            transparencyMode = (Transparency)EditorGUILayout.Popup("Render Mode", (int)transparencyMode, transparencyOptions);
        }
        if (EditorGUI.EndChangeCheck())
        {
            transparencyProperty.floatValue = (float)transparencyMode;
            ValidateRenderQueue(properties, material, transparencyMode);
        }

        // Display the alpha cutout slider if appropriate.
        if (transparencyMode == Transparency.Cutout)
        {
            materialEditor.ShaderProperty(cutoutProperty, cutoutProperty.displayName);
        }

        var inShade = false;
        var inOutline = false;
        foreach (var property in properties)
        {
            if ((property.flags & MaterialProperty.PropFlags.HideInInspector) == MaterialProperty.PropFlags.HideInInspector)
                continue;
            if (property.name.ToLowerInvariant().Contains("outline"))
            {
                if (!inOutline)
                {
                    inOutline = true;
                    EditorGUILayout.BeginVertical("GroupBox");
                    GUILayout.Label("Outline");
                    EditorGUI.indentLevel++;
                    EditorGUILayout.Space();
                }
            }
            else if (inOutline)
            {
                inOutline = false;
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
            if (property.name.ToLowerInvariant().StartsWith("_shade"))
            {
                if (!inShade)
                {
                    inShade = true;
                    EditorGUILayout.BeginVertical("GroupBox");
                    GUILayout.Label("Shading");
                    EditorGUI.indentLevel++;
                    EditorGUILayout.Space();
                }
            }
            else if (inShade)
            {
                inShade = false;
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
            var outlineProperty = ShaderGUI.FindProperty("_Outline", properties);
            if (outlineProperty.floatValue == 0f && property != outlineProperty && property.name.Contains("Outline"))
                continue;
            if (property.name == "_MainTex" || property.name == "_Emission")
            {
                EditorGUILayout.BeginVertical("GroupBox");
            }
            materialEditor.ShaderProperty(property, property.displayName);
            
            if (property.name == "_MainTex" || property.name == "_Emission")
            {
                var uvProperty = ShaderGUI.FindProperty($"{property.name}UV", properties);
                var scrollProperty = ShaderGUI.FindProperty($"{property.name}Scroll", properties);
                var uProperty = ShaderGUI.FindProperty($"{property.name}USpeed", properties);
                var vProperty = ShaderGUI.FindProperty($"{property.name}VSpeed", properties);
                materialEditor.ShaderProperty(uvProperty, uvProperty.displayName);
                materialEditor.ShaderProperty(scrollProperty, scrollProperty.displayName);
                if (scrollProperty.floatValue == 1f)
                {
                    EditorGUILayout.BeginHorizontal();
                    materialEditor.ShaderProperty(uProperty, uProperty.displayName);
                    materialEditor.ShaderProperty(vProperty, vProperty.displayName);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
        }

        if (inShade || inOutline)
        {
            inShade = false;
            inOutline = false;
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
        materialEditor.RenderQueueField();
    }

    private void ValidateTransparency(MaterialProperty[] properties, Material material, Transparency transparency)
    {
        if (transparency > Transparency.Cutout)
        {
            transparency = Transparency.Opaque;
            ShaderGUI.FindProperty("_Transparency", properties).floatValue = (float)Transparency.Opaque;
        }
        material.DisableKeyword("_TRANSPARENCY_OPAQUE");
        material.DisableKeyword("_TRANSPARENCY_CUTOUT");

        switch (transparency)
        {
            case Transparency.Opaque:
                material.EnableKeyword("_TRANSPARENCY_OPAQUE");
                break;

            case Transparency.Cutout:
                material.EnableKeyword("_TRANSPARENCY_CUTOUT");
                break;
        }
    }

    private void ValidateRenderQueue(MaterialProperty[] properties, Material material, Transparency transparency)
    {
        switch (transparency)
        {
            case Transparency.Opaque:
                material.renderQueue = 2000;
                break;

            case Transparency.Cutout:
                material.renderQueue = 2450;
                break;
        }
    }
}