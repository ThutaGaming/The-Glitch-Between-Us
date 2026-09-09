Shader "Unlit/Lens"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // Base Texture
        _Color ("Base Color", Color) = (1,0,0,1) // Base color for the lens
        _Distance ("Focus Distance", Float) = 100 // Distance to focus
        _Scale ("Scale Factor", Float) = 1 // Scaling of the UVs
        _GlowIntensity ("Glow Intensity", Float) = 2 // Intensity of the glow effect
        _OutOfBoundsColor ("Out of Bounds Color", Color) = (0, 0, 0, 1) // Color for out-of-bounds areas
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            ZTest Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Distance;
            float _Scale;
            float4 _Color;
            float _GlowIntensity;
            float4 _OutOfBoundsColor; // Color for out-of-bounds areas

            v2f vert (appdata v)
            {
                v2f o;
                float3 lens_origin = UnityObjectToViewPos(float3(0,0,0));
                float3 p0 = UnityObjectToViewPos(float3(0,0, _Distance));
                float3 n = UnityObjectToViewPos(float3(0,0,1)) - lens_origin;
                float3 uDir = UnityObjectToViewPos(float3(1,0,0)) - lens_origin;
                float3 vDir = UnityObjectToViewPos(float3(0,1,0)) - lens_origin;
                float3 vert = UnityObjectToViewPos(v.vertex);

                float a = dot(p0, n) / dot(vert, n);
                float3 vert_prime = a * vert;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = float2(dot(vert_prime - p0, uDir), dot(vert_prime - p0, vDir));
                o.uv = o.uv / (_Scale * _Distance);
                o.uv += float2(0.5, 0.5);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Check if UV is out of bounds
                if (i.uv.x < 0 || i.uv.x > 1 || i.uv.y < 0 || i.uv.y > 1)
                {
                    // Return the out-of-bounds color
                    return _OutOfBoundsColor;
                }

                // Sample the texture
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Blend the base color with the texture
                fixed4 col = _Color * texColor;

                // Add glow effect
                col.rgb += _Color.rgb * texColor.a * _GlowIntensity;

                // Apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
// CREATED BY MEKAWEY
