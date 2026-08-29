using UnityEngine;

/// <summary>
/// Finds the nearest IInteractable (chairs, doors, ...) in range each frame, shows a prompt,
/// and calls Interact() on it when the player presses the interact key.
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactRange = 2.5f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Prompt")]
    [SerializeField] private int promptFontSize = 24;
    [SerializeField] private float promptBottomOffset = 120f;

    private IInteractable nearest;
    private GUIStyle promptStyle;
    private GUIStyle promptShadowStyle;

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
        float bestDist = interactRange;

        foreach (var interactable in InteractableRegistry.All)
        {
            if (interactable == null || interactable.InteractTransform == null) continue;
            float dist = Vector3.Distance(transform.position, interactable.InteractTransform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
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
        Rect rect = new Rect((Screen.width - width) / 2f, Screen.height - promptBottomOffset, width, height);
        Rect shadowRect = new Rect(rect.x + 2f, rect.y + 2f, rect.width, rect.height);

        GUI.Label(shadowRect, text, promptShadowStyle);
        GUI.Label(rect, text, promptStyle);
    }
}
