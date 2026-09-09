#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Akila.FPSFramework.Internal
{
    [CustomEditor(typeof(SettingsPreset), true)]
    public class SettingsPresetEditor : Editor
    {
        private Vector2 scrollPosition;
        private ReorderableList optionsList;

        private void OnEnable()
        {
            SettingsPreset preset = (SettingsPreset)target;
            if (preset.sections == null || preset.sections.Count == 0) return;

            SetupOptionsList();
        }
        private void SetupOptionsList()
        {
            SettingsPreset preset = (SettingsPreset)target;
            if (preset.sections == null || preset.sections.Count == 0) return;

            var section = preset.sections[preset.currentSelectedSection];

            optionsList = new ReorderableList(section.options, typeof(SettingOption), true, true, true, true);

            optionsList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Options");
            };

            optionsList.elementHeightCallback = (int index) =>
            {
                var option = section.options[index];
                float height = EditorGUIUtility.singleLineHeight + 4f; // Foldout

                if (option.foldout)
                {
                    // Add lines for Name and Function fields
                    height += (EditorGUIUtility.singleLineHeight + 2f) * 2 + 4f;
                }

                return height;
            };

            optionsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var option = section.options[index];
                float lineHeight = EditorGUIUtility.singleLineHeight;
                float spacing = 2f;
                float margin = 10f; // Avoid drag handle overlap

                float x = rect.x + margin;
                float y = rect.y + 2f;
                float width = rect.width - margin;

                // --- Draw Foldout and IsActive Toggle ---
                Rect foldoutRect = new Rect(x, y, width - 60f, lineHeight);
                Rect toggleRect = new Rect(rect.x + rect.width - 55f, y, 55f, lineHeight);

                option.foldout = EditorGUI.Foldout(foldoutRect, option.foldout, option.name, true);
                option.isActive = EditorGUI.ToggleLeft(toggleRect, "Active", option.isActive);

                // --- Draw Inner Fields if Foldout is Open ---
                if (option.foldout)
                {
                    // Gray out content if not active
                    EditorGUI.BeginDisabledGroup(!option.isActive);

                    y += lineHeight + spacing;

                    // Name field
                    option.name = EditorGUI.TextField(
                        new Rect(x, y, width, lineHeight),
                        "Name",
                        option.name
                    );

                    y += lineHeight + spacing;

                    // --- Get available functions ---
                    BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                    MethodInfo[] methods = preset.GetType().GetMethods(flags);
                    List<string> methodNames = new List<string>();

                    foreach (MethodInfo method in methods)
                    {
                        if (!method.IsSpecialName && method.DeclaringType != typeof(object))
                        {
                            methodNames.Add(method.Name);
                        }
                    }

                    // --- Function dropdown ---
                    option.selectedFunction = EditorGUI.Popup(
                        new Rect(x, y, width, lineHeight),
                        "Function",
                        option.selectedFunction,
                        methodNames.ToArray()
                    );

                    if (methodNames.Count > 0)
                    {
                        option.functionName = methodNames[Mathf.Clamp(option.selectedFunction, 0, methodNames.Count - 1)];
                    }

                    EditorGUI.EndDisabledGroup();
                }
            };

            optionsList.onAddCallback = (ReorderableList list) =>
{
    var newOption = new SettingOption();

    int selectedIndex = list.index;
    int count = section.options.Count;

    // Determine source for duplication
    SettingOption sourceOption = null;

    if (selectedIndex >= 0 && selectedIndex < count)
        sourceOption = section.options[selectedIndex];
    else if (count > 0)
        sourceOption = section.options[count - 1];

    if (sourceOption != null)
    {
        newOption.name = sourceOption.name;
        newOption.functionName = sourceOption.functionName;
        newOption.selectedFunction = sourceOption.selectedFunction;
        newOption.foldout = sourceOption.foldout;
    }

    section.options.Add(newOption);
    list.index = section.options.Count - 1;

    EditorUtility.SetDirty(preset);
};


            optionsList.onRemoveCallback = (ReorderableList list) =>
            {
                if (list.index >= 0 && list.index < section.options.Count)
                {
                    section.options.RemoveAt(list.index);
                    EditorUtility.SetDirty(preset);
                }
            };

            optionsList.onReorderCallback = (ReorderableList list) =>
            {
                EditorUtility.SetDirty(preset);
            };
        }



        public override void OnInspectorGUI()
        {
            DrawSections();
        }

        private void DrawSections()
        {
            SettingsPreset preset = (SettingsPreset)target;
            SettingSection section = null;

            Undo.RecordObject(preset, $"Modified {preset.name}");

            List<string> sectionNames = new List<string>();

            //Add all section names to its list
            foreach (SettingSection settingSection in preset.sections)
            {
                sectionNames.Add(settingSection.name);
            }

            if (preset.sections != null && preset.sections.Count > 0) section = preset.sections[preset.currentSelectedSection];

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(preset.sections.Count <= 0);
            //Remove the currently selected section
            if (GUILayout.Button("-", GUILayout.MaxWidth(23)))
            {
                EditorUtility.SetDirty(preset);

                //Get the index of the selected section
                int index = preset.sections.IndexOf(section);

                //If the selected section is getting removed select the section next to it
                if (index == preset.currentSelectedSection && preset.currentSelectedSection > 0) preset.currentSelectedSection--;

                //Remove the section
                preset?.sections.Remove(section);
            }
            EditorGUI.EndDisabledGroup();

            if (preset.sections.Count > 0)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                //Update selected section
                preset.currentSelectedSection = GUILayout.Toolbar(preset.currentSelectedSection, sectionNames.ToArray());

                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                GUILayout.Toolbar(0, new string[] { "None" });
                EditorGUI.EndDisabledGroup();
            }

            //Add new section
            if (GUILayout.Button("+", GUILayout.MaxWidth(23)))
            {
                EditorUtility.SetDirty(preset);

                //Create new section
                SettingSection newSection = new SettingSection("New Section");

                preset?.sections.Add(newSection);

                //Keep selecting section
                preset.currentSelectedSection = preset.sections.IndexOf(newSection);
                preset.currentSelectedSection = Mathf.Clamp(preset.currentSelectedSection, 0, preset.sections.Count);
            }
            EditorGUILayout.EndHorizontal();

            if (preset.sections.Count <= 0)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.HelpBox("There are no sections in this settings preset, click on the + button above to add a new section.", MessageType.Info);
                EditorGUILayout.EndVertical();
            }

            EditorGUI.BeginChangeCheck();

            if (preset.sections.Count != 0)
            {
                preset.sections[preset.currentSelectedSection].name = EditorGUILayout.TextField("Section Name", preset.sections[preset.currentSelectedSection].name);

                EditorGUILayout.Space();

                if (preset.sections.Count > 0)
                    DrawOptions();

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(preset);
                }
            }
        }

        private int lastSelectedSection = -1;

        private void DrawOptions()
        {
            SettingsPreset preset = (SettingsPreset)target;
            if (preset.sections.Count == 0) return;

            if (lastSelectedSection != preset.currentSelectedSection)
            {
                SetupOptionsList();
                lastSelectedSection = preset.currentSelectedSection;
            }

            optionsList?.DoLayoutList();
        }

        private void MoveOptionToTop(SettingOption option)
        {
            SettingsPreset preset = (SettingsPreset)target;
            SettingSection section = null;

            if (preset.sections.Count > 0) section = preset.sections[preset.currentSelectedSection];

            section.options.MoveElement(section.options.IndexOf(option), 0);

            EditorUtility.SetDirty(preset);
        }

        private void MoveOptionToBottom(SettingOption option)
        {
            SettingsPreset preset = (SettingsPreset)target;
            SettingSection section = null;

            if (preset.sections.Count > 0) section = preset.sections[preset.currentSelectedSection];

            section.options.MoveElement(section.options.IndexOf(option), section.options.Count - 1);

            EditorUtility.SetDirty(preset);
        }

        private void MoveOptionUp(SettingOption option)
        {
            SettingsPreset preset = (SettingsPreset)target;
            SettingSection section = null;

            if (preset.sections.Count > 0) section = preset.sections[preset.currentSelectedSection];

            section.options.MoveElementUp(section.options.IndexOf(option));

            EditorUtility.SetDirty(preset);
        }

        private void MoveOptionDown(SettingOption option)
        {
            SettingsPreset preset = (SettingsPreset)target;
            SettingSection section = null;

            if (preset.sections.Count > 0) section = preset.sections[preset.currentSelectedSection];

            section.options.MoveElementDown(section.options.IndexOf(option));

            EditorUtility.SetDirty(preset);
        }

        private void RemoveOption(SettingOption option)
        {
            SettingsPreset preset = (SettingsPreset)target;
            SettingSection section = null;

            if (preset.sections.Count > 0) section = preset.sections[preset.currentSelectedSection];

            section.options.Remove(option);

            EditorUtility.SetDirty(preset);
        }
    }
}
#endif