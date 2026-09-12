using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using InfimaGames.LowPolyShooterPack;

/// <summary>
/// Sci-fi hologram that opens the "Escape" mission: a flickering, scanline-striped cyan panel
/// materialises in front of the player and types out "PROTOCOL: ESCAPE" plus a short briefing.
/// Mouse look and movement are frozen for the duration; the player presses E to dismiss it once
/// everything has finished typing.
///
/// Call <see cref="Begin"/> from TrainingBedIntro.onComplete once the player is standing.
/// IMGUI, matching the rest of this project's HUDs; text is English because Unity's IMGUI/
/// TextMeshPro cannot shape Burmese correctly.
/// </summary>
public class EscapeProtocolHologram : MonoBehaviour
{
    [Header("Player lock")]
    [Tooltip("Player root that carries Movement/CameraLook/PlayerInput. Auto-found by tag if left empty.")]
    [SerializeField] private Character player;
    [SerializeField] private KeyCode dismissKey = KeyCode.E;

    [Header("Start")]
    [SerializeField] private bool playOnStart;
    [SerializeField] private float startDelay = 0.6f;

    [Header("Content")]
    [SerializeField] private string title = "PROTOCOL: ESCAPE";
    [SerializeField]
    private string[] lines =
    {
        "Welcome, Operative.",
        "You have a mission.",
        "A hostage is being held on Floor 5 of this facility.",
        "You must reach her before it's too late.",
        "Weapons have been issued to you. Use them."
    };
    [SerializeField] private string dismissHint = "[ E ] Continue";

    [Header("Timing")]
    [SerializeField] private float materializeDuration = 0.5f;
    [SerializeField] private float typeCharsPerSecond = 34f;
    [SerializeField] private float holdAfterTitle = 0.5f;
    [SerializeField] private float holdPerLine = 0.35f;
    [SerializeField] private float dematerializeDuration = 0.6f;

    [Header("Layout")]
    [SerializeField] private float panelWidth = 980f;
    [SerializeField] private float topOffset = 60f;
    [SerializeField] private int titleFontSize = 40;
    [SerializeField] private int lineFontSize = 26;
    [SerializeField] private int hintFontSize = 18;

    [Header("Look")]
    [SerializeField] private Color holoColor = new Color(0.35f, 0.95f, 1f, 1f);
    [SerializeField] private Color titleColor = new Color(0.75f, 1f, 1f, 1f);
    [Tooltip("Optional 3D projector prop (beam/particles/light) toggled on for the duration of the hologram, so it reads as a real projection in the world instead of just a screen overlay.")]
    [SerializeField] private GameObject emitterVisual;

    [Tooltip("Fires once, right after the player dismisses the hologram and control is handed back.")]
    public UnityEvent onFinished;

    private bool hasPlayed;
    private float panelAlpha;     // materialize/dematerialize envelope, 0..1
    private float flicker = 1f;   // fast sci-fi flicker multiplier on top of panelAlpha
    private string shownTitle = "";
    private readonly System.Collections.Generic.List<string> shownLines = new System.Collections.Generic.List<string>();
    private bool waitingForDismiss;
    private float hintAlpha;

    private Movement movement;
    private CameraLook look;
    private PlayerInput input;
    private bool movementEnabled, lookEnabled, inputEnabled;
    private bool lockApplied;

    private Texture2D solid;
    private GUIStyle titleStyle, lineStyle, hintStyle;

    private void Awake()
    {
        solid = new Texture2D(1, 1, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        solid.SetPixel(0, 0, Color.white);
        solid.Apply();

        if (player == null)
        {
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo != null) player = playerGo.GetComponent<Character>();
        }
        if (player != null)
        {
            movement = player.GetComponent<Movement>();
            look = player.GetComponentInChildren<CameraLook>();
            input = player.GetComponent<PlayerInput>();
        }
    }

    private void OnDestroy()
    {
        if (solid != null) Destroy(solid);
    }

    private void Start()
    {
        if (playOnStart) Begin();
    }

    /// <summary>Plays the hologram once; safe to call again (later calls are ignored).</summary>
    public void Begin()
    {
        if (hasPlayed) return;
        hasPlayed = true;
        StartCoroutine(Sequence());
    }

    private void LockPlayer()
    {
        if (lockApplied || movement == null) return;
        lockApplied = true;
        movementEnabled = movement.enabled; lookEnabled = look.enabled; inputEnabled = input.enabled;
        movement.enabled = false; look.enabled = false; input.enabled = false;
    }

    private void UnlockPlayer()
    {
        if (!lockApplied) return;
        lockApplied = false;
        movement.enabled = movementEnabled; look.enabled = lookEnabled; input.enabled = inputEnabled;
    }

