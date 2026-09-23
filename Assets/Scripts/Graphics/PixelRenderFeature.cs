using Unity.VisualScripting;
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

        public Material compositeMaterials;
    }

    public Settings settings = new();

    private PixelRendererPass pixelRendererPass;

    public override void Create()
    {
        pixelRendererPass = new PixelRendererPass(settings);

        pixelRendererPass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        pixelRendererPass.SetTarget(renderer.cameraColorTargetHandle);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pixelRendererPass);
    }

    private class PixelRendererPass : ScriptableRenderPass
    {
        private PixelRenderFeature.Settings settings;

        private RTHandle lowResolutuinRT;
        private RTHandle sceneColorRT;

        private RTHandle cameraColorTarget;

        private readonly ShaderTagId pixelizerShaderTag = new ShaderTagId("Pixelizer");

        private FilteringSettings filteringSettings;

        public PixelRendererPass(PixelRenderFeature.Settings settings)
        {
            this.settings = settings;

            filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        }

        public void SetTarget(RTHandle cameraColorTarget)
        {
            this.cameraColorTarget = cameraColorTarget;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor cameraDescriptor = renderingData.cameraData.cameraTargetDescriptor;

            cameraDescriptor.width = Mathf.Max(1, cameraDescriptor.width / settings.pixelSize);
            cameraDescriptor.height = Mathf.Max(1, cameraDescriptor.height / settings.pixelSize);

            cameraDescriptor.depthBufferBits = 24;

            RenderingUtils.ReAllocateIfNeeded(ref lowResolutuinRT, cameraDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_PixelLowResolutionRT");

            RenderTextureDescriptor sceneDescriptor = cameraDescriptor;

            sceneDescriptor.depthBufferBits = 0;
            sceneDescriptor.msaaSamples = 1;

            RenderingUtils.ReAllocateIfNeeded(ref sceneColorRT, sceneDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_PixelSceneColorRT");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (cameraColorTarget == null || lowResolutuinRT == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("Pixel Render Pass");

            CoreUtils.SetRenderTarget(cmd, lowResolutuinRT, ClearFlag.All, Color.clear);

            context.ExecuteCommandBuffer(cmd);

            cmd.Clear();

            SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;

            DrawingSettings drawingSettings = CreateDrawingSettings(pixelizerShaderTag, ref renderingData, sortingCriteria);

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

            Blitter.BlitCameraTexture(cmd, lowResolutuinRT, cameraColorTarget);

            context.ExecuteCommandBuffer(cmd);

            CommandBufferPool.Release(cmd);
        }

        public void Dipose()
        {
            lowResolutuinRT.Release();
        }
    }
}
