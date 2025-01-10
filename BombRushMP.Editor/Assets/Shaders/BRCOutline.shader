Shader "LD CrewBoom/Outline Only"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineMultiplier("Outline Multiplier", float) = 0.005
        _MinOutlineSize("Min Outline Multiplier", float) = 0.002
        _MaxOutlineSize("Max Outline Multiplier", float) = 0.008
    }
    SubShader
    {
        Tags { "LightMode"="ForwardBase" }
        LOD 100

        Pass
        {
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            fixed4 _OutlineColor;
            float _OutlineMultiplier;
            float _MinOutlineSize;
            float _MaxOutlineSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct v2f
            {
                float4 clipPos : SV_POSITION;
            };
            v2f vert(appdata v)
            {
                v2f o;
                float4 clipPos = UnityObjectToClipPos(v.vertex);
                float outlineMultiplier = clamp(clipPos.w * _OutlineMultiplier, _MinOutlineSize, _MaxOutlineSize);
                o.clipPos = UnityObjectToClipPos(v.vertex + (v.normal * outlineMultiplier));
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}
