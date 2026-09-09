#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;

namespace Akila.FPSFramework
{
    internal static class RPConvertor
    {
        private static readonly string urpIdentifier = "com.unity.render-pipelines.universal";
        private static readonly string hdrpIdentifier = "com.unity.render-pipelines.high-definition";

        public static void InstallRenderPipline(CoreConfig.RenderPipelineType renderPipelineType)
        {
            if (renderPipelineType == CoreConfig.RenderPipelineType.UniversalRP && !IsRPInstalled(renderPipelineType))
                InstallURP();

            if (renderPipelineType == CoreConfig.RenderPipelineType.HighDefinitionRP && !IsRPInstalled(renderPipelineType))
                InstallHDRP();
        }

        public static void SetupRenderPipline(CoreConfig.RenderPipelineType renderPipelineType, string rpPackageGUID, string[] guidsToDelete = null)
        {
            if(guidsToDelete != null)
            {
                foreach(string guid in guidsToDelete)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);

                    if (!string.IsNullOrEmpty(path))
                        AssetDatabase.DeleteAsset(path);
                }
            }

            if(renderPipelineType == CoreConfig.RenderPipelineType.BuiltIn)
                SetupBIRP();

            if(renderPipelineType == CoreConfig.RenderPipelineType.UniversalRP && RenderPipelineDetector.CurrentPipeline != RenderPipelineDetector.PipelineType.UniversalRP)
                SetupURP();

            if(renderPipelineType == CoreConfig.RenderPipelineType.HighDefinitionRP && RenderPipelineDetector.CurrentPipeline != RenderPipelineDetector.PipelineType.HighDefinitionRP)
                SetupHDRP();

            if (!string.IsNullOrEmpty(rpPackageGUID))
                InstallConversionPackage(rpPackageGUID);

