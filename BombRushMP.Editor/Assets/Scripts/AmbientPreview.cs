using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AmbientPreview : MonoBehaviour
{
    public static Color GlobalLightColor = Color.white;
    public static Color GlobalShadowColor = Color.gray;
    public Color LightColor;
    public Color ShadowColor;
    // Update is called once per frame
    void Update()
    {
        GlobalLightColor = LightColor;
        GlobalShadowColor = ShadowColor;
        Shader.SetGlobalColor("LightColor", LightColor);
        Shader.SetGlobalColor("ShadowColor", ShadowColor);
    }
}
