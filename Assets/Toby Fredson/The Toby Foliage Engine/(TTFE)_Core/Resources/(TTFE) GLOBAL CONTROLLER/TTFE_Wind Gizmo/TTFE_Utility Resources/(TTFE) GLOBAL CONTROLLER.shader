// Made with Amplify Shader Editor v1.9.9.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Toby Fredson/The Toby Foliage Engine/Utility/(TTFE) Global Controller"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
		[TTFE_DrawerTitle] _TTFETREEGIZMOSHADER( "(TTFE) TREE GIZMO SHADER", Float ) = 0
		[TTFE_DrawerFeatureBorder][Space (10)] _TEXTUREMAPS1( "TEXTURE MAPS", Float ) = 0
		[NoScaleOffset][Space (10)][TTFE_Drawer_SingleLineTexture] _AlbedoMap( "Albedo Map", 2D ) = "white" {}
		[NoScaleOffset][Normal][TTFE_Drawer_SingleLineTexture] _NormalMap( "Normal Map", 2D ) = "bump" {}
		[NoScaleOffset][TTFE_Drawer_SingleLineTexture] _MaskMap( "Mask Map", 2D ) = "white" {}
		[TTFE_DrawerFeatureBorder][Space (10)] _TEXTUREMAPS1( "WIND SETTINGS", Float ) = 0
		[Header((Trunk and Branch))] _PivotRandomnessStrength( "Pivot Randomness Strength", Float ) = 0.5
		_PivotRandomness( "Pivot Randomness ", Float ) = 1
		[KeywordEnum( GentleBreeze,WindOff )] _WindType( "Wind Type", Float ) = 1
		_BranchWindLarge( "Branch Wind Large", Range( 0, 5 ) ) = 1
		_BranchWindSmall( "Branch Wind Small", Range( 0, 5 ) ) = 1
		_BranchSwayPower( "Branch Sway Power", Float ) = 1
		_MotionBendingGentleRandom( "Motion Bending Gentle Random", Float ) = 0.1
		_DownwardStrength( "Downward Strength", Float ) = -0.5
		[TTFE_DrawerFeatureBorder][Space (10)] _WINDMASKSETTINGS1( "WIND MASK SETTINGS", Float ) = 0
		[Header((Trunk Mask))] _TrunkHeightandThickness( "Trunk Height and Thickness", Float ) = 0.01
		[Toggle] _CenterofMass( "Center of Mass", Float ) = 0
		[Toggle] _MaskRoots( "Mask Roots", Float ) = 1
		[Toggle] _MaskRootsAuto( "Mask Roots (Auto)", Float ) = 0
		[Header((Spherical Mask))] _Radius( "Radius", Float ) = 2
		_Hardness( "Hardness", Float ) = 1
		[Header((Branch Mask))] _BranchMaskScale( "Branch Mask Scale", Float ) = 0.1
		_BranchMaskRadious( "Branch Mask Radius", Float ) = 0.5
		[Header((Roots Mask))] _RootsRadius( "Roots Radius", Float ) = 2
		_RootsHardness( "Roots Hardness", Float ) = 1
		_RootsPosition( "Roots Position", Float ) = 0
		[TTFE_DrawerFeatureBorder][Space (10)] _ADVANCEDSETTINGS( "ADVANCED SETTINGS", Float ) = 0


		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5

		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		//_InstancedTerrainNormals("Instanced Terrain Normals", Float) = 1.0

		[ToggleOff(_SPECULARHIGHLIGHTS_OFF)] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0
		[HideInInspector][ToggleUI] _ReceiveShadows("Receive Shadows", Float) = 1.0

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

		//[HideInInspector][ToggleUI] _AddPrecomputedVelocity("Add Precomputed Velocity", Float) = 1

		//[HideInInspector] _XRMotionVectorsPass("_XRMotionVectorsPass", Float) = 1

		[HideInInspector] _AlphaClip("__clip", Float) = 0.0
	}

	SubShader
	{
		LOD 0

		

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" "UniversalMaterialType"="Lit" }

		Cull Back
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 4.5
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#if ( SHADER_TARGET > 35 ) && defined( SHADER_API_GLES3 )
			#error For WebGL2/GLES3, please set your shader target to 3.5 via SubShader options. URP shaders in ASE use target 4.5 by default.
		#endif

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
							(( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#define ASE_GEOMETRY
			#define _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma shader_feature_local_fragment _RECEIVE_SHADOWS_OFF
			#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#pragma multi_compile_fragment _ DEBUG_DISPLAY
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_VERSION 19908
			#define ASE_SRP_VERSION 170300


			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_ATLAS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
			#pragma multi_compile_fragment _ _SCREEN_SPACE_IRRADIANCE
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
			#pragma multi_compile _ _LIGHT_LAYERS
			#pragma multi_compile_fragment _ _LIGHT_COOKIES
			#pragma multi_compile _ _CLUSTER_LIGHT_LOOP

			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fragment _ LIGHTMAP_BICUBIC_SAMPLING
			#pragma multi_compile_fragment _ REFLECTION_PROBE_ROTATION
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile _ USE_LEGACY_LIGHTMAPS

			#pragma vertex vert
			#pragma fragment frag

			#if defined( _SPECULAR_SETUP ) && defined( ASE_LIGHTING_SIMPLE )
				#if defined( _SPECULARHIGHLIGHTS_OFF )
					#undef _SPECULAR_COLOR
				#else
					#define _SPECULAR_COLOR
				#endif
			#endif

			#define SHADERPASS SHADERPASS_FORWARD

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Fog.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ProbeVolumeVariants.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#if defined( UNITY_INSTANCING_ENABLED ) && defined( ASE_INSTANCED_TERRAIN ) && ( defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL) || defined(_INSTANCEDTERRAINNORMALS_PIXEL) )
				#define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_WORLD_VIEW_DIR
			#define ASE_NEEDS_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				#if defined(LIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES1)
					float4 texcoord1 : TEXCOORD1;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES2)
					float4 texcoord2 : TEXCOORD2;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				half3 normalWS : TEXCOORD1;
				float4 tangentWS : TEXCOORD2; // holds terrainUV ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL
				float4 lightmapUVOrVertexSH : TEXCOORD3;
				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					half4 fogFactorAndVertexLight : TEXCOORD4;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
					float2 dynamicLightmapUV : TEXCOORD5;
				#endif
				#if defined(USE_APV_PROBE_OCCLUSION)
					float4 probeOcclusion : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _CenterofMass;
			float _WINDMASKSETTINGS1;
			float _TEXTUREMAPS1;
			float _PivotRandomness;
			float _PivotRandomnessStrength;
			float _TrunkHeightandThickness;
			float _MotionBendingGentleRandom;
			float _DownwardStrength;
			float _BranchWindSmall;
			float _BranchWindLarge;
			float _BranchSwayPower;
			float _BranchMaskRadious;
			float _BranchMaskScale;
			float _RootsHardness;
			float _RootsRadius;
			float _RootsPosition;
			float _MaskRoots;
			float _Hardness;
			float _Radius;
			float _MaskRootsAuto;
			float _TTFETREEGIZMOSHADER;
			float _ADVANCEDSETTINGS;
			float _AlphaClip;
			float _Cutoff;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float _GlobalWindStrength;
			float4 _WindDirection;
			float _StrongWindSpeed;
			float _WindMotion;
			sampler2D _AlbedoMap;
			sampler2D _MaskMap;
			sampler2D _NormalMap;


			float3 ASESafeNormalize(float3 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			float4 ASESafeNormalize(float4 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float GlobalVar_WindStrength1531_g2 = _GlobalWindStrength;
				float mulTime1270_g2 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1204_g2 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1220_g2 = dotResult1204_g2;
				float dotResult1206_g2 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1221_g2 = dotResult1206_g2;
				float mulTime1269_g2 = _TimeParameters.x * 4.3;
				float dotResult1205_g2 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1222_g2 = dotResult1205_g2;
				float3 normalizeResult1251_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1417_g2 = saturate( (distance( normalizeResult1251_g2 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1185_0_g2 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1199_g2 = dot( temp_output_1185_0_g2 , temp_output_1185_0_g2 );
				float saferPower1228_g2 = abs( saturate( dotResult1199_g2 ) );
				float temp_output_1249_0_g2 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1228_g2 , _Hardness ) );
				float3 normalizeResult1177_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1416_g2 = saturate( (distance( normalizeResult1177_g2 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1414_g2 = (( _CenterofMass )?( ( temp_output_1249_0_g2 * CenterOfMass1416_g2 ) ):( temp_output_1249_0_g2 ));
				float saferPower2084_g2 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2055_g2 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2084_g2 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2110_g2 = float3( 0, -1, 0 );
				float3 appendResult2113_g2 = (float3(break2110_g2.x , ( break2110_g2.y * _RootsPosition ) , break2110_g2.z));
				float3 temp_output_2117_0_g2 = ( ( input.positionOS.xyz - appendResult2113_g2 ) / _RootsRadius );
				float dotResult2118_g2 = dot( temp_output_2117_0_g2 , temp_output_2117_0_g2 );
				float saferPower2121_g2 = abs( saturate( dotResult2118_g2 ) );
				float RootsMask_ProxySphere2122_g2 = pow( saferPower2121_g2 , _RootsHardness );
				float smoothstepResult1602_g2 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask1603_g2 = smoothstepResult1602_g2;
				float3 rotatedValue1373_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1270_g2 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1220_g2 ) + ( RandomSeedVector_D1221_g2 * 0.02 ) ) ) + sin( ( mulTime1269_g2 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1222_g2 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1417_g2 * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * BranchMask1603_g2 ).x );
				float3 appendResult1873_g2 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1860_g2 = input.positionOS.xyz;
				float3 appendResult1862_g2 = (float3(break1860_g2.x , ( break1860_g2.y * 0.15 ) , 0.0));
				float mulTime1866_g2 = _TimeParameters.x * 2.1;
				float dotResult1278_g2 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1296_g2 = dotResult1278_g2;
				float dotResult1283_g2 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1299_g2 = dotResult1283_g2;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1515_g2 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1523_g2 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1515_g2 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1515_g2 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1529_g2 = saturate( ( smoothstepResult1523_g2 * 0.5 ) );
				float3 appendResult1908_g2 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1883_g2 = input.positionOS.xyz;
				float3 appendResult1889_g2 = (float3(break1883_g2.x , 0.0 , ( break1883_g2.z * 0.15 )));
				float mulTime1896_g2 = _TimeParameters.x * 2.3;
				float dotResult1247_g2 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1273_g2 = dotResult1247_g2;
				float dotResult1187_g2 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1201_g2 = dotResult1187_g2;
				float3 appendResult1906_g2 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1882_g2 = input.positionOS.xyz;
				float3 appendResult1887_g2 = (float3(0.0 , ( break1882_g2.y * 0.2 ) , ( break1882_g2.z * 0.4 )));
				float mulTime1891_g2 = _TimeParameters.x * 2.0;
				float dotResult1279_g2 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1295_g2 = dotResult1279_g2;
				float dotResult1282_g2 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1298_g2 = dotResult1282_g2;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult1553_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1554_g2 = _TimeParameters.x * 0.25;
				float simplePerlin2D1557_g2 = snoise( ( normalizeResult1553_g2 + mulTime1554_g2 ).xy*0.43 );
				float WindMask_LargeB1559_g2 = ( simplePerlin2D1557_g2 * 1.5 );
				float mulTime1316_g2 = _TimeParameters.x * 3.2;
				float3 temp_output_1350_0_g2 = ( ( mulTime1316_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_K1296_g2 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1300_g2 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask1418_g2 = saturate( dotResult1300_g2 );
				float mulTime1313_g2 = _TimeParameters.x * 2.3;
				float dotResult1280_g2 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1297_g2 = dotResult1280_g2;
				float dotResult1281_g2 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1294_g2 = dotResult1281_g2;
				float3 temp_output_1353_0_g2 = ( ( mulTime1313_g2 + RandomSeedVector_L1297_g2 + RandomSeedVector_M1294_g2 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1310_g2 = _TimeParameters.x * 3.6;
				float temp_output_1356_0_g2 = ( ( mulTime1310_g2 + RandomSeedVector_N1298_g2 + RandomSeedVector_O1299_g2 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1540_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1541_g2 = _TimeParameters.x * 0.26;
				float simplePerlin2D1547_g2 = snoise( ( normalizeResult1540_g2 + mulTime1541_g2 ).xy*0.7 );
				float WindMask_LargeC1552_g2 = ( simplePerlin2D1547_g2 * 1.5 );
				float3 temp_output_1450_0_g2 = ( ( ( ( rotatedValue1373_g2 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult1873_g2 + ( appendResult1862_g2 * cos( mulTime1866_g2 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1862_g2 ) ) * sin( ( mulTime1866_g2 + RandomSeedVector_K1296_g2 + RandomSeedVector_O1299_g2 ) ) ) ) * 0.1 ) * BranchMask1603_g2 * RandomIDBranchPositionMask1529_g2 ) + ( ( ( appendResult1908_g2 + ( appendResult1889_g2 * cos( ( mulTime1896_g2 + RandomSeedVector_A1273_g2 + RandomSeedVector_G1201_g2 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1889_g2 ) ) * sin( ( mulTime1896_g2 + RandomSeedVector_D1221_g2 ) ) ) ) * BranchMask1603_g2 ) * 0.1 ) + ( ( ( ( appendResult1906_g2 + ( appendResult1887_g2 * cos( mulTime1891_g2 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1887_g2 ) ) * sin( ( mulTime1891_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_N1298_g2 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1529_g2 * BranchMask1603_g2 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB1559_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) + ( ( ( ( cos( temp_output_1350_0_g2 ) * sin( temp_output_1350_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( cos( temp_output_1353_0_g2 ) * sin( temp_output_1353_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( sin( temp_output_1356_0_g2 ) * cos( temp_output_1356_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC1552_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) );
				float3 PIWI_02Gentle1454_g2 = ( temp_output_1450_0_g2 * 0.4 );
				float4 WindDirection1518_g2 = _WindDirection;
				float4 normalizeResult1466_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 break1463_g2 = (normalizeResult1466_g2).xyz;
				float4 appendResult1482_g2 = (float4(break1463_g2.x , ( break1463_g2.y + _DownwardStrength ) , break1463_g2.z , 0.0));
				float4 WindMotion_BaseG1477_g2 = ( appendResult1482_g2 * saturate( input.positionOS.xyz.y ) );
				float2 appendResult1526_g2 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1528_g2 = appendResult1526_g2;
				float GlobalVar_WindSpeed1535_g2 = _StrongWindSpeed;
				float2 NoiseRotation_Output1227_g2 = ( -(WindDirection1518_g2).xz * _TimeParameters.x * GlobalVar_WindSpeed1535_g2 );
				float2 WPRG2D_S41492_g2 = ( BasicWorldPisitionXY_Out1528_g2 + ( NoiseRotation_Output1227_g2 * 4.0 ) );
				float simplePerlin2D1469_g2 = snoise( WPRG2D_S41492_g2*0.2 );
				simplePerlin2D1469_g2 = simplePerlin2D1469_g2*0.5 + 0.5;
				float WindMotionNoise1485_g2 = simplePerlin2D1469_g2;
				float saferPower1589_g2 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1591_g2 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1589_g2 , 0.1 ) ));
				float TrunkHeightMask1593_g2 = saturate( smoothstepResult1591_g2 );
				float4 MotionBendingGentleRandom1509_g2 = ( WindMotion_BaseG1477_g2 * _MotionBendingGentleRandom * WindMotionNoise1485_g2 * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float GlobalVar_WindMotion1530_g2 = _WindMotion;
				float3 worldToObjDir1498_g2 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG1477_g2 *  (0.0 + ( GlobalVar_WindMotion1530_g2 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise1485_g2 ).xyz, 0.0 ) ).xyz;
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float3 Motion_Bending_Gentle_Wind1512_g2 = ( worldToObjDir1498_g2 * ase_objectScale * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float dotResult1248_g2 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1274_g2 = dotResult1248_g2;
				float3 appendResult1362_g2 = (float3(( sin( ( RandomSeedVector_A1273_g2 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1274_g2 ) ) * _PivotRandomnessStrength )));
				float dotResult1252_g2 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1277_g2 = dotResult1252_g2;
				float4 normalizeResult1663_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 worldToObjDir1665_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult1362_g2 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1277_g2 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult1663_g2 * input.positionOS.xyz.y * TrunkHeightMask1593_g2 ).xyz, 0.0 ) ).xyz;
				float mulTime1244_g2 = _TimeParameters.x * 4.0;
				float dotResult1188_g2 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1202_g2 = dotResult1188_g2;
				float mulTime1245_g2 = _TimeParameters.x * 5.2;
				float dotResult1189_g2 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1203_g2 = dotResult1189_g2;
				float3 rotatedValue1361_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1201_g2 * 0.02 ) + mulTime1244_g2 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1202_g2 ) ) ) + sin( ( mulTime1245_g2 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1203_g2 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ).x );
				float3 worldToObjDir1392_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue1361_g2 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1518_g2 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2147_g2 = ( ( ( worldToObjDir1665_g2 * ase_objectScale * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir1392_g2 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1423_g2 = ( float4( PIWI_02Gentle1454_g2 , 0.0 ) + MotionBendingGentleRandom1509_g2 + float4( Motion_Bending_Gentle_Wind1512_g2 , 0.0 ) + float4( PIWI_01Gentle2147_g2 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output1453_g2 = ( GlobalVar_WindStrength1531_g2 * staticSwitch1423_g2 );
				
				output.ase_texcoord7.xy = input.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord7.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1453_g2.xyz;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif
				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );
				VertexNormalInputs normalInput = GetVertexNormalInputs( input.normalOS, input.tangentOS );

				OUTPUT_LIGHTMAP_UV(input.texcoord1, unity_LightmapST, output.lightmapUVOrVertexSH.xy);
				#if defined(DYNAMICLIGHTMAP_ON)
					output.dynamicLightmapUV.xy = input.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif
				OUTPUT_SH4(vertexInput.positionWS, normalInput.normalWS.xyz, GetWorldSpaceNormalizeViewDir(vertexInput.positionWS), output.lightmapUVOrVertexSH.xyz, output.probeOcclusion);

				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					output.fogFactorAndVertexLight = 0;
					#if defined(ASE_FOG) && !defined(_FOG_FRAGMENT)
						output.fogFactorAndVertexLight.x = ComputeFogFactor(vertexInput.positionCS.z);
					#endif
					#ifdef _ADDITIONAL_LIGHTS_VERTEX
						half3 vertexLight = VertexLighting( vertexInput.positionWS, normalInput.normalWS );
						output.fogFactorAndVertexLight.yzw = vertexLight;
					#endif
				#endif

				output.positionCS = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				output.normalWS = normalInput.normalWS;
				output.tangentWS = float4( normalInput.tangentWS, ( input.tangentOS.w > 0.0 ? 1.0 : -1.0 ) * GetOddNegativeScale() );

				#if defined( ENABLE_TERRAIN_PERPIXEL_NORMAL )
					output.tangentWS.zw = input.texcoord.xy;
					output.tangentWS.xy = input.texcoord.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				#if defined(LIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES1)
					float4 texcoord1 : TEXCOORD1;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES2)
					float4 texcoord2 : TEXCOORD2;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.texcoord = input.texcoord;
				#if defined(LIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES1)
					output.texcoord1 = input.texcoord1;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES2)
					output.texcoord2 = input.texcoord2;
				#endif
				
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				#if defined(LIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES1)
					output.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES2)
					output.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				#endif
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag ( PackedVaryings input
						#if defined( ASE_DEPTH_WRITE_ON )
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out uint outRenderingLayers : SV_Target1
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				#if defined( _SURFACE_TYPE_TRANSPARENT )
					const bool isTransparent = true;
				#else
					const bool isTransparent = false;
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					float4 shadowCoord = TransformWorldToShadowCoord( input.positionWS );
				#else
					float4 shadowCoord = float4(0, 0, 0, 0);
				#endif

				// @diogo: mikktspace compliant
				float renormFactor = 1.0 / max( FLT_MIN, length( input.normalWS ) );

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( PositionWS );
				float3 ViewDirWS = GetWorldSpaceNormalizeViewDir( PositionWS );
				float4 ShadowCoord = shadowCoord;
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos );
				float3 TangentWS = input.tangentWS.xyz * renormFactor;
				float3 BitangentWS = cross( input.normalWS, input.tangentWS.xyz ) * input.tangentWS.w * renormFactor;
				float3 NormalWS = input.normalWS * renormFactor;

				#if defined( ENABLE_TERRAIN_PERPIXEL_NORMAL )
					float2 sampleCoords = (input.tangentWS.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					NormalWS = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					TangentWS = -cross(GetObjectToWorldMatrix()._13_23_33, NormalWS);
					BitangentWS = cross(NormalWS, -TangentWS);
				#endif

				float2 uv_AlbedoMap2 = input.ase_texcoord7.xy;
				float4 tex2DNode2 = tex2D( _AlbedoMap, uv_AlbedoMap2 );
				float2 uv_MaskMap4 = input.ase_texcoord7.xy;
				float4 tex2DNode4 = tex2D( _MaskMap, uv_MaskMap4 );
				
				float2 uv_NormalMap3 = input.ase_texcoord7.xy;
				
				float CustomDRAWERS54 = ( _TTFETREEGIZMOSHADER + _TEXTUREMAPS1 + _ADVANCEDSETTINGS );
				
				float4 color10 = IsGammaSpace() ? float4( 0.2156863, 0.5607843, 0.2, 1 ) : float4( 0.03820438, 0.2746773, 0.03310476, 1 );
				float fresnelNdotV5 = dot( NormalWS, ViewDirWS );
				float fresnelNode5 = ( -0.1 + 5.0 * pow( 1.0 - fresnelNdotV5, 5.0 ) );
				

				float3 BaseColor = ( tex2DNode2 * saturate( tex2DNode4.g ) ).rgb;
				float3 Normal = UnpackNormalScale( tex2D( _NormalMap, uv_NormalMap3 ), 1.0f );
				float3 Specular = 0.5;
				float Metallic = CustomDRAWERS54;
				float Smoothness = tex2DNode4.a;
				float Occlusion = tex2DNode4.g;
				float3 Emission = ( saturate( ( color10 * fresnelNode5 ) ) + (tex2DNode2*0.4 + 0.0) ).rgb;
				float Alpha = 1;
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _Cutoff;
					float AlphaClipThresholdShadow = 0.5;
				#endif
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;

				#if defined( ASE_DEPTH_WRITE_ON )
					input.positionCS.z = input.positionCS.z;
				#endif

				#ifdef _CLEARCOAT
					float CoatMask = 0;
					float CoatSmoothness = 0;
				#endif

				#if defined( _ALPHATEST_ON )
					AlphaDiscard( Alpha, AlphaClipThreshold );
				#endif

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS) && defined(ASE_CHANGES_WORLD_POS)
					ShadowCoord = TransformWorldToShadowCoord( PositionWS );
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = PositionWS;
				inputData.positionCS = input.positionCS;
				inputData.normalizedScreenSpaceUV = ScreenPosNorm.xy;
				inputData.viewDirectionWS = ViewDirWS;
				inputData.shadowCoord = ShadowCoord;

				#ifdef _NORMALMAP
						#if _NORMAL_DROPOFF_TS
							inputData.normalWS = TransformTangentToWorld(Normal, half3x3(TangentWS, BitangentWS, NormalWS));
						#elif _NORMAL_DROPOFF_OS
							inputData.normalWS = TransformObjectToWorldNormal(Normal);
						#elif _NORMAL_DROPOFF_WS
							inputData.normalWS = Normal;
						#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = NormalWS;
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = InitializeInputDataFog(float4(inputData.positionWS, 1.0), input.fogFactorAndVertexLight.x);
				#endif
				#ifdef _ADDITIONAL_LIGHTS_VERTEX
					inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
				#endif

				#if defined( ENABLE_TERRAIN_PERPIXEL_NORMAL )
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = input.lightmapUVOrVertexSH.xyz;
				#endif

				#if defined(_SCREEN_SPACE_IRRADIANCE)
					inputData.bakedGI = SAMPLE_GI(_ScreenSpaceIrradiance, input.positionCS.xy);
				#elif defined(DYNAMICLIGHTMAP_ON)
					inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, input.dynamicLightmapUV.xy, SH, inputData.normalWS);
					inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUVOrVertexSH.xy);
				#elif !defined(LIGHTMAP_ON) && (defined(PROBE_VOLUMES_L1) || defined(PROBE_VOLUMES_L2))
					inputData.bakedGI = SAMPLE_GI( SH, GetAbsolutePositionWS(inputData.positionWS),
						inputData.normalWS,
						inputData.viewDirectionWS,
						input.positionCS.xy,
						input.probeOcclusion,
						inputData.shadowMask );
				#else
					inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, SH, inputData.normalWS);
					inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUVOrVertexSH.xy);
				#endif

				#ifdef ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif

				#if defined(DEBUG_DISPLAY)
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.dynamicLightmapUV = input.dynamicLightmapUV.xy;
					#endif
					#if defined(LIGHTMAP_ON)
						inputData.staticLightmapUV = input.lightmapUVOrVertexSH.xy;
					#else
						inputData.vertexSH = SH;
					#endif
					#if defined(USE_APV_PROBE_OCCLUSION)
						inputData.probeOcclusion = input.probeOcclusion;
					#endif
				#endif

				SurfaceData surfaceData;
				surfaceData.albedo              = BaseColor;
				surfaceData.metallic            = saturate(Metallic);
				surfaceData.specular            = Specular;
				surfaceData.smoothness          = saturate(Smoothness),
				surfaceData.occlusion           = Occlusion,
				surfaceData.emission            = Emission,
				surfaceData.alpha               = saturate(Alpha);
				surfaceData.normalTS            = Normal;
				surfaceData.clearCoatMask       = 0;
				surfaceData.clearCoatSmoothness = 1;

				#ifdef _CLEARCOAT
					surfaceData.clearCoatMask       = saturate(CoatMask);
					surfaceData.clearCoatSmoothness = saturate(CoatSmoothness);
				#endif

				#if defined(_DBUFFER)
					ApplyDecalToSurfaceData(input.positionCS, surfaceData, inputData);
				#endif

				#ifdef ASE_LIGHTING_SIMPLE
					half4 color = UniversalFragmentBlinnPhong( inputData, surfaceData);
				#else
					half4 color = UniversalFragmentPBR( inputData, surfaceData);
				#endif

				#ifdef ASE_TRANSMISSION
				{
					float shadow = _TransmissionShadow;

					#define SUM_LIGHT_TRANSMISSION(Light)\
						float3 atten = Light.color * Light.distanceAttenuation;\
						atten = lerp( atten, atten * Light.shadowAttenuation, shadow );\
						half3 transmission = max( 0, -dot( inputData.normalWS, Light.direction ) ) * atten * Transmission;\
						color.rgb += BaseColor * transmission;

					SUM_LIGHT_TRANSMISSION( GetMainLight( inputData.shadowCoord ) );

					#if defined(_ADDITIONAL_LIGHTS)
						uint meshRenderingLayers = GetMeshRenderingLayer();
						uint pixelLightCount = GetAdditionalLightsCount();
						#if USE_CLUSTER_LIGHT_LOOP
							[loop] for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
							{
								CLUSTER_LIGHT_LOOP_SUBTRACTIVE_LIGHT_CHECK

								Light light = GetAdditionalLight(lightIndex, inputData.positionWS, inputData.shadowMask);
								#ifdef _LIGHT_LAYERS
								if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
								#endif
								{
									SUM_LIGHT_TRANSMISSION( light );
								}
							}
						#endif
						LIGHT_LOOP_BEGIN( pixelLightCount )
							Light light = GetAdditionalLight(lightIndex, inputData.positionWS, inputData.shadowMask);
							#ifdef _LIGHT_LAYERS
							if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
							#endif
							{
								SUM_LIGHT_TRANSMISSION( light );
							}
						LIGHT_LOOP_END
					#endif
				}
				#endif

				#ifdef ASE_TRANSLUCENCY
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					#define SUM_LIGHT_TRANSLUCENCY(Light)\
						float3 atten = Light.color * Light.distanceAttenuation;\
						atten = lerp( atten, atten * Light.shadowAttenuation, shadow );\
						half3 lightDir = Light.direction + inputData.normalWS * normal;\
						half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );\
						half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;\
						color.rgb += BaseColor * translucency * strength;

					SUM_LIGHT_TRANSLUCENCY( GetMainLight( inputData.shadowCoord ) );

					#if defined(_ADDITIONAL_LIGHTS)
						uint meshRenderingLayers = GetMeshRenderingLayer();
						uint pixelLightCount = GetAdditionalLightsCount();
						#if USE_CLUSTER_LIGHT_LOOP
							[loop] for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++)
							{
								CLUSTER_LIGHT_LOOP_SUBTRACTIVE_LIGHT_CHECK

								Light light = GetAdditionalLight(lightIndex, inputData.positionWS, inputData.shadowMask);
								#ifdef _LIGHT_LAYERS
								if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
								#endif
								{
									SUM_LIGHT_TRANSLUCENCY( light );
								}
							}
						#endif
						LIGHT_LOOP_BEGIN( pixelLightCount )
							Light light = GetAdditionalLight(lightIndex, inputData.positionWS, inputData.shadowMask);
							#ifdef _LIGHT_LAYERS
							if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
							#endif
							{
								SUM_LIGHT_TRANSLUCENCY( light );
							}
						LIGHT_LOOP_END
					#endif
				}
				#endif

				#ifdef ASE_REFRACTION
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, float4( NormalWS,0 ) ).xyz * ( 1.0 - dot( NormalWS, ViewDirWS ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos.xy ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3(0,0,0), inputData.fogCoord);
					#else
						color.rgb = MixFog(color.rgb, inputData.fogCoord);
					#endif
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					outputDepth = input.positionCS.z;
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					outRenderingLayers = EncodeMeshRenderingLayer();
				#endif

				#if defined( ASE_OPAQUE_KEEP_ALPHA )
					return half4( color.rgb, color.a );
				#else
					return half4( color.rgb, OutputAlpha( color.a, isTransparent ) );
				#endif
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM

			#define ASE_GEOMETRY
			#define _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#pragma multi_compile_fragment _ DEBUG_DISPLAY
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_VERSION 19908
			#define ASE_SRP_VERSION 170300


			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			#pragma vertex vert
			#pragma fragment frag

			#if defined( _SPECULAR_SETUP ) && defined( ASE_LIGHTING_SIMPLE )
				#if defined( _SPECULARHIGHLIGHTS_OFF )
					#undef _SPECULAR_COLOR
				#else
					#define _SPECULAR_COLOR
				#endif
			#endif

			#define SHADERPASS SHADERPASS_SHADOWCASTER

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _CenterofMass;
			float _WINDMASKSETTINGS1;
			float _TEXTUREMAPS1;
			float _PivotRandomness;
			float _PivotRandomnessStrength;
			float _TrunkHeightandThickness;
			float _MotionBendingGentleRandom;
			float _DownwardStrength;
			float _BranchWindSmall;
			float _BranchWindLarge;
			float _BranchSwayPower;
			float _BranchMaskRadious;
			float _BranchMaskScale;
			float _RootsHardness;
			float _RootsRadius;
			float _RootsPosition;
			float _MaskRoots;
			float _Hardness;
			float _Radius;
			float _MaskRootsAuto;
			float _TTFETREEGIZMOSHADER;
			float _ADVANCEDSETTINGS;
			float _AlphaClip;
			float _Cutoff;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float _GlobalWindStrength;
			float4 _WindDirection;
			float _StrongWindSpeed;
			float _WindMotion;


			float3 _LightDirection;
			float3 _LightPosition;

			float3 ASESafeNormalize(float3 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			float4 ASESafeNormalize(float4 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			

			PackedVaryings VertexFunction( Attributes input )
			{
				PackedVaryings output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );

				float GlobalVar_WindStrength1531_g2 = _GlobalWindStrength;
				float mulTime1270_g2 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1204_g2 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1220_g2 = dotResult1204_g2;
				float dotResult1206_g2 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1221_g2 = dotResult1206_g2;
				float mulTime1269_g2 = _TimeParameters.x * 4.3;
				float dotResult1205_g2 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1222_g2 = dotResult1205_g2;
				float3 normalizeResult1251_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1417_g2 = saturate( (distance( normalizeResult1251_g2 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1185_0_g2 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1199_g2 = dot( temp_output_1185_0_g2 , temp_output_1185_0_g2 );
				float saferPower1228_g2 = abs( saturate( dotResult1199_g2 ) );
				float temp_output_1249_0_g2 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1228_g2 , _Hardness ) );
				float3 normalizeResult1177_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1416_g2 = saturate( (distance( normalizeResult1177_g2 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1414_g2 = (( _CenterofMass )?( ( temp_output_1249_0_g2 * CenterOfMass1416_g2 ) ):( temp_output_1249_0_g2 ));
				float saferPower2084_g2 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2055_g2 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2084_g2 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2110_g2 = float3( 0, -1, 0 );
				float3 appendResult2113_g2 = (float3(break2110_g2.x , ( break2110_g2.y * _RootsPosition ) , break2110_g2.z));
				float3 temp_output_2117_0_g2 = ( ( input.positionOS.xyz - appendResult2113_g2 ) / _RootsRadius );
				float dotResult2118_g2 = dot( temp_output_2117_0_g2 , temp_output_2117_0_g2 );
				float saferPower2121_g2 = abs( saturate( dotResult2118_g2 ) );
				float RootsMask_ProxySphere2122_g2 = pow( saferPower2121_g2 , _RootsHardness );
				float smoothstepResult1602_g2 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask1603_g2 = smoothstepResult1602_g2;
				float3 rotatedValue1373_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1270_g2 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1220_g2 ) + ( RandomSeedVector_D1221_g2 * 0.02 ) ) ) + sin( ( mulTime1269_g2 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1222_g2 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1417_g2 * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * BranchMask1603_g2 ).x );
				float3 appendResult1873_g2 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1860_g2 = input.positionOS.xyz;
				float3 appendResult1862_g2 = (float3(break1860_g2.x , ( break1860_g2.y * 0.15 ) , 0.0));
				float mulTime1866_g2 = _TimeParameters.x * 2.1;
				float dotResult1278_g2 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1296_g2 = dotResult1278_g2;
				float dotResult1283_g2 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1299_g2 = dotResult1283_g2;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1515_g2 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1523_g2 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1515_g2 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1515_g2 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1529_g2 = saturate( ( smoothstepResult1523_g2 * 0.5 ) );
				float3 appendResult1908_g2 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1883_g2 = input.positionOS.xyz;
				float3 appendResult1889_g2 = (float3(break1883_g2.x , 0.0 , ( break1883_g2.z * 0.15 )));
				float mulTime1896_g2 = _TimeParameters.x * 2.3;
				float dotResult1247_g2 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1273_g2 = dotResult1247_g2;
				float dotResult1187_g2 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1201_g2 = dotResult1187_g2;
				float3 appendResult1906_g2 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1882_g2 = input.positionOS.xyz;
				float3 appendResult1887_g2 = (float3(0.0 , ( break1882_g2.y * 0.2 ) , ( break1882_g2.z * 0.4 )));
				float mulTime1891_g2 = _TimeParameters.x * 2.0;
				float dotResult1279_g2 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1295_g2 = dotResult1279_g2;
				float dotResult1282_g2 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1298_g2 = dotResult1282_g2;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult1553_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1554_g2 = _TimeParameters.x * 0.25;
				float simplePerlin2D1557_g2 = snoise( ( normalizeResult1553_g2 + mulTime1554_g2 ).xy*0.43 );
				float WindMask_LargeB1559_g2 = ( simplePerlin2D1557_g2 * 1.5 );
				float mulTime1316_g2 = _TimeParameters.x * 3.2;
				float3 temp_output_1350_0_g2 = ( ( mulTime1316_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_K1296_g2 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1300_g2 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask1418_g2 = saturate( dotResult1300_g2 );
				float mulTime1313_g2 = _TimeParameters.x * 2.3;
				float dotResult1280_g2 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1297_g2 = dotResult1280_g2;
				float dotResult1281_g2 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1294_g2 = dotResult1281_g2;
				float3 temp_output_1353_0_g2 = ( ( mulTime1313_g2 + RandomSeedVector_L1297_g2 + RandomSeedVector_M1294_g2 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1310_g2 = _TimeParameters.x * 3.6;
				float temp_output_1356_0_g2 = ( ( mulTime1310_g2 + RandomSeedVector_N1298_g2 + RandomSeedVector_O1299_g2 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1540_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1541_g2 = _TimeParameters.x * 0.26;
				float simplePerlin2D1547_g2 = snoise( ( normalizeResult1540_g2 + mulTime1541_g2 ).xy*0.7 );
				float WindMask_LargeC1552_g2 = ( simplePerlin2D1547_g2 * 1.5 );
				float3 temp_output_1450_0_g2 = ( ( ( ( rotatedValue1373_g2 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult1873_g2 + ( appendResult1862_g2 * cos( mulTime1866_g2 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1862_g2 ) ) * sin( ( mulTime1866_g2 + RandomSeedVector_K1296_g2 + RandomSeedVector_O1299_g2 ) ) ) ) * 0.1 ) * BranchMask1603_g2 * RandomIDBranchPositionMask1529_g2 ) + ( ( ( appendResult1908_g2 + ( appendResult1889_g2 * cos( ( mulTime1896_g2 + RandomSeedVector_A1273_g2 + RandomSeedVector_G1201_g2 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1889_g2 ) ) * sin( ( mulTime1896_g2 + RandomSeedVector_D1221_g2 ) ) ) ) * BranchMask1603_g2 ) * 0.1 ) + ( ( ( ( appendResult1906_g2 + ( appendResult1887_g2 * cos( mulTime1891_g2 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1887_g2 ) ) * sin( ( mulTime1891_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_N1298_g2 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1529_g2 * BranchMask1603_g2 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB1559_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) + ( ( ( ( cos( temp_output_1350_0_g2 ) * sin( temp_output_1350_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( cos( temp_output_1353_0_g2 ) * sin( temp_output_1353_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( sin( temp_output_1356_0_g2 ) * cos( temp_output_1356_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC1552_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) );
				float3 PIWI_02Gentle1454_g2 = ( temp_output_1450_0_g2 * 0.4 );
				float4 WindDirection1518_g2 = _WindDirection;
				float4 normalizeResult1466_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 break1463_g2 = (normalizeResult1466_g2).xyz;
				float4 appendResult1482_g2 = (float4(break1463_g2.x , ( break1463_g2.y + _DownwardStrength ) , break1463_g2.z , 0.0));
				float4 WindMotion_BaseG1477_g2 = ( appendResult1482_g2 * saturate( input.positionOS.xyz.y ) );
				float2 appendResult1526_g2 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1528_g2 = appendResult1526_g2;
				float GlobalVar_WindSpeed1535_g2 = _StrongWindSpeed;
				float2 NoiseRotation_Output1227_g2 = ( -(WindDirection1518_g2).xz * _TimeParameters.x * GlobalVar_WindSpeed1535_g2 );
				float2 WPRG2D_S41492_g2 = ( BasicWorldPisitionXY_Out1528_g2 + ( NoiseRotation_Output1227_g2 * 4.0 ) );
				float simplePerlin2D1469_g2 = snoise( WPRG2D_S41492_g2*0.2 );
				simplePerlin2D1469_g2 = simplePerlin2D1469_g2*0.5 + 0.5;
				float WindMotionNoise1485_g2 = simplePerlin2D1469_g2;
				float saferPower1589_g2 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1591_g2 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1589_g2 , 0.1 ) ));
				float TrunkHeightMask1593_g2 = saturate( smoothstepResult1591_g2 );
				float4 MotionBendingGentleRandom1509_g2 = ( WindMotion_BaseG1477_g2 * _MotionBendingGentleRandom * WindMotionNoise1485_g2 * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float GlobalVar_WindMotion1530_g2 = _WindMotion;
				float3 worldToObjDir1498_g2 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG1477_g2 *  (0.0 + ( GlobalVar_WindMotion1530_g2 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise1485_g2 ).xyz, 0.0 ) ).xyz;
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float3 Motion_Bending_Gentle_Wind1512_g2 = ( worldToObjDir1498_g2 * ase_objectScale * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float dotResult1248_g2 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1274_g2 = dotResult1248_g2;
				float3 appendResult1362_g2 = (float3(( sin( ( RandomSeedVector_A1273_g2 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1274_g2 ) ) * _PivotRandomnessStrength )));
				float dotResult1252_g2 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1277_g2 = dotResult1252_g2;
				float4 normalizeResult1663_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 worldToObjDir1665_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult1362_g2 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1277_g2 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult1663_g2 * input.positionOS.xyz.y * TrunkHeightMask1593_g2 ).xyz, 0.0 ) ).xyz;
				float mulTime1244_g2 = _TimeParameters.x * 4.0;
				float dotResult1188_g2 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1202_g2 = dotResult1188_g2;
				float mulTime1245_g2 = _TimeParameters.x * 5.2;
				float dotResult1189_g2 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1203_g2 = dotResult1189_g2;
				float3 rotatedValue1361_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1201_g2 * 0.02 ) + mulTime1244_g2 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1202_g2 ) ) ) + sin( ( mulTime1245_g2 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1203_g2 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ).x );
				float3 worldToObjDir1392_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue1361_g2 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1518_g2 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2147_g2 = ( ( ( worldToObjDir1665_g2 * ase_objectScale * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir1392_g2 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1423_g2 = ( float4( PIWI_02Gentle1454_g2 , 0.0 ) + MotionBendingGentleRandom1509_g2 + float4( Motion_Bending_Gentle_Wind1512_g2 , 0.0 ) + float4( PIWI_01Gentle2147_g2 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output1453_g2 = ( GlobalVar_WindStrength1531_g2 * staticSwitch1423_g2 );
				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1453_g2.xyz;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

				float3 positionWS = TransformObjectToWorld( input.positionOS.xyz );
				float3 normalWS = TransformObjectToWorldDir(input.normalOS);

				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif

				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

				//code for UNITY_REVERSED_Z is moved into Shadows.hlsl from 6000.0.22 and or higher
				positionCS = ApplyShadowClamping(positionCS);

				output.positionCS = positionCS;
				output.positionWS = positionWS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(	PackedVaryings input
						#if defined( ASE_DEPTH_WRITE_ON )
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
				#else
					float4 shadowCoord = float4(0, 0, 0, 0);
				#endif

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( input.positionWS );
				float4 ShadowCoord = shadowCoord;
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos );

				

				float Alpha = 1;
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _Cutoff;
					float AlphaClipThresholdShadow = 0.5;
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					input.positionCS.z = input.positionCS.z;
				#endif

				#if defined( _ALPHATEST_ON )
					#if defined( _ALPHATEST_SHADOW_ON )
						AlphaDiscard( Alpha, AlphaClipThresholdShadow );
					#else
						AlphaDiscard( Alpha, AlphaClipThreshold );
					#endif
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					outputDepth = input.positionCS.z;
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

			#define ASE_GEOMETRY
			#define _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#pragma multi_compile_fragment _ DEBUG_DISPLAY
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_VERSION 19908
			#define ASE_SRP_VERSION 170300


			#pragma vertex vert
			#pragma fragment frag

			#if defined( _SPECULAR_SETUP ) && defined( ASE_LIGHTING_SIMPLE )
				#if defined( _SPECULARHIGHLIGHTS_OFF )
					#undef _SPECULAR_COLOR
				#else
					#define _SPECULAR_COLOR
				#endif
			#endif

			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _CenterofMass;
			float _WINDMASKSETTINGS1;
			float _TEXTUREMAPS1;
			float _PivotRandomness;
			float _PivotRandomnessStrength;
			float _TrunkHeightandThickness;
			float _MotionBendingGentleRandom;
			float _DownwardStrength;
			float _BranchWindSmall;
			float _BranchWindLarge;
			float _BranchSwayPower;
			float _BranchMaskRadious;
			float _BranchMaskScale;
			float _RootsHardness;
			float _RootsRadius;
			float _RootsPosition;
			float _MaskRoots;
			float _Hardness;
			float _Radius;
			float _MaskRootsAuto;
			float _TTFETREEGIZMOSHADER;
			float _ADVANCEDSETTINGS;
			float _AlphaClip;
			float _Cutoff;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float _GlobalWindStrength;
			float4 _WindDirection;
			float _StrongWindSpeed;
			float _WindMotion;


			float3 ASESafeNormalize(float3 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			float4 ASESafeNormalize(float4 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float GlobalVar_WindStrength1531_g2 = _GlobalWindStrength;
				float mulTime1270_g2 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1204_g2 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1220_g2 = dotResult1204_g2;
				float dotResult1206_g2 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1221_g2 = dotResult1206_g2;
				float mulTime1269_g2 = _TimeParameters.x * 4.3;
				float dotResult1205_g2 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1222_g2 = dotResult1205_g2;
				float3 normalizeResult1251_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1417_g2 = saturate( (distance( normalizeResult1251_g2 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1185_0_g2 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1199_g2 = dot( temp_output_1185_0_g2 , temp_output_1185_0_g2 );
				float saferPower1228_g2 = abs( saturate( dotResult1199_g2 ) );
				float temp_output_1249_0_g2 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1228_g2 , _Hardness ) );
				float3 normalizeResult1177_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1416_g2 = saturate( (distance( normalizeResult1177_g2 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1414_g2 = (( _CenterofMass )?( ( temp_output_1249_0_g2 * CenterOfMass1416_g2 ) ):( temp_output_1249_0_g2 ));
				float saferPower2084_g2 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2055_g2 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2084_g2 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2110_g2 = float3( 0, -1, 0 );
				float3 appendResult2113_g2 = (float3(break2110_g2.x , ( break2110_g2.y * _RootsPosition ) , break2110_g2.z));
				float3 temp_output_2117_0_g2 = ( ( input.positionOS.xyz - appendResult2113_g2 ) / _RootsRadius );
				float dotResult2118_g2 = dot( temp_output_2117_0_g2 , temp_output_2117_0_g2 );
				float saferPower2121_g2 = abs( saturate( dotResult2118_g2 ) );
				float RootsMask_ProxySphere2122_g2 = pow( saferPower2121_g2 , _RootsHardness );
				float smoothstepResult1602_g2 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask1603_g2 = smoothstepResult1602_g2;
				float3 rotatedValue1373_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1270_g2 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1220_g2 ) + ( RandomSeedVector_D1221_g2 * 0.02 ) ) ) + sin( ( mulTime1269_g2 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1222_g2 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1417_g2 * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * BranchMask1603_g2 ).x );
				float3 appendResult1873_g2 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1860_g2 = input.positionOS.xyz;
				float3 appendResult1862_g2 = (float3(break1860_g2.x , ( break1860_g2.y * 0.15 ) , 0.0));
				float mulTime1866_g2 = _TimeParameters.x * 2.1;
				float dotResult1278_g2 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1296_g2 = dotResult1278_g2;
				float dotResult1283_g2 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1299_g2 = dotResult1283_g2;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1515_g2 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1523_g2 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1515_g2 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1515_g2 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1529_g2 = saturate( ( smoothstepResult1523_g2 * 0.5 ) );
				float3 appendResult1908_g2 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1883_g2 = input.positionOS.xyz;
				float3 appendResult1889_g2 = (float3(break1883_g2.x , 0.0 , ( break1883_g2.z * 0.15 )));
				float mulTime1896_g2 = _TimeParameters.x * 2.3;
				float dotResult1247_g2 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1273_g2 = dotResult1247_g2;
				float dotResult1187_g2 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1201_g2 = dotResult1187_g2;
				float3 appendResult1906_g2 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1882_g2 = input.positionOS.xyz;
				float3 appendResult1887_g2 = (float3(0.0 , ( break1882_g2.y * 0.2 ) , ( break1882_g2.z * 0.4 )));
				float mulTime1891_g2 = _TimeParameters.x * 2.0;
				float dotResult1279_g2 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1295_g2 = dotResult1279_g2;
				float dotResult1282_g2 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1298_g2 = dotResult1282_g2;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult1553_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1554_g2 = _TimeParameters.x * 0.25;
				float simplePerlin2D1557_g2 = snoise( ( normalizeResult1553_g2 + mulTime1554_g2 ).xy*0.43 );
				float WindMask_LargeB1559_g2 = ( simplePerlin2D1557_g2 * 1.5 );
				float mulTime1316_g2 = _TimeParameters.x * 3.2;
				float3 temp_output_1350_0_g2 = ( ( mulTime1316_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_K1296_g2 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1300_g2 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask1418_g2 = saturate( dotResult1300_g2 );
				float mulTime1313_g2 = _TimeParameters.x * 2.3;
				float dotResult1280_g2 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1297_g2 = dotResult1280_g2;
				float dotResult1281_g2 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1294_g2 = dotResult1281_g2;
				float3 temp_output_1353_0_g2 = ( ( mulTime1313_g2 + RandomSeedVector_L1297_g2 + RandomSeedVector_M1294_g2 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1310_g2 = _TimeParameters.x * 3.6;
				float temp_output_1356_0_g2 = ( ( mulTime1310_g2 + RandomSeedVector_N1298_g2 + RandomSeedVector_O1299_g2 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1540_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1541_g2 = _TimeParameters.x * 0.26;
				float simplePerlin2D1547_g2 = snoise( ( normalizeResult1540_g2 + mulTime1541_g2 ).xy*0.7 );
				float WindMask_LargeC1552_g2 = ( simplePerlin2D1547_g2 * 1.5 );
				float3 temp_output_1450_0_g2 = ( ( ( ( rotatedValue1373_g2 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult1873_g2 + ( appendResult1862_g2 * cos( mulTime1866_g2 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1862_g2 ) ) * sin( ( mulTime1866_g2 + RandomSeedVector_K1296_g2 + RandomSeedVector_O1299_g2 ) ) ) ) * 0.1 ) * BranchMask1603_g2 * RandomIDBranchPositionMask1529_g2 ) + ( ( ( appendResult1908_g2 + ( appendResult1889_g2 * cos( ( mulTime1896_g2 + RandomSeedVector_A1273_g2 + RandomSeedVector_G1201_g2 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1889_g2 ) ) * sin( ( mulTime1896_g2 + RandomSeedVector_D1221_g2 ) ) ) ) * BranchMask1603_g2 ) * 0.1 ) + ( ( ( ( appendResult1906_g2 + ( appendResult1887_g2 * cos( mulTime1891_g2 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1887_g2 ) ) * sin( ( mulTime1891_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_N1298_g2 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1529_g2 * BranchMask1603_g2 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB1559_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) + ( ( ( ( cos( temp_output_1350_0_g2 ) * sin( temp_output_1350_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( cos( temp_output_1353_0_g2 ) * sin( temp_output_1353_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( sin( temp_output_1356_0_g2 ) * cos( temp_output_1356_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC1552_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) );
				float3 PIWI_02Gentle1454_g2 = ( temp_output_1450_0_g2 * 0.4 );
				float4 WindDirection1518_g2 = _WindDirection;
				float4 normalizeResult1466_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 break1463_g2 = (normalizeResult1466_g2).xyz;
				float4 appendResult1482_g2 = (float4(break1463_g2.x , ( break1463_g2.y + _DownwardStrength ) , break1463_g2.z , 0.0));
				float4 WindMotion_BaseG1477_g2 = ( appendResult1482_g2 * saturate( input.positionOS.xyz.y ) );
				float2 appendResult1526_g2 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1528_g2 = appendResult1526_g2;
				float GlobalVar_WindSpeed1535_g2 = _StrongWindSpeed;
				float2 NoiseRotation_Output1227_g2 = ( -(WindDirection1518_g2).xz * _TimeParameters.x * GlobalVar_WindSpeed1535_g2 );
				float2 WPRG2D_S41492_g2 = ( BasicWorldPisitionXY_Out1528_g2 + ( NoiseRotation_Output1227_g2 * 4.0 ) );
				float simplePerlin2D1469_g2 = snoise( WPRG2D_S41492_g2*0.2 );
				simplePerlin2D1469_g2 = simplePerlin2D1469_g2*0.5 + 0.5;
				float WindMotionNoise1485_g2 = simplePerlin2D1469_g2;
				float saferPower1589_g2 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1591_g2 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1589_g2 , 0.1 ) ));
				float TrunkHeightMask1593_g2 = saturate( smoothstepResult1591_g2 );
				float4 MotionBendingGentleRandom1509_g2 = ( WindMotion_BaseG1477_g2 * _MotionBendingGentleRandom * WindMotionNoise1485_g2 * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float GlobalVar_WindMotion1530_g2 = _WindMotion;
				float3 worldToObjDir1498_g2 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG1477_g2 *  (0.0 + ( GlobalVar_WindMotion1530_g2 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise1485_g2 ).xyz, 0.0 ) ).xyz;
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float3 Motion_Bending_Gentle_Wind1512_g2 = ( worldToObjDir1498_g2 * ase_objectScale * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float dotResult1248_g2 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1274_g2 = dotResult1248_g2;
				float3 appendResult1362_g2 = (float3(( sin( ( RandomSeedVector_A1273_g2 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1274_g2 ) ) * _PivotRandomnessStrength )));
				float dotResult1252_g2 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1277_g2 = dotResult1252_g2;
				float4 normalizeResult1663_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 worldToObjDir1665_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult1362_g2 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1277_g2 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult1663_g2 * input.positionOS.xyz.y * TrunkHeightMask1593_g2 ).xyz, 0.0 ) ).xyz;
				float mulTime1244_g2 = _TimeParameters.x * 4.0;
				float dotResult1188_g2 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1202_g2 = dotResult1188_g2;
				float mulTime1245_g2 = _TimeParameters.x * 5.2;
				float dotResult1189_g2 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1203_g2 = dotResult1189_g2;
				float3 rotatedValue1361_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1201_g2 * 0.02 ) + mulTime1244_g2 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1202_g2 ) ) ) + sin( ( mulTime1245_g2 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1203_g2 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ).x );
				float3 worldToObjDir1392_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue1361_g2 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1518_g2 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2147_g2 = ( ( ( worldToObjDir1665_g2 * ase_objectScale * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir1392_g2 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1423_g2 = ( float4( PIWI_02Gentle1454_g2 , 0.0 ) + MotionBendingGentleRandom1509_g2 + float4( Motion_Bending_Gentle_Wind1512_g2 , 0.0 ) + float4( PIWI_01Gentle2147_g2 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output1453_g2 = ( GlobalVar_WindStrength1531_g2 * staticSwitch1423_g2 );
				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1453_g2.xyz;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				output.positionCS = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(	PackedVaryings input
						#if defined( ASE_DEPTH_WRITE_ON )
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
				#else
					float4 shadowCoord = float4(0, 0, 0, 0);
				#endif

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( input.positionWS );
				float4 ShadowCoord = shadowCoord;
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos );

				

				float Alpha = 1;
				float AlphaClipThreshold = _Cutoff;

				#if defined( ASE_DEPTH_WRITE_ON )
					input.positionCS.z = input.positionCS.z;
				#endif

				#if defined( _ALPHATEST_ON )
					AlphaDiscard( Alpha, AlphaClipThreshold );
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					outputDepth = input.positionCS.z;
				#endif

				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			#define ASE_GEOMETRY
			#define _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#pragma multi_compile_fragment _ DEBUG_DISPLAY
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_VERSION 19908
			#define ASE_SRP_VERSION 170300

			#pragma shader_feature EDITOR_VISUALIZATION

			#pragma vertex vert
			#pragma fragment frag

			#if defined( _SPECULAR_SETUP ) && defined( ASE_LIGHTING_SIMPLE )
				#if defined( _SPECULARHIGHLIGHTS_OFF )
					#undef _SPECULAR_COLOR
				#else
					#define _SPECULAR_COLOR
				#endif
			#endif

			#define SHADERPASS SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_WORLD_POSITION
			#define ASE_NEEDS_FRAG_WORLD_POSITION
			#define ASE_NEEDS_VERT_NORMAL
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				#ifdef EDITOR_VISUALIZATION
					float4 VizUV : TEXCOORD1;
					float4 LightCoord : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _CenterofMass;
			float _WINDMASKSETTINGS1;
			float _TEXTUREMAPS1;
			float _PivotRandomness;
			float _PivotRandomnessStrength;
			float _TrunkHeightandThickness;
			float _MotionBendingGentleRandom;
			float _DownwardStrength;
			float _BranchWindSmall;
			float _BranchWindLarge;
			float _BranchSwayPower;
			float _BranchMaskRadious;
			float _BranchMaskScale;
			float _RootsHardness;
			float _RootsRadius;
			float _RootsPosition;
			float _MaskRoots;
			float _Hardness;
			float _Radius;
			float _MaskRootsAuto;
			float _TTFETREEGIZMOSHADER;
			float _ADVANCEDSETTINGS;
			float _AlphaClip;
			float _Cutoff;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float _GlobalWindStrength;
			float4 _WindDirection;
			float _StrongWindSpeed;
			float _WindMotion;
			sampler2D _AlbedoMap;
			sampler2D _MaskMap;


			float3 ASESafeNormalize(float3 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			float4 ASESafeNormalize(float4 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float GlobalVar_WindStrength1531_g2 = _GlobalWindStrength;
				float mulTime1270_g2 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1204_g2 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1220_g2 = dotResult1204_g2;
				float dotResult1206_g2 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1221_g2 = dotResult1206_g2;
				float mulTime1269_g2 = _TimeParameters.x * 4.3;
				float dotResult1205_g2 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1222_g2 = dotResult1205_g2;
				float3 normalizeResult1251_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1417_g2 = saturate( (distance( normalizeResult1251_g2 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1185_0_g2 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1199_g2 = dot( temp_output_1185_0_g2 , temp_output_1185_0_g2 );
				float saferPower1228_g2 = abs( saturate( dotResult1199_g2 ) );
				float temp_output_1249_0_g2 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1228_g2 , _Hardness ) );
				float3 normalizeResult1177_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1416_g2 = saturate( (distance( normalizeResult1177_g2 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1414_g2 = (( _CenterofMass )?( ( temp_output_1249_0_g2 * CenterOfMass1416_g2 ) ):( temp_output_1249_0_g2 ));
				float saferPower2084_g2 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2055_g2 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2084_g2 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2110_g2 = float3( 0, -1, 0 );
				float3 appendResult2113_g2 = (float3(break2110_g2.x , ( break2110_g2.y * _RootsPosition ) , break2110_g2.z));
				float3 temp_output_2117_0_g2 = ( ( input.positionOS.xyz - appendResult2113_g2 ) / _RootsRadius );
				float dotResult2118_g2 = dot( temp_output_2117_0_g2 , temp_output_2117_0_g2 );
				float saferPower2121_g2 = abs( saturate( dotResult2118_g2 ) );
				float RootsMask_ProxySphere2122_g2 = pow( saferPower2121_g2 , _RootsHardness );
				float smoothstepResult1602_g2 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask1603_g2 = smoothstepResult1602_g2;
				float3 rotatedValue1373_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1270_g2 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1220_g2 ) + ( RandomSeedVector_D1221_g2 * 0.02 ) ) ) + sin( ( mulTime1269_g2 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1222_g2 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1417_g2 * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * BranchMask1603_g2 ).x );
				float3 appendResult1873_g2 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1860_g2 = input.positionOS.xyz;
				float3 appendResult1862_g2 = (float3(break1860_g2.x , ( break1860_g2.y * 0.15 ) , 0.0));
				float mulTime1866_g2 = _TimeParameters.x * 2.1;
				float dotResult1278_g2 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1296_g2 = dotResult1278_g2;
				float dotResult1283_g2 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1299_g2 = dotResult1283_g2;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1515_g2 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1523_g2 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1515_g2 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1515_g2 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1529_g2 = saturate( ( smoothstepResult1523_g2 * 0.5 ) );
				float3 appendResult1908_g2 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1883_g2 = input.positionOS.xyz;
				float3 appendResult1889_g2 = (float3(break1883_g2.x , 0.0 , ( break1883_g2.z * 0.15 )));
				float mulTime1896_g2 = _TimeParameters.x * 2.3;
				float dotResult1247_g2 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1273_g2 = dotResult1247_g2;
				float dotResult1187_g2 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1201_g2 = dotResult1187_g2;
				float3 appendResult1906_g2 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1882_g2 = input.positionOS.xyz;
				float3 appendResult1887_g2 = (float3(0.0 , ( break1882_g2.y * 0.2 ) , ( break1882_g2.z * 0.4 )));
				float mulTime1891_g2 = _TimeParameters.x * 2.0;
				float dotResult1279_g2 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1295_g2 = dotResult1279_g2;
				float dotResult1282_g2 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1298_g2 = dotResult1282_g2;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult1553_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1554_g2 = _TimeParameters.x * 0.25;
				float simplePerlin2D1557_g2 = snoise( ( normalizeResult1553_g2 + mulTime1554_g2 ).xy*0.43 );
				float WindMask_LargeB1559_g2 = ( simplePerlin2D1557_g2 * 1.5 );
				float mulTime1316_g2 = _TimeParameters.x * 3.2;
				float3 temp_output_1350_0_g2 = ( ( mulTime1316_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_K1296_g2 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1300_g2 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask1418_g2 = saturate( dotResult1300_g2 );
				float mulTime1313_g2 = _TimeParameters.x * 2.3;
				float dotResult1280_g2 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1297_g2 = dotResult1280_g2;
				float dotResult1281_g2 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1294_g2 = dotResult1281_g2;
				float3 temp_output_1353_0_g2 = ( ( mulTime1313_g2 + RandomSeedVector_L1297_g2 + RandomSeedVector_M1294_g2 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1310_g2 = _TimeParameters.x * 3.6;
				float temp_output_1356_0_g2 = ( ( mulTime1310_g2 + RandomSeedVector_N1298_g2 + RandomSeedVector_O1299_g2 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1540_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1541_g2 = _TimeParameters.x * 0.26;
				float simplePerlin2D1547_g2 = snoise( ( normalizeResult1540_g2 + mulTime1541_g2 ).xy*0.7 );
				float WindMask_LargeC1552_g2 = ( simplePerlin2D1547_g2 * 1.5 );
				float3 temp_output_1450_0_g2 = ( ( ( ( rotatedValue1373_g2 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult1873_g2 + ( appendResult1862_g2 * cos( mulTime1866_g2 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1862_g2 ) ) * sin( ( mulTime1866_g2 + RandomSeedVector_K1296_g2 + RandomSeedVector_O1299_g2 ) ) ) ) * 0.1 ) * BranchMask1603_g2 * RandomIDBranchPositionMask1529_g2 ) + ( ( ( appendResult1908_g2 + ( appendResult1889_g2 * cos( ( mulTime1896_g2 + RandomSeedVector_A1273_g2 + RandomSeedVector_G1201_g2 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1889_g2 ) ) * sin( ( mulTime1896_g2 + RandomSeedVector_D1221_g2 ) ) ) ) * BranchMask1603_g2 ) * 0.1 ) + ( ( ( ( appendResult1906_g2 + ( appendResult1887_g2 * cos( mulTime1891_g2 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1887_g2 ) ) * sin( ( mulTime1891_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_N1298_g2 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1529_g2 * BranchMask1603_g2 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB1559_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) + ( ( ( ( cos( temp_output_1350_0_g2 ) * sin( temp_output_1350_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( cos( temp_output_1353_0_g2 ) * sin( temp_output_1353_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( sin( temp_output_1356_0_g2 ) * cos( temp_output_1356_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC1552_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) );
				float3 PIWI_02Gentle1454_g2 = ( temp_output_1450_0_g2 * 0.4 );
				float4 WindDirection1518_g2 = _WindDirection;
				float4 normalizeResult1466_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 break1463_g2 = (normalizeResult1466_g2).xyz;
				float4 appendResult1482_g2 = (float4(break1463_g2.x , ( break1463_g2.y + _DownwardStrength ) , break1463_g2.z , 0.0));
				float4 WindMotion_BaseG1477_g2 = ( appendResult1482_g2 * saturate( input.positionOS.xyz.y ) );
				float2 appendResult1526_g2 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1528_g2 = appendResult1526_g2;
				float GlobalVar_WindSpeed1535_g2 = _StrongWindSpeed;
				float2 NoiseRotation_Output1227_g2 = ( -(WindDirection1518_g2).xz * _TimeParameters.x * GlobalVar_WindSpeed1535_g2 );
				float2 WPRG2D_S41492_g2 = ( BasicWorldPisitionXY_Out1528_g2 + ( NoiseRotation_Output1227_g2 * 4.0 ) );
				float simplePerlin2D1469_g2 = snoise( WPRG2D_S41492_g2*0.2 );
				simplePerlin2D1469_g2 = simplePerlin2D1469_g2*0.5 + 0.5;
				float WindMotionNoise1485_g2 = simplePerlin2D1469_g2;
				float saferPower1589_g2 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1591_g2 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1589_g2 , 0.1 ) ));
				float TrunkHeightMask1593_g2 = saturate( smoothstepResult1591_g2 );
				float4 MotionBendingGentleRandom1509_g2 = ( WindMotion_BaseG1477_g2 * _MotionBendingGentleRandom * WindMotionNoise1485_g2 * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float GlobalVar_WindMotion1530_g2 = _WindMotion;
				float3 worldToObjDir1498_g2 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG1477_g2 *  (0.0 + ( GlobalVar_WindMotion1530_g2 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise1485_g2 ).xyz, 0.0 ) ).xyz;
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float3 Motion_Bending_Gentle_Wind1512_g2 = ( worldToObjDir1498_g2 * ase_objectScale * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float dotResult1248_g2 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1274_g2 = dotResult1248_g2;
				float3 appendResult1362_g2 = (float3(( sin( ( RandomSeedVector_A1273_g2 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1274_g2 ) ) * _PivotRandomnessStrength )));
				float dotResult1252_g2 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1277_g2 = dotResult1252_g2;
				float4 normalizeResult1663_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 worldToObjDir1665_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult1362_g2 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1277_g2 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult1663_g2 * input.positionOS.xyz.y * TrunkHeightMask1593_g2 ).xyz, 0.0 ) ).xyz;
				float mulTime1244_g2 = _TimeParameters.x * 4.0;
				float dotResult1188_g2 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1202_g2 = dotResult1188_g2;
				float mulTime1245_g2 = _TimeParameters.x * 5.2;
				float dotResult1189_g2 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1203_g2 = dotResult1189_g2;
				float3 rotatedValue1361_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1201_g2 * 0.02 ) + mulTime1244_g2 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1202_g2 ) ) ) + sin( ( mulTime1245_g2 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1203_g2 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ).x );
				float3 worldToObjDir1392_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue1361_g2 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1518_g2 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2147_g2 = ( ( ( worldToObjDir1665_g2 * ase_objectScale * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir1392_g2 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1423_g2 = ( float4( PIWI_02Gentle1454_g2 , 0.0 ) + MotionBendingGentleRandom1509_g2 + float4( Motion_Bending_Gentle_Wind1512_g2 , 0.0 ) + float4( PIWI_01Gentle2147_g2 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output1453_g2 = ( GlobalVar_WindStrength1531_g2 * staticSwitch1423_g2 );
				
				float3 ase_normalWS = TransformObjectToWorldNormal( input.normalOS );
				output.ase_texcoord4.xyz = ase_normalWS;
				
				output.ase_texcoord3.xy = input.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;
				output.ase_texcoord4.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1453_g2.xyz;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

				#ifdef EDITOR_VISUALIZATION
					float2 VizUV = 0;
					float4 LightCoord = 0;
					UnityEditorVizData(input.positionOS.xyz, input.texcoord.xy, input.texcoord1.xy, input.texcoord2.xy, VizUV, LightCoord);
					output.VizUV = float4(VizUV, 0, 0);
					output.LightCoord = LightCoord;
				#endif

				output.positionCS = MetaVertexPosition( input.positionOS, input.texcoord1.xy, input.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );
				output.positionWS = TransformObjectToWorld( input.positionOS.xyz );
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.texcoord = input.texcoord;
				output.texcoord1 = input.texcoord1;
				output.texcoord2 = input.texcoord2;
				
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				output.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				output.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
				#else
					float4 shadowCoord = float4(0, 0, 0, 0);
				#endif

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( input.positionWS );
				float4 ShadowCoord = shadowCoord;

				float2 uv_AlbedoMap2 = input.ase_texcoord3.xy;
				float4 tex2DNode2 = tex2D( _AlbedoMap, uv_AlbedoMap2 );
				float2 uv_MaskMap4 = input.ase_texcoord3.xy;
				float4 tex2DNode4 = tex2D( _MaskMap, uv_MaskMap4 );
				
				float4 color10 = IsGammaSpace() ? float4( 0.2156863, 0.5607843, 0.2, 1 ) : float4( 0.03820438, 0.2746773, 0.03310476, 1 );
				float3 ase_viewVectorWS = ( ( unity_OrthoParams.w == 0 ) ? _WorldSpaceCameraPos - PositionWS : UNITY_MATRIX_V[ 2 ].xyz );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 ase_normalWS = input.ase_texcoord4.xyz;
				float fresnelNdotV5 = dot( ase_normalWS, ase_viewDirWS );
				float fresnelNode5 = ( -0.1 + 5.0 * pow( 1.0 - fresnelNdotV5, 5.0 ) );
				

				float3 BaseColor = ( tex2DNode2 * saturate( tex2DNode4.g ) ).rgb;
				float3 Emission = ( saturate( ( color10 * fresnelNode5 ) ) + (tex2DNode2*0.4 + 0.0) ).rgb;
				float Alpha = 1;
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _Cutoff;
				#endif

				#if defined( _ALPHATEST_ON )
					AlphaDiscard( Alpha, AlphaClipThreshold );
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = BaseColor;
				metaInput.Emission = Emission;
				#ifdef EDITOR_VISUALIZATION
					metaInput.VizUV = input.VizUV.xy;
					metaInput.LightCoord = input.LightCoord;
				#endif

				return UnityMetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM

			#define ASE_GEOMETRY
			#define _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#pragma multi_compile_fragment _ DEBUG_DISPLAY
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_VERSION 19908
			#define ASE_SRP_VERSION 170300


			#pragma vertex vert
			#pragma fragment frag

			#if defined( _SPECULAR_SETUP ) && defined( ASE_LIGHTING_SIMPLE )
				#if defined( _SPECULARHIGHLIGHTS_OFF )
					#undef _SPECULAR_COLOR
				#else
					#define _SPECULAR_COLOR
				#endif
			#endif

			#define SHADERPASS SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _CenterofMass;
			float _WINDMASKSETTINGS1;
			float _TEXTUREMAPS1;
			float _PivotRandomness;
			float _PivotRandomnessStrength;
			float _TrunkHeightandThickness;
			float _MotionBendingGentleRandom;
			float _DownwardStrength;
			float _BranchWindSmall;
			float _BranchWindLarge;
			float _BranchSwayPower;
			float _BranchMaskRadious;
			float _BranchMaskScale;
			float _RootsHardness;
			float _RootsRadius;
			float _RootsPosition;
			float _MaskRoots;
			float _Hardness;
			float _Radius;
			float _MaskRootsAuto;
			float _TTFETREEGIZMOSHADER;
			float _ADVANCEDSETTINGS;
			float _AlphaClip;
			float _Cutoff;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float _GlobalWindStrength;
			float4 _WindDirection;
			float _StrongWindSpeed;
			float _WindMotion;
			sampler2D _AlbedoMap;
			sampler2D _MaskMap;


			float3 ASESafeNormalize(float3 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			float4 ASESafeNormalize(float4 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_TRANSFER_INSTANCE_ID( input, output );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );

				float GlobalVar_WindStrength1531_g2 = _GlobalWindStrength;
				float mulTime1270_g2 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1204_g2 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1220_g2 = dotResult1204_g2;
				float dotResult1206_g2 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1221_g2 = dotResult1206_g2;
				float mulTime1269_g2 = _TimeParameters.x * 4.3;
				float dotResult1205_g2 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1222_g2 = dotResult1205_g2;
				float3 normalizeResult1251_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1417_g2 = saturate( (distance( normalizeResult1251_g2 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1185_0_g2 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1199_g2 = dot( temp_output_1185_0_g2 , temp_output_1185_0_g2 );
				float saferPower1228_g2 = abs( saturate( dotResult1199_g2 ) );
				float temp_output_1249_0_g2 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1228_g2 , _Hardness ) );
				float3 normalizeResult1177_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1416_g2 = saturate( (distance( normalizeResult1177_g2 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1414_g2 = (( _CenterofMass )?( ( temp_output_1249_0_g2 * CenterOfMass1416_g2 ) ):( temp_output_1249_0_g2 ));
				float saferPower2084_g2 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2055_g2 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2084_g2 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2110_g2 = float3( 0, -1, 0 );
				float3 appendResult2113_g2 = (float3(break2110_g2.x , ( break2110_g2.y * _RootsPosition ) , break2110_g2.z));
				float3 temp_output_2117_0_g2 = ( ( input.positionOS.xyz - appendResult2113_g2 ) / _RootsRadius );
				float dotResult2118_g2 = dot( temp_output_2117_0_g2 , temp_output_2117_0_g2 );
				float saferPower2121_g2 = abs( saturate( dotResult2118_g2 ) );
				float RootsMask_ProxySphere2122_g2 = pow( saferPower2121_g2 , _RootsHardness );
				float smoothstepResult1602_g2 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask1603_g2 = smoothstepResult1602_g2;
				float3 rotatedValue1373_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1270_g2 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1220_g2 ) + ( RandomSeedVector_D1221_g2 * 0.02 ) ) ) + sin( ( mulTime1269_g2 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1222_g2 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1417_g2 * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * BranchMask1603_g2 ).x );
				float3 appendResult1873_g2 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1860_g2 = input.positionOS.xyz;
				float3 appendResult1862_g2 = (float3(break1860_g2.x , ( break1860_g2.y * 0.15 ) , 0.0));
				float mulTime1866_g2 = _TimeParameters.x * 2.1;
				float dotResult1278_g2 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1296_g2 = dotResult1278_g2;
				float dotResult1283_g2 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1299_g2 = dotResult1283_g2;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1515_g2 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1523_g2 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1515_g2 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1515_g2 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1529_g2 = saturate( ( smoothstepResult1523_g2 * 0.5 ) );
				float3 appendResult1908_g2 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1883_g2 = input.positionOS.xyz;
				float3 appendResult1889_g2 = (float3(break1883_g2.x , 0.0 , ( break1883_g2.z * 0.15 )));
				float mulTime1896_g2 = _TimeParameters.x * 2.3;
				float dotResult1247_g2 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1273_g2 = dotResult1247_g2;
				float dotResult1187_g2 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1201_g2 = dotResult1187_g2;
				float3 appendResult1906_g2 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1882_g2 = input.positionOS.xyz;
				float3 appendResult1887_g2 = (float3(0.0 , ( break1882_g2.y * 0.2 ) , ( break1882_g2.z * 0.4 )));
				float mulTime1891_g2 = _TimeParameters.x * 2.0;
				float dotResult1279_g2 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1295_g2 = dotResult1279_g2;
				float dotResult1282_g2 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1298_g2 = dotResult1282_g2;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult1553_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1554_g2 = _TimeParameters.x * 0.25;
				float simplePerlin2D1557_g2 = snoise( ( normalizeResult1553_g2 + mulTime1554_g2 ).xy*0.43 );
				float WindMask_LargeB1559_g2 = ( simplePerlin2D1557_g2 * 1.5 );
				float mulTime1316_g2 = _TimeParameters.x * 3.2;
				float3 temp_output_1350_0_g2 = ( ( mulTime1316_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_K1296_g2 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1300_g2 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask1418_g2 = saturate( dotResult1300_g2 );
				float mulTime1313_g2 = _TimeParameters.x * 2.3;
				float dotResult1280_g2 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1297_g2 = dotResult1280_g2;
				float dotResult1281_g2 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1294_g2 = dotResult1281_g2;
				float3 temp_output_1353_0_g2 = ( ( mulTime1313_g2 + RandomSeedVector_L1297_g2 + RandomSeedVector_M1294_g2 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1310_g2 = _TimeParameters.x * 3.6;
				float temp_output_1356_0_g2 = ( ( mulTime1310_g2 + RandomSeedVector_N1298_g2 + RandomSeedVector_O1299_g2 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1540_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1541_g2 = _TimeParameters.x * 0.26;
				float simplePerlin2D1547_g2 = snoise( ( normalizeResult1540_g2 + mulTime1541_g2 ).xy*0.7 );
				float WindMask_LargeC1552_g2 = ( simplePerlin2D1547_g2 * 1.5 );
				float3 temp_output_1450_0_g2 = ( ( ( ( rotatedValue1373_g2 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult1873_g2 + ( appendResult1862_g2 * cos( mulTime1866_g2 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1862_g2 ) ) * sin( ( mulTime1866_g2 + RandomSeedVector_K1296_g2 + RandomSeedVector_O1299_g2 ) ) ) ) * 0.1 ) * BranchMask1603_g2 * RandomIDBranchPositionMask1529_g2 ) + ( ( ( appendResult1908_g2 + ( appendResult1889_g2 * cos( ( mulTime1896_g2 + RandomSeedVector_A1273_g2 + RandomSeedVector_G1201_g2 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1889_g2 ) ) * sin( ( mulTime1896_g2 + RandomSeedVector_D1221_g2 ) ) ) ) * BranchMask1603_g2 ) * 0.1 ) + ( ( ( ( appendResult1906_g2 + ( appendResult1887_g2 * cos( mulTime1891_g2 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1887_g2 ) ) * sin( ( mulTime1891_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_N1298_g2 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1529_g2 * BranchMask1603_g2 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB1559_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) + ( ( ( ( cos( temp_output_1350_0_g2 ) * sin( temp_output_1350_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( cos( temp_output_1353_0_g2 ) * sin( temp_output_1353_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( sin( temp_output_1356_0_g2 ) * cos( temp_output_1356_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC1552_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) );
				float3 PIWI_02Gentle1454_g2 = ( temp_output_1450_0_g2 * 0.4 );
				float4 WindDirection1518_g2 = _WindDirection;
				float4 normalizeResult1466_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 break1463_g2 = (normalizeResult1466_g2).xyz;
				float4 appendResult1482_g2 = (float4(break1463_g2.x , ( break1463_g2.y + _DownwardStrength ) , break1463_g2.z , 0.0));
				float4 WindMotion_BaseG1477_g2 = ( appendResult1482_g2 * saturate( input.positionOS.xyz.y ) );
				float2 appendResult1526_g2 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1528_g2 = appendResult1526_g2;
				float GlobalVar_WindSpeed1535_g2 = _StrongWindSpeed;
				float2 NoiseRotation_Output1227_g2 = ( -(WindDirection1518_g2).xz * _TimeParameters.x * GlobalVar_WindSpeed1535_g2 );
				float2 WPRG2D_S41492_g2 = ( BasicWorldPisitionXY_Out1528_g2 + ( NoiseRotation_Output1227_g2 * 4.0 ) );
				float simplePerlin2D1469_g2 = snoise( WPRG2D_S41492_g2*0.2 );
				simplePerlin2D1469_g2 = simplePerlin2D1469_g2*0.5 + 0.5;
				float WindMotionNoise1485_g2 = simplePerlin2D1469_g2;
				float saferPower1589_g2 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1591_g2 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1589_g2 , 0.1 ) ));
				float TrunkHeightMask1593_g2 = saturate( smoothstepResult1591_g2 );
				float4 MotionBendingGentleRandom1509_g2 = ( WindMotion_BaseG1477_g2 * _MotionBendingGentleRandom * WindMotionNoise1485_g2 * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float GlobalVar_WindMotion1530_g2 = _WindMotion;
				float3 worldToObjDir1498_g2 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG1477_g2 *  (0.0 + ( GlobalVar_WindMotion1530_g2 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise1485_g2 ).xyz, 0.0 ) ).xyz;
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float3 Motion_Bending_Gentle_Wind1512_g2 = ( worldToObjDir1498_g2 * ase_objectScale * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float dotResult1248_g2 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1274_g2 = dotResult1248_g2;
				float3 appendResult1362_g2 = (float3(( sin( ( RandomSeedVector_A1273_g2 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1274_g2 ) ) * _PivotRandomnessStrength )));
				float dotResult1252_g2 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1277_g2 = dotResult1252_g2;
				float4 normalizeResult1663_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 worldToObjDir1665_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult1362_g2 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1277_g2 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult1663_g2 * input.positionOS.xyz.y * TrunkHeightMask1593_g2 ).xyz, 0.0 ) ).xyz;
				float mulTime1244_g2 = _TimeParameters.x * 4.0;
				float dotResult1188_g2 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1202_g2 = dotResult1188_g2;
				float mulTime1245_g2 = _TimeParameters.x * 5.2;
				float dotResult1189_g2 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1203_g2 = dotResult1189_g2;
				float3 rotatedValue1361_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1201_g2 * 0.02 ) + mulTime1244_g2 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1202_g2 ) ) ) + sin( ( mulTime1245_g2 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1203_g2 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ).x );
				float3 worldToObjDir1392_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue1361_g2 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1518_g2 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2147_g2 = ( ( ( worldToObjDir1665_g2 * ase_objectScale * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir1392_g2 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1423_g2 = ( float4( PIWI_02Gentle1454_g2 , 0.0 ) + MotionBendingGentleRandom1509_g2 + float4( Motion_Bending_Gentle_Wind1512_g2 , 0.0 ) + float4( PIWI_01Gentle2147_g2 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output1453_g2 = ( GlobalVar_WindStrength1531_g2 * staticSwitch1423_g2 );
				
				output.ase_texcoord1.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord1.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1453_g2.xyz;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				output.positionCS = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.ase_texcoord = input.ase_texcoord;
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag(PackedVaryings input  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
				#else
					float4 shadowCoord = float4(0, 0, 0, 0);
				#endif

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( input.positionWS );
				float4 ShadowCoord = shadowCoord;

				float2 uv_AlbedoMap2 = input.ase_texcoord1.xy;
				float4 tex2DNode2 = tex2D( _AlbedoMap, uv_AlbedoMap2 );
				float2 uv_MaskMap4 = input.ase_texcoord1.xy;
				float4 tex2DNode4 = tex2D( _MaskMap, uv_MaskMap4 );
				

				float3 BaseColor = ( tex2DNode2 * saturate( tex2DNode4.g ) ).rgb;
				float Alpha = 1;
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _Cutoff;
				#endif

				half4 color = half4(BaseColor, Alpha );

				#if defined( _ALPHATEST_ON )
					AlphaDiscard( Alpha, AlphaClipThreshold );
				#endif

				return color;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthNormals"
			Tags { "LightMode"="DepthNormals" }

			ZWrite On
			Blend One Zero
			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

			#define ASE_GEOMETRY
			#define _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#pragma multi_compile_fragment _ DEBUG_DISPLAY
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_VERSION 19908
			#define ASE_SRP_VERSION 170300


			#pragma vertex vert
			#pragma fragment frag

			#if defined( _SPECULAR_SETUP ) && defined( ASE_LIGHTING_SIMPLE )
				#if defined( _SPECULARHIGHLIGHTS_OFF )
					#undef _SPECULAR_COLOR
				#else
					#define _SPECULAR_COLOR
				#endif
			#endif

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
			//#define SHADERPASS SHADERPASS_DEPTHNORMALS

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#if defined( UNITY_INSTANCING_ENABLED ) && defined( ASE_INSTANCED_TERRAIN ) && ( defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL) || defined(_INSTANCEDTERRAINNORMALS_PIXEL) )
				#define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				half4 texcoord : TEXCOORD0;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				half3 normalWS : TEXCOORD1;
				float4 tangentWS : TEXCOORD2; // holds terrainUV ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _CenterofMass;
			float _WINDMASKSETTINGS1;
			float _TEXTUREMAPS1;
			float _PivotRandomness;
			float _PivotRandomnessStrength;
			float _TrunkHeightandThickness;
			float _MotionBendingGentleRandom;
			float _DownwardStrength;
			float _BranchWindSmall;
			float _BranchWindLarge;
			float _BranchSwayPower;
			float _BranchMaskRadious;
			float _BranchMaskScale;
			float _RootsHardness;
			float _RootsRadius;
			float _RootsPosition;
			float _MaskRoots;
			float _Hardness;
			float _Radius;
			float _MaskRootsAuto;
			float _TTFETREEGIZMOSHADER;
			float _ADVANCEDSETTINGS;
			float _AlphaClip;
			float _Cutoff;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float _GlobalWindStrength;
			float4 _WindDirection;
			float _StrongWindSpeed;
			float _WindMotion;
			sampler2D _NormalMap;


			float3 ASESafeNormalize(float3 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			float4 ASESafeNormalize(float4 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float GlobalVar_WindStrength1531_g2 = _GlobalWindStrength;
				float mulTime1270_g2 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1204_g2 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1220_g2 = dotResult1204_g2;
				float dotResult1206_g2 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1221_g2 = dotResult1206_g2;
				float mulTime1269_g2 = _TimeParameters.x * 4.3;
				float dotResult1205_g2 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1222_g2 = dotResult1205_g2;
				float3 normalizeResult1251_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1417_g2 = saturate( (distance( normalizeResult1251_g2 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1185_0_g2 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1199_g2 = dot( temp_output_1185_0_g2 , temp_output_1185_0_g2 );
				float saferPower1228_g2 = abs( saturate( dotResult1199_g2 ) );
				float temp_output_1249_0_g2 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1228_g2 , _Hardness ) );
				float3 normalizeResult1177_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1416_g2 = saturate( (distance( normalizeResult1177_g2 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1414_g2 = (( _CenterofMass )?( ( temp_output_1249_0_g2 * CenterOfMass1416_g2 ) ):( temp_output_1249_0_g2 ));
				float saferPower2084_g2 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2055_g2 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2084_g2 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2110_g2 = float3( 0, -1, 0 );
				float3 appendResult2113_g2 = (float3(break2110_g2.x , ( break2110_g2.y * _RootsPosition ) , break2110_g2.z));
				float3 temp_output_2117_0_g2 = ( ( input.positionOS.xyz - appendResult2113_g2 ) / _RootsRadius );
				float dotResult2118_g2 = dot( temp_output_2117_0_g2 , temp_output_2117_0_g2 );
				float saferPower2121_g2 = abs( saturate( dotResult2118_g2 ) );
				float RootsMask_ProxySphere2122_g2 = pow( saferPower2121_g2 , _RootsHardness );
				float smoothstepResult1602_g2 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask1603_g2 = smoothstepResult1602_g2;
				float3 rotatedValue1373_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1270_g2 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1220_g2 ) + ( RandomSeedVector_D1221_g2 * 0.02 ) ) ) + sin( ( mulTime1269_g2 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1222_g2 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1417_g2 * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * BranchMask1603_g2 ).x );
				float3 appendResult1873_g2 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1860_g2 = input.positionOS.xyz;
				float3 appendResult1862_g2 = (float3(break1860_g2.x , ( break1860_g2.y * 0.15 ) , 0.0));
				float mulTime1866_g2 = _TimeParameters.x * 2.1;
				float dotResult1278_g2 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1296_g2 = dotResult1278_g2;
				float dotResult1283_g2 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1299_g2 = dotResult1283_g2;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1515_g2 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1523_g2 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1515_g2 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1515_g2 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1529_g2 = saturate( ( smoothstepResult1523_g2 * 0.5 ) );
				float3 appendResult1908_g2 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1883_g2 = input.positionOS.xyz;
				float3 appendResult1889_g2 = (float3(break1883_g2.x , 0.0 , ( break1883_g2.z * 0.15 )));
				float mulTime1896_g2 = _TimeParameters.x * 2.3;
				float dotResult1247_g2 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1273_g2 = dotResult1247_g2;
				float dotResult1187_g2 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1201_g2 = dotResult1187_g2;
				float3 appendResult1906_g2 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1882_g2 = input.positionOS.xyz;
				float3 appendResult1887_g2 = (float3(0.0 , ( break1882_g2.y * 0.2 ) , ( break1882_g2.z * 0.4 )));
				float mulTime1891_g2 = _TimeParameters.x * 2.0;
				float dotResult1279_g2 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1295_g2 = dotResult1279_g2;
				float dotResult1282_g2 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1298_g2 = dotResult1282_g2;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult1553_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1554_g2 = _TimeParameters.x * 0.25;
				float simplePerlin2D1557_g2 = snoise( ( normalizeResult1553_g2 + mulTime1554_g2 ).xy*0.43 );
				float WindMask_LargeB1559_g2 = ( simplePerlin2D1557_g2 * 1.5 );
				float mulTime1316_g2 = _TimeParameters.x * 3.2;
				float3 temp_output_1350_0_g2 = ( ( mulTime1316_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_K1296_g2 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1300_g2 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask1418_g2 = saturate( dotResult1300_g2 );
				float mulTime1313_g2 = _TimeParameters.x * 2.3;
				float dotResult1280_g2 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1297_g2 = dotResult1280_g2;
				float dotResult1281_g2 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1294_g2 = dotResult1281_g2;
				float3 temp_output_1353_0_g2 = ( ( mulTime1313_g2 + RandomSeedVector_L1297_g2 + RandomSeedVector_M1294_g2 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1310_g2 = _TimeParameters.x * 3.6;
				float temp_output_1356_0_g2 = ( ( mulTime1310_g2 + RandomSeedVector_N1298_g2 + RandomSeedVector_O1299_g2 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1540_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1541_g2 = _TimeParameters.x * 0.26;
				float simplePerlin2D1547_g2 = snoise( ( normalizeResult1540_g2 + mulTime1541_g2 ).xy*0.7 );
				float WindMask_LargeC1552_g2 = ( simplePerlin2D1547_g2 * 1.5 );
				float3 temp_output_1450_0_g2 = ( ( ( ( rotatedValue1373_g2 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult1873_g2 + ( appendResult1862_g2 * cos( mulTime1866_g2 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1862_g2 ) ) * sin( ( mulTime1866_g2 + RandomSeedVector_K1296_g2 + RandomSeedVector_O1299_g2 ) ) ) ) * 0.1 ) * BranchMask1603_g2 * RandomIDBranchPositionMask1529_g2 ) + ( ( ( appendResult1908_g2 + ( appendResult1889_g2 * cos( ( mulTime1896_g2 + RandomSeedVector_A1273_g2 + RandomSeedVector_G1201_g2 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1889_g2 ) ) * sin( ( mulTime1896_g2 + RandomSeedVector_D1221_g2 ) ) ) ) * BranchMask1603_g2 ) * 0.1 ) + ( ( ( ( appendResult1906_g2 + ( appendResult1887_g2 * cos( mulTime1891_g2 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1887_g2 ) ) * sin( ( mulTime1891_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_N1298_g2 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1529_g2 * BranchMask1603_g2 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB1559_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) + ( ( ( ( cos( temp_output_1350_0_g2 ) * sin( temp_output_1350_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( cos( temp_output_1353_0_g2 ) * sin( temp_output_1353_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( sin( temp_output_1356_0_g2 ) * cos( temp_output_1356_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC1552_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) );
				float3 PIWI_02Gentle1454_g2 = ( temp_output_1450_0_g2 * 0.4 );
				float4 WindDirection1518_g2 = _WindDirection;
				float4 normalizeResult1466_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 break1463_g2 = (normalizeResult1466_g2).xyz;
				float4 appendResult1482_g2 = (float4(break1463_g2.x , ( break1463_g2.y + _DownwardStrength ) , break1463_g2.z , 0.0));
				float4 WindMotion_BaseG1477_g2 = ( appendResult1482_g2 * saturate( input.positionOS.xyz.y ) );
				float2 appendResult1526_g2 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1528_g2 = appendResult1526_g2;
				float GlobalVar_WindSpeed1535_g2 = _StrongWindSpeed;
				float2 NoiseRotation_Output1227_g2 = ( -(WindDirection1518_g2).xz * _TimeParameters.x * GlobalVar_WindSpeed1535_g2 );
				float2 WPRG2D_S41492_g2 = ( BasicWorldPisitionXY_Out1528_g2 + ( NoiseRotation_Output1227_g2 * 4.0 ) );
				float simplePerlin2D1469_g2 = snoise( WPRG2D_S41492_g2*0.2 );
				simplePerlin2D1469_g2 = simplePerlin2D1469_g2*0.5 + 0.5;
				float WindMotionNoise1485_g2 = simplePerlin2D1469_g2;
				float saferPower1589_g2 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1591_g2 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1589_g2 , 0.1 ) ));
				float TrunkHeightMask1593_g2 = saturate( smoothstepResult1591_g2 );
				float4 MotionBendingGentleRandom1509_g2 = ( WindMotion_BaseG1477_g2 * _MotionBendingGentleRandom * WindMotionNoise1485_g2 * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float GlobalVar_WindMotion1530_g2 = _WindMotion;
				float3 worldToObjDir1498_g2 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG1477_g2 *  (0.0 + ( GlobalVar_WindMotion1530_g2 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise1485_g2 ).xyz, 0.0 ) ).xyz;
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float3 Motion_Bending_Gentle_Wind1512_g2 = ( worldToObjDir1498_g2 * ase_objectScale * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float dotResult1248_g2 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1274_g2 = dotResult1248_g2;
				float3 appendResult1362_g2 = (float3(( sin( ( RandomSeedVector_A1273_g2 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1274_g2 ) ) * _PivotRandomnessStrength )));
				float dotResult1252_g2 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1277_g2 = dotResult1252_g2;
				float4 normalizeResult1663_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 worldToObjDir1665_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult1362_g2 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1277_g2 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult1663_g2 * input.positionOS.xyz.y * TrunkHeightMask1593_g2 ).xyz, 0.0 ) ).xyz;
				float mulTime1244_g2 = _TimeParameters.x * 4.0;
				float dotResult1188_g2 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1202_g2 = dotResult1188_g2;
				float mulTime1245_g2 = _TimeParameters.x * 5.2;
				float dotResult1189_g2 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1203_g2 = dotResult1189_g2;
				float3 rotatedValue1361_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1201_g2 * 0.02 ) + mulTime1244_g2 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1202_g2 ) ) ) + sin( ( mulTime1245_g2 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1203_g2 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ).x );
				float3 worldToObjDir1392_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue1361_g2 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1518_g2 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2147_g2 = ( ( ( worldToObjDir1665_g2 * ase_objectScale * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir1392_g2 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1423_g2 = ( float4( PIWI_02Gentle1454_g2 , 0.0 ) + MotionBendingGentleRandom1509_g2 + float4( Motion_Bending_Gentle_Wind1512_g2 , 0.0 ) + float4( PIWI_01Gentle2147_g2 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output1453_g2 = ( GlobalVar_WindStrength1531_g2 * staticSwitch1423_g2 );
				
				output.ase_texcoord3.xy = input.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1453_g2.xyz;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );
				VertexNormalInputs normalInput = GetVertexNormalInputs( input.normalOS, input.tangentOS );

				output.positionCS = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				output.normalWS = normalInput.normalWS;
				output.tangentWS = float4( normalInput.tangentWS, ( input.tangentOS.w > 0.0 ? 1.0 : -1.0 ) * GetOddNegativeScale() );

				#if defined( ENABLE_TERRAIN_PERPIXEL_NORMAL )
					output.tangentWS.zw = input.texcoord.xy;
					output.tangentWS.xy = input.texcoord.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.texcoord = input.texcoord;
				
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			void frag(	PackedVaryings input
						, out half4 outNormalWS : SV_Target0
						#if defined( ASE_DEPTH_WRITE_ON )
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						#ifdef _WRITE_RENDERING_LAYERS
						, out uint outRenderingLayers : SV_Target1
						#endif
						 )
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
				#else
					float4 shadowCoord = float4(0, 0, 0, 0);
				#endif

				// @diogo: mikktspace compliant
				float renormFactor = 1.0 / max( FLT_MIN, length( input.normalWS ) );

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( input.positionWS );
				float4 ShadowCoord = shadowCoord;
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos );
				float3 TangentWS = input.tangentWS.xyz * renormFactor;
				float3 BitangentWS = cross( input.normalWS, input.tangentWS.xyz ) * input.tangentWS.w * renormFactor;
				float3 NormalWS = input.normalWS * renormFactor;

				#if defined( ENABLE_TERRAIN_PERPIXEL_NORMAL )
					float2 sampleCoords = (input.tangentWS.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					NormalWS = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					TangentWS = -cross(GetObjectToWorldMatrix()._13_23_33, NormalWS);
					BitangentWS = cross(NormalWS, -TangentWS);
				#endif

				float2 uv_NormalMap3 = input.ase_texcoord3.xy;
				

				float3 Normal = UnpackNormalScale( tex2D( _NormalMap, uv_NormalMap3 ), 1.0f );
				float Alpha = 1;
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _Cutoff;
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					input.positionCS.z = input.positionCS.z;
				#endif

				#if defined( _ALPHATEST_ON )
					AlphaDiscard( Alpha, AlphaClipThreshold );
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					outputDepth = input.positionCS.z;
				#endif

				#if defined(_GBUFFER_NORMALS_OCT)
					float2 octNormalWS = PackNormalOctQuadEncode(NormalWS);
					float2 remappedOctNormalWS = saturate(octNormalWS * 0.5 + 0.5);
					half3 packedNormalWS = PackFloat2To888(remappedOctNormalWS);
					outNormalWS = half4(packedNormalWS, 0.0);
				#else
					#if defined(_NORMALMAP)
						#if _NORMAL_DROPOFF_TS
							float3 normalWS = TransformTangentToWorld(Normal, half3x3(TangentWS, BitangentWS, NormalWS));
						#elif _NORMAL_DROPOFF_OS
							float3 normalWS = TransformObjectToWorldNormal(Normal);
						#elif _NORMAL_DROPOFF_WS
							float3 normalWS = Normal;
						#endif
					#else
						float3 normalWS = NormalWS;
					#endif
					outNormalWS = half4(NormalizeNormalPerPixel(normalWS), 0.0);
				#endif

				#ifdef _WRITE_RENDERING_LAYERS
					outRenderingLayers = EncodeMeshRenderingLayer();
				#endif
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "GBuffer"
			Tags { "LightMode"="UniversalGBuffer" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define ASE_GEOMETRY
			#define _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma shader_feature_local_fragment _RECEIVE_SHADOWS_OFF
			#pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#pragma multi_compile_fragment _ DEBUG_DISPLAY
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_VERSION 19908
			#define ASE_SRP_VERSION 170300


			// Deferred Rendering Path does not support the OpenGL-based graphics API:
			// Desktop OpenGL, OpenGL ES 3.0, WebGL 2.0.
			#pragma exclude_renderers glcore gles3 

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
			#pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
			#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
			#pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
			#pragma multi_compile_fragment _ _SCREEN_SPACE_IRRADIANCE
			#pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
			#pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
			#pragma multi_compile_fragment _ _RENDER_PASS_ENABLED
			#pragma multi_compile _ _CLUSTER_LIGHT_LOOP

			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			#pragma multi_compile _ SHADOWS_SHADOWMASK
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ USE_LEGACY_LIGHTMAPS
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fragment _ LIGHTMAP_BICUBIC_SAMPLING
			#pragma multi_compile_fragment _ REFLECTION_PROBE_ROTATION
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#if defined( _SPECULAR_SETUP ) && defined( ASE_LIGHTING_SIMPLE )
				#if defined( _SPECULARHIGHLIGHTS_OFF )
					#undef _SPECULAR_COLOR
				#else
					#define _SPECULAR_COLOR
				#endif
			#endif

			#define SHADERPASS SHADERPASS_GBUFFER

			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ProbeVolumeVariants.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
            #endif

			#if defined( UNITY_INSTANCING_ENABLED ) && defined( ASE_INSTANCED_TERRAIN ) && ( defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL) || defined(_INSTANCEDTERRAINNORMALS_PIXEL) )
				#define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_WORLD_VIEW_DIR
			#define ASE_NEEDS_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				#if defined(LIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES1)
					float4 texcoord1 : TEXCOORD1;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES2)
					float4 texcoord2 : TEXCOORD2;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				half3 normalWS : TEXCOORD1;
				float4 tangentWS : TEXCOORD2; // holds terrainUV ifdef ENABLE_TERRAIN_PERPIXEL_NORMAL
				float4 lightmapUVOrVertexSH : TEXCOORD3;
				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					half4 fogFactorAndVertexLight : TEXCOORD4;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON)
					float2 dynamicLightmapUV : TEXCOORD5;
				#endif
				#if defined(USE_APV_PROBE_OCCLUSION)
					float4 probeOcclusion : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _CenterofMass;
			float _WINDMASKSETTINGS1;
			float _TEXTUREMAPS1;
			float _PivotRandomness;
			float _PivotRandomnessStrength;
			float _TrunkHeightandThickness;
			float _MotionBendingGentleRandom;
			float _DownwardStrength;
			float _BranchWindSmall;
			float _BranchWindLarge;
			float _BranchSwayPower;
			float _BranchMaskRadious;
			float _BranchMaskScale;
			float _RootsHardness;
			float _RootsRadius;
			float _RootsPosition;
			float _MaskRoots;
			float _Hardness;
			float _Radius;
			float _MaskRootsAuto;
			float _TTFETREEGIZMOSHADER;
			float _ADVANCEDSETTINGS;
			float _AlphaClip;
			float _Cutoff;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float _GlobalWindStrength;
			float4 _WindDirection;
			float _StrongWindSpeed;
			float _WindMotion;
			sampler2D _AlbedoMap;
			sampler2D _MaskMap;
			sampler2D _NormalMap;


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GBufferOutput.hlsl"

			float3 ASESafeNormalize(float3 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			float4 ASESafeNormalize(float4 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float GlobalVar_WindStrength1531_g2 = _GlobalWindStrength;
				float mulTime1270_g2 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1204_g2 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1220_g2 = dotResult1204_g2;
				float dotResult1206_g2 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1221_g2 = dotResult1206_g2;
				float mulTime1269_g2 = _TimeParameters.x * 4.3;
				float dotResult1205_g2 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1222_g2 = dotResult1205_g2;
				float3 normalizeResult1251_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1417_g2 = saturate( (distance( normalizeResult1251_g2 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1185_0_g2 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1199_g2 = dot( temp_output_1185_0_g2 , temp_output_1185_0_g2 );
				float saferPower1228_g2 = abs( saturate( dotResult1199_g2 ) );
				float temp_output_1249_0_g2 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1228_g2 , _Hardness ) );
				float3 normalizeResult1177_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1416_g2 = saturate( (distance( normalizeResult1177_g2 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1414_g2 = (( _CenterofMass )?( ( temp_output_1249_0_g2 * CenterOfMass1416_g2 ) ):( temp_output_1249_0_g2 ));
				float saferPower2084_g2 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2055_g2 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2084_g2 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2110_g2 = float3( 0, -1, 0 );
				float3 appendResult2113_g2 = (float3(break2110_g2.x , ( break2110_g2.y * _RootsPosition ) , break2110_g2.z));
				float3 temp_output_2117_0_g2 = ( ( input.positionOS.xyz - appendResult2113_g2 ) / _RootsRadius );
				float dotResult2118_g2 = dot( temp_output_2117_0_g2 , temp_output_2117_0_g2 );
				float saferPower2121_g2 = abs( saturate( dotResult2118_g2 ) );
				float RootsMask_ProxySphere2122_g2 = pow( saferPower2121_g2 , _RootsHardness );
				float smoothstepResult1602_g2 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask1603_g2 = smoothstepResult1602_g2;
				float3 rotatedValue1373_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1270_g2 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1220_g2 ) + ( RandomSeedVector_D1221_g2 * 0.02 ) ) ) + sin( ( mulTime1269_g2 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1222_g2 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1417_g2 * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * BranchMask1603_g2 ).x );
				float3 appendResult1873_g2 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1860_g2 = input.positionOS.xyz;
				float3 appendResult1862_g2 = (float3(break1860_g2.x , ( break1860_g2.y * 0.15 ) , 0.0));
				float mulTime1866_g2 = _TimeParameters.x * 2.1;
				float dotResult1278_g2 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1296_g2 = dotResult1278_g2;
				float dotResult1283_g2 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1299_g2 = dotResult1283_g2;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1515_g2 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1523_g2 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1515_g2 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1515_g2 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1529_g2 = saturate( ( smoothstepResult1523_g2 * 0.5 ) );
				float3 appendResult1908_g2 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1883_g2 = input.positionOS.xyz;
				float3 appendResult1889_g2 = (float3(break1883_g2.x , 0.0 , ( break1883_g2.z * 0.15 )));
				float mulTime1896_g2 = _TimeParameters.x * 2.3;
				float dotResult1247_g2 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1273_g2 = dotResult1247_g2;
				float dotResult1187_g2 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1201_g2 = dotResult1187_g2;
				float3 appendResult1906_g2 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1882_g2 = input.positionOS.xyz;
				float3 appendResult1887_g2 = (float3(0.0 , ( break1882_g2.y * 0.2 ) , ( break1882_g2.z * 0.4 )));
				float mulTime1891_g2 = _TimeParameters.x * 2.0;
				float dotResult1279_g2 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1295_g2 = dotResult1279_g2;
				float dotResult1282_g2 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1298_g2 = dotResult1282_g2;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult1553_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1554_g2 = _TimeParameters.x * 0.25;
				float simplePerlin2D1557_g2 = snoise( ( normalizeResult1553_g2 + mulTime1554_g2 ).xy*0.43 );
				float WindMask_LargeB1559_g2 = ( simplePerlin2D1557_g2 * 1.5 );
				float mulTime1316_g2 = _TimeParameters.x * 3.2;
				float3 temp_output_1350_0_g2 = ( ( mulTime1316_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_K1296_g2 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1300_g2 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask1418_g2 = saturate( dotResult1300_g2 );
				float mulTime1313_g2 = _TimeParameters.x * 2.3;
				float dotResult1280_g2 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1297_g2 = dotResult1280_g2;
				float dotResult1281_g2 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1294_g2 = dotResult1281_g2;
				float3 temp_output_1353_0_g2 = ( ( mulTime1313_g2 + RandomSeedVector_L1297_g2 + RandomSeedVector_M1294_g2 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1310_g2 = _TimeParameters.x * 3.6;
				float temp_output_1356_0_g2 = ( ( mulTime1310_g2 + RandomSeedVector_N1298_g2 + RandomSeedVector_O1299_g2 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1540_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1541_g2 = _TimeParameters.x * 0.26;
				float simplePerlin2D1547_g2 = snoise( ( normalizeResult1540_g2 + mulTime1541_g2 ).xy*0.7 );
				float WindMask_LargeC1552_g2 = ( simplePerlin2D1547_g2 * 1.5 );
				float3 temp_output_1450_0_g2 = ( ( ( ( rotatedValue1373_g2 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult1873_g2 + ( appendResult1862_g2 * cos( mulTime1866_g2 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1862_g2 ) ) * sin( ( mulTime1866_g2 + RandomSeedVector_K1296_g2 + RandomSeedVector_O1299_g2 ) ) ) ) * 0.1 ) * BranchMask1603_g2 * RandomIDBranchPositionMask1529_g2 ) + ( ( ( appendResult1908_g2 + ( appendResult1889_g2 * cos( ( mulTime1896_g2 + RandomSeedVector_A1273_g2 + RandomSeedVector_G1201_g2 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1889_g2 ) ) * sin( ( mulTime1896_g2 + RandomSeedVector_D1221_g2 ) ) ) ) * BranchMask1603_g2 ) * 0.1 ) + ( ( ( ( appendResult1906_g2 + ( appendResult1887_g2 * cos( mulTime1891_g2 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1887_g2 ) ) * sin( ( mulTime1891_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_N1298_g2 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1529_g2 * BranchMask1603_g2 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB1559_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) + ( ( ( ( cos( temp_output_1350_0_g2 ) * sin( temp_output_1350_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( cos( temp_output_1353_0_g2 ) * sin( temp_output_1353_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( sin( temp_output_1356_0_g2 ) * cos( temp_output_1356_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC1552_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) );
				float3 PIWI_02Gentle1454_g2 = ( temp_output_1450_0_g2 * 0.4 );
				float4 WindDirection1518_g2 = _WindDirection;
				float4 normalizeResult1466_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 break1463_g2 = (normalizeResult1466_g2).xyz;
				float4 appendResult1482_g2 = (float4(break1463_g2.x , ( break1463_g2.y + _DownwardStrength ) , break1463_g2.z , 0.0));
				float4 WindMotion_BaseG1477_g2 = ( appendResult1482_g2 * saturate( input.positionOS.xyz.y ) );
				float2 appendResult1526_g2 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1528_g2 = appendResult1526_g2;
				float GlobalVar_WindSpeed1535_g2 = _StrongWindSpeed;
				float2 NoiseRotation_Output1227_g2 = ( -(WindDirection1518_g2).xz * _TimeParameters.x * GlobalVar_WindSpeed1535_g2 );
				float2 WPRG2D_S41492_g2 = ( BasicWorldPisitionXY_Out1528_g2 + ( NoiseRotation_Output1227_g2 * 4.0 ) );
				float simplePerlin2D1469_g2 = snoise( WPRG2D_S41492_g2*0.2 );
				simplePerlin2D1469_g2 = simplePerlin2D1469_g2*0.5 + 0.5;
				float WindMotionNoise1485_g2 = simplePerlin2D1469_g2;
				float saferPower1589_g2 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1591_g2 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1589_g2 , 0.1 ) ));
				float TrunkHeightMask1593_g2 = saturate( smoothstepResult1591_g2 );
				float4 MotionBendingGentleRandom1509_g2 = ( WindMotion_BaseG1477_g2 * _MotionBendingGentleRandom * WindMotionNoise1485_g2 * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float GlobalVar_WindMotion1530_g2 = _WindMotion;
				float3 worldToObjDir1498_g2 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG1477_g2 *  (0.0 + ( GlobalVar_WindMotion1530_g2 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise1485_g2 ).xyz, 0.0 ) ).xyz;
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float3 Motion_Bending_Gentle_Wind1512_g2 = ( worldToObjDir1498_g2 * ase_objectScale * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float dotResult1248_g2 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1274_g2 = dotResult1248_g2;
				float3 appendResult1362_g2 = (float3(( sin( ( RandomSeedVector_A1273_g2 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1274_g2 ) ) * _PivotRandomnessStrength )));
				float dotResult1252_g2 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1277_g2 = dotResult1252_g2;
				float4 normalizeResult1663_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 worldToObjDir1665_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult1362_g2 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1277_g2 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult1663_g2 * input.positionOS.xyz.y * TrunkHeightMask1593_g2 ).xyz, 0.0 ) ).xyz;
				float mulTime1244_g2 = _TimeParameters.x * 4.0;
				float dotResult1188_g2 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1202_g2 = dotResult1188_g2;
				float mulTime1245_g2 = _TimeParameters.x * 5.2;
				float dotResult1189_g2 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1203_g2 = dotResult1189_g2;
				float3 rotatedValue1361_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1201_g2 * 0.02 ) + mulTime1244_g2 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1202_g2 ) ) ) + sin( ( mulTime1245_g2 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1203_g2 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ).x );
				float3 worldToObjDir1392_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue1361_g2 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1518_g2 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2147_g2 = ( ( ( worldToObjDir1665_g2 * ase_objectScale * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir1392_g2 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1423_g2 = ( float4( PIWI_02Gentle1454_g2 , 0.0 ) + MotionBendingGentleRandom1509_g2 + float4( Motion_Bending_Gentle_Wind1512_g2 , 0.0 ) + float4( PIWI_01Gentle2147_g2 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output1453_g2 = ( GlobalVar_WindStrength1531_g2 * staticSwitch1423_g2 );
				
				output.ase_texcoord7.xy = input.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord7.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1453_g2.xyz;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;
				input.tangentOS = input.tangentOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );
				VertexNormalInputs normalInput = GetVertexNormalInputs( input.normalOS, input.tangentOS );

				OUTPUT_LIGHTMAP_UV(input.texcoord1, unity_LightmapST, output.lightmapUVOrVertexSH.xy);
				#if defined(DYNAMICLIGHTMAP_ON)
					output.dynamicLightmapUV.xy = input.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				#endif
				OUTPUT_SH4(vertexInput.positionWS, normalInput.normalWS.xyz, GetWorldSpaceNormalizeViewDir(vertexInput.positionWS), output.lightmapUVOrVertexSH.xyz, output.probeOcclusion);

				#if defined(ASE_FOG) || defined(_ADDITIONAL_LIGHTS_VERTEX)
					output.fogFactorAndVertexLight = 0;
					#if defined(ASE_FOG) && !defined(_FOG_FRAGMENT)
						// @diogo: no fog applied in GBuffer
					#endif
					#ifdef _ADDITIONAL_LIGHTS_VERTEX
						half3 vertexLight = VertexLighting( vertexInput.positionWS, normalInput.normalWS );
						output.fogFactorAndVertexLight.yzw = vertexLight;
					#endif
				#endif

				output.positionCS = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				output.normalWS = normalInput.normalWS;
				output.tangentWS = float4( normalInput.tangentWS, ( input.tangentOS.w > 0.0 ? 1.0 : -1.0 ) * GetOddNegativeScale() );

				#if defined( ENABLE_TERRAIN_PERPIXEL_NORMAL )
					output.tangentWS.zw = input.texcoord.xy;
					output.tangentWS.xy = input.texcoord.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				#if defined(LIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES1)
					float4 texcoord1 : TEXCOORD1;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES2)
					float4 texcoord2 : TEXCOORD2;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				output.texcoord = input.texcoord;
				#if defined(LIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES1)
					output.texcoord1 = input.texcoord1;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES2)
					output.texcoord2 = input.texcoord2;
				#endif
				
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				output.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				#if defined(LIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES1)
					output.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				#endif
				#if defined(DYNAMICLIGHTMAP_ON) || defined(ASE_NEEDS_TEXTURE_COORDINATES2)
					output.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				#endif
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			GBufferFragOutput frag ( PackedVaryings input
								#if defined( ASE_DEPTH_WRITE_ON )
								,out float outputDepth : ASE_SV_DEPTH
								#endif
								 )
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					float4 shadowCoord = TransformWorldToShadowCoord( input.positionWS );
				#else
					float4 shadowCoord = float4(0, 0, 0, 0);
				#endif

				// @diogo: mikktspace compliant
				float renormFactor = 1.0 / max( FLT_MIN, length( input.normalWS ) );

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( PositionWS );
				float3 ViewDirWS = GetWorldSpaceNormalizeViewDir( PositionWS );
				float4 ShadowCoord = shadowCoord;
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos );
				float3 TangentWS = input.tangentWS.xyz * renormFactor;
				float3 BitangentWS = cross( input.normalWS, input.tangentWS.xyz ) * input.tangentWS.w * renormFactor;
				float3 NormalWS = input.normalWS * renormFactor;

				#if defined( ENABLE_TERRAIN_PERPIXEL_NORMAL )
					float2 sampleCoords = (input.tangentWS.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					NormalWS = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					TangentWS = -cross(GetObjectToWorldMatrix()._13_23_33, NormalWS);
					BitangentWS = cross(NormalWS, -TangentWS);
				#endif

				float2 uv_AlbedoMap2 = input.ase_texcoord7.xy;
				float4 tex2DNode2 = tex2D( _AlbedoMap, uv_AlbedoMap2 );
				float2 uv_MaskMap4 = input.ase_texcoord7.xy;
				float4 tex2DNode4 = tex2D( _MaskMap, uv_MaskMap4 );
				
				float2 uv_NormalMap3 = input.ase_texcoord7.xy;
				
				float CustomDRAWERS54 = ( _TTFETREEGIZMOSHADER + _TEXTUREMAPS1 + _ADVANCEDSETTINGS );
				
				float4 color10 = IsGammaSpace() ? float4( 0.2156863, 0.5607843, 0.2, 1 ) : float4( 0.03820438, 0.2746773, 0.03310476, 1 );
				float fresnelNdotV5 = dot( NormalWS, ViewDirWS );
				float fresnelNode5 = ( -0.1 + 5.0 * pow( 1.0 - fresnelNdotV5, 5.0 ) );
				

				float3 BaseColor = ( tex2DNode2 * saturate( tex2DNode4.g ) ).rgb;
				float3 Normal = UnpackNormalScale( tex2D( _NormalMap, uv_NormalMap3 ), 1.0f );
				float3 Specular = 0.5;
				float Metallic = CustomDRAWERS54;
				float Smoothness = tex2DNode4.a;
				float Occlusion = tex2DNode4.g;
				float3 Emission = ( saturate( ( color10 * fresnelNode5 ) ) + (tex2DNode2*0.4 + 0.0) ).rgb;
				float Alpha = 1;
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _Cutoff;
					float AlphaClipThresholdShadow = 0.5;
				#endif
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;

				#if defined( ASE_DEPTH_WRITE_ON )
					input.positionCS.z = input.positionCS.z;
				#endif

				#if defined( _ALPHATEST_ON )
					AlphaDiscard( Alpha, AlphaClipThreshold );
				#endif

				#if defined(MAIN_LIGHT_CALCULATE_SHADOWS) && defined(ASE_CHANGES_WORLD_POS)
					ShadowCoord = TransformWorldToShadowCoord( PositionWS );
				#endif

				InputData inputData = (InputData)0;
				inputData.positionWS = PositionWS;
				inputData.positionCS = input.positionCS;
				inputData.normalizedScreenSpaceUV = ScreenPosNorm.xy;
				inputData.shadowCoord = ShadowCoord;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
						inputData.normalWS = TransformTangentToWorld(Normal, half3x3( TangentWS, BitangentWS, NormalWS ));
					#elif _NORMAL_DROPOFF_OS
						inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
						inputData.normalWS = Normal;
					#endif
				#else
					inputData.normalWS = NormalWS;
				#endif

				inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				inputData.viewDirectionWS = SafeNormalize( ViewDirWS );

				#ifdef ASE_FOG
					// @diogo: no fog applied in GBuffer
				#endif
				#ifdef _ADDITIONAL_LIGHTS_VERTEX
					inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
				#endif

				#if defined( ENABLE_TERRAIN_PERPIXEL_NORMAL )
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = input.lightmapUVOrVertexSH.xyz;
				#endif

				#if defined(_SCREEN_SPACE_IRRADIANCE)
					inputData.bakedGI = SAMPLE_GI(_ScreenSpaceIrradiance, input.positionCS.xy);
				#elif defined(DYNAMICLIGHTMAP_ON)
					inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, input.dynamicLightmapUV.xy, SH, inputData.normalWS);
					inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUVOrVertexSH.xy);
				#elif !defined(LIGHTMAP_ON) && (defined(PROBE_VOLUMES_L1) || defined(PROBE_VOLUMES_L2))
					inputData.bakedGI = SAMPLE_GI(SH,
						GetAbsolutePositionWS(inputData.positionWS),
						inputData.normalWS,
						inputData.viewDirectionWS,
						input.positionCS.xy,
						input.probeOcclusion,
						inputData.shadowMask);
				#else
					inputData.bakedGI = SAMPLE_GI(input.lightmapUVOrVertexSH.xy, SH, inputData.normalWS);
					inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUVOrVertexSH.xy);
				#endif

				#ifdef ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif

				#if defined(DEBUG_DISPLAY)
					#if defined(DYNAMICLIGHTMAP_ON)
						inputData.dynamicLightmapUV = input.dynamicLightmapUV.xy;
						#endif
					#if defined(LIGHTMAP_ON)
						inputData.staticLightmapUV = input.lightmapUVOrVertexSH.xy;
					#else
						inputData.vertexSH = SH;
					#endif
					#if defined(USE_APV_PROBE_OCCLUSION)
						inputData.probeOcclusion = input.probeOcclusion;
					#endif
				#endif

				#ifdef _DBUFFER
					ApplyDecal(input.positionCS,
						BaseColor,
						Specular,
						inputData.normalWS,
						Metallic,
						Occlusion,
						Smoothness);
				#endif

				BRDFData brdfData;
				InitializeBRDFData(BaseColor, Metallic, Specular, Smoothness, Alpha, brdfData);

				Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, inputData.shadowMask);
				half4 color;
				MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, inputData.shadowMask);

				color.rgb = GlobalIllumination(brdfData, (BRDFData)0, 0,
                              inputData.bakedGI, Occlusion, inputData.positionWS,
                              inputData.normalWS, inputData.viewDirectionWS, inputData.normalizedScreenSpaceUV);

				color.a = Alpha;

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					outputDepth = input.positionCS.z;
				#endif

				return PackGBuffersBRDFData(brdfData, inputData, Smoothness, Emission + color.rgb, Occlusion);
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "SceneSelectionPass"
			Tags { "LightMode"="SceneSelectionPass" }

			Cull Off
			AlphaToMask Off

			HLSLPROGRAM

			#define ASE_GEOMETRY
			#define _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#pragma multi_compile_fragment _ DEBUG_DISPLAY
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_VERSION 19908
			#define ASE_SRP_VERSION 170300


			#pragma vertex vert
			#pragma fragment frag

			#if defined( _SPECULAR_SETUP ) && defined( ASE_LIGHTING_SIMPLE )
				#if defined( _SPECULARHIGHLIGHTS_OFF )
					#undef _SPECULAR_COLOR
				#else
					#define _SPECULAR_COLOR
				#endif
			#endif

			#define SCENESELECTIONPASS 1

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _CenterofMass;
			float _WINDMASKSETTINGS1;
			float _TEXTUREMAPS1;
			float _PivotRandomness;
			float _PivotRandomnessStrength;
			float _TrunkHeightandThickness;
			float _MotionBendingGentleRandom;
			float _DownwardStrength;
			float _BranchWindSmall;
			float _BranchWindLarge;
			float _BranchSwayPower;
			float _BranchMaskRadious;
			float _BranchMaskScale;
			float _RootsHardness;
			float _RootsRadius;
			float _RootsPosition;
			float _MaskRoots;
			float _Hardness;
			float _Radius;
			float _MaskRootsAuto;
			float _TTFETREEGIZMOSHADER;
			float _ADVANCEDSETTINGS;
			float _AlphaClip;
			float _Cutoff;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float _GlobalWindStrength;
			float4 _WindDirection;
			float _StrongWindSpeed;
			float _WindMotion;


			float3 ASESafeNormalize(float3 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			float4 ASESafeNormalize(float4 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction(Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float GlobalVar_WindStrength1531_g2 = _GlobalWindStrength;
				float mulTime1270_g2 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1204_g2 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1220_g2 = dotResult1204_g2;
				float dotResult1206_g2 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1221_g2 = dotResult1206_g2;
				float mulTime1269_g2 = _TimeParameters.x * 4.3;
				float dotResult1205_g2 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1222_g2 = dotResult1205_g2;
				float3 normalizeResult1251_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1417_g2 = saturate( (distance( normalizeResult1251_g2 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1185_0_g2 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1199_g2 = dot( temp_output_1185_0_g2 , temp_output_1185_0_g2 );
				float saferPower1228_g2 = abs( saturate( dotResult1199_g2 ) );
				float temp_output_1249_0_g2 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1228_g2 , _Hardness ) );
				float3 normalizeResult1177_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1416_g2 = saturate( (distance( normalizeResult1177_g2 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1414_g2 = (( _CenterofMass )?( ( temp_output_1249_0_g2 * CenterOfMass1416_g2 ) ):( temp_output_1249_0_g2 ));
				float saferPower2084_g2 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2055_g2 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2084_g2 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2110_g2 = float3( 0, -1, 0 );
				float3 appendResult2113_g2 = (float3(break2110_g2.x , ( break2110_g2.y * _RootsPosition ) , break2110_g2.z));
				float3 temp_output_2117_0_g2 = ( ( input.positionOS.xyz - appendResult2113_g2 ) / _RootsRadius );
				float dotResult2118_g2 = dot( temp_output_2117_0_g2 , temp_output_2117_0_g2 );
				float saferPower2121_g2 = abs( saturate( dotResult2118_g2 ) );
				float RootsMask_ProxySphere2122_g2 = pow( saferPower2121_g2 , _RootsHardness );
				float smoothstepResult1602_g2 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask1603_g2 = smoothstepResult1602_g2;
				float3 rotatedValue1373_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1270_g2 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1220_g2 ) + ( RandomSeedVector_D1221_g2 * 0.02 ) ) ) + sin( ( mulTime1269_g2 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1222_g2 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1417_g2 * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * BranchMask1603_g2 ).x );
				float3 appendResult1873_g2 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1860_g2 = input.positionOS.xyz;
				float3 appendResult1862_g2 = (float3(break1860_g2.x , ( break1860_g2.y * 0.15 ) , 0.0));
				float mulTime1866_g2 = _TimeParameters.x * 2.1;
				float dotResult1278_g2 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1296_g2 = dotResult1278_g2;
				float dotResult1283_g2 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1299_g2 = dotResult1283_g2;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1515_g2 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1523_g2 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1515_g2 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1515_g2 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1529_g2 = saturate( ( smoothstepResult1523_g2 * 0.5 ) );
				float3 appendResult1908_g2 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1883_g2 = input.positionOS.xyz;
				float3 appendResult1889_g2 = (float3(break1883_g2.x , 0.0 , ( break1883_g2.z * 0.15 )));
				float mulTime1896_g2 = _TimeParameters.x * 2.3;
				float dotResult1247_g2 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1273_g2 = dotResult1247_g2;
				float dotResult1187_g2 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1201_g2 = dotResult1187_g2;
				float3 appendResult1906_g2 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1882_g2 = input.positionOS.xyz;
				float3 appendResult1887_g2 = (float3(0.0 , ( break1882_g2.y * 0.2 ) , ( break1882_g2.z * 0.4 )));
				float mulTime1891_g2 = _TimeParameters.x * 2.0;
				float dotResult1279_g2 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1295_g2 = dotResult1279_g2;
				float dotResult1282_g2 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1298_g2 = dotResult1282_g2;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult1553_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1554_g2 = _TimeParameters.x * 0.25;
				float simplePerlin2D1557_g2 = snoise( ( normalizeResult1553_g2 + mulTime1554_g2 ).xy*0.43 );
				float WindMask_LargeB1559_g2 = ( simplePerlin2D1557_g2 * 1.5 );
				float mulTime1316_g2 = _TimeParameters.x * 3.2;
				float3 temp_output_1350_0_g2 = ( ( mulTime1316_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_K1296_g2 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1300_g2 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask1418_g2 = saturate( dotResult1300_g2 );
				float mulTime1313_g2 = _TimeParameters.x * 2.3;
				float dotResult1280_g2 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1297_g2 = dotResult1280_g2;
				float dotResult1281_g2 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1294_g2 = dotResult1281_g2;
				float3 temp_output_1353_0_g2 = ( ( mulTime1313_g2 + RandomSeedVector_L1297_g2 + RandomSeedVector_M1294_g2 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1310_g2 = _TimeParameters.x * 3.6;
				float temp_output_1356_0_g2 = ( ( mulTime1310_g2 + RandomSeedVector_N1298_g2 + RandomSeedVector_O1299_g2 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1540_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1541_g2 = _TimeParameters.x * 0.26;
				float simplePerlin2D1547_g2 = snoise( ( normalizeResult1540_g2 + mulTime1541_g2 ).xy*0.7 );
				float WindMask_LargeC1552_g2 = ( simplePerlin2D1547_g2 * 1.5 );
				float3 temp_output_1450_0_g2 = ( ( ( ( rotatedValue1373_g2 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult1873_g2 + ( appendResult1862_g2 * cos( mulTime1866_g2 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1862_g2 ) ) * sin( ( mulTime1866_g2 + RandomSeedVector_K1296_g2 + RandomSeedVector_O1299_g2 ) ) ) ) * 0.1 ) * BranchMask1603_g2 * RandomIDBranchPositionMask1529_g2 ) + ( ( ( appendResult1908_g2 + ( appendResult1889_g2 * cos( ( mulTime1896_g2 + RandomSeedVector_A1273_g2 + RandomSeedVector_G1201_g2 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1889_g2 ) ) * sin( ( mulTime1896_g2 + RandomSeedVector_D1221_g2 ) ) ) ) * BranchMask1603_g2 ) * 0.1 ) + ( ( ( ( appendResult1906_g2 + ( appendResult1887_g2 * cos( mulTime1891_g2 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1887_g2 ) ) * sin( ( mulTime1891_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_N1298_g2 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1529_g2 * BranchMask1603_g2 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB1559_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) + ( ( ( ( cos( temp_output_1350_0_g2 ) * sin( temp_output_1350_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( cos( temp_output_1353_0_g2 ) * sin( temp_output_1353_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( sin( temp_output_1356_0_g2 ) * cos( temp_output_1356_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC1552_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) );
				float3 PIWI_02Gentle1454_g2 = ( temp_output_1450_0_g2 * 0.4 );
				float4 WindDirection1518_g2 = _WindDirection;
				float4 normalizeResult1466_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 break1463_g2 = (normalizeResult1466_g2).xyz;
				float4 appendResult1482_g2 = (float4(break1463_g2.x , ( break1463_g2.y + _DownwardStrength ) , break1463_g2.z , 0.0));
				float4 WindMotion_BaseG1477_g2 = ( appendResult1482_g2 * saturate( input.positionOS.xyz.y ) );
				float2 appendResult1526_g2 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1528_g2 = appendResult1526_g2;
				float GlobalVar_WindSpeed1535_g2 = _StrongWindSpeed;
				float2 NoiseRotation_Output1227_g2 = ( -(WindDirection1518_g2).xz * _TimeParameters.x * GlobalVar_WindSpeed1535_g2 );
				float2 WPRG2D_S41492_g2 = ( BasicWorldPisitionXY_Out1528_g2 + ( NoiseRotation_Output1227_g2 * 4.0 ) );
				float simplePerlin2D1469_g2 = snoise( WPRG2D_S41492_g2*0.2 );
				simplePerlin2D1469_g2 = simplePerlin2D1469_g2*0.5 + 0.5;
				float WindMotionNoise1485_g2 = simplePerlin2D1469_g2;
				float saferPower1589_g2 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1591_g2 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1589_g2 , 0.1 ) ));
				float TrunkHeightMask1593_g2 = saturate( smoothstepResult1591_g2 );
				float4 MotionBendingGentleRandom1509_g2 = ( WindMotion_BaseG1477_g2 * _MotionBendingGentleRandom * WindMotionNoise1485_g2 * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float GlobalVar_WindMotion1530_g2 = _WindMotion;
				float3 worldToObjDir1498_g2 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG1477_g2 *  (0.0 + ( GlobalVar_WindMotion1530_g2 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise1485_g2 ).xyz, 0.0 ) ).xyz;
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float3 Motion_Bending_Gentle_Wind1512_g2 = ( worldToObjDir1498_g2 * ase_objectScale * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float dotResult1248_g2 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1274_g2 = dotResult1248_g2;
				float3 appendResult1362_g2 = (float3(( sin( ( RandomSeedVector_A1273_g2 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1274_g2 ) ) * _PivotRandomnessStrength )));
				float dotResult1252_g2 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1277_g2 = dotResult1252_g2;
				float4 normalizeResult1663_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 worldToObjDir1665_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult1362_g2 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1277_g2 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult1663_g2 * input.positionOS.xyz.y * TrunkHeightMask1593_g2 ).xyz, 0.0 ) ).xyz;
				float mulTime1244_g2 = _TimeParameters.x * 4.0;
				float dotResult1188_g2 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1202_g2 = dotResult1188_g2;
				float mulTime1245_g2 = _TimeParameters.x * 5.2;
				float dotResult1189_g2 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1203_g2 = dotResult1189_g2;
				float3 rotatedValue1361_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1201_g2 * 0.02 ) + mulTime1244_g2 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1202_g2 ) ) ) + sin( ( mulTime1245_g2 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1203_g2 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ).x );
				float3 worldToObjDir1392_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue1361_g2 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1518_g2 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2147_g2 = ( ( ( worldToObjDir1665_g2 * ase_objectScale * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir1392_g2 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1423_g2 = ( float4( PIWI_02Gentle1454_g2 , 0.0 ) + MotionBendingGentleRandom1509_g2 + float4( Motion_Bending_Gentle_Wind1512_g2 , 0.0 ) + float4( PIWI_01Gentle2147_g2 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output1453_g2 = ( GlobalVar_WindStrength1531_g2 * staticSwitch1423_g2 );
				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1453_g2.xyz;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				output.positionCS = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag( PackedVaryings input
				#if defined( ASE_DEPTH_WRITE_ON )
				,out float outputDepth : ASE_SV_DEPTH
				#endif
				 ) : SV_Target
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( PositionWS );
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;

				

				surfaceDescription.Alpha = 1;
				#if defined( _ALPHATEST_ON )
					surfaceDescription.AlphaClipThreshold = _Cutoff;
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					input.positionCS.z = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					outputDepth = input.positionCS.z;
				#endif

				return half4( _ObjectId, _PassValue, 1.0, 1.0 );
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ScenePickingPass"
			Tags { "LightMode"="Picking" }

			AlphaToMask Off

			HLSLPROGRAM

			#define ASE_GEOMETRY
			#define _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#define ASE_FOG 1
			#pragma multi_compile_fragment _ DEBUG_DISPLAY
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_VERSION 19908
			#define ASE_SRP_VERSION 170300


			#pragma vertex vert
			#pragma fragment frag

			#if defined( _SPECULAR_SETUP ) && defined( ASE_LIGHTING_SIMPLE )
				#if defined( _SPECULARHIGHLIGHTS_OFF )
					#undef _SPECULAR_COLOR
				#else
					#define _SPECULAR_COLOR
				#endif
			#endif

		    #define SCENEPICKINGPASS 1

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _CenterofMass;
			float _WINDMASKSETTINGS1;
			float _TEXTUREMAPS1;
			float _PivotRandomness;
			float _PivotRandomnessStrength;
			float _TrunkHeightandThickness;
			float _MotionBendingGentleRandom;
			float _DownwardStrength;
			float _BranchWindSmall;
			float _BranchWindLarge;
			float _BranchSwayPower;
			float _BranchMaskRadious;
			float _BranchMaskScale;
			float _RootsHardness;
			float _RootsRadius;
			float _RootsPosition;
			float _MaskRoots;
			float _Hardness;
			float _Radius;
			float _MaskRootsAuto;
			float _TTFETREEGIZMOSHADER;
			float _ADVANCEDSETTINGS;
			float _AlphaClip;
			float _Cutoff;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float _GlobalWindStrength;
			float4 _WindDirection;
			float _StrongWindSpeed;
			float _WindMotion;


			float3 ASESafeNormalize(float3 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			float4 ASESafeNormalize(float4 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output;
				ZERO_INITIALIZE(PackedVaryings, output);

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float GlobalVar_WindStrength1531_g2 = _GlobalWindStrength;
				float mulTime1270_g2 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1204_g2 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1220_g2 = dotResult1204_g2;
				float dotResult1206_g2 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1221_g2 = dotResult1206_g2;
				float mulTime1269_g2 = _TimeParameters.x * 4.3;
				float dotResult1205_g2 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1222_g2 = dotResult1205_g2;
				float3 normalizeResult1251_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1417_g2 = saturate( (distance( normalizeResult1251_g2 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1185_0_g2 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1199_g2 = dot( temp_output_1185_0_g2 , temp_output_1185_0_g2 );
				float saferPower1228_g2 = abs( saturate( dotResult1199_g2 ) );
				float temp_output_1249_0_g2 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1228_g2 , _Hardness ) );
				float3 normalizeResult1177_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1416_g2 = saturate( (distance( normalizeResult1177_g2 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1414_g2 = (( _CenterofMass )?( ( temp_output_1249_0_g2 * CenterOfMass1416_g2 ) ):( temp_output_1249_0_g2 ));
				float saferPower2084_g2 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2055_g2 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2084_g2 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2110_g2 = float3( 0, -1, 0 );
				float3 appendResult2113_g2 = (float3(break2110_g2.x , ( break2110_g2.y * _RootsPosition ) , break2110_g2.z));
				float3 temp_output_2117_0_g2 = ( ( input.positionOS.xyz - appendResult2113_g2 ) / _RootsRadius );
				float dotResult2118_g2 = dot( temp_output_2117_0_g2 , temp_output_2117_0_g2 );
				float saferPower2121_g2 = abs( saturate( dotResult2118_g2 ) );
				float RootsMask_ProxySphere2122_g2 = pow( saferPower2121_g2 , _RootsHardness );
				float smoothstepResult1602_g2 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask1603_g2 = smoothstepResult1602_g2;
				float3 rotatedValue1373_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1270_g2 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1220_g2 ) + ( RandomSeedVector_D1221_g2 * 0.02 ) ) ) + sin( ( mulTime1269_g2 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1222_g2 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1417_g2 * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * BranchMask1603_g2 ).x );
				float3 appendResult1873_g2 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1860_g2 = input.positionOS.xyz;
				float3 appendResult1862_g2 = (float3(break1860_g2.x , ( break1860_g2.y * 0.15 ) , 0.0));
				float mulTime1866_g2 = _TimeParameters.x * 2.1;
				float dotResult1278_g2 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1296_g2 = dotResult1278_g2;
				float dotResult1283_g2 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1299_g2 = dotResult1283_g2;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1515_g2 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1523_g2 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1515_g2 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1515_g2 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1529_g2 = saturate( ( smoothstepResult1523_g2 * 0.5 ) );
				float3 appendResult1908_g2 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1883_g2 = input.positionOS.xyz;
				float3 appendResult1889_g2 = (float3(break1883_g2.x , 0.0 , ( break1883_g2.z * 0.15 )));
				float mulTime1896_g2 = _TimeParameters.x * 2.3;
				float dotResult1247_g2 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1273_g2 = dotResult1247_g2;
				float dotResult1187_g2 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1201_g2 = dotResult1187_g2;
				float3 appendResult1906_g2 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1882_g2 = input.positionOS.xyz;
				float3 appendResult1887_g2 = (float3(0.0 , ( break1882_g2.y * 0.2 ) , ( break1882_g2.z * 0.4 )));
				float mulTime1891_g2 = _TimeParameters.x * 2.0;
				float dotResult1279_g2 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1295_g2 = dotResult1279_g2;
				float dotResult1282_g2 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1298_g2 = dotResult1282_g2;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult1553_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1554_g2 = _TimeParameters.x * 0.25;
				float simplePerlin2D1557_g2 = snoise( ( normalizeResult1553_g2 + mulTime1554_g2 ).xy*0.43 );
				float WindMask_LargeB1559_g2 = ( simplePerlin2D1557_g2 * 1.5 );
				float mulTime1316_g2 = _TimeParameters.x * 3.2;
				float3 temp_output_1350_0_g2 = ( ( mulTime1316_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_K1296_g2 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1300_g2 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask1418_g2 = saturate( dotResult1300_g2 );
				float mulTime1313_g2 = _TimeParameters.x * 2.3;
				float dotResult1280_g2 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1297_g2 = dotResult1280_g2;
				float dotResult1281_g2 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1294_g2 = dotResult1281_g2;
				float3 temp_output_1353_0_g2 = ( ( mulTime1313_g2 + RandomSeedVector_L1297_g2 + RandomSeedVector_M1294_g2 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1310_g2 = _TimeParameters.x * 3.6;
				float temp_output_1356_0_g2 = ( ( mulTime1310_g2 + RandomSeedVector_N1298_g2 + RandomSeedVector_O1299_g2 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1540_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1541_g2 = _TimeParameters.x * 0.26;
				float simplePerlin2D1547_g2 = snoise( ( normalizeResult1540_g2 + mulTime1541_g2 ).xy*0.7 );
				float WindMask_LargeC1552_g2 = ( simplePerlin2D1547_g2 * 1.5 );
				float3 temp_output_1450_0_g2 = ( ( ( ( rotatedValue1373_g2 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult1873_g2 + ( appendResult1862_g2 * cos( mulTime1866_g2 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1862_g2 ) ) * sin( ( mulTime1866_g2 + RandomSeedVector_K1296_g2 + RandomSeedVector_O1299_g2 ) ) ) ) * 0.1 ) * BranchMask1603_g2 * RandomIDBranchPositionMask1529_g2 ) + ( ( ( appendResult1908_g2 + ( appendResult1889_g2 * cos( ( mulTime1896_g2 + RandomSeedVector_A1273_g2 + RandomSeedVector_G1201_g2 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1889_g2 ) ) * sin( ( mulTime1896_g2 + RandomSeedVector_D1221_g2 ) ) ) ) * BranchMask1603_g2 ) * 0.1 ) + ( ( ( ( appendResult1906_g2 + ( appendResult1887_g2 * cos( mulTime1891_g2 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1887_g2 ) ) * sin( ( mulTime1891_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_N1298_g2 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1529_g2 * BranchMask1603_g2 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB1559_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) + ( ( ( ( cos( temp_output_1350_0_g2 ) * sin( temp_output_1350_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( cos( temp_output_1353_0_g2 ) * sin( temp_output_1353_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( sin( temp_output_1356_0_g2 ) * cos( temp_output_1356_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC1552_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) );
				float3 PIWI_02Gentle1454_g2 = ( temp_output_1450_0_g2 * 0.4 );
				float4 WindDirection1518_g2 = _WindDirection;
				float4 normalizeResult1466_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 break1463_g2 = (normalizeResult1466_g2).xyz;
				float4 appendResult1482_g2 = (float4(break1463_g2.x , ( break1463_g2.y + _DownwardStrength ) , break1463_g2.z , 0.0));
				float4 WindMotion_BaseG1477_g2 = ( appendResult1482_g2 * saturate( input.positionOS.xyz.y ) );
				float2 appendResult1526_g2 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1528_g2 = appendResult1526_g2;
				float GlobalVar_WindSpeed1535_g2 = _StrongWindSpeed;
				float2 NoiseRotation_Output1227_g2 = ( -(WindDirection1518_g2).xz * _TimeParameters.x * GlobalVar_WindSpeed1535_g2 );
				float2 WPRG2D_S41492_g2 = ( BasicWorldPisitionXY_Out1528_g2 + ( NoiseRotation_Output1227_g2 * 4.0 ) );
				float simplePerlin2D1469_g2 = snoise( WPRG2D_S41492_g2*0.2 );
				simplePerlin2D1469_g2 = simplePerlin2D1469_g2*0.5 + 0.5;
				float WindMotionNoise1485_g2 = simplePerlin2D1469_g2;
				float saferPower1589_g2 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1591_g2 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1589_g2 , 0.1 ) ));
				float TrunkHeightMask1593_g2 = saturate( smoothstepResult1591_g2 );
				float4 MotionBendingGentleRandom1509_g2 = ( WindMotion_BaseG1477_g2 * _MotionBendingGentleRandom * WindMotionNoise1485_g2 * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float GlobalVar_WindMotion1530_g2 = _WindMotion;
				float3 worldToObjDir1498_g2 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG1477_g2 *  (0.0 + ( GlobalVar_WindMotion1530_g2 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise1485_g2 ).xyz, 0.0 ) ).xyz;
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float3 Motion_Bending_Gentle_Wind1512_g2 = ( worldToObjDir1498_g2 * ase_objectScale * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float dotResult1248_g2 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1274_g2 = dotResult1248_g2;
				float3 appendResult1362_g2 = (float3(( sin( ( RandomSeedVector_A1273_g2 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1274_g2 ) ) * _PivotRandomnessStrength )));
				float dotResult1252_g2 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1277_g2 = dotResult1252_g2;
				float4 normalizeResult1663_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 worldToObjDir1665_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult1362_g2 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1277_g2 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult1663_g2 * input.positionOS.xyz.y * TrunkHeightMask1593_g2 ).xyz, 0.0 ) ).xyz;
				float mulTime1244_g2 = _TimeParameters.x * 4.0;
				float dotResult1188_g2 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1202_g2 = dotResult1188_g2;
				float mulTime1245_g2 = _TimeParameters.x * 5.2;
				float dotResult1189_g2 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1203_g2 = dotResult1189_g2;
				float3 rotatedValue1361_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1201_g2 * 0.02 ) + mulTime1244_g2 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1202_g2 ) ) ) + sin( ( mulTime1245_g2 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1203_g2 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ).x );
				float3 worldToObjDir1392_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue1361_g2 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1518_g2 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2147_g2 = ( ( ( worldToObjDir1665_g2 * ase_objectScale * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir1392_g2 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1423_g2 = ( float4( PIWI_02Gentle1454_g2 , 0.0 ) + MotionBendingGentleRandom1509_g2 + float4( Motion_Bending_Gentle_Wind1512_g2 , 0.0 ) + float4( PIWI_01Gentle2147_g2 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output1453_g2 = ( GlobalVar_WindStrength1531_g2 * staticSwitch1423_g2 );
				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1453_g2.xyz;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = input.normalOS;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				output.positionCS = vertexInput.positionCS;
				output.positionWS = vertexInput.positionWS;
				return output;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 positionOS : INTERNALTESSPOS;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( Attributes input )
			{
				VertexControl output;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				output.positionOS = input.positionOS;
				output.normalOS = input.normalOS;
				output.tangentOS = input.tangentOS;
				
				return output;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> input)
			{
				TessellationFactors output;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(input[0].positionOS, input[1].positionOS, input[2].positionOS, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				output.edge[0] = tf.x; output.edge[1] = tf.y; output.edge[2] = tf.z; output.inside = tf.w;
				return output;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			PackedVaryings DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				Attributes output = (Attributes) 0;
				output.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				output.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				output.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = output.positionOS.xyz - patch[i].normalOS * (dot(output.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				output.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * output.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], output);
				return VertexFunction(output);
			}
			#else
			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}
			#endif

			half4 frag( PackedVaryings input
				#if defined( ASE_DEPTH_WRITE_ON )
				,out float outputDepth : ASE_SV_DEPTH
				#endif
				 ) : SV_Target
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( PositionWS );
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;

				

				surfaceDescription.Alpha = 1;
				#if defined( _ALPHATEST_ON )
					surfaceDescription.AlphaClipThreshold = _Cutoff;
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					input.positionCS.z = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					outputDepth = input.positionCS.z;
				#endif

				return unity_SelectionID;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "MotionVectors"
			Tags { "LightMode"="MotionVectors" }

			ColorMask RG

			HLSLPROGRAM

			#define ASE_GEOMETRY
			#define _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#pragma multi_compile_fragment _ DEBUG_DISPLAY
			#define _EMISSION
			#define _NORMALMAP 1
			#define ASE_VERSION 19908
			#define ASE_SRP_VERSION 170300


			#pragma vertex vert
			#pragma fragment frag

			#if defined( _SPECULAR_SETUP ) && defined( ASE_LIGHTING_SIMPLE )
				#if defined( _SPECULARHIGHLIGHTS_OFF )
					#undef _SPECULAR_COLOR
				#else
					#define _SPECULAR_COLOR
				#endif
			#endif

            #define SHADERPASS SHADERPASS_MOTION_VECTORS

            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
			#include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
		    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
		    #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#if defined(LOD_FADE_CROSSFADE)
				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
			#endif

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MotionVectorsCommon.hlsl"

			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF


			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct Attributes
			{
				float4 positionOS : POSITION;
				float3 positionOld : TEXCOORD4;
				#if _ADD_PRECOMPUTED_VELOCITY
					float3 alembicMotionVector : TEXCOORD5;
				#endif
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 positionCSNoJitter : TEXCOORD0;
				float4 previousPositionCSNoJitter : TEXCOORD1;
				float3 positionWS : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float _CenterofMass;
			float _WINDMASKSETTINGS1;
			float _TEXTUREMAPS1;
			float _PivotRandomness;
			float _PivotRandomnessStrength;
			float _TrunkHeightandThickness;
			float _MotionBendingGentleRandom;
			float _DownwardStrength;
			float _BranchWindSmall;
			float _BranchWindLarge;
			float _BranchSwayPower;
			float _BranchMaskRadious;
			float _BranchMaskScale;
			float _RootsHardness;
			float _RootsRadius;
			float _RootsPosition;
			float _MaskRoots;
			float _Hardness;
			float _Radius;
			float _MaskRootsAuto;
			float _TTFETREEGIZMOSHADER;
			float _ADVANCEDSETTINGS;
			float _AlphaClip;
			float _Cutoff;
			#ifdef ASE_TRANSMISSION
				float _TransmissionShadow;
			#endif
			#ifdef ASE_TRANSLUCENCY
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			#ifdef SCENEPICKINGPASS
				float4 _SelectionID;
			#endif

			#ifdef SCENESELECTIONPASS
				int _ObjectId;
				int _PassValue;
			#endif

			float _GlobalWindStrength;
			float4 _WindDirection;
			float _StrongWindSpeed;
			float _WindMotion;


			float3 ASESafeNormalize(float3 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			
			float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
			{
				original -= center;
				float C = cos( angle );
				float S = sin( angle );
				float t = 1 - C;
				float m00 = t * u.x * u.x + C;
				float m01 = t * u.x * u.y - S * u.z;
				float m02 = t * u.x * u.z + S * u.y;
				float m10 = t * u.x * u.y + S * u.z;
				float m11 = t * u.y * u.y + C;
				float m12 = t * u.y * u.z - S * u.x;
				float m20 = t * u.x * u.z - S * u.y;
				float m21 = t * u.y * u.z + S * u.x;
				float m22 = t * u.z * u.z + C;
				float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
				return mul( finalMatrix, original ) + center;
			}
			
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			
			float4 ASESafeNormalize(float4 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float GlobalVar_WindStrength1531_g2 = _GlobalWindStrength;
				float mulTime1270_g2 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1204_g2 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1220_g2 = dotResult1204_g2;
				float dotResult1206_g2 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1221_g2 = dotResult1206_g2;
				float mulTime1269_g2 = _TimeParameters.x * 4.3;
				float dotResult1205_g2 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1222_g2 = dotResult1205_g2;
				float3 normalizeResult1251_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1417_g2 = saturate( (distance( normalizeResult1251_g2 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1185_0_g2 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1199_g2 = dot( temp_output_1185_0_g2 , temp_output_1185_0_g2 );
				float saferPower1228_g2 = abs( saturate( dotResult1199_g2 ) );
				float temp_output_1249_0_g2 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1228_g2 , _Hardness ) );
				float3 normalizeResult1177_g2 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1416_g2 = saturate( (distance( normalizeResult1177_g2 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1414_g2 = (( _CenterofMass )?( ( temp_output_1249_0_g2 * CenterOfMass1416_g2 ) ):( temp_output_1249_0_g2 ));
				float saferPower2084_g2 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2055_g2 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2084_g2 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2110_g2 = float3( 0, -1, 0 );
				float3 appendResult2113_g2 = (float3(break2110_g2.x , ( break2110_g2.y * _RootsPosition ) , break2110_g2.z));
				float3 temp_output_2117_0_g2 = ( ( input.positionOS.xyz - appendResult2113_g2 ) / _RootsRadius );
				float dotResult2118_g2 = dot( temp_output_2117_0_g2 , temp_output_2117_0_g2 );
				float saferPower2121_g2 = abs( saturate( dotResult2118_g2 ) );
				float RootsMask_ProxySphere2122_g2 = pow( saferPower2121_g2 , _RootsHardness );
				float smoothstepResult1602_g2 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask1603_g2 = smoothstepResult1602_g2;
				float3 rotatedValue1373_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1270_g2 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1220_g2 ) + ( RandomSeedVector_D1221_g2 * 0.02 ) ) ) + sin( ( mulTime1269_g2 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1222_g2 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1417_g2 * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * BranchMask1603_g2 ).x );
				float3 appendResult1873_g2 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1860_g2 = input.positionOS.xyz;
				float3 appendResult1862_g2 = (float3(break1860_g2.x , ( break1860_g2.y * 0.15 ) , 0.0));
				float mulTime1866_g2 = _TimeParameters.x * 2.1;
				float dotResult1278_g2 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1296_g2 = dotResult1278_g2;
				float dotResult1283_g2 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1299_g2 = dotResult1283_g2;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1515_g2 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1523_g2 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1515_g2 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1515_g2 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1529_g2 = saturate( ( smoothstepResult1523_g2 * 0.5 ) );
				float3 appendResult1908_g2 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1883_g2 = input.positionOS.xyz;
				float3 appendResult1889_g2 = (float3(break1883_g2.x , 0.0 , ( break1883_g2.z * 0.15 )));
				float mulTime1896_g2 = _TimeParameters.x * 2.3;
				float dotResult1247_g2 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1273_g2 = dotResult1247_g2;
				float dotResult1187_g2 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1201_g2 = dotResult1187_g2;
				float3 appendResult1906_g2 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1882_g2 = input.positionOS.xyz;
				float3 appendResult1887_g2 = (float3(0.0 , ( break1882_g2.y * 0.2 ) , ( break1882_g2.z * 0.4 )));
				float mulTime1891_g2 = _TimeParameters.x * 2.0;
				float dotResult1279_g2 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1295_g2 = dotResult1279_g2;
				float dotResult1282_g2 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1298_g2 = dotResult1282_g2;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult1553_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1554_g2 = _TimeParameters.x * 0.25;
				float simplePerlin2D1557_g2 = snoise( ( normalizeResult1553_g2 + mulTime1554_g2 ).xy*0.43 );
				float WindMask_LargeB1559_g2 = ( simplePerlin2D1557_g2 * 1.5 );
				float mulTime1316_g2 = _TimeParameters.x * 3.2;
				float3 temp_output_1350_0_g2 = ( ( mulTime1316_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_K1296_g2 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1300_g2 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask1418_g2 = saturate( dotResult1300_g2 );
				float mulTime1313_g2 = _TimeParameters.x * 2.3;
				float dotResult1280_g2 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1297_g2 = dotResult1280_g2;
				float dotResult1281_g2 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1294_g2 = dotResult1281_g2;
				float3 temp_output_1353_0_g2 = ( ( mulTime1313_g2 + RandomSeedVector_L1297_g2 + RandomSeedVector_M1294_g2 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1310_g2 = _TimeParameters.x * 3.6;
				float temp_output_1356_0_g2 = ( ( mulTime1310_g2 + RandomSeedVector_N1298_g2 + RandomSeedVector_O1299_g2 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1540_g2 = ASESafeNormalize( ase_positionWS );
				float mulTime1541_g2 = _TimeParameters.x * 0.26;
				float simplePerlin2D1547_g2 = snoise( ( normalizeResult1540_g2 + mulTime1541_g2 ).xy*0.7 );
				float WindMask_LargeC1552_g2 = ( simplePerlin2D1547_g2 * 1.5 );
				float3 temp_output_1450_0_g2 = ( ( ( ( rotatedValue1373_g2 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult1873_g2 + ( appendResult1862_g2 * cos( mulTime1866_g2 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1862_g2 ) ) * sin( ( mulTime1866_g2 + RandomSeedVector_K1296_g2 + RandomSeedVector_O1299_g2 ) ) ) ) * 0.1 ) * BranchMask1603_g2 * RandomIDBranchPositionMask1529_g2 ) + ( ( ( appendResult1908_g2 + ( appendResult1889_g2 * cos( ( mulTime1896_g2 + RandomSeedVector_A1273_g2 + RandomSeedVector_G1201_g2 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1889_g2 ) ) * sin( ( mulTime1896_g2 + RandomSeedVector_D1221_g2 ) ) ) ) * BranchMask1603_g2 ) * 0.1 ) + ( ( ( ( appendResult1906_g2 + ( appendResult1887_g2 * cos( mulTime1891_g2 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1887_g2 ) ) * sin( ( mulTime1891_g2 + RandomSeedVector_J1295_g2 + RandomSeedVector_N1298_g2 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1529_g2 * BranchMask1603_g2 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB1559_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) + ( ( ( ( cos( temp_output_1350_0_g2 ) * sin( temp_output_1350_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( cos( temp_output_1353_0_g2 ) * sin( temp_output_1353_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) + ( ( sin( temp_output_1356_0_g2 ) * cos( temp_output_1356_0_g2 ) * ( BranchMask1603_g2 * CeneterOfMassThickness_Mask1418_g2 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC1552_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 ) );
				float3 PIWI_02Gentle1454_g2 = ( temp_output_1450_0_g2 * 0.4 );
				float4 WindDirection1518_g2 = _WindDirection;
				float4 normalizeResult1466_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 break1463_g2 = (normalizeResult1466_g2).xyz;
				float4 appendResult1482_g2 = (float4(break1463_g2.x , ( break1463_g2.y + _DownwardStrength ) , break1463_g2.z , 0.0));
				float4 WindMotion_BaseG1477_g2 = ( appendResult1482_g2 * saturate( input.positionOS.xyz.y ) );
				float2 appendResult1526_g2 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1528_g2 = appendResult1526_g2;
				float GlobalVar_WindSpeed1535_g2 = _StrongWindSpeed;
				float2 NoiseRotation_Output1227_g2 = ( -(WindDirection1518_g2).xz * _TimeParameters.x * GlobalVar_WindSpeed1535_g2 );
				float2 WPRG2D_S41492_g2 = ( BasicWorldPisitionXY_Out1528_g2 + ( NoiseRotation_Output1227_g2 * 4.0 ) );
				float simplePerlin2D1469_g2 = snoise( WPRG2D_S41492_g2*0.2 );
				simplePerlin2D1469_g2 = simplePerlin2D1469_g2*0.5 + 0.5;
				float WindMotionNoise1485_g2 = simplePerlin2D1469_g2;
				float saferPower1589_g2 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1591_g2 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1589_g2 , 0.1 ) ));
				float TrunkHeightMask1593_g2 = saturate( smoothstepResult1591_g2 );
				float4 MotionBendingGentleRandom1509_g2 = ( WindMotion_BaseG1477_g2 * _MotionBendingGentleRandom * WindMotionNoise1485_g2 * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float GlobalVar_WindMotion1530_g2 = _WindMotion;
				float3 worldToObjDir1498_g2 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG1477_g2 *  (0.0 + ( GlobalVar_WindMotion1530_g2 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise1485_g2 ).xyz, 0.0 ) ).xyz;
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float3 Motion_Bending_Gentle_Wind1512_g2 = ( worldToObjDir1498_g2 * ase_objectScale * BranchMask1603_g2 * MaskRoots2055_g2 * SphericalMaskProxySphere1414_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 );
				float dotResult1248_g2 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1274_g2 = dotResult1248_g2;
				float3 appendResult1362_g2 = (float3(( sin( ( RandomSeedVector_A1273_g2 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1274_g2 ) ) * _PivotRandomnessStrength )));
				float dotResult1252_g2 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1277_g2 = dotResult1252_g2;
				float4 normalizeResult1663_g2 = ASESafeNormalize( WindDirection1518_g2 );
				float3 worldToObjDir1665_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult1362_g2 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1277_g2 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult1663_g2 * input.positionOS.xyz.y * TrunkHeightMask1593_g2 ).xyz, 0.0 ) ).xyz;
				float mulTime1244_g2 = _TimeParameters.x * 4.0;
				float dotResult1188_g2 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1202_g2 = dotResult1188_g2;
				float mulTime1245_g2 = _TimeParameters.x * 5.2;
				float dotResult1189_g2 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1203_g2 = dotResult1189_g2;
				float3 rotatedValue1361_g2 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1201_g2 * 0.02 ) + mulTime1244_g2 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1202_g2 ) ) ) + sin( ( mulTime1245_g2 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1203_g2 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * TrunkHeightMask1593_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ).x );
				float3 worldToObjDir1392_g2 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue1361_g2 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1518_g2 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2147_g2 = ( ( ( worldToObjDir1665_g2 * ase_objectScale * SphericalMaskProxySphere1414_g2 * MaskRoots2055_g2 * RootsMask_ProxySphere2122_g2 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir1392_g2 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1423_g2 = ( float4( PIWI_02Gentle1454_g2 , 0.0 ) + MotionBendingGentleRandom1509_g2 + float4( Motion_Bending_Gentle_Wind1512_g2 , 0.0 ) + float4( PIWI_01Gentle2147_g2 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1423_g2 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output1453_g2 = ( GlobalVar_WindStrength1531_g2 * staticSwitch1423_g2 );
				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output1453_g2.xyz;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs( input.positionOS.xyz );

				#if defined(APPLICATION_SPACE_WARP_MOTION)
					output.positionCSNoJitter = mul(_NonJitteredViewProjMatrix, mul(UNITY_MATRIX_M, input.positionOS));
					output.positionCS = output.positionCSNoJitter;
				#else
					output.positionCS = vertexInput.positionCS;
					output.positionCSNoJitter = mul(_NonJitteredViewProjMatrix, mul(UNITY_MATRIX_M, input.positionOS));
				#endif

				float4 prevPos = ( unity_MotionVectorsParams.x == 1 ) ? float4( input.positionOld, 1 ) : input.positionOS;

				#if _ADD_PRECOMPUTED_VELOCITY
					prevPos = prevPos - float4(input.alembicMotionVector, 0);
				#endif

				output.previousPositionCSNoJitter = mul( _PrevViewProjMatrix, mul( UNITY_PREV_MATRIX_M, prevPos ) );

				output.positionWS = vertexInput.positionWS;

				// removed in ObjectMotionVectors.hlsl found in unity 6000.0.23 and higher
				//ApplyMotionVectorZBias( output.positionCS );
				return output;
			}

			PackedVaryings vert ( Attributes input )
			{
				return VertexFunction( input );
			}

			half4 frag(	PackedVaryings input
				#if defined( ASE_DEPTH_WRITE_ON )
				,out float outputDepth : ASE_SV_DEPTH
				#endif
				 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( input );

				float3 PositionWS = input.positionWS;
				float3 PositionRWS = GetCameraRelativePositionWS( PositionWS );
				float4 ScreenPosNorm = float4( GetNormalizedScreenSpaceUV( input.positionCS ), input.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, input.positionCS.z ) * input.positionCS.w;

				

				float Alpha = 1;
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _Cutoff;
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					input.positionCS.z = input.positionCS.z;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#if defined(ASE_CHANGES_WORLD_POS)
					float3 positionOS = mul( GetWorldToObjectMatrix(),  float4( PositionWS, 1.0 ) ).xyz;
					float3 previousPositionWS = mul( GetPrevObjectToWorldMatrix(),  float4( positionOS, 1.0 ) ).xyz;
					input.positionCSNoJitter = mul( _NonJitteredViewProjMatrix, float4( PositionWS, 1.0 ) );
					input.previousPositionCSNoJitter = mul( _PrevViewProjMatrix, float4( previousPositionWS, 1.0 ) );
				#endif

				#if defined(LOD_FADE_CROSSFADE)
					LODFadeCrossFade( input.positionCS );
				#endif

				#if defined( ASE_DEPTH_WRITE_ON )
					outputDepth = input.positionCS.z;
				#endif

				#if defined(APPLICATION_SPACE_WARP_MOTION)
					return float4( CalcAswNdcMotionVectorFromCsPositions( input.positionCSNoJitter, input.previousPositionCSNoJitter ), 1 );
				#else
					return float4( CalcNdcMotionVectorFromCsPositions( input.positionCSNoJitter, input.previousPositionCSNoJitter ), 0, 0 );
				#endif
			}
			ENDHLSL
		}

	
	}
	
	CustomEditor "UnityEditor.ShaderGraphLitGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}

/*ASEBEGIN
Version=19908
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;46;-848,-1296;Inherit;False;705;401;;5;60;54;53;47;58;Custom DRAWERS;0,0,0,1;0;0
Node;AmplifyShaderEditor.FresnelNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;5;-768,-592;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;-0.1;False;2;FLOAT;5;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;10;-704,-784;Inherit;False;Constant;_Color2;Color 2;0;0;Create;True;0;0;0;False;0;False;0.2156863,0.5607843,0.2,1;0.2196077,0.5529411,0.1999998,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;8;-432,-656;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;2;-770.438,-110.2618;Inherit;True;Property;_AlbedoMap;Albedo Map;2;2;[Header];[NoScaleOffset];Create;True;0;0;0;False;2;Space (10);TTFE_Drawer_SingleLineTexture;False;-1;None;4465c0aae8371694d8400e4dc45b23e3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;4;-748.3151,281.4371;Inherit;True;Property;_MaskMap;Mask Map;4;1;[NoScaleOffset];Create;True;0;0;0;False;1;TTFE_Drawer_SingleLineTexture;False;-1;None;97cbfaa1a982c434d9829a9ab41c5b0d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;15;-445.4461,284.9381;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;7;-304,-544;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;12;-429.237,-109.4421;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0.4;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;6;-146.635,-179.5742;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3;-768,96;Inherit;True;Property;_NormalMap;Normal Map;3;2;[NoScaleOffset];[Normal];Create;True;0;0;0;False;1;TTFE_Drawer_SingleLineTexture;False;-1;None;4199ccd0e0911f74f9589bfd1dc792a4;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;False;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;59;-319.9271,429.1129;Inherit;False;(TTFE) Tree Bark_Wind System;5;;2;58360699feb112c40b86ba9ba75062e6;0;0;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;58;-800,-1232;Inherit;False;Property;_TTFETREEGIZMOSHADER;(TTFE) TREE GIZMO SHADER;0;0;Create;True;0;0;0;False;1;TTFE_DrawerTitle;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;47;-704,-1136;Inherit;False;Property;_TEXTUREMAPS1;TEXTURE MAPS;1;0;Create;True;0;0;0;False;2;TTFE_DrawerFeatureBorder;Space (10);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;53;-496,-1168;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;54;-368,-1168;Inherit;False;CustomDRAWERS;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;14;-288,160;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;60;-752,-1024;Inherit;False;Property;_ADVANCEDSETTINGS;ADVANCED SETTINGS;28;0;Create;True;0;0;0;False;2;TTFE_DrawerFeatureBorder;Space (10);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;61;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;6;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;62;0,0;Float;False;True;-1;3;UnityEditor.ShaderGraphLitGUI;0;20;Toby Fredson/The Toby Foliage Engine/Utility/(TTFE) Global Controller;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;21;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;1;LightMode=UniversalForward;False;False;0;;0;0;Standard;51;Category;0;0;  Instanced Terrain Normals;1;0;Lighting Model;0;0;Workflow;1;0;Surface;0;0;  Keep Alpha;0;0;  Refraction Model;0;0;  Blend;0;0;Two Sided;1;0;Alpha Clipping;1;0;  Use Shadow Threshold;0;0;Fragment Normal Space;0;0;Forward Only;0;0;Transmission;0;0;  Transmission Shadow;0.5,False,;0;Translucency;0;0;  Translucency Strength;1,False,;0;  Normal Distortion;0.5,False,;0;  Scattering;2,False,;0;  Direct;0.9,False,;0;  Ambient;0.1,False,;0;  Shadow;0.5,False,;0;Cast Shadows;1;0;Receive Shadows;2;0;Specular Highlights;2;0;Environment Reflections;2;0;Receive SSAO;1;0;Motion Vectors;1;0;  Add Precomputed Velocity;0;0;  XR Motion Vectors;0;0;GPU Instancing;1;0;LOD CrossFade;1;0;Built-in Fog;1;0;_FinalColorxAlpha;0;0;Meta Pass;1;0;Override Baked GI;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Write Depth;0;0;  Early Z;0;0;Vertex Position;1;0;Debug Display;1;0;Clear Coat;0;0;0;12;False;True;True;True;True;True;True;True;True;True;True;False;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;63;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;64;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;65;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;66;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;67;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthNormals;0;6;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;False;True;1;LightMode=DepthNormals;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;68;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;GBuffer;0;7;GBuffer;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;1;LightMode=UniversalGBuffer;False;True;12;d3d11;gles;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;switch2;webgpu;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;69;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;SceneSelectionPass;0;8;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;70;0,0;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ScenePickingPass;0;9;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;71;0,100;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;MotionVectors;0;10;MotionVectors;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;False;False;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=MotionVectors;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;72;0,100;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;XRMotionVectors;0;11;XRMotionVectors;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;True;1;False;;255;False;;1;False;;7;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;False;True;1;LightMode=XRMotionVectors;False;False;0;;0;0;Standard;0;False;0
WireConnection;8;0;10;0
WireConnection;8;1;5;0
WireConnection;15;0;4;2
WireConnection;7;0;8;0
WireConnection;12;0;2;0
WireConnection;6;0;7;0
WireConnection;6;1;12;0
WireConnection;53;0;58;0
WireConnection;53;1;47;0
WireConnection;53;2;60;0
WireConnection;54;0;53;0
WireConnection;14;0;2;0
WireConnection;14;1;15;0
WireConnection;62;0;14;0
WireConnection;62;1;3;0
WireConnection;62;3;54;0
WireConnection;62;4;4;4
WireConnection;62;5;4;2
WireConnection;62;2;6;0
WireConnection;62;8;59;0
ASEEND*/
//CHKSM=8ABE5DDD5A987C1FC12D47B1F3A589D6005BEDB2