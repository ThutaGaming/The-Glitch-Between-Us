using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The "VITALS" panel: the red bar and the "current / max" label follow PlayerHealth. After a hit
/// the bar drains down to the new value over a moment instead of snapping, and refills the same
/// way while health regenerates.
/// </summary>
public sealed class SciFiHealthHud : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image fill;
    public TextMeshProUGUI label;
    [Tooltip("How quickly the bar catches up with the player's health. Higher is snappier; 6 takes about half a second.")]
    [SerializeField] private float drainSharpness = 6f;

    private static Sprite solidSprite;
    private float shown = -1f;

    private void Awake()
    {
        // A UI Image with no sprite ignores fillAmount and always draws full, which kept the bar
        // at 100% while only the number went down. A plain white sprite makes Filled mode work.
        if (fill != null && fill.sprite == null)
        {
            if (solidSprite == null)
            {
                var white = Texture2D.whiteTexture;
                solidSprite = Sprite.Create(white, new Rect(0f, 0f, white.width, white.height), new Vector2(0.5f, 0.5f));
            }
            fill.sprite = solidSprite;
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
    }

    private void Update()
    {
        if (playerHealth == null) playerHealth = FindPlayerHealth();
        if (playerHealth == null) return;

        float target = playerHealth.Normalized;
        // Unscaled, so the bar still finishes draining if the death screen stops time.
        shown = shown < 0f ? target : Mathf.Lerp(shown, target, 1f - Mathf.Exp(-drainSharpness * Time.unscaledDeltaTime));
        if (Mathf.Abs(shown - target) < 0.001f) shown = target;

        if (fill != null) fill.fillAmount = shown;
        if (label != null) label.text = playerHealth.CurrentHealth + " / " + playerHealth.maxHealth;
    }

    // Same lookup the enemies use (GetComponent on the Player-tagged object), so the HUD always
    // shows the PlayerHealth that actually takes the hits.
    private static PlayerHealth FindPlayerHealth()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var health = player != null ? player.GetComponent<PlayerHealth>() : null;
        return health != null ? health : FindFirstObjectByType<PlayerHealth>();
    }
}
