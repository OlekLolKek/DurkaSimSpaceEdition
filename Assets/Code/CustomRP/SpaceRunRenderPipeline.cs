using UnityEngine;
using UnityEngine.Rendering;


namespace Code
{
    public sealed class SpaceRunRenderPipeline : RenderPipeline
    {
        private CameraRenderer _cameraRenderer;

        public SpaceRunRenderPipeline()
        {
            GraphicsSettings.useScriptableRenderPipelineBatching = true;
        }
        
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            _cameraRenderer = new CameraRenderer();
            CamerasRender(context, cameras);
        }

        private void CamerasRender(ScriptableRenderContext context, Camera[] cameras)
        {
            foreach (var camera in cameras)
            {
                _cameraRenderer.Render(context, camera);
            }
        }
    }
}