#if UNITY_EDITOR
using Akila.FPSFramework;
using Akila.FPSFramework.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static Akila.FPSFramework.RenderPipelineDetector;

namespace Akila.FPSFramework
{
    public class FPSFrameworkSettingsWindow : EditorWindow
    {
        private bool isResizing = false;
        private float sidebarWidth = 200f;
        private readonly float sidebarMinWidth = 100;
        private readonly float sidebarMaxWidth = 400f;
        private readonly float resizeHandleWidth = 4f;

        internal static CoreConfig preset;

        private int selectedTab = 0;
        private Vector2 sidebarScroll;
        private Vector2 contentScroll;

        private List<string> tabs = new List<string>
    {
            "General",
            "Animation",
            "Audio",
            "Editor"
        };

        private void OnEnable()
        {
            preset = FPSFrameworkSettings.preset;

            EditorApplication.update += RepaintWhenLoading;
        }

        private void RepaintWhenLoading()
        {
            if (versionChecker.IsChecking)
            {
                Repaint();
            }
        }

        [MenuItem(MenuPaths.Settings, false, 0)]
        public static void OpenWindow()
        {
            FPSFrameworkSettingsWindow window = GetWindow<FPSFrameworkSettingsWindow>("FPS Framework Settings");
            window.minSize = new Vector2(600, 400);
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);

            // Sidebar
            DrawSidebar();

            EditorGUI.EndDisabledGroup();

            // Resize handle
            HandleSidebarResize();

