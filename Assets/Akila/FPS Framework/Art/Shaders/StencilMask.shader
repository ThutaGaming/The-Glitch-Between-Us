Shader "Custom/StencilMask"
{
        Properties
    {
        [IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0
        //[Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Float) = 8  // Default: Always
        //[Enum(UnityEngine.Rendering.StencilOp)] _StencilPass ("Stencil Pass Op", Float) = 2  // Default: Replace
    }

    SubShader
    {
        Tags 
        { 
            "Queue" = "Geometry-1" // Renders BEFORE ground
            "RenderType" = "Opaque" 
        }
        
        ColorMask 0
        ZWrite Off
        
        Stencil 
        {
            Ref [_StencilID]
            Comp Always
            Pass Replace
        }

        Pass {}
    }
}
