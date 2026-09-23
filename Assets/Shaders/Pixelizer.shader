Shader "Custom/Pixelizer"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)

        _ShadeSteps("Shade Steps", Range(1, 8)) = 3
        _AmbientLight("Ambient Light", Range(0, 1)) = 0.2

        _ShadowSteps("Shadow Steps", Range(1, 8)) = 2
        _ShadowStrength("Shadow Strength", Range(0, 1)) = 1

        [HideInInspector] _Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
    }


        SubShader
        {
            Tags
            {
                "RenderPipeline" = "UniversalPipeline"
                "RenderType" = "Opaque"
                "Queue" = "Geometry"
            }


            // =========================================================
            // Pixel Color
            // =========================================================

            Pass
            {
                Name "Pixelizer"

                Tags
                {
                    "LightMode" = "Pixelizer"
                }

                ZWrite On
                ZTest LEqual
                Cull Back

                HLSLPROGRAM

                #pragma vertex VertPixel
                #pragma fragment FragPixel

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

                float _Cutoff;

                CBUFFER_END


                struct Attributes
                {
                    float4 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float2 uv : TEXCOORD0;
                };


                struct Varyings
                {
                    float4 positionCS : SV_POSITION;

                    float3 positionWS : TEXCOORD0;
                    float3 normalWS : TEXCOORD1;

                    float2 uv : TEXCOORD2;

                    float4 shadowCoord : TEXCOORD3;
                };


                Varyings VertPixel(Attributes input)
                {
                    Varyings output;

                    VertexPositionInputs positionInputs =
                        GetVertexPositionInputs(input.positionOS.xyz);

                    VertexNormalInputs normalInputs =
                        GetVertexNormalInputs(input.normalOS);


                    output.positionCS = positionInputs.positionCS;
                    output.positionWS = positionInputs.positionWS;

                    output.normalWS = normalInputs.normalWS;

                    output.uv =
                        TRANSFORM_TEX(input.uv, _BaseMap);


                    output.shadowCoord =
                        TransformWorldToShadowCoord(
                            positionInputs.positionWS
                        );


                    return output;
                }


                half4 FragPixel(Varyings input) : SV_Target
                {
                    // =================================================
                    // Base Color
                    // =================================================

                    half4 textureColor =
                        SAMPLE_TEXTURE2D(
                            _BaseMap,
                            sampler_BaseMap,
                            input.uv
                        );


                    half3 baseColor =
                        textureColor.rgb *
                        _BaseColor.rgb;


                    // =================================================
                    // Normal
                    // =================================================

                    float3 normalWS =
                        normalize(input.normalWS);


                    // =================================================
                    // Main Light
                    // =================================================

                    Light mainLight =
                        GetMainLight(input.shadowCoord);


                    float3 lightDirection =
                        normalize(mainLight.direction);


                    // =================================================
                    // Lambert
                    // =================================================

                    float NdotL =
                        saturate(
                            dot(
                                normalWS,
                                lightDirection
                            )
                        );


                    // =================================================
                    // Stepped Lighting
                    // =================================================

                    float shadeSteps =
                        max(1.0, _ShadeSteps);


                    float steppedLight =
                        floor(NdotL * shadeSteps) /
                        shadeSteps;


                    // =================================================
                    // Shadow Quantization
                    // =================================================

                    float shadow =
                        mainLight.shadowAttenuation;


                    float shadowSteps =
                        max(1.0, _ShadowSteps);


                    float steppedShadow =
                        floor(shadow * shadowSteps) /
                        shadowSteps;


                    steppedShadow =
                        lerp(
                            1.0,
                            steppedShadow,
                            _ShadowStrength
                        );


                    steppedLight *= steppedShadow;


                    // =================================================
                    // Ambient
                    // =================================================

                    float lighting =
                        _AmbientLight +
                        steppedLight *
                        (1.0 - _AmbientLight);


                    // =================================================
                    // Final
                    // =================================================

                    half3 finalColor =
                        baseColor *
                        lighting *
                        mainLight.color;


                    return half4(
                        finalColor,
                        textureColor.a * _BaseColor.a
                    );
                }

                ENDHLSL
            }


            // =========================================================
            // Pixel Mask
            // =========================================================

            Pass
            {
                Name "PixelizerMask"

                Tags
                {
                    "LightMode" = "PixelizerMask"
                }

                ZWrite On
                ZTest LEqual
                Cull Back

                HLSLPROGRAM

                #pragma vertex VertMask
                #pragma fragment FragMask

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


                struct Attributes
                {
                    float4 positionOS : POSITION;
                };


                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                };


                Varyings VertMask(Attributes input)
                {
                    Varyings output;

                    VertexPositionInputs positionInputs =
                        GetVertexPositionInputs(input.positionOS.xyz);

                    output.positionCS =
                        positionInputs.positionCS;

                    return output;
                }


                half4 FragMask(Varyings input) : SV_Target
                {
                    return half4(1, 1, 1, 1);
                }

                ENDHLSL
            }


                    // =========================================================
                    // Pixel Depth
                    // =========================================================

                    Pass
                    {
                        Name "PixelizerDepth"

                        Tags
                        {
                            "LightMode" = "PixelizerDepth"
                        }

                        ZWrite On
                        ZTest LEqual
                        Cull Back

                        HLSLPROGRAM

                        #pragma vertex VertDepth
                        #pragma fragment FragDepth

                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


                        struct Attributes
                        {
                            float4 positionOS : POSITION;
                        };


                        struct Varyings
                        {
                            float4 positionCS : SV_POSITION;
                            float3 positionVS : TEXCOORD0;
                        };


                        Varyings VertDepth(Attributes input)
                        {
                            Varyings output;

                            VertexPositionInputs positionInputs =
                                GetVertexPositionInputs(input.positionOS.xyz);

                            output.positionCS =
                                positionInputs.positionCS;

                            output.positionVS =
                                positionInputs.positionVS;

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
                    // Pixel Normal
                    // =========================================================

                    Pass
                    {
                        Name "PixelizerNormal"

                        Tags
                        {
                            "LightMode" = "PixelizerNormal"
                        }

                        ZWrite On
                        ZTest LEqual
                        Cull Back

                        HLSLPROGRAM

                        #pragma vertex VertNormal
                        #pragma fragment FragNormal

                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


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
                                positionInputs.positionCS;


                            output.normalWS =
                                normalInputs.normalWS;


                            return output;
                        }


                        half4 FragNormal(Varyings input) : SV_Target
                        {
                            float3 normalWS =
                                normalize(input.normalWS);


                        // -1 ~ 1
                        //
                        // ↓
                        //
                        // 0 ~ 1

                        float3 encodedNormal =
                            normalWS * 0.5 + 0.5;


                        return half4(
                            encodedNormal,
                            1
                        );
                    }

                    ENDHLSL
                }


                            // =========================================================
                            // Shadow Caster
                            // =========================================================

                            Pass
                            {
                                Name "ShadowCaster"

                                Tags
                                {
                                    "LightMode" = "ShadowCaster"
                                }

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
                                        GetShadowPositionHClip(input);

                                    return output;
                                }


                                half4 ShadowPassFragment(Varyings input) : SV_Target
                                {
                                    return 0;
                                }

                                ENDHLSL
                            }
        }

            FallBack Off
}