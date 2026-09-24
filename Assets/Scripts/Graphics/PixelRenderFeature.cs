using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class PixelSettings
    {
        [Range(1, 16)] public int pixelSize = 4;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        public Material compositeMaterial;

        [Header("Pixel Alignment")]
        public bool pixelSnapEnabled = true;
        [Min(0.01f)] public float pixelSnapReferenceDistance = 10.0f;
    }

    public PixelSettings settings = new PixelSettings();
    private PixelRenderPass pixelRenderPass;

    public override void Create()
    {
        pixelRenderPass = new PixelRenderPass(settings);
        pixelRenderPass.renderPassEvent = settings.renderPassEvent;
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        if (pixelRenderPass == null) return;
        pixelRenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
        pixelRenderPass.SetCameraTargets(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.compositeMaterial == null) return;
        renderer.EnqueuePass(pixelRenderPass);
    }

    protected override void Dispose(bool disposing)
    {
        pixelRenderPass?.Dispose();
    }

    private class PixelRenderPass : ScriptableRenderPass
    {
        private readonly PixelSettings settings;
        private readonly ShaderTagId pixelizerShaderTag = new ShaderTagId("Pixelizer");
        private FilteringSettings filteringSettings;

        private RTHandle cameraColorTarget;
        private RTHandle cameraDepthTarget;

        // Full resolution MRT attachments.
        private RTHandle pixelFullResolutionRT;        // RGB color, A object ID
        private RTHandle pixelDepthFullResolutionRT;   // R eye depth
        private RTHandle pixelNormalFullResolutionRT;  // RGB encoded world normal
        private RTHandle pixelOutlineDataFullResolutionRT; // RGB outline color, A thickness

        // Low resolution buffers consumed by the composite shader.
        private RTHandle lowResolutionRT;
        private RTHandle pixelDepthRT;
        private RTHandle pixelNormalRT;
        private RTHandle pixelOutlineDataRT;
        private RTHandle compositeRT;

        private static readonly int PixelTextureID = Shader.PropertyToID("_PixelTexture");
        private static readonly int PixelDepthTextureID = Shader.PropertyToID("_PixelDepthTexture");
        private static readonly int PixelNormalTextureID = Shader.PropertyToID("_PixelNormalTexture");
        private static readonly int PixelOutlineDataTextureID = Shader.PropertyToID("_PixelOutlineDataTexture");
        private static readonly int PixelDepthTextureTexelSizeID = Shader.PropertyToID("_PixelDepthTexture_TexelSize");
        private static readonly int PixelSizeID = Shader.PropertyToID("_PixelizerPixelSize");
        private static readonly int PixelizerScreenSizeID = Shader.PropertyToID("_PixelizerScreenSize");
        private static readonly int PixelizerSubPixelOffsetID = Shader.PropertyToID("_PixelizerSubPixelOffset");
        private static readonly int PixelizerSnapEnabledID = Shader.PropertyToID("_PixelizerSnapEnabled");

        private int currentFullWidth;
        private int currentFullHeight;
        private int currentLowWidth;
        private int currentLowHeight;

        private Camera lastCamera;
        private Vector3 cameraSnapOrigin;
        private bool cameraSnapOriginInitialized;

        public PixelRenderPass(PixelSettings settings)
        {
            this.settings = settings;
            filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        }

        public void SetCameraTargets(RTHandle colorTarget, RTHandle depthTarget)
        {
            cameraColorTarget = colorTarget;
            cameraDepthTarget = depthTarget;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor cameraDescriptor = renderingData.cameraData.cameraTargetDescriptor;

            int fullWidth = cameraDescriptor.width;
            int fullHeight = cameraDescriptor.height;

            if (cameraDepthTarget != null && cameraDepthTarget.rt != null)
            {
                fullWidth = cameraDepthTarget.rt.width;
                fullHeight = cameraDepthTarget.rt.height;
            }

            currentFullWidth = fullWidth;
            currentFullHeight = fullHeight;

            RenderTextureDescriptor fullDescriptor = cameraDescriptor;
            fullDescriptor.width = fullWidth;
            fullDescriptor.height = fullHeight;
            fullDescriptor.depthBufferBits = 0;
            fullDescriptor.msaaSamples = cameraDepthTarget != null && cameraDepthTarget.rt != null
                ? cameraDepthTarget.rt.antiAliasing
                : 1;

            // Color alpha is now Object ID, so an alpha-capable format is required.
            RenderTextureDescriptor fullColorDescriptor = fullDescriptor;
            fullColorDescriptor.colorFormat = RenderTextureFormat.ARGB32;
            RenderingUtils.ReAllocateIfNeeded(ref pixelFullResolutionRT, fullColorDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_PixelFullResolutionRT");

            RenderTextureDescriptor fullDepthDescriptor = fullDescriptor;
            fullDepthDescriptor.colorFormat = RenderTextureFormat.RFloat;
            RenderingUtils.ReAllocateIfNeeded(ref pixelDepthFullResolutionRT, fullDepthDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_PixelDepthFullResolutionRT");

            RenderTextureDescriptor fullNormalDescriptor = fullDescriptor;
            fullNormalDescriptor.colorFormat = RenderTextureFormat.ARGBHalf;
            RenderingUtils.ReAllocateIfNeeded(ref pixelNormalFullResolutionRT, fullNormalDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_PixelNormalFullResolutionRT");

            RenderTextureDescriptor fullOutlineDescriptor = fullDescriptor;
            fullOutlineDescriptor.colorFormat = RenderTextureFormat.ARGB32;
            RenderingUtils.ReAllocateIfNeeded(ref pixelOutlineDataFullResolutionRT, fullOutlineDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_PixelOutlineDataFullResolutionRT");

            int safePixelSize = Mathf.Max(1, settings.pixelSize);
            currentLowWidth = Mathf.Max(1, fullWidth / safePixelSize);
            currentLowHeight = Mathf.Max(1, fullHeight / safePixelSize);

            RenderTextureDescriptor lowDescriptor = cameraDescriptor;
            lowDescriptor.width = currentLowWidth;
            lowDescriptor.height = currentLowHeight;
            lowDescriptor.depthBufferBits = 0;
            lowDescriptor.msaaSamples = 1;

            RenderTextureDescriptor lowColorDescriptor = lowDescriptor;
            lowColorDescriptor.colorFormat = RenderTextureFormat.ARGB32;
            RenderingUtils.ReAllocateIfNeeded(ref lowResolutionRT, lowColorDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_PixelLowResolutionRT");

            RenderTextureDescriptor lowDepthDescriptor = lowDescriptor;
            lowDepthDescriptor.colorFormat = RenderTextureFormat.RFloat;
            RenderingUtils.ReAllocateIfNeeded(ref pixelDepthRT, lowDepthDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_PixelDepthRT");

            RenderTextureDescriptor lowNormalDescriptor = lowDescriptor;
            lowNormalDescriptor.colorFormat = RenderTextureFormat.ARGBHalf;
            RenderingUtils.ReAllocateIfNeeded(ref pixelNormalRT, lowNormalDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_PixelNormalRT");

            RenderTextureDescriptor lowOutlineDescriptor = lowDescriptor;
            lowOutlineDescriptor.colorFormat = RenderTextureFormat.ARGB32;
            RenderingUtils.ReAllocateIfNeeded(ref pixelOutlineDataRT, lowOutlineDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_PixelOutlineDataRT");

            RenderTextureDescriptor compositeDescriptor = cameraDescriptor;
            compositeDescriptor.depthBufferBits = 0;
            compositeDescriptor.msaaSamples = 1;
            RenderingUtils.ReAllocateIfNeeded(ref compositeRT, compositeDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_PixelCompositeRT");
        }

        private Vector2 CalculateSubPixelOffset(Camera camera)
        {
            if (!settings.pixelSnapEnabled || camera == null || currentLowWidth <= 0 || currentLowHeight <= 0)
                return Vector2.zero;

            if (!cameraSnapOriginInitialized || lastCamera != camera)
            {
                lastCamera = camera;
                cameraSnapOrigin = camera.transform.position;
                cameraSnapOriginInitialized = true;
                return Vector2.zero;
            }

            Vector3 cameraDelta = camera.transform.position - cameraSnapOrigin;
            float horizontalMovement = Vector3.Dot(cameraDelta, camera.transform.right);
            float verticalMovement = Vector3.Dot(cameraDelta, camera.transform.up);
            float referenceDistance = Mathf.Max(0.01f, settings.pixelSnapReferenceDistance);

            float worldHeight;
            if (camera.orthographic)
                worldHeight = camera.orthographicSize * 2.0f;
            else
            {
                float halfFovRadians = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;
                worldHeight = 2.0f * referenceDistance * Mathf.Tan(halfFovRadians);
            }

            if (worldHeight <= 0.00001f) return Vector2.zero;

            float aspect = (float)currentLowWidth / currentLowHeight;
            float worldWidth = worldHeight * aspect;
            float horizontalPixelMovement = horizontalMovement / worldWidth * currentLowWidth;
            float verticalPixelMovement = verticalMovement / worldHeight * currentLowHeight;

            return new Vector2(
                horizontalPixelMovement - Mathf.Round(horizontalPixelMovement),
                verticalPixelMovement - Mathf.Round(verticalPixelMovement)
            );
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (cameraColorTarget == null || cameraDepthTarget == null || settings.compositeMaterial == null)
                return;

            // Four simultaneous color attachments are required by this optimized path.
            if (SystemInfo.supportedRenderTargetCount < 4)
            {
                Debug.LogWarning("Pixelizer MRT requires at least 4 simultaneous render targets on this GPU.");
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get("Pixel Render Pass MRT");
            int safePixelSize = Mathf.Max(1, settings.pixelSize);

            cmd.SetGlobalFloat(PixelSizeID, safePixelSize);
            cmd.SetGlobalVector(PixelizerScreenSizeID, new Vector4(currentFullWidth, currentFullHeight, currentLowWidth, currentLowHeight));

            Camera camera = renderingData.cameraData.camera;
            Vector2 subPixelOffset = CalculateSubPixelOffset(camera);
            cmd.SetGlobalVector(PixelizerSubPixelOffsetID, new Vector4(subPixelOffset.x, subPixelOffset.y, 0.0f, 0.0f));
            cmd.SetGlobalFloat(PixelizerSnapEnabledID, settings.pixelSnapEnabled ? 1.0f : 0.0f);

            SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;

            // =====================================================
            // ONE GEOMETRY DRAW -> FOUR MRT BUFFERS
            // =====================================================
            RenderTargetIdentifier[] mrt =
            {
                pixelFullResolutionRT.nameID,
                pixelDepthFullResolutionRT.nameID,
                pixelNormalFullResolutionRT.nameID,
                pixelOutlineDataFullResolutionRT.nameID
            };

            cmd.SetRenderTarget(mrt, cameraDepthTarget.nameID);
            cmd.ClearRenderTarget(false, true, Color.clear);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            DrawingSettings drawingSettings = CreateDrawingSettings(pixelizerShaderTag, ref renderingData, sortingCriteria);
            drawingSettings.enableInstancing = true;

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

            // =====================================================
            // FULL -> LOW
            // =====================================================
            Blitter.BlitCameraTexture(cmd, pixelFullResolutionRT, lowResolutionRT);
            Blitter.BlitCameraTexture(cmd, pixelDepthFullResolutionRT, pixelDepthRT);
            Blitter.BlitCameraTexture(cmd, pixelNormalFullResolutionRT, pixelNormalRT);
            Blitter.BlitCameraTexture(cmd, pixelOutlineDataFullResolutionRT, pixelOutlineDataRT);

            // Object ID and mask are both encoded in _PixelTexture alpha.
            cmd.SetGlobalTexture(PixelTextureID, lowResolutionRT);
            cmd.SetGlobalTexture(PixelDepthTextureID, pixelDepthRT);
            cmd.SetGlobalTexture(PixelNormalTextureID, pixelNormalRT);
            cmd.SetGlobalTexture(PixelOutlineDataTextureID, pixelOutlineDataRT);

            if (pixelDepthRT != null && pixelDepthRT.rt != null)
            {
                cmd.SetGlobalVector(PixelDepthTextureTexelSizeID, new Vector4(
                    1.0f / pixelDepthRT.rt.width,
                    1.0f / pixelDepthRT.rt.height,
                    pixelDepthRT.rt.width,
                    pixelDepthRT.rt.height
                ));
            }

            Blitter.BlitCameraTexture(cmd, cameraColorTarget, compositeRT, settings.compositeMaterial, 0);
            Blitter.BlitCameraTexture(cmd, compositeRT, cameraColorTarget);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Dispose()
        {
            pixelFullResolutionRT?.Release();
            pixelDepthFullResolutionRT?.Release();
            pixelNormalFullResolutionRT?.Release();
            pixelOutlineDataFullResolutionRT?.Release();

            lowResolutionRT?.Release();
            pixelDepthRT?.Release();
            pixelNormalRT?.Release();
            pixelOutlineDataRT?.Release();
            compositeRT?.Release();
        }
    }
}
