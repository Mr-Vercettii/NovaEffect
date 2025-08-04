using System;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// Invokes OnRenderObject callback
    /// </summary>

    internal class InvokeOnRenderObjectCallbackPass : ScriptableRenderPass
    {
        public InvokeOnRenderObjectCallbackPass(RenderPassEvent evt)
        {
            profilingSampler = new ProfilingSampler("Invoke OnRenderObject Callback");
            renderPassEvent = evt;

#if URP_COMPATIBILITY_MODE
            //TODO: should we fix and re-enable native render pass for this pass?
            // Currently disabled because when the callback is empty it causes an empty Begin/End RenderPass block, which causes artifacts on Vulkan
            useNativeRenderPass = false;
#endif
        }


#if URP_COMPATIBILITY_MODE
        /// <inheritdoc/>
        [Obsolete(DeprecationMessage.CompatibilityScriptingAPIObsoleteFrom2023_3)]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            renderingData.commandBuffer.InvokeOnRenderObjectCallbacks();
        }
#endif

        private class PassData
        {
            internal TextureHandle colorTarget;
            internal TextureHandle depthTarget;
        }

        internal void Render(RenderGraph renderGraph, TextureHandle colorTarget, TextureHandle depthTarget)
        {
            using (var builder = renderGraph.AddUnsafePass<PassData>(passName, out var passData, profilingSampler))
            {
                builder.UseTexture(colorTarget, AccessFlags.Write);
                builder.UseTexture(depthTarget, AccessFlags.Write);
                builder.AllowPassCulling(false);
                builder.SetRenderFunc((PassData data, UnsafeGraphContext context) =>
                {
                    context.cmd.SetRenderTarget(data.colorTarget, data.depthTarget);
                    context.cmd.InvokeOnRenderObjectCallbacks();
                });
            }
        }
    }
}
