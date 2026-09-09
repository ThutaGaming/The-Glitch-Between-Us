Shader "Custom/StencileGeometry"
{
    Properties
    {
        [MainColor] _Color("Color", Color) = (1,1,1,1)
        [MainTexture] _MainTex("Albedo (RGB)", 2D) = "white" {}
        [Normal] _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0,2)) = 1

        [Header(PBR Maps)]
        _MetallicMap ("Metallic/Specular", 2D) = "white" {}
        _Metallic ("Metallic Scale", Range(0,1)) = 0.0
        _RoughnessMap ("Roughness (R)", 2D) = "white" {}
        _Roughness ("Roughness Scale", Range(0,2)) = 0.5
        //_OcclusionMap ("Occlusion (R)", 2D) = "white" {}
        //_Occlusion ("Occlusion Scale", Range(0,2)) = 1
        _AlphaCutoff ("Alpha Cutoff", Range(0,1)) = 0.5


        [Header(Surface Options)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Source Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Destination Blend", Float) = 0
        [Enum(Off,0,On,1)] _ZWrite ("Depth Write", Float) = 1
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Culling", Float) = 2

        [Header(Blend Mode Presets)]
        [KeywordEnum(Alpha,Opaque,Additive,Multiply,Screen)] _BlendMode("Blend Preset", Float) = 0
        [IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0
        //[Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Float) = 8  // Default: Always
        //[Enum(UnityEngine.Rendering.StencilOp)] _StencilPass ("Stencil Pass Op", Float) = 2  // Default: Replace


    }

    HLSLINCLUDE
    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    // #pragma enable_d3d11_debug_symbols

    //enable GPU instancing support
    #pragma multi_compile_instancing
    #pragma multi_compile _ DOTS_INSTANCING_ON

    ENDHLSL

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        Stencil
        {
            Ref [_StencilID]
            Comp NotEqual
            Pass Keep
        }

        // Universal Render Pipeline Pass
        Pass
        {
            Name "ForwardLitURP"
            Tags { "LightMode" = "UniversalForward" }
            
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            Cull [_Cull]
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            TEXTURE2D(_MainTex);         SAMPLER(sampler_MainTex);
            TEXTURE2D(_NormalMap);       SAMPLER(sampler_NormalMap);
            TEXTURE2D(_MetallicMap);     SAMPLER(sampler_MetallicMap);
            TEXTURE2D(_RoughnessMap);    SAMPLER(sampler_RoughnessMap);

            float4 _MainTex_ST;
            float4 _NormalMap_ST;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 tangentWS : TEXCOORD3;
            };

            half4 _Color;
            half _NormalStrength;
            half _Metallic;
            half _Roughness;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.tangentWS = float4(TransformObjectToWorldDir(IN.tangentOS.xyz), IN.tangentOS.w);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Proper URP texture sampling
                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;
                                // Normal mapping
                float3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, IN.uv), _NormalStrength);
                                float3x3 tangentToWorld = float3x3(
                    IN.tangentWS.xyz,
                    cross(IN.normalWS, IN.tangentWS.xyz) * IN.tangentWS.w,
                    IN.normalWS
                );
                float3 normalWS = TransformTangentToWorld(normalTS, tangentToWorld);

                half metallic = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, IN.uv).r * _Metallic;
                half roughness = SAMPLE_TEXTURE2D(_RoughnessMap, sampler_RoughnessMap, IN.uv).r * _Roughness;
                InputData lightingInput = (InputData)0;
                lightingInput.positionWS = IN.positionWS;
                lightingInput.normalWS = normalize(normalWS);
                lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
                lightingInput.shadowCoord = TransformWorldToShadowCoord(IN.positionWS);

                SurfaceData surfaceData;
                surfaceData.albedo = albedo.rgb;
                surfaceData.alpha = albedo.a;
                surfaceData.metallic = metallic;
                surfaceData.specular = 0.0;
                surfaceData.smoothness = roughness;
                surfaceData.occlusion = 1.0;
                surfaceData.emission = 0;
                surfaceData.normalTS = normalTS;
                surfaceData.clearCoatMask = 0;
                surfaceData.clearCoatSmoothness = 0;

                return UniversalFragmentPBR(lightingInput, surfaceData);
            }
            ENDHLSL
        }
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]
        Cull [_Cull]
        Stencil
        {
            Ref [_StencilID]
            Comp NotEqual
            Pass Keep
        }
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
    #pragma target 4.5

        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _MetallicMap;
        sampler2D _RoughnessMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_RoughnessMap;
        };
        half _NormalStrength;
        half _Roughness;
        half _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed3 n = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex)).xyz;
            float roughness = tex2D (_RoughnessMap, IN.uv_RoughnessMap).r * _Roughness;
            n.xy *= _NormalStrength;  
            n.z = sqrt(1.0 - saturate(dot(n.xy, n.xy))); // Reconstruct Z
            o.Albedo = c.rgb;
            o.Normal = n;
            // Metallic and smoothness come from slider variables
            o.Metallic = tex2D (_MetallicMap, IN.uv_MainTex).r * _Metallic;
            //o.Smoothness =  _Roughness;
            o.Smoothness =  1.0 - roughness;
            o.Alpha = c.a;
        }
        ENDCG
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    CustomEditorForRenderPipeline "UnityEditor.Rendering.BuiltIn.ShaderGraph.BuiltInLitGUI" ""
    CustomEditorForRenderPipeline "Rendering.HighDefinition.LitShaderGraphGUI" "UnityEngine.Rendering.HighDefinition.HDRenderPipelineAsset"
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphLitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    FallBack "Standard"
}