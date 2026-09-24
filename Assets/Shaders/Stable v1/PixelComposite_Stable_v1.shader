Shader "Custom/PixelComposite v1"
{
    Properties
    {
        _DepthThreshold("Depth Threshold", Range(0.001, 2.0)) = 0.1
        _NormalThreshold("Normal Threshold", Range(0.001, 1.0)) = 0.2
    }

        SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
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

        // =====================================================
        // CAMERA COLOR
        // =====================================================

        TEXTURE2D_X(_BlitTexture);
        SAMPLER(sampler_BlitTexture);

        // =====================================================
        // PIXELIZER BUFFERS
        // =====================================================

        TEXTURE2D(_PixelTexture);
        SAMPLER(sampler_PixelTexture);

        TEXTURE2D(_PixelMaskTexture);
        SAMPLER(sampler_PixelMaskTexture);

        TEXTURE2D(_PixelDepthTexture);
        SAMPLER(sampler_PixelDepthTexture);

        TEXTURE2D(_PixelNormalTexture);
        SAMPLER(sampler_PixelNormalTexture);

        TEXTURE2D(_PixelOutlineDataTexture);
        SAMPLER(sampler_PixelOutlineDataTexture);

        float4 _PixelDepthTexture_TexelSize;

        float _DepthThreshold;
        float _NormalThreshold;

        struct Attributes
        {
            uint vertexID : SV_VertexID;
        };

        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float2 texcoord : TEXCOORD0;
        };

        Varyings Vert(Attributes input)
        {
            Varyings output;

            output.positionCS =
                GetFullScreenTriangleVertexPosition(
                    input.vertexID
                );

            output.texcoord =
                GetFullScreenTriangleTexCoord(
                    input.vertexID
                );

            return output;
        }

        // =====================================================
        // BUFFER ACCESS
        // =====================================================

        float GetMask(float2 uv)
        {
            float mask =
                SAMPLE_TEXTURE2D(
                    _PixelMaskTexture,
                    sampler_PixelMaskTexture,
                    uv
                ).r;

            return step(
                0.001,
                mask
            );
        }

        float GetDepth(float2 uv)
        {
            return SAMPLE_TEXTURE2D(
                _PixelDepthTexture,
                sampler_PixelDepthTexture,
                uv
            ).r;
        }

        float3 GetNormal(float2 uv)
        {
            float3 encodedNormal =
                SAMPLE_TEXTURE2D(
                    _PixelNormalTexture,
                    sampler_PixelNormalTexture,
                    uv
                ).rgb;

            return normalize(
                encodedNormal *
                2.0 -
                1.0
            );
        }

        half4 GetOutlineData(float2 uv)
        {
            return SAMPLE_TEXTURE2D(
                _PixelOutlineDataTexture,
                sampler_PixelOutlineDataTexture,
                uv
            );
        }

        half4 GetPixelColor(float2 uv)
        {
            return SAMPLE_TEXTURE2D(
                _PixelTexture,
                sampler_PixelTexture,
                uv
            );
        }

        // =====================================================
        // SILHOUETTE
        // =====================================================

        float CheckSilhouette(
            float centerMask,
            float neighborMask)
        {
            if (centerMask > 0.5 &&
                neighborMask < 0.5)
            {
                return 1.0;
            }

            return 0.0;
        }

        // =====================================================
        // DEPTH
        // =====================================================

        float CheckDepthEdge(
            float centerDepth,
            float neighborDepth)
        {
            float depthDifference =
                neighborDepth -
                centerDepth;

            return step(
                _DepthThreshold,
                depthDifference
            );
        }

        // =====================================================
        // NORMAL
        // =====================================================

        float CheckNormalEdge(
            float centerDepth,
            float neighborDepth,
            float3 centerNormal,
            float3 neighborNormal)
        {
            float signedDepthDifference =
                neighborDepth -
                centerDepth;

            if (signedDepthDifference <
                -_DepthThreshold)
            {
                return 0.0;
            }

            float depthDifference =
                abs(
                    signedDepthDifference
                );

            float sameDepth =
                1.0 -
                step(
                    _DepthThreshold,
                    depthDifference
                );

            float normalDifference =
                1.0 -
                saturate(
                    dot(
                        centerNormal,
                        neighborNormal
                    )
                );

            float normalEdge =
                step(
                    _NormalThreshold,
                    normalDifference
                );

            return
                normalEdge *
                sameDepth;
        }

        // =====================================================
        // NEIGHBOR
        // =====================================================

        float CheckNeighbor(
            float2 neighborUV,
            float centerMask,
            float centerDepth,
            float3 centerNormal)
        {
            float neighborMask =
                GetMask(
                    neighborUV
                );

            if (neighborMask < 0.5)
            {
                return CheckSilhouette(
                    centerMask,
                    neighborMask
                );
            }

            float neighborDepth =
                GetDepth(
                    neighborUV
                );

            float signedDepthDifference =
                neighborDepth -
                centerDepth;

            // Neighbor가 현재 Surface보다 앞쪽이면
            // 현재 Surface가 Outline을 소유하지 않는다.
            if (signedDepthDifference <
                -_DepthThreshold)
            {
                return 0.0;
            }

            float depthEdge =
                CheckDepthEdge(
                    centerDepth,
                    neighborDepth
                );

            if (depthEdge > 0.5)
            {
                return 1.0;
            }

            float3 neighborNormal =
                GetNormal(
                    neighborUV
                );

            return CheckNormalEdge(
                centerDepth,
                neighborDepth,
                centerNormal,
                neighborNormal
            );
        }

        // =====================================================
        // OUTLINE
        // =====================================================

        float GetOutline(
            float2 uv,
            float centerMask,
            float centerDepth,
            float3 centerNormal,
            int thickness)
        {
            if (thickness <= 0)
            {
                return 0.0;
            }

            float2 texel =
                _PixelDepthTexture_TexelSize.xy;

            float outline =
                0.0;

            [unroll]
            for (int distance = 1;
                 distance <= 4;
                 distance++)
            {
                if (distance > thickness)
                {
                    break;
                }

                float2 offsetX =
                    float2(
                        texel.x *
                        distance,
                        0.0
                    );

                float2 offsetY =
                    float2(
                        0.0,
                        texel.y *
                        distance
                    );

                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + offsetX,
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );

                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv - offsetX,
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );

                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv + offsetY,
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );

                outline =
                    max(
                        outline,
                        CheckNeighbor(
                            uv - offsetY,
                            centerMask,
                            centerDepth,
                            centerNormal
                        )
                    );
            }

            return outline;
        }

        // =====================================================
        // FRAGMENT
        // =====================================================

        half4 Frag(Varyings input) : SV_Target
        {
            float2 uv =
                input.texcoord;

        // =================================================
        // NORMAL SCENE
        // =================================================

        half4 sceneColor =
            SAMPLE_TEXTURE2D_X(
                _BlitTexture,
                sampler_BlitTexture,
                uv
            );

        // =================================================
        // PIXELIZER MASK
        // =================================================

        float mask =
            GetMask(
                uv
            );

        if (mask < 0.5)
        {
            return sceneColor;
        }

        // =================================================
        // PIXELIZER DATA
        // =================================================

        half4 pixelColor =
            GetPixelColor(
                uv
            );

        float centerDepth =
            GetDepth(
                uv
            );

        float3 centerNormal =
            GetNormal(
                uv
            );

        half4 outlineData =
            GetOutlineData(
                uv
            );

        // =================================================
        // MATERIAL OUTLINE SETTINGS
        // =================================================

        half3 outlineColor =
            outlineData.rgb;

        float decodedThickness =
            outlineData.a *
            4.0;

        int thickness =
            clamp(
                (int)round(
                    decodedThickness
                ),
                0,
                4
            );

        // =================================================
        // OUTLINE
        // =================================================

        float outline =
            0.0;

        if (thickness > 0)
        {
            outline =
                GetOutline(
                    uv,
                    mask,
                    centerDepth,
                    centerNormal,
                    thickness
                );
        }

        // =================================================
        // FINAL
        // =================================================

        half3 finalPixelColor =
            pixelColor.rgb;

        if (outline > 0.5)
        {
            finalPixelColor =
                outlineColor;
        }

        return half4(
            finalPixelColor,
            1.0
        );
    }

    ENDHLSL
}
    }
}