using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class metaball_test : ScriptableRendererFeature
{
    [Serializable]
    public class metaball_testSettings
    {
        public LayerMask metaball_layer;
        public Material threshold_material;
        public RenderPassEvent RenderTiming = RenderPassEvent.AfterRenderingTransparents;
        
    }
    public metaball_testSettings settings = new metaball_testSettings();
    private metaball_testPass m_MetaballPass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_MetaballPass = new metaball_testPass(settings.metaball_layer, settings.threshold_material);


        m_MetaballPass.renderPassEvent = settings.RenderTiming;


    }


    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.threshold_material == null)
        {
            Debug.LogWarning("Metaball requires material");
            return;
        }
        renderer.EnqueuePass(m_MetaballPass);
    }


 

    class metaball_testPass : ScriptableRenderPass
    {
        private LayerMask m_layer_mask;
        private Material m_threshold_material;

        public metaball_testPass(LayerMask layermask, Material material)
        {
            m_layer_mask = layermask;
            m_threshold_material = material;
        }


        private class ParticlePassData
        {
            public RendererListHandle rendererListHandle;
        }

        private class CompositePassData
        {
            public TextureHandle sourceTexture;
            public Material material;
        }


        static void ExecutePass(ParticlePassData data, RasterGraphContext context)
        {
            
        }

        static void ExecuteCompositePass(CompositePassData data, RasterGraphContext context)
        {
            
        }


        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();
            var renderingData = frameData.Get<UniversalRenderingData>();
            var cameraData = frameData.Get<UniversalCameraData>();

            TextureHandle accumulationTexture;

            //pass one
            using (var builder = renderGraph.AddRasterRenderPass<ParticlePassData>("metaball particle accumulation", out var ParticlePassData))
            {
                RenderTextureDescriptor cameraDesc = cameraData.cameraTargetDescriptor;
                TextureDesc textureDesc = new TextureDesc(cameraDesc.width, cameraDesc.height)
                {
                    colorFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm,
                    depthBufferBits = 0,
                    useMipMap = false,
                    msaaSamples = MSAASamples.None,
                    name = "metaball_accumulation_buffer",
                    clearBuffer = true,
                    clearColor = Color.clear
                };
                accumulationTexture = renderGraph.CreateTexture(textureDesc);

                builder.SetRenderAttachment(accumulationTexture, 0, AccessFlags.Write);

                SortingSettings

                DrawingSettings drawSettings = new DrawingSettings(new ShaderTagId("Universal Forward"), sortingSettings);
                drawSettings.SetShaderPassName(1, new ShaderTagId("SRPDefaultUnlit"));


                RendererListParams listParams = new RendererListParams(renderingData.cullResults, drawSettings, filterSettings);

                passData.rendererListHandle = renderGraph.CreateRendererList(listParams);
                builder.UseRendererList(passData.rendererListHandle);
                builder.SetRenderFunc((ParticlePassData data, RasterGraphContext context) => ExecutePass(data, context));
            }

            //pass two
            using (var builder = renderGraph.AddRasterRenderPass<CompositePassData>("metaball threshold composite", out var ParticlePassData))
            {


                builder.SetRenderAttachment(resourceData.activeColorTexture, 0);

                builder.SetRenderFunc((CompositePassData data, RasterGraphContext context) => ExecuteCompositePass(data, context));
            }
        }
    }
}
