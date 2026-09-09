using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Reflection;
using UnityEngine;
using System;
using Akila.FPSFramework.UI;
using System.Linq;
using TMPro;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/UI/Settings Menu/Setting Applier")]
    public class SettingApplier : MonoBehaviour
    {
        public string path = "Section/Option";
        public int selectedPathIndex;

        public SettingsManager settingsManager { get; set; }
        protected SettingApplier[] sisterAppliers { get; set; }

        #region UI Elements
        protected Slider slider;
        protected CarouselSelector carouselSelector;
        protected TMP_Dropdown dropdown;
        protected Toggle toggle;
        #endregion

        protected readonly string saveFileName = "Settings";
        protected 

        private void Awake()
        {
            settingsManager = FindFirstObjectByType<SettingsManager>();
            sisterAppliers = FindObjectsByType<SettingApplier>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);

            slider = this.SearchFor<Slider>();
            carouselSelector = this.SearchFor<CarouselSelector>();
            dropdown = this.SearchFor<TMP_Dropdown>();
            toggle = this.SearchFor<Toggle>();

            if (settingsManager.autoApply)
            {
                if (slider != null)
                {
                    slider.onValueChanged.AddListener(Apply);
                }

                if (carouselSelector != null)
                {
                    carouselSelector.onValueChange.AddListener(Apply);
                }

                if (dropdown != null)
                {
                    dropdown.onValueChanged.AddListener(Apply);
                }

                if (toggle != null)
                {
                    toggle.onValueChanged.AddListener(Apply);
                }
            }

            Load();
        }

        private void OnDestroy()
        {
            Save();
        }

        public void Save()
        {
            if (slider != null)
            {
                SaveSystem.SaveValue<float>(path, slider.value, saveFileName);
            }

            if (carouselSelector != null)
            {
                SaveSystem.SaveValue<int>(path, carouselSelector.value, saveFileName);
            }

            if(dropdown != null)
            {
                SaveSystem.SaveValue<int>(path, dropdown.value, saveFileName);
            }

            if(toggle !=null)
            {
                SaveSystem.SaveValue<bool>(path, toggle.isOn, saveFileName);
            }
        }

        public void SaveAll()
        {
            foreach(SettingApplier applier in sisterAppliers)
            {
                applier.Save();
            }
        }

        public void Load()
        {
            if(slider != null)
            {
                slider.value = SaveSystem.LoadOrSaveValue<float>(path, saveFileName, slider.value);

                Apply(slider.value);
            }

            if(carouselSelector != null)
            {
                carouselSelector.value = SaveSystem.LoadOrSaveValue<int>(path, saveFileName, carouselSelector.value);

                Apply(carouselSelector.value);
            }

            if(dropdown != null)
            {
                dropdown.value = SaveSystem.LoadOrSaveValue<int>(path, saveFileName, dropdown.value);

                Apply(carouselSelector.value);
            }

            if(toggle != null)
            {
                toggle.isOn = SaveSystem.LoadOrSaveValue<bool>(path, saveFileName, toggle.isOn);

                Apply(toggle.isOn);
            }
        }

        public void LoadAll()
        {
            foreach(SettingApplier applier in sisterAppliers)
            {
                applier.Load();
            }
        }

        public void Apply(float value)
        {
            Apply(obj: value);
        }

        public void Apply(int value)
        {
            Apply(obj: value);
        }

        public void Apply(bool value)
        {
            Apply(obj: value);
        }

        private void Apply(object obj)
        {
            settingsManager = FindFirstObjectByType<SettingsManager>(FindObjectsInactive.Include);

            if (!settingsManager) return;

            string fileName = null;

            if(GetOption(ref fileName) == null)
            {
                Debug.LogError("Option not set.", gameObject);

                return;
            }

            if (!GetOption(ref fileName).isActive)
            {
                return;
            }

            string functionName = GetOption(ref fileName).functionName;

            SettingsPreset preset = settingsManager.settingsPresets.ToList().Find(p => p.name == fileName);

            MethodInfo method = preset.GetType().GetMethod(functionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);


            if (method != null)
            {
                object[] parameters = null;

                if (obj.GetType() == typeof(float)) parameters = new object[] { (float)obj };
                if (obj.GetType() == typeof(int)) parameters = new object[] { (int)obj };
                if (obj.GetType() == typeof(bool)) parameters = new object[] { (bool)obj };

                try
                {
                    method.Invoke(preset, parameters);
                }
                catch (TargetInvocationException ex)
                {
                    // Unwrap the inner exception to get the actual exception thrown by the invoked method
                    Exception innerException = ex.InnerException;

                    if (innerException != null)
                    {
                        Debug.LogError($"An error occurred while invoking '{functionName}': {innerException.Message}\n{innerException.StackTrace}", gameObject);
                    }
                    else
                    {
                        Debug.LogError($"An unknown error occurred while invoking '{functionName}': {ex.Message}", gameObject);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Unexpected error while invoking '{functionName}': {ex.Message}\n{ex.StackTrace}", gameObject);
                }
            }
            else
            {
                Debug.LogError($"Function '{functionName}' not found in preset.", gameObject);
            }

            Save();
        }

        public void ApplyAll()
        {
            foreach(SettingApplier applier in sisterAppliers)
            {
                if (applier.slider) applier.Apply(applier.slider.value);
                if(applier.carouselSelector) applier.Apply(applier.carouselSelector.value);
                if(applier.dropdown) applier.Apply(applier.dropdown.value);
                if (applier.toggle) applier.Apply(applier.toggle.isOn);
            }
        }

        public SettingOption GetOption(ref string _fileName)
        {
            if (path == "None" || !settingsManager) return null;

            string[] pathParts = path.Split('/');

            if (pathParts.Length < 2)
            {
                Debug.LogError("Invalid path format. Please use 'Section/Option' format.", gameObject);
                return null;
            }


            string fileName = pathParts[0];
            string sectionName = pathParts[1];
            string optionName = pathParts[2];
            
            _fileName = fileName;

            SettingSection section = settingsManager.GetSection(fileName, sectionName);

            if (section == null)
            {
                Debug.LogError("Section '" + sectionName + "' not found.", gameObject);
                return null;
            }

            SettingOption option = section.GetOption(optionName);

            if (option == null)
            {
                Debug.LogError("Option '" + optionName + "' not found in section '" + sectionName + "'.", gameObject);
                return null;
            }

            return option;
        }
    }
}