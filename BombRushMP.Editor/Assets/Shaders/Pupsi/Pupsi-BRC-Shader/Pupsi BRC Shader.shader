// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Pupsi BRC Shader"
{
	Properties
	{
		[NoScaleOffset]_MainTex("Base Texture", 2D) = "white" {}
		[NoScaleOffset]RotationMask("Base Rotation Mask", 2D) = "black" {}
		_BaseScrollSpeed("Base Scroll Speed", Vector) = (0,0,0,0)
		_BaseRotationCenter("Base Rotation Center", Vector) = (0,0,0,0)
		_BaseRotationSpeed("Base Rotation Speed", Float) = 0
		[Toggle(_NORMALMAPTOGGLE_ON)] _NormalMapToggle("Normal Map Toggle", Float) = 0
		[NoScaleOffset]_NormalMap("Normal Map", 2D) = "bump" {}
		[NoScaleOffset]_Emission("Emission Texture", 2D) = "black" {}
		_EmissionHue("Emission Hue", Range( 0 , 1)) = 0
		_EmissionEmit("Emission Emit", Float) = 1
		[Shadow Options][Toggle]_CustomLightingColorToggle("Custom Lighting Color Toggle", Float) = 0
		_CustomLightingColor("Custom Lighting Color", Color) = (1,1,1,0)
		[Shadow Options][Toggle]_CustomShadowColorToggle("Custom Shadow Color Toggle", Float) = 0
		_CustomShadowColor("Custom Shadow Color", Color) = (0,0,0,0)
		_ShadowSoftness("Shadow Softness", Range( 0.01 , 2)) = 0.01
		_ShadowOffset("Shadow Offset", Range( -1 , 1)) = 0
		[Toggle]_ShadowTextureToggle("Shadow Texture Toggle", Float) = 0
		[NoScaleOffset]_ShadowTexture("Shadow Texture", 2D) = "white" {}
		_ShadowTextureTiling("Shadow Texture Tiling", Vector) = (14,14,0,0)
		_ShadowTextureBlend("Shadow Texture Blend", Range( 0 , 1)) = 0.5
		[Toggle]_SpecularToggle("Specular Toggle", Float) = 0
		[Toggle]_SpecularCustomColorToggle("Specular Custom Color Toggle", Float) = 0
		_SpecularCustomColor("Specular Custom Color", Color) = (1,1,1,0)
		[NoScaleOffset]_SpecularMask("Specular Mask", 2D) = "white" {}
		_SpecularBrightness("Specular Brightness", Float) = 1
		_SpecularPower("Specular Power", Float) = 15
		[Toggle]_ExtraLightToggle("Extra Light Toggle", Float) = 0
		_ExtraLightOffset("Extra Light Offset", Range( -1 , 0)) = -0.6
		_ExtraLightBlend("Extra Light Blend", Range( 0 , 10)) = 2
		[Toggle]_RimLightShadowToggle("Rim Light/Shadow Toggle", Float) = 0
		_RimLightColor("Rim Light Color", Color) = (1,1,1,1)
		[Toggle]_RimLightBlendBaseTexture("Rim Light Blend Base Texture", Float) = 0
		_RimLightPower("Rim Light Power", Range( 0.01 , 10)) = 0.01
		_RimLightOffset("Rim Light Offset", Range( 0 , 1)) = 0.65
		_RimLightBlend("Rim Light Blend", Range( 0 , 25)) = 25
		_RimShadowPower("Rim Shadow Power", Range( 0.01 , 10)) = 10
		_RimShadowOffset("Rim Shadow Offset", Range( -1 , 1)) = 0.65
		_RimShadowOpacity("Rim Shadow Opacity", Range( 0 , 1)) = 0.6676344
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		[Toggle]_OutlineBlendBaseTexture("Outline Blend Base Texture", Float) = 0
		_OutlineMultiplier("Outline Multiplier", Float) = 0.005
		_OutlineMinSize("Outline Min Size", Float) = 0.002
		_OutlineMaxSize("Outline Max Size", Float) = 0.008
		[NoScaleOffset]_MainTex1("Screen Space Texture", 2D) = "white" {}
		[NoScaleOffset]RotationMask1("Screen Space Mask", 2D) = "black" {}
		[Toggle]_ScreenSpaceEmit("Screen Space Emit", Float) = 0
		[Toggle]_ScreenSpaceMaskisScreenSpace("Screen Space Mask is Screen Space", Float) = 0
		_ScreenSpaceTiling("Screen Space Tiling", Float) = 1
		_ScreenSpaceScrollYSpeed("Screen Space Scroll Y Speed", Float) = 0
		_ScreenSpaceScrollXSpeed("Screen Space Scroll X Speed", Float) = 0
		[Toggle]_CubemapToggle("Cubemap Toggle", Float) = 0
		[NoScaleOffset]_CubemapTexture("Cubemap Texture", CUBE) = "white" {}
		[NoScaleOffset]cubemapmask("Cubemap Mask", 2D) = "white" {}
		_CubemapBlend("Cubemap Blend", Range( 0 , 1)) = 1
		[Toggle(_GLOWTOGGLE_ON)] _GlowToggle("Glow Toggle", Float) = 0
		[NoScaleOffset]_GlowMask("Glow Mask", 2D) = "white" {}
		_GlowColor("Glow Color", Color) = (1,0,0,1)
		[Toggle]_GlowCycle("Glow Cycle", Float) = 1
		_GlowSpeed("Glow Speed", Float) = 10
		_GlowEmit("Glow Emit", Float) = 1
		[Toggle]_FlipbookToggle("Flipbook Toggle", Float) = 0
		[NoScaleOffset]_FlipBookTexture("FlipBookTexture", 2D) = "white" {}
		[NoScaleOffset]_FlipBookMask("FlipBookMask", 2D) = "white" {}
		_FlipbookTiling("Flipbook Tiling", Vector) = (1,1,0,0)
		_FlipbookOffset("Flipbook Offset", Vector) = (0,0,0,0)
		_FlipbookColumns("Flipbook Columns", Float) = 8
		_FlipbookRows("Flipbook Rows", Float) = 8
		_FlipbookSpeed("Flipbook Speed", Float) = 1
		_FlipbookEmit("Flipbook Emit", Float) = 1
		[Toggle(_SCROLLTOGGLE_ON)] _ScrollToggle("Scroll Toggle", Float) = 1
		[NoScaleOffset]_ScrollTex("Scroll Tex", 2D) = "white" {}
		[NoScaleOffset]_ScrollMask("Scroll Mask", 2D) = "white" {}
		_ScrollHue("Scroll Hue", Range( 0 , 1)) = 0
		_ScrollSize("Scroll Size", Vector) = (1,1,0,0)
		_ScrollOffset("Scroll Offset", Vector) = (0,0,0,0)
		_ScrollSpeed("Scroll Speed", Vector) = (0,1,0,0)
		_ScrollRotation("Scroll Rotation", Range( 0 , 360)) = 0
		_ScrollEmit("Scroll Emit", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		Tags { "RenderType"="Transparent" }
	LOD 0

		Cull Front
		CGINCLUDE
		#pragma target 4.5
		ENDCG

		
		Pass
		{
			
			Name "ForwardBase"
			Tags { "LightMode"="ForwardBase" }

			CGINCLUDE
			#pragma target 4.5
			ENDCG
			Blend Off
			AlphaToMask On
			Cull Off
			ColorMask RGBA
			ZWrite On
			ZTest LEqual
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityShaderVariables.cginc"
			#include "UnityStandardBRDF.cginc"
			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature_local _NORMALMAPTOGGLE_ON
			#pragma shader_feature_local _SCROLLTOGGLE_ON
			#pragma shader_feature_local _GLOWTOGGLE_ON

			//This is a late directive
			
			uniform float _CubemapToggle;
			uniform float _ScreenSpaceEmit;
			uniform float _SpecularToggle;
			uniform float _RimLightShadowToggle;
			uniform float _ShadowTextureToggle;
			uniform float _CustomShadowColorToggle;
			uniform float4 ShadowColor;
			uniform float4 _CustomShadowColor;
			uniform float _CustomLightingColorToggle;
			uniform float4 LightColor;
			uniform float4 _CustomLightingColor;
			uniform sampler2D _NormalMap;
			uniform float _ShadowOffset;
			uniform float _ShadowSoftness;
			uniform float _ExtraLightToggle;
			uniform float _ExtraLightOffset;
			uniform float _ExtraLightBlend;
			uniform sampler2D _MainTex;
			uniform float2 _BaseRotationCenter;
			uniform float _BaseRotationSpeed;
			uniform sampler2D RotationMask;
			uniform float2 _BaseScrollSpeed;
			uniform sampler2D RotationMask1;
			uniform float _ScreenSpaceMaskisScreenSpace;
			uniform sampler2D _MainTex1;
			uniform float _ScreenSpaceScrollXSpeed;
			uniform float _ScreenSpaceScrollYSpeed;
			uniform float _ScreenSpaceTiling;
			uniform sampler2D _ShadowTexture;
			uniform float2 _ShadowTextureTiling;
			uniform float _ShadowTextureBlend;
			uniform float _RimShadowOffset;
			uniform float _RimShadowPower;
			uniform float _RimShadowOpacity;
			uniform float _RimLightOffset;
			uniform float _RimLightPower;
			uniform float _RimLightBlendBaseTexture;
			uniform float4 _RimLightColor;
			uniform float _RimLightBlend;
			uniform float _FlipbookToggle;
			uniform sampler2D _FlipBookTexture;
			uniform float2 _FlipbookTiling;
			uniform float2 _FlipbookOffset;
			uniform float _FlipbookColumns;
			uniform float _FlipbookRows;
			uniform float _FlipbookSpeed;
			uniform sampler2D _FlipBookMask;
			uniform float _FlipbookEmit;
			uniform float _SpecularPower;
			uniform sampler2D _SpecularMask;
			uniform float _SpecularBrightness;
			uniform float _SpecularCustomColorToggle;
			uniform float4 _SpecularCustomColor;
			uniform samplerCUBE _CubemapTexture;
			uniform sampler2D cubemapmask;
			uniform float _CubemapBlend;
			uniform sampler2D _ScrollMask;
			uniform float _ScrollHue;
			uniform sampler2D _ScrollTex;
			uniform float2 _ScrollSpeed;
			uniform float2 _ScrollSize;
			uniform float2 _ScrollOffset;
			uniform float _ScrollRotation;
			uniform float _ScrollEmit;
			uniform float _EmissionHue;
			uniform sampler2D _Emission;
			uniform float _EmissionEmit;
			uniform float _GlowCycle;
			uniform float4 _GlowColor;
			uniform float _GlowSpeed;
			uniform sampler2D _GlowMask;
			uniform float _GlowEmit;
			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}


			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
			};

			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f,o);
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 ase_worldNormal = UnityObjectToWorldNormal(v.normal);
				o.ase_texcoord1.xyz = ase_worldNormal;
				float3 ase_worldTangent = UnityObjectToWorldDir(v.ase_tangent);
				o.ase_texcoord3.xyz = ase_worldTangent;
				float ase_vertexTangentSign = v.ase_tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord4.xyz = ase_worldBitangent;
				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
				o.ase_texcoord5.xyz = ase_worldPos;
				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord6 = screenPos;
				
				o.ase_texcoord2.xyz = v.ase_texcoord.xyz;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.w = 0;
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.w = 0;
				o.ase_texcoord5.w = 0;

				v.vertex.xyz +=  float3(0,0,0) ;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			float4 frag (v2f i ) : SV_Target
			{
				float3 outColor;
				float outAlpha;

				float3 ase_worldNormal = i.ase_texcoord1.xyz;
				float2 uv_NormalMap835 = i.ase_texcoord2.xyz.xy;
				float3 ase_worldTangent = i.ase_texcoord3.xyz;
				float3 ase_worldBitangent = i.ase_texcoord4.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 tanNormal834 = UnpackNormal( tex2D( _NormalMap, uv_NormalMap835 ) );
				float3 worldNormal834 = float3(dot(tanToWorld0,tanNormal834), dot(tanToWorld1,tanNormal834), dot(tanToWorld2,tanNormal834));
				#ifdef _NORMALMAPTOGGLE_ON
				float3 staticSwitch843 = worldNormal834;
				#else
				float3 staticSwitch843 = ase_worldNormal;
				#endif
				float3 normalizeResult170 = normalize( staticSwitch843 );
				float3 ase_worldPos = i.ase_texcoord5.xyz;
				float3 worldSpaceLightDir = UnityWorldSpaceLightDir(ase_worldPos);
				float dotResult54 = dot( normalizeResult170 , worldSpaceLightDir );
				float temp_output_57_0 = ( ( dotResult54 + _ShadowOffset ) / _ShadowSoftness );
				float4 lerpResult406 = lerp( (( _CustomShadowColorToggle )?( _CustomShadowColor ):( ShadowColor )) , (( _CustomLightingColorToggle )?( _CustomLightingColor ):( LightColor )) , saturate( temp_output_57_0 ));
				float4 lerpResult855 = lerp( float4( 0,0,0,0 ) , LightColor , saturate( ( ( dotResult54 + _ExtraLightOffset ) / _ShadowSoftness ) ));
				float2 texCoord671 = i.ase_texcoord2.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				float mulTime358 = _Time.y * _BaseRotationSpeed;
				float cos356 = cos( mulTime358 );
				float sin356 = sin( mulTime358 );
				float2 rotator356 = mul( texCoord671 - _BaseRotationCenter , float2x2( cos356 , -sin356 , sin356 , cos356 )) + _BaseRotationCenter;
				float2 uvRotationMask667 = i.ase_texcoord2.xyz.xy;
				float4 tex2DNode667 = tex2D( RotationMask, uvRotationMask667 );
				float2 texCoord559 = i.ase_texcoord2.xyz.xy * float2( 1,1 ) + ( _Time.y * _BaseScrollSpeed );
				float4 temp_output_666_0 = ( ( float4( rotator356, 0.0 , 0.0 ) * tex2DNode667 ) + ( float4( texCoord559, 0.0 , 0.0 ) * ( 1.0 - tex2DNode667 ) ) );
				float4 tex2DNode76 = tex2D( _MainTex, temp_output_666_0.rg );
				float4 screenPos = i.ase_texcoord6;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float4 tex2DNode998 = tex2D( RotationMask1, (( _ScreenSpaceMaskisScreenSpace )?( ase_screenPosNorm ):( temp_output_666_0 )).xy );
				float4 temp_output_1001_0 = ( 1.0 - tex2DNode998 );
				float mulTime962 = _Time.y * _ScreenSpaceScrollXSpeed;
				float mulTime968 = _Time.y * _ScreenSpaceScrollYSpeed;
				float4 appendResult965 = (float4(mulTime962 , mulTime968 , 0.0 , 0.0));
				float4 temp_output_964_0 = ( ase_screenPosNorm + appendResult965 );
				float4 temp_output_1033_0 = ( tex2D( _MainTex1, ( temp_output_964_0 * _ScreenSpaceTiling ).xy ) * tex2DNode998 );
				float4 temp_output_1034_0 = ( ( tex2DNode76 * temp_output_1001_0 ) + temp_output_1033_0 );
				float4 temp_output_158_0 = ( ( lerpResult406 + (( _ExtraLightToggle )?( ( lerpResult855 * _ExtraLightBlend ) ):( float4( 0,0,0,0 ) )) ) * temp_output_1034_0 );
				float4 blendOpSrc822 = saturate( ( 1.0 - ( ( 1.0 - temp_output_57_0 ) * tex2D( _ShadowTexture, ( ase_screenPosNorm * float4( _ShadowTextureTiling, 0.0 , 0.0 ) ).xy ) ) ) );
				float4 blendOpDest822 = temp_output_158_0;
				float4 lerpBlendMode822 = lerp(blendOpDest822,( blendOpSrc822 * blendOpDest822 ),_ShadowTextureBlend);
				#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
				float4 ase_lightColor = 0;
				#else //aselc
				float4 ase_lightColor = _LightColor0;
				#endif //aselc
				float4 temp_output_409_0 = ( (( _ShadowTextureToggle )?( ( saturate( lerpBlendMode822 )) ):( temp_output_158_0 )) * ase_lightColor );
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(ase_worldPos);
				ase_worldViewDir = Unity_SafeNormalize( ase_worldViewDir );
				float dotResult992 = dot( normalizedWorldNormal , ase_worldViewDir );
				float temp_output_981_0 = saturate( ( dotResult992 + _RimShadowOffset ) );
				float4 blendOpSrc994 = ( pow( temp_output_981_0 , _RimShadowPower ) * temp_output_409_0 );
				float4 blendOpDest994 = temp_output_409_0;
				float4 lerpBlendMode994 = lerp(blendOpDest994,min( blendOpSrc994 , blendOpDest994 ),_RimShadowOpacity);
				float dotResult423 = dot( normalizedWorldNormal , ase_worldViewDir );
				float2 texCoord631 = i.ase_texcoord2.xyz.xy * _FlipbookTiling + _FlipbookOffset;
				// *** BEGIN Flipbook UV Animation vars ***
				// Total tiles of Flipbook Texture
				float fbtotaltiles634 = _FlipbookColumns * _FlipbookRows;
				// Offsets for cols and rows of Flipbook Texture
				float fbcolsoffset634 = 1.0f / _FlipbookColumns;
				float fbrowsoffset634 = 1.0f / _FlipbookRows;
				// Speed of animation
				float fbspeed634 = _Time[ 1 ] * _FlipbookSpeed;
				// UV Tiling (col and row offset)
				float2 fbtiling634 = float2(fbcolsoffset634, fbrowsoffset634);
				// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
				// Calculate current tile linear index
				float fbcurrenttileindex634 = round( fmod( fbspeed634 + 0.0, fbtotaltiles634) );
				fbcurrenttileindex634 += ( fbcurrenttileindex634 < 0) ? fbtotaltiles634 : 0;
				// Obtain Offset X coordinate from current tile linear index
				float fblinearindextox634 = round ( fmod ( fbcurrenttileindex634, _FlipbookColumns ) );
				// Multiply Offset X by coloffset
				float fboffsetx634 = fblinearindextox634 * fbcolsoffset634;
				// Obtain Offset Y coordinate from current tile linear index
				float fblinearindextoy634 = round( fmod( ( fbcurrenttileindex634 - fblinearindextox634 ) / _FlipbookColumns, _FlipbookRows ) );
				// Reverse Y to get tiles from Top to Bottom
				fblinearindextoy634 = (int)(_FlipbookRows-1) - fblinearindextoy634;
				// Multiply Offset Y by rowoffset
				float fboffsety634 = fblinearindextoy634 * fbrowsoffset634;
				// UV Offset
				float2 fboffset634 = float2(fboffsetx634, fboffsety634);
				// Flipbook UV
				half2 fbuv634 = texCoord631 * fbtiling634 + fboffset634;
				// *** END Flipbook UV Animation vars ***
				float2 uv_FlipBookMask637 = i.ase_texcoord2.xyz.xy;
				float4 temp_output_642_0 = ( (( _RimLightShadowToggle )?( ( ( saturate( lerpBlendMode994 )) + ( ( pow( ( 1.0 - saturate( ( dotResult423 + _RimLightOffset ) ) ) , _RimLightPower ) * (( _RimLightBlendBaseTexture )?( temp_output_409_0 ):( _RimLightColor )) ) * _RimLightBlend ) ) ):( ( temp_output_409_0 + temp_output_1033_0 ) )) + float4( (( _FlipbookToggle )?( ( (tex2D( _FlipBookTexture, fbuv634 )).rgb * tex2D( _FlipBookMask, uv_FlipBookMask637 ).r * _FlipbookEmit ) ):( float3( 0,0,0 ) )) , 0.0 ) );
				float3 normalizeResult1050 = normalize( ( _WorldSpaceCameraPos - ase_worldPos ) );
				float3 normalizeResult1045 = normalize( ( worldSpaceLightDir + normalizeResult1050 ) );
				float dotResult1046 = dot( normalizeResult1045 , staticSwitch843 );
				float2 uv_SpecularMask1060 = i.ase_texcoord2.xyz.xy;
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 worldRefl877 = reflect( -ase_worldViewDir, float3( dot( tanToWorld0, staticSwitch843 ), dot( tanToWorld1, staticSwitch843 ), dot( tanToWorld2, staticSwitch843 ) ) );
				float2 uvcubemapmask881 = i.ase_texcoord2.xyz.xy;
				float2 uv_ScrollMask607 = i.ase_texcoord2.xyz.xy;
				float2 texCoord599 = i.ase_texcoord2.xyz.xy * _ScrollSize + _ScrollOffset;
				float cos598 = cos( radians( _ScrollRotation ) );
				float sin598 = sin( radians( _ScrollRotation ) );
				float2 rotator598 = mul( texCoord599 - float2( 0.5,0.5 ) , float2x2( cos598 , -sin598 , sin598 , cos598 )) + float2( 0.5,0.5 );
				float2 panner597 = ( 1.0 * _Time.y * _ScrollSpeed + rotator598);
				float3 hsvTorgb592 = RGBToHSV( tex2D( _ScrollTex, panner597 ).rgb );
				float3 hsvTorgb595 = HSVToRGB( float3(( _ScrollHue + hsvTorgb592.x ),hsvTorgb592.y,hsvTorgb592.z) );
				#ifdef _SCROLLTOGGLE_ON
				float3 staticSwitch591 = ( tex2D( _ScrollMask, uv_ScrollMask607 ).r * hsvTorgb595 * _ScrollEmit );
				#else
				float3 staticSwitch591 = float3( 0,0,0 );
				#endif
				float2 uv_Emission609 = i.ase_texcoord2.xyz.xy;
				float3 hsvTorgb584 = RGBToHSV( tex2D( _Emission, uv_Emission609 ).rgb );
				float3 hsvTorgb585 = HSVToRGB( float3(( _EmissionHue + hsvTorgb584.x ),hsvTorgb584.y,hsvTorgb584.z) );
				float mulTime619 = _Time.y * _GlowSpeed;
				float2 uv_GlowMask622 = i.ase_texcoord2.xyz.xy;
				float4 tex2DNode622 = tex2D( _GlowMask, uv_GlowMask622 );
				float3 hsvTorgb617 = HSVToRGB( float3(( mulTime619 * 0.1 ),1.0,1.0) );
				#ifdef _GLOWTOGGLE_ON
				float3 staticSwitch590 = ( (( _GlowCycle )?( ( hsvTorgb617 * tex2DNode622.r ) ):( ( (_GlowColor).rgb * (0.0 + (sin( mulTime619 ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) * tex2DNode622.r ) )) * _GlowEmit );
				#else
				float3 staticSwitch590 = float3( 0,0,0 );
				#endif
				

				outColor = ( (( _CubemapToggle )?( ( (( _ScreenSpaceEmit )?( ( (( _SpecularToggle )?( ( temp_output_642_0 + ( ( ( pow( saturate( dotResult1046 ) , _SpecularPower ) * tex2D( _SpecularMask, uv_SpecularMask1060 ) ) * _SpecularBrightness ) * (( _SpecularCustomColorToggle )?( _SpecularCustomColor ):( temp_output_409_0 )) ) ) ):( temp_output_642_0 )) + temp_output_1033_0 ) ):( (( _SpecularToggle )?( ( temp_output_642_0 + ( ( ( pow( saturate( dotResult1046 ) , _SpecularPower ) * tex2D( _SpecularMask, uv_SpecularMask1060 ) ) * _SpecularBrightness ) * (( _SpecularCustomColorToggle )?( _SpecularCustomColor ):( temp_output_409_0 )) ) ) ):( temp_output_642_0 )) )) + ( ( texCUBE( _CubemapTexture, worldRefl877 ) * tex2D( cubemapmask, uvcubemapmask881 ) ) * _CubemapBlend ) ) ):( (( _ScreenSpaceEmit )?( ( (( _SpecularToggle )?( ( temp_output_642_0 + ( ( ( pow( saturate( dotResult1046 ) , _SpecularPower ) * tex2D( _SpecularMask, uv_SpecularMask1060 ) ) * _SpecularBrightness ) * (( _SpecularCustomColorToggle )?( _SpecularCustomColor ):( temp_output_409_0 )) ) ) ):( temp_output_642_0 )) + temp_output_1033_0 ) ):( (( _SpecularToggle )?( ( temp_output_642_0 + ( ( ( pow( saturate( dotResult1046 ) , _SpecularPower ) * tex2D( _SpecularMask, uv_SpecularMask1060 ) ) * _SpecularBrightness ) * (( _SpecularCustomColorToggle )?( _SpecularCustomColor ):( temp_output_409_0 )) ) ) ):( temp_output_642_0 )) )) )) + float4( ( staticSwitch591 + ( hsvTorgb585 * _EmissionEmit ) + staticSwitch590 ) , 0.0 ) ).rgb;
				outAlpha = tex2DNode76.a;
				clip(outAlpha);
				return float4(outColor,outAlpha);
			}
			ENDCG
		}

		
		Pass
		{
			Name "ForwardAdd"
			Tags { "LightMode"="ForwardAdd" }
			ZWrite Off
			Blend One One
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdadd_fullshadows
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityShaderVariables.cginc"
			#include "UnityStandardBRDF.cginc"
			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature_local _NORMALMAPTOGGLE_ON
			#pragma shader_feature_local _SCROLLTOGGLE_ON
			#pragma shader_feature_local _GLOWTOGGLE_ON

			//This is a late directive
			
			uniform float _CubemapToggle;
			uniform float _ScreenSpaceEmit;
			uniform float _SpecularToggle;
			uniform float _RimLightShadowToggle;
			uniform float _ShadowTextureToggle;
			uniform float _CustomShadowColorToggle;
			uniform float4 ShadowColor;
			uniform float4 _CustomShadowColor;
			uniform float _CustomLightingColorToggle;
			uniform float4 LightColor;
			uniform float4 _CustomLightingColor;
			uniform sampler2D _NormalMap;
			uniform float _ShadowOffset;
			uniform float _ShadowSoftness;
			uniform float _ExtraLightToggle;
			uniform float _ExtraLightOffset;
			uniform float _ExtraLightBlend;
			uniform sampler2D _MainTex;
			uniform float2 _BaseRotationCenter;
			uniform float _BaseRotationSpeed;
			uniform sampler2D RotationMask;
			uniform float2 _BaseScrollSpeed;
			uniform sampler2D RotationMask1;
			uniform float _ScreenSpaceMaskisScreenSpace;
			uniform sampler2D _MainTex1;
			uniform float _ScreenSpaceScrollXSpeed;
			uniform float _ScreenSpaceScrollYSpeed;
			uniform float _ScreenSpaceTiling;
			uniform sampler2D _ShadowTexture;
			uniform float2 _ShadowTextureTiling;
			uniform float _ShadowTextureBlend;
			uniform float _RimShadowOffset;
			uniform float _RimShadowPower;
			uniform float _RimShadowOpacity;
			uniform float _RimLightOffset;
			uniform float _RimLightPower;
			uniform float _RimLightBlendBaseTexture;
			uniform float4 _RimLightColor;
			uniform float _RimLightBlend;
			uniform float _FlipbookToggle;
			uniform sampler2D _FlipBookTexture;
			uniform float2 _FlipbookTiling;
			uniform float2 _FlipbookOffset;
			uniform float _FlipbookColumns;
			uniform float _FlipbookRows;
			uniform float _FlipbookSpeed;
			uniform sampler2D _FlipBookMask;
			uniform float _FlipbookEmit;
			uniform float _SpecularPower;
			uniform sampler2D _SpecularMask;
			uniform float _SpecularBrightness;
			uniform float _SpecularCustomColorToggle;
			uniform float4 _SpecularCustomColor;
			uniform samplerCUBE _CubemapTexture;
			uniform sampler2D cubemapmask;
			uniform float _CubemapBlend;
			uniform sampler2D _ScrollMask;
			uniform float _ScrollHue;
			uniform sampler2D _ScrollTex;
			uniform float2 _ScrollSpeed;
			uniform float2 _ScrollSize;
			uniform float2 _ScrollOffset;
			uniform float _ScrollRotation;
			uniform float _ScrollEmit;
			uniform float _EmissionHue;
			uniform sampler2D _Emission;
			uniform float _EmissionEmit;
			uniform float _GlowCycle;
			uniform float4 _GlowColor;
			uniform float _GlowSpeed;
			uniform sampler2D _GlowMask;
			uniform float _GlowEmit;
			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}


			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
			};

			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f,o);
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 ase_worldNormal = UnityObjectToWorldNormal(v.normal);
				o.ase_texcoord1.xyz = ase_worldNormal;
				float3 ase_worldTangent = UnityObjectToWorldDir(v.ase_tangent);
				o.ase_texcoord3.xyz = ase_worldTangent;
				float ase_vertexTangentSign = v.ase_tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord4.xyz = ase_worldBitangent;
				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
				o.ase_texcoord5.xyz = ase_worldPos;
				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord6 = screenPos;
				
				o.ase_texcoord2.xyz = v.ase_texcoord.xyz;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.w = 0;
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.w = 0;
				o.ase_texcoord5.w = 0;

				v.vertex.xyz +=  float3(0,0,0) ;
				o.pos = UnityObjectToClipPos(v.vertex);
				#if ASE_SHADOWS
					#if UNITY_VERSION >= 560
						UNITY_TRANSFER_SHADOW( o, v.texcoord );
					#else
						TRANSFER_SHADOW( o );
					#endif
				#endif
				return o;
			}

			float4 frag (v2f i ) : SV_Target
			{
				float3 outColor;
				float outAlpha;

				float3 ase_worldNormal = i.ase_texcoord1.xyz;
				float2 uv_NormalMap835 = i.ase_texcoord2.xyz.xy;
				float3 ase_worldTangent = i.ase_texcoord3.xyz;
				float3 ase_worldBitangent = i.ase_texcoord4.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 tanNormal834 = UnpackNormal( tex2D( _NormalMap, uv_NormalMap835 ) );
				float3 worldNormal834 = float3(dot(tanToWorld0,tanNormal834), dot(tanToWorld1,tanNormal834), dot(tanToWorld2,tanNormal834));
				#ifdef _NORMALMAPTOGGLE_ON
				float3 staticSwitch843 = worldNormal834;
				#else
				float3 staticSwitch843 = ase_worldNormal;
				#endif
				float3 normalizeResult170 = normalize( staticSwitch843 );
				float3 ase_worldPos = i.ase_texcoord5.xyz;
				float3 worldSpaceLightDir = UnityWorldSpaceLightDir(ase_worldPos);
				float dotResult54 = dot( normalizeResult170 , worldSpaceLightDir );
				float temp_output_57_0 = ( ( dotResult54 + _ShadowOffset ) / _ShadowSoftness );
				float4 lerpResult406 = lerp( (( _CustomShadowColorToggle )?( _CustomShadowColor ):( ShadowColor )) , (( _CustomLightingColorToggle )?( _CustomLightingColor ):( LightColor )) , saturate( temp_output_57_0 ));
				float4 lerpResult855 = lerp( float4( 0,0,0,0 ) , LightColor , saturate( ( ( dotResult54 + _ExtraLightOffset ) / _ShadowSoftness ) ));
				float2 texCoord671 = i.ase_texcoord2.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				float mulTime358 = _Time.y * _BaseRotationSpeed;
				float cos356 = cos( mulTime358 );
				float sin356 = sin( mulTime358 );
				float2 rotator356 = mul( texCoord671 - _BaseRotationCenter , float2x2( cos356 , -sin356 , sin356 , cos356 )) + _BaseRotationCenter;
				float2 uvRotationMask667 = i.ase_texcoord2.xyz.xy;
				float4 tex2DNode667 = tex2D( RotationMask, uvRotationMask667 );
				float2 texCoord559 = i.ase_texcoord2.xyz.xy * float2( 1,1 ) + ( _Time.y * _BaseScrollSpeed );
				float4 temp_output_666_0 = ( ( float4( rotator356, 0.0 , 0.0 ) * tex2DNode667 ) + ( float4( texCoord559, 0.0 , 0.0 ) * ( 1.0 - tex2DNode667 ) ) );
				float4 tex2DNode76 = tex2D( _MainTex, temp_output_666_0.rg );
				float4 screenPos = i.ase_texcoord6;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float4 tex2DNode998 = tex2D( RotationMask1, (( _ScreenSpaceMaskisScreenSpace )?( ase_screenPosNorm ):( temp_output_666_0 )).xy );
				float4 temp_output_1001_0 = ( 1.0 - tex2DNode998 );
				float mulTime962 = _Time.y * _ScreenSpaceScrollXSpeed;
				float mulTime968 = _Time.y * _ScreenSpaceScrollYSpeed;
				float4 appendResult965 = (float4(mulTime962 , mulTime968 , 0.0 , 0.0));
				float4 temp_output_964_0 = ( ase_screenPosNorm + appendResult965 );
				float4 temp_output_1033_0 = ( tex2D( _MainTex1, ( temp_output_964_0 * _ScreenSpaceTiling ).xy ) * tex2DNode998 );
				float4 temp_output_1034_0 = ( ( tex2DNode76 * temp_output_1001_0 ) + temp_output_1033_0 );
				float4 temp_output_158_0 = ( ( lerpResult406 + (( _ExtraLightToggle )?( ( lerpResult855 * _ExtraLightBlend ) ):( float4( 0,0,0,0 ) )) ) * temp_output_1034_0 );
				float4 blendOpSrc822 = saturate( ( 1.0 - ( ( 1.0 - temp_output_57_0 ) * tex2D( _ShadowTexture, ( ase_screenPosNorm * float4( _ShadowTextureTiling, 0.0 , 0.0 ) ).xy ) ) ) );
				float4 blendOpDest822 = temp_output_158_0;
				float4 lerpBlendMode822 = lerp(blendOpDest822,( blendOpSrc822 * blendOpDest822 ),_ShadowTextureBlend);
				#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
				float4 ase_lightColor = 0;
				#else //aselc
				float4 ase_lightColor = _LightColor0;
				#endif //aselc
				float4 temp_output_409_0 = ( (( _ShadowTextureToggle )?( ( saturate( lerpBlendMode822 )) ):( temp_output_158_0 )) * ase_lightColor );
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(ase_worldPos);
				ase_worldViewDir = Unity_SafeNormalize( ase_worldViewDir );
				float dotResult992 = dot( normalizedWorldNormal , ase_worldViewDir );
				float temp_output_981_0 = saturate( ( dotResult992 + _RimShadowOffset ) );
				float4 blendOpSrc994 = ( pow( temp_output_981_0 , _RimShadowPower ) * temp_output_409_0 );
				float4 blendOpDest994 = temp_output_409_0;
				float4 lerpBlendMode994 = lerp(blendOpDest994,min( blendOpSrc994 , blendOpDest994 ),_RimShadowOpacity);
				float dotResult423 = dot( normalizedWorldNormal , ase_worldViewDir );
				float2 texCoord631 = i.ase_texcoord2.xyz.xy * _FlipbookTiling + _FlipbookOffset;
				// *** BEGIN Flipbook UV Animation vars ***
				// Total tiles of Flipbook Texture
				float fbtotaltiles634 = _FlipbookColumns * _FlipbookRows;
				// Offsets for cols and rows of Flipbook Texture
				float fbcolsoffset634 = 1.0f / _FlipbookColumns;
				float fbrowsoffset634 = 1.0f / _FlipbookRows;
				// Speed of animation
				float fbspeed634 = _Time[ 1 ] * _FlipbookSpeed;
				// UV Tiling (col and row offset)
				float2 fbtiling634 = float2(fbcolsoffset634, fbrowsoffset634);
				// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
				// Calculate current tile linear index
				float fbcurrenttileindex634 = round( fmod( fbspeed634 + 0.0, fbtotaltiles634) );
				fbcurrenttileindex634 += ( fbcurrenttileindex634 < 0) ? fbtotaltiles634 : 0;
				// Obtain Offset X coordinate from current tile linear index
				float fblinearindextox634 = round ( fmod ( fbcurrenttileindex634, _FlipbookColumns ) );
				// Multiply Offset X by coloffset
				float fboffsetx634 = fblinearindextox634 * fbcolsoffset634;
				// Obtain Offset Y coordinate from current tile linear index
				float fblinearindextoy634 = round( fmod( ( fbcurrenttileindex634 - fblinearindextox634 ) / _FlipbookColumns, _FlipbookRows ) );
				// Reverse Y to get tiles from Top to Bottom
				fblinearindextoy634 = (int)(_FlipbookRows-1) - fblinearindextoy634;
				// Multiply Offset Y by rowoffset
				float fboffsety634 = fblinearindextoy634 * fbrowsoffset634;
				// UV Offset
				float2 fboffset634 = float2(fboffsetx634, fboffsety634);
				// Flipbook UV
				half2 fbuv634 = texCoord631 * fbtiling634 + fboffset634;
				// *** END Flipbook UV Animation vars ***
				float2 uv_FlipBookMask637 = i.ase_texcoord2.xyz.xy;
				float4 temp_output_642_0 = ( (( _RimLightShadowToggle )?( ( ( saturate( lerpBlendMode994 )) + ( ( pow( ( 1.0 - saturate( ( dotResult423 + _RimLightOffset ) ) ) , _RimLightPower ) * (( _RimLightBlendBaseTexture )?( temp_output_409_0 ):( _RimLightColor )) ) * _RimLightBlend ) ) ):( ( temp_output_409_0 + temp_output_1033_0 ) )) + float4( (( _FlipbookToggle )?( ( (tex2D( _FlipBookTexture, fbuv634 )).rgb * tex2D( _FlipBookMask, uv_FlipBookMask637 ).r * _FlipbookEmit ) ):( float3( 0,0,0 ) )) , 0.0 ) );
				float3 normalizeResult1050 = normalize( ( _WorldSpaceCameraPos - ase_worldPos ) );
				float3 normalizeResult1045 = normalize( ( worldSpaceLightDir + normalizeResult1050 ) );
				float dotResult1046 = dot( normalizeResult1045 , staticSwitch843 );
				float2 uv_SpecularMask1060 = i.ase_texcoord2.xyz.xy;
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 worldRefl877 = reflect( -ase_worldViewDir, float3( dot( tanToWorld0, staticSwitch843 ), dot( tanToWorld1, staticSwitch843 ), dot( tanToWorld2, staticSwitch843 ) ) );
				float2 uvcubemapmask881 = i.ase_texcoord2.xyz.xy;
				float2 uv_ScrollMask607 = i.ase_texcoord2.xyz.xy;
				float2 texCoord599 = i.ase_texcoord2.xyz.xy * _ScrollSize + _ScrollOffset;
				float cos598 = cos( radians( _ScrollRotation ) );
				float sin598 = sin( radians( _ScrollRotation ) );
				float2 rotator598 = mul( texCoord599 - float2( 0.5,0.5 ) , float2x2( cos598 , -sin598 , sin598 , cos598 )) + float2( 0.5,0.5 );
				float2 panner597 = ( 1.0 * _Time.y * _ScrollSpeed + rotator598);
				float3 hsvTorgb592 = RGBToHSV( tex2D( _ScrollTex, panner597 ).rgb );
				float3 hsvTorgb595 = HSVToRGB( float3(( _ScrollHue + hsvTorgb592.x ),hsvTorgb592.y,hsvTorgb592.z) );
				#ifdef _SCROLLTOGGLE_ON
				float3 staticSwitch591 = ( tex2D( _ScrollMask, uv_ScrollMask607 ).r * hsvTorgb595 * _ScrollEmit );
				#else
				float3 staticSwitch591 = float3( 0,0,0 );
				#endif
				float2 uv_Emission609 = i.ase_texcoord2.xyz.xy;
				float3 hsvTorgb584 = RGBToHSV( tex2D( _Emission, uv_Emission609 ).rgb );
				float3 hsvTorgb585 = HSVToRGB( float3(( _EmissionHue + hsvTorgb584.x ),hsvTorgb584.y,hsvTorgb584.z) );
				float mulTime619 = _Time.y * _GlowSpeed;
				float2 uv_GlowMask622 = i.ase_texcoord2.xyz.xy;
				float4 tex2DNode622 = tex2D( _GlowMask, uv_GlowMask622 );
				float3 hsvTorgb617 = HSVToRGB( float3(( mulTime619 * 0.1 ),1.0,1.0) );
				#ifdef _GLOWTOGGLE_ON
				float3 staticSwitch590 = ( (( _GlowCycle )?( ( hsvTorgb617 * tex2DNode622.r ) ):( ( (_GlowColor).rgb * (0.0 + (sin( mulTime619 ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) * tex2DNode622.r ) )) * _GlowEmit );
				#else
				float3 staticSwitch590 = float3( 0,0,0 );
				#endif
				

				outColor = ( (( _CubemapToggle )?( ( (( _ScreenSpaceEmit )?( ( (( _SpecularToggle )?( ( temp_output_642_0 + ( ( ( pow( saturate( dotResult1046 ) , _SpecularPower ) * tex2D( _SpecularMask, uv_SpecularMask1060 ) ) * _SpecularBrightness ) * (( _SpecularCustomColorToggle )?( _SpecularCustomColor ):( temp_output_409_0 )) ) ) ):( temp_output_642_0 )) + temp_output_1033_0 ) ):( (( _SpecularToggle )?( ( temp_output_642_0 + ( ( ( pow( saturate( dotResult1046 ) , _SpecularPower ) * tex2D( _SpecularMask, uv_SpecularMask1060 ) ) * _SpecularBrightness ) * (( _SpecularCustomColorToggle )?( _SpecularCustomColor ):( temp_output_409_0 )) ) ) ):( temp_output_642_0 )) )) + ( ( texCUBE( _CubemapTexture, worldRefl877 ) * tex2D( cubemapmask, uvcubemapmask881 ) ) * _CubemapBlend ) ) ):( (( _ScreenSpaceEmit )?( ( (( _SpecularToggle )?( ( temp_output_642_0 + ( ( ( pow( saturate( dotResult1046 ) , _SpecularPower ) * tex2D( _SpecularMask, uv_SpecularMask1060 ) ) * _SpecularBrightness ) * (( _SpecularCustomColorToggle )?( _SpecularCustomColor ):( temp_output_409_0 )) ) ) ):( temp_output_642_0 )) + temp_output_1033_0 ) ):( (( _SpecularToggle )?( ( temp_output_642_0 + ( ( ( pow( saturate( dotResult1046 ) , _SpecularPower ) * tex2D( _SpecularMask, uv_SpecularMask1060 ) ) * _SpecularBrightness ) * (( _SpecularCustomColorToggle )?( _SpecularCustomColor ):( temp_output_409_0 )) ) ) ):( temp_output_642_0 )) )) )) + float4( ( staticSwitch591 + ( hsvTorgb585 * _EmissionEmit ) + staticSwitch590 ) , 0.0 ) ).rgb;
				outAlpha = tex2DNode76.a;
				clip(outAlpha);
				return float4(outColor,outAlpha);
			}
			ENDCG
		}

		
		Pass
		{
			Name "Deferred"
			Tags { "LightMode"="Deferred" }

			CGINCLUDE
			#pragma target 3.0
			ENDCG
			Blend Off
			AlphaToMask Off
			Cull Back
			ColorMask RGBA
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_prepassfinal
			#include "UnityCG.cginc"
			
			

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};

			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f,o);
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				

				v.vertex.xyz +=  float3(0,0,0) ;
				o.pos = UnityObjectToClipPos(v.vertex);
				#if ASE_SHADOWS
					#if UNITY_VERSION >= 560
						UNITY_TRANSFER_SHADOW( o, v.texcoord );
					#else
						TRANSFER_SHADOW( o );
					#endif
				#endif
				return o;
			}

			void frag (v2f i , out half4 outGBuffer0 : SV_Target0, out half4 outGBuffer1 : SV_Target1, out half4 outGBuffer2 : SV_Target2, out half4 outGBuffer3 : SV_Target3)
			{
				

				outGBuffer0 = 0;
				outGBuffer1 = 0;
				outGBuffer2 = 0;
				outGBuffer3 = 0;
			}
			ENDCG
		}
		
		Pass
		{
			Name "Outline"
			Tags { "CullMode"="Front" }
			CGINCLUDE
			#pragma target 4.5
			ENDCG
			Blend Off
			AlphaToMask On
			Cull Front
			ColorMask RGBA
			ZWrite On
			ZTest LEqual
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature_local _NORMALMAPTOGGLE_ON


			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
			};

			//This is a late directive
			
			uniform float _OutlineMultiplier;
			uniform float _OutlineMinSize;
			uniform float _OutlineMaxSize;
			uniform float _OutlineBlendBaseTexture;
			uniform float4 _OutlineColor;
			uniform float _CustomShadowColorToggle;
			uniform float4 ShadowColor;
			uniform float4 _CustomShadowColor;
			uniform float _CustomLightingColorToggle;
			uniform float4 LightColor;
			uniform float4 _CustomLightingColor;
			uniform sampler2D _NormalMap;
			uniform float _ShadowOffset;
			uniform float _ShadowSoftness;
			uniform float _ExtraLightToggle;
			uniform float _ExtraLightOffset;
			uniform float _ExtraLightBlend;
			uniform sampler2D _MainTex;
			uniform float2 _BaseRotationCenter;
			uniform float _BaseRotationSpeed;
			uniform sampler2D RotationMask;
			uniform float2 _BaseScrollSpeed;
			uniform sampler2D RotationMask1;
			uniform float _ScreenSpaceMaskisScreenSpace;
			uniform sampler2D _MainTex1;
			uniform float _ScreenSpaceScrollXSpeed;
			uniform float _ScreenSpaceScrollYSpeed;
			uniform float _ScreenSpaceTiling;


			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				float3 objectToViewPos = UnityObjectToViewPos(v.vertex.xyz);
				float eyeDepth = -objectToViewPos.z;
				float cameraDepthFade977 = (( eyeDepth -_ProjectionParams.y - 0.0 ) / 1.0);
				float clampResult893 = clamp( ( _OutlineMultiplier * cameraDepthFade977 ) , _OutlineMinSize , _OutlineMaxSize );
				
				float3 ase_worldNormal = UnityObjectToWorldNormal(v.ase_normal);
				o.ase_texcoord.xyz = ase_worldNormal;
				float3 ase_worldTangent = UnityObjectToWorldDir(v.ase_tangent);
				o.ase_texcoord2.xyz = ase_worldTangent;
				float ase_vertexTangentSign = v.ase_tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord3.xyz = ase_worldBitangent;
				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
				o.ase_texcoord4.xyz = ase_worldPos;
				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord5 = screenPos;
				
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.w = 0;
				o.ase_texcoord1.zw = 0;
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.w = 0;

				v.vertex.xyz += ( v.ase_normal * clampResult893 );
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			float4 frag (v2f i ) : SV_Target
			{
				float3 outOutlineColor;
				float outOutlineAlpha;

				float3 ase_worldNormal = i.ase_texcoord.xyz;
				float2 uv_NormalMap835 = i.ase_texcoord1.xy;
				float3 ase_worldTangent = i.ase_texcoord2.xyz;
				float3 ase_worldBitangent = i.ase_texcoord3.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 tanNormal834 = UnpackNormal( tex2D( _NormalMap, uv_NormalMap835 ) );
				float3 worldNormal834 = float3(dot(tanToWorld0,tanNormal834), dot(tanToWorld1,tanNormal834), dot(tanToWorld2,tanNormal834));
				#ifdef _NORMALMAPTOGGLE_ON
				float3 staticSwitch843 = worldNormal834;
				#else
				float3 staticSwitch843 = ase_worldNormal;
				#endif
				float3 normalizeResult170 = normalize( staticSwitch843 );
				float3 ase_worldPos = i.ase_texcoord4.xyz;
				float3 worldSpaceLightDir = UnityWorldSpaceLightDir(ase_worldPos);
				float dotResult54 = dot( normalizeResult170 , worldSpaceLightDir );
				float temp_output_57_0 = ( ( dotResult54 + _ShadowOffset ) / _ShadowSoftness );
				float4 lerpResult406 = lerp( (( _CustomShadowColorToggle )?( _CustomShadowColor ):( ShadowColor )) , (( _CustomLightingColorToggle )?( _CustomLightingColor ):( LightColor )) , saturate( temp_output_57_0 ));
				float4 lerpResult855 = lerp( float4( 0,0,0,0 ) , LightColor , saturate( ( ( dotResult54 + _ExtraLightOffset ) / _ShadowSoftness ) ));
				float2 texCoord671 = i.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float mulTime358 = _Time.y * _BaseRotationSpeed;
				float cos356 = cos( mulTime358 );
				float sin356 = sin( mulTime358 );
				float2 rotator356 = mul( texCoord671 - _BaseRotationCenter , float2x2( cos356 , -sin356 , sin356 , cos356 )) + _BaseRotationCenter;
				float2 uvRotationMask667 = i.ase_texcoord1.xy;
				float4 tex2DNode667 = tex2D( RotationMask, uvRotationMask667 );
				float2 texCoord559 = i.ase_texcoord1.xy * float2( 1,1 ) + ( _Time.y * _BaseScrollSpeed );
				float4 temp_output_666_0 = ( ( float4( rotator356, 0.0 , 0.0 ) * tex2DNode667 ) + ( float4( texCoord559, 0.0 , 0.0 ) * ( 1.0 - tex2DNode667 ) ) );
				float4 tex2DNode76 = tex2D( _MainTex, temp_output_666_0.rg );
				float4 screenPos = i.ase_texcoord5;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float4 tex2DNode998 = tex2D( RotationMask1, (( _ScreenSpaceMaskisScreenSpace )?( ase_screenPosNorm ):( temp_output_666_0 )).xy );
				float4 temp_output_1001_0 = ( 1.0 - tex2DNode998 );
				float mulTime962 = _Time.y * _ScreenSpaceScrollXSpeed;
				float mulTime968 = _Time.y * _ScreenSpaceScrollYSpeed;
				float4 appendResult965 = (float4(mulTime962 , mulTime968 , 0.0 , 0.0));
				float4 temp_output_964_0 = ( ase_screenPosNorm + appendResult965 );
				float4 temp_output_1033_0 = ( tex2D( _MainTex1, ( temp_output_964_0 * _ScreenSpaceTiling ).xy ) * tex2DNode998 );
				float4 temp_output_1034_0 = ( ( tex2DNode76 * temp_output_1001_0 ) + temp_output_1033_0 );
				float4 temp_output_158_0 = ( ( lerpResult406 + (( _ExtraLightToggle )?( ( lerpResult855 * _ExtraLightBlend ) ):( float4( 0,0,0,0 ) )) ) * temp_output_1034_0 );
				

				outOutlineColor = (( _OutlineBlendBaseTexture )?( ( temp_output_158_0 * _OutlineColor ) ):( _OutlineColor )).rgb;
				outOutlineAlpha = tex2DNode76.a;
				clip(outOutlineAlpha);
				return float4(outOutlineColor,outOutlineAlpha);
			}
			ENDCG
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }
			ZWrite On
			ZTest LEqual
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			#include "UnityShaderVariables.cginc"
			#include "UnityStandardBRDF.cginc"
			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature_local _NORMALMAPTOGGLE_ON
			#pragma shader_feature_local _SCROLLTOGGLE_ON
			#pragma shader_feature_local _GLOWTOGGLE_ON

			//This is a late directive
			
			uniform float _CubemapToggle;
			uniform float _ScreenSpaceEmit;
			uniform float _SpecularToggle;
			uniform float _RimLightShadowToggle;
			uniform float _ShadowTextureToggle;
			uniform float _CustomShadowColorToggle;
			uniform float4 ShadowColor;
			uniform float4 _CustomShadowColor;
			uniform float _CustomLightingColorToggle;
			uniform float4 LightColor;
			uniform float4 _CustomLightingColor;
			uniform sampler2D _NormalMap;
			uniform float _ShadowOffset;
			uniform float _ShadowSoftness;
			uniform float _ExtraLightToggle;
			uniform float _ExtraLightOffset;
			uniform float _ExtraLightBlend;
			uniform sampler2D _MainTex;
			uniform float2 _BaseRotationCenter;
			uniform float _BaseRotationSpeed;
			uniform sampler2D RotationMask;
			uniform float2 _BaseScrollSpeed;
			uniform sampler2D RotationMask1;
			uniform float _ScreenSpaceMaskisScreenSpace;
			uniform sampler2D _MainTex1;
			uniform float _ScreenSpaceScrollXSpeed;
			uniform float _ScreenSpaceScrollYSpeed;
			uniform float _ScreenSpaceTiling;
			uniform sampler2D _ShadowTexture;
			uniform float2 _ShadowTextureTiling;
			uniform float _ShadowTextureBlend;
			uniform float _RimShadowOffset;
			uniform float _RimShadowPower;
			uniform float _RimShadowOpacity;
			uniform float _RimLightOffset;
			uniform float _RimLightPower;
			uniform float _RimLightBlendBaseTexture;
			uniform float4 _RimLightColor;
			uniform float _RimLightBlend;
			uniform float _FlipbookToggle;
			uniform sampler2D _FlipBookTexture;
			uniform float2 _FlipbookTiling;
			uniform float2 _FlipbookOffset;
			uniform float _FlipbookColumns;
			uniform float _FlipbookRows;
			uniform float _FlipbookSpeed;
			uniform sampler2D _FlipBookMask;
			uniform float _FlipbookEmit;
			uniform float _SpecularPower;
			uniform sampler2D _SpecularMask;
			uniform float _SpecularBrightness;
			uniform float _SpecularCustomColorToggle;
			uniform float4 _SpecularCustomColor;
			uniform samplerCUBE _CubemapTexture;
			uniform sampler2D cubemapmask;
			uniform float _CubemapBlend;
			uniform sampler2D _ScrollMask;
			uniform float _ScrollHue;
			uniform sampler2D _ScrollTex;
			uniform float2 _ScrollSpeed;
			uniform float2 _ScrollSize;
			uniform float2 _ScrollOffset;
			uniform float _ScrollRotation;
			uniform float _ScrollEmit;
			uniform float _EmissionHue;
			uniform sampler2D _Emission;
			uniform float _EmissionEmit;
			uniform float _GlowCycle;
			uniform float4 _GlowColor;
			uniform float _GlowSpeed;
			uniform sampler2D _GlowMask;
			uniform float _GlowEmit;
			float3 HSVToRGB( float3 c )
			{
				float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
				float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
				return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
			}
			
			float3 RGBToHSV(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
				float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
				float d = q.x - min( q.w, q.y );
				float e = 1.0e-10;
				return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}


			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_tangent : TANGENT;
			};

			struct v2f
			{
				V2F_SHADOW_CASTER;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				float4 ase_texcoord6 : TEXCOORD6;
			};


			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f,o);
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float3 ase_worldNormal = UnityObjectToWorldNormal(v.normal);
				o.ase_texcoord1.xyz = ase_worldNormal;
				float3 ase_worldTangent = UnityObjectToWorldDir(v.ase_tangent);
				o.ase_texcoord3.xyz = ase_worldTangent;
				float ase_vertexTangentSign = v.ase_tangent.w * ( unity_WorldTransformParams.w >= 0.0 ? 1.0 : -1.0 );
				float3 ase_worldBitangent = cross( ase_worldNormal, ase_worldTangent ) * ase_vertexTangentSign;
				o.ase_texcoord4.xyz = ase_worldBitangent;
				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (v.vertex).xyz, 1 )).xyz;
				o.ase_texcoord5.xyz = ase_worldPos;
				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord6 = screenPos;
				
				o.ase_texcoord2.xyz = v.ase_texcoord.xyz;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.w = 0;
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.w = 0;
				o.ase_texcoord4.w = 0;
				o.ase_texcoord5.w = 0;

				v.vertex.xyz +=  float3(0,0,0) ;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			float4 frag (v2f i ) : SV_Target
			{
				float3 outColor;
				float outAlpha;

				float3 ase_worldNormal = i.ase_texcoord1.xyz;
				float2 uv_NormalMap835 = i.ase_texcoord2.xyz.xy;
				float3 ase_worldTangent = i.ase_texcoord3.xyz;
				float3 ase_worldBitangent = i.ase_texcoord4.xyz;
				float3 tanToWorld0 = float3( ase_worldTangent.x, ase_worldBitangent.x, ase_worldNormal.x );
				float3 tanToWorld1 = float3( ase_worldTangent.y, ase_worldBitangent.y, ase_worldNormal.y );
				float3 tanToWorld2 = float3( ase_worldTangent.z, ase_worldBitangent.z, ase_worldNormal.z );
				float3 tanNormal834 = UnpackNormal( tex2D( _NormalMap, uv_NormalMap835 ) );
				float3 worldNormal834 = float3(dot(tanToWorld0,tanNormal834), dot(tanToWorld1,tanNormal834), dot(tanToWorld2,tanNormal834));
				#ifdef _NORMALMAPTOGGLE_ON
				float3 staticSwitch843 = worldNormal834;
				#else
				float3 staticSwitch843 = ase_worldNormal;
				#endif
				float3 normalizeResult170 = normalize( staticSwitch843 );
				float3 ase_worldPos = i.ase_texcoord5.xyz;
				float3 worldSpaceLightDir = UnityWorldSpaceLightDir(ase_worldPos);
				float dotResult54 = dot( normalizeResult170 , worldSpaceLightDir );
				float temp_output_57_0 = ( ( dotResult54 + _ShadowOffset ) / _ShadowSoftness );
				float4 lerpResult406 = lerp( (( _CustomShadowColorToggle )?( _CustomShadowColor ):( ShadowColor )) , (( _CustomLightingColorToggle )?( _CustomLightingColor ):( LightColor )) , saturate( temp_output_57_0 ));
				float4 lerpResult855 = lerp( float4( 0,0,0,0 ) , LightColor , saturate( ( ( dotResult54 + _ExtraLightOffset ) / _ShadowSoftness ) ));
				float2 texCoord671 = i.ase_texcoord2.xyz.xy * float2( 1,1 ) + float2( 0,0 );
				float mulTime358 = _Time.y * _BaseRotationSpeed;
				float cos356 = cos( mulTime358 );
				float sin356 = sin( mulTime358 );
				float2 rotator356 = mul( texCoord671 - _BaseRotationCenter , float2x2( cos356 , -sin356 , sin356 , cos356 )) + _BaseRotationCenter;
				float2 uvRotationMask667 = i.ase_texcoord2.xyz.xy;
				float4 tex2DNode667 = tex2D( RotationMask, uvRotationMask667 );
				float2 texCoord559 = i.ase_texcoord2.xyz.xy * float2( 1,1 ) + ( _Time.y * _BaseScrollSpeed );
				float4 temp_output_666_0 = ( ( float4( rotator356, 0.0 , 0.0 ) * tex2DNode667 ) + ( float4( texCoord559, 0.0 , 0.0 ) * ( 1.0 - tex2DNode667 ) ) );
				float4 tex2DNode76 = tex2D( _MainTex, temp_output_666_0.rg );
				float4 screenPos = i.ase_texcoord6;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float4 tex2DNode998 = tex2D( RotationMask1, (( _ScreenSpaceMaskisScreenSpace )?( ase_screenPosNorm ):( temp_output_666_0 )).xy );
				float4 temp_output_1001_0 = ( 1.0 - tex2DNode998 );
				float mulTime962 = _Time.y * _ScreenSpaceScrollXSpeed;
				float mulTime968 = _Time.y * _ScreenSpaceScrollYSpeed;
				float4 appendResult965 = (float4(mulTime962 , mulTime968 , 0.0 , 0.0));
				float4 temp_output_964_0 = ( ase_screenPosNorm + appendResult965 );
				float4 temp_output_1033_0 = ( tex2D( _MainTex1, ( temp_output_964_0 * _ScreenSpaceTiling ).xy ) * tex2DNode998 );
				float4 temp_output_1034_0 = ( ( tex2DNode76 * temp_output_1001_0 ) + temp_output_1033_0 );
				float4 temp_output_158_0 = ( ( lerpResult406 + (( _ExtraLightToggle )?( ( lerpResult855 * _ExtraLightBlend ) ):( float4( 0,0,0,0 ) )) ) * temp_output_1034_0 );
				float4 blendOpSrc822 = saturate( ( 1.0 - ( ( 1.0 - temp_output_57_0 ) * tex2D( _ShadowTexture, ( ase_screenPosNorm * float4( _ShadowTextureTiling, 0.0 , 0.0 ) ).xy ) ) ) );
				float4 blendOpDest822 = temp_output_158_0;
				float4 lerpBlendMode822 = lerp(blendOpDest822,( blendOpSrc822 * blendOpDest822 ),_ShadowTextureBlend);
				#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
				float4 ase_lightColor = 0;
				#else //aselc
				float4 ase_lightColor = _LightColor0;
				#endif //aselc
				float4 temp_output_409_0 = ( (( _ShadowTextureToggle )?( ( saturate( lerpBlendMode822 )) ):( temp_output_158_0 )) * ase_lightColor );
				float3 normalizedWorldNormal = normalize( ase_worldNormal );
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(ase_worldPos);
				ase_worldViewDir = Unity_SafeNormalize( ase_worldViewDir );
				float dotResult992 = dot( normalizedWorldNormal , ase_worldViewDir );
				float temp_output_981_0 = saturate( ( dotResult992 + _RimShadowOffset ) );
				float4 blendOpSrc994 = ( pow( temp_output_981_0 , _RimShadowPower ) * temp_output_409_0 );
				float4 blendOpDest994 = temp_output_409_0;
				float4 lerpBlendMode994 = lerp(blendOpDest994,min( blendOpSrc994 , blendOpDest994 ),_RimShadowOpacity);
				float dotResult423 = dot( normalizedWorldNormal , ase_worldViewDir );
				float2 texCoord631 = i.ase_texcoord2.xyz.xy * _FlipbookTiling + _FlipbookOffset;
				// *** BEGIN Flipbook UV Animation vars ***
				// Total tiles of Flipbook Texture
				float fbtotaltiles634 = _FlipbookColumns * _FlipbookRows;
				// Offsets for cols and rows of Flipbook Texture
				float fbcolsoffset634 = 1.0f / _FlipbookColumns;
				float fbrowsoffset634 = 1.0f / _FlipbookRows;
				// Speed of animation
				float fbspeed634 = _Time[ 1 ] * _FlipbookSpeed;
				// UV Tiling (col and row offset)
				float2 fbtiling634 = float2(fbcolsoffset634, fbrowsoffset634);
				// UV Offset - calculate current tile linear index, and convert it to (X * coloffset, Y * rowoffset)
				// Calculate current tile linear index
				float fbcurrenttileindex634 = round( fmod( fbspeed634 + 0.0, fbtotaltiles634) );
				fbcurrenttileindex634 += ( fbcurrenttileindex634 < 0) ? fbtotaltiles634 : 0;
				// Obtain Offset X coordinate from current tile linear index
				float fblinearindextox634 = round ( fmod ( fbcurrenttileindex634, _FlipbookColumns ) );
				// Multiply Offset X by coloffset
				float fboffsetx634 = fblinearindextox634 * fbcolsoffset634;
				// Obtain Offset Y coordinate from current tile linear index
				float fblinearindextoy634 = round( fmod( ( fbcurrenttileindex634 - fblinearindextox634 ) / _FlipbookColumns, _FlipbookRows ) );
				// Reverse Y to get tiles from Top to Bottom
				fblinearindextoy634 = (int)(_FlipbookRows-1) - fblinearindextoy634;
				// Multiply Offset Y by rowoffset
				float fboffsety634 = fblinearindextoy634 * fbrowsoffset634;
				// UV Offset
				float2 fboffset634 = float2(fboffsetx634, fboffsety634);
				// Flipbook UV
				half2 fbuv634 = texCoord631 * fbtiling634 + fboffset634;
				// *** END Flipbook UV Animation vars ***
				float2 uv_FlipBookMask637 = i.ase_texcoord2.xyz.xy;
				float4 temp_output_642_0 = ( (( _RimLightShadowToggle )?( ( ( saturate( lerpBlendMode994 )) + ( ( pow( ( 1.0 - saturate( ( dotResult423 + _RimLightOffset ) ) ) , _RimLightPower ) * (( _RimLightBlendBaseTexture )?( temp_output_409_0 ):( _RimLightColor )) ) * _RimLightBlend ) ) ):( ( temp_output_409_0 + temp_output_1033_0 ) )) + float4( (( _FlipbookToggle )?( ( (tex2D( _FlipBookTexture, fbuv634 )).rgb * tex2D( _FlipBookMask, uv_FlipBookMask637 ).r * _FlipbookEmit ) ):( float3( 0,0,0 ) )) , 0.0 ) );
				float3 normalizeResult1050 = normalize( ( _WorldSpaceCameraPos - ase_worldPos ) );
				float3 normalizeResult1045 = normalize( ( worldSpaceLightDir + normalizeResult1050 ) );
				float dotResult1046 = dot( normalizeResult1045 , staticSwitch843 );
				float2 uv_SpecularMask1060 = i.ase_texcoord2.xyz.xy;
				ase_worldViewDir = normalize(ase_worldViewDir);
				float3 worldRefl877 = reflect( -ase_worldViewDir, float3( dot( tanToWorld0, staticSwitch843 ), dot( tanToWorld1, staticSwitch843 ), dot( tanToWorld2, staticSwitch843 ) ) );
				float2 uvcubemapmask881 = i.ase_texcoord2.xyz.xy;
				float2 uv_ScrollMask607 = i.ase_texcoord2.xyz.xy;
				float2 texCoord599 = i.ase_texcoord2.xyz.xy * _ScrollSize + _ScrollOffset;
				float cos598 = cos( radians( _ScrollRotation ) );
				float sin598 = sin( radians( _ScrollRotation ) );
				float2 rotator598 = mul( texCoord599 - float2( 0.5,0.5 ) , float2x2( cos598 , -sin598 , sin598 , cos598 )) + float2( 0.5,0.5 );
				float2 panner597 = ( 1.0 * _Time.y * _ScrollSpeed + rotator598);
				float3 hsvTorgb592 = RGBToHSV( tex2D( _ScrollTex, panner597 ).rgb );
				float3 hsvTorgb595 = HSVToRGB( float3(( _ScrollHue + hsvTorgb592.x ),hsvTorgb592.y,hsvTorgb592.z) );
				#ifdef _SCROLLTOGGLE_ON
				float3 staticSwitch591 = ( tex2D( _ScrollMask, uv_ScrollMask607 ).r * hsvTorgb595 * _ScrollEmit );
				#else
				float3 staticSwitch591 = float3( 0,0,0 );
				#endif
				float2 uv_Emission609 = i.ase_texcoord2.xyz.xy;
				float3 hsvTorgb584 = RGBToHSV( tex2D( _Emission, uv_Emission609 ).rgb );
				float3 hsvTorgb585 = HSVToRGB( float3(( _EmissionHue + hsvTorgb584.x ),hsvTorgb584.y,hsvTorgb584.z) );
				float mulTime619 = _Time.y * _GlowSpeed;
				float2 uv_GlowMask622 = i.ase_texcoord2.xyz.xy;
				float4 tex2DNode622 = tex2D( _GlowMask, uv_GlowMask622 );
				float3 hsvTorgb617 = HSVToRGB( float3(( mulTime619 * 0.1 ),1.0,1.0) );
				#ifdef _GLOWTOGGLE_ON
				float3 staticSwitch590 = ( (( _GlowCycle )?( ( hsvTorgb617 * tex2DNode622.r ) ):( ( (_GlowColor).rgb * (0.0 + (sin( mulTime619 ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) * tex2DNode622.r ) )) * _GlowEmit );
				#else
				float3 staticSwitch590 = float3( 0,0,0 );
				#endif
				

				outColor = ( (( _CubemapToggle )?( ( (( _ScreenSpaceEmit )?( ( (( _SpecularToggle )?( ( temp_output_642_0 + ( ( ( pow( saturate( dotResult1046 ) , _SpecularPower ) * tex2D( _SpecularMask, uv_SpecularMask1060 ) ) * _SpecularBrightness ) * (( _SpecularCustomColorToggle )?( _SpecularCustomColor ):( temp_output_409_0 )) ) ) ):( temp_output_642_0 )) + temp_output_1033_0 ) ):( (( _SpecularToggle )?( ( temp_output_642_0 + ( ( ( pow( saturate( dotResult1046 ) , _SpecularPower ) * tex2D( _SpecularMask, uv_SpecularMask1060 ) ) * _SpecularBrightness ) * (( _SpecularCustomColorToggle )?( _SpecularCustomColor ):( temp_output_409_0 )) ) ) ):( temp_output_642_0 )) )) + ( ( texCUBE( _CubemapTexture, worldRefl877 ) * tex2D( cubemapmask, uvcubemapmask881 ) ) * _CubemapBlend ) ) ):( (( _ScreenSpaceEmit )?( ( (( _SpecularToggle )?( ( temp_output_642_0 + ( ( ( pow( saturate( dotResult1046 ) , _SpecularPower ) * tex2D( _SpecularMask, uv_SpecularMask1060 ) ) * _SpecularBrightness ) * (( _SpecularCustomColorToggle )?( _SpecularCustomColor ):( temp_output_409_0 )) ) ) ):( temp_output_642_0 )) + temp_output_1033_0 ) ):( (( _SpecularToggle )?( ( temp_output_642_0 + ( ( ( pow( saturate( dotResult1046 ) , _SpecularPower ) * tex2D( _SpecularMask, uv_SpecularMask1060 ) ) * _SpecularBrightness ) * (( _SpecularCustomColorToggle )?( _SpecularCustomColor ):( temp_output_409_0 )) ) ) ):( temp_output_642_0 )) )) )) + float4( ( staticSwitch591 + ( hsvTorgb585 * _EmissionEmit ) + staticSwitch590 ) , 0.0 ) ).rgb;
				outAlpha = tex2DNode76.a;
				clip(outAlpha);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
		
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.CommentaryNode;361;31.57494,2484.31;Inherit;False;1487.958;514.1742;;14;666;665;668;664;667;559;562;561;560;358;360;359;356;671;Rotation & Scroll;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;226;1492.356,1592.955;Inherit;False;980.267;403.8341;;7;835;834;170;843;53;837;54;Normals, Base Light;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;242;1942.888,2189.487;Inherit;False;1855.488;608.4642;;19;76;995;970;406;408;857;158;625;60;59;971;624;409;769;57;74;58;378;381;Shadows and Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;550;1833.16,412.4896;Inherit;False;1976.592;475.3736;;14;427;858;423;420;426;428;424;558;863;862;564;436;430;429;Rim Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;581;1933.281,-1453.291;Inherit;False;1880.365;759.4941;;14;623;622;621;620;619;618;617;616;615;614;613;612;611;610;Glowing;0.4481132,0.6304269,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;582;69.64522,-681.5258;Inherit;False;2077.342;538.7622;;16;593;607;596;588;605;601;600;606;608;604;599;598;597;595;594;592;ScrollingMasked;0.9008027,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;583;2431.064,-674.0502;Inherit;False;1382.785;524.939;;8;609;603;602;589;587;586;585;584;Emission;0.6419381,0.9433962,0.4850481,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;627;2258.291,-1948.888;Inherit;False;1554.1;476.6765;;12;639;638;637;636;635;634;633;632;631;630;629;628;AnimatedTexture;0.4997601,0.4206123,0.6415094,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;844;2665.375,1395.669;Inherit;False;1142.913;327.8334;;8;867;856;855;866;852;854;853;851;Extra Lighting Pass;1,1,1,1;0;0
Node;AmplifyShaderEditor.RGBToHSVNode;584;2791.046,-464.2994;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.HSVToRGBNode;585;3152.781,-442.598;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;586;3027.86,-465.6219;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;589;3599.438,-540.9744;Inherit;True;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RGBToHSVNode;592;1261.869,-344.1337;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;594;1488.682,-413.4563;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;595;1621.601,-325.4323;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PannerNode;597;802.7104,-319.7243;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;598;600.2661,-487.4334;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;599;353.1503,-488.1154;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RadiansOpNode;604;408.1021,-275.2887;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;618;2373.306,-1212.204;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;619;2178.635,-1123.892;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;621;2524.331,-1211.018;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;623;2558.015,-1021.639;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;620;2003.635,-1122.872;Inherit;False;Property;_GlowSpeed;Glow Speed;58;0;Create;True;0;0;0;False;0;False;10;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;615;2670.344,-1394.291;Inherit;False;Property;_GlowColor;Glow Color;56;0;Create;True;0;0;0;False;0;False;1,0,0,1;0,1,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;608;1197.464,-415.1406;Inherit;False;Property;_ScrollHue;Scroll Hue;72;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;606;122.9617,-274.288;Inherit;False;Property;_ScrollRotation;Scroll Rotation;76;0;Create;True;0;0;0;False;0;False;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;600;138.4171,-395.4172;Inherit;False;Property;_ScrollOffset;Scroll Offset;74;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;601;167.7834,-519.1023;Inherit;False;Property;_ScrollSize;Scroll Size;73;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;605;592.1217,-293.7513;Inherit;False;Property;_ScrollSpeed;Scroll Speed;75;0;Create;True;0;0;0;False;0;False;0,1;0,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;588;1928.776,-534.0127;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;587;2519.526,-552.5554;Inherit;False;Property;_EmissionHue;Emission Hue;8;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;602;3186.479,-295.468;Inherit;False;Property;_EmissionEmit;Emission Emit;9;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;596;1681.195,-402.7046;Inherit;False;Property;_ScrollEmit;Scroll Emit;77;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;610;2927.588,-1265.903;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;611;3163.529,-981.9182;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;612;3162.736,-1232.939;Inherit;True;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.HSVToRGBNode;617;2764.373,-886.1568;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ToggleSwitchNode;616;3404.858,-1079.179;Inherit;False;Property;_GlowCycle;Glow Cycle;57;0;Create;True;0;0;0;False;0;False;1;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;607;1553.419,-608.3159;Inherit;True;Property;_ScrollMask;Scroll Mask;71;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;593;981.6982,-343.0817;Inherit;True;Property;_ScrollTex;Scroll Tex;70;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;868;2980.31,1749.076;Inherit;False;824.413;427.9334;;6;881;883;882;878;869;877;Cubemap;1,1,1,1;0;0
Node;AmplifyShaderEditor.StaticSwitch;591;2169.355,-668.9346;Inherit;False;Property;_ScrollToggle;Scroll Toggle;69;0;Create;True;0;0;0;False;0;False;0;1;1;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;631;2484.292,-1866.888;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCFlipBookUVAnimation;634;2750.29,-1746.887;Inherit;False;0;0;6;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;1;False;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;636;3650.992,-1769.887;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector2Node;632;2308.291,-1898.887;Inherit;False;Property;_FlipbookTiling;Flipbook Tiling;63;0;Create;True;0;0;0;False;0;False;1,1;4,4;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;633;2308.291,-1754.887;Inherit;False;Property;_FlipbookOffset;Flipbook Offset;64;0;Create;True;0;0;0;False;0;False;0,0;0.06,-0.16;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;630;2516.292,-1722.887;Inherit;False;Property;_FlipbookColumns;Flipbook Columns;65;0;Create;True;0;0;0;False;0;False;8;8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;629;2548.292,-1643.886;Inherit;False;Property;_FlipbookRows;Flipbook Rows;66;0;Create;True;0;0;0;False;0;False;8;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;628;2548.292,-1563.886;Inherit;False;Property;_FlipbookSpeed;Flipbook Speed;67;0;Create;True;0;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;640;3891.51,-1704.567;Inherit;False;Property;_FlipbookToggle;Flipbook Toggle;60;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;639;3406.238,-1867.111;Inherit;False;Property;_FlipbookEmit;Flipbook Emit;68;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;637;3289.29,-1693.887;Inherit;True;Property;_FlipBookMask;FlipBookMask;62;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;638;3005.775,-1770.473;Inherit;True;Property;_FlipBookTexture;FlipBookTexture;61;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;921;2865.652,3827.744;Inherit;False;945.1694;488.8861;;12;977;978;889;893;888;887;892;923;924;894;895;922;Outline Pass;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;969;1544.624,2822.211;Inherit;False;2258.01;547.0607;;19;1028;1033;1034;1035;1001;1000;999;998;1002;964;966;967;965;968;962;927;1038;1037;1089;Screen Space;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;429;2615.476,657.9794;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;428;2455.477,657.9794;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;426;2327.477,656.9794;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;430;2775.474,651.9793;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;862;3304.343,481.8564;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;863;3517.522,488.8262;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;424;1863.477,625.9793;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;423;2087.478,545.9796;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;420;1863.477,465.9796;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;979;2170.524,-88.62469;Inherit;False;1636.984;483.7798;;12;983;997;989;988;993;992;991;982;981;994;986;996;Rim Shadow;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;558;3240.87,703.2561;Inherit;False;Property;_RimLightBlend;Rim Light Blend;34;0;Create;True;0;0;0;False;0;False;25;0;0;25;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;564;2995.897,477.42;Inherit;False;Property;_RimLightBlendBaseTexture;Rim Light Blend Base Texture;31;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;436;2786.474,479.9796;Inherit;False;Property;_RimLightColor;Rim Light Color;30;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;858;2448.616,756.8726;Inherit;False;Property;_RimLightPower;Rim Light Power;32;0;Create;True;0;0;0;False;0;False;0.01;0.01;0.01;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;1036;1991.853,3391.234;Inherit;False;1815.989;416.4595;;10;822;771;823;676;687;678;688;768;677;702;Screen Space Shadow Texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;603;3394.778,-379.068;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;613;3466.646,-974.0479;Inherit;False;Property;_GlowEmit;Glow Emit;59;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;635;3292.997,-1770.924;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;614;3629.646,-1078.049;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;427;2023.478,801.9797;Inherit;False;Property;_RimLightOffset;Rim Light Offset;33;0;Create;True;0;0;0;False;0;False;0.65;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;851;2972.615,1465.695;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;853;2846.844,1467.21;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;852;3177.071,1469.724;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;866;3583.226,1585.502;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;855;3334.944,1435.01;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ToggleSwitchNode;856;3602.744,1479.311;Inherit;False;Property;_ExtraLightToggle;Extra Light Toggle;26;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;854;2676.542,1597.21;Inherit;False;Property;_ExtraLightOffset;Extra Light Offset;27;0;Create;True;0;0;0;False;0;False;-0.6;-0.5;-1;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;867;3292.226,1644.502;Inherit;False;Property;_ExtraLightBlend;Extra Light Blend;28;0;Create;True;0;0;0;False;0;False;2;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RotatorNode;356;570.4819,2538.404;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;359;43.48133,2536.405;Inherit;False;Property;_BaseRotationCenter;Base Rotation Center;3;0;Create;True;0;0;0;False;0;False;0,0;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;360;44.48158,2658.405;Inherit;False;Property;_BaseRotationSpeed;Base Rotation Speed;4;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;358;280.4813,2661.405;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;560;286.6014,2748.627;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;561;44.84669,2824.418;Inherit;False;Property;_BaseScrollSpeed;Base Scroll Speed;2;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;562;44.60438,2744.117;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;559;440.611,2749.122;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;381;2371.549,2616.136;Inherit;False;Global;LightColor;LightColor;11;0;Create;True;0;0;0;False;0;False;0.9,0.7,0.4,0;1,1,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;378;2213.762,2231.275;Inherit;False;Global;ShadowColor;ShadowColor;11;0;Create;True;1;Shadow Options;0;0;False;0;False;0.2,0.2,0.2,0;0.5,0.5,0.5,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;58;2253.416,2402.753;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;74;2575.934,2402.626;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;57;2372.479,2399.597;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;769;3528.409,2528.495;Inherit;False;Property;_ShadowTextureToggle;Shadow Texture Toggle;16;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;170;2031.346,1761.201;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;834;1806.679,1633.011;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StaticSwitch;843;1994.343,1645.634;Inherit;False;Property;_NormalMapToggle;Normal Map Toggle;5;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;835;1512.257,1636.757;Inherit;True;Property;_NormalMap;Normal Map;6;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;c125aba81b199ee4986df53b611fe813;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;837;1636.024,1803.573;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldReflectionVector;877;2997.047,1799.014;Inherit;True;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;54;2265.713,1756.613;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;878;3511.147,1804.014;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;882;3658.647,1809.214;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;883;3498.244,1905.915;Inherit;False;Property;_CubemapBlend;Cubemap Blend;53;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;409;3635.287,2233.906;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;922;3181.566,3869.595;Inherit;False;Property;_OutlineColor;Outline Color;38;0;Create;True;0;0;0;False;0;False;0,0,0,1;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;671;262.1205,2526;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;624;1962.414,2235.252;Inherit;False;Property;_CustomShadowColor;Custom Shadow Color;13;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;971;2831.41,2666.693;Inherit;False;Property;_CustomLightingColorToggle;Custom Lighting Color Toggle;10;0;Create;True;0;0;0;False;1;Shadow Options;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;59;1966.97,2418.577;Float;False;Property;_ShadowOffset;Shadow Offset;15;0;Create;True;1;Shadow Options;0;0;False;0;False;0;0.01;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;1973.045,2500.264;Float;False;Property;_ShadowSoftness;Shadow Softness;14;0;Create;True;1;Shadow Options;0;0;False;0;False;0.01;0.01;0.01;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;895;3319.853,4226.94;Inherit;False;Property;_OutlineMaxSize;Outline Max Size;42;0;Create;True;0;0;0;False;0;False;0.008;0.008;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;894;3318.853,4157.94;Inherit;False;Property;_OutlineMinSize;Outline Min Size;41;0;Create;True;0;0;0;False;0;False;0.002;0.003;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;924;3543.232,3870.351;Inherit;False;Property;_OutlineBlendBaseTexture;Outline Blend Base Texture;39;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;923;3397.302,3868.302;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;892;3153.853,4052.938;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;887;3142.378,4156.185;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;893;3524.853,4089.94;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;889;2940.378,3887.182;Inherit;False;Property;_OutlineMultiplier;Outline Multiplier;40;0;Create;True;0;0;0;False;0;False;0.005;0.005;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;978;2939.593,3982.96;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CameraDepthFade;977;2898.893,4169.363;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;625;2459.414,2262.252;Inherit;False;Property;_CustomShadowColorToggle;Custom Shadow Color Toggle;12;0;Create;True;0;0;0;False;1;Shadow Options;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;3328.924,2528.264;Inherit;True;2;2;0;COLOR;1,1,1,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;857;3223.679,2407.756;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;408;3450.76,2237.604;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LerpOp;406;2990.145,2248.976;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;970;2579.41,2614.693;Inherit;False;Property;_CustomLightingColor;Custom Lighting Color;11;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;995;2763.311,2396.093;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;667;666.5232,2684.457;Inherit;True;Property;RotationMask;Base Rotation Mask;1;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;dcf8fa3a5fa078041a91d913db6563fb;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;665;1114.524,2771.456;Inherit;True;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;666;1309.525,2673.457;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;668;953.5233,2765.456;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;664;956.5233,2550.459;Inherit;True;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;927;1868.294,2865.762;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;962;1864.908,3033.18;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;968;1858.908,3109.18;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;965;2047.91,3036.18;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;964;2047.91,2868.18;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1002;2500.127,2865.091;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;998;2189.127,2994.838;Inherit;True;Property;RotationMask1;Screen Space Mask;44;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;dcf8fa3a5fa078041a91d913db6563fb;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;76;1964.591,2584.713;Inherit;True;Property;_MainTex;Base Texture;0;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;dcf8fa3a5fa078041a91d913db6563fb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;999;2720.127,2986.087;Inherit;True;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1000;2945.127,2942.088;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;1001;2524.127,3090.087;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;702;2031.282,3459.466;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;677;2630.618,3473.641;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;768;2206.018,3469.952;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;14,14;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;688;3160.747,3476.257;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;678;2797.766,3475.662;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;687;2997.87,3475.792;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;676;2350.554,3472.065;Inherit;True;Property;_ShadowTexture;Shadow Texture;17;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;b9552255b337ed1468609738595c184a;b9552255b337ed1468609738595c184a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;823;3058.002,3703.564;Inherit;False;Property;_ShadowTextureBlend;Shadow Texture Blend;19;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;771;2030.787,3625.875;Inherit;False;Property;_ShadowTextureTiling;Shadow Texture Tiling;18;0;Create;True;1;Shadow Options;0;0;False;0;False;14,14;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.BlendOpsNode;822;3334.001,3473.563;Inherit;True;Multiply;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0.5;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1033;3291.926,3114.498;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1028;2931.695,3167.118;Inherit;True;Property;_MainTex1;Screen Space Texture;43;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;dcf8fa3a5fa078041a91d913db6563fb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;966;1560.908,3032.18;Inherit;False;Property;_ScreenSpaceScrollXSpeed;Screen Space Scroll X Speed;49;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;967;1559.908,3106.18;Inherit;False;Property;_ScreenSpaceScrollYSpeed;Screen Space Scroll Y Speed;48;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1037;2747.465,3200.477;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;2;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;1038;2522.804,3296.824;Inherit;False;Property;_ScreenSpaceTiling;Screen Space Tiling;47;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;53;1832.417,1804.647;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;1058;1680.904,901.0906;Inherit;False;2130.147;475.957;;18;1053;1074;1073;1060;1065;1063;1064;1061;1062;1052;1046;1045;1051;1050;1042;1047;1049;1048;Specular;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;869;3216.314,1795.703;Inherit;True;Property;_CubemapTexture;Cubemap Texture;51;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;34b1a0522b0f44f48afe8efdffa7cb39;34b1a0522b0f44f48afe8efdffa7cb39;True;0;False;white;Auto;False;Object;-1;Auto;Cube;8;0;SAMPLERCUBE;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;1048;1928.45,942.5652;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;1049;1747.5,949.0944;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceCameraPos;1047;1695.457,1102.051;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;1042;2076.874,942.8367;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;1050;2075.756,1094.23;Inherit;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1051;2284.404,946.442;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalizeNode;1045;2397.942,945.0647;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;1046;2378.761,1053.084;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;1052;2590.986,951.3604;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;996;3293.016,248.8109;Inherit;False;Property;_RimShadowOpacity;Rim Shadow Opacity;37;0;Create;True;0;0;0;False;0;False;0.6676344;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;986;3350.329,-42.76672;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;994;3569,-38.39869;Inherit;True;Darken;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;981;2797.886,165.2712;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;982;2669.886,164.2712;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;991;2205.885,133.2711;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;992;2429.887,53.27143;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;993;2205.885,-26.7285;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;988;2791.025,264.1647;Inherit;False;Property;_RimShadowPower;Rim Shadow Power;35;0;Create;True;0;0;0;False;0;False;10;0.01;0.01;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;989;2365.886,309.2714;Inherit;False;Property;_RimShadowOffset;Rim Shadow Offset;36;0;Create;True;0;0;0;False;0;False;0.65;1;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;997;2956.371,159.3805;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;983;3115.883,-41.72885;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1063;3471.288,986.5499;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1064;3255.776,1091.711;Inherit;False;Property;_SpecularBrightness;Specular Brightness;24;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;1053;2762.142,948.0969;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;12;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1062;2567.571,1187.652;Inherit;False;Property;_SpecularPower;Specular Power;25;0;Create;True;0;0;0;False;0;False;15;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1073;3055.923,1182.904;Inherit;False;Property;_SpecularCustomColor;Specular Custom Color;22;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1061;3247.397,956.29;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;1074;3310.487,1183.248;Inherit;False;Property;_SpecularCustomColorToggle;Specular Custom Color Toggle;21;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1065;3639.809,984.8562;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;1089;1948.795,3201.959;Inherit;False;Property;_ScreenSpaceMaskisScreenSpace;Screen Space Mask is Screen Space;46;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;888;3673.378,4064.182;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1035;3247.265,2891.171;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1034;3516.164,2904.469;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1060;2753.072,1170.499;Inherit;True;Property;_SpecularMask;Specular Mask;23;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;34b1a0522b0f44f48afe8efdffa7cb39;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;912;4997.112,536.8265;Float;False;False;-1;2;ASEMaterialInspector;100;12;New Amplify Shader;fd5163ddf7350f946aff871fd3d7e3fe;True;ShadowCaster;0;4;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;True;0;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.ToggleSwitchNode;1088;4827.026,340.3181;Inherit;False;Property;_SpecularToggle;Specular Toggle;20;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;642;4549.279,354.6125;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1087;4697.74,353.3051;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;475;4154.306,354.4348;Inherit;False;Property;_RimLightShadowToggle;Rim Light/Shadow Toggle;29;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;860;4026.849,356.916;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1040;3900.151,360.2333;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;1097;5129.193,340.7504;Inherit;False;Property;_ScreenSpaceEmit;Screen Space Emit;45;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1096;4976.178,458.4897;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;1099;5372.507,343.389;Inherit;False;Property;_CubemapToggle;Cubemap Toggle;50;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1098;5309.928,452.178;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;881;3209.646,1985.614;Inherit;True;Property;cubemapmask;Cubemap Mask;52;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;910;6065.152,347.1395;Float;False;False;-1;2;ASEMaterialInspector;100;12;New Amplify Shader;fd5163ddf7350f946aff871fd3d7e3fe;True;Deferred;0;2;Deferred;4;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Deferred;True;2;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;911;5866.65,513.0403;Float;False;False;-1;2;ASEMaterialInspector;100;12;New Amplify Shader;fd5163ddf7350f946aff871fd3d7e3fe;True;Outline;0;3;Outline;3;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;True;1;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;0;False;;True;0;False;;True;False;13333;False;;33333;False;;True;1;CullMode=Front;True;5;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;909;5867.152,458.1395;Float;False;False;-1;2;ASEMaterialInspector;100;12;New Amplify Shader;fd5163ddf7350f946aff871fd3d7e3fe;True;ForwardAdd;0;1;ForwardAdd;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;True;4;1;False;;1;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;True;1;LightMode=ForwardAdd;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;908;5867.152,346.1395;Float;False;True;-1;2;ASEMaterialInspector;0;12;Pupsi BRC Shader;fd5163ddf7350f946aff871fd3d7e3fe;True;ForwardBase;0;0;ForwardBase;3;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;1;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;RenderType=Transparent=RenderType;True;5;False;0;True;True;0;5;False;;10;False;;0;5;False;;10;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;True;2;False;;True;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;0;False;;True;0;False;;True;False;0;False;;0;False;;True;1;LightMode=ForwardBase;True;5;False;2;Include;;False;;Native;False;0;0;;Include;;False;;Custom;False;0;0;;;0;0;Standard;0;0;5;True;True;True;True;True;False;;False;0
Node;AmplifyShaderEditor.SimpleAddOpNode;472;5630.691,341.9547;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;622;2755.8,-1117.635;Inherit;True;Property;_GlowMask;Glow Mask;55;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;609;2503.133,-464.035;Inherit;True;Property;_Emission;Emission Texture;7;1;[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;dcf8fa3a5fa078041a91d913db6563fb;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;590;3840.299,-909.5723;Inherit;False;Property;_GlowToggle;Glow Toggle;54;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
WireConnection;584;0;609;0
WireConnection;585;0;586;0
WireConnection;585;1;584;2
WireConnection;585;2;584;3
WireConnection;586;0;587;0
WireConnection;586;1;584;1
WireConnection;589;0;591;0
WireConnection;589;1;603;0
WireConnection;589;2;590;0
WireConnection;592;0;593;0
WireConnection;594;0;608;0
WireConnection;594;1;592;1
WireConnection;595;0;594;0
WireConnection;595;1;592;2
WireConnection;595;2;592;3
WireConnection;597;0;598;0
WireConnection;597;2;605;0
WireConnection;598;0;599;0
WireConnection;598;2;604;0
WireConnection;599;0;601;0
WireConnection;599;1;600;0
WireConnection;604;0;606;0
WireConnection;618;0;619;0
WireConnection;619;0;620;0
WireConnection;621;0;618;0
WireConnection;623;0;619;0
WireConnection;588;0;607;1
WireConnection;588;1;595;0
WireConnection;588;2;596;0
WireConnection;610;0;615;0
WireConnection;611;0;617;0
WireConnection;611;1;622;1
WireConnection;612;0;610;0
WireConnection;612;1;621;0
WireConnection;612;2;622;1
WireConnection;617;0;623;0
WireConnection;616;0;612;0
WireConnection;616;1;611;0
WireConnection;593;1;597;0
WireConnection;591;0;588;0
WireConnection;631;0;632;0
WireConnection;631;1;633;0
WireConnection;634;0;631;0
WireConnection;634;1;630;0
WireConnection;634;2;629;0
WireConnection;634;3;628;0
WireConnection;636;0;635;0
WireConnection;636;1;637;1
WireConnection;636;2;639;0
WireConnection;640;1;636;0
WireConnection;638;1;634;0
WireConnection;429;0;428;0
WireConnection;428;0;426;0
WireConnection;426;0;423;0
WireConnection;426;1;427;0
WireConnection;430;0;429;0
WireConnection;430;1;858;0
WireConnection;862;0;430;0
WireConnection;862;1;564;0
WireConnection;863;0;862;0
WireConnection;863;1;558;0
WireConnection;423;0;420;0
WireConnection;423;1;424;0
WireConnection;564;0;436;0
WireConnection;564;1;409;0
WireConnection;603;0;585;0
WireConnection;603;1;602;0
WireConnection;635;0;638;0
WireConnection;614;0;616;0
WireConnection;614;1;613;0
WireConnection;851;0;853;0
WireConnection;851;1;60;0
WireConnection;853;0;54;0
WireConnection;853;1;854;0
WireConnection;852;0;851;0
WireConnection;866;0;855;0
WireConnection;866;1;867;0
WireConnection;855;1;381;0
WireConnection;855;2;852;0
WireConnection;856;1;866;0
WireConnection;356;0;671;0
WireConnection;356;1;359;0
WireConnection;356;2;358;0
WireConnection;358;0;360;0
WireConnection;560;0;562;0
WireConnection;560;1;561;0
WireConnection;559;1;560;0
WireConnection;58;0;54;0
WireConnection;58;1;59;0
WireConnection;74;0;57;0
WireConnection;57;0;58;0
WireConnection;57;1;60;0
WireConnection;769;0;158;0
WireConnection;769;1;822;0
WireConnection;170;0;843;0
WireConnection;834;0;835;0
WireConnection;843;1;837;0
WireConnection;843;0;834;0
WireConnection;877;0;843;0
WireConnection;54;0;170;0
WireConnection;54;1;53;0
WireConnection;878;0;869;0
WireConnection;878;1;881;0
WireConnection;882;0;878;0
WireConnection;882;1;883;0
WireConnection;409;0;769;0
WireConnection;409;1;408;0
WireConnection;971;0;381;0
WireConnection;971;1;970;0
WireConnection;924;0;922;0
WireConnection;924;1;923;0
WireConnection;923;0;158;0
WireConnection;923;1;922;0
WireConnection;892;0;889;0
WireConnection;892;1;977;0
WireConnection;893;0;892;0
WireConnection;893;1;894;0
WireConnection;893;2;895;0
WireConnection;625;0;378;0
WireConnection;625;1;624;0
WireConnection;158;0;857;0
WireConnection;158;1;1034;0
WireConnection;857;0;406;0
WireConnection;857;1;856;0
WireConnection;406;0;625;0
WireConnection;406;1;971;0
WireConnection;406;2;74;0
WireConnection;995;0;1034;0
WireConnection;995;1;971;0
WireConnection;665;0;559;0
WireConnection;665;1;668;0
WireConnection;666;0;664;0
WireConnection;666;1;665;0
WireConnection;668;0;667;0
WireConnection;664;0;356;0
WireConnection;664;1;667;0
WireConnection;962;0;966;0
WireConnection;968;0;967;0
WireConnection;965;0;962;0
WireConnection;965;1;968;0
WireConnection;964;0;927;0
WireConnection;964;1;965;0
WireConnection;1002;0;964;0
WireConnection;1002;1;998;0
WireConnection;998;1;1089;0
WireConnection;76;1;666;0
WireConnection;999;0;559;0
WireConnection;999;1;1001;0
WireConnection;1000;0;1002;0
WireConnection;1000;1;999;0
WireConnection;1001;0;998;0
WireConnection;677;0;57;0
WireConnection;768;0;702;0
WireConnection;768;1;771;0
WireConnection;688;0;687;0
WireConnection;678;0;677;0
WireConnection;678;1;676;0
WireConnection;687;0;678;0
WireConnection;676;1;768;0
WireConnection;822;0;688;0
WireConnection;822;1;158;0
WireConnection;822;2;823;0
WireConnection;1033;0;1028;0
WireConnection;1033;1;998;0
WireConnection;1028;1;1037;0
WireConnection;1037;0;964;0
WireConnection;1037;1;1038;0
WireConnection;869;1;877;0
WireConnection;1048;0;1047;0
WireConnection;1048;1;1049;0
WireConnection;1050;0;1048;0
WireConnection;1051;0;1042;0
WireConnection;1051;1;1050;0
WireConnection;1045;0;1051;0
WireConnection;1046;0;1045;0
WireConnection;1046;1;843;0
WireConnection;1052;0;1046;0
WireConnection;986;0;983;0
WireConnection;986;1;409;0
WireConnection;994;0;986;0
WireConnection;994;1;409;0
WireConnection;994;2;996;0
WireConnection;981;0;982;0
WireConnection;982;0;992;0
WireConnection;982;1;989;0
WireConnection;992;0;993;0
WireConnection;992;1;991;0
WireConnection;997;0;981;0
WireConnection;983;0;981;0
WireConnection;983;1;988;0
WireConnection;1063;0;1061;0
WireConnection;1063;1;1064;0
WireConnection;1053;0;1052;0
WireConnection;1053;1;1062;0
WireConnection;1061;0;1053;0
WireConnection;1061;1;1060;0
WireConnection;1074;0;409;0
WireConnection;1074;1;1073;0
WireConnection;1065;0;1063;0
WireConnection;1065;1;1074;0
WireConnection;1089;0;666;0
WireConnection;1089;1;927;0
WireConnection;888;0;887;0
WireConnection;888;1;893;0
WireConnection;1035;0;76;0
WireConnection;1035;1;1001;0
WireConnection;1034;0;1035;0
WireConnection;1034;1;1033;0
WireConnection;1088;0;642;0
WireConnection;1088;1;1087;0
WireConnection;642;0;475;0
WireConnection;642;1;640;0
WireConnection;1087;0;642;0
WireConnection;1087;1;1065;0
WireConnection;475;0;1040;0
WireConnection;475;1;860;0
WireConnection;860;0;994;0
WireConnection;860;1;863;0
WireConnection;1040;0;409;0
WireConnection;1040;1;1033;0
WireConnection;1097;0;1088;0
WireConnection;1097;1;1096;0
WireConnection;1096;0;1088;0
WireConnection;1096;1;1033;0
WireConnection;1099;0;1097;0
WireConnection;1099;1;1098;0
WireConnection;1098;0;1097;0
WireConnection;1098;1;882;0
WireConnection;911;0;924;0
WireConnection;911;1;76;4
WireConnection;911;2;888;0
WireConnection;908;0;472;0
WireConnection;908;1;76;4
WireConnection;472;0;1099;0
WireConnection;472;1;589;0
WireConnection;590;0;614;0
ASEEND*/
//CHKSM=798EC3F7A172C7FE6336B086E3CE7F11E66A7FD4