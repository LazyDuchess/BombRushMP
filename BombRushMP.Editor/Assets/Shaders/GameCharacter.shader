Shader "LD CrewBoom/Game Character"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset] _Emission ("Emission", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "LightMode"="ForwardBase" }
        LOD 100

        Pass
        {
            Cull Front
            CGPROGRAM
            #define OUTLINE_MULTIPLIER 0.005
            #define MIN_OUTLINE_MULTIPLIER 0.002
            #define MAX_OUTLINE_MUTLIPLIER 0.008
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

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
                float outlineMultiplier = clamp(clipPos.w * OUTLINE_MULTIPLIER, MIN_OUTLINE_MULTIPLIER, MAX_OUTLINE_MUTLIPLIER);
                o.clipPos = UnityObjectToClipPos(v.vertex + (v.normal * outlineMultiplier));
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                return float4(0,0,0,1);
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "BRCCommon.cginc"
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
            };
            float4 LightColor;
            float4 ShadowColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Emission;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed lighting = saturate(dot(i.normal, _WorldSpaceLightPos0) * LIGHT_MULTIPLY);
                float4 lightColor = lerp(ShadowColor, LightColor, lighting);
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= lightColor * _LightColor0.rgb;
                fixed3 emissionCol = tex2D(_Emission, i.uv).rgb;
                col.rgb += emissionCol.rgb;
                return col;
            }
            ENDCG
        }
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
    CustomEditor "GameCharacterMaterialEditor"
}
