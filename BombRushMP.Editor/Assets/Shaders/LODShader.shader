Shader "All City Network/LODShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Front
            CGPROGRAM
            #define OUTLINE_MULTIPLIER 0.008
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
                float4 clipPos : SV_POSITION;
            };
            v2f vert(appdata v)
            {
                v2f o;
                float4 clipPos = UnityObjectToClipPos(v.vertex);
                o.clipPos = UnityObjectToClipPos(v.vertex + (v.normal * OUTLINE_MULTIPLIER));
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                half2 uv : TEXCOORD0;
                half3 normal : NORMAL;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                half3 normal : TEXCOORD1;
            };

            sampler2D _MainTex;
            half4 _MainTex_ST;
            half4 LightColor;
            half4 ShadowColor;
            half4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv) * _Color;
                clip(col.a - 0.9);
                half3 lightCol = LightColor.rgb * _LightColor0.rgb;
                half3 shadCol = ShadowColor.rgb * _LightColor0.rgb;
                float lightDot = saturate(dot(i.normal, _WorldSpaceLightPos0)) > 0.01 ? 1.0 : 0.0;
                col.rgb = lerp(col.rgb * shadCol, col.rgb * lightCol, lightDot);
                return col;
            }
            ENDCG
        }
    }
}
