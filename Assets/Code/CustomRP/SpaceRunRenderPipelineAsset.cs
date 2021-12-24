using UnityEngine;
using UnityEngine.Rendering;


namespace Code
{
    [CreateAssetMenu(menuName = "Rendering/SpaceRunRenderPipelineAsset")]
    public sealed class SpaceRunRenderPipelineAsset : RenderPipelineAsset
    {
        protected override RenderPipeline CreatePipeline()
        {
            return new SpaceRunRenderPipeline();
        }
    }
}