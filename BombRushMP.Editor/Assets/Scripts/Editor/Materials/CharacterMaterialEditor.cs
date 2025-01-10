using UnityEngine;
using UnityEditor;

// Custom Inspector for the BRC Character material.
public class CharacterMaterialEditor : ShaderGUI
{
    private enum Transparency
    {
        Opaque,
        Cutout,
        Transparent
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

    private bool IsTextureProperty(string property)
    {
        return (property == "_MainTex" || property == "_Emission" || property == "_EmissionMask" || property == "_OverlayTex" || property == "_OverlayMask" || property == "_OutlineMask");
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
            "Cutout",
            "Transparent"
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
            if (IsTextureProperty(property.name))
            {
                EditorGUILayout.BeginVertical("GroupBox");
            }
            materialEditor.ShaderProperty(property, property.displayName);
            
            if (IsTextureProperty(property.name))
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
        var blendSrcProperty = ShaderGUI.FindProperty("_BlendSrc", properties);
        var blendDestProperty = ShaderGUI.FindProperty("_BlendDst", properties);

        if (transparency == Transparency.Transparent)
        {
            blendSrcProperty.floatValue = 5f;
            blendDestProperty.floatValue = 10f;
        }
        else
        {
            blendSrcProperty.floatValue = 1f;
            blendDestProperty.floatValue = 0f;
        }

        material.DisableKeyword("_TRANSPARENCY_OPAQUE");
        material.DisableKeyword("_TRANSPARENCY_CUTOUT");
        material.DisableKeyword("_TRANSPARENCY_TRANSPARENT");

        switch (transparency)
        {
            case Transparency.Opaque:
                material.EnableKeyword("_TRANSPARENCY_OPAQUE");
                break;

            case Transparency.Cutout:
                material.EnableKeyword("_TRANSPARENCY_CUTOUT");
                break;

            case Transparency.Transparent:
                material.EnableKeyword("_TRANSPARENCY_TRANSPARENT");
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

            case Transparency.Transparent:
                material.renderQueue = 3000;
                break;
        }
    }
}