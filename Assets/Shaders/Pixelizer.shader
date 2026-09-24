Shader "Custom/Pixelizer"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [Toggle] _UVRepeatEnabled("UV Repeat Enabled", Float) = 0
        _UVRepeat("UV Repeat", Vector) = (1, 1, 0, 0)

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
        _OutlineThickness("Outline Thickness", Range(0, 4)) = 1

        _ObjectID("Object ID", Range(1, 255)) = 1
    }

        SubShader
        {
            Tags
            {
                "RenderType" = "Opaque"
                "RenderPipeline" = "UniversalPipeline"
            }

            // =========================================================
            // MRT: COLOR + EYE DEPTH + NORMAL + OUTLINE DATA
            // Pixelizer geometry is drawn once for all four buffers.
            // Color alpha stores Object ID (0 = background).
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
                #pragma multi_compile_instancing

                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
                #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_SCREEN
                #pragma multi_compile_fragment _ _SHADOWS_SOFT
                #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
                #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

                TEXTURE2D(_BaseMap);
                SAMPLER(sampler_BaseMap);

                CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                float _UVRepeatEnabled;
                float4 _UVRepeat;
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
                float _ObjectID;
                CBUFFER_END

                float4 _AutoUVMin;
                float4 _AutoUVSize;

                float _PixelizerPixelSize;
                float4 _PixelizerScreenSize;
                float4 _PixelizerSubPixelOffset;
                float _PixelizerSnapEnabled;

                struct Attributes
                {
                    float4 positionOS : POSITION;
                    float3 normalOS : NORMAL;
                    float2 uv : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct Varyings
                {
                    float4 positionCS : SV_POSITION;
                    float3 normalWS : TEXCOORD0;
                    float2 uv : TEXCOORD1;
                    float4 shadowCoord : TEXCOORD2;
                    float3 positionWS : TEXCOORD3;
                    float3 positionVS : TEXCOORD4;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct MRTOutput
                {
                    half4 color : SV_Target0;
                    float4 depth : SV_Target1;
                    half4 normal : SV_Target2;
                    half4 outlineData : SV_Target3;
                };

                float4 ApplyPixelCameraCorrection(float4 positionCS)
                {
                    if (_PixelizerSnapEnabled < 0.5)
                        return positionCS;

                    float2 lowResolution = max(_PixelizerScreenSize.zw, float2(1.0, 1.0));
                    float2 ndcOffset = (_PixelizerSubPixelOffset.xy / lowResolution) * 2.0;
                    positionCS.xy += ndcOffset * positionCS.w;
                    return positionCS;
                }

                Varyings Vert(Attributes input)
                {
                    Varyings output;
                    UNITY_SETUP_INSTANCE_ID(input);
                    UNITY_TRANSFER_INSTANCE_ID(input, output);

                    VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
                    VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS);

                    output.positionCS = ApplyPixelCameraCorrection(positionInputs.positionCS);
                    output.normalWS = normalInputs.normalWS;
                    output.uv = input.uv;
                    output.shadowCoord = TransformWorldToShadowCoord(positionInputs.positionWS);
                    output.positionWS = positionInputs.positionWS;
                    output.positionVS = TransformWorldToView(positionInputs.positionWS);
                    return output;
                }

                float GetBayer4x4(int2 pixelPosition)
                {
                    int x = pixelPosition.x & 3;
                    int y = pixelPosition.y & 3;
                    int index = x + y * 4;
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

                float2 GetRepeatedAtlasUV(float2 rawUV)
                {
                    if (_UVRepeatEnabled < 0.5)
                        return rawUV * _BaseMap_ST.xy + _BaseMap_ST.zw;

                    float2 uvMin = _AutoUVMin.xy;
                    float2 uvSize = max(_AutoUVSize.xy, float2(0.000001, 0.000001));
                    float2 repeatCount = max(_UVRepeat.xy, float2(1.0, 1.0));

                    // Convert the UV rectangle already used by this mesh to local 0..1.
                    float2 localUV = (rawUV - uvMin) / uvSize;

                    // Repeat only that existing UV rectangle.
                    localUV = frac(localUV * repeatCount);

                    // Convert back to the same atlas rectangle.
                    float2 atlasUV = uvMin + localUV * uvSize;

                    // Preserve normal Base Map Tiling/Offset after the automatic repeat.
                    return atlasUV * _BaseMap_ST.xy + _BaseMap_ST.zw;
                }

                half3 ApplyColorQuantization(half3 color, float4 positionCS)
                {
                    float steps = max(2.0, _ColorSteps);

                    if (_DitheringEnabled > 0.5)
                    {
                        float pixelSize = max(1.0, _PixelizerPixelSize);
                        int2 pixelGridPosition = (int2)floor(positionCS.xy / pixelSize);
                        float bayer = GetBayer4x4(pixelGridPosition);
                        float centeredBayer = bayer - 0.5;
                        float stepSize = 1.0 / steps;
                        color += centeredBayer * stepSize * _DitherStrength;
                    }

                    color = saturate(color);
                    return floor(color * steps) / steps;
                }

                MRTOutput Frag(Varyings input)
                {
                    UNITY_SETUP_INSTANCE_ID(input);
                    MRTOutput output;

                    float2 sampleUV = GetRepeatedAtlasUV(input.uv);
                    half4 textureColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, sampleUV);
                    half3 baseColor = textureColor.rgb * _BaseColor.rgb;
                    float3 normalWS = normalize(input.normalWS);

                    Light mainLight = GetMainLight(input.shadowCoord);
                    float3 lightDirection = normalize(mainLight.direction);
                    float NdotL = saturate(dot(normalWS, lightDirection));

                    float shadeSteps = max(1.0, _ShadeSteps);
                    float steppedLight = floor(NdotL * shadeSteps) / shadeSteps;
                    float shadowSteps = max(1.0, _ShadowSteps);
                    float steppedShadow = floor(mainLight.shadowAttenuation * shadowSteps) / shadowSteps;
                    steppedShadow = lerp(1.0, steppedShadow, _ShadowStrength);
                    steppedLight *= steppedShadow;

                    float lighting = _AmbientLight + steppedLight * (1.0 - _AmbientLight);
                    half3 finalColor = baseColor * lighting * mainLight.color;

                    #if defined(_ADDITIONAL_LIGHTS)
                    uint additionalLightCount = GetAdditionalLightsCount();
                    for (uint lightIndex = 0u; lightIndex < additionalLightCount; ++lightIndex)
                    {
                        Light additionalLight = GetAdditionalLight(lightIndex, input.positionWS);
                        float additionalNdotL = saturate(dot(normalWS, normalize(additionalLight.direction)));
                        float additionalAttenuation = saturate(additionalLight.distanceAttenuation);
                        float additionalSteppedShadow = floor(additionalLight.shadowAttenuation * shadowSteps) / shadowSteps;
                        additionalSteppedShadow = lerp(1.0, additionalSteppedShadow, _ShadowStrength);

                        float additionalIntensity = additionalNdotL * additionalAttenuation;
                        float steppedAdditional = floor(additionalIntensity * shadeSteps) / shadeSteps;
                        steppedAdditional *= additionalSteppedShadow;
                        finalColor += baseColor * steppedAdditional * additionalLight.color;
                    }
                    #endif

                    if (_ColorQuantizationEnabled > 0.5)
                        finalColor = ApplyColorQuantization(finalColor, input.positionCS);

                    float encodedObjectID = round(clamp(_ObjectID, 1.0, 255.0)) / 255.0;
                    float eyeDepth = -input.positionVS.z;
                    float3 encodedNormal = normalWS * 0.5 + 0.5;
                    float thickness = clamp(_OutlineThickness, 0.0, 4.0);
                    float encodedThickness = (thickness / 4.0) * _OutlineEnabled;

                    // Alpha is intentionally Object ID, not source texture alpha.
                    // Pixelizer currently renders opaque objects only.
                    output.color = half4(finalColor, encodedObjectID);
                    output.depth = float4(eyeDepth, 0, 0, 1);
                    output.normal = half4(encodedNormal, 1);
                    output.outlineData = half4(_OutlineColor.rgb, encodedThickness);
                    return output;
                }

                ENDHLSL
            }

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
                                        #pragma multi_compile_instancing
                                        #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                                        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

                                        CBUFFER_START(UnityPerMaterial)
                                        float4 _BaseMap_ST;
                                        half4 _BaseColor;
                                        float _UVRepeatEnabled;
                                        float4 _UVRepeat;
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
                                        float _ObjectID;
                                        CBUFFER_END

                                        float3 _LightDirection;
                                        float3 _LightPosition;

                                        struct Attributes
                                        {
                                            float4 positionOS : POSITION;
                                            float3 normalOS : NORMAL;
                                            UNITY_VERTEX_INPUT_INSTANCE_ID
                                        };

                                        struct Varyings
                                        {
                                            float4 positionCS : SV_POSITION;
                                        };

                                        float4 GetShadowPositionHClip(Attributes input)
                                        {
                                            float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                                            float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                                            float3 lightDirectionWS = _LightDirection;

                                            #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                                                lightDirectionWS = normalize(_LightPosition - positionWS);
                                            #endif

                                            float4 positionCS = TransformWorldToHClip(
                                                ApplyShadowBias(positionWS, normalWS, lightDirectionWS)
                                            );

                                            #if UNITY_REVERSED_Z
                                                positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE * positionCS.w);
                                            #else
                                                positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE * positionCS.w);
                                            #endif

                                            return positionCS;
                                        }

                                        Varyings ShadowPassVertex(Attributes input)
                                        {
                                            UNITY_SETUP_INSTANCE_ID(input);
                                            Varyings output;
                                            output.positionCS = GetShadowPositionHClip(input);
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