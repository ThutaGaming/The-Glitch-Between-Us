// Made with Amplify Shader Editor v1.9.9.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Toby Fredson/The Toby Foliage Engine/(TTFE) Tree Foliage (Mobile)"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[TTFE_DrawerTitle] _TTFETREEFOLIAGESHADERMOBILE( "(TTFE) TREE FOLIAGE SHADER (MOBILE)", Float ) = 0
		[TTFE_DrawerFeatureBorder][Space (10)] _FACERENDERING( "FACE RENDERING", Float ) = 0
		[Enum(Off,0,Front,1,Back,2)][Space (10)] _BackfaceCulling( "Backface Culling", Float ) = 0
		[TTFE_DrawerFeatureBorder][Space (10)] _TEXTUREMAPS( "TEXTURE MAPS", Float ) = 0
		[NoScaleOffset][Space (10)][TTFE_Drawer_SingleLineTexture] _AlbedoMap( "Albedo Map", 2D ) = "white" {}
		[NoScaleOffset][Normal][TTFE_Drawer_SingleLineTexture] _NormalMap( "Normal Map", 2D ) = "bump" {}
		[NoScaleOffset][TTFE_Drawer_SingleLineTexture] _SpecularMap( "Specular Map (Grayscale)", 2D ) = "white" {}
		[NoScaleOffset][TTFE_Drawer_SingleLineTexture] _MaskMapRGBA( "Mask Map *RGB(A)", 2D ) = "white" {}
		[NoScaleOffset][TTFE_Drawer_SingleLineTexture] _EmissionMap( "Emission Map", 2D ) = "black" {}
		[NoScaleOffset][TTFE_Drawer_SingleLineTexture] _NoiseMapGrayscale( "Noise Map (Grayscale)", 2D ) = "white" {}
		[TTFE_DrawerFeatureBorder][Space (10)] _TEXTURESETTINGS( "TEXTURE SETTINGS", Float ) = 0
		[Header((Albedo))] _AlbedoColor( "Albedo Color", Color ) = ( 1, 1, 1, 0 )
		[Header((Normal))] _NormalIntenisty( "Normal Intenisty", Float ) = 1
		[Toggle] _NormalBackFaceFixBranch( "Normal Back Face Fix (Branch)", Float ) = 0
		[Header((Smoothness))] _SmoothnessIntensity( "Smoothness Intensity", Range( 0, 1 ) ) = 1
		[Header((Ambient Occlusion))] _AmbientOcclusionIntensity( "Ambient Occlusion Intensity", Range( 0, 1 ) ) = 1
		[Header((Specular))] _SpecularPower( "Specular Power", Range( 0, 1 ) ) = 1
		[Toggle] _SpecularBackfaceOcclusion( "Specular Backface Occlusion", Float ) = 0
		_SpecularStrength( "Specular Strength", Float ) = 2
		_SpecularBias( "Specular Bias", Float ) = 1
		_SpecularScale( "Specular Scale", Float ) = -5
		_SpecularMapScale( "Specular Map Scale", Float ) = 1
		_SpecularMapOffset( "Specular Map Offset", Float ) = 0
		[Header((Emission))] _EmissionIntensity( "Emission Intensity", Float ) = 0
		_EmColor( "Emission Color", Color ) = ( 1, 1, 1, 0 )
		[TTFE_DrawerFeatureBorder][Space (10)] _SHADINGSETTINGS( "SHADING SETTINGS", Float ) = 0
		[Header((Self Shading))] _VertexLighting( "Vertex Lighting", Float ) = 3
		_VertexShadow( "Vertex Shadow", Float ) = 0
		_VertexAo( "Vertex Ao", Range( 0, 1 ) ) = 0
		[Toggle] _SelfShadingVertexColor( "Self Shading (Vertex Color)", Float ) = 0
		[Toggle] _LightDetectBackface( "Light Detect (Back face)", Float ) = 1
		[Toggle( _MOBILESHADINGWORLDUP_ON )] _MobileShadingWorldUp( "Mobile Shading (World Up)", Float ) = 0
		[TTFE_DrawerFeatureBorder][Space (10)] _SEASONSETTINGS( "SEASON SETTINGS", Float ) = 0
		[Header((Color Variation))] _ColorVariation( "Color Variation", Range( 0, 1 ) ) = 0.2
		_RandomColorScale( "Random Color Scale", Float ) = 1
		[Header((Texture Based Color Variation))] _TBCVMapIntenisty( "TBCV Map Intenisty", Float ) = 2
		_ZaWorldoScale( "TBCV Wold Scale", Float ) = 1
		_TBCVMapOffset( "TBCV Map Offset", Float ) = -0.35
		[Header((Season Controll))] _DryLeafColor( "Dry Leaf Color", Color ) = ( 0.5568628, 0.3730685, 0.1764706, 0 )
		_DryLeavesScale( "Dry Leaves - Scale", Float ) = 1
		_DryLeavesOffset( "Dry Leaves - Offset", Float ) = -0.5
		[Toggle] _SeasonVertexColorR( "Season Vertex Color (R)", Float ) = 1
		[Toggle] _BranchMaskR( "Branch Mask *(R)", Float ) = 1
		_AlphaClipping( "Alpha Clipping", Float ) = 0.4
		[TTFE_DrawerFeatureBorder][Space (10)] _TEXTUREMAPS1( "WIND SETTINGS", Float ) = 0
		[Header((Pivot))] _GrassSwayPowerGentleWind( "Grass Sway Power (Gentle Wind)", Float ) = 1
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
		[Toggle] _MaskRoots( "Mask Roots", Float ) = 1
		[Toggle] _MaskRootsAuto( "Mask Roots (Auto)", Float ) = 0
		[Toggle] _CenterofMass( "Center of Mass", Float ) = 0
		[Header((Spherical Mask))] _Radius( "Radius", Float ) = 2
		_Hardness( "Hardness", Float ) = 1
		[Header((Branch Mask))] _BranchMaskScale( "Branch Mask Scale", Float ) = 0.1
		_BranchMaskRadious( "Branch Mask Radius", Float ) = 0.5
		[Header((Roots Mask))] _RootsRadius( "Roots Radius", Float ) = 2
		_RootsHardness( "Roots Hardness", Float ) = 1
		_RootsPosition( "Roots Position", Float ) = 0
		[Header((Vertex Color Mask))] _VertexColorPower( "Vertex Color Power", Float ) = 1
		[Toggle] _SwitchVGreenToRGBA( "Switch VGreen To RGBA", Float ) = 0
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

		

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="TransparentCutout" "Queue"="AlphaTest" "UniversalMaterialType"="Lit" }

		Cull Off
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
			#define _SPECULAR_SETUP 1
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

			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_TANGENT
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_POSITION
			#define ASE_NEEDS_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_BITANGENT
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF
			#pragma shader_feature_local _MOBILESHADINGWORLDUP_ON


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
				float4 ase_color : COLOR;
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
				float4 ase_texcoord8 : TEXCOORD8;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _EmColor;
			float4 _DryLeafColor;
			float4 _AlbedoColor;
			float _BackfaceCulling;
			float _TBCVMapOffset;
			float _ColorVariation;
			float _BranchMaskR;
			float _VertexAo;
			float _VertexLighting;
			float _VertexShadow;
			float _TEXTUREMAPS;
			float _TEXTURESETTINGS;
			float _SEASONSETTINGS;
			float _SHADINGSETTINGS;
			float _NormalBackFaceFixBranch;
			float _NormalIntenisty;
			float _SpecularBackfaceOcclusion;
			float _SpecularPower;
			float _SpecularMapScale;
			float _SpecularMapOffset;
			float _SpecularBias;
			float _SpecularScale;
			float _SpecularStrength;
			float _SmoothnessIntensity;
			float _AmbientOcclusionIntensity;
			float _TTFETREEFOLIAGESHADERMOBILE;
			float _FACERENDERING;
			float _ADVANCEDSETTINGS;
			float _TBCVMapIntenisty;
			float _ZaWorldoScale;
			float _DryLeavesOffset;
			float _DryLeavesScale;
			float _CenterofMass;
			float _MaskRootsAuto;
			float _Radius;
			float _Hardness;
			float _MaskRoots;
			float _RootsPosition;
			float _RootsRadius;
			float _RootsHardness;
			float _BranchMaskScale;
			float _BranchMaskRadious;
			float _BranchSwayPower;
			float _BranchWindLarge;
			float _BranchWindSmall;
			float _SwitchVGreenToRGBA;
			float _VertexColorPower;
			float _GrassSwayPowerGentleWind;
			float _DownwardStrength;
			float _MotionBendingGentleRandom;
			float _TrunkHeightandThickness;
			float _PivotRandomnessStrength;
			float _PivotRandomness;
			float _TEXTUREMAPS1;
			float _WINDMASKSETTINGS1;
			float _LightDetectBackface;
			float _SelfShadingVertexColor;
			float _SeasonVertexColorR;
			float _RandomColorScale;
			float _EmissionIntensity;
			float _AlphaClipping;
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
			sampler2D _AlbedoMap;
			sampler2D _NoiseMapGrayscale;
			float _SeasonChangeGlobal;
			sampler2D _MaskMapRGBA;
			sampler2D _SpecularMap;
			sampler2D _EmissionMap;


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
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			float4 ASESafeNormalize(float4 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			
			
			float4 SampleGradient( Gradient gradient, float time )
			{
				float3 color = gradient.colors[0].rgb;
				UNITY_UNROLL
				for (int c = 1; c < 8; c++)
				{
				float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, gradient.colorsLength-1));
				color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
				}
				#ifndef UNITY_COLORSPACE_GAMMA
				color = SRGBToLinear(color);
				#endif
				float alpha = gradient.alphas[0].x;
				UNITY_UNROLL
				for (int a = 1; a < 8; a++)
				{
				float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, gradient.alphasLength-1));
				alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
				}
				return float4(color, alpha);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float GlobalVar_WindStrength2401_g2334 = _GlobalWindStrength;
				float mulTime1797_g2334 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1669_g2334 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1703_g2334 = dotResult1669_g2334;
				float dotResult1671_g2334 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1704_g2334 = dotResult1671_g2334;
				float mulTime1796_g2334 = _TimeParameters.x * 4.3;
				float dotResult1670_g2334 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1705_g2334 = dotResult1670_g2334;
				float3 normalizeResult1764_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1996_g2334 = saturate( (distance( normalizeResult1764_g2334 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1635_0_g2334 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1653_g2334 = dot( temp_output_1635_0_g2334 , temp_output_1635_0_g2334 );
				float saferPower1713_g2334 = abs( saturate( dotResult1653_g2334 ) );
				float temp_output_1761_0_g2334 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1713_g2334 , _Hardness ) );
				float3 normalizeResult1623_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1717_g2334 = saturate( (distance( normalizeResult1623_g2334 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1924_g2334 = (( _CenterofMass )?( ( temp_output_1761_0_g2334 * CenterOfMass1717_g2334 ) ):( temp_output_1761_0_g2334 ));
				float saferPower2686_g2334 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2691_g2334 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2686_g2334 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2782_g2334 = float3( 0, -1, 0 );
				float3 appendResult2785_g2334 = (float3(break2782_g2334.x , ( break2782_g2334.y * _RootsPosition ) , break2782_g2334.z));
				float3 temp_output_2789_0_g2334 = ( ( input.positionOS.xyz - appendResult2785_g2334 ) / _RootsRadius );
				float dotResult2790_g2334 = dot( temp_output_2789_0_g2334 , temp_output_2789_0_g2334 );
				float saferPower2793_g2334 = abs( saturate( dotResult2790_g2334 ) );
				float RootsMask_ProxySphere2794_g2334 = pow( saferPower2793_g2334 , _RootsHardness );
				float smoothstepResult1959_g2334 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask2026_g2334 = smoothstepResult1959_g2334;
				float3 rotatedValue2147_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1797_g2334 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1703_g2334 ) + ( RandomSeedVector_D1704_g2334 * 0.02 ) ) ) + sin( ( mulTime1796_g2334 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1705_g2334 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1996_g2334 * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * BranchMask2026_g2334 ).x );
				float3 appendResult2083_g2334 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1775_g2334 = input.positionOS.xyz;
				float3 appendResult1885_g2334 = (float3(break1775_g2334.x , ( break1775_g2334.y * 0.15 ) , 0.0));
				float mulTime1956_g2334 = _TimeParameters.x * 2.1;
				float dotResult1831_g2334 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1889_g2334 = dotResult1831_g2334;
				float dotResult1836_g2334 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1892_g2334 = dotResult1836_g2334;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1604_g2334 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1652_g2334 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1604_g2334 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1604_g2334 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1760_g2334 = saturate( ( smoothstepResult1652_g2334 * 0.5 ) );
				float3 appendResult2078_g2334 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1772_g2334 = input.positionOS.xyz;
				float3 appendResult1880_g2334 = (float3(break1772_g2334.x , 0.0 , ( break1772_g2334.z * 0.15 )));
				float mulTime1949_g2334 = _TimeParameters.x * 2.3;
				float dotResult1754_g2334 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1810_g2334 = dotResult1754_g2334;
				float dotResult1640_g2334 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1666_g2334 = dotResult1640_g2334;
				float3 appendResult2019_g2334 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1728_g2334 = input.positionOS.xyz;
				float3 appendResult1828_g2334 = (float3(0.0 , ( break1728_g2334.y * 0.2 ) , ( break1728_g2334.z * 0.4 )));
				float mulTime1882_g2334 = _TimeParameters.x * 2.0;
				float dotResult1832_g2334 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1888_g2334 = dotResult1832_g2334;
				float dotResult1835_g2334 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1891_g2334 = dotResult1835_g2334;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult2066_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime2067_g2334 = _TimeParameters.x * 0.25;
				float simplePerlin2D3191_g2334 = snoise( ( normalizeResult2066_g2334 + mulTime2067_g2334 ).xy*0.43 );
				float WindMask_LargeB2270_g2334 = ( simplePerlin2D3191_g2334 * 1.5 );
				float mulTime1940_g2334 = _TimeParameters.x * 3.2;
				float3 temp_output_2070_0_g2334 = ( ( mulTime1940_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_K1889_g2334 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1893_g2334 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask2025_g2334 = saturate( dotResult1893_g2334 );
				float mulTime1937_g2334 = _TimeParameters.x * 2.3;
				float dotResult1833_g2334 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1890_g2334 = dotResult1833_g2334;
				float dotResult1834_g2334 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1887_g2334 = dotResult1834_g2334;
				float3 temp_output_2073_0_g2334 = ( ( mulTime1937_g2334 + RandomSeedVector_L1890_g2334 + RandomSeedVector_M1887_g2334 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1934_g2334 = _TimeParameters.x * 3.6;
				float temp_output_2076_0_g2334 = ( ( mulTime1934_g2334 + RandomSeedVector_N1891_g2334 + RandomSeedVector_O1892_g2334 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1817_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime1818_g2334 = _TimeParameters.x * 0.26;
				float simplePerlin2D1923_g2334 = snoise( ( normalizeResult1817_g2334 + mulTime1818_g2334 ).xy*0.7 );
				float WindMask_LargeC2062_g2334 = ( simplePerlin2D1923_g2334 * 1.5 );
				float3 PIWI_02Gentle2481_g2334 = ( ( ( ( ( rotatedValue2147_g2334 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult2083_g2334 + ( appendResult1885_g2334 * cos( mulTime1956_g2334 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1885_g2334 ) ) * sin( ( mulTime1956_g2334 + RandomSeedVector_K1889_g2334 + RandomSeedVector_O1892_g2334 ) ) ) ) * 0.1 ) * BranchMask2026_g2334 * RandomIDBranchPositionMask1760_g2334 ) + ( ( ( appendResult2078_g2334 + ( appendResult1880_g2334 * cos( ( mulTime1949_g2334 + RandomSeedVector_A1810_g2334 + RandomSeedVector_G1666_g2334 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1880_g2334 ) ) * sin( ( mulTime1949_g2334 + RandomSeedVector_D1704_g2334 ) ) ) ) * BranchMask2026_g2334 ) * 0.1 ) + ( ( ( ( appendResult2019_g2334 + ( appendResult1828_g2334 * cos( mulTime1882_g2334 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1828_g2334 ) ) * sin( ( mulTime1882_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_N1891_g2334 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1760_g2334 * BranchMask2026_g2334 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB2270_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) + ( ( ( ( cos( temp_output_2070_0_g2334 ) * sin( temp_output_2070_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( cos( temp_output_2073_0_g2334 ) * sin( temp_output_2073_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( sin( temp_output_2076_0_g2334 ) * cos( temp_output_2076_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC2062_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) ) * 0.4 );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float2 appendResult1711_g2334 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1759_g2334 = appendResult1711_g2334;
				float4 WindDirection1609_g2334 = _WindDirection;
				float GlobalVar_WindSpeed1633_g2334 = _StrongWindSpeed;
				float2 NoiseRotation_Output1710_g2334 = ( -(WindDirection1609_g2334).xz * _TimeParameters.x * GlobalVar_WindSpeed1633_g2334 );
				float2 WPRG2D1990_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + NoiseRotation_Output1710_g2334 );
				float simpleNoise2607_g2334 = SimpleNoise( WPRG2D1990_g2334 );
				float3 break2587_g2334 = sin( ( ( _TimeParameters.y * 10.0 ) + ( ase_positionWS * 10.0 ) + input.tangentOS.xyz ) );
				float3 appendResult2598_g2334 = (float3(break2587_g2334.x , ( break2587_g2334.y * 0.3 ) , break2587_g2334.z));
				float3 smoothstepResult2606_g2334 = smoothstep( float3( 0,0,0 ) , float3( 2,2,2 ) , appendResult2598_g2334);
				float smoothstepResult1925_g2334 = smoothstep( 0.0 , _VertexColorPower , input.ase_color.g);
				float4 temp_cast_9 = (saturate( smoothstepResult1925_g2334 )).xxxx;
				float4 LeafVertexColor_Main2117_g2334 = (( _SwitchVGreenToRGBA )?( input.ase_color ):( temp_cast_9 ));
				float3 worldToObjDir2632_g2334 = mul( GetWorldToObjectMatrix(), float4( ( (simpleNoise2607_g2334*2.2 + -1.05) * float4( ( smoothstepResult2606_g2334 * 0.2 ) , 0.0 ) * input.positionOS.xyz.y * sin( _TimeParameters.x * 0.125 ) * LeafVertexColor_Main2117_g2334 ).rgb, 0.0 ) ).xyz;
				float3 NormaliseRotationAxis2409_g2334 = float3( 0, 1, 0 );
				float simplePerlin2D2430_g2334 = snoise( WPRG2D1990_g2334*0.6 );
				simplePerlin2D2430_g2334 = simplePerlin2D2430_g2334*0.5 + 0.5;
				float NoiseLarge2431_g2334 = simplePerlin2D2430_g2334;
				float mulTime2580_g2334 = _TimeParameters.x * 2.0;
				float3 rotatedValue2609_g2334 = RotateAroundAxis( float3( 0,0,0 ), ( cos( ( ( ase_positionWS * 0.2 ) + mulTime2580_g2334 ) ) * _GrassSwayPowerGentleWind * saturate( input.positionOS.xyz.y ) ), NormaliseRotationAxis2409_g2334, NoiseLarge2431_g2334 );
				float3 worldToObjDir2623_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindDirection1609_g2334 * float4( rotatedValue2609_g2334 , 0.0 ) ).xyz, 0.0 ) ).xyz;
				float simplePerlin2D2557_g2334 = snoise( WPRG2D1990_g2334*0.1 );
				float MaskRotation2559_g2334 = saturate( simplePerlin2D2557_g2334 );
				float3 temp_output_2605_0_g2334 = ( input.positionOS.xyz * 0.25 );
				float3 rotatedValue2610_g2334 = RotateAroundAxis( float3( 0,0,0 ), temp_output_2605_0_g2334, NormaliseRotationAxis2409_g2334, ( saturate( input.positionOS.xyz.y ) * NoiseLarge2431_g2334 ) );
				float3 worldToObjDir2628_g2334 = mul( GetWorldToObjectMatrix(), float4( ( rotatedValue2610_g2334 - temp_output_2605_0_g2334 ), 0.0 ) ).xyz;
				float4 GentleNoise2639_g2334 = ( float4( ( ase_objectScale * worldToObjDir2632_g2334 ) , 0.0 ) + ( ( float4( worldToObjDir2623_g2334 , 0.0 ) * MaskRotation2559_g2334 * LeafVertexColor_Main2117_g2334 * float4( ase_objectScale , 0.0 ) ) * 0.4 ) + ( MaskRotation2559_g2334 * float4( worldToObjDir2628_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 ) );
				float4 normalizeResult2396_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 break2391_g2334 = (normalizeResult2396_g2334).xyz;
				float4 appendResult2387_g2334 = (float4(break2391_g2334.x , ( break2391_g2334.y + _DownwardStrength ) , break2391_g2334.z , 0.0));
				float4 WindMotion_BaseG2644_g2334 = ( appendResult2387_g2334 * saturate( input.positionOS.xyz.y ) );
				float2 WPRG2D_S41918_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + ( NoiseRotation_Output1710_g2334 * 4.0 ) );
				float simplePerlin2D2394_g2334 = snoise( WPRG2D_S41918_g2334*0.2 );
				simplePerlin2D2394_g2334 = simplePerlin2D2394_g2334*0.5 + 0.5;
				float WindMotionNoise2407_g2334 = simplePerlin2D2394_g2334;
				float saferPower1873_g2334 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1999_g2334 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1873_g2334 , 0.1 ) ));
				float TrunkHeightMask2118_g2334 = saturate( smoothstepResult1999_g2334 );
				float4 MotionBendingGentleRandom2426_g2334 = ( WindMotion_BaseG2644_g2334 * _MotionBendingGentleRandom * WindMotionNoise2407_g2334 * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float GlobalVar_WindMotion1991_g2334 = _WindMotion;
				float3 worldToObjDir2379_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG2644_g2334 *  (0.0 + ( GlobalVar_WindMotion1991_g2334 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise2407_g2334 ).xyz, 0.0 ) ).xyz;
				float3 MotionBendingGentleWind2427_g2334 = ( worldToObjDir2379_g2334 * ase_objectScale * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float4 MotionBendingGentleWind22811_g2334 = ( float4( worldToObjDir2379_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 );
				float dotResult1755_g2334 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1811_g2334 = dotResult1755_g2334;
				float3 appendResult2093_g2334 = (float3(( sin( ( RandomSeedVector_A1810_g2334 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1811_g2334 ) ) * _PivotRandomnessStrength )));
				float dotResult1767_g2334 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1819_g2334 = dotResult1767_g2334;
				float4 normalizeResult2158_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 worldToObjDir2240_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult2093_g2334 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1819_g2334 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult2158_g2334 * input.positionOS.xyz.y * TrunkHeightMask2118_g2334 ).xyz, 0.0 ) ).xyz;
				float mulTime1748_g2334 = _TimeParameters.x * 4.0;
				float dotResult1641_g2334 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1667_g2334 = dotResult1641_g2334;
				float mulTime1749_g2334 = _TimeParameters.x * 5.2;
				float dotResult1642_g2334 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1668_g2334 = dotResult1642_g2334;
				float3 rotatedValue2089_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1666_g2334 * 0.02 ) + mulTime1748_g2334 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1667_g2334 ) ) ) + sin( ( mulTime1749_g2334 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1668_g2334 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 ).x );
				float3 worldToObjDir2238_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue2089_g2334 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1609_g2334 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2815_g2334 = ( ( ( worldToObjDir2240_g2334 * ase_objectScale * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir2238_g2334 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1496_g2334 = ( float4( PIWI_02Gentle2481_g2334 , 0.0 ) + GentleNoise2639_g2334 + MotionBendingGentleRandom2426_g2334 + float4( MotionBendingGentleWind2427_g2334 , 0.0 ) + MotionBendingGentleWind22811_g2334 + float4( PIWI_01Gentle2815_g2334 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output163_g2334 = ( GlobalVar_WindStrength2401_g2334 * staticSwitch1496_g2334 );
				
				float3 ase_viewVectorWS = ( ( unity_OrthoParams.w == 0 ) ? _WorldSpaceCameraPos - ase_positionWS : UNITY_MATRIX_V[ 2 ].xyz );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 ase_normalWS = TransformObjectToWorldNormal( input.normalOS );
				float3 normalizedWorldNormal = normalize( ase_normalWS );
				float dotResult494_g2338 = dot( ase_viewDirWS , normalizedWorldNormal );
				float2 uv_NormalMap789_g2338 = input.texcoord.xy;
				float3 unpack490_g2338 = UnpackNormalScale( -tex2Dlod( _NormalMap, float4( uv_NormalMap789_g2338, 0, 0.0) ), -1.0 );
				unpack490_g2338.z = lerp( 1, unpack490_g2338.z, saturate(-1.0) );
				float3 ifLocalVar497_g2338 = 0;
				if( dotResult494_g2338 > 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 == 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 < 0.0 )
				ifLocalVar497_g2338 = -input.normalOS;
				float4 transform500_g2338 = mul(GetObjectToWorldMatrix(),float4( ifLocalVar497_g2338 , 0.0 ));
				float dotResult504_g2338 = dot( float4( SafeNormalize( _MainLightPosition.xyz ) , 0.0 ) , transform500_g2338 );
				float3 ifLocalVar511_g2338 = 0;
				if( dotResult504_g2338 >= 0.0 )
				ifLocalVar511_g2338 = ifLocalVar497_g2338;
				else
				ifLocalVar511_g2338 = -ifLocalVar497_g2338;
				float3 break514_g2338 = ifLocalVar511_g2338;
				float3 temp_cast_35 = (dotResult504_g2338).xxx;
				float4 appendResult525_g2338 = (float4(break514_g2338.x , ( break514_g2338.y + saturate( ( 1.0 - ( ( distance( float3( 0,0,0 ) , temp_cast_35 ) - 0.2 ) / max( 0.2, 1E-05 ) ) ) ) ) , break514_g2338.z , 0.0));
				float4 LightDetectBackface595_g2338 = appendResult525_g2338;
				#ifdef _MOBILESHADINGWORLDUP_ON
				float4 staticSwitch622_g2338 = float4( float3( 0, 1, 0 ) , 0.0 );
				#else
				float4 staticSwitch622_g2338 = (( _LightDetectBackface )?( LightDetectBackface595_g2338 ):( float4( input.normalOS , 0.0 ) ));
				#endif
				float4 LightDetect_Output597_g2338 = staticSwitch622_g2338;
				
				output.ase_texcoord7.xy = input.texcoord.xy;
				output.ase_texcoord8 = input.positionOS;
				output.ase_color = input.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord7.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output163_g2334.rgb;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif
				input.normalOS = LightDetect_Output597_g2338.xyz;
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
				float4 ase_color : COLOR;

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
				output.ase_color = input.ase_color;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
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
						, uint ase_vface : SV_IsFrontFace ) : SV_Target
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

				float2 uv_AlbedoMap513_g2338 = input.ase_texcoord7.xy;
				float2 uv_AlbedoMap662_g2338 = input.ase_texcoord7.xy;
				float4 tex2DNode662_g2338 = tex2D( _AlbedoMap, uv_AlbedoMap662_g2338 );
				float2 uv_NoiseMapGrayscale669_g2338 = input.ase_texcoord7.xy;
				float4 tex2DNode669_g2338 = tex2D( _NoiseMapGrayscale, uv_NoiseMapGrayscale669_g2338 );
				float4 transform910_g2338 = mul(GetObjectToWorldMatrix(),float4( 1,1,1,1 ));
				float4 break906_g2338 = transform910_g2338;
				float RandomColorFix912_g2338 = floor( ( ( break906_g2338.x + break906_g2338.z ) * _RandomColorScale ) );
				float2 temp_cast_0 = (RandomColorFix912_g2338).xx;
				float dotResult4_g2340 = dot( temp_cast_0 , float2( 12.9898,78.233 ) );
				float lerpResult10_g2340 = lerp( 0.0 , 1.0 , frac( ( sin( dotResult4_g2340 ) * 43758.55 ) ));
				float temp_output_915_0_g2338 = saturate( lerpResult10_g2340 );
				float3 normalizeResult439_g2338 = ASESafeNormalize( input.ase_texcoord8.xyz );
				float DryLeafPositionMask443_g2338 = ( (distance( normalizeResult439_g2338 , float3( 0,0.8,0 ) )*1.0 + 0.0) * 1 );
				float temp_output_667_0_g2338 = ( 1.0 - input.ase_color.r );
				float GlobalVar_SeasonChange920_g2338 = _SeasonChangeGlobal;
				float4 lerpResult677_g2338 = lerp( ( _DryLeafColor * ( tex2DNode662_g2338.g * 2 ) ) , tex2DNode662_g2338 , saturate( (( (( _SeasonVertexColorR )?( ( ( temp_output_667_0_g2338 * 0.9 ) + ( temp_output_667_0_g2338 * DryLeafPositionMask443_g2338 * tex2DNode669_g2338.r ) + temp_output_915_0_g2338 ) ):( ( tex2DNode669_g2338.r * temp_output_915_0_g2338 * DryLeafPositionMask443_g2338 ) )) - GlobalVar_SeasonChange920_g2338 )*_DryLeavesScale + _DryLeavesOffset) ));
				float4 SeasonControl_Output676_g2338 = lerpResult677_g2338;
				Gradient gradient752_g2338 = NewGradient( 0, 2, 2, float4( 1, 0.276868, 0, 0 ), float4( 0, 1, 0.7818019, 1 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float4 TextureBasedColorVariation952_g2338 = (tex2D( _NoiseMapGrayscale, ( ase_objectPosition * _ZaWorldoScale ).xy )*_TBCVMapIntenisty + _TBCVMapOffset);
				float4 lerpResult515_g2338 = lerp( SeasonControl_Output676_g2338 , ( ( SeasonControl_Output676_g2338 * 0.5 ) + ( SampleGradient( gradient752_g2338, TextureBasedColorVariation952_g2338.r ) * SeasonControl_Output676_g2338 ) ) , _ColorVariation);
				float2 uv_MaskMapRGBA505_g2338 = input.ase_texcoord7.xy;
				float4 lerpResult521_g2338 = lerp( tex2D( _AlbedoMap, uv_AlbedoMap513_g2338 ) , lerpResult515_g2338 , (( _BranchMaskR )?( tex2D( _MaskMapRGBA, uv_MaskMapRGBA505_g2338 ).r ):( 1.0 )));
				float4 LeafColorVariationSeasons_Output959_g2338 = lerpResult521_g2338;
				float saferPower678_g2338 = abs( input.ase_color.r );
				float3 temp_output_465_0_g2338 = ( ( input.ase_texcoord8.xyz * float3( 2,1.3,2 ) ) / 25.0 );
				float dotResult471_g2338 = dot( temp_output_465_0_g2338 , temp_output_465_0_g2338 );
				float saferPower480_g2338 = abs( saturate( dotResult471_g2338 ) );
				float3 normalizeResult457_g2338 = ASESafeNormalize( input.ase_texcoord8.xyz );
				float SelfShading601_g2338 = saturate( ( saturate( pow( saferPower678_g2338 , _VertexAo ) ) * (( pow( saferPower480_g2338 , 1.5 ) + ( ( 1.0 - (distance( normalizeResult457_g2338 , float3( 0,0.8,0 ) )*0.5 + 0.0) ) * 0.6 ) )*0.92 + -0.16) ) );
				float4 GrassColorVariation_Output586_g2338 = (( _SelfShadingVertexColor )?( ( LeafColorVariationSeasons_Output959_g2338 * (SelfShading601_g2338*_VertexLighting + _VertexShadow) ) ):( LeafColorVariationSeasons_Output959_g2338 ));
				float CustomDRAWERS868_g2338 = ( _TEXTUREMAPS + _TEXTURESETTINGS + _SEASONSETTINGS + _SHADINGSETTINGS );
				float Branch_Mask1022_g2338 = (( _BranchMaskR )?( tex2D( _MaskMapRGBA, uv_MaskMapRGBA505_g2338 ).r ):( 1.0 ));
				float4 lerpResult1023_g2338 = lerp( GrassColorVariation_Output586_g2338 , ( ( CustomDRAWERS868_g2338 + _AlbedoColor ) * GrassColorVariation_Output586_g2338 ) , Branch_Mask1022_g2338);
				float4 Snow_Output834_g2338 = lerpResult1023_g2338;
				float4 AlbedoFinal613_g2338 = Snow_Output834_g2338;
				
				float2 uv_NormalMap531_g2338 = input.ase_texcoord7.xy;
				float3 unpack531_g2338 = UnpackNormalScale( tex2D( _NormalMap, uv_NormalMap531_g2338 ), _NormalIntenisty );
				unpack531_g2338.z = lerp( 1, unpack531_g2338.z, saturate(_NormalIntenisty) );
				float3 tex2DNode531_g2338 = unpack531_g2338;
				float3 switchResult1020_g2338 = (((ase_vface>0)?(tex2DNode531_g2338):(-tex2DNode531_g2338)));
				float3 Normal_Output557_g2338 = (( _NormalBackFaceFixBranch )?( switchResult1020_g2338 ):( tex2DNode531_g2338 ));
				
				float2 uv_SpecularMap893_g2338 = input.ase_texcoord7.xy;
				float3 tanToWorld0 = float3( TangentWS.x, BitangentWS.x, NormalWS.x );
				float3 tanToWorld1 = float3( TangentWS.y, BitangentWS.y, NormalWS.y );
				float3 tanToWorld2 = float3( TangentWS.z, BitangentWS.z, NormalWS.z );
				float3 tanNormal879_g2338 = TangentWS;
				float fresnelNdotV879_g2338 = dot( float3( dot( tanToWorld0, tanNormal879_g2338 ), dot( tanToWorld1, tanNormal879_g2338 ), dot( tanToWorld2, tanNormal879_g2338 ) ), SafeNormalize( _MainLightPosition.xyz ) );
				float fresnelNode879_g2338 = ( _SpecularBias + _SpecularScale * pow( max( 1.0 - fresnelNdotV879_g2338 , 0.0001 ), _SpecularStrength ) );
				float SpecularRecalculate876_g2338 = saturate( fresnelNode879_g2338 );
				float Specular_Output882_g2338 = (( _SpecularBackfaceOcclusion )?( ( ( 0.2 * _SpecularPower ) * saturate( (tex2D( _SpecularMap, uv_SpecularMap893_g2338 ).r*_SpecularMapScale + _SpecularMapOffset) ) * SpecularRecalculate876_g2338 ) ):( 0.04 ));
				float3 temp_cast_4 = (Specular_Output882_g2338).xxx;
				
				float2 uv_MaskMapRGBA535_g2338 = input.ase_texcoord7.xy;
				float4 tex2DNode535_g2338 = tex2D( _MaskMapRGBA, uv_MaskMapRGBA535_g2338 );
				float Smoothness_Output1013_g2338 = ( tex2DNode535_g2338.a * _SmoothnessIntensity );
				
				float AoMapBase538_g2338 = tex2DNode535_g2338.g;
				float saferPower580_g2338 = abs( AoMapBase538_g2338 );
				float AmbientOcclusion_Output582_g2338 = pow( saferPower580_g2338 , _AmbientOcclusionIntensity );
				
				float2 uv_EmissionMap977_g2338 = input.ase_texcoord7.xy;
				float4 Emission_Output978_g2338 = ( float4( tex2D( _EmissionMap, uv_EmissionMap977_g2338 ).rgb , 0.0 ) * _EmColor * _EmissionIntensity );
				
				float2 uv_AlbedoMap555_g2338 = input.ase_texcoord7.xy;
				float Opacity_Output559_g2338 = tex2D( _AlbedoMap, uv_AlbedoMap555_g2338 ).a;
				

				float3 BaseColor = AlbedoFinal613_g2338.rgb;
				float3 Normal = Normal_Output557_g2338;
				float3 Specular = temp_cast_4;
				float Metallic = 0;
				float Smoothness = Smoothness_Output1013_g2338;
				float Occlusion = AmbientOcclusion_Output582_g2338;
				float3 Emission = ( _TTFETREEFOLIAGESHADERMOBILE + _FACERENDERING + _ADVANCEDSETTINGS + Emission_Output978_g2338 ).rgb;
				float Alpha = Opacity_Output559_g2338;
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _AlphaClipping;
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
			#define _SPECULAR_SETUP 1
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
			#define ASE_NEEDS_VERT_TANGENT
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES0
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF
			#pragma shader_feature_local _MOBILESHADINGWORLDUP_ON


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
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _EmColor;
			float4 _DryLeafColor;
			float4 _AlbedoColor;
			float _BackfaceCulling;
			float _TBCVMapOffset;
			float _ColorVariation;
			float _BranchMaskR;
			float _VertexAo;
			float _VertexLighting;
			float _VertexShadow;
			float _TEXTUREMAPS;
			float _TEXTURESETTINGS;
			float _SEASONSETTINGS;
			float _SHADINGSETTINGS;
			float _NormalBackFaceFixBranch;
			float _NormalIntenisty;
			float _SpecularBackfaceOcclusion;
			float _SpecularPower;
			float _SpecularMapScale;
			float _SpecularMapOffset;
			float _SpecularBias;
			float _SpecularScale;
			float _SpecularStrength;
			float _SmoothnessIntensity;
			float _AmbientOcclusionIntensity;
			float _TTFETREEFOLIAGESHADERMOBILE;
			float _FACERENDERING;
			float _ADVANCEDSETTINGS;
			float _TBCVMapIntenisty;
			float _ZaWorldoScale;
			float _DryLeavesOffset;
			float _DryLeavesScale;
			float _CenterofMass;
			float _MaskRootsAuto;
			float _Radius;
			float _Hardness;
			float _MaskRoots;
			float _RootsPosition;
			float _RootsRadius;
			float _RootsHardness;
			float _BranchMaskScale;
			float _BranchMaskRadious;
			float _BranchSwayPower;
			float _BranchWindLarge;
			float _BranchWindSmall;
			float _SwitchVGreenToRGBA;
			float _VertexColorPower;
			float _GrassSwayPowerGentleWind;
			float _DownwardStrength;
			float _MotionBendingGentleRandom;
			float _TrunkHeightandThickness;
			float _PivotRandomnessStrength;
			float _PivotRandomness;
			float _TEXTUREMAPS1;
			float _WINDMASKSETTINGS1;
			float _LightDetectBackface;
			float _SelfShadingVertexColor;
			float _SeasonVertexColorR;
			float _RandomColorScale;
			float _EmissionIntensity;
			float _AlphaClipping;
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
			sampler2D _AlbedoMap;


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
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
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

				float GlobalVar_WindStrength2401_g2334 = _GlobalWindStrength;
				float mulTime1797_g2334 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1669_g2334 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1703_g2334 = dotResult1669_g2334;
				float dotResult1671_g2334 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1704_g2334 = dotResult1671_g2334;
				float mulTime1796_g2334 = _TimeParameters.x * 4.3;
				float dotResult1670_g2334 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1705_g2334 = dotResult1670_g2334;
				float3 normalizeResult1764_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1996_g2334 = saturate( (distance( normalizeResult1764_g2334 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1635_0_g2334 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1653_g2334 = dot( temp_output_1635_0_g2334 , temp_output_1635_0_g2334 );
				float saferPower1713_g2334 = abs( saturate( dotResult1653_g2334 ) );
				float temp_output_1761_0_g2334 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1713_g2334 , _Hardness ) );
				float3 normalizeResult1623_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1717_g2334 = saturate( (distance( normalizeResult1623_g2334 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1924_g2334 = (( _CenterofMass )?( ( temp_output_1761_0_g2334 * CenterOfMass1717_g2334 ) ):( temp_output_1761_0_g2334 ));
				float saferPower2686_g2334 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2691_g2334 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2686_g2334 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2782_g2334 = float3( 0, -1, 0 );
				float3 appendResult2785_g2334 = (float3(break2782_g2334.x , ( break2782_g2334.y * _RootsPosition ) , break2782_g2334.z));
				float3 temp_output_2789_0_g2334 = ( ( input.positionOS.xyz - appendResult2785_g2334 ) / _RootsRadius );
				float dotResult2790_g2334 = dot( temp_output_2789_0_g2334 , temp_output_2789_0_g2334 );
				float saferPower2793_g2334 = abs( saturate( dotResult2790_g2334 ) );
				float RootsMask_ProxySphere2794_g2334 = pow( saferPower2793_g2334 , _RootsHardness );
				float smoothstepResult1959_g2334 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask2026_g2334 = smoothstepResult1959_g2334;
				float3 rotatedValue2147_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1797_g2334 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1703_g2334 ) + ( RandomSeedVector_D1704_g2334 * 0.02 ) ) ) + sin( ( mulTime1796_g2334 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1705_g2334 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1996_g2334 * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * BranchMask2026_g2334 ).x );
				float3 appendResult2083_g2334 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1775_g2334 = input.positionOS.xyz;
				float3 appendResult1885_g2334 = (float3(break1775_g2334.x , ( break1775_g2334.y * 0.15 ) , 0.0));
				float mulTime1956_g2334 = _TimeParameters.x * 2.1;
				float dotResult1831_g2334 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1889_g2334 = dotResult1831_g2334;
				float dotResult1836_g2334 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1892_g2334 = dotResult1836_g2334;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1604_g2334 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1652_g2334 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1604_g2334 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1604_g2334 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1760_g2334 = saturate( ( smoothstepResult1652_g2334 * 0.5 ) );
				float3 appendResult2078_g2334 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1772_g2334 = input.positionOS.xyz;
				float3 appendResult1880_g2334 = (float3(break1772_g2334.x , 0.0 , ( break1772_g2334.z * 0.15 )));
				float mulTime1949_g2334 = _TimeParameters.x * 2.3;
				float dotResult1754_g2334 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1810_g2334 = dotResult1754_g2334;
				float dotResult1640_g2334 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1666_g2334 = dotResult1640_g2334;
				float3 appendResult2019_g2334 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1728_g2334 = input.positionOS.xyz;
				float3 appendResult1828_g2334 = (float3(0.0 , ( break1728_g2334.y * 0.2 ) , ( break1728_g2334.z * 0.4 )));
				float mulTime1882_g2334 = _TimeParameters.x * 2.0;
				float dotResult1832_g2334 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1888_g2334 = dotResult1832_g2334;
				float dotResult1835_g2334 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1891_g2334 = dotResult1835_g2334;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult2066_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime2067_g2334 = _TimeParameters.x * 0.25;
				float simplePerlin2D3191_g2334 = snoise( ( normalizeResult2066_g2334 + mulTime2067_g2334 ).xy*0.43 );
				float WindMask_LargeB2270_g2334 = ( simplePerlin2D3191_g2334 * 1.5 );
				float mulTime1940_g2334 = _TimeParameters.x * 3.2;
				float3 temp_output_2070_0_g2334 = ( ( mulTime1940_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_K1889_g2334 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1893_g2334 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask2025_g2334 = saturate( dotResult1893_g2334 );
				float mulTime1937_g2334 = _TimeParameters.x * 2.3;
				float dotResult1833_g2334 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1890_g2334 = dotResult1833_g2334;
				float dotResult1834_g2334 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1887_g2334 = dotResult1834_g2334;
				float3 temp_output_2073_0_g2334 = ( ( mulTime1937_g2334 + RandomSeedVector_L1890_g2334 + RandomSeedVector_M1887_g2334 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1934_g2334 = _TimeParameters.x * 3.6;
				float temp_output_2076_0_g2334 = ( ( mulTime1934_g2334 + RandomSeedVector_N1891_g2334 + RandomSeedVector_O1892_g2334 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1817_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime1818_g2334 = _TimeParameters.x * 0.26;
				float simplePerlin2D1923_g2334 = snoise( ( normalizeResult1817_g2334 + mulTime1818_g2334 ).xy*0.7 );
				float WindMask_LargeC2062_g2334 = ( simplePerlin2D1923_g2334 * 1.5 );
				float3 PIWI_02Gentle2481_g2334 = ( ( ( ( ( rotatedValue2147_g2334 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult2083_g2334 + ( appendResult1885_g2334 * cos( mulTime1956_g2334 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1885_g2334 ) ) * sin( ( mulTime1956_g2334 + RandomSeedVector_K1889_g2334 + RandomSeedVector_O1892_g2334 ) ) ) ) * 0.1 ) * BranchMask2026_g2334 * RandomIDBranchPositionMask1760_g2334 ) + ( ( ( appendResult2078_g2334 + ( appendResult1880_g2334 * cos( ( mulTime1949_g2334 + RandomSeedVector_A1810_g2334 + RandomSeedVector_G1666_g2334 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1880_g2334 ) ) * sin( ( mulTime1949_g2334 + RandomSeedVector_D1704_g2334 ) ) ) ) * BranchMask2026_g2334 ) * 0.1 ) + ( ( ( ( appendResult2019_g2334 + ( appendResult1828_g2334 * cos( mulTime1882_g2334 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1828_g2334 ) ) * sin( ( mulTime1882_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_N1891_g2334 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1760_g2334 * BranchMask2026_g2334 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB2270_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) + ( ( ( ( cos( temp_output_2070_0_g2334 ) * sin( temp_output_2070_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( cos( temp_output_2073_0_g2334 ) * sin( temp_output_2073_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( sin( temp_output_2076_0_g2334 ) * cos( temp_output_2076_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC2062_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) ) * 0.4 );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float2 appendResult1711_g2334 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1759_g2334 = appendResult1711_g2334;
				float4 WindDirection1609_g2334 = _WindDirection;
				float GlobalVar_WindSpeed1633_g2334 = _StrongWindSpeed;
				float2 NoiseRotation_Output1710_g2334 = ( -(WindDirection1609_g2334).xz * _TimeParameters.x * GlobalVar_WindSpeed1633_g2334 );
				float2 WPRG2D1990_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + NoiseRotation_Output1710_g2334 );
				float simpleNoise2607_g2334 = SimpleNoise( WPRG2D1990_g2334 );
				float3 break2587_g2334 = sin( ( ( _TimeParameters.y * 10.0 ) + ( ase_positionWS * 10.0 ) + input.tangentOS.xyz ) );
				float3 appendResult2598_g2334 = (float3(break2587_g2334.x , ( break2587_g2334.y * 0.3 ) , break2587_g2334.z));
				float3 smoothstepResult2606_g2334 = smoothstep( float3( 0,0,0 ) , float3( 2,2,2 ) , appendResult2598_g2334);
				float smoothstepResult1925_g2334 = smoothstep( 0.0 , _VertexColorPower , input.ase_color.g);
				float4 temp_cast_9 = (saturate( smoothstepResult1925_g2334 )).xxxx;
				float4 LeafVertexColor_Main2117_g2334 = (( _SwitchVGreenToRGBA )?( input.ase_color ):( temp_cast_9 ));
				float3 worldToObjDir2632_g2334 = mul( GetWorldToObjectMatrix(), float4( ( (simpleNoise2607_g2334*2.2 + -1.05) * float4( ( smoothstepResult2606_g2334 * 0.2 ) , 0.0 ) * input.positionOS.xyz.y * sin( _TimeParameters.x * 0.125 ) * LeafVertexColor_Main2117_g2334 ).rgb, 0.0 ) ).xyz;
				float3 NormaliseRotationAxis2409_g2334 = float3( 0, 1, 0 );
				float simplePerlin2D2430_g2334 = snoise( WPRG2D1990_g2334*0.6 );
				simplePerlin2D2430_g2334 = simplePerlin2D2430_g2334*0.5 + 0.5;
				float NoiseLarge2431_g2334 = simplePerlin2D2430_g2334;
				float mulTime2580_g2334 = _TimeParameters.x * 2.0;
				float3 rotatedValue2609_g2334 = RotateAroundAxis( float3( 0,0,0 ), ( cos( ( ( ase_positionWS * 0.2 ) + mulTime2580_g2334 ) ) * _GrassSwayPowerGentleWind * saturate( input.positionOS.xyz.y ) ), NormaliseRotationAxis2409_g2334, NoiseLarge2431_g2334 );
				float3 worldToObjDir2623_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindDirection1609_g2334 * float4( rotatedValue2609_g2334 , 0.0 ) ).xyz, 0.0 ) ).xyz;
				float simplePerlin2D2557_g2334 = snoise( WPRG2D1990_g2334*0.1 );
				float MaskRotation2559_g2334 = saturate( simplePerlin2D2557_g2334 );
				float3 temp_output_2605_0_g2334 = ( input.positionOS.xyz * 0.25 );
				float3 rotatedValue2610_g2334 = RotateAroundAxis( float3( 0,0,0 ), temp_output_2605_0_g2334, NormaliseRotationAxis2409_g2334, ( saturate( input.positionOS.xyz.y ) * NoiseLarge2431_g2334 ) );
				float3 worldToObjDir2628_g2334 = mul( GetWorldToObjectMatrix(), float4( ( rotatedValue2610_g2334 - temp_output_2605_0_g2334 ), 0.0 ) ).xyz;
				float4 GentleNoise2639_g2334 = ( float4( ( ase_objectScale * worldToObjDir2632_g2334 ) , 0.0 ) + ( ( float4( worldToObjDir2623_g2334 , 0.0 ) * MaskRotation2559_g2334 * LeafVertexColor_Main2117_g2334 * float4( ase_objectScale , 0.0 ) ) * 0.4 ) + ( MaskRotation2559_g2334 * float4( worldToObjDir2628_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 ) );
				float4 normalizeResult2396_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 break2391_g2334 = (normalizeResult2396_g2334).xyz;
				float4 appendResult2387_g2334 = (float4(break2391_g2334.x , ( break2391_g2334.y + _DownwardStrength ) , break2391_g2334.z , 0.0));
				float4 WindMotion_BaseG2644_g2334 = ( appendResult2387_g2334 * saturate( input.positionOS.xyz.y ) );
				float2 WPRG2D_S41918_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + ( NoiseRotation_Output1710_g2334 * 4.0 ) );
				float simplePerlin2D2394_g2334 = snoise( WPRG2D_S41918_g2334*0.2 );
				simplePerlin2D2394_g2334 = simplePerlin2D2394_g2334*0.5 + 0.5;
				float WindMotionNoise2407_g2334 = simplePerlin2D2394_g2334;
				float saferPower1873_g2334 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1999_g2334 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1873_g2334 , 0.1 ) ));
				float TrunkHeightMask2118_g2334 = saturate( smoothstepResult1999_g2334 );
				float4 MotionBendingGentleRandom2426_g2334 = ( WindMotion_BaseG2644_g2334 * _MotionBendingGentleRandom * WindMotionNoise2407_g2334 * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float GlobalVar_WindMotion1991_g2334 = _WindMotion;
				float3 worldToObjDir2379_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG2644_g2334 *  (0.0 + ( GlobalVar_WindMotion1991_g2334 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise2407_g2334 ).xyz, 0.0 ) ).xyz;
				float3 MotionBendingGentleWind2427_g2334 = ( worldToObjDir2379_g2334 * ase_objectScale * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float4 MotionBendingGentleWind22811_g2334 = ( float4( worldToObjDir2379_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 );
				float dotResult1755_g2334 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1811_g2334 = dotResult1755_g2334;
				float3 appendResult2093_g2334 = (float3(( sin( ( RandomSeedVector_A1810_g2334 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1811_g2334 ) ) * _PivotRandomnessStrength )));
				float dotResult1767_g2334 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1819_g2334 = dotResult1767_g2334;
				float4 normalizeResult2158_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 worldToObjDir2240_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult2093_g2334 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1819_g2334 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult2158_g2334 * input.positionOS.xyz.y * TrunkHeightMask2118_g2334 ).xyz, 0.0 ) ).xyz;
				float mulTime1748_g2334 = _TimeParameters.x * 4.0;
				float dotResult1641_g2334 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1667_g2334 = dotResult1641_g2334;
				float mulTime1749_g2334 = _TimeParameters.x * 5.2;
				float dotResult1642_g2334 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1668_g2334 = dotResult1642_g2334;
				float3 rotatedValue2089_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1666_g2334 * 0.02 ) + mulTime1748_g2334 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1667_g2334 ) ) ) + sin( ( mulTime1749_g2334 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1668_g2334 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 ).x );
				float3 worldToObjDir2238_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue2089_g2334 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1609_g2334 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2815_g2334 = ( ( ( worldToObjDir2240_g2334 * ase_objectScale * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir2238_g2334 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1496_g2334 = ( float4( PIWI_02Gentle2481_g2334 , 0.0 ) + GentleNoise2639_g2334 + MotionBendingGentleRandom2426_g2334 + float4( MotionBendingGentleWind2427_g2334 , 0.0 ) + MotionBendingGentleWind22811_g2334 + float4( PIWI_01Gentle2815_g2334 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output163_g2334 = ( GlobalVar_WindStrength2401_g2334 * staticSwitch1496_g2334 );
				
				float3 ase_viewVectorWS = ( ( unity_OrthoParams.w == 0 ) ? _WorldSpaceCameraPos - ase_positionWS : UNITY_MATRIX_V[ 2 ].xyz );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 ase_normalWS = TransformObjectToWorldNormal( input.normalOS );
				float3 normalizedWorldNormal = normalize( ase_normalWS );
				float dotResult494_g2338 = dot( ase_viewDirWS , normalizedWorldNormal );
				float2 uv_NormalMap789_g2338 = input.ase_texcoord.xy;
				float3 unpack490_g2338 = UnpackNormalScale( -tex2Dlod( _NormalMap, float4( uv_NormalMap789_g2338, 0, 0.0) ), -1.0 );
				unpack490_g2338.z = lerp( 1, unpack490_g2338.z, saturate(-1.0) );
				float3 ifLocalVar497_g2338 = 0;
				if( dotResult494_g2338 > 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 == 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 < 0.0 )
				ifLocalVar497_g2338 = -input.normalOS;
				float4 transform500_g2338 = mul(GetObjectToWorldMatrix(),float4( ifLocalVar497_g2338 , 0.0 ));
				float dotResult504_g2338 = dot( float4( SafeNormalize( _MainLightPosition.xyz ) , 0.0 ) , transform500_g2338 );
				float3 ifLocalVar511_g2338 = 0;
				if( dotResult504_g2338 >= 0.0 )
				ifLocalVar511_g2338 = ifLocalVar497_g2338;
				else
				ifLocalVar511_g2338 = -ifLocalVar497_g2338;
				float3 break514_g2338 = ifLocalVar511_g2338;
				float3 temp_cast_35 = (dotResult504_g2338).xxx;
				float4 appendResult525_g2338 = (float4(break514_g2338.x , ( break514_g2338.y + saturate( ( 1.0 - ( ( distance( float3( 0,0,0 ) , temp_cast_35 ) - 0.2 ) / max( 0.2, 1E-05 ) ) ) ) ) , break514_g2338.z , 0.0));
				float4 LightDetectBackface595_g2338 = appendResult525_g2338;
				#ifdef _MOBILESHADINGWORLDUP_ON
				float4 staticSwitch622_g2338 = float4( float3( 0, 1, 0 ) , 0.0 );
				#else
				float4 staticSwitch622_g2338 = (( _LightDetectBackface )?( LightDetectBackface595_g2338 ):( float4( input.normalOS , 0.0 ) ));
				#endif
				float4 LightDetect_Output597_g2338 = staticSwitch622_g2338;
				
				output.ase_texcoord1.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord1.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output163_g2334.rgb;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = LightDetect_Output597_g2338.xyz;
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
				float4 ase_color : COLOR;
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
				output.ase_color = input.ase_color;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
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

				float2 uv_AlbedoMap555_g2338 = input.ase_texcoord1.xy;
				float Opacity_Output559_g2338 = tex2D( _AlbedoMap, uv_AlbedoMap555_g2338 ).a;
				

				float Alpha = Opacity_Output559_g2338;
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _AlphaClipping;
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
			ColorMask R
			AlphaToMask Off

			HLSLPROGRAM

			#define ASE_GEOMETRY
			#define _ALPHATEST_ON
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#define ASE_FOG 1
			#pragma multi_compile_fragment _ DEBUG_DISPLAY
			#define _SPECULAR_SETUP 1
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
			#define ASE_NEEDS_VERT_TANGENT
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES0
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF
			#pragma shader_feature_local _MOBILESHADINGWORLDUP_ON


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
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _EmColor;
			float4 _DryLeafColor;
			float4 _AlbedoColor;
			float _BackfaceCulling;
			float _TBCVMapOffset;
			float _ColorVariation;
			float _BranchMaskR;
			float _VertexAo;
			float _VertexLighting;
			float _VertexShadow;
			float _TEXTUREMAPS;
			float _TEXTURESETTINGS;
			float _SEASONSETTINGS;
			float _SHADINGSETTINGS;
			float _NormalBackFaceFixBranch;
			float _NormalIntenisty;
			float _SpecularBackfaceOcclusion;
			float _SpecularPower;
			float _SpecularMapScale;
			float _SpecularMapOffset;
			float _SpecularBias;
			float _SpecularScale;
			float _SpecularStrength;
			float _SmoothnessIntensity;
			float _AmbientOcclusionIntensity;
			float _TTFETREEFOLIAGESHADERMOBILE;
			float _FACERENDERING;
			float _ADVANCEDSETTINGS;
			float _TBCVMapIntenisty;
			float _ZaWorldoScale;
			float _DryLeavesOffset;
			float _DryLeavesScale;
			float _CenterofMass;
			float _MaskRootsAuto;
			float _Radius;
			float _Hardness;
			float _MaskRoots;
			float _RootsPosition;
			float _RootsRadius;
			float _RootsHardness;
			float _BranchMaskScale;
			float _BranchMaskRadious;
			float _BranchSwayPower;
			float _BranchWindLarge;
			float _BranchWindSmall;
			float _SwitchVGreenToRGBA;
			float _VertexColorPower;
			float _GrassSwayPowerGentleWind;
			float _DownwardStrength;
			float _MotionBendingGentleRandom;
			float _TrunkHeightandThickness;
			float _PivotRandomnessStrength;
			float _PivotRandomness;
			float _TEXTUREMAPS1;
			float _WINDMASKSETTINGS1;
			float _LightDetectBackface;
			float _SelfShadingVertexColor;
			float _SeasonVertexColorR;
			float _RandomColorScale;
			float _EmissionIntensity;
			float _AlphaClipping;
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
			sampler2D _AlbedoMap;


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
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
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

				float GlobalVar_WindStrength2401_g2334 = _GlobalWindStrength;
				float mulTime1797_g2334 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1669_g2334 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1703_g2334 = dotResult1669_g2334;
				float dotResult1671_g2334 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1704_g2334 = dotResult1671_g2334;
				float mulTime1796_g2334 = _TimeParameters.x * 4.3;
				float dotResult1670_g2334 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1705_g2334 = dotResult1670_g2334;
				float3 normalizeResult1764_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1996_g2334 = saturate( (distance( normalizeResult1764_g2334 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1635_0_g2334 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1653_g2334 = dot( temp_output_1635_0_g2334 , temp_output_1635_0_g2334 );
				float saferPower1713_g2334 = abs( saturate( dotResult1653_g2334 ) );
				float temp_output_1761_0_g2334 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1713_g2334 , _Hardness ) );
				float3 normalizeResult1623_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1717_g2334 = saturate( (distance( normalizeResult1623_g2334 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1924_g2334 = (( _CenterofMass )?( ( temp_output_1761_0_g2334 * CenterOfMass1717_g2334 ) ):( temp_output_1761_0_g2334 ));
				float saferPower2686_g2334 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2691_g2334 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2686_g2334 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2782_g2334 = float3( 0, -1, 0 );
				float3 appendResult2785_g2334 = (float3(break2782_g2334.x , ( break2782_g2334.y * _RootsPosition ) , break2782_g2334.z));
				float3 temp_output_2789_0_g2334 = ( ( input.positionOS.xyz - appendResult2785_g2334 ) / _RootsRadius );
				float dotResult2790_g2334 = dot( temp_output_2789_0_g2334 , temp_output_2789_0_g2334 );
				float saferPower2793_g2334 = abs( saturate( dotResult2790_g2334 ) );
				float RootsMask_ProxySphere2794_g2334 = pow( saferPower2793_g2334 , _RootsHardness );
				float smoothstepResult1959_g2334 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask2026_g2334 = smoothstepResult1959_g2334;
				float3 rotatedValue2147_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1797_g2334 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1703_g2334 ) + ( RandomSeedVector_D1704_g2334 * 0.02 ) ) ) + sin( ( mulTime1796_g2334 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1705_g2334 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1996_g2334 * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * BranchMask2026_g2334 ).x );
				float3 appendResult2083_g2334 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1775_g2334 = input.positionOS.xyz;
				float3 appendResult1885_g2334 = (float3(break1775_g2334.x , ( break1775_g2334.y * 0.15 ) , 0.0));
				float mulTime1956_g2334 = _TimeParameters.x * 2.1;
				float dotResult1831_g2334 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1889_g2334 = dotResult1831_g2334;
				float dotResult1836_g2334 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1892_g2334 = dotResult1836_g2334;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1604_g2334 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1652_g2334 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1604_g2334 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1604_g2334 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1760_g2334 = saturate( ( smoothstepResult1652_g2334 * 0.5 ) );
				float3 appendResult2078_g2334 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1772_g2334 = input.positionOS.xyz;
				float3 appendResult1880_g2334 = (float3(break1772_g2334.x , 0.0 , ( break1772_g2334.z * 0.15 )));
				float mulTime1949_g2334 = _TimeParameters.x * 2.3;
				float dotResult1754_g2334 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1810_g2334 = dotResult1754_g2334;
				float dotResult1640_g2334 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1666_g2334 = dotResult1640_g2334;
				float3 appendResult2019_g2334 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1728_g2334 = input.positionOS.xyz;
				float3 appendResult1828_g2334 = (float3(0.0 , ( break1728_g2334.y * 0.2 ) , ( break1728_g2334.z * 0.4 )));
				float mulTime1882_g2334 = _TimeParameters.x * 2.0;
				float dotResult1832_g2334 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1888_g2334 = dotResult1832_g2334;
				float dotResult1835_g2334 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1891_g2334 = dotResult1835_g2334;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult2066_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime2067_g2334 = _TimeParameters.x * 0.25;
				float simplePerlin2D3191_g2334 = snoise( ( normalizeResult2066_g2334 + mulTime2067_g2334 ).xy*0.43 );
				float WindMask_LargeB2270_g2334 = ( simplePerlin2D3191_g2334 * 1.5 );
				float mulTime1940_g2334 = _TimeParameters.x * 3.2;
				float3 temp_output_2070_0_g2334 = ( ( mulTime1940_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_K1889_g2334 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1893_g2334 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask2025_g2334 = saturate( dotResult1893_g2334 );
				float mulTime1937_g2334 = _TimeParameters.x * 2.3;
				float dotResult1833_g2334 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1890_g2334 = dotResult1833_g2334;
				float dotResult1834_g2334 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1887_g2334 = dotResult1834_g2334;
				float3 temp_output_2073_0_g2334 = ( ( mulTime1937_g2334 + RandomSeedVector_L1890_g2334 + RandomSeedVector_M1887_g2334 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1934_g2334 = _TimeParameters.x * 3.6;
				float temp_output_2076_0_g2334 = ( ( mulTime1934_g2334 + RandomSeedVector_N1891_g2334 + RandomSeedVector_O1892_g2334 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1817_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime1818_g2334 = _TimeParameters.x * 0.26;
				float simplePerlin2D1923_g2334 = snoise( ( normalizeResult1817_g2334 + mulTime1818_g2334 ).xy*0.7 );
				float WindMask_LargeC2062_g2334 = ( simplePerlin2D1923_g2334 * 1.5 );
				float3 PIWI_02Gentle2481_g2334 = ( ( ( ( ( rotatedValue2147_g2334 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult2083_g2334 + ( appendResult1885_g2334 * cos( mulTime1956_g2334 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1885_g2334 ) ) * sin( ( mulTime1956_g2334 + RandomSeedVector_K1889_g2334 + RandomSeedVector_O1892_g2334 ) ) ) ) * 0.1 ) * BranchMask2026_g2334 * RandomIDBranchPositionMask1760_g2334 ) + ( ( ( appendResult2078_g2334 + ( appendResult1880_g2334 * cos( ( mulTime1949_g2334 + RandomSeedVector_A1810_g2334 + RandomSeedVector_G1666_g2334 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1880_g2334 ) ) * sin( ( mulTime1949_g2334 + RandomSeedVector_D1704_g2334 ) ) ) ) * BranchMask2026_g2334 ) * 0.1 ) + ( ( ( ( appendResult2019_g2334 + ( appendResult1828_g2334 * cos( mulTime1882_g2334 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1828_g2334 ) ) * sin( ( mulTime1882_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_N1891_g2334 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1760_g2334 * BranchMask2026_g2334 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB2270_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) + ( ( ( ( cos( temp_output_2070_0_g2334 ) * sin( temp_output_2070_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( cos( temp_output_2073_0_g2334 ) * sin( temp_output_2073_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( sin( temp_output_2076_0_g2334 ) * cos( temp_output_2076_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC2062_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) ) * 0.4 );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float2 appendResult1711_g2334 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1759_g2334 = appendResult1711_g2334;
				float4 WindDirection1609_g2334 = _WindDirection;
				float GlobalVar_WindSpeed1633_g2334 = _StrongWindSpeed;
				float2 NoiseRotation_Output1710_g2334 = ( -(WindDirection1609_g2334).xz * _TimeParameters.x * GlobalVar_WindSpeed1633_g2334 );
				float2 WPRG2D1990_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + NoiseRotation_Output1710_g2334 );
				float simpleNoise2607_g2334 = SimpleNoise( WPRG2D1990_g2334 );
				float3 break2587_g2334 = sin( ( ( _TimeParameters.y * 10.0 ) + ( ase_positionWS * 10.0 ) + input.tangentOS.xyz ) );
				float3 appendResult2598_g2334 = (float3(break2587_g2334.x , ( break2587_g2334.y * 0.3 ) , break2587_g2334.z));
				float3 smoothstepResult2606_g2334 = smoothstep( float3( 0,0,0 ) , float3( 2,2,2 ) , appendResult2598_g2334);
				float smoothstepResult1925_g2334 = smoothstep( 0.0 , _VertexColorPower , input.ase_color.g);
				float4 temp_cast_9 = (saturate( smoothstepResult1925_g2334 )).xxxx;
				float4 LeafVertexColor_Main2117_g2334 = (( _SwitchVGreenToRGBA )?( input.ase_color ):( temp_cast_9 ));
				float3 worldToObjDir2632_g2334 = mul( GetWorldToObjectMatrix(), float4( ( (simpleNoise2607_g2334*2.2 + -1.05) * float4( ( smoothstepResult2606_g2334 * 0.2 ) , 0.0 ) * input.positionOS.xyz.y * sin( _TimeParameters.x * 0.125 ) * LeafVertexColor_Main2117_g2334 ).rgb, 0.0 ) ).xyz;
				float3 NormaliseRotationAxis2409_g2334 = float3( 0, 1, 0 );
				float simplePerlin2D2430_g2334 = snoise( WPRG2D1990_g2334*0.6 );
				simplePerlin2D2430_g2334 = simplePerlin2D2430_g2334*0.5 + 0.5;
				float NoiseLarge2431_g2334 = simplePerlin2D2430_g2334;
				float mulTime2580_g2334 = _TimeParameters.x * 2.0;
				float3 rotatedValue2609_g2334 = RotateAroundAxis( float3( 0,0,0 ), ( cos( ( ( ase_positionWS * 0.2 ) + mulTime2580_g2334 ) ) * _GrassSwayPowerGentleWind * saturate( input.positionOS.xyz.y ) ), NormaliseRotationAxis2409_g2334, NoiseLarge2431_g2334 );
				float3 worldToObjDir2623_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindDirection1609_g2334 * float4( rotatedValue2609_g2334 , 0.0 ) ).xyz, 0.0 ) ).xyz;
				float simplePerlin2D2557_g2334 = snoise( WPRG2D1990_g2334*0.1 );
				float MaskRotation2559_g2334 = saturate( simplePerlin2D2557_g2334 );
				float3 temp_output_2605_0_g2334 = ( input.positionOS.xyz * 0.25 );
				float3 rotatedValue2610_g2334 = RotateAroundAxis( float3( 0,0,0 ), temp_output_2605_0_g2334, NormaliseRotationAxis2409_g2334, ( saturate( input.positionOS.xyz.y ) * NoiseLarge2431_g2334 ) );
				float3 worldToObjDir2628_g2334 = mul( GetWorldToObjectMatrix(), float4( ( rotatedValue2610_g2334 - temp_output_2605_0_g2334 ), 0.0 ) ).xyz;
				float4 GentleNoise2639_g2334 = ( float4( ( ase_objectScale * worldToObjDir2632_g2334 ) , 0.0 ) + ( ( float4( worldToObjDir2623_g2334 , 0.0 ) * MaskRotation2559_g2334 * LeafVertexColor_Main2117_g2334 * float4( ase_objectScale , 0.0 ) ) * 0.4 ) + ( MaskRotation2559_g2334 * float4( worldToObjDir2628_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 ) );
				float4 normalizeResult2396_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 break2391_g2334 = (normalizeResult2396_g2334).xyz;
				float4 appendResult2387_g2334 = (float4(break2391_g2334.x , ( break2391_g2334.y + _DownwardStrength ) , break2391_g2334.z , 0.0));
				float4 WindMotion_BaseG2644_g2334 = ( appendResult2387_g2334 * saturate( input.positionOS.xyz.y ) );
				float2 WPRG2D_S41918_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + ( NoiseRotation_Output1710_g2334 * 4.0 ) );
				float simplePerlin2D2394_g2334 = snoise( WPRG2D_S41918_g2334*0.2 );
				simplePerlin2D2394_g2334 = simplePerlin2D2394_g2334*0.5 + 0.5;
				float WindMotionNoise2407_g2334 = simplePerlin2D2394_g2334;
				float saferPower1873_g2334 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1999_g2334 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1873_g2334 , 0.1 ) ));
				float TrunkHeightMask2118_g2334 = saturate( smoothstepResult1999_g2334 );
				float4 MotionBendingGentleRandom2426_g2334 = ( WindMotion_BaseG2644_g2334 * _MotionBendingGentleRandom * WindMotionNoise2407_g2334 * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float GlobalVar_WindMotion1991_g2334 = _WindMotion;
				float3 worldToObjDir2379_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG2644_g2334 *  (0.0 + ( GlobalVar_WindMotion1991_g2334 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise2407_g2334 ).xyz, 0.0 ) ).xyz;
				float3 MotionBendingGentleWind2427_g2334 = ( worldToObjDir2379_g2334 * ase_objectScale * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float4 MotionBendingGentleWind22811_g2334 = ( float4( worldToObjDir2379_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 );
				float dotResult1755_g2334 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1811_g2334 = dotResult1755_g2334;
				float3 appendResult2093_g2334 = (float3(( sin( ( RandomSeedVector_A1810_g2334 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1811_g2334 ) ) * _PivotRandomnessStrength )));
				float dotResult1767_g2334 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1819_g2334 = dotResult1767_g2334;
				float4 normalizeResult2158_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 worldToObjDir2240_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult2093_g2334 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1819_g2334 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult2158_g2334 * input.positionOS.xyz.y * TrunkHeightMask2118_g2334 ).xyz, 0.0 ) ).xyz;
				float mulTime1748_g2334 = _TimeParameters.x * 4.0;
				float dotResult1641_g2334 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1667_g2334 = dotResult1641_g2334;
				float mulTime1749_g2334 = _TimeParameters.x * 5.2;
				float dotResult1642_g2334 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1668_g2334 = dotResult1642_g2334;
				float3 rotatedValue2089_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1666_g2334 * 0.02 ) + mulTime1748_g2334 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1667_g2334 ) ) ) + sin( ( mulTime1749_g2334 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1668_g2334 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 ).x );
				float3 worldToObjDir2238_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue2089_g2334 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1609_g2334 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2815_g2334 = ( ( ( worldToObjDir2240_g2334 * ase_objectScale * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir2238_g2334 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1496_g2334 = ( float4( PIWI_02Gentle2481_g2334 , 0.0 ) + GentleNoise2639_g2334 + MotionBendingGentleRandom2426_g2334 + float4( MotionBendingGentleWind2427_g2334 , 0.0 ) + MotionBendingGentleWind22811_g2334 + float4( PIWI_01Gentle2815_g2334 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output163_g2334 = ( GlobalVar_WindStrength2401_g2334 * staticSwitch1496_g2334 );
				
				float3 ase_viewVectorWS = ( ( unity_OrthoParams.w == 0 ) ? _WorldSpaceCameraPos - ase_positionWS : UNITY_MATRIX_V[ 2 ].xyz );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 ase_normalWS = TransformObjectToWorldNormal( input.normalOS );
				float3 normalizedWorldNormal = normalize( ase_normalWS );
				float dotResult494_g2338 = dot( ase_viewDirWS , normalizedWorldNormal );
				float2 uv_NormalMap789_g2338 = input.ase_texcoord.xy;
				float3 unpack490_g2338 = UnpackNormalScale( -tex2Dlod( _NormalMap, float4( uv_NormalMap789_g2338, 0, 0.0) ), -1.0 );
				unpack490_g2338.z = lerp( 1, unpack490_g2338.z, saturate(-1.0) );
				float3 ifLocalVar497_g2338 = 0;
				if( dotResult494_g2338 > 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 == 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 < 0.0 )
				ifLocalVar497_g2338 = -input.normalOS;
				float4 transform500_g2338 = mul(GetObjectToWorldMatrix(),float4( ifLocalVar497_g2338 , 0.0 ));
				float dotResult504_g2338 = dot( float4( SafeNormalize( _MainLightPosition.xyz ) , 0.0 ) , transform500_g2338 );
				float3 ifLocalVar511_g2338 = 0;
				if( dotResult504_g2338 >= 0.0 )
				ifLocalVar511_g2338 = ifLocalVar497_g2338;
				else
				ifLocalVar511_g2338 = -ifLocalVar497_g2338;
				float3 break514_g2338 = ifLocalVar511_g2338;
				float3 temp_cast_35 = (dotResult504_g2338).xxx;
				float4 appendResult525_g2338 = (float4(break514_g2338.x , ( break514_g2338.y + saturate( ( 1.0 - ( ( distance( float3( 0,0,0 ) , temp_cast_35 ) - 0.2 ) / max( 0.2, 1E-05 ) ) ) ) ) , break514_g2338.z , 0.0));
				float4 LightDetectBackface595_g2338 = appendResult525_g2338;
				#ifdef _MOBILESHADINGWORLDUP_ON
				float4 staticSwitch622_g2338 = float4( float3( 0, 1, 0 ) , 0.0 );
				#else
				float4 staticSwitch622_g2338 = (( _LightDetectBackface )?( LightDetectBackface595_g2338 ):( float4( input.normalOS , 0.0 ) ));
				#endif
				float4 LightDetect_Output597_g2338 = staticSwitch622_g2338;
				
				output.ase_texcoord1.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord1.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output163_g2334.rgb;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = LightDetect_Output597_g2338.xyz;
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
				float4 ase_color : COLOR;
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
				output.ase_color = input.ase_color;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
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

				float2 uv_AlbedoMap555_g2338 = input.ase_texcoord1.xy;
				float Opacity_Output559_g2338 = tex2D( _AlbedoMap, uv_AlbedoMap555_g2338 ).a;
				

				float Alpha = Opacity_Output559_g2338;
				float AlphaClipThreshold = _AlphaClipping;

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
			#define _SPECULAR_SETUP 1
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

			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_TANGENT
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_POSITION
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF
			#pragma shader_feature_local _MOBILESHADINGWORLDUP_ON


			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
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
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _EmColor;
			float4 _DryLeafColor;
			float4 _AlbedoColor;
			float _BackfaceCulling;
			float _TBCVMapOffset;
			float _ColorVariation;
			float _BranchMaskR;
			float _VertexAo;
			float _VertexLighting;
			float _VertexShadow;
			float _TEXTUREMAPS;
			float _TEXTURESETTINGS;
			float _SEASONSETTINGS;
			float _SHADINGSETTINGS;
			float _NormalBackFaceFixBranch;
			float _NormalIntenisty;
			float _SpecularBackfaceOcclusion;
			float _SpecularPower;
			float _SpecularMapScale;
			float _SpecularMapOffset;
			float _SpecularBias;
			float _SpecularScale;
			float _SpecularStrength;
			float _SmoothnessIntensity;
			float _AmbientOcclusionIntensity;
			float _TTFETREEFOLIAGESHADERMOBILE;
			float _FACERENDERING;
			float _ADVANCEDSETTINGS;
			float _TBCVMapIntenisty;
			float _ZaWorldoScale;
			float _DryLeavesOffset;
			float _DryLeavesScale;
			float _CenterofMass;
			float _MaskRootsAuto;
			float _Radius;
			float _Hardness;
			float _MaskRoots;
			float _RootsPosition;
			float _RootsRadius;
			float _RootsHardness;
			float _BranchMaskScale;
			float _BranchMaskRadious;
			float _BranchSwayPower;
			float _BranchWindLarge;
			float _BranchWindSmall;
			float _SwitchVGreenToRGBA;
			float _VertexColorPower;
			float _GrassSwayPowerGentleWind;
			float _DownwardStrength;
			float _MotionBendingGentleRandom;
			float _TrunkHeightandThickness;
			float _PivotRandomnessStrength;
			float _PivotRandomness;
			float _TEXTUREMAPS1;
			float _WINDMASKSETTINGS1;
			float _LightDetectBackface;
			float _SelfShadingVertexColor;
			float _SeasonVertexColorR;
			float _RandomColorScale;
			float _EmissionIntensity;
			float _AlphaClipping;
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
			sampler2D _AlbedoMap;
			sampler2D _NoiseMapGrayscale;
			float _SeasonChangeGlobal;
			sampler2D _MaskMapRGBA;
			sampler2D _EmissionMap;


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
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			float4 ASESafeNormalize(float4 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			
			
			float4 SampleGradient( Gradient gradient, float time )
			{
				float3 color = gradient.colors[0].rgb;
				UNITY_UNROLL
				for (int c = 1; c < 8; c++)
				{
				float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, gradient.colorsLength-1));
				color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
				}
				#ifndef UNITY_COLORSPACE_GAMMA
				color = SRGBToLinear(color);
				#endif
				float alpha = gradient.alphas[0].x;
				UNITY_UNROLL
				for (int a = 1; a < 8; a++)
				{
				float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, gradient.alphasLength-1));
				alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
				}
				return float4(color, alpha);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float GlobalVar_WindStrength2401_g2334 = _GlobalWindStrength;
				float mulTime1797_g2334 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1669_g2334 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1703_g2334 = dotResult1669_g2334;
				float dotResult1671_g2334 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1704_g2334 = dotResult1671_g2334;
				float mulTime1796_g2334 = _TimeParameters.x * 4.3;
				float dotResult1670_g2334 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1705_g2334 = dotResult1670_g2334;
				float3 normalizeResult1764_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1996_g2334 = saturate( (distance( normalizeResult1764_g2334 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1635_0_g2334 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1653_g2334 = dot( temp_output_1635_0_g2334 , temp_output_1635_0_g2334 );
				float saferPower1713_g2334 = abs( saturate( dotResult1653_g2334 ) );
				float temp_output_1761_0_g2334 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1713_g2334 , _Hardness ) );
				float3 normalizeResult1623_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1717_g2334 = saturate( (distance( normalizeResult1623_g2334 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1924_g2334 = (( _CenterofMass )?( ( temp_output_1761_0_g2334 * CenterOfMass1717_g2334 ) ):( temp_output_1761_0_g2334 ));
				float saferPower2686_g2334 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2691_g2334 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2686_g2334 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2782_g2334 = float3( 0, -1, 0 );
				float3 appendResult2785_g2334 = (float3(break2782_g2334.x , ( break2782_g2334.y * _RootsPosition ) , break2782_g2334.z));
				float3 temp_output_2789_0_g2334 = ( ( input.positionOS.xyz - appendResult2785_g2334 ) / _RootsRadius );
				float dotResult2790_g2334 = dot( temp_output_2789_0_g2334 , temp_output_2789_0_g2334 );
				float saferPower2793_g2334 = abs( saturate( dotResult2790_g2334 ) );
				float RootsMask_ProxySphere2794_g2334 = pow( saferPower2793_g2334 , _RootsHardness );
				float smoothstepResult1959_g2334 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask2026_g2334 = smoothstepResult1959_g2334;
				float3 rotatedValue2147_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1797_g2334 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1703_g2334 ) + ( RandomSeedVector_D1704_g2334 * 0.02 ) ) ) + sin( ( mulTime1796_g2334 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1705_g2334 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1996_g2334 * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * BranchMask2026_g2334 ).x );
				float3 appendResult2083_g2334 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1775_g2334 = input.positionOS.xyz;
				float3 appendResult1885_g2334 = (float3(break1775_g2334.x , ( break1775_g2334.y * 0.15 ) , 0.0));
				float mulTime1956_g2334 = _TimeParameters.x * 2.1;
				float dotResult1831_g2334 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1889_g2334 = dotResult1831_g2334;
				float dotResult1836_g2334 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1892_g2334 = dotResult1836_g2334;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1604_g2334 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1652_g2334 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1604_g2334 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1604_g2334 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1760_g2334 = saturate( ( smoothstepResult1652_g2334 * 0.5 ) );
				float3 appendResult2078_g2334 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1772_g2334 = input.positionOS.xyz;
				float3 appendResult1880_g2334 = (float3(break1772_g2334.x , 0.0 , ( break1772_g2334.z * 0.15 )));
				float mulTime1949_g2334 = _TimeParameters.x * 2.3;
				float dotResult1754_g2334 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1810_g2334 = dotResult1754_g2334;
				float dotResult1640_g2334 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1666_g2334 = dotResult1640_g2334;
				float3 appendResult2019_g2334 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1728_g2334 = input.positionOS.xyz;
				float3 appendResult1828_g2334 = (float3(0.0 , ( break1728_g2334.y * 0.2 ) , ( break1728_g2334.z * 0.4 )));
				float mulTime1882_g2334 = _TimeParameters.x * 2.0;
				float dotResult1832_g2334 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1888_g2334 = dotResult1832_g2334;
				float dotResult1835_g2334 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1891_g2334 = dotResult1835_g2334;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult2066_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime2067_g2334 = _TimeParameters.x * 0.25;
				float simplePerlin2D3191_g2334 = snoise( ( normalizeResult2066_g2334 + mulTime2067_g2334 ).xy*0.43 );
				float WindMask_LargeB2270_g2334 = ( simplePerlin2D3191_g2334 * 1.5 );
				float mulTime1940_g2334 = _TimeParameters.x * 3.2;
				float3 temp_output_2070_0_g2334 = ( ( mulTime1940_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_K1889_g2334 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1893_g2334 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask2025_g2334 = saturate( dotResult1893_g2334 );
				float mulTime1937_g2334 = _TimeParameters.x * 2.3;
				float dotResult1833_g2334 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1890_g2334 = dotResult1833_g2334;
				float dotResult1834_g2334 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1887_g2334 = dotResult1834_g2334;
				float3 temp_output_2073_0_g2334 = ( ( mulTime1937_g2334 + RandomSeedVector_L1890_g2334 + RandomSeedVector_M1887_g2334 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1934_g2334 = _TimeParameters.x * 3.6;
				float temp_output_2076_0_g2334 = ( ( mulTime1934_g2334 + RandomSeedVector_N1891_g2334 + RandomSeedVector_O1892_g2334 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1817_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime1818_g2334 = _TimeParameters.x * 0.26;
				float simplePerlin2D1923_g2334 = snoise( ( normalizeResult1817_g2334 + mulTime1818_g2334 ).xy*0.7 );
				float WindMask_LargeC2062_g2334 = ( simplePerlin2D1923_g2334 * 1.5 );
				float3 PIWI_02Gentle2481_g2334 = ( ( ( ( ( rotatedValue2147_g2334 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult2083_g2334 + ( appendResult1885_g2334 * cos( mulTime1956_g2334 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1885_g2334 ) ) * sin( ( mulTime1956_g2334 + RandomSeedVector_K1889_g2334 + RandomSeedVector_O1892_g2334 ) ) ) ) * 0.1 ) * BranchMask2026_g2334 * RandomIDBranchPositionMask1760_g2334 ) + ( ( ( appendResult2078_g2334 + ( appendResult1880_g2334 * cos( ( mulTime1949_g2334 + RandomSeedVector_A1810_g2334 + RandomSeedVector_G1666_g2334 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1880_g2334 ) ) * sin( ( mulTime1949_g2334 + RandomSeedVector_D1704_g2334 ) ) ) ) * BranchMask2026_g2334 ) * 0.1 ) + ( ( ( ( appendResult2019_g2334 + ( appendResult1828_g2334 * cos( mulTime1882_g2334 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1828_g2334 ) ) * sin( ( mulTime1882_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_N1891_g2334 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1760_g2334 * BranchMask2026_g2334 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB2270_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) + ( ( ( ( cos( temp_output_2070_0_g2334 ) * sin( temp_output_2070_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( cos( temp_output_2073_0_g2334 ) * sin( temp_output_2073_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( sin( temp_output_2076_0_g2334 ) * cos( temp_output_2076_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC2062_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) ) * 0.4 );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float2 appendResult1711_g2334 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1759_g2334 = appendResult1711_g2334;
				float4 WindDirection1609_g2334 = _WindDirection;
				float GlobalVar_WindSpeed1633_g2334 = _StrongWindSpeed;
				float2 NoiseRotation_Output1710_g2334 = ( -(WindDirection1609_g2334).xz * _TimeParameters.x * GlobalVar_WindSpeed1633_g2334 );
				float2 WPRG2D1990_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + NoiseRotation_Output1710_g2334 );
				float simpleNoise2607_g2334 = SimpleNoise( WPRG2D1990_g2334 );
				float3 break2587_g2334 = sin( ( ( _TimeParameters.y * 10.0 ) + ( ase_positionWS * 10.0 ) + input.tangentOS.xyz ) );
				float3 appendResult2598_g2334 = (float3(break2587_g2334.x , ( break2587_g2334.y * 0.3 ) , break2587_g2334.z));
				float3 smoothstepResult2606_g2334 = smoothstep( float3( 0,0,0 ) , float3( 2,2,2 ) , appendResult2598_g2334);
				float smoothstepResult1925_g2334 = smoothstep( 0.0 , _VertexColorPower , input.ase_color.g);
				float4 temp_cast_9 = (saturate( smoothstepResult1925_g2334 )).xxxx;
				float4 LeafVertexColor_Main2117_g2334 = (( _SwitchVGreenToRGBA )?( input.ase_color ):( temp_cast_9 ));
				float3 worldToObjDir2632_g2334 = mul( GetWorldToObjectMatrix(), float4( ( (simpleNoise2607_g2334*2.2 + -1.05) * float4( ( smoothstepResult2606_g2334 * 0.2 ) , 0.0 ) * input.positionOS.xyz.y * sin( _TimeParameters.x * 0.125 ) * LeafVertexColor_Main2117_g2334 ).rgb, 0.0 ) ).xyz;
				float3 NormaliseRotationAxis2409_g2334 = float3( 0, 1, 0 );
				float simplePerlin2D2430_g2334 = snoise( WPRG2D1990_g2334*0.6 );
				simplePerlin2D2430_g2334 = simplePerlin2D2430_g2334*0.5 + 0.5;
				float NoiseLarge2431_g2334 = simplePerlin2D2430_g2334;
				float mulTime2580_g2334 = _TimeParameters.x * 2.0;
				float3 rotatedValue2609_g2334 = RotateAroundAxis( float3( 0,0,0 ), ( cos( ( ( ase_positionWS * 0.2 ) + mulTime2580_g2334 ) ) * _GrassSwayPowerGentleWind * saturate( input.positionOS.xyz.y ) ), NormaliseRotationAxis2409_g2334, NoiseLarge2431_g2334 );
				float3 worldToObjDir2623_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindDirection1609_g2334 * float4( rotatedValue2609_g2334 , 0.0 ) ).xyz, 0.0 ) ).xyz;
				float simplePerlin2D2557_g2334 = snoise( WPRG2D1990_g2334*0.1 );
				float MaskRotation2559_g2334 = saturate( simplePerlin2D2557_g2334 );
				float3 temp_output_2605_0_g2334 = ( input.positionOS.xyz * 0.25 );
				float3 rotatedValue2610_g2334 = RotateAroundAxis( float3( 0,0,0 ), temp_output_2605_0_g2334, NormaliseRotationAxis2409_g2334, ( saturate( input.positionOS.xyz.y ) * NoiseLarge2431_g2334 ) );
				float3 worldToObjDir2628_g2334 = mul( GetWorldToObjectMatrix(), float4( ( rotatedValue2610_g2334 - temp_output_2605_0_g2334 ), 0.0 ) ).xyz;
				float4 GentleNoise2639_g2334 = ( float4( ( ase_objectScale * worldToObjDir2632_g2334 ) , 0.0 ) + ( ( float4( worldToObjDir2623_g2334 , 0.0 ) * MaskRotation2559_g2334 * LeafVertexColor_Main2117_g2334 * float4( ase_objectScale , 0.0 ) ) * 0.4 ) + ( MaskRotation2559_g2334 * float4( worldToObjDir2628_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 ) );
				float4 normalizeResult2396_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 break2391_g2334 = (normalizeResult2396_g2334).xyz;
				float4 appendResult2387_g2334 = (float4(break2391_g2334.x , ( break2391_g2334.y + _DownwardStrength ) , break2391_g2334.z , 0.0));
				float4 WindMotion_BaseG2644_g2334 = ( appendResult2387_g2334 * saturate( input.positionOS.xyz.y ) );
				float2 WPRG2D_S41918_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + ( NoiseRotation_Output1710_g2334 * 4.0 ) );
				float simplePerlin2D2394_g2334 = snoise( WPRG2D_S41918_g2334*0.2 );
				simplePerlin2D2394_g2334 = simplePerlin2D2394_g2334*0.5 + 0.5;
				float WindMotionNoise2407_g2334 = simplePerlin2D2394_g2334;
				float saferPower1873_g2334 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1999_g2334 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1873_g2334 , 0.1 ) ));
				float TrunkHeightMask2118_g2334 = saturate( smoothstepResult1999_g2334 );
				float4 MotionBendingGentleRandom2426_g2334 = ( WindMotion_BaseG2644_g2334 * _MotionBendingGentleRandom * WindMotionNoise2407_g2334 * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float GlobalVar_WindMotion1991_g2334 = _WindMotion;
				float3 worldToObjDir2379_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG2644_g2334 *  (0.0 + ( GlobalVar_WindMotion1991_g2334 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise2407_g2334 ).xyz, 0.0 ) ).xyz;
				float3 MotionBendingGentleWind2427_g2334 = ( worldToObjDir2379_g2334 * ase_objectScale * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float4 MotionBendingGentleWind22811_g2334 = ( float4( worldToObjDir2379_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 );
				float dotResult1755_g2334 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1811_g2334 = dotResult1755_g2334;
				float3 appendResult2093_g2334 = (float3(( sin( ( RandomSeedVector_A1810_g2334 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1811_g2334 ) ) * _PivotRandomnessStrength )));
				float dotResult1767_g2334 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1819_g2334 = dotResult1767_g2334;
				float4 normalizeResult2158_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 worldToObjDir2240_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult2093_g2334 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1819_g2334 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult2158_g2334 * input.positionOS.xyz.y * TrunkHeightMask2118_g2334 ).xyz, 0.0 ) ).xyz;
				float mulTime1748_g2334 = _TimeParameters.x * 4.0;
				float dotResult1641_g2334 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1667_g2334 = dotResult1641_g2334;
				float mulTime1749_g2334 = _TimeParameters.x * 5.2;
				float dotResult1642_g2334 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1668_g2334 = dotResult1642_g2334;
				float3 rotatedValue2089_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1666_g2334 * 0.02 ) + mulTime1748_g2334 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1667_g2334 ) ) ) + sin( ( mulTime1749_g2334 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1668_g2334 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 ).x );
				float3 worldToObjDir2238_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue2089_g2334 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1609_g2334 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2815_g2334 = ( ( ( worldToObjDir2240_g2334 * ase_objectScale * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir2238_g2334 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1496_g2334 = ( float4( PIWI_02Gentle2481_g2334 , 0.0 ) + GentleNoise2639_g2334 + MotionBendingGentleRandom2426_g2334 + float4( MotionBendingGentleWind2427_g2334 , 0.0 ) + MotionBendingGentleWind22811_g2334 + float4( PIWI_01Gentle2815_g2334 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output163_g2334 = ( GlobalVar_WindStrength2401_g2334 * staticSwitch1496_g2334 );
				
				float3 ase_viewVectorWS = ( ( unity_OrthoParams.w == 0 ) ? _WorldSpaceCameraPos - ase_positionWS : UNITY_MATRIX_V[ 2 ].xyz );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 ase_normalWS = TransformObjectToWorldNormal( input.normalOS );
				float3 normalizedWorldNormal = normalize( ase_normalWS );
				float dotResult494_g2338 = dot( ase_viewDirWS , normalizedWorldNormal );
				float2 uv_NormalMap789_g2338 = input.texcoord.xy;
				float3 unpack490_g2338 = UnpackNormalScale( -tex2Dlod( _NormalMap, float4( uv_NormalMap789_g2338, 0, 0.0) ), -1.0 );
				unpack490_g2338.z = lerp( 1, unpack490_g2338.z, saturate(-1.0) );
				float3 ifLocalVar497_g2338 = 0;
				if( dotResult494_g2338 > 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 == 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 < 0.0 )
				ifLocalVar497_g2338 = -input.normalOS;
				float4 transform500_g2338 = mul(GetObjectToWorldMatrix(),float4( ifLocalVar497_g2338 , 0.0 ));
				float dotResult504_g2338 = dot( float4( SafeNormalize( _MainLightPosition.xyz ) , 0.0 ) , transform500_g2338 );
				float3 ifLocalVar511_g2338 = 0;
				if( dotResult504_g2338 >= 0.0 )
				ifLocalVar511_g2338 = ifLocalVar497_g2338;
				else
				ifLocalVar511_g2338 = -ifLocalVar497_g2338;
				float3 break514_g2338 = ifLocalVar511_g2338;
				float3 temp_cast_35 = (dotResult504_g2338).xxx;
				float4 appendResult525_g2338 = (float4(break514_g2338.x , ( break514_g2338.y + saturate( ( 1.0 - ( ( distance( float3( 0,0,0 ) , temp_cast_35 ) - 0.2 ) / max( 0.2, 1E-05 ) ) ) ) ) , break514_g2338.z , 0.0));
				float4 LightDetectBackface595_g2338 = appendResult525_g2338;
				#ifdef _MOBILESHADINGWORLDUP_ON
				float4 staticSwitch622_g2338 = float4( float3( 0, 1, 0 ) , 0.0 );
				#else
				float4 staticSwitch622_g2338 = (( _LightDetectBackface )?( LightDetectBackface595_g2338 ):( float4( input.normalOS , 0.0 ) ));
				#endif
				float4 LightDetect_Output597_g2338 = staticSwitch622_g2338;
				
				output.ase_texcoord3.xy = input.texcoord.xy;
				output.ase_texcoord4 = input.positionOS;
				output.ase_color = input.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output163_g2334.rgb;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = LightDetect_Output597_g2338.xyz;
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
				float4 ase_color : COLOR;

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
				output.ase_color = input.ase_color;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
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

				float2 uv_AlbedoMap513_g2338 = input.ase_texcoord3.xy;
				float2 uv_AlbedoMap662_g2338 = input.ase_texcoord3.xy;
				float4 tex2DNode662_g2338 = tex2D( _AlbedoMap, uv_AlbedoMap662_g2338 );
				float2 uv_NoiseMapGrayscale669_g2338 = input.ase_texcoord3.xy;
				float4 tex2DNode669_g2338 = tex2D( _NoiseMapGrayscale, uv_NoiseMapGrayscale669_g2338 );
				float4 transform910_g2338 = mul(GetObjectToWorldMatrix(),float4( 1,1,1,1 ));
				float4 break906_g2338 = transform910_g2338;
				float RandomColorFix912_g2338 = floor( ( ( break906_g2338.x + break906_g2338.z ) * _RandomColorScale ) );
				float2 temp_cast_0 = (RandomColorFix912_g2338).xx;
				float dotResult4_g2340 = dot( temp_cast_0 , float2( 12.9898,78.233 ) );
				float lerpResult10_g2340 = lerp( 0.0 , 1.0 , frac( ( sin( dotResult4_g2340 ) * 43758.55 ) ));
				float temp_output_915_0_g2338 = saturate( lerpResult10_g2340 );
				float3 normalizeResult439_g2338 = ASESafeNormalize( input.ase_texcoord4.xyz );
				float DryLeafPositionMask443_g2338 = ( (distance( normalizeResult439_g2338 , float3( 0,0.8,0 ) )*1.0 + 0.0) * 1 );
				float temp_output_667_0_g2338 = ( 1.0 - input.ase_color.r );
				float GlobalVar_SeasonChange920_g2338 = _SeasonChangeGlobal;
				float4 lerpResult677_g2338 = lerp( ( _DryLeafColor * ( tex2DNode662_g2338.g * 2 ) ) , tex2DNode662_g2338 , saturate( (( (( _SeasonVertexColorR )?( ( ( temp_output_667_0_g2338 * 0.9 ) + ( temp_output_667_0_g2338 * DryLeafPositionMask443_g2338 * tex2DNode669_g2338.r ) + temp_output_915_0_g2338 ) ):( ( tex2DNode669_g2338.r * temp_output_915_0_g2338 * DryLeafPositionMask443_g2338 ) )) - GlobalVar_SeasonChange920_g2338 )*_DryLeavesScale + _DryLeavesOffset) ));
				float4 SeasonControl_Output676_g2338 = lerpResult677_g2338;
				Gradient gradient752_g2338 = NewGradient( 0, 2, 2, float4( 1, 0.276868, 0, 0 ), float4( 0, 1, 0.7818019, 1 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float4 TextureBasedColorVariation952_g2338 = (tex2D( _NoiseMapGrayscale, ( ase_objectPosition * _ZaWorldoScale ).xy )*_TBCVMapIntenisty + _TBCVMapOffset);
				float4 lerpResult515_g2338 = lerp( SeasonControl_Output676_g2338 , ( ( SeasonControl_Output676_g2338 * 0.5 ) + ( SampleGradient( gradient752_g2338, TextureBasedColorVariation952_g2338.r ) * SeasonControl_Output676_g2338 ) ) , _ColorVariation);
				float2 uv_MaskMapRGBA505_g2338 = input.ase_texcoord3.xy;
				float4 lerpResult521_g2338 = lerp( tex2D( _AlbedoMap, uv_AlbedoMap513_g2338 ) , lerpResult515_g2338 , (( _BranchMaskR )?( tex2D( _MaskMapRGBA, uv_MaskMapRGBA505_g2338 ).r ):( 1.0 )));
				float4 LeafColorVariationSeasons_Output959_g2338 = lerpResult521_g2338;
				float saferPower678_g2338 = abs( input.ase_color.r );
				float3 temp_output_465_0_g2338 = ( ( input.ase_texcoord4.xyz * float3( 2,1.3,2 ) ) / 25.0 );
				float dotResult471_g2338 = dot( temp_output_465_0_g2338 , temp_output_465_0_g2338 );
				float saferPower480_g2338 = abs( saturate( dotResult471_g2338 ) );
				float3 normalizeResult457_g2338 = ASESafeNormalize( input.ase_texcoord4.xyz );
				float SelfShading601_g2338 = saturate( ( saturate( pow( saferPower678_g2338 , _VertexAo ) ) * (( pow( saferPower480_g2338 , 1.5 ) + ( ( 1.0 - (distance( normalizeResult457_g2338 , float3( 0,0.8,0 ) )*0.5 + 0.0) ) * 0.6 ) )*0.92 + -0.16) ) );
				float4 GrassColorVariation_Output586_g2338 = (( _SelfShadingVertexColor )?( ( LeafColorVariationSeasons_Output959_g2338 * (SelfShading601_g2338*_VertexLighting + _VertexShadow) ) ):( LeafColorVariationSeasons_Output959_g2338 ));
				float CustomDRAWERS868_g2338 = ( _TEXTUREMAPS + _TEXTURESETTINGS + _SEASONSETTINGS + _SHADINGSETTINGS );
				float Branch_Mask1022_g2338 = (( _BranchMaskR )?( tex2D( _MaskMapRGBA, uv_MaskMapRGBA505_g2338 ).r ):( 1.0 ));
				float4 lerpResult1023_g2338 = lerp( GrassColorVariation_Output586_g2338 , ( ( CustomDRAWERS868_g2338 + _AlbedoColor ) * GrassColorVariation_Output586_g2338 ) , Branch_Mask1022_g2338);
				float4 Snow_Output834_g2338 = lerpResult1023_g2338;
				float4 AlbedoFinal613_g2338 = Snow_Output834_g2338;
				
				float2 uv_EmissionMap977_g2338 = input.ase_texcoord3.xy;
				float4 Emission_Output978_g2338 = ( float4( tex2D( _EmissionMap, uv_EmissionMap977_g2338 ).rgb , 0.0 ) * _EmColor * _EmissionIntensity );
				
				float2 uv_AlbedoMap555_g2338 = input.ase_texcoord3.xy;
				float Opacity_Output559_g2338 = tex2D( _AlbedoMap, uv_AlbedoMap555_g2338 ).a;
				

				float3 BaseColor = AlbedoFinal613_g2338.rgb;
				float3 Emission = ( _TTFETREEFOLIAGESHADERMOBILE + _FACERENDERING + _ADVANCEDSETTINGS + Emission_Output978_g2338 ).rgb;
				float Alpha = Opacity_Output559_g2338;
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _AlphaClipping;
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
			#define _SPECULAR_SETUP 1
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

			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_TANGENT
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_POSITION
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF
			#pragma shader_feature_local _MOBILESHADINGWORLDUP_ON


			struct Attributes
			{
				float4 positionOS : POSITION;
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _EmColor;
			float4 _DryLeafColor;
			float4 _AlbedoColor;
			float _BackfaceCulling;
			float _TBCVMapOffset;
			float _ColorVariation;
			float _BranchMaskR;
			float _VertexAo;
			float _VertexLighting;
			float _VertexShadow;
			float _TEXTUREMAPS;
			float _TEXTURESETTINGS;
			float _SEASONSETTINGS;
			float _SHADINGSETTINGS;
			float _NormalBackFaceFixBranch;
			float _NormalIntenisty;
			float _SpecularBackfaceOcclusion;
			float _SpecularPower;
			float _SpecularMapScale;
			float _SpecularMapOffset;
			float _SpecularBias;
			float _SpecularScale;
			float _SpecularStrength;
			float _SmoothnessIntensity;
			float _AmbientOcclusionIntensity;
			float _TTFETREEFOLIAGESHADERMOBILE;
			float _FACERENDERING;
			float _ADVANCEDSETTINGS;
			float _TBCVMapIntenisty;
			float _ZaWorldoScale;
			float _DryLeavesOffset;
			float _DryLeavesScale;
			float _CenterofMass;
			float _MaskRootsAuto;
			float _Radius;
			float _Hardness;
			float _MaskRoots;
			float _RootsPosition;
			float _RootsRadius;
			float _RootsHardness;
			float _BranchMaskScale;
			float _BranchMaskRadious;
			float _BranchSwayPower;
			float _BranchWindLarge;
			float _BranchWindSmall;
			float _SwitchVGreenToRGBA;
			float _VertexColorPower;
			float _GrassSwayPowerGentleWind;
			float _DownwardStrength;
			float _MotionBendingGentleRandom;
			float _TrunkHeightandThickness;
			float _PivotRandomnessStrength;
			float _PivotRandomness;
			float _TEXTUREMAPS1;
			float _WINDMASKSETTINGS1;
			float _LightDetectBackface;
			float _SelfShadingVertexColor;
			float _SeasonVertexColorR;
			float _RandomColorScale;
			float _EmissionIntensity;
			float _AlphaClipping;
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
			sampler2D _AlbedoMap;
			sampler2D _NoiseMapGrayscale;
			float _SeasonChangeGlobal;
			sampler2D _MaskMapRGBA;


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
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			float4 ASESafeNormalize(float4 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			
			
			float4 SampleGradient( Gradient gradient, float time )
			{
				float3 color = gradient.colors[0].rgb;
				UNITY_UNROLL
				for (int c = 1; c < 8; c++)
				{
				float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, gradient.colorsLength-1));
				color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
				}
				#ifndef UNITY_COLORSPACE_GAMMA
				color = SRGBToLinear(color);
				#endif
				float alpha = gradient.alphas[0].x;
				UNITY_UNROLL
				for (int a = 1; a < 8; a++)
				{
				float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, gradient.alphasLength-1));
				alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
				}
				return float4(color, alpha);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID( input );
				UNITY_TRANSFER_INSTANCE_ID( input, output );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );

				float GlobalVar_WindStrength2401_g2334 = _GlobalWindStrength;
				float mulTime1797_g2334 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1669_g2334 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1703_g2334 = dotResult1669_g2334;
				float dotResult1671_g2334 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1704_g2334 = dotResult1671_g2334;
				float mulTime1796_g2334 = _TimeParameters.x * 4.3;
				float dotResult1670_g2334 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1705_g2334 = dotResult1670_g2334;
				float3 normalizeResult1764_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1996_g2334 = saturate( (distance( normalizeResult1764_g2334 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1635_0_g2334 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1653_g2334 = dot( temp_output_1635_0_g2334 , temp_output_1635_0_g2334 );
				float saferPower1713_g2334 = abs( saturate( dotResult1653_g2334 ) );
				float temp_output_1761_0_g2334 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1713_g2334 , _Hardness ) );
				float3 normalizeResult1623_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1717_g2334 = saturate( (distance( normalizeResult1623_g2334 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1924_g2334 = (( _CenterofMass )?( ( temp_output_1761_0_g2334 * CenterOfMass1717_g2334 ) ):( temp_output_1761_0_g2334 ));
				float saferPower2686_g2334 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2691_g2334 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2686_g2334 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2782_g2334 = float3( 0, -1, 0 );
				float3 appendResult2785_g2334 = (float3(break2782_g2334.x , ( break2782_g2334.y * _RootsPosition ) , break2782_g2334.z));
				float3 temp_output_2789_0_g2334 = ( ( input.positionOS.xyz - appendResult2785_g2334 ) / _RootsRadius );
				float dotResult2790_g2334 = dot( temp_output_2789_0_g2334 , temp_output_2789_0_g2334 );
				float saferPower2793_g2334 = abs( saturate( dotResult2790_g2334 ) );
				float RootsMask_ProxySphere2794_g2334 = pow( saferPower2793_g2334 , _RootsHardness );
				float smoothstepResult1959_g2334 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask2026_g2334 = smoothstepResult1959_g2334;
				float3 rotatedValue2147_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1797_g2334 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1703_g2334 ) + ( RandomSeedVector_D1704_g2334 * 0.02 ) ) ) + sin( ( mulTime1796_g2334 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1705_g2334 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1996_g2334 * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * BranchMask2026_g2334 ).x );
				float3 appendResult2083_g2334 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1775_g2334 = input.positionOS.xyz;
				float3 appendResult1885_g2334 = (float3(break1775_g2334.x , ( break1775_g2334.y * 0.15 ) , 0.0));
				float mulTime1956_g2334 = _TimeParameters.x * 2.1;
				float dotResult1831_g2334 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1889_g2334 = dotResult1831_g2334;
				float dotResult1836_g2334 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1892_g2334 = dotResult1836_g2334;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1604_g2334 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1652_g2334 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1604_g2334 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1604_g2334 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1760_g2334 = saturate( ( smoothstepResult1652_g2334 * 0.5 ) );
				float3 appendResult2078_g2334 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1772_g2334 = input.positionOS.xyz;
				float3 appendResult1880_g2334 = (float3(break1772_g2334.x , 0.0 , ( break1772_g2334.z * 0.15 )));
				float mulTime1949_g2334 = _TimeParameters.x * 2.3;
				float dotResult1754_g2334 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1810_g2334 = dotResult1754_g2334;
				float dotResult1640_g2334 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1666_g2334 = dotResult1640_g2334;
				float3 appendResult2019_g2334 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1728_g2334 = input.positionOS.xyz;
				float3 appendResult1828_g2334 = (float3(0.0 , ( break1728_g2334.y * 0.2 ) , ( break1728_g2334.z * 0.4 )));
				float mulTime1882_g2334 = _TimeParameters.x * 2.0;
				float dotResult1832_g2334 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1888_g2334 = dotResult1832_g2334;
				float dotResult1835_g2334 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1891_g2334 = dotResult1835_g2334;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult2066_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime2067_g2334 = _TimeParameters.x * 0.25;
				float simplePerlin2D3191_g2334 = snoise( ( normalizeResult2066_g2334 + mulTime2067_g2334 ).xy*0.43 );
				float WindMask_LargeB2270_g2334 = ( simplePerlin2D3191_g2334 * 1.5 );
				float mulTime1940_g2334 = _TimeParameters.x * 3.2;
				float3 temp_output_2070_0_g2334 = ( ( mulTime1940_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_K1889_g2334 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1893_g2334 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask2025_g2334 = saturate( dotResult1893_g2334 );
				float mulTime1937_g2334 = _TimeParameters.x * 2.3;
				float dotResult1833_g2334 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1890_g2334 = dotResult1833_g2334;
				float dotResult1834_g2334 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1887_g2334 = dotResult1834_g2334;
				float3 temp_output_2073_0_g2334 = ( ( mulTime1937_g2334 + RandomSeedVector_L1890_g2334 + RandomSeedVector_M1887_g2334 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1934_g2334 = _TimeParameters.x * 3.6;
				float temp_output_2076_0_g2334 = ( ( mulTime1934_g2334 + RandomSeedVector_N1891_g2334 + RandomSeedVector_O1892_g2334 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1817_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime1818_g2334 = _TimeParameters.x * 0.26;
				float simplePerlin2D1923_g2334 = snoise( ( normalizeResult1817_g2334 + mulTime1818_g2334 ).xy*0.7 );
				float WindMask_LargeC2062_g2334 = ( simplePerlin2D1923_g2334 * 1.5 );
				float3 PIWI_02Gentle2481_g2334 = ( ( ( ( ( rotatedValue2147_g2334 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult2083_g2334 + ( appendResult1885_g2334 * cos( mulTime1956_g2334 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1885_g2334 ) ) * sin( ( mulTime1956_g2334 + RandomSeedVector_K1889_g2334 + RandomSeedVector_O1892_g2334 ) ) ) ) * 0.1 ) * BranchMask2026_g2334 * RandomIDBranchPositionMask1760_g2334 ) + ( ( ( appendResult2078_g2334 + ( appendResult1880_g2334 * cos( ( mulTime1949_g2334 + RandomSeedVector_A1810_g2334 + RandomSeedVector_G1666_g2334 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1880_g2334 ) ) * sin( ( mulTime1949_g2334 + RandomSeedVector_D1704_g2334 ) ) ) ) * BranchMask2026_g2334 ) * 0.1 ) + ( ( ( ( appendResult2019_g2334 + ( appendResult1828_g2334 * cos( mulTime1882_g2334 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1828_g2334 ) ) * sin( ( mulTime1882_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_N1891_g2334 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1760_g2334 * BranchMask2026_g2334 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB2270_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) + ( ( ( ( cos( temp_output_2070_0_g2334 ) * sin( temp_output_2070_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( cos( temp_output_2073_0_g2334 ) * sin( temp_output_2073_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( sin( temp_output_2076_0_g2334 ) * cos( temp_output_2076_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC2062_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) ) * 0.4 );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float2 appendResult1711_g2334 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1759_g2334 = appendResult1711_g2334;
				float4 WindDirection1609_g2334 = _WindDirection;
				float GlobalVar_WindSpeed1633_g2334 = _StrongWindSpeed;
				float2 NoiseRotation_Output1710_g2334 = ( -(WindDirection1609_g2334).xz * _TimeParameters.x * GlobalVar_WindSpeed1633_g2334 );
				float2 WPRG2D1990_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + NoiseRotation_Output1710_g2334 );
				float simpleNoise2607_g2334 = SimpleNoise( WPRG2D1990_g2334 );
				float3 break2587_g2334 = sin( ( ( _TimeParameters.y * 10.0 ) + ( ase_positionWS * 10.0 ) + input.tangentOS.xyz ) );
				float3 appendResult2598_g2334 = (float3(break2587_g2334.x , ( break2587_g2334.y * 0.3 ) , break2587_g2334.z));
				float3 smoothstepResult2606_g2334 = smoothstep( float3( 0,0,0 ) , float3( 2,2,2 ) , appendResult2598_g2334);
				float smoothstepResult1925_g2334 = smoothstep( 0.0 , _VertexColorPower , input.ase_color.g);
				float4 temp_cast_9 = (saturate( smoothstepResult1925_g2334 )).xxxx;
				float4 LeafVertexColor_Main2117_g2334 = (( _SwitchVGreenToRGBA )?( input.ase_color ):( temp_cast_9 ));
				float3 worldToObjDir2632_g2334 = mul( GetWorldToObjectMatrix(), float4( ( (simpleNoise2607_g2334*2.2 + -1.05) * float4( ( smoothstepResult2606_g2334 * 0.2 ) , 0.0 ) * input.positionOS.xyz.y * sin( _TimeParameters.x * 0.125 ) * LeafVertexColor_Main2117_g2334 ).rgb, 0.0 ) ).xyz;
				float3 NormaliseRotationAxis2409_g2334 = float3( 0, 1, 0 );
				float simplePerlin2D2430_g2334 = snoise( WPRG2D1990_g2334*0.6 );
				simplePerlin2D2430_g2334 = simplePerlin2D2430_g2334*0.5 + 0.5;
				float NoiseLarge2431_g2334 = simplePerlin2D2430_g2334;
				float mulTime2580_g2334 = _TimeParameters.x * 2.0;
				float3 rotatedValue2609_g2334 = RotateAroundAxis( float3( 0,0,0 ), ( cos( ( ( ase_positionWS * 0.2 ) + mulTime2580_g2334 ) ) * _GrassSwayPowerGentleWind * saturate( input.positionOS.xyz.y ) ), NormaliseRotationAxis2409_g2334, NoiseLarge2431_g2334 );
				float3 worldToObjDir2623_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindDirection1609_g2334 * float4( rotatedValue2609_g2334 , 0.0 ) ).xyz, 0.0 ) ).xyz;
				float simplePerlin2D2557_g2334 = snoise( WPRG2D1990_g2334*0.1 );
				float MaskRotation2559_g2334 = saturate( simplePerlin2D2557_g2334 );
				float3 temp_output_2605_0_g2334 = ( input.positionOS.xyz * 0.25 );
				float3 rotatedValue2610_g2334 = RotateAroundAxis( float3( 0,0,0 ), temp_output_2605_0_g2334, NormaliseRotationAxis2409_g2334, ( saturate( input.positionOS.xyz.y ) * NoiseLarge2431_g2334 ) );
				float3 worldToObjDir2628_g2334 = mul( GetWorldToObjectMatrix(), float4( ( rotatedValue2610_g2334 - temp_output_2605_0_g2334 ), 0.0 ) ).xyz;
				float4 GentleNoise2639_g2334 = ( float4( ( ase_objectScale * worldToObjDir2632_g2334 ) , 0.0 ) + ( ( float4( worldToObjDir2623_g2334 , 0.0 ) * MaskRotation2559_g2334 * LeafVertexColor_Main2117_g2334 * float4( ase_objectScale , 0.0 ) ) * 0.4 ) + ( MaskRotation2559_g2334 * float4( worldToObjDir2628_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 ) );
				float4 normalizeResult2396_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 break2391_g2334 = (normalizeResult2396_g2334).xyz;
				float4 appendResult2387_g2334 = (float4(break2391_g2334.x , ( break2391_g2334.y + _DownwardStrength ) , break2391_g2334.z , 0.0));
				float4 WindMotion_BaseG2644_g2334 = ( appendResult2387_g2334 * saturate( input.positionOS.xyz.y ) );
				float2 WPRG2D_S41918_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + ( NoiseRotation_Output1710_g2334 * 4.0 ) );
				float simplePerlin2D2394_g2334 = snoise( WPRG2D_S41918_g2334*0.2 );
				simplePerlin2D2394_g2334 = simplePerlin2D2394_g2334*0.5 + 0.5;
				float WindMotionNoise2407_g2334 = simplePerlin2D2394_g2334;
				float saferPower1873_g2334 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1999_g2334 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1873_g2334 , 0.1 ) ));
				float TrunkHeightMask2118_g2334 = saturate( smoothstepResult1999_g2334 );
				float4 MotionBendingGentleRandom2426_g2334 = ( WindMotion_BaseG2644_g2334 * _MotionBendingGentleRandom * WindMotionNoise2407_g2334 * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float GlobalVar_WindMotion1991_g2334 = _WindMotion;
				float3 worldToObjDir2379_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG2644_g2334 *  (0.0 + ( GlobalVar_WindMotion1991_g2334 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise2407_g2334 ).xyz, 0.0 ) ).xyz;
				float3 MotionBendingGentleWind2427_g2334 = ( worldToObjDir2379_g2334 * ase_objectScale * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float4 MotionBendingGentleWind22811_g2334 = ( float4( worldToObjDir2379_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 );
				float dotResult1755_g2334 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1811_g2334 = dotResult1755_g2334;
				float3 appendResult2093_g2334 = (float3(( sin( ( RandomSeedVector_A1810_g2334 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1811_g2334 ) ) * _PivotRandomnessStrength )));
				float dotResult1767_g2334 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1819_g2334 = dotResult1767_g2334;
				float4 normalizeResult2158_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 worldToObjDir2240_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult2093_g2334 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1819_g2334 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult2158_g2334 * input.positionOS.xyz.y * TrunkHeightMask2118_g2334 ).xyz, 0.0 ) ).xyz;
				float mulTime1748_g2334 = _TimeParameters.x * 4.0;
				float dotResult1641_g2334 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1667_g2334 = dotResult1641_g2334;
				float mulTime1749_g2334 = _TimeParameters.x * 5.2;
				float dotResult1642_g2334 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1668_g2334 = dotResult1642_g2334;
				float3 rotatedValue2089_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1666_g2334 * 0.02 ) + mulTime1748_g2334 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1667_g2334 ) ) ) + sin( ( mulTime1749_g2334 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1668_g2334 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 ).x );
				float3 worldToObjDir2238_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue2089_g2334 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1609_g2334 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2815_g2334 = ( ( ( worldToObjDir2240_g2334 * ase_objectScale * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir2238_g2334 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1496_g2334 = ( float4( PIWI_02Gentle2481_g2334 , 0.0 ) + GentleNoise2639_g2334 + MotionBendingGentleRandom2426_g2334 + float4( MotionBendingGentleWind2427_g2334 , 0.0 ) + MotionBendingGentleWind22811_g2334 + float4( PIWI_01Gentle2815_g2334 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output163_g2334 = ( GlobalVar_WindStrength2401_g2334 * staticSwitch1496_g2334 );
				
				float3 ase_viewVectorWS = ( ( unity_OrthoParams.w == 0 ) ? _WorldSpaceCameraPos - ase_positionWS : UNITY_MATRIX_V[ 2 ].xyz );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 ase_normalWS = TransformObjectToWorldNormal( input.normalOS );
				float3 normalizedWorldNormal = normalize( ase_normalWS );
				float dotResult494_g2338 = dot( ase_viewDirWS , normalizedWorldNormal );
				float2 uv_NormalMap789_g2338 = input.ase_texcoord.xy;
				float3 unpack490_g2338 = UnpackNormalScale( -tex2Dlod( _NormalMap, float4( uv_NormalMap789_g2338, 0, 0.0) ), -1.0 );
				unpack490_g2338.z = lerp( 1, unpack490_g2338.z, saturate(-1.0) );
				float3 ifLocalVar497_g2338 = 0;
				if( dotResult494_g2338 > 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 == 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 < 0.0 )
				ifLocalVar497_g2338 = -input.normalOS;
				float4 transform500_g2338 = mul(GetObjectToWorldMatrix(),float4( ifLocalVar497_g2338 , 0.0 ));
				float dotResult504_g2338 = dot( float4( SafeNormalize( _MainLightPosition.xyz ) , 0.0 ) , transform500_g2338 );
				float3 ifLocalVar511_g2338 = 0;
				if( dotResult504_g2338 >= 0.0 )
				ifLocalVar511_g2338 = ifLocalVar497_g2338;
				else
				ifLocalVar511_g2338 = -ifLocalVar497_g2338;
				float3 break514_g2338 = ifLocalVar511_g2338;
				float3 temp_cast_35 = (dotResult504_g2338).xxx;
				float4 appendResult525_g2338 = (float4(break514_g2338.x , ( break514_g2338.y + saturate( ( 1.0 - ( ( distance( float3( 0,0,0 ) , temp_cast_35 ) - 0.2 ) / max( 0.2, 1E-05 ) ) ) ) ) , break514_g2338.z , 0.0));
				float4 LightDetectBackface595_g2338 = appendResult525_g2338;
				#ifdef _MOBILESHADINGWORLDUP_ON
				float4 staticSwitch622_g2338 = float4( float3( 0, 1, 0 ) , 0.0 );
				#else
				float4 staticSwitch622_g2338 = (( _LightDetectBackface )?( LightDetectBackface595_g2338 ):( float4( input.normalOS , 0.0 ) ));
				#endif
				float4 LightDetect_Output597_g2338 = staticSwitch622_g2338;
				
				output.ase_texcoord1.xy = input.ase_texcoord.xy;
				output.ase_texcoord2 = input.positionOS;
				output.ase_color = input.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord1.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output163_g2334.rgb;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = LightDetect_Output597_g2338.xyz;
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
				float4 ase_color : COLOR;
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
				output.ase_color = input.ase_color;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
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

				float2 uv_AlbedoMap513_g2338 = input.ase_texcoord1.xy;
				float2 uv_AlbedoMap662_g2338 = input.ase_texcoord1.xy;
				float4 tex2DNode662_g2338 = tex2D( _AlbedoMap, uv_AlbedoMap662_g2338 );
				float2 uv_NoiseMapGrayscale669_g2338 = input.ase_texcoord1.xy;
				float4 tex2DNode669_g2338 = tex2D( _NoiseMapGrayscale, uv_NoiseMapGrayscale669_g2338 );
				float4 transform910_g2338 = mul(GetObjectToWorldMatrix(),float4( 1,1,1,1 ));
				float4 break906_g2338 = transform910_g2338;
				float RandomColorFix912_g2338 = floor( ( ( break906_g2338.x + break906_g2338.z ) * _RandomColorScale ) );
				float2 temp_cast_0 = (RandomColorFix912_g2338).xx;
				float dotResult4_g2340 = dot( temp_cast_0 , float2( 12.9898,78.233 ) );
				float lerpResult10_g2340 = lerp( 0.0 , 1.0 , frac( ( sin( dotResult4_g2340 ) * 43758.55 ) ));
				float temp_output_915_0_g2338 = saturate( lerpResult10_g2340 );
				float3 normalizeResult439_g2338 = ASESafeNormalize( input.ase_texcoord2.xyz );
				float DryLeafPositionMask443_g2338 = ( (distance( normalizeResult439_g2338 , float3( 0,0.8,0 ) )*1.0 + 0.0) * 1 );
				float temp_output_667_0_g2338 = ( 1.0 - input.ase_color.r );
				float GlobalVar_SeasonChange920_g2338 = _SeasonChangeGlobal;
				float4 lerpResult677_g2338 = lerp( ( _DryLeafColor * ( tex2DNode662_g2338.g * 2 ) ) , tex2DNode662_g2338 , saturate( (( (( _SeasonVertexColorR )?( ( ( temp_output_667_0_g2338 * 0.9 ) + ( temp_output_667_0_g2338 * DryLeafPositionMask443_g2338 * tex2DNode669_g2338.r ) + temp_output_915_0_g2338 ) ):( ( tex2DNode669_g2338.r * temp_output_915_0_g2338 * DryLeafPositionMask443_g2338 ) )) - GlobalVar_SeasonChange920_g2338 )*_DryLeavesScale + _DryLeavesOffset) ));
				float4 SeasonControl_Output676_g2338 = lerpResult677_g2338;
				Gradient gradient752_g2338 = NewGradient( 0, 2, 2, float4( 1, 0.276868, 0, 0 ), float4( 0, 1, 0.7818019, 1 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float4 TextureBasedColorVariation952_g2338 = (tex2D( _NoiseMapGrayscale, ( ase_objectPosition * _ZaWorldoScale ).xy )*_TBCVMapIntenisty + _TBCVMapOffset);
				float4 lerpResult515_g2338 = lerp( SeasonControl_Output676_g2338 , ( ( SeasonControl_Output676_g2338 * 0.5 ) + ( SampleGradient( gradient752_g2338, TextureBasedColorVariation952_g2338.r ) * SeasonControl_Output676_g2338 ) ) , _ColorVariation);
				float2 uv_MaskMapRGBA505_g2338 = input.ase_texcoord1.xy;
				float4 lerpResult521_g2338 = lerp( tex2D( _AlbedoMap, uv_AlbedoMap513_g2338 ) , lerpResult515_g2338 , (( _BranchMaskR )?( tex2D( _MaskMapRGBA, uv_MaskMapRGBA505_g2338 ).r ):( 1.0 )));
				float4 LeafColorVariationSeasons_Output959_g2338 = lerpResult521_g2338;
				float saferPower678_g2338 = abs( input.ase_color.r );
				float3 temp_output_465_0_g2338 = ( ( input.ase_texcoord2.xyz * float3( 2,1.3,2 ) ) / 25.0 );
				float dotResult471_g2338 = dot( temp_output_465_0_g2338 , temp_output_465_0_g2338 );
				float saferPower480_g2338 = abs( saturate( dotResult471_g2338 ) );
				float3 normalizeResult457_g2338 = ASESafeNormalize( input.ase_texcoord2.xyz );
				float SelfShading601_g2338 = saturate( ( saturate( pow( saferPower678_g2338 , _VertexAo ) ) * (( pow( saferPower480_g2338 , 1.5 ) + ( ( 1.0 - (distance( normalizeResult457_g2338 , float3( 0,0.8,0 ) )*0.5 + 0.0) ) * 0.6 ) )*0.92 + -0.16) ) );
				float4 GrassColorVariation_Output586_g2338 = (( _SelfShadingVertexColor )?( ( LeafColorVariationSeasons_Output959_g2338 * (SelfShading601_g2338*_VertexLighting + _VertexShadow) ) ):( LeafColorVariationSeasons_Output959_g2338 ));
				float CustomDRAWERS868_g2338 = ( _TEXTUREMAPS + _TEXTURESETTINGS + _SEASONSETTINGS + _SHADINGSETTINGS );
				float Branch_Mask1022_g2338 = (( _BranchMaskR )?( tex2D( _MaskMapRGBA, uv_MaskMapRGBA505_g2338 ).r ):( 1.0 ));
				float4 lerpResult1023_g2338 = lerp( GrassColorVariation_Output586_g2338 , ( ( CustomDRAWERS868_g2338 + _AlbedoColor ) * GrassColorVariation_Output586_g2338 ) , Branch_Mask1022_g2338);
				float4 Snow_Output834_g2338 = lerpResult1023_g2338;
				float4 AlbedoFinal613_g2338 = Snow_Output834_g2338;
				
				float2 uv_AlbedoMap555_g2338 = input.ase_texcoord1.xy;
				float Opacity_Output559_g2338 = tex2D( _AlbedoMap, uv_AlbedoMap555_g2338 ).a;
				

				float3 BaseColor = AlbedoFinal613_g2338.rgb;
				float Alpha = Opacity_Output559_g2338;
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _AlphaClipping;
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
			#define _SPECULAR_SETUP 1
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
			#define ASE_NEEDS_VERT_TANGENT
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF
			#pragma shader_feature_local _MOBILESHADINGWORLDUP_ON


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
				float4 ase_color : COLOR;
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
			float4 _EmColor;
			float4 _DryLeafColor;
			float4 _AlbedoColor;
			float _BackfaceCulling;
			float _TBCVMapOffset;
			float _ColorVariation;
			float _BranchMaskR;
			float _VertexAo;
			float _VertexLighting;
			float _VertexShadow;
			float _TEXTUREMAPS;
			float _TEXTURESETTINGS;
			float _SEASONSETTINGS;
			float _SHADINGSETTINGS;
			float _NormalBackFaceFixBranch;
			float _NormalIntenisty;
			float _SpecularBackfaceOcclusion;
			float _SpecularPower;
			float _SpecularMapScale;
			float _SpecularMapOffset;
			float _SpecularBias;
			float _SpecularScale;
			float _SpecularStrength;
			float _SmoothnessIntensity;
			float _AmbientOcclusionIntensity;
			float _TTFETREEFOLIAGESHADERMOBILE;
			float _FACERENDERING;
			float _ADVANCEDSETTINGS;
			float _TBCVMapIntenisty;
			float _ZaWorldoScale;
			float _DryLeavesOffset;
			float _DryLeavesScale;
			float _CenterofMass;
			float _MaskRootsAuto;
			float _Radius;
			float _Hardness;
			float _MaskRoots;
			float _RootsPosition;
			float _RootsRadius;
			float _RootsHardness;
			float _BranchMaskScale;
			float _BranchMaskRadious;
			float _BranchSwayPower;
			float _BranchWindLarge;
			float _BranchWindSmall;
			float _SwitchVGreenToRGBA;
			float _VertexColorPower;
			float _GrassSwayPowerGentleWind;
			float _DownwardStrength;
			float _MotionBendingGentleRandom;
			float _TrunkHeightandThickness;
			float _PivotRandomnessStrength;
			float _PivotRandomness;
			float _TEXTUREMAPS1;
			float _WINDMASKSETTINGS1;
			float _LightDetectBackface;
			float _SelfShadingVertexColor;
			float _SeasonVertexColorR;
			float _RandomColorScale;
			float _EmissionIntensity;
			float _AlphaClipping;
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
			sampler2D _AlbedoMap;


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
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
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

				float GlobalVar_WindStrength2401_g2334 = _GlobalWindStrength;
				float mulTime1797_g2334 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1669_g2334 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1703_g2334 = dotResult1669_g2334;
				float dotResult1671_g2334 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1704_g2334 = dotResult1671_g2334;
				float mulTime1796_g2334 = _TimeParameters.x * 4.3;
				float dotResult1670_g2334 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1705_g2334 = dotResult1670_g2334;
				float3 normalizeResult1764_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1996_g2334 = saturate( (distance( normalizeResult1764_g2334 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1635_0_g2334 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1653_g2334 = dot( temp_output_1635_0_g2334 , temp_output_1635_0_g2334 );
				float saferPower1713_g2334 = abs( saturate( dotResult1653_g2334 ) );
				float temp_output_1761_0_g2334 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1713_g2334 , _Hardness ) );
				float3 normalizeResult1623_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1717_g2334 = saturate( (distance( normalizeResult1623_g2334 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1924_g2334 = (( _CenterofMass )?( ( temp_output_1761_0_g2334 * CenterOfMass1717_g2334 ) ):( temp_output_1761_0_g2334 ));
				float saferPower2686_g2334 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2691_g2334 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2686_g2334 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2782_g2334 = float3( 0, -1, 0 );
				float3 appendResult2785_g2334 = (float3(break2782_g2334.x , ( break2782_g2334.y * _RootsPosition ) , break2782_g2334.z));
				float3 temp_output_2789_0_g2334 = ( ( input.positionOS.xyz - appendResult2785_g2334 ) / _RootsRadius );
				float dotResult2790_g2334 = dot( temp_output_2789_0_g2334 , temp_output_2789_0_g2334 );
				float saferPower2793_g2334 = abs( saturate( dotResult2790_g2334 ) );
				float RootsMask_ProxySphere2794_g2334 = pow( saferPower2793_g2334 , _RootsHardness );
				float smoothstepResult1959_g2334 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask2026_g2334 = smoothstepResult1959_g2334;
				float3 rotatedValue2147_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1797_g2334 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1703_g2334 ) + ( RandomSeedVector_D1704_g2334 * 0.02 ) ) ) + sin( ( mulTime1796_g2334 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1705_g2334 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1996_g2334 * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * BranchMask2026_g2334 ).x );
				float3 appendResult2083_g2334 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1775_g2334 = input.positionOS.xyz;
				float3 appendResult1885_g2334 = (float3(break1775_g2334.x , ( break1775_g2334.y * 0.15 ) , 0.0));
				float mulTime1956_g2334 = _TimeParameters.x * 2.1;
				float dotResult1831_g2334 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1889_g2334 = dotResult1831_g2334;
				float dotResult1836_g2334 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1892_g2334 = dotResult1836_g2334;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1604_g2334 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1652_g2334 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1604_g2334 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1604_g2334 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1760_g2334 = saturate( ( smoothstepResult1652_g2334 * 0.5 ) );
				float3 appendResult2078_g2334 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1772_g2334 = input.positionOS.xyz;
				float3 appendResult1880_g2334 = (float3(break1772_g2334.x , 0.0 , ( break1772_g2334.z * 0.15 )));
				float mulTime1949_g2334 = _TimeParameters.x * 2.3;
				float dotResult1754_g2334 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1810_g2334 = dotResult1754_g2334;
				float dotResult1640_g2334 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1666_g2334 = dotResult1640_g2334;
				float3 appendResult2019_g2334 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1728_g2334 = input.positionOS.xyz;
				float3 appendResult1828_g2334 = (float3(0.0 , ( break1728_g2334.y * 0.2 ) , ( break1728_g2334.z * 0.4 )));
				float mulTime1882_g2334 = _TimeParameters.x * 2.0;
				float dotResult1832_g2334 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1888_g2334 = dotResult1832_g2334;
				float dotResult1835_g2334 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1891_g2334 = dotResult1835_g2334;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult2066_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime2067_g2334 = _TimeParameters.x * 0.25;
				float simplePerlin2D3191_g2334 = snoise( ( normalizeResult2066_g2334 + mulTime2067_g2334 ).xy*0.43 );
				float WindMask_LargeB2270_g2334 = ( simplePerlin2D3191_g2334 * 1.5 );
				float mulTime1940_g2334 = _TimeParameters.x * 3.2;
				float3 temp_output_2070_0_g2334 = ( ( mulTime1940_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_K1889_g2334 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1893_g2334 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask2025_g2334 = saturate( dotResult1893_g2334 );
				float mulTime1937_g2334 = _TimeParameters.x * 2.3;
				float dotResult1833_g2334 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1890_g2334 = dotResult1833_g2334;
				float dotResult1834_g2334 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1887_g2334 = dotResult1834_g2334;
				float3 temp_output_2073_0_g2334 = ( ( mulTime1937_g2334 + RandomSeedVector_L1890_g2334 + RandomSeedVector_M1887_g2334 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1934_g2334 = _TimeParameters.x * 3.6;
				float temp_output_2076_0_g2334 = ( ( mulTime1934_g2334 + RandomSeedVector_N1891_g2334 + RandomSeedVector_O1892_g2334 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1817_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime1818_g2334 = _TimeParameters.x * 0.26;
				float simplePerlin2D1923_g2334 = snoise( ( normalizeResult1817_g2334 + mulTime1818_g2334 ).xy*0.7 );
				float WindMask_LargeC2062_g2334 = ( simplePerlin2D1923_g2334 * 1.5 );
				float3 PIWI_02Gentle2481_g2334 = ( ( ( ( ( rotatedValue2147_g2334 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult2083_g2334 + ( appendResult1885_g2334 * cos( mulTime1956_g2334 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1885_g2334 ) ) * sin( ( mulTime1956_g2334 + RandomSeedVector_K1889_g2334 + RandomSeedVector_O1892_g2334 ) ) ) ) * 0.1 ) * BranchMask2026_g2334 * RandomIDBranchPositionMask1760_g2334 ) + ( ( ( appendResult2078_g2334 + ( appendResult1880_g2334 * cos( ( mulTime1949_g2334 + RandomSeedVector_A1810_g2334 + RandomSeedVector_G1666_g2334 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1880_g2334 ) ) * sin( ( mulTime1949_g2334 + RandomSeedVector_D1704_g2334 ) ) ) ) * BranchMask2026_g2334 ) * 0.1 ) + ( ( ( ( appendResult2019_g2334 + ( appendResult1828_g2334 * cos( mulTime1882_g2334 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1828_g2334 ) ) * sin( ( mulTime1882_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_N1891_g2334 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1760_g2334 * BranchMask2026_g2334 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB2270_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) + ( ( ( ( cos( temp_output_2070_0_g2334 ) * sin( temp_output_2070_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( cos( temp_output_2073_0_g2334 ) * sin( temp_output_2073_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( sin( temp_output_2076_0_g2334 ) * cos( temp_output_2076_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC2062_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) ) * 0.4 );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float2 appendResult1711_g2334 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1759_g2334 = appendResult1711_g2334;
				float4 WindDirection1609_g2334 = _WindDirection;
				float GlobalVar_WindSpeed1633_g2334 = _StrongWindSpeed;
				float2 NoiseRotation_Output1710_g2334 = ( -(WindDirection1609_g2334).xz * _TimeParameters.x * GlobalVar_WindSpeed1633_g2334 );
				float2 WPRG2D1990_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + NoiseRotation_Output1710_g2334 );
				float simpleNoise2607_g2334 = SimpleNoise( WPRG2D1990_g2334 );
				float3 break2587_g2334 = sin( ( ( _TimeParameters.y * 10.0 ) + ( ase_positionWS * 10.0 ) + input.tangentOS.xyz ) );
				float3 appendResult2598_g2334 = (float3(break2587_g2334.x , ( break2587_g2334.y * 0.3 ) , break2587_g2334.z));
				float3 smoothstepResult2606_g2334 = smoothstep( float3( 0,0,0 ) , float3( 2,2,2 ) , appendResult2598_g2334);
				float smoothstepResult1925_g2334 = smoothstep( 0.0 , _VertexColorPower , input.ase_color.g);
				float4 temp_cast_9 = (saturate( smoothstepResult1925_g2334 )).xxxx;
				float4 LeafVertexColor_Main2117_g2334 = (( _SwitchVGreenToRGBA )?( input.ase_color ):( temp_cast_9 ));
				float3 worldToObjDir2632_g2334 = mul( GetWorldToObjectMatrix(), float4( ( (simpleNoise2607_g2334*2.2 + -1.05) * float4( ( smoothstepResult2606_g2334 * 0.2 ) , 0.0 ) * input.positionOS.xyz.y * sin( _TimeParameters.x * 0.125 ) * LeafVertexColor_Main2117_g2334 ).rgb, 0.0 ) ).xyz;
				float3 NormaliseRotationAxis2409_g2334 = float3( 0, 1, 0 );
				float simplePerlin2D2430_g2334 = snoise( WPRG2D1990_g2334*0.6 );
				simplePerlin2D2430_g2334 = simplePerlin2D2430_g2334*0.5 + 0.5;
				float NoiseLarge2431_g2334 = simplePerlin2D2430_g2334;
				float mulTime2580_g2334 = _TimeParameters.x * 2.0;
				float3 rotatedValue2609_g2334 = RotateAroundAxis( float3( 0,0,0 ), ( cos( ( ( ase_positionWS * 0.2 ) + mulTime2580_g2334 ) ) * _GrassSwayPowerGentleWind * saturate( input.positionOS.xyz.y ) ), NormaliseRotationAxis2409_g2334, NoiseLarge2431_g2334 );
				float3 worldToObjDir2623_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindDirection1609_g2334 * float4( rotatedValue2609_g2334 , 0.0 ) ).xyz, 0.0 ) ).xyz;
				float simplePerlin2D2557_g2334 = snoise( WPRG2D1990_g2334*0.1 );
				float MaskRotation2559_g2334 = saturate( simplePerlin2D2557_g2334 );
				float3 temp_output_2605_0_g2334 = ( input.positionOS.xyz * 0.25 );
				float3 rotatedValue2610_g2334 = RotateAroundAxis( float3( 0,0,0 ), temp_output_2605_0_g2334, NormaliseRotationAxis2409_g2334, ( saturate( input.positionOS.xyz.y ) * NoiseLarge2431_g2334 ) );
				float3 worldToObjDir2628_g2334 = mul( GetWorldToObjectMatrix(), float4( ( rotatedValue2610_g2334 - temp_output_2605_0_g2334 ), 0.0 ) ).xyz;
				float4 GentleNoise2639_g2334 = ( float4( ( ase_objectScale * worldToObjDir2632_g2334 ) , 0.0 ) + ( ( float4( worldToObjDir2623_g2334 , 0.0 ) * MaskRotation2559_g2334 * LeafVertexColor_Main2117_g2334 * float4( ase_objectScale , 0.0 ) ) * 0.4 ) + ( MaskRotation2559_g2334 * float4( worldToObjDir2628_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 ) );
				float4 normalizeResult2396_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 break2391_g2334 = (normalizeResult2396_g2334).xyz;
				float4 appendResult2387_g2334 = (float4(break2391_g2334.x , ( break2391_g2334.y + _DownwardStrength ) , break2391_g2334.z , 0.0));
				float4 WindMotion_BaseG2644_g2334 = ( appendResult2387_g2334 * saturate( input.positionOS.xyz.y ) );
				float2 WPRG2D_S41918_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + ( NoiseRotation_Output1710_g2334 * 4.0 ) );
				float simplePerlin2D2394_g2334 = snoise( WPRG2D_S41918_g2334*0.2 );
				simplePerlin2D2394_g2334 = simplePerlin2D2394_g2334*0.5 + 0.5;
				float WindMotionNoise2407_g2334 = simplePerlin2D2394_g2334;
				float saferPower1873_g2334 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1999_g2334 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1873_g2334 , 0.1 ) ));
				float TrunkHeightMask2118_g2334 = saturate( smoothstepResult1999_g2334 );
				float4 MotionBendingGentleRandom2426_g2334 = ( WindMotion_BaseG2644_g2334 * _MotionBendingGentleRandom * WindMotionNoise2407_g2334 * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float GlobalVar_WindMotion1991_g2334 = _WindMotion;
				float3 worldToObjDir2379_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG2644_g2334 *  (0.0 + ( GlobalVar_WindMotion1991_g2334 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise2407_g2334 ).xyz, 0.0 ) ).xyz;
				float3 MotionBendingGentleWind2427_g2334 = ( worldToObjDir2379_g2334 * ase_objectScale * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float4 MotionBendingGentleWind22811_g2334 = ( float4( worldToObjDir2379_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 );
				float dotResult1755_g2334 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1811_g2334 = dotResult1755_g2334;
				float3 appendResult2093_g2334 = (float3(( sin( ( RandomSeedVector_A1810_g2334 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1811_g2334 ) ) * _PivotRandomnessStrength )));
				float dotResult1767_g2334 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1819_g2334 = dotResult1767_g2334;
				float4 normalizeResult2158_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 worldToObjDir2240_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult2093_g2334 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1819_g2334 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult2158_g2334 * input.positionOS.xyz.y * TrunkHeightMask2118_g2334 ).xyz, 0.0 ) ).xyz;
				float mulTime1748_g2334 = _TimeParameters.x * 4.0;
				float dotResult1641_g2334 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1667_g2334 = dotResult1641_g2334;
				float mulTime1749_g2334 = _TimeParameters.x * 5.2;
				float dotResult1642_g2334 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1668_g2334 = dotResult1642_g2334;
				float3 rotatedValue2089_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1666_g2334 * 0.02 ) + mulTime1748_g2334 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1667_g2334 ) ) ) + sin( ( mulTime1749_g2334 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1668_g2334 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 ).x );
				float3 worldToObjDir2238_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue2089_g2334 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1609_g2334 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2815_g2334 = ( ( ( worldToObjDir2240_g2334 * ase_objectScale * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir2238_g2334 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1496_g2334 = ( float4( PIWI_02Gentle2481_g2334 , 0.0 ) + GentleNoise2639_g2334 + MotionBendingGentleRandom2426_g2334 + float4( MotionBendingGentleWind2427_g2334 , 0.0 ) + MotionBendingGentleWind22811_g2334 + float4( PIWI_01Gentle2815_g2334 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output163_g2334 = ( GlobalVar_WindStrength2401_g2334 * staticSwitch1496_g2334 );
				
				float3 ase_viewVectorWS = ( ( unity_OrthoParams.w == 0 ) ? _WorldSpaceCameraPos - ase_positionWS : UNITY_MATRIX_V[ 2 ].xyz );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 ase_normalWS = TransformObjectToWorldNormal( input.normalOS );
				float3 normalizedWorldNormal = normalize( ase_normalWS );
				float dotResult494_g2338 = dot( ase_viewDirWS , normalizedWorldNormal );
				float2 uv_NormalMap789_g2338 = input.texcoord.xy;
				float3 unpack490_g2338 = UnpackNormalScale( -tex2Dlod( _NormalMap, float4( uv_NormalMap789_g2338, 0, 0.0) ), -1.0 );
				unpack490_g2338.z = lerp( 1, unpack490_g2338.z, saturate(-1.0) );
				float3 ifLocalVar497_g2338 = 0;
				if( dotResult494_g2338 > 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 == 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 < 0.0 )
				ifLocalVar497_g2338 = -input.normalOS;
				float4 transform500_g2338 = mul(GetObjectToWorldMatrix(),float4( ifLocalVar497_g2338 , 0.0 ));
				float dotResult504_g2338 = dot( float4( SafeNormalize( _MainLightPosition.xyz ) , 0.0 ) , transform500_g2338 );
				float3 ifLocalVar511_g2338 = 0;
				if( dotResult504_g2338 >= 0.0 )
				ifLocalVar511_g2338 = ifLocalVar497_g2338;
				else
				ifLocalVar511_g2338 = -ifLocalVar497_g2338;
				float3 break514_g2338 = ifLocalVar511_g2338;
				float3 temp_cast_35 = (dotResult504_g2338).xxx;
				float4 appendResult525_g2338 = (float4(break514_g2338.x , ( break514_g2338.y + saturate( ( 1.0 - ( ( distance( float3( 0,0,0 ) , temp_cast_35 ) - 0.2 ) / max( 0.2, 1E-05 ) ) ) ) ) , break514_g2338.z , 0.0));
				float4 LightDetectBackface595_g2338 = appendResult525_g2338;
				#ifdef _MOBILESHADINGWORLDUP_ON
				float4 staticSwitch622_g2338 = float4( float3( 0, 1, 0 ) , 0.0 );
				#else
				float4 staticSwitch622_g2338 = (( _LightDetectBackface )?( LightDetectBackface595_g2338 ):( float4( input.normalOS , 0.0 ) ));
				#endif
				float4 LightDetect_Output597_g2338 = staticSwitch622_g2338;
				
				output.ase_texcoord3.xy = input.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output163_g2334.rgb;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = LightDetect_Output597_g2338.xyz;
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
				float4 ase_color : COLOR;

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
				output.ase_color = input.ase_color;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
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
						, uint ase_vface : SV_IsFrontFace )
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

				float2 uv_NormalMap531_g2338 = input.ase_texcoord3.xy;
				float3 unpack531_g2338 = UnpackNormalScale( tex2D( _NormalMap, uv_NormalMap531_g2338 ), _NormalIntenisty );
				unpack531_g2338.z = lerp( 1, unpack531_g2338.z, saturate(_NormalIntenisty) );
				float3 tex2DNode531_g2338 = unpack531_g2338;
				float3 switchResult1020_g2338 = (((ase_vface>0)?(tex2DNode531_g2338):(-tex2DNode531_g2338)));
				float3 Normal_Output557_g2338 = (( _NormalBackFaceFixBranch )?( switchResult1020_g2338 ):( tex2DNode531_g2338 ));
				
				float2 uv_AlbedoMap555_g2338 = input.ase_texcoord3.xy;
				float Opacity_Output559_g2338 = tex2D( _AlbedoMap, uv_AlbedoMap555_g2338 ).a;
				

				float3 Normal = Normal_Output557_g2338;
				float Alpha = Opacity_Output559_g2338;
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _AlphaClipping;
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
			#define _SPECULAR_SETUP 1
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

			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_VERT_TANGENT
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_COLOR
			#define ASE_NEEDS_FRAG_POSITION
			#define ASE_NEEDS_WORLD_TANGENT
			#define ASE_NEEDS_FRAG_WORLD_TANGENT
			#define ASE_NEEDS_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_WORLD_BITANGENT
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF
			#pragma shader_feature_local _MOBILESHADINGWORLDUP_ON


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
				float4 ase_color : COLOR;
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
				float4 ase_texcoord8 : TEXCOORD8;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _EmColor;
			float4 _DryLeafColor;
			float4 _AlbedoColor;
			float _BackfaceCulling;
			float _TBCVMapOffset;
			float _ColorVariation;
			float _BranchMaskR;
			float _VertexAo;
			float _VertexLighting;
			float _VertexShadow;
			float _TEXTUREMAPS;
			float _TEXTURESETTINGS;
			float _SEASONSETTINGS;
			float _SHADINGSETTINGS;
			float _NormalBackFaceFixBranch;
			float _NormalIntenisty;
			float _SpecularBackfaceOcclusion;
			float _SpecularPower;
			float _SpecularMapScale;
			float _SpecularMapOffset;
			float _SpecularBias;
			float _SpecularScale;
			float _SpecularStrength;
			float _SmoothnessIntensity;
			float _AmbientOcclusionIntensity;
			float _TTFETREEFOLIAGESHADERMOBILE;
			float _FACERENDERING;
			float _ADVANCEDSETTINGS;
			float _TBCVMapIntenisty;
			float _ZaWorldoScale;
			float _DryLeavesOffset;
			float _DryLeavesScale;
			float _CenterofMass;
			float _MaskRootsAuto;
			float _Radius;
			float _Hardness;
			float _MaskRoots;
			float _RootsPosition;
			float _RootsRadius;
			float _RootsHardness;
			float _BranchMaskScale;
			float _BranchMaskRadious;
			float _BranchSwayPower;
			float _BranchWindLarge;
			float _BranchWindSmall;
			float _SwitchVGreenToRGBA;
			float _VertexColorPower;
			float _GrassSwayPowerGentleWind;
			float _DownwardStrength;
			float _MotionBendingGentleRandom;
			float _TrunkHeightandThickness;
			float _PivotRandomnessStrength;
			float _PivotRandomness;
			float _TEXTUREMAPS1;
			float _WINDMASKSETTINGS1;
			float _LightDetectBackface;
			float _SelfShadingVertexColor;
			float _SeasonVertexColorR;
			float _RandomColorScale;
			float _EmissionIntensity;
			float _AlphaClipping;
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
			sampler2D _AlbedoMap;
			sampler2D _NoiseMapGrayscale;
			float _SeasonChangeGlobal;
			sampler2D _MaskMapRGBA;
			sampler2D _SpecularMap;
			sampler2D _EmissionMap;


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
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
			}
			
			float4 ASESafeNormalize(float4 inVec)
			{
				float dp3 = max(1.175494351e-38, dot(inVec, inVec));
				return inVec* rsqrt(dp3);
			}
			
			
			float4 SampleGradient( Gradient gradient, float time )
			{
				float3 color = gradient.colors[0].rgb;
				UNITY_UNROLL
				for (int c = 1; c < 8; c++)
				{
				float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, gradient.colorsLength-1));
				color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
				}
				#ifndef UNITY_COLORSPACE_GAMMA
				color = SRGBToLinear(color);
				#endif
				float alpha = gradient.alphas[0].x;
				UNITY_UNROLL
				for (int a = 1; a < 8; a++)
				{
				float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, gradient.alphasLength-1));
				alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
				}
				return float4(color, alpha);
			}
			

			PackedVaryings VertexFunction( Attributes input  )
			{
				PackedVaryings output = (PackedVaryings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float GlobalVar_WindStrength2401_g2334 = _GlobalWindStrength;
				float mulTime1797_g2334 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1669_g2334 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1703_g2334 = dotResult1669_g2334;
				float dotResult1671_g2334 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1704_g2334 = dotResult1671_g2334;
				float mulTime1796_g2334 = _TimeParameters.x * 4.3;
				float dotResult1670_g2334 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1705_g2334 = dotResult1670_g2334;
				float3 normalizeResult1764_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1996_g2334 = saturate( (distance( normalizeResult1764_g2334 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1635_0_g2334 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1653_g2334 = dot( temp_output_1635_0_g2334 , temp_output_1635_0_g2334 );
				float saferPower1713_g2334 = abs( saturate( dotResult1653_g2334 ) );
				float temp_output_1761_0_g2334 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1713_g2334 , _Hardness ) );
				float3 normalizeResult1623_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1717_g2334 = saturate( (distance( normalizeResult1623_g2334 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1924_g2334 = (( _CenterofMass )?( ( temp_output_1761_0_g2334 * CenterOfMass1717_g2334 ) ):( temp_output_1761_0_g2334 ));
				float saferPower2686_g2334 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2691_g2334 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2686_g2334 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2782_g2334 = float3( 0, -1, 0 );
				float3 appendResult2785_g2334 = (float3(break2782_g2334.x , ( break2782_g2334.y * _RootsPosition ) , break2782_g2334.z));
				float3 temp_output_2789_0_g2334 = ( ( input.positionOS.xyz - appendResult2785_g2334 ) / _RootsRadius );
				float dotResult2790_g2334 = dot( temp_output_2789_0_g2334 , temp_output_2789_0_g2334 );
				float saferPower2793_g2334 = abs( saturate( dotResult2790_g2334 ) );
				float RootsMask_ProxySphere2794_g2334 = pow( saferPower2793_g2334 , _RootsHardness );
				float smoothstepResult1959_g2334 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask2026_g2334 = smoothstepResult1959_g2334;
				float3 rotatedValue2147_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1797_g2334 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1703_g2334 ) + ( RandomSeedVector_D1704_g2334 * 0.02 ) ) ) + sin( ( mulTime1796_g2334 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1705_g2334 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1996_g2334 * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * BranchMask2026_g2334 ).x );
				float3 appendResult2083_g2334 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1775_g2334 = input.positionOS.xyz;
				float3 appendResult1885_g2334 = (float3(break1775_g2334.x , ( break1775_g2334.y * 0.15 ) , 0.0));
				float mulTime1956_g2334 = _TimeParameters.x * 2.1;
				float dotResult1831_g2334 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1889_g2334 = dotResult1831_g2334;
				float dotResult1836_g2334 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1892_g2334 = dotResult1836_g2334;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1604_g2334 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1652_g2334 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1604_g2334 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1604_g2334 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1760_g2334 = saturate( ( smoothstepResult1652_g2334 * 0.5 ) );
				float3 appendResult2078_g2334 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1772_g2334 = input.positionOS.xyz;
				float3 appendResult1880_g2334 = (float3(break1772_g2334.x , 0.0 , ( break1772_g2334.z * 0.15 )));
				float mulTime1949_g2334 = _TimeParameters.x * 2.3;
				float dotResult1754_g2334 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1810_g2334 = dotResult1754_g2334;
				float dotResult1640_g2334 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1666_g2334 = dotResult1640_g2334;
				float3 appendResult2019_g2334 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1728_g2334 = input.positionOS.xyz;
				float3 appendResult1828_g2334 = (float3(0.0 , ( break1728_g2334.y * 0.2 ) , ( break1728_g2334.z * 0.4 )));
				float mulTime1882_g2334 = _TimeParameters.x * 2.0;
				float dotResult1832_g2334 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1888_g2334 = dotResult1832_g2334;
				float dotResult1835_g2334 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1891_g2334 = dotResult1835_g2334;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult2066_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime2067_g2334 = _TimeParameters.x * 0.25;
				float simplePerlin2D3191_g2334 = snoise( ( normalizeResult2066_g2334 + mulTime2067_g2334 ).xy*0.43 );
				float WindMask_LargeB2270_g2334 = ( simplePerlin2D3191_g2334 * 1.5 );
				float mulTime1940_g2334 = _TimeParameters.x * 3.2;
				float3 temp_output_2070_0_g2334 = ( ( mulTime1940_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_K1889_g2334 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1893_g2334 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask2025_g2334 = saturate( dotResult1893_g2334 );
				float mulTime1937_g2334 = _TimeParameters.x * 2.3;
				float dotResult1833_g2334 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1890_g2334 = dotResult1833_g2334;
				float dotResult1834_g2334 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1887_g2334 = dotResult1834_g2334;
				float3 temp_output_2073_0_g2334 = ( ( mulTime1937_g2334 + RandomSeedVector_L1890_g2334 + RandomSeedVector_M1887_g2334 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1934_g2334 = _TimeParameters.x * 3.6;
				float temp_output_2076_0_g2334 = ( ( mulTime1934_g2334 + RandomSeedVector_N1891_g2334 + RandomSeedVector_O1892_g2334 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1817_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime1818_g2334 = _TimeParameters.x * 0.26;
				float simplePerlin2D1923_g2334 = snoise( ( normalizeResult1817_g2334 + mulTime1818_g2334 ).xy*0.7 );
				float WindMask_LargeC2062_g2334 = ( simplePerlin2D1923_g2334 * 1.5 );
				float3 PIWI_02Gentle2481_g2334 = ( ( ( ( ( rotatedValue2147_g2334 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult2083_g2334 + ( appendResult1885_g2334 * cos( mulTime1956_g2334 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1885_g2334 ) ) * sin( ( mulTime1956_g2334 + RandomSeedVector_K1889_g2334 + RandomSeedVector_O1892_g2334 ) ) ) ) * 0.1 ) * BranchMask2026_g2334 * RandomIDBranchPositionMask1760_g2334 ) + ( ( ( appendResult2078_g2334 + ( appendResult1880_g2334 * cos( ( mulTime1949_g2334 + RandomSeedVector_A1810_g2334 + RandomSeedVector_G1666_g2334 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1880_g2334 ) ) * sin( ( mulTime1949_g2334 + RandomSeedVector_D1704_g2334 ) ) ) ) * BranchMask2026_g2334 ) * 0.1 ) + ( ( ( ( appendResult2019_g2334 + ( appendResult1828_g2334 * cos( mulTime1882_g2334 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1828_g2334 ) ) * sin( ( mulTime1882_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_N1891_g2334 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1760_g2334 * BranchMask2026_g2334 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB2270_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) + ( ( ( ( cos( temp_output_2070_0_g2334 ) * sin( temp_output_2070_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( cos( temp_output_2073_0_g2334 ) * sin( temp_output_2073_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( sin( temp_output_2076_0_g2334 ) * cos( temp_output_2076_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC2062_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) ) * 0.4 );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float2 appendResult1711_g2334 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1759_g2334 = appendResult1711_g2334;
				float4 WindDirection1609_g2334 = _WindDirection;
				float GlobalVar_WindSpeed1633_g2334 = _StrongWindSpeed;
				float2 NoiseRotation_Output1710_g2334 = ( -(WindDirection1609_g2334).xz * _TimeParameters.x * GlobalVar_WindSpeed1633_g2334 );
				float2 WPRG2D1990_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + NoiseRotation_Output1710_g2334 );
				float simpleNoise2607_g2334 = SimpleNoise( WPRG2D1990_g2334 );
				float3 break2587_g2334 = sin( ( ( _TimeParameters.y * 10.0 ) + ( ase_positionWS * 10.0 ) + input.tangentOS.xyz ) );
				float3 appendResult2598_g2334 = (float3(break2587_g2334.x , ( break2587_g2334.y * 0.3 ) , break2587_g2334.z));
				float3 smoothstepResult2606_g2334 = smoothstep( float3( 0,0,0 ) , float3( 2,2,2 ) , appendResult2598_g2334);
				float smoothstepResult1925_g2334 = smoothstep( 0.0 , _VertexColorPower , input.ase_color.g);
				float4 temp_cast_9 = (saturate( smoothstepResult1925_g2334 )).xxxx;
				float4 LeafVertexColor_Main2117_g2334 = (( _SwitchVGreenToRGBA )?( input.ase_color ):( temp_cast_9 ));
				float3 worldToObjDir2632_g2334 = mul( GetWorldToObjectMatrix(), float4( ( (simpleNoise2607_g2334*2.2 + -1.05) * float4( ( smoothstepResult2606_g2334 * 0.2 ) , 0.0 ) * input.positionOS.xyz.y * sin( _TimeParameters.x * 0.125 ) * LeafVertexColor_Main2117_g2334 ).rgb, 0.0 ) ).xyz;
				float3 NormaliseRotationAxis2409_g2334 = float3( 0, 1, 0 );
				float simplePerlin2D2430_g2334 = snoise( WPRG2D1990_g2334*0.6 );
				simplePerlin2D2430_g2334 = simplePerlin2D2430_g2334*0.5 + 0.5;
				float NoiseLarge2431_g2334 = simplePerlin2D2430_g2334;
				float mulTime2580_g2334 = _TimeParameters.x * 2.0;
				float3 rotatedValue2609_g2334 = RotateAroundAxis( float3( 0,0,0 ), ( cos( ( ( ase_positionWS * 0.2 ) + mulTime2580_g2334 ) ) * _GrassSwayPowerGentleWind * saturate( input.positionOS.xyz.y ) ), NormaliseRotationAxis2409_g2334, NoiseLarge2431_g2334 );
				float3 worldToObjDir2623_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindDirection1609_g2334 * float4( rotatedValue2609_g2334 , 0.0 ) ).xyz, 0.0 ) ).xyz;
				float simplePerlin2D2557_g2334 = snoise( WPRG2D1990_g2334*0.1 );
				float MaskRotation2559_g2334 = saturate( simplePerlin2D2557_g2334 );
				float3 temp_output_2605_0_g2334 = ( input.positionOS.xyz * 0.25 );
				float3 rotatedValue2610_g2334 = RotateAroundAxis( float3( 0,0,0 ), temp_output_2605_0_g2334, NormaliseRotationAxis2409_g2334, ( saturate( input.positionOS.xyz.y ) * NoiseLarge2431_g2334 ) );
				float3 worldToObjDir2628_g2334 = mul( GetWorldToObjectMatrix(), float4( ( rotatedValue2610_g2334 - temp_output_2605_0_g2334 ), 0.0 ) ).xyz;
				float4 GentleNoise2639_g2334 = ( float4( ( ase_objectScale * worldToObjDir2632_g2334 ) , 0.0 ) + ( ( float4( worldToObjDir2623_g2334 , 0.0 ) * MaskRotation2559_g2334 * LeafVertexColor_Main2117_g2334 * float4( ase_objectScale , 0.0 ) ) * 0.4 ) + ( MaskRotation2559_g2334 * float4( worldToObjDir2628_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 ) );
				float4 normalizeResult2396_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 break2391_g2334 = (normalizeResult2396_g2334).xyz;
				float4 appendResult2387_g2334 = (float4(break2391_g2334.x , ( break2391_g2334.y + _DownwardStrength ) , break2391_g2334.z , 0.0));
				float4 WindMotion_BaseG2644_g2334 = ( appendResult2387_g2334 * saturate( input.positionOS.xyz.y ) );
				float2 WPRG2D_S41918_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + ( NoiseRotation_Output1710_g2334 * 4.0 ) );
				float simplePerlin2D2394_g2334 = snoise( WPRG2D_S41918_g2334*0.2 );
				simplePerlin2D2394_g2334 = simplePerlin2D2394_g2334*0.5 + 0.5;
				float WindMotionNoise2407_g2334 = simplePerlin2D2394_g2334;
				float saferPower1873_g2334 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1999_g2334 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1873_g2334 , 0.1 ) ));
				float TrunkHeightMask2118_g2334 = saturate( smoothstepResult1999_g2334 );
				float4 MotionBendingGentleRandom2426_g2334 = ( WindMotion_BaseG2644_g2334 * _MotionBendingGentleRandom * WindMotionNoise2407_g2334 * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float GlobalVar_WindMotion1991_g2334 = _WindMotion;
				float3 worldToObjDir2379_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG2644_g2334 *  (0.0 + ( GlobalVar_WindMotion1991_g2334 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise2407_g2334 ).xyz, 0.0 ) ).xyz;
				float3 MotionBendingGentleWind2427_g2334 = ( worldToObjDir2379_g2334 * ase_objectScale * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float4 MotionBendingGentleWind22811_g2334 = ( float4( worldToObjDir2379_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 );
				float dotResult1755_g2334 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1811_g2334 = dotResult1755_g2334;
				float3 appendResult2093_g2334 = (float3(( sin( ( RandomSeedVector_A1810_g2334 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1811_g2334 ) ) * _PivotRandomnessStrength )));
				float dotResult1767_g2334 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1819_g2334 = dotResult1767_g2334;
				float4 normalizeResult2158_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 worldToObjDir2240_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult2093_g2334 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1819_g2334 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult2158_g2334 * input.positionOS.xyz.y * TrunkHeightMask2118_g2334 ).xyz, 0.0 ) ).xyz;
				float mulTime1748_g2334 = _TimeParameters.x * 4.0;
				float dotResult1641_g2334 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1667_g2334 = dotResult1641_g2334;
				float mulTime1749_g2334 = _TimeParameters.x * 5.2;
				float dotResult1642_g2334 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1668_g2334 = dotResult1642_g2334;
				float3 rotatedValue2089_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1666_g2334 * 0.02 ) + mulTime1748_g2334 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1667_g2334 ) ) ) + sin( ( mulTime1749_g2334 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1668_g2334 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 ).x );
				float3 worldToObjDir2238_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue2089_g2334 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1609_g2334 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2815_g2334 = ( ( ( worldToObjDir2240_g2334 * ase_objectScale * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir2238_g2334 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1496_g2334 = ( float4( PIWI_02Gentle2481_g2334 , 0.0 ) + GentleNoise2639_g2334 + MotionBendingGentleRandom2426_g2334 + float4( MotionBendingGentleWind2427_g2334 , 0.0 ) + MotionBendingGentleWind22811_g2334 + float4( PIWI_01Gentle2815_g2334 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output163_g2334 = ( GlobalVar_WindStrength2401_g2334 * staticSwitch1496_g2334 );
				
				float3 ase_viewVectorWS = ( ( unity_OrthoParams.w == 0 ) ? _WorldSpaceCameraPos - ase_positionWS : UNITY_MATRIX_V[ 2 ].xyz );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 ase_normalWS = TransformObjectToWorldNormal( input.normalOS );
				float3 normalizedWorldNormal = normalize( ase_normalWS );
				float dotResult494_g2338 = dot( ase_viewDirWS , normalizedWorldNormal );
				float2 uv_NormalMap789_g2338 = input.texcoord.xy;
				float3 unpack490_g2338 = UnpackNormalScale( -tex2Dlod( _NormalMap, float4( uv_NormalMap789_g2338, 0, 0.0) ), -1.0 );
				unpack490_g2338.z = lerp( 1, unpack490_g2338.z, saturate(-1.0) );
				float3 ifLocalVar497_g2338 = 0;
				if( dotResult494_g2338 > 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 == 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 < 0.0 )
				ifLocalVar497_g2338 = -input.normalOS;
				float4 transform500_g2338 = mul(GetObjectToWorldMatrix(),float4( ifLocalVar497_g2338 , 0.0 ));
				float dotResult504_g2338 = dot( float4( SafeNormalize( _MainLightPosition.xyz ) , 0.0 ) , transform500_g2338 );
				float3 ifLocalVar511_g2338 = 0;
				if( dotResult504_g2338 >= 0.0 )
				ifLocalVar511_g2338 = ifLocalVar497_g2338;
				else
				ifLocalVar511_g2338 = -ifLocalVar497_g2338;
				float3 break514_g2338 = ifLocalVar511_g2338;
				float3 temp_cast_35 = (dotResult504_g2338).xxx;
				float4 appendResult525_g2338 = (float4(break514_g2338.x , ( break514_g2338.y + saturate( ( 1.0 - ( ( distance( float3( 0,0,0 ) , temp_cast_35 ) - 0.2 ) / max( 0.2, 1E-05 ) ) ) ) ) , break514_g2338.z , 0.0));
				float4 LightDetectBackface595_g2338 = appendResult525_g2338;
				#ifdef _MOBILESHADINGWORLDUP_ON
				float4 staticSwitch622_g2338 = float4( float3( 0, 1, 0 ) , 0.0 );
				#else
				float4 staticSwitch622_g2338 = (( _LightDetectBackface )?( LightDetectBackface595_g2338 ):( float4( input.normalOS , 0.0 ) ));
				#endif
				float4 LightDetect_Output597_g2338 = staticSwitch622_g2338;
				
				output.ase_texcoord7.xy = input.texcoord.xy;
				output.ase_texcoord8 = input.positionOS;
				output.ase_color = input.ase_color;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord7.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output163_g2334.rgb;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = LightDetect_Output597_g2338.xyz;
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
				float4 ase_color : COLOR;

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
				output.ase_color = input.ase_color;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
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
								, uint ase_vface : SV_IsFrontFace )
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

				float2 uv_AlbedoMap513_g2338 = input.ase_texcoord7.xy;
				float2 uv_AlbedoMap662_g2338 = input.ase_texcoord7.xy;
				float4 tex2DNode662_g2338 = tex2D( _AlbedoMap, uv_AlbedoMap662_g2338 );
				float2 uv_NoiseMapGrayscale669_g2338 = input.ase_texcoord7.xy;
				float4 tex2DNode669_g2338 = tex2D( _NoiseMapGrayscale, uv_NoiseMapGrayscale669_g2338 );
				float4 transform910_g2338 = mul(GetObjectToWorldMatrix(),float4( 1,1,1,1 ));
				float4 break906_g2338 = transform910_g2338;
				float RandomColorFix912_g2338 = floor( ( ( break906_g2338.x + break906_g2338.z ) * _RandomColorScale ) );
				float2 temp_cast_0 = (RandomColorFix912_g2338).xx;
				float dotResult4_g2340 = dot( temp_cast_0 , float2( 12.9898,78.233 ) );
				float lerpResult10_g2340 = lerp( 0.0 , 1.0 , frac( ( sin( dotResult4_g2340 ) * 43758.55 ) ));
				float temp_output_915_0_g2338 = saturate( lerpResult10_g2340 );
				float3 normalizeResult439_g2338 = ASESafeNormalize( input.ase_texcoord8.xyz );
				float DryLeafPositionMask443_g2338 = ( (distance( normalizeResult439_g2338 , float3( 0,0.8,0 ) )*1.0 + 0.0) * 1 );
				float temp_output_667_0_g2338 = ( 1.0 - input.ase_color.r );
				float GlobalVar_SeasonChange920_g2338 = _SeasonChangeGlobal;
				float4 lerpResult677_g2338 = lerp( ( _DryLeafColor * ( tex2DNode662_g2338.g * 2 ) ) , tex2DNode662_g2338 , saturate( (( (( _SeasonVertexColorR )?( ( ( temp_output_667_0_g2338 * 0.9 ) + ( temp_output_667_0_g2338 * DryLeafPositionMask443_g2338 * tex2DNode669_g2338.r ) + temp_output_915_0_g2338 ) ):( ( tex2DNode669_g2338.r * temp_output_915_0_g2338 * DryLeafPositionMask443_g2338 ) )) - GlobalVar_SeasonChange920_g2338 )*_DryLeavesScale + _DryLeavesOffset) ));
				float4 SeasonControl_Output676_g2338 = lerpResult677_g2338;
				Gradient gradient752_g2338 = NewGradient( 0, 2, 2, float4( 1, 0.276868, 0, 0 ), float4( 0, 1, 0.7818019, 1 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float4 TextureBasedColorVariation952_g2338 = (tex2D( _NoiseMapGrayscale, ( ase_objectPosition * _ZaWorldoScale ).xy )*_TBCVMapIntenisty + _TBCVMapOffset);
				float4 lerpResult515_g2338 = lerp( SeasonControl_Output676_g2338 , ( ( SeasonControl_Output676_g2338 * 0.5 ) + ( SampleGradient( gradient752_g2338, TextureBasedColorVariation952_g2338.r ) * SeasonControl_Output676_g2338 ) ) , _ColorVariation);
				float2 uv_MaskMapRGBA505_g2338 = input.ase_texcoord7.xy;
				float4 lerpResult521_g2338 = lerp( tex2D( _AlbedoMap, uv_AlbedoMap513_g2338 ) , lerpResult515_g2338 , (( _BranchMaskR )?( tex2D( _MaskMapRGBA, uv_MaskMapRGBA505_g2338 ).r ):( 1.0 )));
				float4 LeafColorVariationSeasons_Output959_g2338 = lerpResult521_g2338;
				float saferPower678_g2338 = abs( input.ase_color.r );
				float3 temp_output_465_0_g2338 = ( ( input.ase_texcoord8.xyz * float3( 2,1.3,2 ) ) / 25.0 );
				float dotResult471_g2338 = dot( temp_output_465_0_g2338 , temp_output_465_0_g2338 );
				float saferPower480_g2338 = abs( saturate( dotResult471_g2338 ) );
				float3 normalizeResult457_g2338 = ASESafeNormalize( input.ase_texcoord8.xyz );
				float SelfShading601_g2338 = saturate( ( saturate( pow( saferPower678_g2338 , _VertexAo ) ) * (( pow( saferPower480_g2338 , 1.5 ) + ( ( 1.0 - (distance( normalizeResult457_g2338 , float3( 0,0.8,0 ) )*0.5 + 0.0) ) * 0.6 ) )*0.92 + -0.16) ) );
				float4 GrassColorVariation_Output586_g2338 = (( _SelfShadingVertexColor )?( ( LeafColorVariationSeasons_Output959_g2338 * (SelfShading601_g2338*_VertexLighting + _VertexShadow) ) ):( LeafColorVariationSeasons_Output959_g2338 ));
				float CustomDRAWERS868_g2338 = ( _TEXTUREMAPS + _TEXTURESETTINGS + _SEASONSETTINGS + _SHADINGSETTINGS );
				float Branch_Mask1022_g2338 = (( _BranchMaskR )?( tex2D( _MaskMapRGBA, uv_MaskMapRGBA505_g2338 ).r ):( 1.0 ));
				float4 lerpResult1023_g2338 = lerp( GrassColorVariation_Output586_g2338 , ( ( CustomDRAWERS868_g2338 + _AlbedoColor ) * GrassColorVariation_Output586_g2338 ) , Branch_Mask1022_g2338);
				float4 Snow_Output834_g2338 = lerpResult1023_g2338;
				float4 AlbedoFinal613_g2338 = Snow_Output834_g2338;
				
				float2 uv_NormalMap531_g2338 = input.ase_texcoord7.xy;
				float3 unpack531_g2338 = UnpackNormalScale( tex2D( _NormalMap, uv_NormalMap531_g2338 ), _NormalIntenisty );
				unpack531_g2338.z = lerp( 1, unpack531_g2338.z, saturate(_NormalIntenisty) );
				float3 tex2DNode531_g2338 = unpack531_g2338;
				float3 switchResult1020_g2338 = (((ase_vface>0)?(tex2DNode531_g2338):(-tex2DNode531_g2338)));
				float3 Normal_Output557_g2338 = (( _NormalBackFaceFixBranch )?( switchResult1020_g2338 ):( tex2DNode531_g2338 ));
				
				float2 uv_SpecularMap893_g2338 = input.ase_texcoord7.xy;
				float3 tanToWorld0 = float3( TangentWS.x, BitangentWS.x, NormalWS.x );
				float3 tanToWorld1 = float3( TangentWS.y, BitangentWS.y, NormalWS.y );
				float3 tanToWorld2 = float3( TangentWS.z, BitangentWS.z, NormalWS.z );
				float3 tanNormal879_g2338 = TangentWS;
				float fresnelNdotV879_g2338 = dot( float3( dot( tanToWorld0, tanNormal879_g2338 ), dot( tanToWorld1, tanNormal879_g2338 ), dot( tanToWorld2, tanNormal879_g2338 ) ), SafeNormalize( _MainLightPosition.xyz ) );
				float fresnelNode879_g2338 = ( _SpecularBias + _SpecularScale * pow( max( 1.0 - fresnelNdotV879_g2338 , 0.0001 ), _SpecularStrength ) );
				float SpecularRecalculate876_g2338 = saturate( fresnelNode879_g2338 );
				float Specular_Output882_g2338 = (( _SpecularBackfaceOcclusion )?( ( ( 0.2 * _SpecularPower ) * saturate( (tex2D( _SpecularMap, uv_SpecularMap893_g2338 ).r*_SpecularMapScale + _SpecularMapOffset) ) * SpecularRecalculate876_g2338 ) ):( 0.04 ));
				float3 temp_cast_4 = (Specular_Output882_g2338).xxx;
				
				float2 uv_MaskMapRGBA535_g2338 = input.ase_texcoord7.xy;
				float4 tex2DNode535_g2338 = tex2D( _MaskMapRGBA, uv_MaskMapRGBA535_g2338 );
				float Smoothness_Output1013_g2338 = ( tex2DNode535_g2338.a * _SmoothnessIntensity );
				
				float AoMapBase538_g2338 = tex2DNode535_g2338.g;
				float saferPower580_g2338 = abs( AoMapBase538_g2338 );
				float AmbientOcclusion_Output582_g2338 = pow( saferPower580_g2338 , _AmbientOcclusionIntensity );
				
				float2 uv_EmissionMap977_g2338 = input.ase_texcoord7.xy;
				float4 Emission_Output978_g2338 = ( float4( tex2D( _EmissionMap, uv_EmissionMap977_g2338 ).rgb , 0.0 ) * _EmColor * _EmissionIntensity );
				
				float2 uv_AlbedoMap555_g2338 = input.ase_texcoord7.xy;
				float Opacity_Output559_g2338 = tex2D( _AlbedoMap, uv_AlbedoMap555_g2338 ).a;
				

				float3 BaseColor = AlbedoFinal613_g2338.rgb;
				float3 Normal = Normal_Output557_g2338;
				float3 Specular = temp_cast_4;
				float Metallic = 0;
				float Smoothness = Smoothness_Output1013_g2338;
				float Occlusion = AmbientOcclusion_Output582_g2338;
				float3 Emission = ( _TTFETREEFOLIAGESHADERMOBILE + _FACERENDERING + _ADVANCEDSETTINGS + Emission_Output978_g2338 ).rgb;
				float Alpha = Opacity_Output559_g2338;
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _AlphaClipping;
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
			#define _SPECULAR_SETUP 1
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
			#define ASE_NEEDS_VERT_TANGENT
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES0
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF
			#pragma shader_feature_local _MOBILESHADINGWORLDUP_ON


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
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _EmColor;
			float4 _DryLeafColor;
			float4 _AlbedoColor;
			float _BackfaceCulling;
			float _TBCVMapOffset;
			float _ColorVariation;
			float _BranchMaskR;
			float _VertexAo;
			float _VertexLighting;
			float _VertexShadow;
			float _TEXTUREMAPS;
			float _TEXTURESETTINGS;
			float _SEASONSETTINGS;
			float _SHADINGSETTINGS;
			float _NormalBackFaceFixBranch;
			float _NormalIntenisty;
			float _SpecularBackfaceOcclusion;
			float _SpecularPower;
			float _SpecularMapScale;
			float _SpecularMapOffset;
			float _SpecularBias;
			float _SpecularScale;
			float _SpecularStrength;
			float _SmoothnessIntensity;
			float _AmbientOcclusionIntensity;
			float _TTFETREEFOLIAGESHADERMOBILE;
			float _FACERENDERING;
			float _ADVANCEDSETTINGS;
			float _TBCVMapIntenisty;
			float _ZaWorldoScale;
			float _DryLeavesOffset;
			float _DryLeavesScale;
			float _CenterofMass;
			float _MaskRootsAuto;
			float _Radius;
			float _Hardness;
			float _MaskRoots;
			float _RootsPosition;
			float _RootsRadius;
			float _RootsHardness;
			float _BranchMaskScale;
			float _BranchMaskRadious;
			float _BranchSwayPower;
			float _BranchWindLarge;
			float _BranchWindSmall;
			float _SwitchVGreenToRGBA;
			float _VertexColorPower;
			float _GrassSwayPowerGentleWind;
			float _DownwardStrength;
			float _MotionBendingGentleRandom;
			float _TrunkHeightandThickness;
			float _PivotRandomnessStrength;
			float _PivotRandomness;
			float _TEXTUREMAPS1;
			float _WINDMASKSETTINGS1;
			float _LightDetectBackface;
			float _SelfShadingVertexColor;
			float _SeasonVertexColorR;
			float _RandomColorScale;
			float _EmissionIntensity;
			float _AlphaClipping;
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
			sampler2D _AlbedoMap;


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
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
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

				float GlobalVar_WindStrength2401_g2334 = _GlobalWindStrength;
				float mulTime1797_g2334 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1669_g2334 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1703_g2334 = dotResult1669_g2334;
				float dotResult1671_g2334 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1704_g2334 = dotResult1671_g2334;
				float mulTime1796_g2334 = _TimeParameters.x * 4.3;
				float dotResult1670_g2334 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1705_g2334 = dotResult1670_g2334;
				float3 normalizeResult1764_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1996_g2334 = saturate( (distance( normalizeResult1764_g2334 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1635_0_g2334 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1653_g2334 = dot( temp_output_1635_0_g2334 , temp_output_1635_0_g2334 );
				float saferPower1713_g2334 = abs( saturate( dotResult1653_g2334 ) );
				float temp_output_1761_0_g2334 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1713_g2334 , _Hardness ) );
				float3 normalizeResult1623_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1717_g2334 = saturate( (distance( normalizeResult1623_g2334 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1924_g2334 = (( _CenterofMass )?( ( temp_output_1761_0_g2334 * CenterOfMass1717_g2334 ) ):( temp_output_1761_0_g2334 ));
				float saferPower2686_g2334 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2691_g2334 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2686_g2334 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2782_g2334 = float3( 0, -1, 0 );
				float3 appendResult2785_g2334 = (float3(break2782_g2334.x , ( break2782_g2334.y * _RootsPosition ) , break2782_g2334.z));
				float3 temp_output_2789_0_g2334 = ( ( input.positionOS.xyz - appendResult2785_g2334 ) / _RootsRadius );
				float dotResult2790_g2334 = dot( temp_output_2789_0_g2334 , temp_output_2789_0_g2334 );
				float saferPower2793_g2334 = abs( saturate( dotResult2790_g2334 ) );
				float RootsMask_ProxySphere2794_g2334 = pow( saferPower2793_g2334 , _RootsHardness );
				float smoothstepResult1959_g2334 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask2026_g2334 = smoothstepResult1959_g2334;
				float3 rotatedValue2147_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1797_g2334 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1703_g2334 ) + ( RandomSeedVector_D1704_g2334 * 0.02 ) ) ) + sin( ( mulTime1796_g2334 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1705_g2334 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1996_g2334 * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * BranchMask2026_g2334 ).x );
				float3 appendResult2083_g2334 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1775_g2334 = input.positionOS.xyz;
				float3 appendResult1885_g2334 = (float3(break1775_g2334.x , ( break1775_g2334.y * 0.15 ) , 0.0));
				float mulTime1956_g2334 = _TimeParameters.x * 2.1;
				float dotResult1831_g2334 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1889_g2334 = dotResult1831_g2334;
				float dotResult1836_g2334 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1892_g2334 = dotResult1836_g2334;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1604_g2334 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1652_g2334 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1604_g2334 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1604_g2334 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1760_g2334 = saturate( ( smoothstepResult1652_g2334 * 0.5 ) );
				float3 appendResult2078_g2334 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1772_g2334 = input.positionOS.xyz;
				float3 appendResult1880_g2334 = (float3(break1772_g2334.x , 0.0 , ( break1772_g2334.z * 0.15 )));
				float mulTime1949_g2334 = _TimeParameters.x * 2.3;
				float dotResult1754_g2334 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1810_g2334 = dotResult1754_g2334;
				float dotResult1640_g2334 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1666_g2334 = dotResult1640_g2334;
				float3 appendResult2019_g2334 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1728_g2334 = input.positionOS.xyz;
				float3 appendResult1828_g2334 = (float3(0.0 , ( break1728_g2334.y * 0.2 ) , ( break1728_g2334.z * 0.4 )));
				float mulTime1882_g2334 = _TimeParameters.x * 2.0;
				float dotResult1832_g2334 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1888_g2334 = dotResult1832_g2334;
				float dotResult1835_g2334 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1891_g2334 = dotResult1835_g2334;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult2066_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime2067_g2334 = _TimeParameters.x * 0.25;
				float simplePerlin2D3191_g2334 = snoise( ( normalizeResult2066_g2334 + mulTime2067_g2334 ).xy*0.43 );
				float WindMask_LargeB2270_g2334 = ( simplePerlin2D3191_g2334 * 1.5 );
				float mulTime1940_g2334 = _TimeParameters.x * 3.2;
				float3 temp_output_2070_0_g2334 = ( ( mulTime1940_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_K1889_g2334 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1893_g2334 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask2025_g2334 = saturate( dotResult1893_g2334 );
				float mulTime1937_g2334 = _TimeParameters.x * 2.3;
				float dotResult1833_g2334 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1890_g2334 = dotResult1833_g2334;
				float dotResult1834_g2334 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1887_g2334 = dotResult1834_g2334;
				float3 temp_output_2073_0_g2334 = ( ( mulTime1937_g2334 + RandomSeedVector_L1890_g2334 + RandomSeedVector_M1887_g2334 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1934_g2334 = _TimeParameters.x * 3.6;
				float temp_output_2076_0_g2334 = ( ( mulTime1934_g2334 + RandomSeedVector_N1891_g2334 + RandomSeedVector_O1892_g2334 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1817_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime1818_g2334 = _TimeParameters.x * 0.26;
				float simplePerlin2D1923_g2334 = snoise( ( normalizeResult1817_g2334 + mulTime1818_g2334 ).xy*0.7 );
				float WindMask_LargeC2062_g2334 = ( simplePerlin2D1923_g2334 * 1.5 );
				float3 PIWI_02Gentle2481_g2334 = ( ( ( ( ( rotatedValue2147_g2334 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult2083_g2334 + ( appendResult1885_g2334 * cos( mulTime1956_g2334 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1885_g2334 ) ) * sin( ( mulTime1956_g2334 + RandomSeedVector_K1889_g2334 + RandomSeedVector_O1892_g2334 ) ) ) ) * 0.1 ) * BranchMask2026_g2334 * RandomIDBranchPositionMask1760_g2334 ) + ( ( ( appendResult2078_g2334 + ( appendResult1880_g2334 * cos( ( mulTime1949_g2334 + RandomSeedVector_A1810_g2334 + RandomSeedVector_G1666_g2334 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1880_g2334 ) ) * sin( ( mulTime1949_g2334 + RandomSeedVector_D1704_g2334 ) ) ) ) * BranchMask2026_g2334 ) * 0.1 ) + ( ( ( ( appendResult2019_g2334 + ( appendResult1828_g2334 * cos( mulTime1882_g2334 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1828_g2334 ) ) * sin( ( mulTime1882_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_N1891_g2334 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1760_g2334 * BranchMask2026_g2334 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB2270_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) + ( ( ( ( cos( temp_output_2070_0_g2334 ) * sin( temp_output_2070_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( cos( temp_output_2073_0_g2334 ) * sin( temp_output_2073_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( sin( temp_output_2076_0_g2334 ) * cos( temp_output_2076_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC2062_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) ) * 0.4 );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float2 appendResult1711_g2334 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1759_g2334 = appendResult1711_g2334;
				float4 WindDirection1609_g2334 = _WindDirection;
				float GlobalVar_WindSpeed1633_g2334 = _StrongWindSpeed;
				float2 NoiseRotation_Output1710_g2334 = ( -(WindDirection1609_g2334).xz * _TimeParameters.x * GlobalVar_WindSpeed1633_g2334 );
				float2 WPRG2D1990_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + NoiseRotation_Output1710_g2334 );
				float simpleNoise2607_g2334 = SimpleNoise( WPRG2D1990_g2334 );
				float3 break2587_g2334 = sin( ( ( _TimeParameters.y * 10.0 ) + ( ase_positionWS * 10.0 ) + input.tangentOS.xyz ) );
				float3 appendResult2598_g2334 = (float3(break2587_g2334.x , ( break2587_g2334.y * 0.3 ) , break2587_g2334.z));
				float3 smoothstepResult2606_g2334 = smoothstep( float3( 0,0,0 ) , float3( 2,2,2 ) , appendResult2598_g2334);
				float smoothstepResult1925_g2334 = smoothstep( 0.0 , _VertexColorPower , input.ase_color.g);
				float4 temp_cast_9 = (saturate( smoothstepResult1925_g2334 )).xxxx;
				float4 LeafVertexColor_Main2117_g2334 = (( _SwitchVGreenToRGBA )?( input.ase_color ):( temp_cast_9 ));
				float3 worldToObjDir2632_g2334 = mul( GetWorldToObjectMatrix(), float4( ( (simpleNoise2607_g2334*2.2 + -1.05) * float4( ( smoothstepResult2606_g2334 * 0.2 ) , 0.0 ) * input.positionOS.xyz.y * sin( _TimeParameters.x * 0.125 ) * LeafVertexColor_Main2117_g2334 ).rgb, 0.0 ) ).xyz;
				float3 NormaliseRotationAxis2409_g2334 = float3( 0, 1, 0 );
				float simplePerlin2D2430_g2334 = snoise( WPRG2D1990_g2334*0.6 );
				simplePerlin2D2430_g2334 = simplePerlin2D2430_g2334*0.5 + 0.5;
				float NoiseLarge2431_g2334 = simplePerlin2D2430_g2334;
				float mulTime2580_g2334 = _TimeParameters.x * 2.0;
				float3 rotatedValue2609_g2334 = RotateAroundAxis( float3( 0,0,0 ), ( cos( ( ( ase_positionWS * 0.2 ) + mulTime2580_g2334 ) ) * _GrassSwayPowerGentleWind * saturate( input.positionOS.xyz.y ) ), NormaliseRotationAxis2409_g2334, NoiseLarge2431_g2334 );
				float3 worldToObjDir2623_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindDirection1609_g2334 * float4( rotatedValue2609_g2334 , 0.0 ) ).xyz, 0.0 ) ).xyz;
				float simplePerlin2D2557_g2334 = snoise( WPRG2D1990_g2334*0.1 );
				float MaskRotation2559_g2334 = saturate( simplePerlin2D2557_g2334 );
				float3 temp_output_2605_0_g2334 = ( input.positionOS.xyz * 0.25 );
				float3 rotatedValue2610_g2334 = RotateAroundAxis( float3( 0,0,0 ), temp_output_2605_0_g2334, NormaliseRotationAxis2409_g2334, ( saturate( input.positionOS.xyz.y ) * NoiseLarge2431_g2334 ) );
				float3 worldToObjDir2628_g2334 = mul( GetWorldToObjectMatrix(), float4( ( rotatedValue2610_g2334 - temp_output_2605_0_g2334 ), 0.0 ) ).xyz;
				float4 GentleNoise2639_g2334 = ( float4( ( ase_objectScale * worldToObjDir2632_g2334 ) , 0.0 ) + ( ( float4( worldToObjDir2623_g2334 , 0.0 ) * MaskRotation2559_g2334 * LeafVertexColor_Main2117_g2334 * float4( ase_objectScale , 0.0 ) ) * 0.4 ) + ( MaskRotation2559_g2334 * float4( worldToObjDir2628_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 ) );
				float4 normalizeResult2396_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 break2391_g2334 = (normalizeResult2396_g2334).xyz;
				float4 appendResult2387_g2334 = (float4(break2391_g2334.x , ( break2391_g2334.y + _DownwardStrength ) , break2391_g2334.z , 0.0));
				float4 WindMotion_BaseG2644_g2334 = ( appendResult2387_g2334 * saturate( input.positionOS.xyz.y ) );
				float2 WPRG2D_S41918_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + ( NoiseRotation_Output1710_g2334 * 4.0 ) );
				float simplePerlin2D2394_g2334 = snoise( WPRG2D_S41918_g2334*0.2 );
				simplePerlin2D2394_g2334 = simplePerlin2D2394_g2334*0.5 + 0.5;
				float WindMotionNoise2407_g2334 = simplePerlin2D2394_g2334;
				float saferPower1873_g2334 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1999_g2334 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1873_g2334 , 0.1 ) ));
				float TrunkHeightMask2118_g2334 = saturate( smoothstepResult1999_g2334 );
				float4 MotionBendingGentleRandom2426_g2334 = ( WindMotion_BaseG2644_g2334 * _MotionBendingGentleRandom * WindMotionNoise2407_g2334 * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float GlobalVar_WindMotion1991_g2334 = _WindMotion;
				float3 worldToObjDir2379_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG2644_g2334 *  (0.0 + ( GlobalVar_WindMotion1991_g2334 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise2407_g2334 ).xyz, 0.0 ) ).xyz;
				float3 MotionBendingGentleWind2427_g2334 = ( worldToObjDir2379_g2334 * ase_objectScale * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float4 MotionBendingGentleWind22811_g2334 = ( float4( worldToObjDir2379_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 );
				float dotResult1755_g2334 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1811_g2334 = dotResult1755_g2334;
				float3 appendResult2093_g2334 = (float3(( sin( ( RandomSeedVector_A1810_g2334 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1811_g2334 ) ) * _PivotRandomnessStrength )));
				float dotResult1767_g2334 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1819_g2334 = dotResult1767_g2334;
				float4 normalizeResult2158_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 worldToObjDir2240_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult2093_g2334 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1819_g2334 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult2158_g2334 * input.positionOS.xyz.y * TrunkHeightMask2118_g2334 ).xyz, 0.0 ) ).xyz;
				float mulTime1748_g2334 = _TimeParameters.x * 4.0;
				float dotResult1641_g2334 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1667_g2334 = dotResult1641_g2334;
				float mulTime1749_g2334 = _TimeParameters.x * 5.2;
				float dotResult1642_g2334 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1668_g2334 = dotResult1642_g2334;
				float3 rotatedValue2089_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1666_g2334 * 0.02 ) + mulTime1748_g2334 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1667_g2334 ) ) ) + sin( ( mulTime1749_g2334 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1668_g2334 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 ).x );
				float3 worldToObjDir2238_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue2089_g2334 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1609_g2334 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2815_g2334 = ( ( ( worldToObjDir2240_g2334 * ase_objectScale * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir2238_g2334 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1496_g2334 = ( float4( PIWI_02Gentle2481_g2334 , 0.0 ) + GentleNoise2639_g2334 + MotionBendingGentleRandom2426_g2334 + float4( MotionBendingGentleWind2427_g2334 , 0.0 ) + MotionBendingGentleWind22811_g2334 + float4( PIWI_01Gentle2815_g2334 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output163_g2334 = ( GlobalVar_WindStrength2401_g2334 * staticSwitch1496_g2334 );
				
				float3 ase_viewVectorWS = ( ( unity_OrthoParams.w == 0 ) ? _WorldSpaceCameraPos - ase_positionWS : UNITY_MATRIX_V[ 2 ].xyz );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 ase_normalWS = TransformObjectToWorldNormal( input.normalOS );
				float3 normalizedWorldNormal = normalize( ase_normalWS );
				float dotResult494_g2338 = dot( ase_viewDirWS , normalizedWorldNormal );
				float2 uv_NormalMap789_g2338 = input.ase_texcoord.xy;
				float3 unpack490_g2338 = UnpackNormalScale( -tex2Dlod( _NormalMap, float4( uv_NormalMap789_g2338, 0, 0.0) ), -1.0 );
				unpack490_g2338.z = lerp( 1, unpack490_g2338.z, saturate(-1.0) );
				float3 ifLocalVar497_g2338 = 0;
				if( dotResult494_g2338 > 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 == 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 < 0.0 )
				ifLocalVar497_g2338 = -input.normalOS;
				float4 transform500_g2338 = mul(GetObjectToWorldMatrix(),float4( ifLocalVar497_g2338 , 0.0 ));
				float dotResult504_g2338 = dot( float4( SafeNormalize( _MainLightPosition.xyz ) , 0.0 ) , transform500_g2338 );
				float3 ifLocalVar511_g2338 = 0;
				if( dotResult504_g2338 >= 0.0 )
				ifLocalVar511_g2338 = ifLocalVar497_g2338;
				else
				ifLocalVar511_g2338 = -ifLocalVar497_g2338;
				float3 break514_g2338 = ifLocalVar511_g2338;
				float3 temp_cast_35 = (dotResult504_g2338).xxx;
				float4 appendResult525_g2338 = (float4(break514_g2338.x , ( break514_g2338.y + saturate( ( 1.0 - ( ( distance( float3( 0,0,0 ) , temp_cast_35 ) - 0.2 ) / max( 0.2, 1E-05 ) ) ) ) ) , break514_g2338.z , 0.0));
				float4 LightDetectBackface595_g2338 = appendResult525_g2338;
				#ifdef _MOBILESHADINGWORLDUP_ON
				float4 staticSwitch622_g2338 = float4( float3( 0, 1, 0 ) , 0.0 );
				#else
				float4 staticSwitch622_g2338 = (( _LightDetectBackface )?( LightDetectBackface595_g2338 ):( float4( input.normalOS , 0.0 ) ));
				#endif
				float4 LightDetect_Output597_g2338 = staticSwitch622_g2338;
				
				output.ase_texcoord1.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord1.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output163_g2334.rgb;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = LightDetect_Output597_g2338.xyz;

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
				float4 ase_color : COLOR;
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
				output.ase_color = input.ase_color;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
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

				float2 uv_AlbedoMap555_g2338 = input.ase_texcoord1.xy;
				float Opacity_Output559_g2338 = tex2D( _AlbedoMap, uv_AlbedoMap555_g2338 ).a;
				

				surfaceDescription.Alpha = Opacity_Output559_g2338;
				#if defined( _ALPHATEST_ON )
					surfaceDescription.AlphaClipThreshold = _AlphaClipping;
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
			#define _SPECULAR_SETUP 1
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
			#define ASE_NEEDS_VERT_TANGENT
			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_VERT_TEXTURE_COORDINATES0
			#pragma shader_feature _WINDTYPE_GENTLEBREEZE _WINDTYPE_WINDOFF
			#pragma shader_feature_local _MOBILESHADINGWORLDUP_ON


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
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _EmColor;
			float4 _DryLeafColor;
			float4 _AlbedoColor;
			float _BackfaceCulling;
			float _TBCVMapOffset;
			float _ColorVariation;
			float _BranchMaskR;
			float _VertexAo;
			float _VertexLighting;
			float _VertexShadow;
			float _TEXTUREMAPS;
			float _TEXTURESETTINGS;
			float _SEASONSETTINGS;
			float _SHADINGSETTINGS;
			float _NormalBackFaceFixBranch;
			float _NormalIntenisty;
			float _SpecularBackfaceOcclusion;
			float _SpecularPower;
			float _SpecularMapScale;
			float _SpecularMapOffset;
			float _SpecularBias;
			float _SpecularScale;
			float _SpecularStrength;
			float _SmoothnessIntensity;
			float _AmbientOcclusionIntensity;
			float _TTFETREEFOLIAGESHADERMOBILE;
			float _FACERENDERING;
			float _ADVANCEDSETTINGS;
			float _TBCVMapIntenisty;
			float _ZaWorldoScale;
			float _DryLeavesOffset;
			float _DryLeavesScale;
			float _CenterofMass;
			float _MaskRootsAuto;
			float _Radius;
			float _Hardness;
			float _MaskRoots;
			float _RootsPosition;
			float _RootsRadius;
			float _RootsHardness;
			float _BranchMaskScale;
			float _BranchMaskRadious;
			float _BranchSwayPower;
			float _BranchWindLarge;
			float _BranchWindSmall;
			float _SwitchVGreenToRGBA;
			float _VertexColorPower;
			float _GrassSwayPowerGentleWind;
			float _DownwardStrength;
			float _MotionBendingGentleRandom;
			float _TrunkHeightandThickness;
			float _PivotRandomnessStrength;
			float _PivotRandomness;
			float _TEXTUREMAPS1;
			float _WINDMASKSETTINGS1;
			float _LightDetectBackface;
			float _SelfShadingVertexColor;
			float _SeasonVertexColorR;
			float _RandomColorScale;
			float _EmissionIntensity;
			float _AlphaClipping;
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
			sampler2D _AlbedoMap;


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
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
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

				float GlobalVar_WindStrength2401_g2334 = _GlobalWindStrength;
				float mulTime1797_g2334 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1669_g2334 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1703_g2334 = dotResult1669_g2334;
				float dotResult1671_g2334 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1704_g2334 = dotResult1671_g2334;
				float mulTime1796_g2334 = _TimeParameters.x * 4.3;
				float dotResult1670_g2334 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1705_g2334 = dotResult1670_g2334;
				float3 normalizeResult1764_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1996_g2334 = saturate( (distance( normalizeResult1764_g2334 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1635_0_g2334 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1653_g2334 = dot( temp_output_1635_0_g2334 , temp_output_1635_0_g2334 );
				float saferPower1713_g2334 = abs( saturate( dotResult1653_g2334 ) );
				float temp_output_1761_0_g2334 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1713_g2334 , _Hardness ) );
				float3 normalizeResult1623_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1717_g2334 = saturate( (distance( normalizeResult1623_g2334 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1924_g2334 = (( _CenterofMass )?( ( temp_output_1761_0_g2334 * CenterOfMass1717_g2334 ) ):( temp_output_1761_0_g2334 ));
				float saferPower2686_g2334 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2691_g2334 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2686_g2334 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2782_g2334 = float3( 0, -1, 0 );
				float3 appendResult2785_g2334 = (float3(break2782_g2334.x , ( break2782_g2334.y * _RootsPosition ) , break2782_g2334.z));
				float3 temp_output_2789_0_g2334 = ( ( input.positionOS.xyz - appendResult2785_g2334 ) / _RootsRadius );
				float dotResult2790_g2334 = dot( temp_output_2789_0_g2334 , temp_output_2789_0_g2334 );
				float saferPower2793_g2334 = abs( saturate( dotResult2790_g2334 ) );
				float RootsMask_ProxySphere2794_g2334 = pow( saferPower2793_g2334 , _RootsHardness );
				float smoothstepResult1959_g2334 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask2026_g2334 = smoothstepResult1959_g2334;
				float3 rotatedValue2147_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1797_g2334 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1703_g2334 ) + ( RandomSeedVector_D1704_g2334 * 0.02 ) ) ) + sin( ( mulTime1796_g2334 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1705_g2334 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1996_g2334 * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * BranchMask2026_g2334 ).x );
				float3 appendResult2083_g2334 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1775_g2334 = input.positionOS.xyz;
				float3 appendResult1885_g2334 = (float3(break1775_g2334.x , ( break1775_g2334.y * 0.15 ) , 0.0));
				float mulTime1956_g2334 = _TimeParameters.x * 2.1;
				float dotResult1831_g2334 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1889_g2334 = dotResult1831_g2334;
				float dotResult1836_g2334 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1892_g2334 = dotResult1836_g2334;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1604_g2334 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1652_g2334 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1604_g2334 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1604_g2334 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1760_g2334 = saturate( ( smoothstepResult1652_g2334 * 0.5 ) );
				float3 appendResult2078_g2334 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1772_g2334 = input.positionOS.xyz;
				float3 appendResult1880_g2334 = (float3(break1772_g2334.x , 0.0 , ( break1772_g2334.z * 0.15 )));
				float mulTime1949_g2334 = _TimeParameters.x * 2.3;
				float dotResult1754_g2334 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1810_g2334 = dotResult1754_g2334;
				float dotResult1640_g2334 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1666_g2334 = dotResult1640_g2334;
				float3 appendResult2019_g2334 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1728_g2334 = input.positionOS.xyz;
				float3 appendResult1828_g2334 = (float3(0.0 , ( break1728_g2334.y * 0.2 ) , ( break1728_g2334.z * 0.4 )));
				float mulTime1882_g2334 = _TimeParameters.x * 2.0;
				float dotResult1832_g2334 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1888_g2334 = dotResult1832_g2334;
				float dotResult1835_g2334 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1891_g2334 = dotResult1835_g2334;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult2066_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime2067_g2334 = _TimeParameters.x * 0.25;
				float simplePerlin2D3191_g2334 = snoise( ( normalizeResult2066_g2334 + mulTime2067_g2334 ).xy*0.43 );
				float WindMask_LargeB2270_g2334 = ( simplePerlin2D3191_g2334 * 1.5 );
				float mulTime1940_g2334 = _TimeParameters.x * 3.2;
				float3 temp_output_2070_0_g2334 = ( ( mulTime1940_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_K1889_g2334 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1893_g2334 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask2025_g2334 = saturate( dotResult1893_g2334 );
				float mulTime1937_g2334 = _TimeParameters.x * 2.3;
				float dotResult1833_g2334 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1890_g2334 = dotResult1833_g2334;
				float dotResult1834_g2334 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1887_g2334 = dotResult1834_g2334;
				float3 temp_output_2073_0_g2334 = ( ( mulTime1937_g2334 + RandomSeedVector_L1890_g2334 + RandomSeedVector_M1887_g2334 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1934_g2334 = _TimeParameters.x * 3.6;
				float temp_output_2076_0_g2334 = ( ( mulTime1934_g2334 + RandomSeedVector_N1891_g2334 + RandomSeedVector_O1892_g2334 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1817_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime1818_g2334 = _TimeParameters.x * 0.26;
				float simplePerlin2D1923_g2334 = snoise( ( normalizeResult1817_g2334 + mulTime1818_g2334 ).xy*0.7 );
				float WindMask_LargeC2062_g2334 = ( simplePerlin2D1923_g2334 * 1.5 );
				float3 PIWI_02Gentle2481_g2334 = ( ( ( ( ( rotatedValue2147_g2334 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult2083_g2334 + ( appendResult1885_g2334 * cos( mulTime1956_g2334 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1885_g2334 ) ) * sin( ( mulTime1956_g2334 + RandomSeedVector_K1889_g2334 + RandomSeedVector_O1892_g2334 ) ) ) ) * 0.1 ) * BranchMask2026_g2334 * RandomIDBranchPositionMask1760_g2334 ) + ( ( ( appendResult2078_g2334 + ( appendResult1880_g2334 * cos( ( mulTime1949_g2334 + RandomSeedVector_A1810_g2334 + RandomSeedVector_G1666_g2334 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1880_g2334 ) ) * sin( ( mulTime1949_g2334 + RandomSeedVector_D1704_g2334 ) ) ) ) * BranchMask2026_g2334 ) * 0.1 ) + ( ( ( ( appendResult2019_g2334 + ( appendResult1828_g2334 * cos( mulTime1882_g2334 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1828_g2334 ) ) * sin( ( mulTime1882_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_N1891_g2334 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1760_g2334 * BranchMask2026_g2334 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB2270_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) + ( ( ( ( cos( temp_output_2070_0_g2334 ) * sin( temp_output_2070_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( cos( temp_output_2073_0_g2334 ) * sin( temp_output_2073_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( sin( temp_output_2076_0_g2334 ) * cos( temp_output_2076_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC2062_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) ) * 0.4 );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float2 appendResult1711_g2334 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1759_g2334 = appendResult1711_g2334;
				float4 WindDirection1609_g2334 = _WindDirection;
				float GlobalVar_WindSpeed1633_g2334 = _StrongWindSpeed;
				float2 NoiseRotation_Output1710_g2334 = ( -(WindDirection1609_g2334).xz * _TimeParameters.x * GlobalVar_WindSpeed1633_g2334 );
				float2 WPRG2D1990_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + NoiseRotation_Output1710_g2334 );
				float simpleNoise2607_g2334 = SimpleNoise( WPRG2D1990_g2334 );
				float3 break2587_g2334 = sin( ( ( _TimeParameters.y * 10.0 ) + ( ase_positionWS * 10.0 ) + input.tangentOS.xyz ) );
				float3 appendResult2598_g2334 = (float3(break2587_g2334.x , ( break2587_g2334.y * 0.3 ) , break2587_g2334.z));
				float3 smoothstepResult2606_g2334 = smoothstep( float3( 0,0,0 ) , float3( 2,2,2 ) , appendResult2598_g2334);
				float smoothstepResult1925_g2334 = smoothstep( 0.0 , _VertexColorPower , input.ase_color.g);
				float4 temp_cast_9 = (saturate( smoothstepResult1925_g2334 )).xxxx;
				float4 LeafVertexColor_Main2117_g2334 = (( _SwitchVGreenToRGBA )?( input.ase_color ):( temp_cast_9 ));
				float3 worldToObjDir2632_g2334 = mul( GetWorldToObjectMatrix(), float4( ( (simpleNoise2607_g2334*2.2 + -1.05) * float4( ( smoothstepResult2606_g2334 * 0.2 ) , 0.0 ) * input.positionOS.xyz.y * sin( _TimeParameters.x * 0.125 ) * LeafVertexColor_Main2117_g2334 ).rgb, 0.0 ) ).xyz;
				float3 NormaliseRotationAxis2409_g2334 = float3( 0, 1, 0 );
				float simplePerlin2D2430_g2334 = snoise( WPRG2D1990_g2334*0.6 );
				simplePerlin2D2430_g2334 = simplePerlin2D2430_g2334*0.5 + 0.5;
				float NoiseLarge2431_g2334 = simplePerlin2D2430_g2334;
				float mulTime2580_g2334 = _TimeParameters.x * 2.0;
				float3 rotatedValue2609_g2334 = RotateAroundAxis( float3( 0,0,0 ), ( cos( ( ( ase_positionWS * 0.2 ) + mulTime2580_g2334 ) ) * _GrassSwayPowerGentleWind * saturate( input.positionOS.xyz.y ) ), NormaliseRotationAxis2409_g2334, NoiseLarge2431_g2334 );
				float3 worldToObjDir2623_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindDirection1609_g2334 * float4( rotatedValue2609_g2334 , 0.0 ) ).xyz, 0.0 ) ).xyz;
				float simplePerlin2D2557_g2334 = snoise( WPRG2D1990_g2334*0.1 );
				float MaskRotation2559_g2334 = saturate( simplePerlin2D2557_g2334 );
				float3 temp_output_2605_0_g2334 = ( input.positionOS.xyz * 0.25 );
				float3 rotatedValue2610_g2334 = RotateAroundAxis( float3( 0,0,0 ), temp_output_2605_0_g2334, NormaliseRotationAxis2409_g2334, ( saturate( input.positionOS.xyz.y ) * NoiseLarge2431_g2334 ) );
				float3 worldToObjDir2628_g2334 = mul( GetWorldToObjectMatrix(), float4( ( rotatedValue2610_g2334 - temp_output_2605_0_g2334 ), 0.0 ) ).xyz;
				float4 GentleNoise2639_g2334 = ( float4( ( ase_objectScale * worldToObjDir2632_g2334 ) , 0.0 ) + ( ( float4( worldToObjDir2623_g2334 , 0.0 ) * MaskRotation2559_g2334 * LeafVertexColor_Main2117_g2334 * float4( ase_objectScale , 0.0 ) ) * 0.4 ) + ( MaskRotation2559_g2334 * float4( worldToObjDir2628_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 ) );
				float4 normalizeResult2396_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 break2391_g2334 = (normalizeResult2396_g2334).xyz;
				float4 appendResult2387_g2334 = (float4(break2391_g2334.x , ( break2391_g2334.y + _DownwardStrength ) , break2391_g2334.z , 0.0));
				float4 WindMotion_BaseG2644_g2334 = ( appendResult2387_g2334 * saturate( input.positionOS.xyz.y ) );
				float2 WPRG2D_S41918_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + ( NoiseRotation_Output1710_g2334 * 4.0 ) );
				float simplePerlin2D2394_g2334 = snoise( WPRG2D_S41918_g2334*0.2 );
				simplePerlin2D2394_g2334 = simplePerlin2D2394_g2334*0.5 + 0.5;
				float WindMotionNoise2407_g2334 = simplePerlin2D2394_g2334;
				float saferPower1873_g2334 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1999_g2334 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1873_g2334 , 0.1 ) ));
				float TrunkHeightMask2118_g2334 = saturate( smoothstepResult1999_g2334 );
				float4 MotionBendingGentleRandom2426_g2334 = ( WindMotion_BaseG2644_g2334 * _MotionBendingGentleRandom * WindMotionNoise2407_g2334 * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float GlobalVar_WindMotion1991_g2334 = _WindMotion;
				float3 worldToObjDir2379_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG2644_g2334 *  (0.0 + ( GlobalVar_WindMotion1991_g2334 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise2407_g2334 ).xyz, 0.0 ) ).xyz;
				float3 MotionBendingGentleWind2427_g2334 = ( worldToObjDir2379_g2334 * ase_objectScale * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float4 MotionBendingGentleWind22811_g2334 = ( float4( worldToObjDir2379_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 );
				float dotResult1755_g2334 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1811_g2334 = dotResult1755_g2334;
				float3 appendResult2093_g2334 = (float3(( sin( ( RandomSeedVector_A1810_g2334 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1811_g2334 ) ) * _PivotRandomnessStrength )));
				float dotResult1767_g2334 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1819_g2334 = dotResult1767_g2334;
				float4 normalizeResult2158_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 worldToObjDir2240_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult2093_g2334 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1819_g2334 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult2158_g2334 * input.positionOS.xyz.y * TrunkHeightMask2118_g2334 ).xyz, 0.0 ) ).xyz;
				float mulTime1748_g2334 = _TimeParameters.x * 4.0;
				float dotResult1641_g2334 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1667_g2334 = dotResult1641_g2334;
				float mulTime1749_g2334 = _TimeParameters.x * 5.2;
				float dotResult1642_g2334 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1668_g2334 = dotResult1642_g2334;
				float3 rotatedValue2089_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1666_g2334 * 0.02 ) + mulTime1748_g2334 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1667_g2334 ) ) ) + sin( ( mulTime1749_g2334 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1668_g2334 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 ).x );
				float3 worldToObjDir2238_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue2089_g2334 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1609_g2334 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2815_g2334 = ( ( ( worldToObjDir2240_g2334 * ase_objectScale * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir2238_g2334 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1496_g2334 = ( float4( PIWI_02Gentle2481_g2334 , 0.0 ) + GentleNoise2639_g2334 + MotionBendingGentleRandom2426_g2334 + float4( MotionBendingGentleWind2427_g2334 , 0.0 ) + MotionBendingGentleWind22811_g2334 + float4( PIWI_01Gentle2815_g2334 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output163_g2334 = ( GlobalVar_WindStrength2401_g2334 * staticSwitch1496_g2334 );
				
				float3 ase_viewVectorWS = ( ( unity_OrthoParams.w == 0 ) ? _WorldSpaceCameraPos - ase_positionWS : UNITY_MATRIX_V[ 2 ].xyz );
				float3 ase_viewDirWS = normalize( ase_viewVectorWS );
				float3 ase_normalWS = TransformObjectToWorldNormal( input.normalOS );
				float3 normalizedWorldNormal = normalize( ase_normalWS );
				float dotResult494_g2338 = dot( ase_viewDirWS , normalizedWorldNormal );
				float2 uv_NormalMap789_g2338 = input.ase_texcoord.xy;
				float3 unpack490_g2338 = UnpackNormalScale( -tex2Dlod( _NormalMap, float4( uv_NormalMap789_g2338, 0, 0.0) ), -1.0 );
				unpack490_g2338.z = lerp( 1, unpack490_g2338.z, saturate(-1.0) );
				float3 ifLocalVar497_g2338 = 0;
				if( dotResult494_g2338 > 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 == 0.0 )
				ifLocalVar497_g2338 = unpack490_g2338;
				else if( dotResult494_g2338 < 0.0 )
				ifLocalVar497_g2338 = -input.normalOS;
				float4 transform500_g2338 = mul(GetObjectToWorldMatrix(),float4( ifLocalVar497_g2338 , 0.0 ));
				float dotResult504_g2338 = dot( float4( SafeNormalize( _MainLightPosition.xyz ) , 0.0 ) , transform500_g2338 );
				float3 ifLocalVar511_g2338 = 0;
				if( dotResult504_g2338 >= 0.0 )
				ifLocalVar511_g2338 = ifLocalVar497_g2338;
				else
				ifLocalVar511_g2338 = -ifLocalVar497_g2338;
				float3 break514_g2338 = ifLocalVar511_g2338;
				float3 temp_cast_35 = (dotResult504_g2338).xxx;
				float4 appendResult525_g2338 = (float4(break514_g2338.x , ( break514_g2338.y + saturate( ( 1.0 - ( ( distance( float3( 0,0,0 ) , temp_cast_35 ) - 0.2 ) / max( 0.2, 1E-05 ) ) ) ) ) , break514_g2338.z , 0.0));
				float4 LightDetectBackface595_g2338 = appendResult525_g2338;
				#ifdef _MOBILESHADINGWORLDUP_ON
				float4 staticSwitch622_g2338 = float4( float3( 0, 1, 0 ) , 0.0 );
				#else
				float4 staticSwitch622_g2338 = (( _LightDetectBackface )?( LightDetectBackface595_g2338 ):( float4( input.normalOS , 0.0 ) ));
				#endif
				float4 LightDetect_Output597_g2338 = staticSwitch622_g2338;
				
				output.ase_texcoord1.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord1.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output163_g2334.rgb;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					input.positionOS.xyz = vertexValue;
				#else
					input.positionOS.xyz += vertexValue;
				#endif

				input.normalOS = LightDetect_Output597_g2338.xyz;

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
				float4 ase_color : COLOR;
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
				output.ase_color = input.ase_color;
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
				output.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
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

				float2 uv_AlbedoMap555_g2338 = input.ase_texcoord1.xy;
				float Opacity_Output559_g2338 = tex2D( _AlbedoMap, uv_AlbedoMap555_g2338 ).a;
				

				surfaceDescription.Alpha = Opacity_Output559_g2338;
				#if defined( _ALPHATEST_ON )
					surfaceDescription.AlphaClipThreshold = _AlphaClipping;
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
			#define _SPECULAR_SETUP 1
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
			#define ASE_NEEDS_VERT_TANGENT
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
				float3 positionOld : TEXCOORD4;
				#if _ADD_PRECOMPUTED_VELOCITY
					float3 alembicMotionVector : TEXCOORD5;
				#endif
				half3 normalOS : NORMAL;
				half4 tangentOS : TANGENT;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryings
			{
				float4 positionCS : SV_POSITION;
				float4 positionCSNoJitter : TEXCOORD0;
				float4 previousPositionCSNoJitter : TEXCOORD1;
				float3 positionWS : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _EmColor;
			float4 _DryLeafColor;
			float4 _AlbedoColor;
			float _BackfaceCulling;
			float _TBCVMapOffset;
			float _ColorVariation;
			float _BranchMaskR;
			float _VertexAo;
			float _VertexLighting;
			float _VertexShadow;
			float _TEXTUREMAPS;
			float _TEXTURESETTINGS;
			float _SEASONSETTINGS;
			float _SHADINGSETTINGS;
			float _NormalBackFaceFixBranch;
			float _NormalIntenisty;
			float _SpecularBackfaceOcclusion;
			float _SpecularPower;
			float _SpecularMapScale;
			float _SpecularMapOffset;
			float _SpecularBias;
			float _SpecularScale;
			float _SpecularStrength;
			float _SmoothnessIntensity;
			float _AmbientOcclusionIntensity;
			float _TTFETREEFOLIAGESHADERMOBILE;
			float _FACERENDERING;
			float _ADVANCEDSETTINGS;
			float _TBCVMapIntenisty;
			float _ZaWorldoScale;
			float _DryLeavesOffset;
			float _DryLeavesScale;
			float _CenterofMass;
			float _MaskRootsAuto;
			float _Radius;
			float _Hardness;
			float _MaskRoots;
			float _RootsPosition;
			float _RootsRadius;
			float _RootsHardness;
			float _BranchMaskScale;
			float _BranchMaskRadious;
			float _BranchSwayPower;
			float _BranchWindLarge;
			float _BranchWindSmall;
			float _SwitchVGreenToRGBA;
			float _VertexColorPower;
			float _GrassSwayPowerGentleWind;
			float _DownwardStrength;
			float _MotionBendingGentleRandom;
			float _TrunkHeightandThickness;
			float _PivotRandomnessStrength;
			float _PivotRandomness;
			float _TEXTUREMAPS1;
			float _WINDMASKSETTINGS1;
			float _LightDetectBackface;
			float _SelfShadingVertexColor;
			float _SeasonVertexColorR;
			float _RandomColorScale;
			float _EmissionIntensity;
			float _AlphaClipping;
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
			
			inline float noise_randomValue (float2 uv) { return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453); }
			inline float noise_interpolate (float a, float b, float t) { return (1.0-t)*a + (t*b); }
			inline float valueNoise (float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac( uv );
				f = f* f * (3.0 - 2.0 * f);
				uv = abs( frac(uv) - 0.5);
				float2 c0 = i + float2( 0.0, 0.0 );
				float2 c1 = i + float2( 1.0, 0.0 );
				float2 c2 = i + float2( 0.0, 1.0 );
				float2 c3 = i + float2( 1.0, 1.0 );
				float r0 = noise_randomValue( c0 );
				float r1 = noise_randomValue( c1 );
				float r2 = noise_randomValue( c2 );
				float r3 = noise_randomValue( c3 );
				float bottomOfGrid = noise_interpolate( r0, r1, f.x );
				float topOfGrid = noise_interpolate( r2, r3, f.x );
				float t = noise_interpolate( bottomOfGrid, topOfGrid, f.y );
				return t;
			}
			
			float SimpleNoise(float2 UV)
			{
				float t = 0.0;
				float freq = pow( 2.0, float( 0 ) );
				float amp = pow( 0.5, float( 3 - 0 ) );
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(1));
				amp = pow(0.5, float(3-1));
				t += valueNoise( UV/freq )*amp;
				freq = pow(2.0, float(2));
				amp = pow(0.5, float(3-2));
				t += valueNoise( UV/freq )*amp;
				return t;
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

				float GlobalVar_WindStrength2401_g2334 = _GlobalWindStrength;
				float mulTime1797_g2334 = _TimeParameters.x * 5.2;
				float3 ase_objectPosition = GetAbsolutePositionWS( UNITY_MATRIX_M._m03_m13_m23 );
				float dotResult1669_g2334 = dot( ase_objectPosition , float3( 69.42, 37.86, 82.03 ) );
				float RandomSeedVector_E1703_g2334 = dotResult1669_g2334;
				float dotResult1671_g2334 = dot( ase_objectPosition , float3( 44.18, 51.13, 22.05 ) );
				float RandomSeedVector_D1704_g2334 = dotResult1671_g2334;
				float mulTime1796_g2334 = _TimeParameters.x * 4.3;
				float dotResult1670_g2334 = dot( ase_objectPosition , float3( 93.44, 53.12, 48.9 ) );
				float RandomSeedVector_F1705_g2334 = dotResult1670_g2334;
				float3 normalizeResult1764_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMassTrunkUP1996_g2334 = saturate( (distance( normalizeResult1764_g2334 , float3( 0, 1, 0 ) )*1.0 + -0.05) );
				float3 temp_output_1635_0_g2334 = ( ( input.positionOS.xyz - float3( 0, -1, 0 ) ) / _Radius );
				float dotResult1653_g2334 = dot( temp_output_1635_0_g2334 , temp_output_1635_0_g2334 );
				float saferPower1713_g2334 = abs( saturate( dotResult1653_g2334 ) );
				float temp_output_1761_0_g2334 = ( (( _MaskRootsAuto )?( saturate( input.positionOS.xyz.y ) ):( 1.0 )) * pow( saferPower1713_g2334 , _Hardness ) );
				float3 normalizeResult1623_g2334 = ASESafeNormalize( input.positionOS.xyz );
				float CenterOfMass1717_g2334 = saturate( (distance( normalizeResult1623_g2334 , float3( 0, 1, 0 ) )*2.0 + 0.0) );
				float SphericalMaskProxySphere1924_g2334 = (( _CenterofMass )?( ( temp_output_1761_0_g2334 * CenterOfMass1717_g2334 ) ):( temp_output_1761_0_g2334 ));
				float saferPower2686_g2334 = abs( saturate( ( -100.0 / input.positionOS.xyz.y ) ) );
				float MaskRoots2691_g2334 = (( _MaskRoots )?( saturate( ( 1.0 - pow( saferPower2686_g2334 , 0.1 ) ) ) ):( 1.0 ));
				float3 break2782_g2334 = float3( 0, -1, 0 );
				float3 appendResult2785_g2334 = (float3(break2782_g2334.x , ( break2782_g2334.y * _RootsPosition ) , break2782_g2334.z));
				float3 temp_output_2789_0_g2334 = ( ( input.positionOS.xyz - appendResult2785_g2334 ) / _RootsRadius );
				float dotResult2790_g2334 = dot( temp_output_2789_0_g2334 , temp_output_2789_0_g2334 );
				float saferPower2793_g2334 = abs( saturate( dotResult2790_g2334 ) );
				float RootsMask_ProxySphere2794_g2334 = pow( saferPower2793_g2334 , _RootsHardness );
				float smoothstepResult1959_g2334 = smoothstep( _BranchMaskScale , _BranchMaskRadious , saturate(  (0.0 + ( length( (input.positionOS).xzw ) - 0.0 ) * ( 1.0 - 0.0 ) / ( 10.0 - 0.0 ) ) ));
				float BranchMask2026_g2334 = smoothstepResult1959_g2334;
				float3 rotatedValue2147_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 1.2, 1, 0.8 ) ), ( ( ( cos( ( mulTime1797_g2334 + ( float3( 0.3, 0.55, 0.25 ) * 0.3 * RandomSeedVector_E1703_g2334 ) + ( RandomSeedVector_D1704_g2334 * 0.02 ) ) ) + sin( ( mulTime1796_g2334 + ( float3( 0.8, 0.33, 0.6 ) * 0.6 * RandomSeedVector_F1705_g2334 ) + ( input.positionOS.xyz * 1 ) ) ) ) * 0.1 ) * CenterOfMassTrunkUP1996_g2334 * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * BranchMask2026_g2334 ).x );
				float3 appendResult2083_g2334 = (float3(0.0 , 0.0 , saturate( input.positionOS.xyz ).z));
				float3 break1775_g2334 = input.positionOS.xyz;
				float3 appendResult1885_g2334 = (float3(break1775_g2334.x , ( break1775_g2334.y * 0.15 ) , 0.0));
				float mulTime1956_g2334 = _TimeParameters.x * 2.1;
				float dotResult1831_g2334 = dot( ase_objectPosition , float3( 75.09, 24.54, 63.12 ) );
				float RandomSeedVector_K1889_g2334 = dotResult1831_g2334;
				float dotResult1836_g2334 = dot( ase_objectPosition , float3( 45.3, 35.05, 58.9 ) );
				float RandomSeedVector_O1892_g2334 = dotResult1836_g2334;
				float3 temp_cast_1 = (input.positionOS.xyz.y).xxx;
				float2 appendResult1604_g2334 = (float2(input.positionOS.xyz.x , input.positionOS.xyz.z));
				float3 temp_cast_3 = (input.positionOS.xyz.x).xxx;
				float3 smoothstepResult1652_g2334 = smoothstep( float3( 0,0,0 ) , float3( 1,1,1 ) , saturate( ( cross( temp_cast_1 , float3( appendResult1604_g2334 ,  0.0 ) ) + cross( temp_cast_3 , float3( appendResult1604_g2334 ,  0.0 ) ) ) ));
				float3 RandomIDBranchPositionMask1760_g2334 = saturate( ( smoothstepResult1652_g2334 * 0.5 ) );
				float3 appendResult2078_g2334 = (float3(0.0 , input.positionOS.xyz.y , 0.0));
				float3 break1772_g2334 = input.positionOS.xyz;
				float3 appendResult1880_g2334 = (float3(break1772_g2334.x , 0.0 , ( break1772_g2334.z * 0.15 )));
				float mulTime1949_g2334 = _TimeParameters.x * 2.3;
				float dotResult1754_g2334 = dot( ase_objectPosition , float3( 12.34, 56.78, 90.12 ) );
				float RandomSeedVector_A1810_g2334 = dotResult1754_g2334;
				float dotResult1640_g2334 = dot( ase_objectPosition , float3( 39.4, 97.33, 55.12 ) );
				float RandomSeedVector_G1666_g2334 = dotResult1640_g2334;
				float3 appendResult2019_g2334 = (float3(input.positionOS.xyz.x , 0.0 , 0.0));
				float3 break1728_g2334 = input.positionOS.xyz;
				float3 appendResult1828_g2334 = (float3(0.0 , ( break1728_g2334.y * 0.2 ) , ( break1728_g2334.z * 0.4 )));
				float mulTime1882_g2334 = _TimeParameters.x * 2.0;
				float dotResult1832_g2334 = dot( ase_objectPosition , float3( 63.47, 32.7, 12.05 ) );
				float RandomSeedVector_J1888_g2334 = dotResult1832_g2334;
				float dotResult1835_g2334 = dot( ase_objectPosition , float3( 35.35, 68.4, 30.24 ) );
				float RandomSeedVector_N1891_g2334 = dotResult1835_g2334;
				float3 ase_positionWS = TransformObjectToWorld( ( input.positionOS ).xyz );
				float3 normalizeResult2066_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime2067_g2334 = _TimeParameters.x * 0.25;
				float simplePerlin2D3191_g2334 = snoise( ( normalizeResult2066_g2334 + mulTime2067_g2334 ).xy*0.43 );
				float WindMask_LargeB2270_g2334 = ( simplePerlin2D3191_g2334 * 1.5 );
				float mulTime1940_g2334 = _TimeParameters.x * 3.2;
				float3 temp_output_2070_0_g2334 = ( ( mulTime1940_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_K1889_g2334 ) + float3( 0.4, 0.3, 0.1 ) + ( input.positionOS.xyz.x * 0.02 ) + ( 0.14 * input.positionOS.xyz.y ) + ( input.positionOS.xyz.z * 0.16 ) );
				float dotResult1893_g2334 = dot( (input.positionOS.xyz*0.02 + 0.0) , input.positionOS.xyz );
				float CeneterOfMassThickness_Mask2025_g2334 = saturate( dotResult1893_g2334 );
				float mulTime1937_g2334 = _TimeParameters.x * 2.3;
				float dotResult1833_g2334 = dot( ase_objectPosition , float3( 51.59, 33.79, 38.54 ) );
				float RandomSeedVector_L1890_g2334 = dotResult1833_g2334;
				float dotResult1834_g2334 = dot( ase_objectPosition , float3( 14.19, 63.9, 24.6 ) );
				float RandomSeedVector_M1887_g2334 = dotResult1834_g2334;
				float3 temp_output_2073_0_g2334 = ( ( mulTime1937_g2334 + RandomSeedVector_L1890_g2334 + RandomSeedVector_M1887_g2334 ) + ( 0.2 * input.positionOS.xyz ) + float3( 0.4, 0.3, 0.1 ) );
				float mulTime1934_g2334 = _TimeParameters.x * 3.6;
				float temp_output_2076_0_g2334 = ( ( mulTime1934_g2334 + RandomSeedVector_N1891_g2334 + RandomSeedVector_O1892_g2334 ) + ( 0.2 * input.positionOS.xyz.x ) );
				float3 normalizeResult1817_g2334 = ASESafeNormalize( ase_positionWS );
				float mulTime1818_g2334 = _TimeParameters.x * 0.26;
				float simplePerlin2D1923_g2334 = snoise( ( normalizeResult1817_g2334 + mulTime1818_g2334 ).xy*0.7 );
				float WindMask_LargeC2062_g2334 = ( simplePerlin2D1923_g2334 * 1.5 );
				float3 PIWI_02Gentle2481_g2334 = ( ( ( ( ( rotatedValue2147_g2334 - input.positionOS.xyz ) * _BranchSwayPower ) * 0.2 ) + ( ( ( ( ( ( appendResult2083_g2334 + ( appendResult1885_g2334 * cos( mulTime1956_g2334 ) ) + ( cross( float3( 1.2, 0.6, 1 ) , ( float3( 0.7, 1, 0.8 ) * appendResult1885_g2334 ) ) * sin( ( mulTime1956_g2334 + RandomSeedVector_K1889_g2334 + RandomSeedVector_O1892_g2334 ) ) ) ) * 0.1 ) * BranchMask2026_g2334 * RandomIDBranchPositionMask1760_g2334 ) + ( ( ( appendResult2078_g2334 + ( appendResult1880_g2334 * cos( ( mulTime1949_g2334 + RandomSeedVector_A1810_g2334 + RandomSeedVector_G1666_g2334 ) ) ) + ( cross( float3( 0.9, 1, 1.2 ) , ( float3( 1, 1, 1 ) * appendResult1880_g2334 ) ) * sin( ( mulTime1949_g2334 + RandomSeedVector_D1704_g2334 ) ) ) ) * BranchMask2026_g2334 ) * 0.1 ) + ( ( ( ( appendResult2019_g2334 + ( appendResult1828_g2334 * cos( mulTime1882_g2334 ) ) + ( cross( float3( 1.1, 1.3, 0.8 ) , ( float3( 1.4, 0.8, 1.1 ) * appendResult1828_g2334 ) ) * sin( ( mulTime1882_g2334 + RandomSeedVector_J1888_g2334 + RandomSeedVector_N1891_g2334 ) ) ) ) * 0.1 ) * RandomIDBranchPositionMask1760_g2334 * BranchMask2026_g2334 ) * 0.05 ) ) * 0.2 ) * _BranchWindLarge * WindMask_LargeB2270_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) + ( ( ( ( cos( temp_output_2070_0_g2334 ) * sin( temp_output_2070_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( cos( temp_output_2073_0_g2334 ) * sin( temp_output_2073_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) + ( ( sin( temp_output_2076_0_g2334 ) * cos( temp_output_2076_0_g2334 ) * ( BranchMask2026_g2334 * CeneterOfMassThickness_Mask2025_g2334 ) ) * 0.1 ) ) * _BranchWindSmall * WindMask_LargeC2062_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 ) ) * 0.4 );
				float3 ase_objectScale = float3( length( GetObjectToWorldMatrix()[ 0 ].xyz ), length( GetObjectToWorldMatrix()[ 1 ].xyz ), length( GetObjectToWorldMatrix()[ 2 ].xyz ) );
				float2 appendResult1711_g2334 = (float2(ase_positionWS.x , ase_positionWS.z));
				float2 BasicWorldPisitionXY_Out1759_g2334 = appendResult1711_g2334;
				float4 WindDirection1609_g2334 = _WindDirection;
				float GlobalVar_WindSpeed1633_g2334 = _StrongWindSpeed;
				float2 NoiseRotation_Output1710_g2334 = ( -(WindDirection1609_g2334).xz * _TimeParameters.x * GlobalVar_WindSpeed1633_g2334 );
				float2 WPRG2D1990_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + NoiseRotation_Output1710_g2334 );
				float simpleNoise2607_g2334 = SimpleNoise( WPRG2D1990_g2334 );
				float3 break2587_g2334 = sin( ( ( _TimeParameters.y * 10.0 ) + ( ase_positionWS * 10.0 ) + input.tangentOS.xyz ) );
				float3 appendResult2598_g2334 = (float3(break2587_g2334.x , ( break2587_g2334.y * 0.3 ) , break2587_g2334.z));
				float3 smoothstepResult2606_g2334 = smoothstep( float3( 0,0,0 ) , float3( 2,2,2 ) , appendResult2598_g2334);
				float smoothstepResult1925_g2334 = smoothstep( 0.0 , _VertexColorPower , input.ase_color.g);
				float4 temp_cast_9 = (saturate( smoothstepResult1925_g2334 )).xxxx;
				float4 LeafVertexColor_Main2117_g2334 = (( _SwitchVGreenToRGBA )?( input.ase_color ):( temp_cast_9 ));
				float3 worldToObjDir2632_g2334 = mul( GetWorldToObjectMatrix(), float4( ( (simpleNoise2607_g2334*2.2 + -1.05) * float4( ( smoothstepResult2606_g2334 * 0.2 ) , 0.0 ) * input.positionOS.xyz.y * sin( _TimeParameters.x * 0.125 ) * LeafVertexColor_Main2117_g2334 ).rgb, 0.0 ) ).xyz;
				float3 NormaliseRotationAxis2409_g2334 = float3( 0, 1, 0 );
				float simplePerlin2D2430_g2334 = snoise( WPRG2D1990_g2334*0.6 );
				simplePerlin2D2430_g2334 = simplePerlin2D2430_g2334*0.5 + 0.5;
				float NoiseLarge2431_g2334 = simplePerlin2D2430_g2334;
				float mulTime2580_g2334 = _TimeParameters.x * 2.0;
				float3 rotatedValue2609_g2334 = RotateAroundAxis( float3( 0,0,0 ), ( cos( ( ( ase_positionWS * 0.2 ) + mulTime2580_g2334 ) ) * _GrassSwayPowerGentleWind * saturate( input.positionOS.xyz.y ) ), NormaliseRotationAxis2409_g2334, NoiseLarge2431_g2334 );
				float3 worldToObjDir2623_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindDirection1609_g2334 * float4( rotatedValue2609_g2334 , 0.0 ) ).xyz, 0.0 ) ).xyz;
				float simplePerlin2D2557_g2334 = snoise( WPRG2D1990_g2334*0.1 );
				float MaskRotation2559_g2334 = saturate( simplePerlin2D2557_g2334 );
				float3 temp_output_2605_0_g2334 = ( input.positionOS.xyz * 0.25 );
				float3 rotatedValue2610_g2334 = RotateAroundAxis( float3( 0,0,0 ), temp_output_2605_0_g2334, NormaliseRotationAxis2409_g2334, ( saturate( input.positionOS.xyz.y ) * NoiseLarge2431_g2334 ) );
				float3 worldToObjDir2628_g2334 = mul( GetWorldToObjectMatrix(), float4( ( rotatedValue2610_g2334 - temp_output_2605_0_g2334 ), 0.0 ) ).xyz;
				float4 GentleNoise2639_g2334 = ( float4( ( ase_objectScale * worldToObjDir2632_g2334 ) , 0.0 ) + ( ( float4( worldToObjDir2623_g2334 , 0.0 ) * MaskRotation2559_g2334 * LeafVertexColor_Main2117_g2334 * float4( ase_objectScale , 0.0 ) ) * 0.4 ) + ( MaskRotation2559_g2334 * float4( worldToObjDir2628_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 ) );
				float4 normalizeResult2396_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 break2391_g2334 = (normalizeResult2396_g2334).xyz;
				float4 appendResult2387_g2334 = (float4(break2391_g2334.x , ( break2391_g2334.y + _DownwardStrength ) , break2391_g2334.z , 0.0));
				float4 WindMotion_BaseG2644_g2334 = ( appendResult2387_g2334 * saturate( input.positionOS.xyz.y ) );
				float2 WPRG2D_S41918_g2334 = ( BasicWorldPisitionXY_Out1759_g2334 + ( NoiseRotation_Output1710_g2334 * 4.0 ) );
				float simplePerlin2D2394_g2334 = snoise( WPRG2D_S41918_g2334*0.2 );
				simplePerlin2D2394_g2334 = simplePerlin2D2394_g2334*0.5 + 0.5;
				float WindMotionNoise2407_g2334 = simplePerlin2D2394_g2334;
				float saferPower1873_g2334 = abs( saturate( ( _TrunkHeightandThickness / input.positionOS.xyz.y ) ) );
				float smoothstepResult1999_g2334 = smoothstep( 0.2 , 0.8 , ( 1.0 - pow( saferPower1873_g2334 , 0.1 ) ));
				float TrunkHeightMask2118_g2334 = saturate( smoothstepResult1999_g2334 );
				float4 MotionBendingGentleRandom2426_g2334 = ( WindMotion_BaseG2644_g2334 * _MotionBendingGentleRandom * WindMotionNoise2407_g2334 * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float GlobalVar_WindMotion1991_g2334 = _WindMotion;
				float3 worldToObjDir2379_g2334 = mul( GetWorldToObjectMatrix(), float4( ( WindMotion_BaseG2644_g2334 *  (0.0 + ( GlobalVar_WindMotion1991_g2334 - 0.0 ) * ( 0.3 - 0.0 ) / ( 1.0 - 0.0 ) ) * WindMotionNoise2407_g2334 ).xyz, 0.0 ) ).xyz;
				float3 MotionBendingGentleWind2427_g2334 = ( worldToObjDir2379_g2334 * ase_objectScale * BranchMask2026_g2334 * MaskRoots2691_g2334 * SphericalMaskProxySphere1924_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 );
				float4 MotionBendingGentleWind22811_g2334 = ( float4( worldToObjDir2379_g2334 , 0.0 ) * float4( ase_objectScale , 0.0 ) * LeafVertexColor_Main2117_g2334 );
				float dotResult1755_g2334 = dot( ase_objectPosition , float3( 34.56, 78.9, 12.34 ) );
				float RandomSeedVector_B1811_g2334 = dotResult1755_g2334;
				float3 appendResult2093_g2334 = (float3(( sin( ( RandomSeedVector_A1810_g2334 + _TimeParameters.x ) ) * _PivotRandomnessStrength ) , 0.0 , ( sin( ( _TimeParameters.x + RandomSeedVector_B1811_g2334 ) ) * _PivotRandomnessStrength )));
				float dotResult1767_g2334 = dot( ase_objectPosition , float3( 78.9, 12.34, 56.78 ) );
				float RandomSeedVector_C1819_g2334 = dotResult1767_g2334;
				float4 normalizeResult2158_g2334 = ASESafeNormalize( WindDirection1609_g2334 );
				float3 worldToObjDir2240_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( appendResult2093_g2334 * ( sin( ( _TimeParameters.x + RandomSeedVector_C1819_g2334 ) ) * _PivotRandomness ) ) , 0.0 ) * normalizeResult2158_g2334 * input.positionOS.xyz.y * TrunkHeightMask2118_g2334 ).xyz, 0.0 ) ).xyz;
				float mulTime1748_g2334 = _TimeParameters.x * 4.0;
				float dotResult1641_g2334 = dot( ase_objectPosition , float3( 13.17, 65.8, 80.42 ) );
				float RandomSeedVector_H1667_g2334 = dotResult1641_g2334;
				float mulTime1749_g2334 = _TimeParameters.x * 5.2;
				float dotResult1642_g2334 = dot( ase_objectPosition , float3( 85.9, 12.56, 43.1 ) );
				float RandomSeedVector_I1668_g2334 = dotResult1642_g2334;
				float3 rotatedValue2089_g2334 = RotateAroundAxis( float3( 0,0,0 ), input.positionOS.xyz, normalize( float3( 0.6, 1, 0.1 ) ), ( ( ( cos( ( ( RandomSeedVector_G1666_g2334 * 0.02 ) + mulTime1748_g2334 + ( float3( 0.6, 1, 0.8 ) * 0.3 * RandomSeedVector_H1667_g2334 ) ) ) + sin( ( mulTime1749_g2334 + ( float3( 0.3, 0.4, 1 ) * RandomSeedVector_I1668_g2334 * 0.5 ) + ( input.positionOS.xyz * 0.2 ) ) ) ) * 0.1 ) * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * TrunkHeightMask2118_g2334 * RootsMask_ProxySphere2794_g2334 ).x );
				float3 worldToObjDir2238_g2334 = mul( GetWorldToObjectMatrix(), float4( ( float4( ( rotatedValue2089_g2334 - input.positionOS.xyz ) , 0.0 ) * 1.5 * WindDirection1609_g2334 * saturate( input.positionOS.xyz.y ) ).xyz, 0.0 ) ).xyz;
				float3 PIWI_01Gentle2815_g2334 = ( ( ( worldToObjDir2240_g2334 * ase_objectScale * SphericalMaskProxySphere1924_g2334 * MaskRoots2691_g2334 * RootsMask_ProxySphere2794_g2334 * saturate( input.positionOS.xyz.y ) ) + ( ( worldToObjDir2238_g2334 * ase_objectScale ) * 0.4 ) ) * 0.2 );
				#if defined( _WINDTYPE_GENTLEBREEZE )
				float4 staticSwitch1496_g2334 = ( float4( PIWI_02Gentle2481_g2334 , 0.0 ) + GentleNoise2639_g2334 + MotionBendingGentleRandom2426_g2334 + float4( MotionBendingGentleWind2427_g2334 , 0.0 ) + MotionBendingGentleWind22811_g2334 + float4( PIWI_01Gentle2815_g2334 , 0.0 ) + _TEXTUREMAPS1 + _WINDMASKSETTINGS1 );
				#elif defined( _WINDTYPE_WINDOFF )
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#else
				float4 staticSwitch1496_g2334 = float4( 0,0,0,0 );
				#endif
				float4 FinalWind_Output163_g2334 = ( GlobalVar_WindStrength2401_g2334 * staticSwitch1496_g2334 );
				
				output.ase_texcoord3.xy = input.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				output.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = input.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = FinalWind_Output163_g2334.rgb;

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

				float2 uv_AlbedoMap555_g2338 = input.ase_texcoord3.xy;
				float Opacity_Output559_g2338 = tex2D( _AlbedoMap, uv_AlbedoMap555_g2338 ).a;
				

				float Alpha = Opacity_Output559_g2338;
				#if defined( _ALPHATEST_ON )
					float AlphaClipThreshold = _AlphaClipping;
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
Node;AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;2878;720,-304;Inherit;False;548.4902;406.0598;;5;3018;2860;3016;2859;3015;DRAWERS;0,0,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3015;816,-176;Inherit;False;Property;_FACERENDERING;FACE RENDERING;1;0;Create;True;0;0;0;False;2;TTFE_DrawerFeatureBorder;Space (10);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;2859;752,-256;Inherit;False;Property;_TTFETREEFOLIAGESHADERMOBILE;(TTFE) TREE FOLIAGE SHADER (MOBILE);0;0;Create;True;0;0;0;False;1;TTFE_DrawerTitle;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3018;800,0;Inherit;False;Property;_ADVANCEDSETTINGS;ADVANCED SETTINGS;70;0;Create;True;0;0;0;False;2;TTFE_DrawerFeatureBorder;Space (10);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3016;816,-96;Inherit;False;Property;_BackfaceCulling;Backface Culling;2;1;[Enum];Create;True;0;3;Off;0;Front;1;Back;2;0;True;1;Space (10);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;2860;1104,-96;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3097;688,416;Inherit;False;(TTFE) Tree Foliage_Wind System (Mobile);45;;2334;66519fe4703a06e4796e4962d99c1984;0;0;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3110;896,512;Inherit;False;Property;_AlphaClipping;Alpha Clipping;44;0;Create;True;0;0;0;False;0;False;0.4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3112;720,144;Inherit;False;(TTFE) Tree Foliage_Shading (Mobile);3;;2338;b4356e3d3e5ee2e4db8b6a78d110d107;0;0;8;COLOR;152;FLOAT3;153;FLOAT;24;FLOAT;27;FLOAT;25;FLOAT;26;FLOAT4;28;COLOR;980
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3098;1338.528,120.0737;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;6;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3099;1338.528,120.0737;Float;False;True;-1;3;UnityEditor.ShaderGraphLitGUI;0;20;Toby Fredson/The Toby Foliage Engine/(TTFE) Tree Foliage (Mobile);94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;21;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=TransparentCutout=RenderType;Queue=AlphaTest=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;1;LightMode=UniversalForward;False;False;0;;0;0;Standard;51;Category;0;0;  Instanced Terrain Normals;1;0;Lighting Model;0;0;Workflow;0;639135744468698358;Surface;0;0;  Keep Alpha;0;0;  Refraction Model;0;0;  Blend;0;0;Two Sided;0;639135744572859758;Alpha Clipping;1;639135744640253618;  Use Shadow Threshold;0;0;Fragment Normal Space;0;0;Forward Only;0;0;Transmission;0;0;  Transmission Shadow;0.5,True,_ASETransmissionShadow;0;Translucency;0;0;  Translucency Strength;1,True,_ASETranslucencyStrength;0;  Normal Distortion;0.5,True,_ASETranslucencyNormalDistortion;0;  Scattering;2,True,_ASETranslucencyScattering;0;  Direct;0.9,True,_ASETranslucencyDirect;0;  Ambient;0.1,True,_ASETranslucencyAmbient;0;  Shadow;0.5,True,_ASETranslucencyShadow;0;Cast Shadows;1;0;Receive Shadows;2;0;Specular Highlights;2;0;Environment Reflections;2;0;Receive SSAO;1;0;Motion Vectors;1;0;  Add Precomputed Velocity;0;0;  XR Motion Vectors;0;0;GPU Instancing;1;0;LOD CrossFade;1;0;Built-in Fog;1;0;_FinalColorxAlpha;0;0;Meta Pass;1;0;Override Baked GI;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,True,_TessellationPhong;0;  Type;0;0;  Tess;16,True,_TessellationStrength;0;  Min;10,True,_TessellationDistanceMin;0;  Max;25,True,_TessellationDistanceMax;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Write Depth;0;0;  Early Z;0;0;Vertex Position;1;0;Debug Display;1;0;Clear Coat;0;0;0;12;False;True;True;True;True;True;True;True;True;True;True;False;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3100;1338.528,120.0737;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3101;1338.528,120.0737;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;True;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3102;1338.528,120.0737;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3103;1338.528,120.0737;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3104;1338.528,120.0737;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthNormals;0;6;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;False;True;1;LightMode=DepthNormals;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3105;1338.528,120.0737;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;GBuffer;0;7;GBuffer;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;True;1;1;False;;0;False;;1;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;1;LightMode=UniversalGBuffer;False;True;12;d3d11;gles;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;switch2;webgpu;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3106;1338.528,120.0737;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;SceneSelectionPass;0;8;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3107;1338.528,120.0737;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ScenePickingPass;0;9;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3108;1338.528,120.0737;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;MotionVectors;0;10;MotionVectors;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;False;False;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=MotionVectors;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;3109;1338.528,120.0737;Float;False;False;-1;3;UnityEditor.ShaderGraphLitGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;XRMotionVectors;0;11;XRMotionVectors;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Lit;True;5;True;14;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;True;1;False;;255;False;;1;False;;7;False;;3;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;False;True;1;LightMode=XRMotionVectors;False;False;0;;0;0;Standard;0;False;0
WireConnection;2860;0;2859;0
WireConnection;2860;1;3015;0
WireConnection;2860;2;3018;0
WireConnection;2860;3;3112;980
WireConnection;3099;0;3112;152
WireConnection;3099;1;3112;153
WireConnection;3099;9;3112;24
WireConnection;3099;4;3112;27
WireConnection;3099;5;3112;25
WireConnection;3099;2;2860;0
WireConnection;3099;6;3112;26
WireConnection;3099;7;3110;0
WireConnection;3099;8;3097;0
WireConnection;3099;10;3112;28
ASEEND*/
//CHKSM=A5C8A43BAB61181E3623B5F64E5F308C1CE3D146