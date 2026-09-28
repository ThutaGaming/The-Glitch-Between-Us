using System.Collections.Generic;
using InfimaGames.LowPolyShooterPack;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// In-game pause menu, opened with Esc in any scene that has a Player: Continue, Settings (mouse
/// sensitivity, master volume, FPS counter) and Exit to the main menu. It creates itself once at
/// startup and lives across scene loads, so no scene needs wiring. The main menu's Setting button
/// opens the same settings page through <see cref="OpenSettings"/>, without pausing anything.
///
/// Pausing stops time and audio, frees the cursor, and switches off the player's look/move/
/// interact scripts - only the ones that were on, so a cutscene that had them off stays that way.
/// A screen that closes itself with Esc (the network terminal, the friends menu) calls
/// <see cref="ConsumeEscape"/> so the same press doesn't also open the menu.
///
/// Text is English because Unity's IMGUI cannot shape Burmese.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    private const string MainMenuScene = "Mainmenu Scene";
    private const float ReferenceHeight = 1080f;

    private static PauseMenu instance;
    private static int escapeConsumedFrame = -1;

    private enum Page { Main, Settings }

    private bool paused;
    private bool menuSettings;
    private System.Action onMenuSettingsClosed;
    private Page page;
    private float savedTimeScale = 1f;
    private CursorLockMode savedLockState;
    private bool savedCursorVisible;
    private Character infimaCharacter;
    private bool infimaWasLocked;
    private readonly List<Behaviour> frozen = new List<Behaviour>();

    private Texture2D pixel;
    private GUIStyle titleStyle, buttonStyle, labelStyle, valueStyle, hintStyle;

    public static bool IsPaused => instance != null && instance.paused;

    /// <summary>Call from a screen that just used Esc to close itself.</summary>
    public static void ConsumeEscape() => escapeConsumedFrame = Time.frameCount;

    /// <summary>Shows just the settings page (main menu). <paramref name="onClosed"/> runs when the
    /// player leaves it with Back or Esc.</summary>
    public static void OpenSettings(System.Action onClosed)
    {
        Bootstrap();
        instance.menuSettings = true;
        instance.page = Page.Settings;
        instance.onMenuSettingsClosed = onClosed;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null) return;
        var go = new GameObject("~PauseMenu");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<PauseMenu>();
    }

    private void Awake()
    {
        pixel = new Texture2D(1, 1) { hideFlags = HideFlags.HideAndDontSave };
        pixel.SetPixel(0, 0, Color.white);
        pixel.Apply();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (pixel != null) Destroy(pixel);
        if (instance == this) instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // A new scene always starts unpaused, whatever happened in the last one.
        if (paused) Unfreeze(restoreCursor: false);
        paused = false;
        menuSettings = false;
        onMenuSettingsClosed = null;
    }

    private void CloseMenuSettings()
    {
        GameSettings.Save();
        menuSettings = false;
        var closed = onMenuSettingsClosed;
        onMenuSettingsClosed = null;
        closed?.Invoke();
    }

    // LateUpdate, so every screen's own Esc handling (in Update) has already run this frame.
    private void LateUpdate()
    {
        bool esc = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        if (!esc || escapeConsumedFrame == Time.frameCount) return;

        if (menuSettings)
        {
            CloseMenuSettings();
            return;
        }

        if (paused)
        {
            if (page == Page.Settings) page = Page.Main;
            else Resume();
        }
        else if (CanPause())
        {
            Pause();
        }
    }

    private static bool CanPause()
    {
        if (SceneManager.GetActiveScene().name == MainMenuScene) return false;
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;
        var health = player.GetComponent<PlayerHealth>();
        return health == null || !health.IsDead;
    }

    private void Pause()
    {
        paused = true;
        page = Page.Main;

        savedTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        AudioListener.pause = true;

        savedLockState = Cursor.lockState;
        savedCursorVisible = Cursor.visible;

        var player = GameObject.FindGameObjectWithTag("Player");
        infimaCharacter = player != null ? player.GetComponent<Character>() : null;
        if (infimaCharacter != null)
        {
            // Infima reads look/move/fire only while its cursor is locked, so unlocking it is
            // what stops the level's player from turning or shooting behind the menu.
            infimaWasLocked = infimaCharacter.IsCursorLocked();
            infimaCharacter.SetCursorLocked(false);
        }

        frozen.Clear();
        if (player != null)
            foreach (var b in player.GetComponentsInChildren<Behaviour>())
                if (b.enabled && (b is MouseLook || b is PlayerMovement || b is PlayerInteractor))
                {
                    b.enabled = false;
                    frozen.Add(b);
                }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Resume()
    {
        paused = false;
        Unfreeze(restoreCursor: true);
        GameSettings.Save();
    }

    private void Unfreeze(bool restoreCursor)
    {
        Time.timeScale = savedTimeScale > 0f ? savedTimeScale : 1f;
        AudioListener.pause = false;

        foreach (var b in frozen) if (b != null) b.enabled = true;
        frozen.Clear();

        if (!restoreCursor) return;
        if (infimaCharacter != null) infimaCharacter.SetCursorLocked(infimaWasLocked);
        else
        {
            Cursor.lockState = savedLockState;
            Cursor.visible = savedCursorVisible;
        }
        infimaCharacter = null;
    }

    private void ExitToMainMenu()
    {
        paused = false;
        Unfreeze(restoreCursor: false);
        GameSettings.Save();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(MainMenuScene);
    }

    // ---------------- UI ----------------

    private void OnGUI()
    {
        if (!paused && !menuSettings) return;
        EnsureStyles();
        GUI.depth = -1000;

        float scale = Screen.height / ReferenceHeight;
        Matrix4x4 saved = GUI.matrix;
        GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
        float w = Screen.width / scale, h = ReferenceHeight;

        Fill(new Rect(0, 0, w, h), new Color(0f, 0f, 0f, 0.62f));

        const float panelW = 520f;
        float panelH = page == Page.Main ? 400f : 500f;
        var panel = new Rect((w - panelW) * 0.5f, (h - panelH) * 0.5f, panelW, panelH);
        Fill(panel, new Color(0.02f, 0.06f, 0.09f, 0.96f));
        Fill(new Rect(panel.x, panel.y, panel.width, 4f), new Color(0.2f, 0.95f, 1f, 1f));
        Fill(new Rect(panel.x, panel.yMax - 2f, panel.width, 2f), new Color(0.2f, 0.95f, 1f, 0.5f));

        if (page == Page.Main) DrawMain(panel);
        else DrawSettings(panel);

        // Swallow clicks outside the panel so nothing underneath reacts while paused.
        GUI.Button(new Rect(0, 0, w, h), GUIContent.none, GUIStyle.none);
        GUI.matrix = saved;
    }

    private void DrawMain(Rect panel)
    {
        GUI.Label(new Rect(panel.x, panel.y + 30f, panel.width, 50f), "PAUSED", titleStyle);

        float bw = 340f, bh = 58f, x = panel.x + (panel.width - bw) * 0.5f, y = panel.y + 115f;
        if (GUI.Button(new Rect(x, y, bw, bh), "CONTINUE", buttonStyle)) Resume();
        if (GUI.Button(new Rect(x, y + 78f, bw, bh), "SETTINGS", buttonStyle)) page = Page.Settings;
        if (GUI.Button(new Rect(x, y + 156f, bw, bh), "EXIT TO MAIN MENU", buttonStyle)) ExitToMainMenu();

        GUI.Label(new Rect(panel.x, panel.yMax - 44f, panel.width, 24f), "[Esc]  Continue", hintStyle);
    }

    private void DrawSettings(Rect panel)
    {
        GUI.Label(new Rect(panel.x, panel.y + 30f, panel.width, 50f), "SETTINGS", titleStyle);

        float x = panel.x + 60f, width = panel.width - 120f, y = panel.y + 115f;

        GUI.Label(new Rect(x, y, width, 28f), "MOUSE SENSITIVITY", labelStyle);
        GUI.Label(new Rect(x, y, width, 28f), GameSettings.Sensitivity.ToString("0.00") + "x", valueStyle);
        GameSettings.Sensitivity = Slider(new Rect(x, y + 38f, width, 24f), GameSettings.Sensitivity, GameSettings.MinSensitivity, GameSettings.MaxSensitivity);

        y += 100f;
        GUI.Label(new Rect(x, y, width, 28f), "MASTER VOLUME", labelStyle);
        GUI.Label(new Rect(x, y, width, 28f), Mathf.RoundToInt(GameSettings.MasterVolume * 100f) + "%", valueStyle);
        GameSettings.MasterVolume = Slider(new Rect(x, y + 38f, width, 24f), GameSettings.MasterVolume, 0f, 1f);

        y += 100f;
        GUI.Label(new Rect(x, y + 8f, width, 28f), "SHOW FPS", labelStyle);
        if (GUI.Button(new Rect(x + width - 120f, y, 120f, 44f), GameSettings.ShowFps ? "ON" : "OFF", buttonStyle))
            GameSettings.ShowFps = !GameSettings.ShowFps;

        float bw = 240f, bh = 54f;
        if (GUI.Button(new Rect(panel.x + (panel.width - bw) * 0.5f, panel.yMax - 100f, bw, bh), "BACK", buttonStyle))
        {
            if (menuSettings) CloseMenuSettings();
            else
            {
                GameSettings.Save();
                page = Page.Main;
            }
        }
    }

    private float Slider(Rect r, float value, float min, float max)
    {
        // Track and fill drawn by hand in the HUD's cyan; the stock slider on top does the input.
        var track = new Rect(r.x, r.y + r.height * 0.5f - 3f, r.width, 6f);
        Fill(track, new Color(0.08f, 0.2f, 0.25f, 1f));
        Fill(new Rect(track.x, track.y, track.width * Mathf.InverseLerp(min, max, value), track.height), new Color(0.2f, 0.95f, 1f, 1f));
        Color old = GUI.color;
        GUI.color = new Color(0.6f, 1f, 1f, 1f);
        float result = GUI.HorizontalSlider(r, value, min, max);
        GUI.color = old;
        return result;
    }

    private void Fill(Rect r, Color c)
    {
        Color old = GUI.color;
        GUI.color = c;
        GUI.DrawTexture(r, pixel);
        GUI.color = old;
    }

    private void EnsureStyles()
    {
        if (titleStyle != null) return;

        titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 38, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        titleStyle.normal.textColor = new Color(0.2f, 0.95f, 1f);

        buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 22, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        buttonStyle.normal.background = SolidTexture(new Color(0.05f, 0.14f, 0.18f, 1f));
        buttonStyle.hover.background = SolidTexture(new Color(0.09f, 0.34f, 0.42f, 1f));
        buttonStyle.active.background = SolidTexture(new Color(0.2f, 0.95f, 1f, 1f));
        buttonStyle.normal.textColor = Color.white;
        buttonStyle.hover.textColor = Color.white;
        buttonStyle.active.textColor = new Color(0.02f, 0.06f, 0.09f);

        labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 19, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleLeft };
        labelStyle.normal.textColor = new Color(0.85f, 0.95f, 1f);

        valueStyle = new GUIStyle(labelStyle) { alignment = TextAnchor.MiddleRight };
        valueStyle.normal.textColor = new Color(0.2f, 0.95f, 1f);

        hintStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
        hintStyle.normal.textColor = new Color(0.55f, 0.7f, 0.75f);
    }

    private static Texture2D SolidTexture(Color c)
    {
        var t = new Texture2D(1, 1) { hideFlags = HideFlags.HideAndDontSave };
        t.SetPixel(0, 0, c);
        t.Apply();
        return t;
    }
}
