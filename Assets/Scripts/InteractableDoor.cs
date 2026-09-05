using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Swings this transform open/closed around its local Y axis on interact. Assumes the
/// transform's pivot already sits at the hinge edge (true for the Door_1m_A_left leaf
/// meshes) — rotating a center-pivoted mesh would look wrong. `openAngle`'s magnitude sets
/// how far the door swings; its sign is picked dynamically each time you open it, based on
/// which side of the door the player is standing on, so the leaf always swings away from
/// whoever opened it rather than through them.
/// </summary>
public class InteractableDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openDuration = 0.8f;

    [Header("Lock")]
    [SerializeField] private bool isLocked;
    [SerializeField] private string lockedPrompt = "(Locked)";

    [Header("Audio")]
    [Tooltip("Played whenever the door actually swings open or closed (not when skipOpenAnimation short-circuits it).")]
    [SerializeField] private AudioSource doorSound;

    /// <summary>Fired every time the door opens (not on close).</summary>
    public UnityEvent onOpened;

    /// <summary>
    /// When true, the next open skips the swing animation entirely and just fires
    /// <see cref="onOpened"/> — for callers (e.g. a scripted scene exit) that take over what
    /// "opening" means and don't want the slab visibly moving.
    /// </summary>
    [SerializeField] private bool skipOpenAnimation;

    private Quaternion closedRotation;
    private BoxCollider slabCollider;
    private bool isOpen;
    private Coroutine animRoutine;

    public Transform InteractTransform => transform;
    public bool IsLocked => isLocked;
    public void SetLocked(bool value) => isLocked = value;
    public void SetSkipOpenAnimation(bool value) => skipOpenAnimation = value;

    private void Awake()
    {
        closedRotation = transform.localRotation;
        slabCollider = GetComponent<BoxCollider>();
    }

    private void OnEnable() => InteractableRegistry.All.Add(this);
    private void OnDisable() => InteractableRegistry.All.Remove(this);

    public string GetPrompt() => isLocked ? lockedPrompt : (isOpen ? "(E) Close Door" : "(E) Open Door");

    public void Interact(GameObject player)
    {
        if (isLocked) return;

        isOpen = !isOpen;

        if (isOpen && skipOpenAnimation)
        {
            onOpened?.Invoke();
            return;
        }

        Quaternion target = isOpen ? GetOpenRotation(player) : closedRotation;
        if (animRoutine != null) StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(Animate(target));
        if (doorSound != null) doorSound.Play();
        if (isOpen) onOpened?.Invoke();
    }

    /// <summary>
    /// Picks whichever swing direction (+/- openAngle) ends up farther from the player,
    /// using the collider's horizontal offset from the hinge as the slab's swing arm.
    /// Falls back to the serialized sign of openAngle when there's no collider to measure with.
    /// </summary>
    private Quaternion GetOpenRotation(GameObject player)
    {
        float magnitude = Mathf.Abs(openAngle);
        if (slabCollider == null || player == null)
        {
            return closedRotation * Quaternion.Euler(0f, openAngle, 0f);
        }

        Vector3 armLocal = new Vector3(slabCollider.center.x, 0f, slabCollider.center.z);
        Vector3 hingePos = transform.position;

        Vector3 armPositive = transform.rotation * Quaternion.Euler(0f, magnitude, 0f) * armLocal;
        Vector3 armNegative = transform.rotation * Quaternion.Euler(0f, -magnitude, 0f) * armLocal;

        float distPositive = Vector3.Distance(hingePos + armPositive, player.transform.position);
        float distNegative = Vector3.Distance(hingePos + armNegative, player.transform.position);

        float chosenAngle = distPositive >= distNegative ? magnitude : -magnitude;
        return closedRotation * Quaternion.Euler(0f, chosenAngle, 0f);
    }

    private IEnumerator Animate(Quaternion target)
    {
        Quaternion start = transform.localRotation;
        float t = 0f;
        while (t < openDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / openDuration));
            transform.localRotation = Quaternion.Slerp(start, target, p);
            yield return null;
        }

        transform.localRotation = target;
    }
}
