using UnityEngine;
using UnityEngine.Rendering;


namespace Code
{
    partial class CameraRenderer
    {
        partial void DrawUnsupportedShaders();

#if UNITY_EDITOR
        private static readonly ShaderTagId[] _legacyShaderTagIds =
        {
            new ShaderTagId("Always"),
            new ShaderTagId("ForwardBase"),
            new ShaderTagId("PrepassBase"),
            new ShaderTagId("Vertex"),
            new ShaderTagId("VertexLMRGBM"),
            new ShaderTagId("VertexLM"),
        };

        private static Material _errorMaterial = new
            Material(Shader.Find("Hidden/InternalErrorShader"));

        partial void DrawUnsupportedShaders()
        {
            var drawingSettings = new DrawingSettings(_legacyShaderTagIds[0], new SortingSettings(_camera))
            {
                overrideMaterial = _errorMaterial
            };

            for (int i = 0; i < _legacyShaderTagIds.Length; i++)
            {
                drawingSettings.SetShaderPassName(i, _legacyShaderTagIds[i]);
            }

            var filteringSettings = FilteringSettings.defaultValue;
            _context.DrawRenderers(_cullingResult, ref drawingSettings, ref filteringSettings);
        }

        partial void DrawSceneUI()
        {
            // found it here: https://catlikecoding.com/unity/tutorials/scriptable-render-pipeline/custom-pipeline/
            if (_camera.cameraType == CameraType.SceneView)
            {
                ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
            }
        }
#endif
        partial void DrawGizmos();

        partial void DrawSceneUI();
    }
}