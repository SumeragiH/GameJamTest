Shader "Universal Render Pipeline/2D/alpha_outline_URP"
{
    Properties //导入的mesh type需要设置为full rect
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _OutlineWidth("OutlineWidth (Pixels)", Float) = 8.0

        [Header(State Control)]
        //方便预览，对应0,1,2,3
        [Enum(Normal,0, Hover,1, Valid,2, Invalid,3)] _State("Interaction State", Float) = 0

        [Header(Hover State)]//蓝色
        _HoverOutlineColor("Hover Outline", Color) = (0, 0.5, 1, 1) 

        [Header(Valid State)]//绿色
        _ValidOutlineColor("Valid Outline", Color) = (0, 1, 0, 1)
        _ValidFillColor("Valid Fill", Color) = (0, 1, 0, 0.5)

        [Header(Invalid State)]//红色
        _InvalidOutlineColor("Invalid Outline", Color) = (1, 0, 0, 1)
        _InvalidFillColor("Invalid Fill", Color) = (1, 0, 0, 0.5)
    }

    SubShader
    {
        Tags 
        { 
            "Queue" = "Transparent" 
            "IgnoreProjector" = "True" 
            "RenderType" = "Transparent" 
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 color        : COLOR; 
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _OutlineWidth;
                float _State;
                float4 _HoverOutlineColor;
                float4 _ValidOutlineColor; 
                float4 _ValidFillColor;
                float4 _InvalidOutlineColor; 
                float4 _InvalidFillColor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                //URP坐标转换
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color * _Color; 
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                //采样
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * IN.color;

                float4 targetOutlineColor = float4(0,0,0,0);
                float4 targetFillColor = float4(0,0,0,0);
                float enableOutline = 0;

                if (_State == 1)//Hover
                {
                    targetOutlineColor = _HoverOutlineColor;
                    enableOutline = 1;
                }
                else if (_State == 2)//Valid
                {
                    targetOutlineColor = _ValidOutlineColor;
                    targetFillColor = _ValidFillColor;
                    enableOutline = 1;
                }
                else if (_State == 3)//Invalid
                {
                    targetOutlineColor = _InvalidOutlineColor;
                    targetFillColor = _InvalidFillColor;
                    enableOutline = 1;
                }

                float3 filledRGB = lerp(texColor.rgb, targetFillColor.rgb, targetFillColor.a);

                float2 offset = _MainTex_TexelSize.xy * _OutlineWidth;
                
                //采样8个方向的Alpha
                float up    = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(0, offset.y)).a;
                float down  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(0, -offset.y)).a;
                float left  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(-offset.x, 0)).a;
                float right = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(offset.x, 0)).a;
                
                float upRight   = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(offset.x, offset.y)).a;
                float downRight = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(offset.x, -offset.y)).a;
                float upLeft    = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(-offset.x, offset.y)).a;
                float downLeft  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(-offset.x, -offset.y)).a;

                float neighborAlphaSum = saturate(up + down + left + right + upRight + downRight + upLeft + downLeft);
                float outlineMask = saturate(neighborAlphaSum - texColor.a) * enableOutline;

                half4 finalColor;
                finalColor.rgb = lerp(filledRGB, targetOutlineColor.rgb, outlineMask);
                finalColor.a = max(texColor.a, outlineMask);

                return finalColor;
            }
            ENDHLSL
        }
    }
}