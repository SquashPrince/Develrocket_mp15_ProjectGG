using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class PixelRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        [Range(1, 16)]
        public int pixelSize = 4;

        public Material compositeMaterial;
    }


    public Settings settings = new Settings();

    private PixelRenderPass pixelRenderPass;


    public override void Create()
    {
        pixelRenderPass = new PixelRenderPass(settings);

        pixelRenderPass.renderPassEvent =
            RenderPassEvent.AfterRenderingOpaques;
    }


    public override void SetupRenderPasses(
        ScriptableRenderer renderer,
        in RenderingData renderingData
    )
    {
        pixelRenderPass.ConfigureInput(
            ScriptableRenderPassInput.Color |
            ScriptableRenderPassInput.Depth
        );

        pixelRenderPass.SetCameraTargets(
            renderer.cameraColorTargetHandle,
            renderer.cameraDepthTargetHandle
        );
    }


    public override void AddRenderPasses(
        ScriptableRenderer renderer,
        ref RenderingData renderingData
    )
    {
        if (settings.compositeMaterial == null)
        {
            return;
        }

        renderer.EnqueuePass(pixelRenderPass);
    }


    protected override void Dispose(bool disposing)
    {
        pixelRenderPass?.Dispose();
    }



    private class PixelRenderPass : ScriptableRenderPass
    {
        private readonly Settings settings;


        // =====================================================
        // Shader Tags
        // =====================================================

        private readonly ShaderTagId pixelizerShaderTag =
            new ShaderTagId("Pixelizer");

        private readonly ShaderTagId pixelizerMaskShaderTag =
            new ShaderTagId("PixelizerMask");

        private readonly ShaderTagId pixelizerDepthShaderTag =
            new ShaderTagId("PixelizerDepth");

        private readonly ShaderTagId pixelizerNormalShaderTag =
            new ShaderTagId("PixelizerNormal");


        // =====================================================
        // Filtering
        // =====================================================

        private FilteringSettings filteringSettings;


        // =====================================================
        // Camera
        // =====================================================

        private RTHandle cameraColorTarget;
        private RTHandle cameraDepthTarget;


        // =====================================================
        // Full Resolution
        // =====================================================

        private RTHandle pixelFullResolutionRT;
        private RTHandle pixelMaskFullResolutionRT;
        private RTHandle pixelDepthFullResolutionRT;
        private RTHandle pixelNormalFullResolutionRT;


        // =====================================================
        // Low Resolution
        // =====================================================

        private RTHandle lowResolutionRT;
        private RTHandle pixelMaskRT;
        private RTHandle pixelDepthRT;
        private RTHandle pixelNormalRT;


        // =====================================================
        // Composite
        // =====================================================

        private RTHandle compositeRT;


        // =====================================================
        // Property IDs
        // =====================================================

        private static readonly int PixelTextureID =
            Shader.PropertyToID("_PixelTexture");

        private static readonly int PixelMaskTextureID =
            Shader.PropertyToID("_PixelMaskTexture");

        private static readonly int PixelDepthTextureID =
            Shader.PropertyToID("_PixelDepthTexture");

        private static readonly int PixelNormalTextureID =
            Shader.PropertyToID("_PixelNormalTexture");

        private static readonly int PixelDepthTextureTexelSizeID =
            Shader.PropertyToID("_PixelDepthTexture_TexelSize");


        public PixelRenderPass(Settings settings)
        {
            this.settings = settings;

            filteringSettings =
                new FilteringSettings(
                    RenderQueueRange.opaque
                );
        }


        public void SetCameraTargets(
            RTHandle colorTarget,
            RTHandle depthTarget
        )
        {
            cameraColorTarget = colorTarget;
            cameraDepthTarget = depthTarget;
        }


        public override void OnCameraSetup(
            CommandBuffer cmd,
            ref RenderingData renderingData
        )
        {
            RenderTextureDescriptor cameraDescriptor =
                renderingData.cameraData.cameraTargetDescriptor;


            // =================================================
            // Full Resolution Size
            // =================================================

            int fullWidth =
                cameraDescriptor.width;

            int fullHeight =
                cameraDescriptor.height;


            if (
                cameraDepthTarget != null &&
                cameraDepthTarget.rt != null
            )
            {
                fullWidth =
                    cameraDepthTarget.rt.width;

                fullHeight =
                    cameraDepthTarget.rt.height;
            }


            // =================================================
            // Full Resolution Descriptor
            // =================================================

            RenderTextureDescriptor fullDescriptor =
                cameraDescriptor;

            fullDescriptor.width = fullWidth;
            fullDescriptor.height = fullHeight;

            fullDescriptor.depthBufferBits = 0;


            if (
                cameraDepthTarget != null &&
                cameraDepthTarget.rt != null
            )
            {
                fullDescriptor.msaaSamples =
                    cameraDepthTarget.rt.antiAliasing;
            }
            else
            {
                fullDescriptor.msaaSamples = 1;
            }


            // =================================================
            // Color FullRes
            // =================================================

            RenderingUtils.ReAllocateIfNeeded(
                ref pixelFullResolutionRT,
                fullDescriptor,
                FilterMode.Point,
                TextureWrapMode.Clamp,
                name: "_PixelFullResolutionRT"
            );


            // =================================================
            // Mask FullRes
            // =================================================

            RenderingUtils.ReAllocateIfNeeded(
                ref pixelMaskFullResolutionRT,
                fullDescriptor,
                FilterMode.Point,
                TextureWrapMode.Clamp,
                name: "_PixelMaskFullResolutionRT"
            );


            // =================================================
            // Depth FullRes
            // =================================================

            RenderTextureDescriptor fullDepthDescriptor =
                fullDescriptor;

            fullDepthDescriptor.colorFormat =
                RenderTextureFormat.RFloat;


            RenderingUtils.ReAllocateIfNeeded(
                ref pixelDepthFullResolutionRT,
                fullDepthDescriptor,
                FilterMode.Point,
                TextureWrapMode.Clamp,
                name: "_PixelDepthFullResolutionRT"
            );


            // =================================================
            // Normal FullRes
            //
            // RGB에 Normal XYZ를 저장해야 하므로
            // ARGBHalf를 사용한다.
            // =================================================

            RenderTextureDescriptor fullNormalDescriptor =
                fullDescriptor;

            fullNormalDescriptor.colorFormat =
                RenderTextureFormat.ARGBHalf;


            RenderingUtils.ReAllocateIfNeeded(
                ref pixelNormalFullResolutionRT,
                fullNormalDescriptor,
                FilterMode.Point,
                TextureWrapMode.Clamp,
                name: "_PixelNormalFullResolutionRT"
            );


            // =================================================
            // Low Resolution Size
            // =================================================

            int lowWidth =
                Mathf.Max(
                    1,
                    fullWidth / settings.pixelSize
                );

            int lowHeight =
                Mathf.Max(
                    1,
                    fullHeight / settings.pixelSize
                );


            RenderTextureDescriptor lowDescriptor =
                cameraDescriptor;

            lowDescriptor.width = lowWidth;
            lowDescriptor.height = lowHeight;

            lowDescriptor.depthBufferBits = 0;
            lowDescriptor.msaaSamples = 1;


            // =================================================
            // Color LowRes
            // =================================================

            RenderingUtils.ReAllocateIfNeeded(
                ref lowResolutionRT,
                lowDescriptor,
                FilterMode.Point,
                TextureWrapMode.Clamp,
                name: "_PixelLowResolutionRT"
            );


            // =================================================
            // Mask LowRes
            // =================================================

            RenderingUtils.ReAllocateIfNeeded(
                ref pixelMaskRT,
                lowDescriptor,
                FilterMode.Point,
                TextureWrapMode.Clamp,
                name: "_PixelMaskRT"
            );


            // =================================================
            // Depth LowRes
            // =================================================

            RenderTextureDescriptor lowDepthDescriptor =
                lowDescriptor;

            lowDepthDescriptor.colorFormat =
                RenderTextureFormat.RFloat;


            RenderingUtils.ReAllocateIfNeeded(
                ref pixelDepthRT,
                lowDepthDescriptor,
                FilterMode.Point,
                TextureWrapMode.Clamp,
                name: "_PixelDepthRT"
            );


            // =================================================
            // Normal LowRes
            // =================================================

            RenderTextureDescriptor lowNormalDescriptor =
                lowDescriptor;

            lowNormalDescriptor.colorFormat =
                RenderTextureFormat.ARGBHalf;


            RenderingUtils.ReAllocateIfNeeded(
                ref pixelNormalRT,
                lowNormalDescriptor,
                FilterMode.Point,
                TextureWrapMode.Clamp,
                name: "_PixelNormalRT"
            );


            // =================================================
            // Composite
            // =================================================

            RenderTextureDescriptor compositeDescriptor =
                cameraDescriptor;

            compositeDescriptor.depthBufferBits = 0;
            compositeDescriptor.msaaSamples = 1;


            RenderingUtils.ReAllocateIfNeeded(
                ref compositeRT,
                compositeDescriptor,
                FilterMode.Bilinear,
                TextureWrapMode.Clamp,
                name: "_PixelCompositeRT"
            );


            ConfigureTarget(cameraColorTarget);
        }


        public override void Execute(
            ScriptableRenderContext context,
            ref RenderingData renderingData
        )
        {
            if (
                settings.compositeMaterial == null ||
                cameraColorTarget == null ||
                cameraDepthTarget == null
            )
            {
                return;
            }


            CommandBuffer cmd =
                CommandBufferPool.Get(
                    "Pixel Render Pass"
                );


            SortingCriteria sortingCriteria =
                renderingData.cameraData.defaultOpaqueSortFlags;


            // =================================================
            // 1. Original Scene
            // =================================================

            Blitter.BlitCameraTexture(
                cmd,
                cameraColorTarget,
                compositeRT
            );


            // =================================================
            // 2. Pixel Color FullRes
            // =================================================

            CoreUtils.SetRenderTarget(
                cmd,
                pixelFullResolutionRT,
                cameraDepthTarget,
                ClearFlag.Color,
                Color.black
            );

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();


            DrawingSettings pixelDrawingSettings =
                CreateDrawingSettings(
                    pixelizerShaderTag,
                    ref renderingData,
                    sortingCriteria
                );


            context.DrawRenderers(
                renderingData.cullResults,
                ref pixelDrawingSettings,
                ref filteringSettings
            );


            // =================================================
            // 3. Mask FullRes
            // =================================================

            CoreUtils.SetRenderTarget(
                cmd,
                pixelMaskFullResolutionRT,
                cameraDepthTarget,
                ClearFlag.Color,
                Color.black
            );

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();


            DrawingSettings maskDrawingSettings =
                CreateDrawingSettings(
                    pixelizerMaskShaderTag,
                    ref renderingData,
                    sortingCriteria
                );


            context.DrawRenderers(
                renderingData.cullResults,
                ref maskDrawingSettings,
                ref filteringSettings
            );


            // =================================================
            // 4. Depth FullRes
            // =================================================

            CoreUtils.SetRenderTarget(
                cmd,
                pixelDepthFullResolutionRT,
                cameraDepthTarget,
                ClearFlag.Color,
                Color.black
            );

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();


            DrawingSettings depthDrawingSettings =
                CreateDrawingSettings(
                    pixelizerDepthShaderTag,
                    ref renderingData,
                    sortingCriteria
                );


            context.DrawRenderers(
                renderingData.cullResults,
                ref depthDrawingSettings,
                ref filteringSettings
            );


            // =================================================
            // 5. Normal FullRes
            //
            // Depth attachment로 Camera Depth를 사용한다.
            // 따라서 가려진 Pixelizer Normal은 기록되지 않는다.
            // =================================================

            CoreUtils.SetRenderTarget(
                cmd,
                pixelNormalFullResolutionRT,
                cameraDepthTarget,
                ClearFlag.Color,
                Color.black
            );

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();


            DrawingSettings normalDrawingSettings =
                CreateDrawingSettings(
                    pixelizerNormalShaderTag,
                    ref renderingData,
                    sortingCriteria
                );


            context.DrawRenderers(
                renderingData.cullResults,
                ref normalDrawingSettings,
                ref filteringSettings
            );


            // =================================================
            // 6. Downsample Color
            // =================================================

            Blitter.BlitCameraTexture(
                cmd,
                pixelFullResolutionRT,
                lowResolutionRT
            );


            // =================================================
            // 7. Downsample Mask
            // =================================================

            Blitter.BlitCameraTexture(
                cmd,
                pixelMaskFullResolutionRT,
                pixelMaskRT
            );


            // =================================================
            // 8. Downsample Depth
            // =================================================

            Blitter.BlitCameraTexture(
                cmd,
                pixelDepthFullResolutionRT,
                pixelDepthRT
            );


            // =================================================
            // 9. Downsample Normal
            // =================================================

            Blitter.BlitCameraTexture(
                cmd,
                pixelNormalFullResolutionRT,
                pixelNormalRT
            );


            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();


            // =================================================
            // 10. Global Textures
            // =================================================

            cmd.SetGlobalTexture(
                PixelTextureID,
                lowResolutionRT
            );


            cmd.SetGlobalTexture(
                PixelMaskTextureID,
                pixelMaskRT
            );


            cmd.SetGlobalTexture(
                PixelDepthTextureID,
                pixelDepthRT
            );


            cmd.SetGlobalTexture(
                PixelNormalTextureID,
                pixelNormalRT
            );


            // =================================================
            // Texel Size
            //
            // Depth와 Normal의 해상도가 같으므로
            // 같은 texelSize를 사용할 수 있다.
            // =================================================

            if (
                pixelDepthRT != null &&
                pixelDepthRT.rt != null
            )
            {
                float width =
                    pixelDepthRT.rt.width;

                float height =
                    pixelDepthRT.rt.height;


                cmd.SetGlobalVector(
                    PixelDepthTextureTexelSizeID,
                    new Vector4(
                        1.0f / width,
                        1.0f / height,
                        width,
                        height
                    )
                );
            }


            // =================================================
            // 11. Composite
            // =================================================

            Blitter.BlitCameraTexture(
                cmd,
                compositeRT,
                cameraColorTarget,
                settings.compositeMaterial,
                0
            );


            context.ExecuteCommandBuffer(cmd);

            CommandBufferPool.Release(cmd);
        }


        public void Dispose()
        {
            pixelFullResolutionRT?.Release();
            pixelMaskFullResolutionRT?.Release();
            pixelDepthFullResolutionRT?.Release();
            pixelNormalFullResolutionRT?.Release();

            lowResolutionRT?.Release();
            pixelMaskRT?.Release();
            pixelDepthRT?.Release();
            pixelNormalRT?.Release();

            compositeRT?.Release();
        }
    }
}