            // Content
            DrawContent();

            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                if (preset != null)
                    EditorUtility.SetDirty(preset);
            }
        }


        private void DrawSidebar()
        {
            if (preset == null) selectedTab = 0;

            EditorGUILayout.BeginVertical(GUILayout.Width(sidebarWidth));
            sidebarScroll = EditorGUILayout.BeginScrollView(sidebarScroll);

            for (int i = 0; i < tabs.Count; i++)
            {
                bool isSelected = (i == selectedTab);
                Rect rect = GUILayoutUtility.GetRect(sidebarWidth, 30, GUILayout.ExpandWidth(true));

                GUIStyle buttonStyle = new GUIStyle(EditorStyles.label)
                {
                    fontSize = 13,
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset(15, 10, 5, 5),
                    normal = { textColor = Color.white },
                    hover = { textColor = new Color(0.82f, 0.82f, 0.82f) },
                    fontStyle = isSelected ? FontStyle.Bold : FontStyle.Normal
                };

                if (isSelected)
                    EditorGUI.DrawRect(rect, new Color(0.2f, 0.4f, 0.8f, 0.5f));

                EditorGUI.BeginDisabledGroup(preset == null && i != 0);

                if (GUI.Button(rect, tabs[i], buttonStyle))
                    selectedTab = i;

                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        private void HandleSidebarResize()
        {
            Rect resizeRect = new Rect(sidebarWidth, 0, resizeHandleWidth, position.height);
            EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeHorizontal);

            // Draw handle line
            EditorGUI.DrawRect(new Rect(sidebarWidth, 0, 1, position.height), new Color(0.2f, 0.2f, 0.2f, 1f));

            Event e = Event.current;

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (resizeRect.Contains(e.mousePosition))
                    {
                        isResizing = true;
                        e.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (isResizing)
                    {
                        sidebarWidth = Mathf.Clamp(e.mousePosition.x, sidebarMinWidth, sidebarMaxWidth);
                        Repaint();
                    }
                    break;

                case EventType.MouseUp:
                    if (isResizing)
                    {
                        isResizing = false;
                        e.Use();
                    }
                    break;
            }
        }

        private void DrawContent()
        {
            EditorGUILayout.BeginVertical();
            contentScroll = EditorGUILayout.BeginScrollView(contentScroll, GUILayout.ExpandHeight(true));

            switch (selectedTab)
            {
                case 0: DrawGeneral(); break;
                case 1: DrawAnimationSettings(); break;
                case 2: DrawAudioSettings(); break;
                case 3: DrawEditorSettings(); break;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawGeneral()
        {
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 15 // Increased font size
            };


            EditorGUILayout.LabelField("General", titleStyle);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("About", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.TextField("Version", FPSFrameworkCore.version);
            EditorGUILayout.TextField("Tier", FPSFrameworkCore.IsFPSFrameworkProInstalled() ? "Pro" : "Regular");

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            EditorGUILayout.Space();

            //TODO: Use debug level in 2.1.0 and use the code below.

            /*
            EditorGUILayout.LabelField("Debugging", EditorStyles.boldLabel);
            preset.debugLevel = (MessageLevel)EditorGUILayout.EnumPopup("Debug Level", preset.debugLevel);

            EditorGUILayout.Space();
            */

            EditorGUILayout.LabelField("Updates", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            preset.automaticUpdatesChecking = EditorGUILayout.ToggleLeft("Automatic Updates Checking", preset.automaticUpdatesChecking);

            if (GUILayout.Button("Check For Updates"))
            {
                CheckForUpdates();
            }

            if (string.IsNullOrEmpty(versionChecker.StatusMessage) == false)
            {
                MessageType messageType = MessageType.None;

                if (versionChecker.WasFailure)
                    messageType = MessageType.Error;

                if (versionChecker.WasSuccessful)
                    messageType = MessageType.Info;

                if (versionChecker.IsUpdateAvailable)
                    messageType = MessageType.Warning;


                EditorGUILayout.BeginHorizontal();

                if (versionChecker.IsChecking)
                {

                    // Update frame every 100 ms
                    if (EditorApplication.timeSinceStartup - lastTime > 0.1)
                    {
                        spinnerFrame = (spinnerFrame + 1) % 12;
                        lastTime = EditorApplication.timeSinceStartup;
                        Repaint();
                    }

                    GUIContent spinner = EditorGUIUtility.IconContent("WaitSpin" + spinnerFrame.ToString("00"));
                    GUILayout.Label(spinner, GUILayout.MaxWidth(20));

                }

                EditorGUILayout.HelpBox(versionChecker.StatusMessage, messageType);

                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("RPs Convertor", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Current RP", RenderPipelineDetector.CurrentPiplineName);
            EditorGUI.EndDisabledGroup();
            
            preset.renderPipelineType = (CoreConfig.RenderPipelineType)EditorGUILayout.EnumPopup("Target Pipeline Type", preset.renderPipelineType);

            EditorGUILayout.HelpBox(
                "Converting settings will adjust your FPS Framework configuration to match the selected render pipeline. " +
                "Make sure you've selected the correct pipeline before proceeding, as some settings may be overwritten.",
                MessageType.Info
            );

            if (preset.renderPipelineType != CoreConfig.RenderPipelineType.BuiltIn)
            {
                bool canNotInstall = false;

                if (RenderPipelineDetector.CurrentPipeline == PipelineType.BuiltIn && preset.renderPipelineType == CoreConfig.RenderPipelineType.BuiltIn)
                    canNotInstall = true;

                if (RenderPipelineDetector.CurrentPipeline == PipelineType.UniversalRP && preset.renderPipelineType == CoreConfig.RenderPipelineType.UniversalRP)
                    canNotInstall = true;

                if (RenderPipelineDetector.CurrentPipeline == PipelineType.HighDefinitionRP && preset.renderPipelineType == CoreConfig.RenderPipelineType.HighDefinitionRP)
                    canNotInstall = true;

                if (RPConvertor.IsRPInstalled(preset.renderPipelineType))
                    canNotInstall = true;

                if (tryingToInstal)
                    canNotInstall = true;

                EditorGUI.BeginDisabledGroup(canNotInstall);

                if (GUILayout.Button("Install"))
                {
                    tryingToInstal = true;

                    bool confirm = EditorUtility.DisplayDialog(
                        "Confirm Installation",
                        "This process will configure your FPS Framework to work with the selected Render Pipeline.\n\n" +
                        "Some settings and assets may be automatically modified or replaced to ensure compatibility.\n\n" +
                        "Do you want to continue?",
                        "Proceed",
                        "Cancel"
                    );

                    if (confirm)
                    {
                        string currentGUID = "";

                        if (preset.renderPipelineType == CoreConfig.RenderPipelineType.BuiltIn)
                        {
                            currentGUID = WizardGUIDs.birpPackage;
                        }

                        if (preset.renderPipelineType == CoreConfig.RenderPipelineType.UniversalRP)
                        {
                            currentGUID = WizardGUIDs.urpPackage;
                        }

                        RPConvertor.InstallRenderPipline(preset.renderPipelineType);
                    }
                }


                EditorGUI.EndDisabledGroup();
            }

            bool canNotBeSetup = false;

            if (RPConvertor.IsRPInstalled(preset.renderPipelineType) == false)
                canNotBeSetup = true;

            EditorGUI.BeginDisabledGroup(canNotBeSetup);

            if (GUILayout.Button("Setup"))
            {
                bool confirm = EditorUtility.DisplayDialog(
                     "Confirm Setup",
                     "This process will configure your FPS Framework to work with the selected Render Pipeline.\n\n" +
                     "Important:\n" +
                     "All related prefabs, materials, scenes, and configuration files will be overwritten or replaced to match the selected pipeline.\n\n" +
                     "It is strongly recommended to back up your project before continuing.\n\n" +
                     "Do you want to proceed?",
                     "Continue",
                     "Cancel"
                );

                if (confirm)
                {
                    string currentGUID = "";

                    if (preset.renderPipelineType == CoreConfig.RenderPipelineType.BuiltIn)
                    {
                        currentGUID = WizardGUIDs.birpPackage;
                    }

                    if (preset.renderPipelineType == CoreConfig.RenderPipelineType.UniversalRP)
                    {
                        currentGUID = WizardGUIDs.urpPackage;
                    }

                    if(preset.renderPipelineType == CoreConfig.RenderPipelineType.HighDefinitionRP)
                    {
                        currentGUID = WizardGUIDs.hdrpPackage;
                    }

                    RPConvertor.SetupRenderPipline(preset.renderPipelineType, currentGUID);
                }
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Saves", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Deletes all FPSF save data (all JSON files in Application.persistentDataPath).",
                MessageType.Info
            );

            if (GUILayout.Button("Delete All Saves"))
            {
                bool deleteAll = EditorUtility.DisplayDialog(
                    "Confirm Deletion",
                    "This will permanently delete all saved data. Are you sure you want to continue?",
                    "Delete All",
                    "Cancel"
                );

                if (deleteAll)
                    SaveSystem.DeleteAllSaves();
            }



            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Reset Framework", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox("Reseting all settings will reset all the settings for FPSF. Before reseting make sure to not reset on accident.", MessageType.Info);

            if (GUILayout.Button("Reset All Settings"))
            {
                bool reset = EditorUtility.DisplayDialog(
                    "Confirm Reset",
                    "This will restore all settings to their default values. Are you sure you want to continue?",
                    "Reset Settings",
                    "Cancel"
                );

                if (reset)
                    preset.ResetAllSettings();
            }
        }

        private int spinnerFrame = 0;
        private double lastTime = 0;

        private void DrawAnimationSettings()
        {
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 15 // Increased font size
            };

            EditorGUILayout.LabelField("Animation", titleStyle);
            EditorGUILayout.Space();

            preset.globalAnimationWeight = EditorGUILayout.Slider("Global Weight", preset.globalAnimationWeight, 0f, 1f);
            preset.globalAnimationSpeed = EditorGUILayout.Slider("Global Speed", preset.globalAnimationSpeed, 0f, 1f);
            preset.maxAnimationFramerate = EditorGUILayout.IntField("Max Framerate", preset.maxAnimationFramerate);
        }

        private void DrawAudioSettings()
        {
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 15 // Increased font size
            };

            EditorGUILayout.LabelField("Audio", titleStyle);
            EditorGUILayout.Space();

            preset.masterAudioVolume = EditorGUILayout.Slider("Master Volume", preset.masterAudioVolume, 0f, 1f);
        }

        private void DrawEditorSettings()
        {
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 15 // Increased font size
            };

            EditorGUILayout.LabelField("Editor", titleStyle);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling || Application.isPlaying);

            preset.shortenMenus = EditorGUILayout.Toggle("Shorten Menus", preset.shortenMenus);

            EditorGUI.EndDisabledGroup();

            if (EditorApplication.isCompiling || Application.isPlaying)
            {
                EditorGUILayout.HelpBox("'Shorten Menus' is disabled during script compilation and in play mode because it requires recompilation.", MessageType.Info);
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (preset.shortenMenus)
                    FPSFrameworkCore.AddCustomDefineSymbol("FPS_FRAMEWORK_SHORTEN_MENUS");
                else
                    FPSFrameworkCore.RemoveCustomDefineSymbol("FPS_FRAMEWORK_SHORTEN_MENUS");
            }
        }

        private async void CheckForUpdates()
        { 
            versionChecker = new FPSFrameworkVersionChecker();

            await versionChecker.CheckForUpdateAsync();

            versionChecker.OnCheckFinished += Repaint;
        }


        private FPSFrameworkVersionChecker versionChecker = new FPSFrameworkVersionChecker();
        private bool tryingToInstal;
    }
}
#endif