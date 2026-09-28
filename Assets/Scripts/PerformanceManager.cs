using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

/// <summary>
/// Keeps the game at the same frame rate in every scene and on every laptop:
///  - Caps it at 60 fps with vSync off, so a fast machine or a 144 Hz screen doesn't run faster
///    than the machine the game was tuned on.
///  - When the graphics card can't hold 60, it lowers the 3D render resolution a step at a time
///    (the HUD stays sharp) and raises it again once there is headroom.
/// It also draws the FPS counter when Settings > Show FPS is on. It creates itself at startup,
/// so no scene needs wiring.
/// </summary>
public class PerformanceManager : MonoBehaviour
{
    public const int TargetFps = 60;

    private const float MinScale = 0.6f;
    private const float MaxScale = 1f;
    private const float ScaleStep = 0.05f;
    private const float SampleWindow = 1f;
    // GPU time per frame (ms): above Slow a step down is taken, below Fast a step up is tried.
    // A 60 fps frame is 16.7 ms, so both leave some margin.
    private const float GpuSlowMs = 15f;
    private const float GpuFastMs = 11f;
    // A frame this long is a loading hitch, not the scene's real cost.
    private const float HitchSeconds = 0.25f;

    private static PerformanceManager instance;

    /// <summary>Lets the render resolution follow the frame rate. Off keeps it where it is.</summary>
    public static bool AutoResolution = true;

    private UniversalRenderPipelineAsset pipeline;
    private float originalScale = 1f;
    private float scale = 1f;

    private float windowTime;
    private int windowFrames;
    private float windowWorst;
    private float cooldown;
    private int goodWindows;
    private int slowWindows;
    private float costBeforeStep;
    private bool resolutionDoesntHelp;
    private float shownFps;

    private readonly FrameTiming[] timings = new FrameTiming[12];
    private GUIStyle fpsStyle;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = TargetFps;
        if (instance != null) return;
        var go = new GameObject("~PerformanceManager");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<PerformanceManager>();
    }

    private void Awake()
    {
        pipeline = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        if (pipeline != null)
        {
            originalScale = pipeline.renderScale;
            scale = Mathf.Clamp(originalScale, MinScale, MaxScale);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        RestoreScale();
        if (instance == this) instance = null;
    }

    // The pipeline is a project asset: put its scale back so a Play session in the editor
    // doesn't leave it changed.
    private void OnApplicationQuit() => RestoreScale();

    private void RestoreScale()
    {
        if (pipeline != null) pipeline.renderScale = originalScale;
    }

    // Loading a scene stalls a few frames; start measuring afresh.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SkipGuiLayoutPass();
        windowTime = 0f;
        windowFrames = 0;
        windowWorst = 0f;
        goodWindows = 0;
        slowWindows = 0;
        costBeforeStep = 0f;
        // Whether a lower resolution helps depends on the scene; ask again in each one.
        resolutionDoesntHelp = false;
    }

    /// <summary>All the HUDs and prompts here draw with GUI, never GUILayout, so Unity's extra
    /// layout pass - one more call of every OnGUI each frame - is wasted work. Levels have
    /// dozens of them.</summary>
    private static void SkipGuiLayoutPass()
    {
        foreach (var behaviour in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            behaviour.useGUILayout = false;
    }

    private void Update()
    {
        float dt = Time.unscaledDeltaTime;
        windowTime += dt;
        windowFrames++;
        windowWorst = Mathf.Max(windowWorst, dt);
        if (cooldown > 0f) cooldown -= dt;
        if (windowTime < SampleWindow) return;

        float fps = windowFrames / windowTime;
        bool hitch = windowWorst > HitchSeconds;
        shownFps = fps;
        windowTime = 0f;
        windowFrames = 0;
        windowWorst = 0f;

        if (pipeline != null && !hitch && AutoResolution) AdjustScale(fps);
    }

    private void AdjustScale(float fps)
    {
        bool slow, fast;
        float gpuMs = AverageGpuMs();
        // Frame cost the last step down is judged by: GPU time when known, else frame time.
        float cost = gpuMs > 0f ? gpuMs : 1000f / Mathf.Max(1f, fps);
        if (gpuMs > 0f)
        {
            slow = gpuMs > GpuSlowMs;
            fast = gpuMs < GpuFastMs;
        }
        else
        {
            // No GPU timings on this machine: judge by the frame rate itself, a little more
            // cautiously, since a slow CPU looks the same from here.
            slow = fps < TargetFps - 10;
            fast = fps >= TargetFps - 2;
        }

        // A scene whose cost is geometry or CPU, not pixels, gets no faster at a lower
        // resolution - only blurrier. If the last step down didn't pay off, undo it and stop.
        if (costBeforeStep > 0f)
        {
            bool helped = cost < costBeforeStep * 0.93f;
            costBeforeStep = 0f;
            if (!helped)
            {
                SetScale(scale + ScaleStep);
                resolutionDoesntHelp = true;
                return;
            }
        }

        if (slow && scale > MinScale && !resolutionDoesntHelp)
        {
            if (++slowWindows < 2) return;   // one slow second can be a burst of effects
            costBeforeStep = cost;
            SetScale(scale - ScaleStep);
            slowWindows = 0;
            goodWindows = 0;
            cooldown = 8f;   // don't bounce straight back up
        }
        else if (fast && scale < MaxScale && cooldown <= 0f)
        {
            if (++goodWindows >= 3)
            {
                SetScale(scale + ScaleStep);
                goodWindows = 0;
            }
        }
        else if (!fast)
        {
            goodWindows = 0;
        }
        if (!slow) slowWindows = 0;
    }

    private void SetScale(float value)
    {
        scale = Mathf.Clamp(Mathf.Round(value * 100f) / 100f, MinScale, MaxScale);
        pipeline.renderScale = scale;
    }

    /// <summary>Average GPU time of the last few frames in ms, or 0 when the platform doesn't
    /// report it (needs Player Settings > Frame Timing Stats).</summary>
    private float AverageGpuMs()
    {
        FrameTimingManager.CaptureFrameTimings();
        uint count = FrameTimingManager.GetLatestTimings((uint)timings.Length, timings);
        double sum = 0;
        int used = 0;
        for (int i = 0; i < count; i++)
        {
            if (timings[i].gpuFrameTime <= 0) continue;
            sum += timings[i].gpuFrameTime;
            used++;
        }
        return used > 0 ? (float)(sum / used) : 0f;
    }

    private void OnGUI()
    {
        if (!GameSettings.ShowFps) return;
        if (fpsStyle == null)
        {
            fpsStyle = new GUIStyle(GUI.skin.label) { fontSize = Mathf.Max(14, Screen.height / 54), fontStyle = FontStyle.Bold };
            fpsStyle.normal.textColor = new Color(0.2f, 0.95f, 1f);
        }
        GUI.depth = -500;
        string text = Mathf.RoundToInt(shownFps) + " FPS";
        if (scale < MaxScale) text += "   RES " + Mathf.RoundToInt(scale * 100f) + "%";
        float h = fpsStyle.fontSize * 1.6f;
        GUI.Label(new Rect(12f, Screen.height - h - 8f, 400f, h), text, fpsStyle);
    }
}
