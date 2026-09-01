using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bottom-centre subtitle line with a typewriter reveal, queued so several lines play in order.
/// IMGUI, matching PlayerInteractor's prompt; text is English because Unity's IMGUI/TextMeshPro
/// cannot shape Burmese correctly.
/// </summary>
public class DialogueHUD : MonoBehaviour
{
    public static DialogueHUD Instance { get; private set; }

    [Header("Layout")]
    [SerializeField] private float maxWidth = 900f;
    [Tooltip("Gap between the bottom of the screen and the bottom of the box. This sits low " +
             "enough to overlap PlayerInteractor's prompt (120 px up), so the prompt is nudged " +
             "upward while a line is playing - see PlayerInteractor.promptBottomOffsetDuringDialogue.")]
    [SerializeField] private float bottomOffset = 70f;
    [SerializeField] private int speakerFontSize = 16;
    [SerializeField] private int lineFontSize = 22;

    [Header("Timing")]
    [Tooltip("Characters revealed per second.")]
    [SerializeField] private float typeSpeed = 38f;
    [Tooltip("Extra seconds a finished line stays up before the next one starts.")]
    [SerializeField] private float defaultHold = 1.7f;
    [SerializeField] private float fadeDuration = 0.28f;

    [Header("Colours")]
    [SerializeField] private Color speakerColor = new Color(0.95f, 0.82f, 0.35f, 1f);
    [SerializeField] private Color lineColor = Color.white;
    [SerializeField] private Color backdropColor = new Color(0f, 0f, 0f, 0.55f);

    private struct Line
    {
        public string speaker;
        public string text;
        public float hold;
    }

    private readonly Queue<Line> queue = new Queue<Line>();
    private Coroutine playRoutine;

    private string currentSpeaker = "";
    private string currentText = "";
    private int revealedChars;
    private float alpha;

    private Texture2D solid;
    private GUIStyle speakerStyle;
    private GUIStyle lineStyle;

    public bool IsPlaying => playRoutine != null;

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

    /// <summary>Queues one subtitle line. Pass hold &lt; 0 to use the default hold time.</summary>
    public void Say(string speaker, string text, float hold = -1f)
    {
        queue.Enqueue(new Line { speaker = speaker, text = text, hold = hold < 0f ? defaultHold : hold });
        if (playRoutine == null) playRoutine = StartCoroutine(PlayQueue());
    }

    /// <summary>Drops anything queued and clears the current line immediately.</summary>
    public void Clear()
    {
        queue.Clear();
        if (playRoutine != null) StopCoroutine(playRoutine);
        playRoutine = null;
        currentText = "";
        currentSpeaker = "";
        revealedChars = 0;
        alpha = 0f;
    }

    private IEnumerator PlayQueue()
    {
        while (queue.Count > 0)
        {
            Line line = queue.Dequeue();
            currentSpeaker = line.speaker;
            currentText = line.text;
            revealedChars = 0;

            yield return Fade(0f, 1f, fadeDuration);

            float typed = 0f;
            while (revealedChars < currentText.Length)
            {
                typed += typeSpeed * Time.deltaTime;
                revealedChars = Mathf.Min(currentText.Length, Mathf.FloorToInt(typed));
                yield return null;
            }
            revealedChars = currentText.Length;

            yield return new WaitForSeconds(line.hold);

            // Keep the backdrop up between lines of the same conversation.
            if (queue.Count == 0) yield return Fade(1f, 0f, fadeDuration);
        }

        currentText = "";
        currentSpeaker = "";
        alpha = 0f;
        playRoutine = null;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            alpha = Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));
            yield return null;
        }
        alpha = to;
    }

    private void OnGUI()
    {
        if (alpha <= 0.001f || string.IsNullOrEmpty(currentText)) return;

        EnsureStyles();

        string shown = currentText.Substring(0, Mathf.Clamp(revealedChars, 0, currentText.Length));

        float width = Mathf.Min(maxWidth, Screen.width - 80f);
        float x = (Screen.width - width) * 0.5f;
        float padding = 18f;

        float speakerHeight = string.IsNullOrEmpty(currentSpeaker) ? 0f : speakerFontSize + 8f;
        // Measure against the full line so the box doesn't resize as characters appear.
        float textHeight = lineStyle.CalcHeight(new GUIContent(currentText), width - padding * 2f);
        float boxHeight = textHeight + speakerHeight + padding * 2f;
        float y = Screen.height - bottomOffset - boxHeight;

        Color previous = GUI.color;

        GUI.color = new Color(backdropColor.r, backdropColor.g, backdropColor.b, backdropColor.a * alpha);
        GUI.DrawTexture(new Rect(x, y, width, boxHeight), solid);

        if (speakerHeight > 0f)
        {
            var speakerRect = new Rect(x + padding, y + padding * 0.6f, width - padding * 2f, speakerHeight);
            DrawLabel(speakerRect, currentSpeaker, speakerStyle,
                new Color(speakerColor.r, speakerColor.g, speakerColor.b, speakerColor.a * alpha));
        }

        var textRect = new Rect(x + padding, y + padding * 0.6f + speakerHeight, width - padding * 2f, textHeight);
        DrawLabel(textRect, shown, lineStyle, new Color(lineColor.r, lineColor.g, lineColor.b, lineColor.a * alpha));

        GUI.color = previous;
    }

    private void DrawLabel(Rect rect, string text, GUIStyle style, Color color)
    {
        GUI.color = new Color(0f, 0f, 0f, color.a * 0.9f);
        GUI.Label(new Rect(rect.x + 1.5f, rect.y + 1.5f, rect.width, rect.height), text, style);
        GUI.color = color;
        GUI.Label(rect, text, style);
    }

    private void EnsureStyles()
    {
        if (lineStyle != null) return;

        speakerStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = speakerFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.UpperLeft
        };
        speakerStyle.normal.textColor = Color.white;

        lineStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = lineFontSize,
            alignment = TextAnchor.UpperLeft,
            wordWrap = true
        };
        lineStyle.normal.textColor = Color.white;
    }

    private static Texture2D BuildSolidTexture()
    {
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return tex;
    }
}
