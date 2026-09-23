Shader "Custom/PixelComposite"
{
    Properties
    {
        _PixelTexture ("Pixel Texture", 2D) = "black" {}
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
        }

        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "PixelComposite"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"



            TEXTURE2D(_PixelTexture);
            SAMPLER(sampler_PixelTexture);



            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;

                half4 sceneColor =
                    SAMPLE_TEXTURE2D(
                        _BlitTexture,
                        sampler_LinearClamp,
                        uv
                    );

                half4 pixelColor =
                    SAMPLE_TEXTURE2D(
                        _PixelTexture,
                        sampler_PixelTexture,
                        uv
                    );

                half3 finalColor =
                    lerp(
                        sceneColor.rgb,
                        pixelColor.rgb,
                        pixelColor.a
                    );

                return half4(
                    finalColor,
                    sceneColor.a
                );
            }

            ENDHLSL
        }
    }
