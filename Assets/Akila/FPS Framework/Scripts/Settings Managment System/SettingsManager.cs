using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/UI/Settings Menu/Settings Manager")]
    public class SettingsManager : MonoBehaviour
    {
        public bool autoApply = true;
        public SettingsPreset[] settingsPresets;

        public static SettingsManager Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            foreach (var setting in settingsPresets)
            {
                setting?.OnAwake();
            }
        }

        private void Start()
        {
            foreach (var setting in settingsPresets)
            {
                setting?.OnStart();
            }
        }

        private void Update()
        {
            foreach (var setting in settingsPresets)
            {
                setting?.OnUpdate();
            }
        }

        private void OnApplicationQuit()
        {
            foreach (var setting in settingsPresets)
            {
                setting?.OnApplicationQuit();
            }
        }

        public SettingSection GetSection(string fileName, string sectionName)
        {
            var setting = settingsPresets.ToList().Find(p => p.name == fileName);

            if (setting == null)
            {
                Debug.LogError("SettingsPreset not set.", gameObject);

                return null;
            }

            foreach (SettingSection section in setting.sections)
            {
                if (section.name == sectionName)
                {
                    return section;
                }
            }

            return null;
        }
    }
}