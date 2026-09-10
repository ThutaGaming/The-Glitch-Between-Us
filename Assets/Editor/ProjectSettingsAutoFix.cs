using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// This project's ProjectSettings (GraphicsSettings.asset, InputManager.asset) have repeatedly
/// come back with a NULL render pipeline / empty input axes across sessions - the exact cause
/// wasn't pinned down (uncommitted ProjectSettings state reverting is the leading suspect), but
/// rather than fix it by hand every time it recurs, this just self-heals on every Editor load.
/// </summary>
[InitializeOnLoad]
internal static class ProjectSettingsAutoFix
{
    private const string UrpAssetPath = "Assets/Settings/PC_RPAsset.asset";

    static ProjectSettingsAutoFix()
    {
        EditorApplication.delayCall += FixRenderPipelineIfNeeded;
        EditorApplication.delayCall += FixInputAxesIfNeeded;
    }

    private static void FixRenderPipelineIfNeeded()
    {
        if (GraphicsSettings.defaultRenderPipeline != null) return;

        var urpAsset = AssetDatabase.LoadAssetAtPath<RenderPipelineAsset>(UrpAssetPath);
        if (urpAsset == null)
        {
            Debug.LogWarning($"ProjectSettingsAutoFix: render pipeline is null but {UrpAssetPath} wasn't found - can't self-heal.");
            return;
        }

        GraphicsSettings.defaultRenderPipeline = urpAsset;
        QualitySettings.renderPipeline = urpAsset;
        Debug.Log("ProjectSettingsAutoFix: render pipeline was null, restored to " + UrpAssetPath + ".");
    }

    private static void FixInputAxesIfNeeded()
    {
        var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset");
        if (assets == null || assets.Length == 0) return;

        var so = new SerializedObject(assets[0]);
        var axesProp = so.FindProperty("m_Axes");
        if (axesProp.arraySize > 0) return;

        AddAxis(axesProp, "Horizontal", "left", "right", "a", "d", 3f, 0.001f, 3f, true, 0, 0);
        AddAxis(axesProp, "Vertical", "down", "up", "s", "w", 3f, 0.001f, 3f, true, 0, 0);
        AddAxis(axesProp, "Mouse X", "", "", "", "", 0f, 0f, 0.1f, false, 1, 0);
        AddAxis(axesProp, "Mouse Y", "", "", "", "", 0f, 0f, 0.1f, false, 1, 1);
        so.ApplyModifiedProperties();

        Debug.Log("ProjectSettingsAutoFix: Input Manager axes were empty, restored Horizontal/Vertical/Mouse X/Mouse Y.");
    }

    private static void AddAxis(SerializedProperty axesProp, string name, string neg, string pos,
        string altNeg, string altPos, float gravity, float dead, float sensitivity, bool snap, int type, int axis)
    {
        int i = axesProp.arraySize;
        axesProp.InsertArrayElementAtIndex(i);
        var elem = axesProp.GetArrayElementAtIndex(i);
        elem.FindPropertyRelative("m_Name").stringValue = name;
        elem.FindPropertyRelative("negativeButton").stringValue = neg;
        elem.FindPropertyRelative("positiveButton").stringValue = pos;
        elem.FindPropertyRelative("altNegativeButton").stringValue = altNeg;
        elem.FindPropertyRelative("altPositiveButton").stringValue = altPos;
        elem.FindPropertyRelative("gravity").floatValue = gravity;
        elem.FindPropertyRelative("dead").floatValue = dead;
        elem.FindPropertyRelative("sensitivity").floatValue = sensitivity;
        elem.FindPropertyRelative("snap").boolValue = snap;
        elem.FindPropertyRelative("type").intValue = type;
        elem.FindPropertyRelative("axis").intValue = axis;
        elem.FindPropertyRelative("joyNum").intValue = 0;
    }
}
