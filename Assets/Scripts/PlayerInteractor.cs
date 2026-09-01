using UnityEngine;

/// <summary>
/// Finds the nearest IInteractable (chairs, doors, ...) in range each frame, shows a prompt,
/// and calls Interact() on it when the player presses the interact key.
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactRange = 2.5f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private Transform lookTransform;
    [Tooltip("Meters of score penalty per degree off-center. Lets an object you're looking straight at win over a merely-closer one that's off to the side (e.g. two doors placed within range of each other).")]
    [SerializeField] private float anglePenaltyPerDegree = 0.025f;

    [Header("Prompt")]
    [SerializeField] private int promptFontSize = 24;
    [SerializeField] private float promptBottomOffset = 120f;
    [Tooltip("The dialogue box sits low on the screen, so the prompt moves up out of its way " +
             "while a line is playing.")]
    [SerializeField] private float promptBottomOffsetDuringDialogue = 230f;

    private IInteractable nearest;
    private GUIStyle promptStyle;
    private GUIStyle promptShadowStyle;

    private void Awake()
    {
        if (lookTransform == null)
        {
            var cam = GetComponentInChildren<Camera>(true);
            lookTransform = cam != null ? cam.transform : transform;
        }
    }

    private void Update()
    {
        nearest = FindNearest();

        if (nearest != null && Input.GetKeyDown(interactKey))
        {
            nearest.Interact(gameObject);
        }
    }

    private IInteractable FindNearest()
    {
        IInteractable best = null;
        float bestScore = float.MaxValue;
        Vector3 origin = lookTransform.position;
        Vector3 forward = lookTransform.forward;

        foreach (var interactable in InteractableRegistry.All)
        {
            if (interactable == null || interactable.InteractTransform == null) continue;

            Vector3 toTarget = interactable.InteractTransform.position - origin;
            float dist = toTarget.magnitude;
            if (dist > interactRange) continue;

            float angle = dist > 0.01f ? Vector3.Angle(forward, toTarget) : 0f;
            float score = dist + angle * anglePenaltyPerDegree;
            if (score < bestScore)
            {
                bestScore = score;
                best = interactable;
            }
        }

        return best;
    }

    private void OnGUI()
    {
        if (nearest == null) return;
        string text = nearest.GetPrompt();
        if (string.IsNullOrEmpty(text)) return;

        if (promptStyle == null)
        {
            promptStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = promptFontSize
            };
            promptStyle.normal.textColor = Color.white;

            promptShadowStyle = new GUIStyle(promptStyle);
            promptShadowStyle.normal.textColor = Color.black;
        }

        float width = 320f;
        float height = 40f;
        float bottom = DialogueHUD.Instance != null && DialogueHUD.Instance.IsPlaying
            ? promptBottomOffsetDuringDialogue
            : promptBottomOffset;
        Rect rect = new Rect((Screen.width - width) / 2f, Screen.height - bottom, width, height);
        Rect shadowRect = new Rect(rect.x + 2f, rect.y + 2f, rect.width, rect.height);

        GUI.Label(shadowRect, text, promptShadowStyle);
        GUI.Label(rect, text, promptStyle);
    }
}
