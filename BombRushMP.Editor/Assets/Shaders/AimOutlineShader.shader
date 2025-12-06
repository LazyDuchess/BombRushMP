Shader "All City Network/Aim Outline Shader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _OutlineThickness ("Outline Thickness", float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Front
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            half4 _Color;
            float _OutlineThickness;

            v2f vert (appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                worldPos += worldNormal * _OutlineThickness;
                o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}
