Shader "Custom/Pixelizer v1"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)

        _ShadeSteps("Shade Steps", Range(1, 8)) = 3
        _AmbientLight("Ambient Light", Range(0, 1)) = 0.2

        _ShadowSteps("Shadow Steps", Range(1, 8)) = 2
        _ShadowStrength("Shadow Strength", Range(0, 1)) = 1

        [Toggle] _ColorQuantizationEnabled("Color Quantization Enabled", Float) = 0
        _ColorSteps("Color Steps", Range(2, 32)) = 8

        [Toggle] _DitheringEnabled("Dithering Enabled", Float) = 0
        _DitherStrength("Dither Strength", Range(0, 1)) = 1

        [Toggle] _OutlineEnabled("Outline Enabled", Float) = 1
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness("Outline Thickness", Range(1, 4)) = 1
    }

        SubShader
        {
            Tags
            {
                "RenderType" = "Opaque"
                "RenderPipeline" = "UniversalPipeline"
            }

            // =========================================================
            // COLOR
            // =========================================================

            Pass
            {
                Name "Pixelizer"
                Tags { "LightMode" = "Pixelizer" }

                ZWrite On
                ZTest LEqual
                Cull Back

                HLSLPROGRAM

                #pragma vertex Vert
                #pragma fragment Frag

                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_SCREEN
                #pragma multi_compile_fragment _ _SHADOWS_SOFT

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

                TEXTURE2D(_BaseMap);
                SAMPLER(sampler_BaseMap);

                CBUFFER_START(UnityPerMaterial)

                float4 _BaseMap_ST;
                half4 _BaseColor;

                float _ShadeSteps;
                float _AmbientLight;

                float _ShadowSteps;
                float _ShadowStrength;

                float _ColorQuantizationEnabled;
                float _ColorSteps;

                float _DitheringEnabled;
                float _DitherStrength;

                float _OutlineEnabled;
                half4 _OutlineColor;
                float _OutlineThickness;

                CBUFFER_END

                float _PixelizerPixelSize;
                float4 _PixelizerScreenSize;
                float4 _PixelizerSubPixelOffset;
                float _PixelizerSnapEnabled;

                struct Attributes
                {
                    float4 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float2 uv : TEXCOORD0;
                };

                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 normalWS : TEXCOORD0;
                    float2 uv : TEXCOORD1;
                    float4 shadowCoord : TEXCOORD2;
                };

                float4 ApplyPixelCameraCorrection(float4 positionCS)
                {
                    if (_PixelizerSnapEnabled < 0.5)
                        return positionCS;

                    float2 lowResolution =
                        max(
                            _PixelizerScreenSize.zw,
                            float2(1.0, 1.0)
                        );

                    float2 ndcOffset =
                        (_PixelizerSubPixelOffset.xy /
                        lowResolution) *
                        2.0;

                    positionCS.xy +=
                        ndcOffset *
                        positionCS.w;

                    return positionCS;
                }

                Varyings Vert(Attributes input)
                {
                    Varyings output;

                    VertexPositionInputs positionInputs =
                        GetVertexPositionInputs(
                            input.positionOS.xyz
                        );

                    VertexNormalInputs normalInputs =
                        GetVertexNormalInputs(
                            input.normalOS
                        );

                    output.positionCS =
                        ApplyPixelCameraCorrection(
                            positionInputs.positionCS
                        );

                    output.normalWS =
                        normalInputs.normalWS;

                    output.uv =
                        TRANSFORM_TEX(
                            input.uv,
                            _BaseMap
                        );

                    output.shadowCoord =
                        TransformWorldToShadowCoord(
                            positionInputs.positionWS
                        );

                    return output;
                }

                // =====================================================
                // BAYER 4x4
                // =====================================================

                float GetBayer4x4(int2 pixelPosition)
                {
                    int x = pixelPosition.x & 3;
                    int y = pixelPosition.y & 3;

                    int index =
                        x +
                        y * 4;

                    float value = 0.0;

                    if (index == 0)       value = 0.0;
                    else if (index == 1)  value = 8.0;
                    else if (index == 2)  value = 2.0;
                    else if (index == 3)  value = 10.0;

                    else if (index == 4)  value = 12.0;
                    else if (index == 5)  value = 4.0;
                    else if (index == 6)  value = 14.0;
                    else if (index == 7)  value = 6.0;

                    else if (index == 8)  value = 3.0;
                    else if (index == 9)  value = 11.0;
                    else if (index == 10) value = 1.0;
                    else if (index == 11) value = 9.0;

                    else if (index == 12) value = 15.0;
                    else if (index == 13) value = 7.0;
                    else if (index == 14) value = 13.0;
                    else                  value = 5.0;

                    return (value + 0.5) / 16.0;
                }

                half3 ApplyColorQuantization(
                    half3 color,
                    float4 positionCS)
                {
                    float steps =
                        max(
                            2.0,
                            _ColorSteps
                        );

                    if (_DitheringEnabled > 0.5)
                    {
                        float pixelSize =
                            max(
                                1.0,
                                _PixelizerPixelSize
                            );

                        int2 pixelGridPosition =
                            (int2)floor(
                                positionCS.xy /
                                pixelSize
                            );

                        float bayer =
                            GetBayer4x4(
                                pixelGridPosition
                            );

                        float centeredBayer =
                            bayer -
                            0.5;

                        float stepSize =
                            1.0 /
                            steps;

                        color +=
                            centeredBayer *
                            stepSize *
                            _DitherStrength;
                    }

                    color =
                        saturate(
                            color
                        );

                    return
                        floor(
                            color *
                            steps
                        ) /
                        steps;
                }

                half4 Frag(Varyings input) : SV_Target
                {
                    half4 textureColor =
                        SAMPLE_TEXTURE2D(
                            _BaseMap,
                            sampler_BaseMap,
                            input.uv
                        );

                    half3 baseColor =
                        textureColor.rgb *
                        _BaseColor.rgb;

                    float3 normalWS =
                        normalize(
                            input.normalWS
                        );

                    Light mainLight =
                        GetMainLight(
                            input.shadowCoord
                        );

                    float3 lightDirection =
                        normalize(
                            mainLight.direction
                        );

                    float NdotL =
                        saturate(
                            dot(
                                normalWS,
                                lightDirection
                            )
                        );

                    // =================================================
                    // STEPPED LIGHT
                    // =================================================

                    float shadeSteps =
                        max(
                            1.0,
                            _ShadeSteps
                        );

                    float steppedLight =
                        floor(
                            NdotL *
                            shadeSteps
                        ) /
                        shadeSteps;

                    // =================================================
                    // STEPPED SHADOW
                    // =================================================

                    float shadow =
                        mainLight.shadowAttenuation;

                    float shadowSteps =
                        max(
                            1.0,
                            _ShadowSteps
                        );

                    float steppedShadow =
                        floor(
                            shadow *
                            shadowSteps
                        ) /
                        shadowSteps;

                    steppedShadow =
                        lerp(
                            1.0,
                            steppedShadow,
                            _ShadowStrength
                        );

                    steppedLight *=
                        steppedShadow;

                    float lighting =
                        _AmbientLight +
                        steppedLight *
                        (1.0 - _AmbientLight);

                    half3 finalColor =
                        baseColor *
                        lighting *
                        mainLight.color;

                    // =================================================
                    // COLOR QUANTIZATION + DITHER
                    // =================================================

                    if (_ColorQuantizationEnabled > 0.5)
                    {
                        finalColor =
                            ApplyColorQuantization(
                                finalColor,
                                input.positionCS
                            );
                    }

                    return half4(
                        finalColor,
                        textureColor.a *
                        _BaseColor.a
                    );
                }

                ENDHLSL
            }

            // =========================================================
            // MASK
            // =========================================================

            Pass
            {
                Name "PixelizerMask"
                Tags { "LightMode" = "PixelizerMask" }

                ZWrite On
                ZTest LEqual
                Cull Back

                HLSLPROGRAM

                #pragma vertex VertMask
                #pragma fragment FragMask

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                CBUFFER_START(UnityPerMaterial)

                float4 _BaseMap_ST;
                half4 _BaseColor;

                float _ShadeSteps;
                float _AmbientLight;

                float _ShadowSteps;
                float _ShadowStrength;

                float _ColorQuantizationEnabled;
                float _ColorSteps;

                float _DitheringEnabled;
                float _DitherStrength;

                float _OutlineEnabled;
                half4 _OutlineColor;
                float _OutlineThickness;

                CBUFFER_END

                float4 _PixelizerScreenSize;
                float4 _PixelizerSubPixelOffset;
                float _PixelizerSnapEnabled;

                struct Attributes
                {
                    float4 positionOS : POSITION;
                };

                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                };

                float4 ApplyPixelCameraCorrection(float4 positionCS)
                {
                    if (_PixelizerSnapEnabled < 0.5)
                        return positionCS;

                    float2 lowResolution =
                        max(
                            _PixelizerScreenSize.zw,
                            float2(1.0, 1.0)
                        );

                    float2 ndcOffset =
                        (_PixelizerSubPixelOffset.xy /
                        lowResolution) *
                        2.0;

                    positionCS.xy +=
                        ndcOffset *
                        positionCS.w;

                    return positionCS;
                }

                Varyings VertMask(Attributes input)
                {
                    Varyings output;

                    VertexPositionInputs positionInputs =
                        GetVertexPositionInputs(
                            input.positionOS.xyz
                        );

                    output.positionCS =
                        ApplyPixelCameraCorrection(
                            positionInputs.positionCS
                        );

                    return output;
                }

                half4 FragMask(Varyings input) : SV_Target
                {
                    return half4(
                        1,
                        1,
                        1,
                        1
                    );
                }

                ENDHLSL
            }

                    // =========================================================
                    // DEPTH
                    // =========================================================

                    Pass
                    {
                        Name "PixelizerDepth"
                        Tags { "LightMode" = "PixelizerDepth" }

                        ZWrite On
                        ZTest LEqual
                        Cull Back

                        HLSLPROGRAM

                        #pragma vertex VertDepth
                        #pragma fragment FragDepth

                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                        CBUFFER_START(UnityPerMaterial)

                        float4 _BaseMap_ST;
                        half4 _BaseColor;

                        float _ShadeSteps;
                        float _AmbientLight;

                        float _ShadowSteps;
                        float _ShadowStrength;

                        float _ColorQuantizationEnabled;
                        float _ColorSteps;

                        float _DitheringEnabled;
                        float _DitherStrength;

                        float _OutlineEnabled;
                        half4 _OutlineColor;
                        float _OutlineThickness;

                        CBUFFER_END

                        float4 _PixelizerScreenSize;
                        float4 _PixelizerSubPixelOffset;
                        float _PixelizerSnapEnabled;

                        struct Attributes
                        {
                            float4 positionOS : POSITION;
                        };

                        struct Varyings
                        {
                            float4 positionCS : SV_POSITION;
                            float3 positionVS : TEXCOORD0;
                        };

                        float4 ApplyPixelCameraCorrection(float4 positionCS)
                        {
                            if (_PixelizerSnapEnabled < 0.5)
                                return positionCS;

                            float2 lowResolution =
                                max(
                                    _PixelizerScreenSize.zw,
                                    float2(1.0, 1.0)
                                );

                            float2 ndcOffset =
                                (_PixelizerSubPixelOffset.xy /
                                lowResolution) *
                                2.0;

                            positionCS.xy +=
                                ndcOffset *
                                positionCS.w;

                            return positionCS;
                        }

                        Varyings VertDepth(Attributes input)
                        {
                            Varyings output;

                            VertexPositionInputs positionInputs =
                                GetVertexPositionInputs(
                                    input.positionOS.xyz
                                );

                            output.positionCS =
                                ApplyPixelCameraCorrection(
                                    positionInputs.positionCS
                                );

                            output.positionVS =
                                TransformWorldToView(
                                    positionInputs.positionWS
                                );

                            return output;
                        }

                        float4 FragDepth(Varyings input) : SV_Target
                        {
                            float eyeDepth =
                                -input.positionVS.z;

                            return float4(
                                eyeDepth,
                                0,
                                0,
                                1
                            );
                        }

                        ENDHLSL
                    }

                    // =========================================================
                    // NORMAL
                    // =========================================================

                    Pass
                    {
                        Name "PixelizerNormal"
                        Tags { "LightMode" = "PixelizerNormal" }

                        ZWrite On
                        ZTest LEqual
                        Cull Back

                        HLSLPROGRAM

                        #pragma vertex VertNormal
                        #pragma fragment FragNormal

                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                        CBUFFER_START(UnityPerMaterial)

                        float4 _BaseMap_ST;
                        half4 _BaseColor;

                        float _ShadeSteps;
                        float _AmbientLight;

                        float _ShadowSteps;
                        float _ShadowStrength;

                        float _ColorQuantizationEnabled;
                        float _ColorSteps;

                        float _DitheringEnabled;
                        float _DitherStrength;

                        float _OutlineEnabled;
                        half4 _OutlineColor;
                        float _OutlineThickness;

                        CBUFFER_END

                        float4 _PixelizerScreenSize;
                        float4 _PixelizerSubPixelOffset;
                        float _PixelizerSnapEnabled;

                        struct Attributes
                        {
                            float4 positionOS : POSITION;
                            float3 normalOS : NORMAL;
                        };

                        struct Varyings
                        {
                            float4 positionCS : SV_POSITION;
                            float3 normalWS : TEXCOORD0;
                        };

                        float4 ApplyPixelCameraCorrection(float4 positionCS)
                        {
                            if (_PixelizerSnapEnabled < 0.5)
                                return positionCS;

                            float2 lowResolution =
                                max(
                                    _PixelizerScreenSize.zw,
                                    float2(1.0, 1.0)
                                );

                            float2 ndcOffset =
                                (_PixelizerSubPixelOffset.xy /
                                lowResolution) *
                                2.0;

                            positionCS.xy +=
                                ndcOffset *
                                positionCS.w;

                            return positionCS;
                        }

                        Varyings VertNormal(Attributes input)
                        {
                            Varyings output;

                            VertexPositionInputs positionInputs =
                                GetVertexPositionInputs(
                                    input.positionOS.xyz
                                );

                            VertexNormalInputs normalInputs =
                                GetVertexNormalInputs(
                                    input.normalOS
                                );

                            output.positionCS =
                                ApplyPixelCameraCorrection(
                                    positionInputs.positionCS
                                );

                            output.normalWS =
                                normalInputs.normalWS;

                            return output;
                        }

                        half4 FragNormal(Varyings input) : SV_Target
                        {
                            float3 normalWS =
                                normalize(
                                    input.normalWS
                                );

                            float3 encodedNormal =
                                normalWS *
                                0.5 +
                                0.5;

                            return half4(
                                encodedNormal,
                                1
                            );
                        }

                        ENDHLSL
                    }

                            // =========================================================
                            // OUTLINE DATA
                            // =========================================================

                            Pass
                            {
                                Name "PixelizerOutlineData"
                                Tags { "LightMode" = "PixelizerOutlineData" }

                                ZWrite On
                                ZTest LEqual
                                Cull Back

                                HLSLPROGRAM

                                #pragma vertex VertOutlineData
                                #pragma fragment FragOutlineData

                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                                CBUFFER_START(UnityPerMaterial)

                                float4 _BaseMap_ST;
                                half4 _BaseColor;

                                float _ShadeSteps;
                                float _AmbientLight;

                                float _ShadowSteps;
                                float _ShadowStrength;

                                float _ColorQuantizationEnabled;
                                float _ColorSteps;

                                float _DitheringEnabled;
                                float _DitherStrength;

                                float _OutlineEnabled;
                                half4 _OutlineColor;
                                float _OutlineThickness;

                                CBUFFER_END

                                float4 _PixelizerScreenSize;
                                float4 _PixelizerSubPixelOffset;
                                float _PixelizerSnapEnabled;

                                struct Attributes
                                {
                                    float4 positionOS : POSITION;
                                };

                                struct Varyings
                                {
                                    float4 positionCS : SV_POSITION;
                                };

                                float4 ApplyPixelCameraCorrection(float4 positionCS)
                                {
                                    if (_PixelizerSnapEnabled < 0.5)
                                        return positionCS;

                                    float2 lowResolution =
                                        max(
                                            _PixelizerScreenSize.zw,
                                            float2(1.0, 1.0)
                                        );

                                    float2 ndcOffset =
                                        (_PixelizerSubPixelOffset.xy /
                                        lowResolution) *
                                        2.0;

                                    positionCS.xy +=
                                        ndcOffset *
                                        positionCS.w;

                                    return positionCS;
                                }

                                Varyings VertOutlineData(Attributes input)
                                {
                                    Varyings output;

                                    VertexPositionInputs positionInputs =
                                        GetVertexPositionInputs(
                                            input.positionOS.xyz
                                        );

                                    output.positionCS =
                                        ApplyPixelCameraCorrection(
                                            positionInputs.positionCS
                                        );

                                    return output;
                                }

                                half4 FragOutlineData(Varyings input) : SV_Target
                                {
                                    float thickness =
                                        clamp(
                                            _OutlineThickness,
                                            1.0,
                                            4.0
                                        );

                                    float encodedThickness =
                                        (thickness / 4.0) *
                                        _OutlineEnabled;

                                    return half4(
                                        _OutlineColor.rgb,
                                        encodedThickness
                                    );
                                }

                                ENDHLSL
                            }

                            // =========================================================
                            // SHADOW CASTER
                            // =========================================================

                            Pass
                            {
                                Name "ShadowCaster"
                                Tags { "LightMode" = "ShadowCaster" }

                                ZWrite On
                                ZTest LEqual
                                ColorMask 0
                                Cull Back

                                HLSLPROGRAM

                                #pragma vertex ShadowPassVertex
                                #pragma fragment ShadowPassFragment

                                #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

                                CBUFFER_START(UnityPerMaterial)

                                float4 _BaseMap_ST;
                                half4 _BaseColor;

                                float _ShadeSteps;
                                float _AmbientLight;

                                float _ShadowSteps;
                                float _ShadowStrength;

                                float _ColorQuantizationEnabled;
                                float _ColorSteps;

                                float _DitheringEnabled;
                                float _DitherStrength;

                                float _OutlineEnabled;
                                half4 _OutlineColor;
                                float _OutlineThickness;

                                CBUFFER_END

                                float3 _LightDirection;
                                float3 _LightPosition;

                                struct Attributes
                                {
                                    float4 positionOS : POSITION;
                                    float3 normalOS : NORMAL;
                                };

                                struct Varyings
                                {
                                    float4 positionCS : SV_POSITION;
                                };

                                float4 GetShadowPositionHClip(Attributes input)
                                {
                                    float3 positionWS =
                                        TransformObjectToWorld(
                                            input.positionOS.xyz
                                        );

                                    float3 normalWS =
                                        TransformObjectToWorldNormal(
                                            input.normalOS
                                        );

                                    float3 lightDirectionWS =
                                        _LightDirection;

                                    #if _CASTING_PUNCTUAL_LIGHT_SHADOW

                                        lightDirectionWS =
                                            normalize(
                                                _LightPosition -
                                                positionWS
                                            );

                                    #endif

                                    float4 positionCS =
                                        TransformWorldToHClip(
                                            ApplyShadowBias(
                                                positionWS,
                                                normalWS,
                                                lightDirectionWS
                                            )
                                        );

                                    #if UNITY_REVERSED_Z

                                        positionCS.z =
                                            min(
                                                positionCS.z,
                                                UNITY_NEAR_CLIP_VALUE *
                                                positionCS.w
                                            );

                                    #else

                                        positionCS.z =
                                            max(
                                                positionCS.z,
                                                UNITY_NEAR_CLIP_VALUE *
                                                positionCS.w
                                            );

                                    #endif

                                    return positionCS;
                                }

                                Varyings ShadowPassVertex(Attributes input)
                                {
                                    Varyings output;

                                    output.positionCS =
                                        GetShadowPositionHClip(
                                            input
                                        );

                                    return output;
                                }

                                half4 ShadowPassFragment(Varyings input) : SV_Target
                                {
                                    return 0;
                                }

                                ENDHLSL
                            }
        }
}