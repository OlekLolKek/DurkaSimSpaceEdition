using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;


namespace Code
{
    partial class CameraRenderer
    {
        private readonly CommandBuffer _commandBuffer = new CommandBuffer {name = BUFFER_NAME};

        private static readonly List<ShaderTagId> _drawingShaderTagIds = new List<ShaderTagId>
        {
            new ShaderTagId("SRPDefaultUnlit")
        };
        private const string BUFFER_NAME = "Camera Render";
        
        private ScriptableRenderContext _context;
        private CullingResults _cullingResult;
        private Camera _camera;

        public void Render(ScriptableRenderContext context, Camera camera)
        {
            _context = context;
            _camera = camera;

            if (!Cull(out var parameters))
            {
                return;
            }
            
            Settings(parameters);
            DrawVisible();
            DrawUnsupportedShaders();
            DrawGizmos();
            Submit();
        }

        private void Settings(ScriptableCullingParameters parameters)
        {
            _cullingResult = _context.Cull(ref parameters);
            _context.SetupCameraProperties(_camera);
            _commandBuffer.ClearRenderTarget(true, true, Color.clear);
            _commandBuffer.BeginSample(BUFFER_NAME);
            ExecuteCommandBuffer();
        }
        
        private void Submit()
        {
            _commandBuffer.EndSample(BUFFER_NAME);
            ExecuteCommandBuffer();
            _context.Submit();
        }
        
        private DrawingSettings CreateDrawingSettings(List<ShaderTagId> shaderTags, SortingCriteria sortingCriteria,
            out SortingSettings sortingSettings)
        {
            sortingSettings = new SortingSettings(_camera)
            {
                criteria = sortingCriteria,
            };
            
            var drawingSettings = new DrawingSettings(shaderTags[0], sortingSettings);
            
            for (var i = 1; i < shaderTags.Count; i++)
            {
                drawingSettings.SetShaderPassName(i, shaderTags[i]);
            }
            
            return drawingSettings;
        }

        
        private void ExecuteCommandBuffer()
        {
            _context.ExecuteCommandBuffer(_commandBuffer);
            _commandBuffer.Clear();
        }

        private bool Cull(out ScriptableCullingParameters parameters)
        {
            return _camera.TryGetCullingParameters(out parameters);
        }

        private void DrawVisible()
        {
            var drawingSettings = CreateDrawingSettings(_drawingShaderTagIds, SortingCriteria.CommonOpaque,
                out var sortingSettings);
            var filteringSettings = new FilteringSettings(RenderQueueRange.all);

            _context.DrawRenderers(_cullingResult, ref drawingSettings, ref filteringSettings);
            
            _context.DrawSkybox(_camera);

            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            drawingSettings.sortingSettings = sortingSettings;
            filteringSettings.renderQueueRange = RenderQueueRange.transparent;
            
            _context.DrawRenderers(_cullingResult, ref drawingSettings, ref filteringSettings);
        }

        private void DrawGizmos()
        {
            if (!Handles.ShouldRenderGizmos())
            {
                return;
            }

            _context.DrawGizmos(_camera, GizmoSubset.PreImageEffects);
            _context.DrawGizmos(_camera, GizmoSubset.PostImageEffects);
        }
    }
}