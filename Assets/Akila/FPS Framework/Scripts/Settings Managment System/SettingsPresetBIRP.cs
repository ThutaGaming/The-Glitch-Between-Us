using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Akila.FPSFramework.Examples
{

    [CreateAssetMenu(fileName = "New Settings Preset BIRP", menuName = "Akila/FPS Framework/Settings System/Settings Preset BIRP")]
    public class SettingsPresetBIRP : SettingsPreset
    {
        public void SetMSAA(int value)
        {
            if (value == 0) QualitySettings.antiAliasing = 8;
            if (value == 1) QualitySettings.antiAliasing = 4;
            if (value == 2) QualitySettings.antiAliasing = 2;
            if (value == 3) QualitySettings.antiAliasing = 0;
        }

        public void SetShadowResolution(int value)
        {
            ShadowResolution resolution = ShadowResolution.VeryHigh;

            if (value == 0) resolution = ShadowResolution.VeryHigh;
            if (value == 1) resolution = ShadowResolution.High;
            if (value == 2) resolution = ShadowResolution.Medium;
            if (value == 3) resolution = ShadowResolution.Low;

            QualitySettings.shadowResolution = resolution;
        }

        public void SetShadowDistance(int value)
        {
            float distance = 0;

            if (value == 0) distance = 200;
            if (value == 1) distance = 150;
            if (value == 2) distance = 100;
            if (value == 3) distance = 50;
            if (value == 4) distance = 30;
            if (value == 5) distance = 0;

            QualitySettings.shadowDistance = distance;
        }

        public void SetShadowCascade(int value)
        {
            int count = 0;
            if (value == 0) count = 4;
            if (value == 1) count = 2;
            if (value == 2) count = 0;

            QualitySettings.shadowCascades = count;
        }

        public void SetSoftShadow(int value)
        {
            if (value == 0) QualitySettings.shadows = ShadowQuality.All;
            if (value == 1) QualitySettings.shadows = ShadowQuality.HardOnly;
        }

        public void SetPostProcssing(int value)
        {
            float finalAmount = 1;

            PostProcessVolume volume = FindAnyObjectByType<PostProcessVolume>();

            //if (volume == null) return;

            if (value == 0) finalAmount = 1;
            if (value == 1) finalAmount = 0.8f;
            if (value == 2) finalAmount = 0.6f;
            if (value == 3) finalAmount = 0.5f;
            if (value == 4) finalAmount = 0.2f;
            if (value == 5) finalAmount = 0;

            volume.weight = finalAmount;
        }
    }
}