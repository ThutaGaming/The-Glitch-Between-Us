using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Sitting in `chair` cuts to a full-screen flat 2D "using the computer" interface - background,
/// a monitor drawn entirely in code, and a login card inside its screen. This replaces the 3D
/// view outright rather than projecting UI onto the 3D monitor mesh, the way Summertime Saga
/// (and similar games) handle "use the computer": a dedicated illustrated interface, not a
/// world-space object viewed through the game camera. Esc ends it and hands control back.
/// </summary>
public class ComputerUseSequence : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SittableChair chair;
    [SerializeField] private MouseLook playerLook;

    [Header("Transition")]
    [SerializeField] private float fadeDuration = 0.35f;

    [Header("Monitor illustration")]
    [SerializeField] private Color pageBackgroundColor = new Color(0.16f, 0.16f, 0.18f, 1f);
    [SerializeField] private Color monitorBezelColor = new Color(0.14f, 0.15f, 0.18f, 1f);
    [SerializeField] private Color monitorStandColor = new Color(0.10f, 0.11f, 0.13f, 1f);
    [SerializeField] private Color screenColor = new Color(0.66f, 0.70f, 0.75f, 1f);
    [Tooltip("Monitor width as a fraction of the narrower of screen width / (screen height * 1.35), so it stays a sane size on both wide and narrow windows.")]
    [Range(0.3f, 0.85f)]
    [SerializeField] private float monitorWidthFraction = 0.62f;

    [Header("Login card")]
    [SerializeField] private string playerName = "Thuta";
    [SerializeField] private Color avatarColor = new Color(0.35f, 0.42f, 0.55f, 1f);
    [SerializeField] private float loadDuration = 2.5f;
    [SerializeField] private string loadingText = "Loading...";
    [SerializeField] private string readyText = "Ready to play!";
    [Tooltip("Extra pause on \"Ready to play!\" before the desktop takes over.")]
    [SerializeField] private float readyHoldDuration = 0.7f;

    [Header("Desktop (shown after login) - wallpaper + logo")]
    [SerializeField] private Color gradientTopColor = new Color(0.35f, 0.85f, 0.90f, 1f);
    [SerializeField] private Color gradientBottomColor = new Color(0.30f, 0.45f, 0.95f, 1f);
    [SerializeField] private Color logoColor = new Color(0.80f, 0.98f, 0.96f, 1f);
    [Tooltip("Logo width as a fraction of screen width.")]
    [Range(0.1f, 0.5f)]
    [SerializeField] private float logoWidthFraction = 0.24f;

    [Header("Desktop - taskbar")]
    [SerializeField] private Color taskbarColor = new Color(0.88f, 0.89f, 0.94f, 0.92f);
    [Tooltip("How many small app-icon squares to scatter across the taskbar - purely decorative " +
             "except for the one at gamesIconIndex.")]
    [Range(4, 20)]
    [SerializeField] private int taskbarIconCount = 13;
    [SerializeField] private int gamesIconIndex = 4;
    [SerializeField] private Color gamesIconColor = new Color(0.25f, 0.65f, 0.35f, 1f);
    [SerializeField] private string weatherText = "72°";

    [Header("Desktop - window (opens when the games icon is clicked)")]
    [SerializeField] private string windowTitle = "My Games";
    [SerializeField] private Color titleBarColor = new Color(0.16f, 0.18f, 0.24f, 1f);
    [SerializeField] private Color windowBodyColor = new Color(0.92f, 0.93f, 0.95f, 1f);
    [SerializeField] private string windowBodyText = "(nothing installed yet)";
    [Tooltip("Window size as a fraction of the desktop area, per axis.")]
    [Range(0.4f, 0.95f)]
    [SerializeField] private float windowSizeFraction = 0.78f;

    [Header("Desktop shortcut - Mail")]
    [SerializeField] private string mailShortcutLabel = "Mail";
    [Tooltip("Shortcut position as a fraction of the desktop area (0,0 = top-left).")]
    [SerializeField] private Vector2 mailShortcutPosition = new Vector2(0.06f, 0.12f);
    [SerializeField] private string mailWindowTitle = "Mail";

    [Header("Story Beat - References")]
    [SerializeField] private DialogueHUD dialogue;
    [SerializeField] private MissionHUD mission;

    [Header("Story Beat - Toast notification (fires once, shortly after the desktop first shows)")]
    [SerializeField] private string toastSender = "Developer";
    [SerializeField] private string toastSubject = "Beta Testing";
    [SerializeField] private float toastDelay = 1.2f;
    [SerializeField] private float toastHold = 3.5f;
    [SerializeField] private float toastFadeDuration = 0.35f;

    [Header("Story Beat - Dialogue & objectives")]
    [SerializeField] private string wonderWhoLine = "Huh? Wonder who's emailing me.";
    [SerializeField] private string openEmailObjective = "Open email";
    [SerializeField] private string interestedLine = "\"Protocol: ESCAPE\"... okay, that actually looks pretty interesting.";
    [SerializeField] private string clickLinkObjective = "Click the link";

    [Header("Story Beat - Email content")]
    [SerializeField] private string emailSenderName = "Developer Minn Chit";
    [SerializeField] private string emailSubject = "Beta Testing Invite";
    [TextArea]
    [SerializeField] private string emailBodyIntro =
        "Hey, we're looking for a few players to beta test our new game before launch. " +
        "Follow the link below if you're interested.";
    [SerializeField] private string emailLinkLabel = "PLAY: Protocol: ESCAPE ->";

    [Header("Story Beat - Glitch / faint (plays after the link is clicked)")]
    [SerializeField] private float glitchBuildDuration = 2.6f;
    [Tooltip("How far into the glitch (seconds) Thuta's line plays - after the chaos is already visible, not right at the start.")]
    [SerializeField] private float glitchLineDelay = 0.9f;
    [SerializeField] private string glitchLine = "Wait— what's happening?!";
    [SerializeField] private float glitchLineHold = 1.2f;
    [SerializeField] private float blackoutFadeDuration = 0.9f;
    [Range(2, 40)]
    [SerializeField] private int glitchStreakCount = 24;
    [Tooltip("Max random screen-shake offset in pixels at full glitch intensity.")]
    [SerializeField] private float glitchShakeStrength = 14f;
    [Tooltip("Fires once the screen has gone fully black - hook the next story beat to this.")]
    public UnityEvent onPlayerFainted;

    [Header("Screen glow (used for both the mail shortcut and the link, while their objective is active)")]
    [SerializeField] private Color glowColor = new Color(0.35f, 0.55f, 1f, 1f);
    [SerializeField] private float glowPulseSpeed = 2.5f;

    private bool isUsing;
    private bool loaded;
    private bool desktopShown;
    private bool windowOpen;
    private bool mailOpen;
    private enum MailView { List, Detail }
    private MailView mailView;
    private float loadElapsed;
    private float readyElapsed;
    private float fadeAlpha;

    private bool storyTriggered;
    private float toastAlpha;
    private bool mailIconGlowing;
    private bool linkGlowing;
    private bool linkClicked;
    private bool isGlitching;
    private float glitchIntensity;
    private float blackoutAlpha;

    private static readonly Color[] GlitchStreakColors =
    {
        Color.white, new Color(0.45f, 0.90f, 1f, 1f), new Color(1f, 0.30f, 0.80f, 1f), new Color(0.55f, 0.70f, 1f, 1f),
    };

    private GUIStyle nameStyle;
    private GUIStyle statusStyle;
    private GUIStyle clockStyle;
    private GUIStyle weatherStyle;
    private GUIStyle windowTitleStyle;
    private GUIStyle closeButtonStyle;
    private GUIStyle windowBodyStyle;
    private GUIStyle shortcutLabelStyle;
    private GUIStyle mailFieldLabelStyle;
    private GUIStyle toastTitleStyle;
    private GUIStyle toastSubjectStyle;
    private GUIStyle inboxSenderStyle;
    private GUIStyle inboxSubjectStyle;
    private GUIStyle emailBodyStyle;
    private GUIStyle linkStyle;
    private Texture2D solid;
    private Texture2D avatarTex;
    private Texture2D gradientTex;
    private Color[] iconColors;
    private Texture2D[] iconGlyphs;
    private Texture2D wifiIconTex;
    private Texture2D speakerIconTex;
    private Texture2D mailShortcutIconTex;

    private static readonly Color[] IconPalette =
    {
        new Color(0.85f, 0.35f, 0.35f, 1f), new Color(0.35f, 0.55f, 0.85f, 1f),
        new Color(0.95f, 0.70f, 0.25f, 1f), new Color(0.55f, 0.35f, 0.80f, 1f),
        new Color(0.30f, 0.70f, 0.70f, 1f), new Color(0.85f, 0.45f, 0.65f, 1f),
        new Color(0.45f, 0.75f, 0.35f, 1f), new Color(0.35f, 0.40f, 0.85f, 1f),
    };

    /// <summary>Simple silhouette shapes drawn per-pixel (see GlyphCovers) so each taskbar tile
    /// reads as a distinct generic app icon (envelope, folder, gear...) instead of a flat square -
    /// original blocky glyphs, not any real app's actual icon.</summary>
    private enum GlyphKind { Circle, Ring, Triangle, Envelope, Folder, Document, Magnifier, Gear, House, Bubble, Star, Cloud, Wifi, Speaker }

    private static readonly GlyphKind[] GlyphCycle =
    {
        GlyphKind.Circle, GlyphKind.Envelope, GlyphKind.Folder, GlyphKind.Document,
        GlyphKind.Magnifier, GlyphKind.Bubble, GlyphKind.House, GlyphKind.Ring,
        GlyphKind.Star, GlyphKind.Cloud, GlyphKind.Gear,
    };

    private void Awake()
    {
        solid = BuildSolidTexture();
        avatarTex = BuildCircleTexture(128, avatarColor);
        gradientTex = BuildGradientTexture(64, gradientTopColor, gradientBottomColor);
        wifiIconTex = BuildGlyphTexture(28, GlyphKind.Wifi);
        speakerIconTex = BuildGlyphTexture(28, GlyphKind.Speaker);
        mailShortcutIconTex = BuildGlyphTexture(56, GlyphKind.Envelope);

        iconColors = new Color[taskbarIconCount];
        iconGlyphs = new Texture2D[taskbarIconCount];
        for (int i = 0; i < taskbarIconCount; i++)
        {
            iconColors[i] = i == gamesIconIndex ? gamesIconColor : IconPalette[i % IconPalette.Length];
            GlyphKind kind = i == gamesIconIndex ? GlyphKind.Triangle : GlyphCycle[i % GlyphCycle.Length];
            iconGlyphs[i] = BuildGlyphTexture(28, kind);
        }
    }

    private void OnEnable()
    {
        if (chair != null) chair.onSat += HandleSat;
    }

    private void OnDisable()
    {
        if (chair != null) chair.onSat -= HandleSat;
    }

    private void OnDestroy()
    {
        if (solid != null) Destroy(solid);
        if (avatarTex != null) Destroy(avatarTex);
        if (gradientTex != null) Destroy(gradientTex);
        if (wifiIconTex != null) Destroy(wifiIconTex);
        if (speakerIconTex != null) Destroy(speakerIconTex);
        if (mailShortcutIconTex != null) Destroy(mailShortcutIconTex);
        if (iconGlyphs != null)
        {
            foreach (var t in iconGlyphs)
            {
                if (t != null) Destroy(t);
            }
        }
    }

    private void HandleSat()
    {
        StartCoroutine(EnterRoutine());
    }

    private IEnumerator EnterRoutine()
    {
        if (chair != null) chair.SetStandUpBlocked(true);

        yield return Fade(0f, 1f, fadeDuration);

        if (playerLook != null) playerLook.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        loaded = false;
        desktopShown = false;
        windowOpen = false;
        mailOpen = false;
        mailView = MailView.List;
        loadElapsed = 0f;
        readyElapsed = 0f;
        isUsing = true;
    }

    private void Update()
    {
        if (!isUsing) return;

        if (!loaded)
        {
            loadElapsed += Time.deltaTime;
            if (loadElapsed >= loadDuration) loaded = true;
            return;
        }

        if (!desktopShown)
        {
            readyElapsed += Time.deltaTime;
            if (readyElapsed >= readyHoldDuration)
            {
                desktopShown = true;
                // Bare desktop, like a real login - the games window only opens once the player
                // clicks its taskbar icon.

                if (!storyTriggered)
                {
                    storyTriggered = true;
                    StartCoroutine(StoryRoutine());
                }
            }
        }
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadeAlpha = Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));
            yield return null;
        }
        fadeAlpha = to;
    }

    /// <summary>
    /// The one-time beta-testing email beat: a toast notification, Thuta wondering who sent it,
    /// an "Open email" objective with a glowing mail shortcut, the email itself (from "Developer
    /// Minn Chit", about a game called "Protocol: ESCAPE"), a "Click the link" objective with a
    /// glowing link, and finally the glitch/faint cutscene once the link is clicked. There's no
    /// stepping away from the computer once seated (no Escape handler), so this always runs to
    /// completion in one sitting.
    /// </summary>
    private IEnumerator StoryRoutine()
    {
        ResolveStoryReferences();

        yield return new WaitForSeconds(toastDelay);
        yield return ShowToast();

        if (dialogue != null) dialogue.Say("Thuta", wonderWhoLine);
        yield return null;
        yield return new WaitWhile(() => dialogue != null && dialogue.IsPlaying);

        if (mission != null) mission.SetObjective(openEmailObjective);
        mailIconGlowing = true;

        yield return new WaitUntil(() => mailOpen);
        mailIconGlowing = false;

        yield return new WaitUntil(() => mailView == MailView.Detail);
        if (mission != null) mission.CompleteObjective();

        if (dialogue != null) dialogue.Say("Thuta", interestedLine);
        yield return null;
        yield return new WaitWhile(() => dialogue != null && dialogue.IsPlaying);

        if (mission != null) mission.SetObjective(clickLinkObjective);
        linkGlowing = true;

        yield return new WaitUntil(() => linkClicked);
        linkGlowing = false;
        if (mission != null) mission.CompleteObjective();

        yield return GlitchAndFaintRoutine();
    }

    private void ResolveStoryReferences()
    {
        if (dialogue == null) dialogue = DialogueHUD.Instance != null
            ? DialogueHUD.Instance
            : FindFirstObjectByType<DialogueHUD>();

        if (mission == null) mission = MissionHUD.Instance != null
            ? MissionHUD.Instance
            : FindFirstObjectByType<MissionHUD>();
    }

    private IEnumerator ShowToast()
    {
        float t = 0f;
        while (t < toastFadeDuration)
        {
            t += Time.deltaTime;
            toastAlpha = Mathf.Clamp01(t / toastFadeDuration);
            yield return null;
        }
        toastAlpha = 1f;

        yield return new WaitForSeconds(toastHold);

        t = 0f;
        while (t < toastFadeDuration)
        {
            t += Time.deltaTime;
            toastAlpha = 1f - Mathf.Clamp01(t / toastFadeDuration);
            yield return null;
        }
        toastAlpha = 0f;
    }

    /// <summary>Rough, shaking chromatic glitch that builds up while Thuta reacts partway
    /// through, then a hard fade to black once he's said his line - the "player faints" beat.
    /// onPlayerFainted fires once the screen is fully black, for whatever comes next to hook
    /// onto.</summary>
    private IEnumerator GlitchAndFaintRoutine()
    {
        isGlitching = true;
        bool linePlayed = false;

        float t = 0f;
        while (t < glitchBuildDuration)
        {
            t += Time.deltaTime;
            glitchIntensity = Mathf.Clamp01(t / glitchBuildDuration);

            if (!linePlayed && t >= glitchLineDelay)
            {
                linePlayed = true;
                if (dialogue != null) dialogue.Say("Thuta", glitchLine, glitchLineHold);
            }

            yield return null;
        }

        if (!linePlayed && dialogue != null) dialogue.Say("Thuta", glitchLine, glitchLineHold);

        // Let him finish the line - still shaking/glitching at full intensity - before the
        // screen actually goes black.
        yield return new WaitWhile(() => dialogue != null && dialogue.IsPlaying);

        t = 0f;
        while (t < blackoutFadeDuration)
        {
            t += Time.deltaTime;
            blackoutAlpha = Mathf.Clamp01(t / blackoutFadeDuration);
            yield return null;
        }
        blackoutAlpha = 1f;
        isGlitching = false;

        onPlayerFainted?.Invoke();
    }

    private void OnGUI()
    {
        if (fadeAlpha <= 0.001f) return;

        EnsureStyles();

        // Full-screen backdrop - covers the 3D view entirely once faded in. Fading its own alpha
        // (rather than cutting instantly) is the whole transition; there is no 3D camera move.
        GUI.color = new Color(pageBackgroundColor.r, pageBackgroundColor.g, pageBackgroundColor.b, fadeAlpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), solid);
        GUI.color = Color.white;

        // Skip the illustration mid-fade so it doesn't flash in half-transparent.
        if (fadeAlpha < 0.98f) return;

        // The shake only jitters the "picture" (monitor + toast + streaks), never the full-screen
        // backdrop/blackout rects below - those stay screen-locked so the shake can never reveal
        // a gap at the screen edge.
        Matrix4x4 savedMatrix = GUI.matrix;
        if (isGlitching && glitchShakeStrength > 0f) ApplyGlitchShake();

        DrawMonitorIllustrationAndLogin();
        if (toastAlpha > 0.001f) DrawEmailToast();
        if (isGlitching) DrawGlitchStreaks();

        GUI.matrix = savedMatrix;

        if (blackoutAlpha > 0.001f) DrawBlackout();
    }

    private void ApplyGlitchShake()
    {
        float shake = glitchShakeStrength * glitchIntensity;
        Vector2 offset = new Vector2((Random.value - 0.5f) * 2f * shake, (Random.value - 0.5f) * 2f * shake);
        GUI.matrix = Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one) * GUI.matrix;
    }

    /// <summary>A toast card near the top-right of the actual screen (not the illustrated
    /// monitor) - like an OS notification popping up over whatever the player is doing.</summary>
    private void DrawEmailToast()
    {
        float width = 320f;
        float height = 78f;
        float margin = 24f;
        Rect cardRect = new Rect(Screen.width - width - margin, margin, width, height);

        GUI.color = new Color(0.10f, 0.11f, 0.14f, 0.92f * toastAlpha);
        GUI.DrawTexture(cardRect, solid);
        GUI.color = new Color(glowColor.r, glowColor.g, glowColor.b, toastAlpha);
        GUI.DrawTexture(new Rect(cardRect.x, cardRect.y, 4f, cardRect.height), solid);

        float iconSize = height * 0.5f;
        Rect iconRect = new Rect(cardRect.x + 14f, cardRect.y + (height - iconSize) * 0.5f, iconSize, iconSize);
        GUI.color = new Color(1f, 1f, 1f, toastAlpha);
        if (mailShortcutIconTex != null) GUI.DrawTexture(iconRect, mailShortcutIconTex);

        Rect titleRect = new Rect(iconRect.xMax + 12f, cardRect.y + 14f, cardRect.width - iconSize - 40f, 20f);
        Rect subjectRect = new Rect(iconRect.xMax + 12f, cardRect.y + 38f, cardRect.width - iconSize - 40f, 20f);
        GUI.color = new Color(1f, 1f, 1f, toastAlpha);
        GUI.Label(titleRect, toastSender + " sent you a message", toastTitleStyle);
        GUI.color = new Color(0.7f, 0.75f, 0.85f, toastAlpha);
        GUI.Label(subjectRect, toastSubject, toastSubjectStyle);

        GUI.color = Color.white;
    }

    /// <summary>Chromatic streaks plus chunkier "corruption" blocks that build up with
    /// glitchIntensity - rougher than a plain fade, meant to read as the screen actively
    /// breaking rather than just dimming.</summary>
    private void DrawGlitchStreaks()
    {
        int streaks = Mathf.RoundToInt(Mathf.Lerp(3, glitchStreakCount, glitchIntensity));
        for (int i = 0; i < streaks; i++)
        {
            float rx = Random.value;
            float ry = Random.value;
            bool chunky = Random.value < 0.35f;
            float rw = chunky ? Mathf.Lerp(60f, 220f, Random.value) : Mathf.Lerp(60f, 320f, Random.value);
            float rh = chunky ? Mathf.Lerp(30f, 140f, Random.value) : Mathf.Lerp(3f, 16f, Random.value);
            Color c = GlitchStreakColors[Random.Range(0, GlitchStreakColors.Length)];
            GUI.color = new Color(c.r, c.g, c.b, Mathf.Lerp(0.35f, 1f, glitchIntensity) * (chunky ? 0.7f : 1f));
            GUI.DrawTexture(new Rect(rx * Screen.width, ry * Screen.height, rw, rh), solid);
        }

        // A harsh strobing flash under everything, so the whole view feels like it's breaking.
        float flash = (Mathf.Sin(Time.unscaledTime * 55f) + 1f) * 0.5f;
        GUI.color = new Color(1f, 1f, 1f, flash * glitchIntensity * 0.25f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), solid);
        GUI.color = Color.white;
    }

    private void DrawBlackout()
    {
        GUI.color = new Color(0f, 0f, 0f, blackoutAlpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), solid);
        GUI.color = Color.white;
    }

    /// <summary>
    /// The monitor itself, drawn as flat shapes (bezel + stand), not projected from the 3D mesh -
    /// this is a dedicated 2D interface, so it draws its own version of "a monitor".
    /// </summary>
    private void DrawMonitorIllustrationAndLogin()
    {
        float monitorWidth = Mathf.Min(Screen.width * monitorWidthFraction, Screen.height * 1.35f);
        float monitorHeight = monitorWidth * 0.66f;
        float bezelThickness = monitorWidth * 0.035f;

        float mx = (Screen.width - monitorWidth) * 0.5f;
        float my = (Screen.height - monitorHeight) * 0.5f - Screen.height * 0.05f;

        Rect bezelRect = new Rect(mx, my, monitorWidth, monitorHeight);
        Rect screenRect = new Rect(bezelRect.x + bezelThickness, bezelRect.y + bezelThickness,
            monitorWidth - bezelThickness * 2f, monitorHeight - bezelThickness * 2f);

        float standWidth = monitorWidth * 0.16f;
        float standNeckHeight = monitorHeight * 0.10f;
        float standBaseHeight = monitorHeight * 0.045f;
        float standBaseWidth = monitorWidth * 0.34f;

        GUI.color = monitorStandColor;
        GUI.DrawTexture(new Rect(mx + (monitorWidth - standWidth) * 0.5f, bezelRect.yMax, standWidth, standNeckHeight), solid);
        GUI.DrawTexture(new Rect(mx + (monitorWidth - standBaseWidth) * 0.5f, bezelRect.yMax + standNeckHeight, standBaseWidth, standBaseHeight), solid);

        GUI.color = monitorBezelColor;
        GUI.DrawTexture(bezelRect, solid);

        GUI.color = screenColor;
        GUI.DrawTexture(screenRect, solid);
        GUI.color = Color.white;

        // GUI.BeginGroup clips everything drawn inside to the screen rect, so the desktop/window
        // content can never spill out over the bezel even while windows/taskbar elements sit
        // near its edges.
        GUI.BeginGroup(screenRect);
        Rect local = new Rect(0f, 0f, screenRect.width, screenRect.height);
        if (desktopShown) DrawDesktop(local);
        else DrawLoginContent(local);
        GUI.EndGroup();
    }

    /// <summary>Gradient wallpaper + a simple drawn "flag" logo + taskbar (icons, clock) + the
    /// games window once opened - an original desktop mockup in the spirit of a typical OS
    /// login/home screen, not a reproduction of any specific one.</summary>
    private void DrawDesktop(Rect screenRect)
    {
        GUI.DrawTexture(screenRect, gradientTex, ScaleMode.StretchToFill);

        float logoWidth = screenRect.width * logoWidthFraction;
        Rect logoArea = new Rect(screenRect.x + screenRect.width * 0.62f,
            screenRect.y + screenRect.height * 0.32f - logoWidth * 0.66f * 0.5f, logoWidth, logoWidth * 0.66f);
        DrawFlagLogo(logoArea);

        DrawMailShortcut(screenRect);

        float taskbarHeight = screenRect.height * 0.10f;
        Rect taskbarRect = new Rect(screenRect.x, screenRect.yMax - taskbarHeight, screenRect.width, taskbarHeight);
        GUI.color = taskbarColor;
        GUI.DrawTexture(taskbarRect, solid);
        GUI.color = Color.white;

        // Weather widget, bottom-left.
        Rect weatherRect = new Rect(taskbarRect.x + 8f, taskbarRect.y, 70f, taskbarHeight);
        GUI.Label(weatherRect, weatherText, weatherStyle);

        // A row of small app-icon squares, one of which (gamesIconIndex) actually opens the window.
        float iconSize = taskbarHeight * 0.62f;
        float iconGap = iconSize * 0.35f;
        float iconsTotalWidth = taskbarIconCount * iconSize + (taskbarIconCount - 1) * iconGap;
        float iconsStartX = taskbarRect.x + (taskbarRect.width - iconsTotalWidth) * 0.5f;
        float iconY = taskbarRect.y + (taskbarHeight - iconSize) * 0.5f;

        for (int i = 0; i < taskbarIconCount; i++)
        {
            Rect iconRect = new Rect(iconsStartX + i * (iconSize + iconGap), iconY, iconSize, iconSize);

            GUI.color = iconColors[i];
            GUI.DrawTexture(iconRect, solid);
            GUI.color = new Color(1f, 1f, 1f, 0.95f);
            float glyphInset = iconSize * 0.24f;
            Rect glyphRect = new Rect(iconRect.x + glyphInset * 0.5f, iconRect.y + glyphInset * 0.5f,
                iconSize - glyphInset, iconSize - glyphInset);
            if (iconGlyphs != null && iconGlyphs[i] != null) GUI.DrawTexture(glyphRect, iconGlyphs[i]);

            if (i == gamesIconIndex && GUI.Button(iconRect, GUIContent.none, GUIStyle.none)) windowOpen = true;
        }
        GUI.color = Color.white;

        // System tray: wifi + volume glyphs, then the clock - right end of the bar, like a
        // normal OS taskbar tray.
        float traySize = taskbarHeight * 0.42f;
        float trayY = taskbarRect.y + (taskbarHeight - traySize) * 0.5f;
        Rect clockRect = new Rect(taskbarRect.xMax - 90f, taskbarRect.y, 84f, taskbarHeight);
        Rect speakerRect = new Rect(clockRect.x - traySize - 10f, trayY, traySize, traySize);
        Rect wifiRect = new Rect(speakerRect.x - traySize - 8f, trayY, traySize, traySize);

        GUI.color = new Color(0.15f, 0.16f, 0.20f, 0.85f);
        if (wifiIconTex != null) GUI.DrawTexture(wifiRect, wifiIconTex);
        if (speakerIconTex != null) GUI.DrawTexture(speakerRect, speakerIconTex);
        GUI.color = Color.white;

        GUI.Label(clockRect, System.DateTime.Now.ToString("HH:mm"), clockStyle);

        if (windowOpen) DrawWindow(screenRect, taskbarHeight);
        if (mailOpen) DrawMailApp(screenRect, taskbarHeight);
    }

    /// <summary>A desktop icon (envelope glyph + label) sitting directly on the wallpaper -
    /// clicking it opens the mail app. Pulses with glowColor while mailIconGlowing is armed
    /// (the "Open email" objective), same convention as the 3D world's ObjectiveGlow markers.</summary>
    private void DrawMailShortcut(Rect screenRect)
    {
        float size = Mathf.Min(screenRect.width, screenRect.height) * 0.10f;
        float x = screenRect.x + screenRect.width * mailShortcutPosition.x;
        float y = screenRect.y + screenRect.height * mailShortcutPosition.y;

        Rect iconRect = new Rect(x, y, size, size);
        Rect labelRect = new Rect(x - size * 0.35f, y + size + 4f, size * 1.7f, 20f);
        Rect hitRect = new Rect(iconRect.x - 6f, iconRect.y - 6f, iconRect.width + 12f, labelRect.yMax - iconRect.y + 6f);

        if (mailIconGlowing) DrawPulsingGlow(iconRect, 8f, 6f);

        GUI.color = new Color(1f, 1f, 1f, 0.92f);
        if (mailShortcutIconTex != null) GUI.DrawTexture(iconRect, mailShortcutIconTex);
        GUI.color = Color.white;
        GUI.Label(labelRect, mailShortcutLabel, shortcutLabelStyle);

        if (GUI.Button(hitRect, GUIContent.none, GUIStyle.none)) mailOpen = true;
    }

    /// <summary>A soft pulsing glow rect behind `target`, same blue as the 3D ObjectiveGlow rim
    /// shader - used for both the mail shortcut and the email's link while their objective is
    /// the current one.</summary>
    private void DrawPulsingGlow(Rect target, float baseGrow, float pulseGrow)
    {
        float phase = (Mathf.Sin(Time.time * glowPulseSpeed) + 1f) * 0.5f;
        float alpha = Mathf.Lerp(0.25f, 0.55f, phase);
        float grow = baseGrow + pulseGrow * phase;
        GUI.color = new Color(glowColor.r, glowColor.g, glowColor.b, alpha);
        GUI.DrawTexture(new Rect(target.x - grow, target.y - grow, target.width + grow * 2f, target.height + grow * 2f), solid);
        GUI.color = Color.white;
    }

    /// <summary>The mail app window: an inbox with the one story email, or that email's content
    /// once opened - an original mail-client mockup, not tied to any real email client.</summary>
    private void DrawMailApp(Rect screenRect, float taskbarHeight)
    {
        float winW = screenRect.width * windowSizeFraction;
        float winH = (screenRect.height - taskbarHeight) * windowSizeFraction;
        float wx = screenRect.x + (screenRect.width - winW) * 0.5f;
        float wy = screenRect.y + (screenRect.height - taskbarHeight - winH) * 0.5f;

        float titleBarHeight = winH * 0.10f;

        GUI.color = titleBarColor;
        GUI.DrawTexture(new Rect(wx, wy, winW, titleBarHeight), solid);
        GUI.color = Color.white;
        string title = mailView == MailView.Detail ? emailSubject : mailWindowTitle;
        GUI.Label(new Rect(wx + 8f, wy, winW - titleBarHeight - 8f, titleBarHeight), title, windowTitleStyle);

        if (GUI.Button(new Rect(wx + winW - titleBarHeight, wy, titleBarHeight, titleBarHeight), "X", closeButtonStyle))
        {
            mailOpen = false;
            return;
        }

        GUI.color = windowBodyColor;
        Rect bodyRect = new Rect(wx, wy + titleBarHeight, winW, winH - titleBarHeight);
        GUI.DrawTexture(bodyRect, solid);
        GUI.color = Color.white;

        if (mailView == MailView.Detail) DrawEmailDetail(bodyRect);
        else DrawMailInbox(bodyRect);
    }

    /// <summary>A single unread-looking row for the one story email - clicking it opens the
    /// email content.</summary>
    private void DrawMailInbox(Rect bodyRect)
    {
        float pad = bodyRect.width * 0.03f;
        float rowHeight = Mathf.Max(56f, bodyRect.height * 0.16f);
        Rect rowRect = new Rect(bodyRect.x + pad, bodyRect.y + pad, bodyRect.width - pad * 2f, rowHeight);

        GUI.color = Color.white;
        GUI.DrawTexture(rowRect, solid);

        GUI.color = new Color(0.30f, 0.55f, 0.95f, 1f);
        GUI.DrawTexture(new Rect(rowRect.x + 10f, rowRect.y + rowHeight * 0.5f - 4f, 8f, 8f), solid);

        Rect senderRect = new Rect(rowRect.x + 28f, rowRect.y + 8f, rowRect.width - 40f, 22f);
        Rect subjectRect = new Rect(rowRect.x + 28f, rowRect.y + 30f, rowRect.width - 40f, 20f);
        GUI.color = new Color(0.15f, 0.17f, 0.22f, 1f);
        GUI.Label(senderRect, emailSenderName, inboxSenderStyle);
        GUI.color = new Color(0.45f, 0.48f, 0.54f, 1f);
        GUI.Label(subjectRect, emailSubject, inboxSubjectStyle);
        GUI.color = Color.white;

        if (GUI.Button(rowRect, GUIContent.none, GUIStyle.none)) mailView = MailView.Detail;
    }

    /// <summary>The opened email: sender/subject header, body text, and the clickable link -
    /// the link pulses with glowColor while linkGlowing is armed (the "Click the link"
    /// objective).</summary>
    private void DrawEmailDetail(Rect bodyRect)
    {
        float pad = bodyRect.width * 0.05f;
        float y = bodyRect.y + pad;
        float textWidth = bodyRect.width - pad * 2f;

        GUI.color = new Color(0.15f, 0.17f, 0.22f, 1f);
        GUI.Label(new Rect(bodyRect.x + pad, y, textWidth, 22f), "From: " + emailSenderName, mailFieldLabelStyle);
        y += 24f;
        GUI.Label(new Rect(bodyRect.x + pad, y, textWidth, 22f), "Subject: " + emailSubject, mailFieldLabelStyle);
        y += 36f;

        GUI.color = new Color(0.25f, 0.27f, 0.30f, 1f);
        float bodyTextHeight = emailBodyStyle.CalcHeight(new GUIContent(emailBodyIntro), textWidth);
        GUI.Label(new Rect(bodyRect.x + pad, y, textWidth, bodyTextHeight), emailBodyIntro, emailBodyStyle);
        y += bodyTextHeight + 20f;
        GUI.color = Color.white;

        Rect linkRect = new Rect(bodyRect.x + pad, y, Mathf.Min(textWidth, 340f), 40f);
        if (linkGlowing) DrawPulsingGlow(linkRect, 5f, 5f);

        GUI.color = new Color(0.20f, 0.45f, 0.90f, 1f);
        GUI.DrawTexture(linkRect, solid);
        GUI.color = Color.white;
        GUI.Label(linkRect, emailLinkLabel, linkStyle);

        if (GUI.Button(linkRect, GUIContent.none, GUIStyle.none)) linkClicked = true;
    }

    /// <summary>Four squares with a gap, like a simple abstract "flag" mark - an original shape, not any specific logo.</summary>
    private void DrawFlagLogo(Rect area)
    {
        float gap = area.width * 0.045f;
        float halfW = (area.width - gap) * 0.5f;
        float halfH = (area.height - gap) * 0.5f;

        GUI.color = logoColor;
        GUI.DrawTexture(new Rect(area.x, area.y, halfW, halfH), solid);
        GUI.DrawTexture(new Rect(area.x + halfW + gap, area.y, halfW, halfH), solid);
        GUI.DrawTexture(new Rect(area.x, area.y + halfH + gap, halfW, halfH), solid);
        GUI.DrawTexture(new Rect(area.x + halfW + gap, area.y + halfH + gap, halfW, halfH), solid);
        GUI.color = Color.white;
    }

    private void DrawWindow(Rect screenRect, float taskbarHeight)
    {
        float winW = screenRect.width * windowSizeFraction;
        float winH = (screenRect.height - taskbarHeight) * windowSizeFraction;
        float wx = screenRect.x + (screenRect.width - winW) * 0.5f;
        float wy = screenRect.y + (screenRect.height - taskbarHeight - winH) * 0.5f;

        float titleBarHeight = winH * 0.10f;

        GUI.color = titleBarColor;
        GUI.DrawTexture(new Rect(wx, wy, winW, titleBarHeight), solid);
        GUI.color = Color.white;
        GUI.Label(new Rect(wx + 8f, wy, winW - titleBarHeight - 8f, titleBarHeight), windowTitle, windowTitleStyle);

        if (GUI.Button(new Rect(wx + winW - titleBarHeight, wy, titleBarHeight, titleBarHeight), "X", closeButtonStyle))
        {
            windowOpen = false;
            return;
        }

        GUI.color = windowBodyColor;
        Rect bodyRect = new Rect(wx, wy + titleBarHeight, winW, winH - titleBarHeight);
        GUI.DrawTexture(bodyRect, solid);
        GUI.color = Color.white;
        GUI.Label(bodyRect, windowBodyText, windowBodyStyle);
    }

    private void DrawLoginContent(Rect screenRect)
    {
        float x = screenRect.x, y = screenRect.y, w = screenRect.width, h = screenRect.height;

        float avatarSize = Mathf.Min(w, h) * 0.22f;
        Rect avatarRect = new Rect(x + (w - avatarSize) * 0.5f, y + h * 0.28f - avatarSize * 0.5f, avatarSize, avatarSize);
        GUI.DrawTexture(avatarRect, avatarTex);

        Rect nameRect = new Rect(x, avatarRect.yMax + 10f, w, 30f);
        GUI.Label(nameRect, playerName, nameStyle);

        float barWidth = w * 0.45f;
        float barHeight = 14f;
        Rect barRect = new Rect(x + (w - barWidth) * 0.5f, nameRect.yMax + 14f, barWidth, barHeight);
        GUI.color = new Color(1f, 1f, 1f, 0.55f);
        GUI.DrawTexture(barRect, solid);
        float fill = loaded ? 1f : Mathf.Clamp01(loadElapsed / Mathf.Max(0.01f, loadDuration));
        GUI.color = new Color(0.30f, 0.55f, 0.95f, 1f);
        GUI.DrawTexture(new Rect(barRect.x, barRect.y, barRect.width * fill, barRect.height), solid);
        GUI.color = Color.white;

        Rect statusRect = new Rect(x, barRect.yMax + 8f, w, 22f);
        GUI.Label(statusRect, loaded ? readyText : loadingText, statusStyle);
    }

    private void EnsureStyles()
    {
        if (nameStyle != null) return;

        nameStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        nameStyle.normal.textColor = new Color(0.15f, 0.17f, 0.22f, 1f);

        statusStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 13,
            alignment = TextAnchor.MiddleCenter
        };
        statusStyle.normal.textColor = new Color(0.35f, 0.38f, 0.44f, 1f);

        clockStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 13,
            alignment = TextAnchor.MiddleCenter
        };
        clockStyle.normal.textColor = Color.white;

        weatherStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 13,
            alignment = TextAnchor.MiddleLeft
        };
        weatherStyle.normal.textColor = Color.white;

        windowTitleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
        windowTitleStyle.normal.textColor = Color.white;

        closeButtonStyle = new GUIStyle(GUI.skin.button) { fontSize = 12 };

        windowBodyStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleCenter
        };
        windowBodyStyle.normal.textColor = new Color(0.4f, 0.4f, 0.45f, 1f);

        shortcutLabelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            alignment = TextAnchor.UpperCenter
        };
        shortcutLabelStyle.normal.textColor = Color.white;

        mailFieldLabelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 13,
            alignment = TextAnchor.MiddleLeft
        };
        mailFieldLabelStyle.normal.textColor = new Color(0.3f, 0.32f, 0.36f, 1f);

        toastTitleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 13,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperLeft
        };
        toastTitleStyle.normal.textColor = Color.white;

        toastSubjectStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 13,
            alignment = TextAnchor.UpperLeft
        };
        toastSubjectStyle.normal.textColor = Color.white;

        inboxSenderStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperLeft
        };
        inboxSenderStyle.normal.textColor = Color.white;

        inboxSubjectStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 13,
            alignment = TextAnchor.UpperLeft
        };
        inboxSubjectStyle.normal.textColor = Color.white;

        emailBodyStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            alignment = TextAnchor.UpperLeft,
            wordWrap = true
        };
        emailBodyStyle.normal.textColor = Color.white;

        linkStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        linkStyle.normal.textColor = Color.white;
    }

    private static Texture2D BuildSolidTexture()
    {
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return tex;
    }

    /// <summary>Filled circle on a transparent background, for a generic avatar placeholder (no photo asset to work with).</summary>
    private static Texture2D BuildCircleTexture(int size, Color color)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        float r = size * 0.5f;
        var pixels = new Color[size * size];
        for (int py = 0; py < size; py++)
        {
            for (int px = 0; px < size; px++)
            {
                float dx = px + 0.5f - r;
                float dy = py + 0.5f - r;
                bool inside = dx * dx + dy * dy <= r * r;
                pixels[py * size + px] = inside ? color : new Color(0f, 0f, 0f, 0f);
            }
        }
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    /// <summary>Vertical top-to-bottom gradient, for the desktop wallpaper.</summary>
    private static Texture2D BuildGradientTexture(int size, Color top, Color bottom)
    {
        var tex = new Texture2D(1, size, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        var pixels = new Color[size];
        for (int y = 0; y < size; y++)
        {
            // Row 0 is the texture's bottom in Unity's UV space, but GUI.DrawTexture maps a Rect
            // top-down, so put "top" at the last row and "bottom" at row 0.
            pixels[y] = Color.Lerp(bottom, top, y / (float)(size - 1));
        }
        tex.SetPixels(pixels);
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;
        return tex;
    }

    /// <summary>White glyph shape on a transparent background, tinted via GUI.color when drawn.</summary>
    private static Texture2D BuildGlyphTexture(int size, GlyphKind kind)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        float r = size * 0.5f;
        var pixels = new Color[size * size];
        for (int py = 0; py < size; py++)
        {
            for (int px = 0; px < size; px++)
            {
                float nx = (px + 0.5f - r) / r;
                float ny = (py + 0.5f - r) / r;
                pixels[py * size + px] = GlyphCovers(kind, nx, ny) ? Color.white : new Color(0f, 0f, 0f, 0f);
            }
        }
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    /// <summary>Per-pixel silhouette test for BuildGlyphTexture, in coordinates normalized to
    /// [-1,1] with +y downward (image space).</summary>
    private static bool GlyphCovers(GlyphKind kind, float nx, float ny)
    {
        float dist = Mathf.Sqrt(nx * nx + ny * ny);
        switch (kind)
        {
            case GlyphKind.Circle:
                return dist <= 0.72f;

            case GlyphKind.Ring:
                return dist <= 0.78f && dist >= 0.48f;

            case GlyphKind.Triangle:
                return nx >= -0.55f && nx <= 0.7f && Mathf.Abs(ny) <= (0.7f - nx) * 0.62f;

            case GlyphKind.Envelope:
            {
                bool inRect = nx >= -0.75f && nx <= 0.75f && ny >= -0.5f && ny <= 0.5f;
                if (!inRect) return false;
                bool onBorder = nx <= -0.68f || nx >= 0.68f || ny <= -0.43f || ny >= 0.43f;
                bool onFlap = DistToSegment(nx, ny, -0.75f, -0.5f, 0f, 0.08f) <= 0.07f
                    || DistToSegment(nx, ny, 0.75f, -0.5f, 0f, 0.08f) <= 0.07f;
                return onBorder || onFlap;
            }

            case GlyphKind.Folder:
            {
                bool body = nx >= -0.75f && nx <= 0.75f && ny >= -0.3f && ny <= 0.6f;
                bool tab = nx >= -0.75f && nx <= -0.15f && ny >= -0.58f && ny <= -0.3f;
                return body || tab;
            }

            case GlyphKind.Document:
            {
                bool border = nx >= -0.5f && nx <= 0.5f && ny >= -0.75f && ny <= 0.75f
                    && (nx <= -0.42f || nx >= 0.42f || ny <= -0.66f || ny >= 0.66f);
                bool line1 = ny >= -0.28f && ny <= -0.16f && nx >= -0.35f && nx <= 0.35f;
                bool line2 = ny >= 0.0f && ny <= 0.12f && nx >= -0.35f && nx <= 0.35f;
                bool line3 = ny >= 0.28f && ny <= 0.4f && nx >= -0.35f && nx <= 0.35f;
                return border || line1 || line2 || line3;
            }

            case GlyphKind.Magnifier:
            {
                float gx = nx + 0.12f, gy = ny + 0.12f;
                float gd = Mathf.Sqrt(gx * gx + gy * gy);
                bool ring = gd <= 0.5f && gd >= 0.34f;
                bool handle = DistToSegment(nx, ny, 0.28f, 0.28f, 0.72f, 0.72f) <= 0.09f;
                return ring || handle;
            }

            case GlyphKind.Gear:
            {
                float theta = Mathf.Atan2(ny, nx);
                float toothed = 0.62f + (Mathf.Cos(theta * 8f) > 0.2f ? 0.14f : 0f);
                return dist <= toothed && dist >= 0.26f;
            }

            case GlyphKind.House:
            {
                bool roof = ny <= -0.05f && ny >= -0.85f && Mathf.Abs(nx) <= (ny + 0.85f) * 0.9f;
                bool body = ny >= -0.05f && ny <= 0.75f && nx >= -0.55f && nx <= 0.55f;
                return roof || body;
            }

            case GlyphKind.Bubble:
            {
                bool body = nx >= -0.75f && nx <= 0.75f && ny >= -0.55f && ny <= 0.35f;
                bool tail = DistToSegment(nx, ny, -0.35f, 0.35f, -0.55f, 0.75f) <= 0.09f;
                return body || tail;
            }

            case GlyphKind.Star:
            {
                bool vBar = Mathf.Abs(nx) <= 0.14f && Mathf.Abs(ny) <= 0.78f;
                bool hBar = Mathf.Abs(ny) <= 0.14f && Mathf.Abs(nx) <= 0.78f;
                return vBar || hBar;
            }

            case GlyphKind.Cloud:
            {
                float d1 = Mathf.Sqrt((nx + 0.28f) * (nx + 0.28f) + (ny + 0.05f) * (ny + 0.05f));
                float d2 = Mathf.Sqrt((nx - 0.12f) * (nx - 0.12f) + (ny + 0.15f) * (ny + 0.15f));
                float d3 = Mathf.Sqrt((nx - 0.42f) * (nx - 0.42f) + (ny + 0.02f) * (ny + 0.02f));
                bool lobes = d1 <= 0.33f || d2 <= 0.4f || d3 <= 0.3f;
                bool baseRect = ny >= 0.05f && ny <= 0.28f && nx >= -0.55f && nx <= 0.68f;
                return lobes || baseRect;
            }

            case GlyphKind.Wifi:
            {
                float ax = 0f, ay = 0.55f;
                float dx = nx - ax, dy = ny - ay;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                bool dot = d <= 0.09f;
                float upCos = d > 0.001f ? -dy / d : 1f;
                bool inCone = upCos >= 0.6f;
                bool arc1 = inCone && d >= 0.22f && d <= 0.30f;
                bool arc2 = inCone && d >= 0.42f && d <= 0.50f;
                bool arc3 = inCone && d >= 0.62f && d <= 0.70f;
                return dot || arc1 || arc2 || arc3;
            }

            case GlyphKind.Speaker:
            {
                bool box = nx >= -0.65f && nx <= -0.15f && ny >= -0.22f && ny <= 0.22f;
                bool cone = nx >= -0.15f && nx <= 0.28f && Mathf.Abs(ny) <= 0.22f + (nx + 0.15f) * 1.1f;
                float ax = 0.30f, ay = 0f;
                float dx = nx - ax, dy = ny - ay;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                float rightCos = d > 0.001f ? dx / d : 1f;
                bool inCone = rightCos >= 0.35f;
                bool wave1 = inCone && d >= 0.32f && d <= 0.40f;
                bool wave2 = inCone && d >= 0.52f && d <= 0.60f;
                return box || cone || wave1 || wave2;
            }

            default:
                return dist <= 0.7f;
        }
    }

    private static float DistToSegment(float px, float py, float ax, float ay, float bx, float by)
    {
        float abx = bx - ax, aby = by - ay;
        float apx = px - ax, apy = py - ay;
        float abLenSq = abx * abx + aby * aby;
        float t = abLenSq > 0.0001f ? Mathf.Clamp01((apx * abx + apy * aby) / abLenSq) : 0f;
        float cx = ax + abx * t, cy = ay + aby * t;
        float dx = px - cx, dy = py - cy;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }
}