            AssetDatabase.SaveAssets();
        }

        public static bool IsRPInstalled(CoreConfig.RenderPipelineType renderPipelineType)
        {
            if(renderPipelineType == CoreConfig.RenderPipelineType.BuiltIn)
                return true;

            string assemplyName = "";
            string packageIdentifier = "";

            if (renderPipelineType == CoreConfig.RenderPipelineType.UniversalRP)
                packageIdentifier = urpIdentifier;

            if (renderPipelineType == CoreConfig.RenderPipelineType.HighDefinitionRP)
                packageIdentifier = hdrpIdentifier;

            if (renderPipelineType == CoreConfig.RenderPipelineType.UniversalRP)
                assemplyName = "Unity.RenderPipelines.Universal.Runtime";

            if (renderPipelineType == CoreConfig.RenderPipelineType.HighDefinitionRP)
                assemplyName = "Unity.RenderPipelines.HighDefinition.Runtime";

            if (string.IsNullOrEmpty(assemplyName))
                return false;

            if (string.IsNullOrEmpty(packageIdentifier))
                return false;

            // Reflection check: look for the URP assembly
            var urpAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == assemplyName);

            if (urpAssembly != null)
                return true;

            // Fallback: check PackageManager manifest
            string manifestPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "../Packages/manifest.json");
            if (!System.IO.File.Exists(manifestPath))
                return false;

            string manifestText = System.IO.File.ReadAllText(manifestPath);
            return manifestText.Contains(packageIdentifier);
        }


        private static void InstallURP()
        {
            // Use the Package Manager to add URP
            var request = Client.Add(urpIdentifier);
            EditorApplication.update += () =>
            {
                if (request.IsCompleted)
                {
                    if (request.Status == StatusCode.Success)
                    {
                        
                    }
                    else if (request.Status == StatusCode.Failure)
                        Debug.LogError("Failed to install URP: " + request.Error.message);

                    EditorApplication.update -= () => { };
                }
            };
        }
        private static void InstallHDRP()
        {
            // Use the Package Manager to add HDRP
            var request = Client.Add(hdrpIdentifier);
            EditorApplication.update += () =>
            {
                if (request.IsCompleted)
                {
                    if (request.Status == StatusCode.Success)
                    {

                    }
                    else if (request.Status == StatusCode.Failure)
                        Debug.LogError("Failed to install HDRP: " + request.Error.message);

                    EditorApplication.update -= () => { };
                }
            };
        }

        private static void InstallConversionPackage(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            AssetDatabase.ImportPackage(path, false);
        }
        
        private static void SetupBIRP()
        {
            GraphicsSettings.defaultRenderPipeline = null;
            QualitySettings.renderPipeline = null;

            UpdateLighting();
        }

        private static void SetupURP()
        {
            var folder = "Assets/Settings";
            if (!AssetDatabase.IsValidFolder(folder))
                AssetDatabase.CreateFolder("Assets", "Settings");

            var urpAssetType = Type.GetType("UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset, Unity.RenderPipelines.Universal.Runtime");
            var rendererDataType = Type.GetType("UnityEngine.Rendering.Universal.UniversalRendererData, Unity.RenderPipelines.Universal.Runtime");

            if (urpAssetType == null || rendererDataType == null)
            {
                Debug.LogWarning("[RPConvertor] URP not installed or not loaded yet.");
                return;
            }

            // Create renderer data
            var rendererData = ScriptableObject.CreateInstance(rendererDataType);
            string rendererPath = $"{folder}/ForwardRenderer.asset";
            AssetDatabase.CreateAsset(rendererData, rendererPath);

            // Create URP asset
            var urpAsset = ScriptableObject.CreateInstance(urpAssetType);
            string urpPath = $"{folder}/UniversalRenderPipelineAsset.asset";
            AssetDatabase.CreateAsset(urpAsset, urpPath);

            // Reflection fields
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var fieldList = urpAssetType.GetField("m_RendererDataList", flags)
                          ?? urpAssetType.GetField("m_RendererData", flags);
            var fieldIndex = urpAssetType.GetField("m_DefaultRendererIndex", flags);

            if (fieldList != null)
            {
                var array = Array.CreateInstance(rendererDataType, 1);
                array.SetValue(rendererData, 0);
                fieldList.SetValue(urpAsset, array);
            }

            if (fieldIndex != null)
                fieldIndex.SetValue(urpAsset, 0);

            EditorUtility.SetDirty(urpAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Assign URP asset
            var loadedURP = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(urpPath);
            if (loadedURP != null)
            {
                GraphicsSettings.defaultRenderPipeline = loadedURP;
                QualitySettings.renderPipeline = loadedURP;
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.LogError("[RPConvertor] Failed to assign URP asset.");
            }

            UpdateLighting();

            EditorUtility.RequestScriptReload();
        }   

        private static void UpdateLighting()
        {
            string skyBoxPath = AssetDatabase.GUIDToAssetPath(WizardGUIDs.defaultSkybox);

            Material skyBoxMat = AssetDatabase.LoadAssetAtPath(skyBoxPath, typeof(Material)) as Material;

            RenderSettings.skybox = skyBoxMat;
            DynamicGI.UpdateEnvironment();
        }
        private static void SetupHDRP()
        {
            var folder = "Assets/Settings";
            if (!AssetDatabase.IsValidFolder(folder))
                AssetDatabase.CreateFolder("Assets", "Settings");

            var hdrpAssetType = Type.GetType("UnityEngine.Rendering.HighDefinition.HDRenderPipelineAsset, Unity.RenderPipelines.HighDefinition.Runtime");
            var hdrpGlobalSettingsType = Type.GetType("UnityEngine.Rendering.HighDefinition.HDRenderPipelineGlobalSettings, Unity.RenderPipelines.HighDefinition.Runtime");

            if (hdrpAssetType == null || hdrpGlobalSettingsType == null)
            {
                Debug.LogWarning("[RPConvertor] HDRP not installed or not loaded yet.");
                return;
            }

            // Create HDRP Global Settings
            var globalSettings = ScriptableObject.CreateInstance(hdrpGlobalSettingsType);
            string globalSettingsPath = $"{folder}/HDRenderPipelineGlobalSettings.asset";
            AssetDatabase.CreateAsset(globalSettings, globalSettingsPath);

            // Create HDRP Asset
            var hdrpAsset = ScriptableObject.CreateInstance(hdrpAssetType);
            string hdrpPath = $"{folder}/HDRenderPipelineAsset.asset";
            AssetDatabase.CreateAsset(hdrpAsset, hdrpPath);

            EditorUtility.SetDirty(hdrpAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Assign HDRP asset
            var loadedHDRP = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(hdrpPath);
            if (loadedHDRP != null)
            {
                GraphicsSettings.defaultRenderPipeline = loadedHDRP;
                QualitySettings.renderPipeline = loadedHDRP;
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.LogError("[RPConvertor] Failed to assign HDRP asset.");
            }

            // Assign HDRP Global Settings
            var method = typeof(RenderPipelineManager).Assembly
                .GetType("UnityEngine.Rendering.HighDefinition.HDRenderPipelineGlobalSettings")
                ?.GetProperty("instance", BindingFlags.Public | BindingFlags.Static);
            if (method != null)
            {
                method.SetValue(null, globalSettings);
            }

            EditorUtility.RequestScriptReload();
        }
    }
}
#endif