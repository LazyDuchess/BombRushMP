using UnityEngine;
using UnityEditor;

// Custom Inspector for the BRC Environment material.
public class EnvironmentMaterialEditor : ShaderGUI
{
    private enum Transparency
    {
        Opaque,
        Cutout,
        Transparent
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
        foreach(var property in properties)
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
        materialEditor.RenderQueueField();
        //base.OnGUI(materialEditor, properties);
    }

    // Validate all properties when first assigning this shader to a material so we don't get things like ZWrite Off in opaque render mode.
    public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(material, oldShader, newShader);
        ValidateNewShader(material);
    }

    private void ValidateNewShader(Material material)
    {
        var materials = new Material[] { material };
        var properties = MaterialEditor.GetMaterialProperties(materials);
        var transparencyProperty = ShaderGUI.FindProperty("_Transparency", properties);
        ValidateTransparency(properties, material, (Transparency)transparencyProperty.floatValue);
        ValidateRenderQueue(properties, material, (Transparency)transparencyProperty.floatValue);
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

    private void ValidateTransparency(MaterialProperty[] properties, Material material, Transparency transparency)
    {
        var blendSrcProperty = ShaderGUI.FindProperty("_BlendSrc", properties);
        var blendDestProperty = ShaderGUI.FindProperty("_BlendDst", properties);

        material.DisableKeyword("_TRANSPARENCY_OPAQUE");
        material.DisableKeyword("_TRANSPARENCY_CUTOUT");
        material.DisableKeyword("_TRANSPARENCY_TRANSPARENT");
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
}