    private IEnumerator Sequence()
    {
        if (startDelay > 0f) yield return new WaitForSeconds(startDelay);

        LockPlayer();
        if (emitterVisual != null) emitterVisual.SetActive(true);

        // Materialize: fade the panel in with a rough, flickery power-on rather than a clean lerp.
        for (float t = 0f; t < materializeDuration; t += Time.deltaTime)
        {
            panelAlpha = Mathf.Clamp01(t / materializeDuration);
            yield return null;
        }
        panelAlpha = 1f;

        yield return TypeText(title, s => shownTitle = s);
        yield return new WaitForSeconds(holdAfterTitle);

        foreach (string line in lines)
        {
            shownLines.Add("");
            int idx = shownLines.Count - 1;
            yield return TypeText(line, s => shownLines[idx] = s);
            yield return new WaitForSeconds(holdPerLine);
        }

        // Everything has been typed - wait for the player to actively dismiss it.
        waitingForDismiss = true;
        while (waitingForDismiss)
        {
            hintAlpha = 0.6f + 0.4f * Mathf.Sin(Time.unscaledTime * 4f);
            if (Input.GetKeyDown(dismissKey)) waitingForDismiss = false;
            yield return null;
        }

        // Dematerialize.
        for (float t = 0f; t < dematerializeDuration; t += Time.deltaTime)
        {
            panelAlpha = 1f - Mathf.Clamp01(t / dematerializeDuration);
            yield return null;
        }
        panelAlpha = 0f;
        if (emitterVisual != null) emitterVisual.SetActive(false);

        UnlockPlayer();

        onFinished?.Invoke();
    }

    private IEnumerator TypeText(string full, System.Action<string> apply)
    {
        var sb = new StringBuilder();
        float chars = 0f;
        while (sb.Length < full.Length)
        {
            chars += typeCharsPerSecond * Time.deltaTime;
            int count = Mathf.Clamp(Mathf.FloorToInt(chars), 0, full.Length);
            if (count != sb.Length)
            {
                sb.Length = 0;
                sb.Append(full, 0, count);
                apply(sb.ToString());
            }
            yield return null;
        }
        apply(full);
    }

    private void Update()
    {
        // A fast, irregular flicker on top of the fade envelope - two mismatched sine waves plus
        // occasional harder dips read as an unstable hologram rather than a clean UI fade.
        if (panelAlpha <= 0f) { flicker = 1f; return; }
        float f = 0.88f + 0.08f * Mathf.Sin(Time.unscaledTime * 37f) + 0.04f * Mathf.Sin(Time.unscaledTime * 91f + 1.7f);
        if (Random.value < 0.01f) f *= 0.5f;
        flicker = Mathf.Clamp01(f);
    }

    private void OnGUI()
    {
        if (panelAlpha <= 0.001f) return;

        EnsureStyles();

        float alpha = panelAlpha * flicker;
        // A brief horizontal tear during hard flicker dips - classic unstable-hologram glitch.
        float glitchOffset = flicker < 0.7f && Random.value < 0.3f ? (Random.value - 0.5f) * 10f : 0f;

        string bodyText = string.Join("\n\n", shownLines);
        float width = Mathf.Min(panelWidth, Screen.width - 80f);
        float titleHeight = titleFontSize + 14f;
        float bodyHeight = string.IsNullOrEmpty(bodyText) ? 0f : lineStyle.CalcHeight(new GUIContent(bodyText), width - 64f) + 24f;
        float hintHeight = waitingForDismiss ? hintFontSize + 24f : 0f;
        float panelHeight = titleHeight + bodyHeight + hintHeight + 44f;
        float x = (Screen.width - width) * 0.5f + glitchOffset;
        float y = topOffset;

        Color prev = GUI.color;

        // Backdrop glass.
        GUI.color = new Color(0.02f, 0.08f, 0.1f, 0.6f * alpha);
        GUI.DrawTexture(new Rect(x, y, width, panelHeight), solid);

        // Border.
        GUI.color = new Color(holoColor.r, holoColor.g, holoColor.b, 0.9f * alpha);
        DrawBorder(x, y, width, panelHeight, 3f);

        // Sci-fi corner brackets, offset outward from the panel like a HUD targeting frame.
        DrawCornerBrackets(x, y, width, panelHeight, 26f, 3f, alpha);

        // Small hexagon accents flanking the title, echoing the facility's hex motif.
        DrawHexAccent(x + 14f, y + titleHeight * 0.5f + 20f, 9f, alpha);
        DrawHexAccent(x + width - 14f, y + titleHeight * 0.5f + 20f, 9f, alpha);

        // Scanlines scrolling slowly upward.
        float scanSpacing = 7f;
        float scroll = (Time.unscaledTime * 20f) % scanSpacing;
        GUI.color = new Color(holoColor.r, holoColor.g, holoColor.b, 0.07f * alpha);
        for (float sy = y - scroll; sy < y + panelHeight; sy += scanSpacing)
        {
            if (sy < y || sy > y + panelHeight) continue;
            GUI.DrawTexture(new Rect(x, sy, width, 2f), solid);
        }

        // Emitter bar under the panel, like a projector base.
        GUI.color = new Color(holoColor.r, holoColor.g, holoColor.b, 0.8f * alpha);
        GUI.DrawTexture(new Rect(x + width * 0.5f - 60f, y + panelHeight + 6f, 120f, 4f), solid);

        var titleRect = new Rect(x + 32f, y + 20f, width - 64f, titleHeight);
        DrawGlowLabel(titleRect, shownTitle, titleStyle, titleColor, alpha, glitchOffset);

        if (!string.IsNullOrEmpty(bodyText))
        {
            var bodyRect = new Rect(x + 32f, y + 20f + titleHeight, width - 64f, bodyHeight);
            DrawGlowLabel(bodyRect, bodyText, lineStyle, holoColor, alpha, glitchOffset);
        }

        if (waitingForDismiss)
        {
            var hintRect = new Rect(x + 32f, y + panelHeight - hintHeight, width - 64f, hintHeight);
            DrawGlowLabel(hintRect, dismissHint, hintStyle, holoColor, alpha * hintAlpha, 0f);
        }

        GUI.color = prev;
    }

