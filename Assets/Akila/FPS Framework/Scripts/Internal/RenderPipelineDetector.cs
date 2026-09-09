#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Akila.FPSFramework
{
    internal static class RenderPipelineDetector
    {
        public enum PipelineType
        {
            BuiltIn,
            UniversalRP,
            HighDefinitionRP,
            CustomSRP
        }

        private static PipelineType? _cache;

        public static PipelineType CurrentPipeline
        {
            get
            {
                _cache = DetectPipeline();
                return _cache.Value;
            }
        }

        public static string CurrentPiplineName
        {
            get
            {
                if (CurrentPipeline == PipelineType.BuiltIn)
                    return "Built-In";

                if (CurrentPipeline == PipelineType.UniversalRP)
                    return "Universal RP";

                if (CurrentPipeline == PipelineType.HighDefinitionRP)
                    return "High Definition RP";

                return "Custom SRP";
            }
        }

        private static PipelineType DetectPipeline()
        {
            try
            {
                var gp = GraphicsSettings.currentRenderPipeline;
                var typeName = SafeTypeName(gp);
                var result = ClassifyByTypeName(typeName);
                if (result != PipelineType.BuiltIn) return result;

                var qp = QualitySettings.renderPipeline;
                typeName = SafeTypeName(qp);
                result = ClassifyByTypeName(typeName);
                if (result != PipelineType.BuiltIn) return result;

                var rp = RenderPipelineManager.currentPipeline;
                typeName = rp != null ? rp.GetType().FullName : null;
                result = ClassifyByTypeName(typeName);
                if (result != PipelineType.BuiltIn) return result;

                return PipelineType.BuiltIn;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"RenderPipelineDetector: exception while detecting RP: {e.Message}");
                return PipelineType.BuiltIn;
            }
        }

        private static string SafeTypeName(UnityEngine.Object asset)
        {
            try
            {
                return asset != null ? asset.GetType().FullName : null;
            }
            catch
            {
                return null;
            }
        }

        private static PipelineType ClassifyByTypeName(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName)) return PipelineType.BuiltIn;

            if (fullTypeName.IndexOf("UniversalRenderPipeline", StringComparison.OrdinalIgnoreCase) >= 0 ||
                fullTypeName.IndexOf("UniversalRenderPipelineAsset", StringComparison.OrdinalIgnoreCase) >= 0 ||
                fullTypeName.IndexOf("UnityEngine.Rendering.Universal", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return PipelineType.UniversalRP;
            }

            if (fullTypeName.IndexOf("HDRenderPipeline", StringComparison.OrdinalIgnoreCase) >= 0 ||
                fullTypeName.IndexOf("HighDefinition", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return PipelineType.HighDefinitionRP;
            }

            return PipelineType.CustomSRP;
        }
    }
}
