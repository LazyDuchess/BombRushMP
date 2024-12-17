Shader "LD CrewBoom/Stony/ScreenSpaceShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Emission("Emission", 2D) = "black" {}  // Regular UV mapping
        _ScrollToggle("ScrollToggle", Float) = 1
        _ScrollMask("ScrollMask", 2D) = "white" {}
        _ScrollTex("ScrollTex", 2D) = "white" {}
        _ScrollEmission("Scroll Emission", 2D) = "black" {}
        _ScrollSize("ScrollSize", Vector) = (1,1,0,0)
        _ScrollOffset("ScrollOffset", Vector) = (0,0,0,0)
        _ScrollRotation("ScrollRotation", Range(0, 360)) = 0
        _ScrollSpeed("ScrollSpeed", Vector) = (0,1,0,0)
        _ScrollHue("ScrollHue", Range(0, 1)) = 0
        _ScrollHDR("ScrollHDR", Float) = 1
        _MaskTexCoord("MaskTexCoord", Vector) = (0,0,0,0) // Mask UV coordinate property
        
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineMultiplier("Outline Multiplier", Float) = 0.005
        _MinOutlineSize("Min Outline Multiplier", Float) = 0.002
        _MaxOutlineSize("Max Outline Multiplier", Float) = 0.008
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" }

        // Outline Pass
        Pass
        {
            Name "OUTLINE_PASS"
            Cull Front
            ZWrite On

            CGPROGRAM
            #pragma vertex vertOutline
            #pragma fragment fragOutline
            #include "UnityCG.cginc"

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

            v2f vertOutline(appdata v)
            {
                v2f o;
                float4 clipPos = UnityObjectToClipPos(v.vertex);
                float outlineMultiplier = clamp(clipPos.w * _OutlineMultiplier, _MinOutlineSize, _MaxOutlineSize);
                o.clipPos = UnityObjectToClipPos(v.vertex + (v.normal * outlineMultiplier));
                return o;
            }

            fixed4 fragOutline(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        // Texture Pass
        Pass
        {
            Name "TEXTURE_PASS"
            Tags { "LightMode" = "ForwardBase" }
            ZWrite On

            CGPROGRAM
            #define LIGHT_MULTIPLY 150.0
            #pragma vertex vertTexture
            #pragma fragment fragTexture
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            sampler2D _MainTex;
            sampler2D _Emission; // Regular emission (uses UV mapping)
            uniform sampler2D _ScrollMask;
            uniform sampler2D _ScrollTex;
            uniform float _ScrollHue;
            uniform float2 _ScrollSpeed;
            uniform float2 _ScrollSize;
            uniform float2 _ScrollOffset;
            uniform float _ScrollRotation;
            uniform float _ScrollHDR;
            uniform float _ScrollToggle;
            sampler2D _ScrollEmission; // Scrolling emission
            float4 LightColor;
            float4 ShadowColor;

            // Helper functions for HSV to RGB conversion
            float3 HSVToRGB(float3 c) {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            float3 RGBToHSV(float3 c) {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 maskUV : TEXCOORD1; // Mask UV coordinates
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 screenUV : TEXCOORD1; // Screen-space UV coordinates for scrolling textures
                float2 maskUV : TEXCOORD2; // Mask UV coordinates
                float3 normal : TEXCOORD3;
            };

            v2f vertTexture(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;               // Use regular UV mapping for _MainTex and _Emission
                o.screenUV = o.pos.xy / o.pos.w;  // Screen-space UVs for scrolling textures
                o.screenUV = o.screenUV * 0.5 + 0.5;    // Remap from [-1, 1] to [0, 1] for UVs
                o.maskUV = v.maskUV;        // Pass through mask UV coordinates for scrolling effects
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 fragTexture(v2f i) : SV_Target
            {
                // Sample the main texture using regular UV mapping
                fixed4 col = tex2D(_MainTex, i.uv);

                // Sample the regular emission texture using UV mapping
                col.rgb += tex2D(_Emission, i.uv).rgb;

                // Scroll emission and texture only if _ScrollToggle is enabled
                float3 scrollEmission = float3(0.0,0.0,0.0);

                if (_ScrollToggle > 0.0)
                {
                    // Scrolling functionality for screen-space effects using screen-space UVs
                    float2 uv_TexCoord = i.screenUV * _ScrollSize + _ScrollOffset;

                    float cosRotation = cos(radians(_ScrollRotation));
                    float sinRotation = sin(radians(_ScrollRotation));
                    float2 rotatedUV = mul(uv_TexCoord - float2(0.5, 0.5), float2x2(cosRotation, -sinRotation, sinRotation, cosRotation)) + float2(0.5, 0.5);

                    float2 scrolledUV = _Time.y * _ScrollSpeed + rotatedUV;

                    // HSV color manipulation for scrolling texture
                    float3 hsvTexColor = RGBToHSV(tex2D(_ScrollTex, scrolledUV).rgb);
                    float3 scrolledColor = HSVToRGB(float3((_ScrollHue + hsvTexColor.x), hsvTexColor.y, hsvTexColor.z));

                    float3 scrollEffect = tex2D(_ScrollMask, i.uv).r * scrolledColor * _ScrollHDR;
                    scrollEmission = tex2D(_ScrollEmission, scrolledUV).rgb;
                    scrollEmission *= tex2D(_ScrollMask, i.uv).r * scrolledColor * _ScrollHDR;
                    col.rgb += scrollEffect;
                }

                // Basic lighting
                fixed lighting = saturate(dot(i.normal, _WorldSpaceLightPos0) * LIGHT_MULTIPLY);
                float4 lightColor = lerp(ShadowColor, LightColor, lighting);
                col.rgb *= lightColor * _LightColor0.rgb;

                // Add scroll emission on top
                col.rgb += scrollEmission;

                return col;
            }
            ENDCG
        }
    }
}
