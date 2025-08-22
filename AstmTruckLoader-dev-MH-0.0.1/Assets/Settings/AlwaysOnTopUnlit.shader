Shader "URP/AlwaysOnTopUnlit"
{
    Properties{
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Color", Color) = (1,0,0,0.65)
    }
    SubShader{
        Tags{ "RenderPipeline"="UniversalRenderPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Pass{
            Name "AlwaysOnTop"
            Tags{ "LightMode"="UniversalForward" }
            Cull Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            float4 _BaseColor;

            struct Attributes { float4 positionOS: POSITION; float2 uv: TEXCOORD0; };
            struct Varyings   { float4 positionHCS: SV_POSITION; float2 uv: TEXCOORD0; };

            Varyings vert(Attributes v){
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }
            half4 frag(Varyings i): SV_Target{
                half4 t = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                return t * _BaseColor;
            }
            ENDHLSL
        }
    }
    FallBack Off
}
