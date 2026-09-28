using UnityEngine;

/// <summary>
/// Player settings changed from the pause menu, kept in PlayerPrefs so they survive restarts.
/// Sensitivity is a multiplier both look scripts apply (Infima's CameraLook in the levels,
/// MouseLook in Bedroom/School); volume is the master AudioListener volume. The main menu's
/// Setting button opens the same page.
/// </summary>
public static class GameSettings
{
    private const string SensitivityKey = "Settings.Sensitivity";
    private const string VolumeKey = "Settings.MasterVolume";
    private const string ShowFpsKey = "Settings.ShowFps";

    public const float MinSensitivity = 0.1f;
    public const float MaxSensitivity = 3f;

    private static float sensitivity = -1f;
    private static int showFps = -1;

    public static float Sensitivity
    {
        get
        {
            if (sensitivity < 0f) sensitivity = PlayerPrefs.GetFloat(SensitivityKey, 1f);
            return sensitivity;
        }
        set
        {
            sensitivity = Mathf.Clamp(value, MinSensitivity, MaxSensitivity);
            PlayerPrefs.SetFloat(SensitivityKey, sensitivity);
        }
    }

    public static float MasterVolume
    {
        get => PlayerPrefs.GetFloat(VolumeKey, 1f);
        set
        {
            float v = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(VolumeKey, v);
            AudioListener.volume = v;
        }
    }

    /// <summary>Draws the frame-rate counter (PerformanceManager) in the corner.</summary>
    public static bool ShowFps
    {
        get
        {
            if (showFps < 0) showFps = PlayerPrefs.GetInt(ShowFpsKey, 0);
            return showFps == 1;
        }
        set
        {
            showFps = value ? 1 : 0;
            PlayerPrefs.SetInt(ShowFpsKey, showFps);
        }
    }

    public static void Save() => PlayerPrefs.Save();

    // Apply the saved volume before the first scene plays anything.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplyOnStartup()
    {
        sensitivity = -1f;
        showFps = -1;
        AudioListener.volume = MasterVolume;
    }
}
