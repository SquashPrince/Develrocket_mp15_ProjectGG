Shader "Hidden/PixelComposite"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)

        _DepthThreshold("Depth Threshold", Float) = 0.1
        _NormalThreshold("Normal Threshold", Range(0, 1)) = 0.2

        _OutlineThickness("Outline Thickness", Range(1, 4)) = 1
    }


        SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
        }


        Pass
        {
            Name "PixelComposite"

            ZWrite Off
            ZTest Always
            Cull Off


            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag


            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"


        // =====================================================
        // Pixel Color
        // =====================================================

        TEXTURE2D(_PixelTexture);
        SAMPLER(sampler_PixelTexture);


        // =====================================================
        // Mask
        // =====================================================

        TEXTURE2D(_PixelMaskTexture);
        SAMPLER(sampler_PixelMaskTexture);


        // =====================================================
        // Depth
        // =====================================================

        TEXTURE2D(_PixelDepthTexture);
        SAMPLER(sampler_PixelDepthTexture);

        float4 _PixelDepthTexture_TexelSize;


        // =====================================================
        // Normal
        // =====================================================

        TEXTURE2D(_PixelNormalTexture);
        SAMPLER(sampler_PixelNormalTexture);


        // =====================================================
        // Material Properties
        // =====================================================

        CBUFFER_START(UnityPerMaterial)

        half4 _OutlineColor;

        float _DepthThreshold;
        float _NormalThreshold;

        float _OutlineThickness;

        CBUFFER_END


            // =====================================================
            // Mask
            // =====================================================

            float SamplePixelMask(float2 uv)
            {
                float value =
                    SAMPLE_TEXTURE2D(
                        _PixelMaskTexture,
                        sampler_PixelMaskTexture,
                        uv
                    ).r;

                return step(0.001, value);
            }


        // =====================================================
        // Depth
        // =====================================================

        float SamplePixelDepth(float2 uv)
        {
            return SAMPLE_TEXTURE2D(
                _PixelDepthTexture,
                sampler_PixelDepthTexture,
                uv
            ).r;
        }


        // =====================================================
        // Normal
        // =====================================================

        float3 DecodePixelNormal(float2 uv)
        {
            float3 encoded =
                SAMPLE_TEXTURE2D(
                    _PixelNormalTexture,
                    sampler_PixelNormalTexture,
                    uv
                ).rgb;

            return normalize(
                encoded * 2.0 - 1.0
            );
        }


        // =====================================================
        // Silhouette Edge
        //
        // center = Pixelizer
        // neighbor = Background
        // =====================================================

        float CheckSilhouetteEdge(
            float centerMask,
            float neighborMask
        )
        {
            return centerMask *
                (1.0 - neighborMask);
        }


        // =====================================================
        // Depth Edge
        //
        // 앞쪽 Pixel에만 선을 만든다.
        //
        // neighborDepth > centerDepth
        //
        // 이면 현재 Pixel이 더 앞쪽이다.
        // =====================================================

        float CheckDepthEdge(
            float centerMask,
            float neighborMask,
            float centerDepth,
            float neighborDepth
        )
        {
            float difference =
                neighborDepth -
                centerDepth;

            return
                step(
                    _DepthThreshold,
                    difference
                )
                *
                centerMask
                *
                neighborMask;
        }


        // =====================================================
        // Normal Edge
        // =====================================================

        float CheckNormalEdge(
            float centerMask,
            float neighborMask,
            float3 centerNormal,
            float3 neighborNormal
        )
        {
            float difference =
                1.0 -
                saturate(
                    dot(
                        centerNormal,
                        neighborNormal
                    )
                );

            return
                step(
                    _NormalThreshold,
                    difference
                )
                *
                centerMask
                *
                neighborMask;
        }


        // =====================================================
        // Check Neighbor
        //
        // 현재 픽셀과 특정 위치의 Neighbor를 비교한다.
        // =====================================================

        float CheckNeighbor(
            float2 neighborUV,
            float centerMask,
            float centerDepth,
            float3 centerNormal
        )
        {
            float neighborMask =
                SamplePixelMask(neighborUV);


            // ---------------------------------------------
            // Silhouette
            // ---------------------------------------------

            float silhouetteEdge =
                CheckSilhouetteEdge(
                    centerMask,
                    neighborMask
                );


            // ---------------------------------------------
            // Pixelizer가 아닌 경우에는
            // Depth/Normal을 비교할 필요가 없다.
            // ---------------------------------------------

            if (neighborMask < 0.5)
            {
                return silhouetteEdge;
            }


            // ---------------------------------------------
            // Depth
            // ---------------------------------------------

            float neighborDepth =
                SamplePixelDepth(
                    neighborUV
                );


            float depthEdge =
                CheckDepthEdge(
                    centerMask,
                    neighborMask,
                    centerDepth,
                    neighborDepth
                );


            // ---------------------------------------------
            // Normal
            // ---------------------------------------------

            float3 neighborNormal =
                DecodePixelNormal(
                    neighborUV
                );


            float normalEdge =
                CheckNormalEdge(
                    centerMask,
                    neighborMask,
                    centerNormal,
                    neighborNormal
                );


            return max(
                silhouetteEdge,
                max(
                    depthEdge,
                    normalEdge
                )
            );
        }


        // =====================================================
        // Fragment
        // =====================================================

        half4 Frag(Varyings input) : SV_Target
        {
            float2 uv =
                input.texcoord;


            float2 texelSize =
                _PixelDepthTexture_TexelSize.xy;


            // =================================================
            // Original Scene
            // =================================================

            half4 sceneColor =
                SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_LinearClamp,
                    uv
                );


            // =================================================
            // Pixel Color
            // =================================================

            half4 pixelColor =
                SAMPLE_TEXTURE2D(
                    _PixelTexture,
                    sampler_PixelTexture,
                    uv
                );


            // =================================================
            // Center Data
            // =================================================

            float centerMask =
                SamplePixelMask(uv);


            // Pixelizer가 아닌 곳은
            // Scene 그대로 출력
            if (centerMask < 0.5)
            {
                return sceneColor;
            }


            float centerDepth =
                SamplePixelDepth(uv);


            float3 centerNormal =
                DecodePixelNormal(uv);


            // =================================================
            // Outline
            // =================================================

            float outline = 0.0;


            // =================================================
            // Thickness 1
            // =================================================

            if (_OutlineThickness >= 1.0)
            {
                float distance = 1.0;


                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + float2(
                                -texelSize.x * distance,
                                0
                            ),
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );


                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + float2(
                                texelSize.x * distance,
                                0
                            ),
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );


                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + float2(
                                0,
                                texelSize.y * distance
                            ),
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );


                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + float2(
                                0,
                                -texelSize.y * distance
                            ),
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );
            }


            // =================================================
            // Thickness 2
            // =================================================

            if (_OutlineThickness >= 2.0)
            {
                float distance = 2.0;


                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + float2(
                                -texelSize.x * distance,
                                0
                            ),
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );


                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + float2(
                                texelSize.x * distance,
                                0
                            ),
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );


                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + float2(
                                0,
                                texelSize.y * distance
                            ),
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );


                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + float2(
                                0,
                                -texelSize.y * distance
                            ),
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );
            }


            // =================================================
            // Thickness 3
            // =================================================

            if (_OutlineThickness >= 3.0)
            {
                float distance = 3.0;


                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + float2(
                                -texelSize.x * distance,
                                0
                            ),
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );


                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + float2(
                                texelSize.x * distance,
                                0
                            ),
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );


                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + float2(
                                0,
                                texelSize.y * distance
                            ),
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );


                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + float2(
                                0,
                                -texelSize.y * distance
                            ),
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );
            }


            // =================================================
            // Thickness 4
            // =================================================

            if (_OutlineThickness >= 4.0)
            {
                float distance = 4.0;


                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + float2(
                                -texelSize.x * distance,
                                0
                            ),
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );


                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + float2(
                                texelSize.x * distance,
                                0
                            ),
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );


                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + float2(
                                0,
                                texelSize.y * distance
                            ),
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );


                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + float2(
                                0,
                                -texelSize.y * distance
                            ),
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );
            }


            // =================================================
            // Scene + Pixelizer
            // =================================================

            half4 result =
                lerp(
                    sceneColor,
                    pixelColor,
                    centerMask
                );


            // =================================================
            // Outline
            // =================================================

            result.rgb =
                lerp(
                    result.rgb,
                    _OutlineColor.rgb,
                    outline
                );


            return result;
        }

        ENDHLSL
    }
    }
}