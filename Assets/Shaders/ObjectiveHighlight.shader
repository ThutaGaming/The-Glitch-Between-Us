Shader "Custom/ObjectiveHighlight"
{
    Properties
    {
        _Color ("Glow Color", Color) = (0.35, 0.55, 1, 1)
        _RimPower ("Rim Power", Range(0.1, 8)) = 2.5
        _Intensity ("Intensity", Range(0, 5)) = 1.2
    }

    SubShader
    {
        // Queue is pushed past normal transparents and ZTest is disabled so this renders
        // through walls/furniture - it is a UI-style "objective marker", not a lit surface.
        Tags { "RenderType"="Transparent" "Queue"="Transparent+100" "RenderPipeline"="UniversalPipeline" }
        ZWrite Off
        ZTest Always
        Cull Back
        Blend SrcAlpha One

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _RimPower;
                float _Intensity;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vpi = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vpi.positionCS;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceViewDir(vpi.positionWS);
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float3 n = normalize(IN.normalWS);
                float3 v = normalize(IN.viewDirWS);
                float rim = pow(1.0 - saturate(dot(n, v)), _RimPower);
                float alpha = saturate(rim * _Intensity);
                return float4(_Color.rgb * _Intensity, alpha);
            }
            ENDHLSL
        }
    }
}
