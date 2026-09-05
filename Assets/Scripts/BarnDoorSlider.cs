using System.Collections;
using UnityEngine;

/// <summary>
/// Self-contained sliding barn door: resolves the player camera itself, measures its own
/// distance, draws its own "(E) Open Door" prompt and reads its own key press. Deliberately
/// independent of IInteractable / PlayerInteractor / InteractableRegistry so nothing outside
/// this file has to be wired up for it to work.
///
/// Attach to the door's moving "Hanger" child — the one carrying Wheel.01/Wheel.02 and Door,
/// whose local X axis runs along the rail.
/// </summary>
public class BarnDoorSlider : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactRange = 4f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Slide")]
    [SerializeField] private float openDuration = 0.8f;
    [Tooltip("Local -X is the rail's open side on these BarnDoor_03 prefabs. Tick to slide the other way.")]
    [SerializeField] private bool openTowardPositiveX = false;
    [Tooltip("0 = slide exactly one door width, measured from the mesh at Awake.")]
    [SerializeField] private float slideDistanceOverride = 0f;

    [Header("Lock")]
    [Tooltip("A locked door still shows a prompt, but E does nothing until it is unlocked.")]
    [SerializeField] private bool isLocked;
    [SerializeField] private string lockedPrompt = "Talk with your friends first";

    [Header("Blocking")]
    [Tooltip("Adds a BoxCollider sized to the door panel so the player can't walk through it " +
             "while closed. Disabled automatically while the door is open.")]
    [SerializeField] private bool addBlockingCollider = true;

    [Header("Prompt")]
    [SerializeField] private int promptFontSize = 24;
    [SerializeField] private float promptBottomOffset = 120f;

    // Only the nearest door owns the prompt and the key press each frame, so a stairwell with
    // six doors can't stack six labels on top of each other.
    private static BarnDoorSlider owner;
    private static float ownerDistance;
    private static int claimFrame = -1;

    private Renderer[] doorRenderers;
    private Camera playerCam;
    private BoxCollider blockingCollider;
    private Vector3 closedLocalPos;
    private float slideDistance;
    private bool isOpen;
    private Coroutine animRoutine;
    private GUIStyle promptStyle;
    private GUIStyle shadowStyle;

    private void Awake()
    {
        doorRenderers = GetComponentsInChildren<Renderer>();
        closedLocalPos = transform.localPosition;

        ComputeLocalBounds(out Vector3 localMin, out Vector3 localMax);
        slideDistance = slideDistanceOverride > 0f
            ? slideDistanceOverride
            : (localMax.x > localMin.x ? localMax.x - localMin.x : 1.2f);

        if (addBlockingCollider)
        {
            blockingCollider = GetComponent<BoxCollider>();
            if (blockingCollider == null) blockingCollider = gameObject.AddComponent<BoxCollider>();
            blockingCollider.center = (localMin + localMax) * 0.5f;
            blockingCollider.size = localMax - localMin;
            blockingCollider.enabled = !isOpen;
        }

        playerCam = ResolvePlayerCamera();
    }

    private void OnDisable()
    {
        if (owner == this) owner = null;
    }

    public bool IsLocked => isLocked;
    public void SetLocked(bool value) => isLocked = value;
    public void SetLockedPrompt(string value) => lockedPrompt = value;

    private void Update()
    {
        if (playerCam == null) playerCam = ResolvePlayerCamera();
        if (playerCam == null) return;

        if (claimFrame != Time.frameCount)
        {
            claimFrame = Time.frameCount;
            owner = null;
            ownerDistance = float.MaxValue;
        }

        float dist = Vector3.Distance(playerCam.transform.position, PanelCenter());
        if (dist <= interactRange && dist < ownerDistance)
        {
            ownerDistance = dist;
            owner = this;
        }
    }

    /// <summary>
    /// Input is read after every door has claimed in Update, so the nearest one wins the press
    /// regardless of the order Unity happens to tick the six components in.
    /// </summary>
    private void LateUpdate()
    {
        if (owner == this && !isLocked && Input.GetKeyDown(interactKey)) Toggle();
    }

    private void Toggle()
    {
        isOpen = !isOpen;
        float sign = openTowardPositiveX ? 1f : -1f;
        Vector3 target = isOpen
            ? closedLocalPos + new Vector3(sign * slideDistance, 0f, 0f)
            : closedLocalPos;
        if (blockingCollider != null) blockingCollider.enabled = !isOpen;
        if (animRoutine != null) StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(Animate(target));
    }

    private IEnumerator Animate(Vector3 target)
    {
        Vector3 start = transform.localPosition;
        float t = 0f;
        while (t < openDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / openDuration));
            transform.localPosition = Vector3.Lerp(start, target, p);
            yield return null;
        }

        transform.localPosition = target;
    }

    /// <summary>
    /// The centre of the door panel's combined mesh bounds — roughly chest height in the
    /// doorway. The transform itself sits on the rail hanger near the top of the frame, which
    /// is over 2m above a player standing at the threshold and therefore useless for range.
    /// </summary>
    private Vector3 PanelCenter()
    {
        if (doorRenderers == null || doorRenderers.Length == 0) return transform.position;

        Bounds b = doorRenderers[0].bounds;
        for (int i = 1; i < doorRenderers.Length; i++)
        {
            b.Encapsulate(doorRenderers[i].bounds);
        }
        return b.center;
    }

    /// <summary>
    /// Full local-space AABB (all three axes) of the door meshes, from world-space renderer
    /// bounds projected back into this transform's local space — correct regardless of this
    /// instance's world rotation/scale. Used for both the slide distance (X) and the blocking
    /// collider's center/size (all three axes).
    /// </summary>
    private void ComputeLocalBounds(out Vector3 localMin, out Vector3 localMax)
    {
        localMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        localMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        foreach (var r in doorRenderers)
        {
            Vector3 c = r.bounds.center;
            Vector3 e = r.bounds.extents;
            for (int sx = -1; sx <= 1; sx += 2)
                for (int sy = -1; sy <= 1; sy += 2)
                    for (int sz = -1; sz <= 1; sz += 2)
                    {
                        Vector3 local = transform.InverseTransformPoint(c + Vector3.Scale(e, new Vector3(sx, sy, sz)));
                        localMin = Vector3.Min(localMin, local);
                        localMax = Vector3.Max(localMax, local);
                    }
        }
    }

    private static Camera ResolvePlayerCamera()
    {
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null)
        {
            var cam = playerGo.GetComponentInChildren<Camera>(true);
            if (cam != null) return cam;
        }
        return Camera.main;
    }

    private void OnGUI()
    {
        if (owner != this) return;

        if (promptStyle == null)
        {
            promptStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = promptFontSize
            };
            promptStyle.normal.textColor = Color.white;

            shadowStyle = new GUIStyle(promptStyle);
            shadowStyle.normal.textColor = Color.black;
        }

        string text = isLocked ? lockedPrompt : (isOpen ? "(E) Close Door" : "(E) Open Door");
        float width = 320f;
        float height = 40f;
        Rect rect = new Rect((Screen.width - width) / 2f, Screen.height - promptBottomOffset, width, height);
        Rect shadowRect = new Rect(rect.x + 2f, rect.y + 2f, rect.width, rect.height);

        GUI.Label(shadowRect, text, shadowStyle);
        GUI.Label(rect, text, promptStyle);
    }
}
