Shader "LD CrewBoom/Character"
{
    Properties
    {
        [HideInInspector] [Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc ("Blend mode Source", Int) = 1
        [HideInInspector] [Enum(UnityEngine.Rendering.BlendMode)] _BlendDst ("Blend mode Destination", Int) = 0
        [HideInInspector] [KeywordEnum(Opaque, Cutout, Transparent)] _Transparency ("Transparency", Float) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", Float) = 2
        [HideInInspector] _CutOut("Alpha Cutout", Range(0,1)) = 0.1

        [HideInInspector] [KeywordEnum(UV0, UV1, Screen)] _MainTexUV ("UV Map", Float) = 0
        [HideInInspector] [KeywordEnum(UV0, UV1, Screen)] _OverlayTexUV ("UV Map", Float) = 0
        [HideInInspector] [KeywordEnum(UV0, UV1, Screen)] _OverlayMaskUV ("UV Map", Float) = 0
        [HideInInspector] [KeywordEnum(UV0, UV1, Screen)] _EmissionUV ("UV Map", Float) = 0
        [HideInInspector] [KeywordEnum(UV0, UV1, Screen)] _EmissionMaskUV ("UV Map", Float) = 0
        [HideInInspector] [KeywordEnum(UV0, UV1, Screen)] _OutlineMaskUV ("UV Map", Float) = 0
        [HideInInspector] [Toggle] _MainTexScroll ("Scroll", Float) = 0
        [HideInInspector] [Toggle] _OverlayTexScroll ("Scroll", Float) = 0
        [HideInInspector] [Toggle] _OverlayMaskScroll ("Scroll", Float) = 0
        [HideInInspector] [Toggle] _EmissionScroll ("Scroll", Float) = 0
        [HideInInspector] [Toggle] _EmissionMaskScroll ("Scroll", Float) = 0
        [HideInInspector] [Toggle] _OutlineMaskScroll ("Scroll", Float) = 0

        [HideInInspector] _MainTexUSpeed ("U Speed", Float) = 0
        [HideInInspector] _MainTexVSpeed ("V Speed", Float) = 0

        [HideInInspector] _OverlayTexUSpeed ("U Speed", Float) = 0
        [HideInInspector] _OverlayTexVSpeed ("V Speed", Float) = 0

        [HideInInspector] _OverlayMaskUSpeed ("U Speed", Float) = 0
        [HideInInspector] _OverlayMaskVSpeed ("V Speed", Float) = 0

        [HideInInspector] _EmissionUSpeed ("U Speed", Float) = 0
        [HideInInspector] _EmissionVSpeed ("V Speed", Float) = 0

        [HideInInspector] _EmissionMaskUSpeed ("U Speed", Float) = 0
        [HideInInspector] _EmissionMaskVSpeed ("V Speed", Float) = 0

        [HideInInspector] _OutlineMaskUSpeed ("U Speed", Float) = 0
        [HideInInspector] _OutlineMaskVSpeed ("V Speed", Float) = 0

        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        [KeywordEnum(Mix, Add, Multiply)] _OverlayMode("Overlay Mode", Float) = 0
        _OverlayTex ("Overlay", 2D) = "black" {}
        _OverlayMask ("Overlay Mask", 2D) = "white" {}
        _Emission ("Emission", 2D) = "black" {}
        _EmissionMask ("Emission Mask", 2D) = "white" {}
        _OutlineMask ("Outline Mask", 2D) = "white" {}

        [Toggle] _Outline ("Outline", Float) = 1
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineMultiplier("Outline Multiplier", float) = 0.005
        _MinOutlineSize("Min Outline Multiplier", float) = 0.002
        _MaxOutlineSize("Max Outline Multiplier", float) = 0.008

        _ShadeCel("Shading Toon Falloff", Range(0,1.5)) = 1
        _ShadeShadowOffset("Shading Offset", Range(-1,1)) = 0
        _ShadeLightTint("Light Tint", Color) = (1,1,1,1)
        _ShadeShadowTint("Shadow Tint", Color) = (1,1,1,1)
        _ShadeEnvLight("Game Light Strength", Range(0,1)) = 1
        _ShadeEnvShadow("Game Shadow Strength", Range(0,1)) = 1
        _ShadeSunLight("Sun Light Strength", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "LightMode"="ForwardBase" }
        LOD 100

        Pass
        {
            Cull [_Cull]
            Blend [_BlendSrc] [_BlendDst]
            CGPROGRAM
            #pragma shader_feature _TRANSPARENCY_OPAQUE _TRANSPARENCY_CUTOUT
            #pragma shader_feature _MAINTEXSCROLL_ON
            #pragma shader_feature _OVERLAYTEXSCROLL_ON
            #pragma shader_feature _OVERLAYMASKSCROLL_ON
            #pragma shader_feature _EMISSIONSCROLL_ON
            #pragma shader_feature _EMISSIONMASKSCROLL_ON
            #pragma shader_feature _MAINTEXUV_UV0 _MAINTEXUV_UV1 _MAINTEXUV_SCREEN
            #pragma shader_feature _OVERLAYTEXUV_UV0 _OVERLAYTEXUV_UV1 _OVERLAYTEXUV_SCREEN
            #pragma shader_feature _OVERLAYMASKUV_UV0 _OVERLAYMASKUV_UV1 _OVERLAYMASKUV_SCREEN
            #pragma shader_feature _EMISSIONUV_UV0 _EMISSIONUV_UV1 _EMISSIONUV_SCREEN
            #pragma shader_feature _EMISSIONMASKUV_UV0 _EMISSIONMASKUV_UV1 _EMISSIONMASKUV_SCREEN
            #pragma shader_feature _OVERLAYMODE_MIX _OVERLAYMODE_ADD _OVERLAYMODE_MULTIPLY
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "BRCCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float3 normal : NORMAL;
                float4 color : COLOR0;
            };
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD2;
                float2 uv3 : TEXCOORD3;
                float2 uv4 : TEXCOORD4;
                float2 uv5 : TEXCOORD5;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float4 color : COLOR0;
            };
            float4 LightColor;
            float4 ShadowColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Emission;
            float4 _Emission_ST;
            sampler2D _EmissionMask;
            float4 _EmissionMask_ST;
            sampler2D _OverlayTex;
            float4 _OverlayTex_ST;
            sampler2D _OverlayMask;
            float4 _OverlayMask_ST;
            float4 _Color;
            float4 _ShadeLightTint;
            float4 _ShadeShadowTint;
            float _ShadeEnvLight;
            float _ShadeEnvShadow;

            #if _MAINTEXSCROLL_ON
            float _MainTexUSpeed;
            float _MainTexVSpeed;
            #endif

            #if _OVERLAYTEXSCROLL_ON
            float _OverlayTexUSpeed;
            float _OverlayTexVSpeed;
            #endif

            #if _OVERLAYMASKSCROLL_ON
            float _OverlayMaskUSpeed;
            float _OverlayMaskVSpeed;
            #endif

            #if _EMISSIONSCROLL_ON
            float _EmissionUSpeed;
            float _EmissionVSpeed;
            #endif

            #if _EMISSIONMASKSCROLL_ON
            float _EmissionMaskUSpeed;
            float _EmissionMaskVSpeed;
            #endif

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                #if _MAINTEXUV_UV0
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                #endif
                #if _MAINTEXUV_UV1
                o.uv = TRANSFORM_TEX(v.uv1, _MainTex);
                #endif
                #if _MAINTEXUV_SCREEN
                o.uv = TRANSFORM_TEX(UnityObjectToViewPos(v.vertex).xy, _MainTex);
                #endif

                #if _OVERLAYTEXUV_UV0
                o.uv4 = TRANSFORM_TEX(v.uv, _OverlayTex);
                #endif
                #if _OVERLAYTEXUV_UV1
                o.uv4 = TRANSFORM_TEX(v.uv1, _OverlayTex);
                #endif
                #if _OVERLAYTEXUV_SCREEN
                o.uv4 = TRANSFORM_TEX(UnityObjectToViewPos(v.vertex).xy, _OverlayTex);
                #endif

                #if _OVERLAYMASKUV_UV0
                o.uv5 = TRANSFORM_TEX(v.uv, _OverlayMask);
                #endif
                #if _OVERLAYMASKUV_UV1
                o.uv5 = TRANSFORM_TEX(v.uv1, _OverlayMask);
                #endif
                #if _OVERLAYMASKUV_SCREEN
                o.uv5 = TRANSFORM_TEX(UnityObjectToViewPos(v.vertex).xy, _OverlayMask);
                #endif

                #if _EMISSIONUV_UV0
                o.uv2 = TRANSFORM_TEX(v.uv, _Emission);
                #endif
                #if _EMISSIONUV_UV1
                o.uv2 = TRANSFORM_TEX(v.uv1, _Emission);
                #endif
                #if _EMISSIONUV_SCREEN
                o.uv2 = TRANSFORM_TEX(UnityObjectToViewPos(v.vertex).xy, _Emission);
                #endif

                #if _EMISSIONMASKUV_UV0
                o.uv3 = TRANSFORM_TEX(v.uv, _EmissionMask);
                #endif
                #if _EMISSIONMASKUV_UV1
                o.uv3 = TRANSFORM_TEX(v.uv1, _EmissionMask);
                #endif
                #if _EMISSIONMASKUV_SCREEN
                o.uv3 = TRANSFORM_TEX(UnityObjectToViewPos(v.vertex).xy, _EmissionMask);
                #endif

                #if _MAINTEXSCROLL_ON
                o.uv += float2(_MainTexUSpeed, _MainTexVSpeed) * _Time;
                #endif
                #if _EMISSIONSCROLL_ON
                o.uv2 += float2(_EmissionUSpeed, _EmissionVSpeed) * _Time;
                #endif
                #if _EMISSIONMASKSCROLL_ON
                o.uv3 += float2(_EmissionMaskUSpeed, _EmissionMaskVSpeed) * _Time;
                #endif
                #if _OVERLAYTEXSCROLL_ON
                o.uv4 += float2(_OverlayTexUSpeed, _OverlayTexVSpeed) * _Time;
                #endif
                #if _OVERLAYMASKSCROLL_ON
                o.uv5 += float2(_OverlayMaskUSpeed, _OverlayMaskVSpeed) * _Time;
                #endif

                o.normal = UnityObjectToWorldNormal(v.normal);
                o.color = v.color * _Color;
                return o;
            }

            #if _TRANSPARENCY_CUTOUT
            float _CutOut;
            #endif
            float _ShadeCel;
            float _ShadeSunLight;
            float _ShadeShadowOffset;

            fixed4 frag(v2f i) : SV_Target
            {
                float shadeLerp = pow(_ShadeCel, 4);
                //float shadeLerp = _ShadeCel;
                float smoothness = lerp(1, LIGHT_MULTIPLY, shadeLerp);
                fixed lighting = saturate((dot(i.normal, _WorldSpaceLightPos0) - _ShadeShadowOffset) * smoothness);
                float4 lColor = lerp(1, LightColor, _ShadeEnvLight) * _ShadeLightTint;
                float4 sColor = lerp(1, ShadowColor, _ShadeEnvShadow) * _ShadeShadowTint;
                float4 lightColor = lerp(sColor, lColor, lighting);
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 overlayCol = tex2D(_OverlayTex, i.uv4);
                overlayCol.a *= tex2D(_OverlayMask, i.uv5).r;
                #if _OVERLAYMODE_MIX
                col.rgb = lerp(col.rgb, overlayCol.rgb, overlayCol.a);
                #endif
                #if _OVERLAYMODE_ADD
                col.rgb += overlayCol.rgb * overlayCol.a;
                #endif
                #if _OVERLAYMODE_MULTIPLY
                col.rgb *= overlayCol.rgb * overlayCol.a;
                #endif
                col *= i.color;
                //col.a = 1.0;
                col.rgb *= lightColor * lerp(1, _LightColor0.rgb, _ShadeSunLight);
                fixed3 emissionCol = tex2D(_Emission, i.uv2).rgb * tex2D(_EmissionMask, i.uv3).r;
                col.rgb += emissionCol.rgb;
                #if _TRANSPARENCY_CUTOUT
                clip(col.a - _CutOut);
                #endif
                return col;
            }
            ENDCG
        }

        Pass
        {
            Cull Front
            Blend [_BlendSrc] [_BlendDst]
            CGPROGRAM
            #pragma shader_feature _OUTLINE_ON
            #pragma shader_feature _TRANSPARENCY_OPAQUE _TRANSPARENCY_CUTOUT TRANSPARENCY_TRANSPARENT
            #pragma shader_feature _MAINTEXSCROLL_ON
            #pragma shader_feature _MAINTEXUV_UV0 _MAINTEXUV_UV1 _MAINTEXUV_SCREEN
            #pragma shader_feature _OUTLINEMASKSCROLL_ON
            #pragma shader_feature _OUTLINEMASKUV_UV0 _OUTLINEMASKUV_UV1 _OUTLINEMASKUV_SCREEN
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            fixed4 _OutlineColor;
            float _OutlineMultiplier;
            float _MinOutlineSize;
            float _MaxOutlineSize;

            #if _TRANSPARENCY_CUTOUT
            #if _MAINTEXSCROLL_ON
            float _MainTexUSpeed;
            float _MainTexVSpeed;
            #endif
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _CutOut;
            float4 _Color;
            #endif
            sampler2D _OutlineMask;
            float4 _OutlineMask_ST;
            float _OutlineMaskUSpeed;
            float _OutlineMaskVSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                #if _TRANSPARENCY_CUTOUT
                float4 color : COLOR0;
                #endif
            };
            struct v2f
            {
                float4 clipPos : SV_POSITION;
                #if _TRANSPARENCY_CUTOUT
                float2 uv : TEXCOORD0;
                float4 color : COLOR0;
                #endif
            };
            v2f vert(appdata v)
            {
                v2f o;
                float4 clipPos = UnityObjectToClipPos(v.vertex);
                float outlineMultiplier = clamp(clipPos.w * _OutlineMultiplier, _MinOutlineSize, _MaxOutlineSize);
                float2 multiplierUv = float2(0,0);
                #if _OUTLINEMASKUV_UV0
                multiplierUv = TRANSFORM_TEX(v.uv, _OutlineMask);
                #endif
                #if _OUTLINEMASKUV_UV1
                multiplierUv = TRANSFORM_TEX(v.uv1, _OutlineMask);
                #endif
                #if _OUTLINEMASKUV_SCREEN
                multiplierUv = TRANSFORM_TEX(UnityObjectToViewPos(v.vertex).xy, _OutlineMask);
                #endif
                #if _OUTLINEMASKSCROLL_ON
                multiplierUv += float2(_OutlineMaskUSpeed, _OutlineMaskVSpeed) * _Time;
                #endif
                float outlineMask = tex2Dlod(_OutlineMask, float4(multiplierUv, 0, 0)).r;
                o.clipPos = UnityObjectToClipPos(v.vertex + (v.normal * outlineMultiplier * outlineMask));
                #if _OUTLINE_ON
                #if _TRANSPARENCY_CUTOUT
                #if _MAINTEXUV_UV0
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                #endif
                #if _MAINTEXUV_UV1
                o.uv = TRANSFORM_TEX(v.uv1, _MainTex);
                #endif
                #if _MAINTEXUV_SCREEN
                o.uv = TRANSFORM_TEX(UnityObjectToViewPos(v.vertex).xy, _MainTex);
                #endif

                #if _MAINTEXSCROLL_ON
                o.uv += float2(_MainTexUSpeed, _MainTexVSpeed) * _Time;
                #endif
                o.color = v.color * _Color;
                #endif
                #endif
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                #if _OUTLINE_ON
                #if _TRANSPARENCY_CUTOUT
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                clip(col.a - _CutOut);
                #endif
                #else
                clip(-1);
                #endif
                return _OutlineColor;
            }
            ENDCG
        }

        Pass
        {
            Tags {"LightMode" = "ShadowCaster"}

            CGPROGRAM
            #pragma shader_feature _TRANSPARENCY_OPAQUE _TRANSPARENCY_CUTOUT
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster

            #include "UnityCG.cginc"

                struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                V2F_SHADOW_CASTER;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.uv = v.uv;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            sampler2D _MainTex;
            float _CutOut;

            float4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                #if _TRANSPARENCY_CUTOUT
                    clip(col.a - _CutOut);
                #endif
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    CustomEditor "CharacterMaterialEditor"
}
