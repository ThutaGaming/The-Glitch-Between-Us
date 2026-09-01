using UnityEngine;

/// <summary>
/// Shows the item the player is carrying as a first-person "viewmodel": the real world object is
/// re-parented to the camera and parked in the lower-right of the view, with a gentle idle sway
/// so it reads as being held in a hand rather than pasted onto the screen.
///
/// Put this on the player's camera. There is no visible player body in this game, so the item
/// itself is the only thing that sells the carrying.
/// </summary>
public class HeldItemView : MonoBehaviour
{
    public static HeldItemView Instance { get; private set; }

    [Header("Hold Pose (local to the camera)")]
    [Tooltip("Right / down / forward from the camera. Forward must exceed the camera's near " +
             "clip plane (0.3) or the item is clipped away.")]
    [SerializeField] private Vector3 holdLocalPosition = new Vector3(0.30f, -0.22f, 0.62f);
    [SerializeField] private Vector3 holdLocalEuler = new Vector3(-12f, -155f, 10f);
    [Tooltip("Scale applied while held. 1 keeps the object's own size.")]
    [SerializeField] private float holdScale = 1f;

    [Header("Pick-Up Motion")]
    [SerializeField] private float raiseDuration = 0.28f;

    [Header("Idle Sway")]
    [SerializeField] private float swayAmount = 0.006f;
    [SerializeField] private float swaySpeed = 1.7f;
    [SerializeField] private float tiltAmount = 1.4f;

    private Transform held;
    private Transform originalParent;
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    private Vector3 originalLocalScale;
    private Collider[] disabledColliders;
    private float raiseTime;

    public bool IsHolding => held != null;
    public Transform HeldItem => held;

    private void Awake() => Instance = this;
    private void OnDestroy() { if (Instance == this) Instance = null; }

    /// <summary>Takes the object out of the world and into the player's hands. False if already full.</summary>
    public bool Hold(Transform item)
    {
        if (held != null || item == null) return false;

        held = item;
        originalParent = item.parent;
        originalLocalPosition = item.localPosition;
        originalLocalRotation = item.localRotation;
        originalLocalScale = item.localScale;

        // A collider riding on the camera would shove the player and the scenery around.
        disabledColliders = item.GetComponentsInChildren<Collider>(true);
        foreach (var c in disabledColliders) c.enabled = false;

        item.SetParent(transform, false);
        item.localScale = originalLocalScale * holdScale;
        item.localRotation = Quaternion.Euler(holdLocalEuler);
        // Start low and swing up into view.
        item.localPosition = holdLocalPosition + new Vector3(0f, -0.25f, -0.1f);
        raiseTime = 0f;
        return true;
    }

    /// <summary>Puts the item back exactly where it came from.</summary>
    public void Release()
    {
        if (held == null) return;

        held.SetParent(originalParent, false);
        held.localPosition = originalLocalPosition;
        held.localRotation = originalLocalRotation;
        held.localScale = originalLocalScale;
        RestoreColliders();
        held = null;
    }

    /// <summary>
    /// Hands the item off (into a bag, say): it leaves the hands and is hidden, but is restored
    /// to its original transform first so re-enabling it later puts it back sensibly.
    /// </summary>
    public Transform Consume()
    {
        if (held == null) return null;

        Transform item = held;
        Release();
        item.gameObject.SetActive(false);
        return item;
    }

    private void RestoreColliders()
    {
        if (disabledColliders == null) return;
        foreach (var c in disabledColliders) if (c != null) c.enabled = true;
        disabledColliders = null;
    }

    private void LateUpdate()
    {
        if (held == null) return;

        raiseTime += Time.deltaTime;
        float raise = raiseDuration > 0f ? Smooth01(raiseTime / raiseDuration) : 1f;

        float t = Time.time * swaySpeed;
        Vector3 sway = new Vector3(Mathf.Sin(t) * swayAmount, Mathf.Sin(t * 1.6f) * swayAmount * 0.7f, 0f);

        Vector3 from = holdLocalPosition + new Vector3(0f, -0.25f, -0.1f);
        held.localPosition = Vector3.Lerp(from, holdLocalPosition + sway, raise);
        held.localRotation = Quaternion.Euler(holdLocalEuler +
            new Vector3(Mathf.Sin(t * 1.3f) * tiltAmount, Mathf.Sin(t) * tiltAmount, 0f));
    }

    /// <summary>GLSL-style smoothstep on a normalised t (Mathf.SmoothStep is a smoothed lerp).</summary>
    private static float Smooth01(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3f - 2f * t);
    }
}