    private void DrawCornerBrackets(float x, float y, float w, float h, float size, float thickness, float alpha)
    {
        GUI.color = new Color(holoColor.r, holoColor.g, holoColor.b, alpha);
        float o = 8f; // outward offset from the panel edge

        // Top-left.
        GUI.DrawTexture(new Rect(x - o, y - o, size, thickness), solid);
        GUI.DrawTexture(new Rect(x - o, y - o, thickness, size), solid);
        // Top-right.
        GUI.DrawTexture(new Rect(x + w + o - size, y - o, size, thickness), solid);
        GUI.DrawTexture(new Rect(x + w + o - thickness, y - o, thickness, size), solid);
        // Bottom-left.
        GUI.DrawTexture(new Rect(x - o, y + h + o - thickness, size, thickness), solid);
        GUI.DrawTexture(new Rect(x - o, y + h + o - size, thickness, size), solid);
        // Bottom-right.
        GUI.DrawTexture(new Rect(x + w + o - size, y + h + o - thickness, size, thickness), solid);
        GUI.DrawTexture(new Rect(x + w + o - thickness, y + h + o - size, thickness, size), solid);
    }

    private void DrawHexAccent(float cx, float cy, float r, float alpha)
    {
        GUI.color = new Color(holoColor.r, holoColor.g, holoColor.b, 0.5f * alpha);
        // A hexagon rendered as a thin diamond-ish cross of bars reads fine at this tiny size.
        GUI.DrawTexture(new Rect(cx - r, cy - 1.5f, r * 2f, 3f), solid);
        GUI.DrawTexture(new Rect(cx - 1.5f, cy - r, 3f, r * 2f), solid);
    }

    private void DrawBorder(float x, float y, float w, float h, float thickness)
    {
        GUI.DrawTexture(new Rect(x, y, w, thickness), solid);
        GUI.DrawTexture(new Rect(x, y + h - thickness, w, thickness), solid);
        GUI.DrawTexture(new Rect(x, y, thickness, h), solid);
        GUI.DrawTexture(new Rect(x + w - thickness, y, thickness, h), solid);
    }

    private void DrawGlowLabel(Rect rect, string text, GUIStyle style, Color color, float alpha, float glitchOffset)
    {
        if (string.IsNullOrEmpty(text)) return;
        Color prev = GUI.color;

        // Soft glow: a couple of offset, low-alpha passes under the crisp top pass.
        GUI.color = new Color(color.r, color.g, color.b, 0.18f * alpha);
        GUI.Label(new Rect(rect.x - 1f, rect.y, rect.width, rect.height), text, style);
        GUI.Label(new Rect(rect.x + 1f, rect.y, rect.width, rect.height), text, style);
        GUI.Label(new Rect(rect.x, rect.y - 1f, rect.width, rect.height), text, style);

        // Chromatic-split glitch pass: faint red/blue ghosts either side during a hard flicker dip.
        if (Mathf.Abs(glitchOffset) > 0.01f)
        {
            GUI.color = new Color(1f, 0.2f, 0.25f, 0.25f * alpha);
            GUI.Label(new Rect(rect.x - glitchOffset, rect.y, rect.width, rect.height), text, style);
            GUI.color = new Color(0.2f, 0.4f, 1f, 0.25f * alpha);
            GUI.Label(new Rect(rect.x + glitchOffset, rect.y, rect.width, rect.height), text, style);
        }

        GUI.color = new Color(color.r, color.g, color.b, alpha);
        GUI.Label(rect, text, style);

        GUI.color = prev;
    }

    private void EnsureStyles()
    {
        if (titleStyle != null) return;

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = titleFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperLeft
        };
        titleStyle.normal.textColor = Color.white;

        lineStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = lineFontSize,
            alignment = TextAnchor.UpperLeft,
            wordWrap = true
        };
        lineStyle.normal.textColor = Color.white;

        hintStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = hintFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.LowerLeft
        };
        hintStyle.normal.textColor = Color.white;
    }
}
