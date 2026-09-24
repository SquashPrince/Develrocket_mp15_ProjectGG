Shader "Custom/PixelComposite"
{
    Properties
    {
        _DepthThreshold("Depth Threshold", Range(0.001, 2.0)) = 0.1
        _NormalThreshold("Normal Threshold", Range(0.001, 1.0)) = 0.2
        _ObjectDepthThreshold("Object Depth Threshold", Range(0.0001, 0.5)) = 0.01
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

            TEXTURE2D_X(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            TEXTURE2D(_PixelTexture);
            SAMPLER(sampler_PixelTexture);

            TEXTURE2D(_PixelDepthTexture);
            SAMPLER(sampler_PixelDepthTexture);

            TEXTURE2D(_PixelNormalTexture);
            SAMPLER(sampler_PixelNormalTexture);

            TEXTURE2D(_PixelOutlineDataTexture);
            SAMPLER(sampler_PixelOutlineDataTexture);

            float4 _PixelDepthTexture_TexelSize;

            float _DepthThreshold;
            float _NormalThreshold;
            float _ObjectDepthThreshold;

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

                output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
                output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);

                return output;
            }

            float GetMask(float2 uv)
            {
                float encodedID = SAMPLE_TEXTURE2D(_PixelTexture, sampler_PixelTexture, uv).a;
                return step(0.001, encodedID);
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
                float3 encodedNormal = SAMPLE_TEXTURE2D(
                    _PixelNormalTexture,
                    sampler_PixelNormalTexture,
                    uv
                ).rgb;

                return normalize(encodedNormal * 2.0 - 1.0);
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

            int GetObjectID(float2 uv)
            {
                float encodedID = SAMPLE_TEXTURE2D(_PixelTexture, sampler_PixelTexture, uv).a;
                return (int)round(encodedID * 255.0);
            }

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

            float CheckNormalEdge(
                float centerDepth,
                float neighborDepth,
                float3 centerNormal,
                float3 neighborNormal)
            {
                float signedDepthDifference =
                    neighborDepth -
                    centerDepth;

                if (signedDepthDifference < -_DepthThreshold)
                {
                    return 0.0;
                }

                float depthDifference =
                    abs(signedDepthDifference);

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

                return normalEdge * sameDepth;
            }

            float CheckDifferentObjectEdge(
                float centerDepth,
                float neighborDepth)
            {
                float depthDifference =
                    neighborDepth -
                    centerDepth;

                // Neighbor가 확실히 앞.
                if (depthDifference < -_ObjectDepthThreshold)
                {
                    return 0.0;
                }

                // Center가 앞이거나 거의 같은 깊이.
                return 1.0;
            }

            float CheckNeighbor(
                float2 neighborUV,
                float centerMask,
                float centerDepth,
                float3 centerNormal,
                int centerObjectID)
            {
                float neighborMask =
                    GetMask(neighborUV);

                // Pixelizer -> Background
                if (neighborMask < 0.5)
                {
                    return CheckSilhouette(
                        centerMask,
                        neighborMask
                    );
                }

                float neighborDepth =
                    GetDepth(neighborUV);

                int neighborObjectID =
                    GetObjectID(neighborUV);

                // 다른 Object
                if (centerObjectID != neighborObjectID)
                {
                    return CheckDifferentObjectEdge(
                        centerDepth,
                        neighborDepth
                    );
                }

                // 같은 Object
                float signedDepthDifference =
                    neighborDepth -
                    centerDepth;

                if (signedDepthDifference < -_DepthThreshold)
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
                    GetNormal(neighborUV);

                return CheckNormalEdge(
                    centerDepth,
                    neighborDepth,
                    centerNormal,
                    neighborNormal
                );
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

            float GetOutline(
                float2 uv,
                float centerMask,
                float centerDepth,
                float3 centerNormal,
                int centerObjectID,
                float thickness)
            {
                thickness = clamp(thickness, 0.0, 4.0);

                if (thickness <= 0.0)
                    return 0.0;

                float2 texel = _PixelDepthTexture_TexelSize.xy;
                int fullRings = (int)floor(thickness);
                float fractionalRing = frac(thickness);
                float outline = 0.0;

                [unroll]
                for (int distance = 1; distance <= 4; distance++)
                {
                    float ringCoverage = 0.0;

                    if (distance <= fullRings)
                    {
                        ringCoverage = 1.0;
                    }
                    else if (distance == fullRings + 1 && fractionalRing > 0.0)
                    {
                        int2 pixelPosition = (int2)floor(uv / texel);
                        float bayer = GetBayer4x4(pixelPosition);
                        ringCoverage = step(bayer, fractionalRing);
                    }
                    else
                    {
                        break;
                    }

                    if (ringCoverage <= 0.0)
                        continue;

                    float2 offsetX = float2(texel.x * distance, 0.0);
                    float2 offsetY = float2(0.0, texel.y * distance);
                    float2 offsetXY = float2(texel.x * distance, texel.y * distance);

                    float ringOutline = 0.0;

                    ringOutline = max(ringOutline, CheckNeighbor(uv + offsetX, centerMask, centerDepth, centerNormal, centerObjectID));
                    ringOutline = max(ringOutline, CheckNeighbor(uv - offsetX, centerMask, centerDepth, centerNormal, centerObjectID));
                    ringOutline = max(ringOutline, CheckNeighbor(uv + offsetY, centerMask, centerDepth, centerNormal, centerObjectID));
                    ringOutline = max(ringOutline, CheckNeighbor(uv - offsetY, centerMask, centerDepth, centerNormal, centerObjectID));
                    ringOutline = max(ringOutline, CheckNeighbor(uv + offsetXY, centerMask, centerDepth, centerNormal, centerObjectID));
                    ringOutline = max(ringOutline, CheckNeighbor(uv - offsetXY, centerMask, centerDepth, centerNormal, centerObjectID));
                    ringOutline = max(ringOutline, CheckNeighbor(uv + float2(-offsetXY.x, offsetXY.y), centerMask, centerDepth, centerNormal, centerObjectID));
                    ringOutline = max(ringOutline, CheckNeighbor(uv + float2(offsetXY.x, -offsetXY.y), centerMask, centerDepth, centerNormal, centerObjectID));

                    outline = max(outline, ringOutline * ringCoverage);
                }

                return outline;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv =
                    input.texcoord;

                half4 sceneColor =
                    SAMPLE_TEXTURE2D_X(
                        _BlitTexture,
                        sampler_BlitTexture,
                        uv
                    );

                float mask =
                    GetMask(uv);

                if (mask < 0.5)
                {
                    return sceneColor;
                }

                half4 pixelColor =
                    GetPixelColor(uv);

                float centerDepth =
                    GetDepth(uv);

                float3 centerNormal =
                    GetNormal(uv);

                int centerObjectID =
                    GetObjectID(uv);

                half4 outlineData =
                    GetOutlineData(uv);

                half3 outlineColor =
                    outlineData.rgb;

                float decodedThickness =
                    outlineData.a * 4.0;

                float thickness = clamp(decodedThickness, 0.0, 4.0);

                float outline = 0.0;

                if (thickness > 0.0)
                {
                    outline =
                        GetOutline(
                            uv,
                            mask,
                            centerDepth,
                            centerNormal,
                            centerObjectID,
                            thickness
                        );
                }

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