using UnityEngine;

/// <summary>
/// Top-right objective panel ("MISSION / Wash your face"). IMGUI, so it needs no Canvas and
/// matches how PlayerInteractor already draws its prompt.
/// Text is English on purpose: Unity's IMGUI and TextMeshPro cannot shape Burmese correctly.
/// </summary>
public class MissionHUD : MonoBehaviour
{
    public static MissionHUD Instance { get; private set; }

    [Header("Layout")]
    [SerializeField] private float panelWidth = 360f;
    [SerializeField] private float marginRight = 28f;
    [SerializeField] private float marginTop = 28f;
    [SerializeField] private int headerFontSize = 15;
    [SerializeField] private int objectiveFontSize = 19;

    [Header("Timing")]
    [SerializeField] private float fadeInDuration = 0.4f;
    [Tooltip("How long the panel stays on screen after the objective is completed.")]
    [SerializeField] private float holdAfterComplete = 2.5f;
    [SerializeField] private float fadeOutDuration = 0.8f;

    [Header("Colours")]
    [SerializeField] private Color headerColor = new Color(0.75f, 0.78f, 0.85f, 1f);
    [SerializeField] private Color objectiveColor = Color.white;
    [SerializeField] private Color completedColor = new Color(0.55f, 0.92f, 0.60f, 1f);
    [SerializeField] private Color backdropColor = new Color(0f, 0f, 0f, 0.66f);
    [SerializeField] private Color accentColor = new Color(0.95f, 0.82f, 0.35f, 1f);

    private string objective;
    private int progressCurrent;
    private int progressTotal;   // 0 = no counter shown
    private bool isCompleted;
    private bool isVisible;
    private float visibleTime;
    private float completedTime;
    private float alpha;

    private Texture2D solid;
    private GUIStyle headerStyle;
    private GUIStyle objectiveStyle;

    private void Awake()
    {
        Instance = this;
        solid = BuildSolidTexture();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (solid != null) Destroy(solid);
    }

    /// <summary>Shows (or replaces) the current objective.</summary>
    public void SetObjective(string text)
    {
        SetObjective(text, 0);
    }

    /// <summary>
    /// Shows an objective with a progress counter, e.g. "Put your books in the bag  (1/2)".
    /// Pass total = 0 for a plain objective with no counter.
    /// </summary>
    public void SetObjective(string text, int total)
    {
        objective = text;
        progressTotal = Mathf.Max(0, total);
        progressCurrent = 0;
        isCompleted = false;
        isVisible = true;
        visibleTime = 0f;
    }

    /// <summary>Updates the counter. Does not complete the objective on its own.</summary>
    public void SetProgress(int current)
    {
        progressCurrent = Mathf.Clamp(current, 0, Mathf.Max(current, progressTotal));
    }

    /// <summary>Ticks the objective off; the panel lingers briefly, then fades away.</summary>
    public void CompleteObjective()
    {
        if (!isVisible || isCompleted) return;
        isCompleted = true;
        completedTime = 0f;
    }

    public bool HasActiveObjective => isVisible && !isCompleted;

    private void Update()
    {
        if (!isVisible) return;

        visibleTime += Time.deltaTime;

        if (isCompleted)
        {
            completedTime += Time.deltaTime;
            float t = completedTime - holdAfterComplete;
            if (t > 0f)
            {
                alpha = 1f - Mathf.Clamp01(t / fadeOutDuration);
                if (alpha <= 0f) isVisible = false;
                return;
            }
        }

        alpha = fadeInDuration > 0f ? Mathf.Clamp01(visibleTime / fadeInDuration) : 1f;
    }

    private void OnGUI()
    {
        if (!isVisible || alpha <= 0.001f || string.IsNullOrEmpty(objective)) return;

        EnsureStyles();

        Color bodyColorEarly = isCompleted ? completedColor : objectiveColor;
        string bullet = isCompleted ? "[x]  " : "[ ]  ";
        string counter = progressTotal > 0 ? "   (" + progressCurrent + "/" + progressTotal + ")" : "";
        string objectiveText = bullet + objective + counter;

        float x = Screen.width - panelWidth - marginRight;
        float y = marginTop;
        float headerHeight = headerFontSize + 10f;
        float padding = 12f;
        float textWidth = panelWidth - padding * 2f - 6f;
        // Measure the wrapped text instead of assuming one line, or a two-line objective
        // spills out of the panel and gets clipped.
        float objectiveHeight = objectiveStyle.CalcHeight(new GUIContent(objectiveText), textWidth) + 6f;
        float panelHeight = headerHeight + objectiveHeight + padding * 2f;

        Color previous = GUI.color;

        // Backdrop
        GUI.color = new Color(backdropColor.r, backdropColor.g, backdropColor.b, backdropColor.a * alpha);
        GUI.DrawTexture(new Rect(x, y, panelWidth, panelHeight), solid);

        // Accent bar down the left edge of the panel
        GUI.color = new Color(accentColor.r, accentColor.g, accentColor.b,
            (isCompleted ? 0.35f : 1f) * alpha);
        GUI.DrawTexture(new Rect(x, y, 3f, panelHeight), solid);

        var headerRect = new Rect(x + padding + 6f, y + padding, textWidth, headerHeight);
        var objectiveRect = new Rect(x + padding + 6f, y + padding + headerHeight, textWidth, objectiveHeight);

        DrawLabel(headerRect, "MISSION", headerStyle,
            new Color(headerColor.r, headerColor.g, headerColor.b, headerColor.a * alpha));

        Color bodyColor = bodyColorEarly;
        DrawLabel(objectiveRect, objectiveText, objectiveStyle,
            new Color(bodyColor.r, bodyColor.g, bodyColor.b, bodyColor.a * alpha));

        GUI.color = previous;
    }

    private void DrawLabel(Rect rect, string text, GUIStyle style, Color color)
    {
        // Same style for both passes; GUI.color does the tinting, so the shadow always
        // matches the label's own font size.
        GUI.color = new Color(0f, 0f, 0f, color.a * 0.9f);
        GUI.Label(new Rect(rect.x + 1.5f, rect.y + 1.5f, rect.width, rect.height), text, style);
        GUI.color = color;
        GUI.Label(rect, text, style);
    }

    private void EnsureStyles()
    {
        if (headerStyle != null) return;

        headerStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = headerFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
        headerStyle.normal.textColor = Color.white;

        objectiveStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = objectiveFontSize,
            // Upper-left, not middle-left: the rect is measured to fit, so vertical centring
            // only pushes wrapped text around.
            alignment = TextAnchor.UpperLeft,
            wordWrap = true
        };
        objectiveStyle.normal.textColor = Color.white;
    }

    private static Texture2D BuildSolidTexture()
    {
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return tex;
    }
}
