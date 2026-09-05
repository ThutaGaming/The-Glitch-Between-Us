using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// One friend the player can pick out of the talk menu. Lines are editable per friend in the
/// Inspector; English on purpose, since Unity's IMGUI cannot shape Burmese.
/// </summary>
[System.Serializable]
public class ConversationFriend
{
    [Tooltip("Shown in the menu and used as the dialogue speaker name.")]
    public string displayName = "Friend";
    public Transform friend;
    [TextArea(1, 3)]
    public string[] lines = { "Hey, morning!" };

    // Runtime only - resets every play session, never baked into the scene.
    [System.NonSerialized] public bool hasTalked;
}

/// <summary>
/// Stand near the group and press E to open a talk menu: 1/2/3 picks a friend, Esc leaves.
/// Picking someone turns the player toward them, turns them toward the player, and plays their
/// lines through <see cref="DialogueHUD"/>. Anyone already spoken to stays greyed out for the
/// rest of the session, so re-opening the menu only offers whoever is left.
/// </summary>
public class FriendGroupConversation : MonoBehaviour, IInteractable
{
    [Header("Friends (menu order = keys 1, 2, 3)")]
    [SerializeField] private ConversationFriend[] friends = new ConversationFriend[0];

    [Header("Prompt")]
    [SerializeField] private string prompt = "(E) Talk";
    [Tooltip("Shown once everyone has been spoken to. Leave empty to hide the prompt entirely.")]
    [SerializeField] private string allDonePrompt = "";
    [Tooltip("Optional. Where the interact prompt measures from; defaults to this transform.")]
    [SerializeField] private Transform interactAnchor;

    [Header("Turning")]
    [SerializeField] private float playerTurnDuration = 0.55f;
    [SerializeField] private float friendTurnDuration = 0.45f;
    [Tooltip("Where on the friend the camera aims, measured up their renderer bounds: " +
             "0 = mid-body, 1 = top of the head. Bounds rather than the pivot, because these " +
             "character pivots are not all at foot level.")]
    [Range(0f, 1f)]
    [SerializeField] private float aimHeightFraction = 0.7f;

    [Header("Dialogue")]
    [SerializeField] private float delayBeforeFirstLine = 0.25f;
    [SerializeField] private float holdPerLine = 2.4f;
    [Tooltip("Pause after the last line before the menu comes back.")]
    [SerializeField] private float delayAfterLastLine = 0.4f;

    [Tooltip("Fires each time a conversation finishes, whichever friend it was.")]
    public UnityEvent onFriendTalked;

    [Header("Menu Look")]
    [SerializeField] private float menuWidth = 440f;
    [SerializeField] private int headerFontSize = 16;
    [SerializeField] private int entryFontSize = 21;
    [SerializeField] private Color headerColor = new Color(0.75f, 0.78f, 0.85f, 1f);
    [SerializeField] private Color entryColor = Color.white;
    [SerializeField] private Color doneColor = new Color(0.55f, 0.58f, 0.62f, 1f);
    [SerializeField] private Color backdropColor = new Color(0f, 0f, 0f, 0.72f);
    [SerializeField] private Color accentColor = new Color(0.95f, 0.82f, 0.35f, 1f);

    private Transform playerBody;
    private Transform playerCamera;
    private MouseLook mouseLook;
    private PlayerMovement movement;
    private PlayerInteractor interactor;

    private bool menuOpen;
    private bool talking;

    private Texture2D solid;
    private GUIStyle headerStyle;
    private GUIStyle entryStyle;

    public Transform InteractTransform => interactAnchor != null ? interactAnchor : transform;

    private void Awake() => solid = BuildSolidTexture();

    private void OnDestroy()
    {
        if (solid != null) Destroy(solid);
    }

    private void OnEnable() => InteractableRegistry.All.Add(this);
    private void OnDisable() => InteractableRegistry.All.Remove(this);

    public string GetPrompt()
    {
        if (menuOpen) return "";
        return AnyoneLeft() ? prompt : allDonePrompt;
    }

    public void Interact(GameObject player)
    {
        if (menuOpen || talking || player == null || !AnyoneLeft()) return;

        playerBody = player.transform;
        var cam = player.GetComponentInChildren<Camera>(true);
        playerCamera = cam != null ? cam.transform : null;
        mouseLook = player.GetComponentInChildren<MouseLook>(true);
        movement = player.GetComponent<PlayerMovement>();
        interactor = player.GetComponent<PlayerInteractor>();

        OpenMenu();
    }

    private void OpenMenu()
    {
        menuOpen = true;
        // MouseLook and PlayerMovement rewrite the camera every frame, so they have to be off
        // for the scripted turn; the interactor is off so E can't re-trigger mid-conversation.
        if (mouseLook != null) mouseLook.enabled = false;
        if (movement != null) movement.enabled = false;
        if (interactor != null) interactor.enabled = false;
    }

    private void CloseMenu()
    {
        menuOpen = false;

        // Back to a clean pose before MouseLook re-seeds its pitch from the camera's euler.
        if (playerCamera != null) playerCamera.localRotation = Quaternion.identity;
        if (mouseLook != null) mouseLook.enabled = true;
        if (movement != null) movement.enabled = true;
        if (interactor != null) interactor.enabled = true;
    }

    private void Update()
    {
        if (!menuOpen || talking) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMenu();
            return;
        }

        for (int i = 0; i < friends.Length && i < 9; i++)
        {
            if (!Input.GetKeyDown(KeyCode.Alpha1 + i)) continue;

            var entry = friends[i];
            if (entry == null || entry.hasTalked || entry.friend == null) continue;

            StartCoroutine(TalkRoutine(entry));
            return;
        }
    }

    private IEnumerator TalkRoutine(ConversationFriend entry)
    {
        talking = true;

        Vector3 lookTarget = AimPointOn(entry.friend);

        // Player turns to them (body yaw + camera pitch, the same split WashFaceStation uses,
        // since MouseLook owns yaw on the body and pitch on the camera).
        yield return TurnPlayerTowards(lookTarget);

        // ...and they turn back to the player.
        yield return TurnFriendTowards(entry.friend, playerBody != null ? playerBody.position : transform.position);

        if (delayBeforeFirstLine > 0f) yield return new WaitForSeconds(delayBeforeFirstLine);

        var dialogue = DialogueHUD.Instance;
        if (dialogue != null && entry.lines != null)
        {
            foreach (string line in entry.lines)
            {
                if (!string.IsNullOrEmpty(line)) dialogue.Say(entry.displayName, line, holdPerLine);
            }

            // Let the queue start before waiting on it, or IsPlaying is still false.
            yield return null;
            yield return new WaitWhile(() => dialogue.IsPlaying);
        }

        if (delayAfterLastLine > 0f) yield return new WaitForSeconds(delayAfterLastLine);

        entry.hasTalked = true;
        talking = false;
        onFriendTalked?.Invoke();

        // Nobody left to pick - drop straight out instead of showing an empty menu.
        if (!AnyoneLeft()) CloseMenu();
    }

    private IEnumerator TurnPlayerTowards(Vector3 worldPoint)
    {
        if (playerBody == null) yield break;

        Vector3 flat = worldPoint - playerBody.position;
        flat.y = 0f;

        Quaternion fromBody = playerBody.rotation;
        Quaternion toBody = flat.sqrMagnitude < 0.0001f
            ? fromBody
            : Quaternion.LookRotation(flat, Vector3.up);

        Quaternion fromCam = playerCamera != null ? playerCamera.localRotation : Quaternion.identity;

        float t = 0f;
        while (t < playerTurnDuration)
        {
            t += Time.deltaTime;
            float p = Smooth01(t / playerTurnDuration);

            playerBody.rotation = Quaternion.Slerp(fromBody, toBody, p);

            if (playerCamera != null)
            {
                // Recomputed each frame so the pitch stays true while the body swings around.
                Quaternion toCam = PitchTowards(playerCamera, worldPoint);
                playerCamera.localRotation = Quaternion.Slerp(fromCam, toCam, p);
            }

            yield return null;
        }

        playerBody.rotation = toBody;
        if (playerCamera != null) playerCamera.localRotation = PitchTowards(playerCamera, worldPoint);
    }

    /// <summary>
    /// Face-height point on a friend, measured from their combined renderer bounds. These
    /// character pivots sit at different heights (NaingKoKoOo's is chest-high, the others are
    /// at the feet), so anything measured off <c>transform.position</c> aims wildly high or low.
    /// </summary>
    private Vector3 AimPointOn(Transform friend)
    {
        var renderers = friend.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0) return friend.position;

        Bounds b = renderers[0].bounds;
        foreach (var r in renderers) b.Encapsulate(r.bounds);

        return new Vector3(b.center.x, Mathf.Lerp(b.center.y, b.max.y, aimHeightFraction), b.center.z);
    }

    /// <summary>Camera-local rotation that aims at a world point from where the camera is now.</summary>
    private static Quaternion PitchTowards(Transform cam, Vector3 worldPoint)
    {
        Vector3 toTarget = worldPoint - cam.position;
        float horizontal = new Vector2(toTarget.x, toTarget.z).magnitude;
        float pitch = Mathf.Atan2(-toTarget.y, Mathf.Max(0.01f, horizontal)) * Mathf.Rad2Deg;
        return Quaternion.Euler(Mathf.Clamp(pitch, -85f, 85f), 0f, 0f);
    }

    private IEnumerator TurnFriendTowards(Transform friend, Vector3 worldPoint)
    {
        Vector3 flat = worldPoint - friend.position;
        flat.y = 0f;
        if (flat.sqrMagnitude < 0.0001f) yield break;

        Quaternion from = friend.rotation;
        Quaternion to = Quaternion.LookRotation(flat, Vector3.up);

        float t = 0f;
        while (t < friendTurnDuration)
        {
            t += Time.deltaTime;
            friend.rotation = Quaternion.Slerp(from, to, Smooth01(t / friendTurnDuration));
            yield return null;
        }

        friend.rotation = to;
    }

    private bool AnyoneLeft()
    {
        foreach (var f in friends)
        {
            if (f != null && !f.hasTalked && f.friend != null) return true;
        }
        return false;
    }

    private void OnGUI()
    {
        if (!menuOpen || talking) return;

        EnsureStyles();

        float padding = 14f;
        float headerHeight = headerFontSize + 12f;
        float rowHeight = entryFontSize + 12f;
        float panelHeight = headerHeight + rowHeight * (friends.Length + 1) + padding * 2f;

        float x = (Screen.width - menuWidth) * 0.5f;
        float y = (Screen.height - panelHeight) * 0.5f;

        Color previous = GUI.color;

        GUI.color = backdropColor;
        GUI.DrawTexture(new Rect(x, y, menuWidth, panelHeight), solid);
        GUI.color = accentColor;
        GUI.DrawTexture(new Rect(x, y, 3f, panelHeight), solid);

        float textX = x + padding + 6f;
        float textWidth = menuWidth - padding * 2f - 6f;

        DrawLabel(new Rect(textX, y + padding, textWidth, headerHeight), "TALK TO", headerStyle, headerColor);

        float rowY = y + padding + headerHeight;
        for (int i = 0; i < friends.Length; i++)
        {
            var entry = friends[i];
            string name = entry != null ? entry.displayName : "-";
            bool done = entry == null || entry.hasTalked || entry.friend == null;
            string text = "[" + (i + 1) + "]  " + name + (done ? "   (done)" : "");

            DrawLabel(new Rect(textX, rowY, textWidth, rowHeight), text, entryStyle, done ? doneColor : entryColor);
            rowY += rowHeight;
        }

        DrawLabel(new Rect(textX, rowY, textWidth, rowHeight), "[Esc]  Leave", entryStyle, headerColor);

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
        if (headerStyle != null) return;

        headerStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = headerFontSize,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft
        };
        headerStyle.normal.textColor = Color.white;

        entryStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = entryFontSize,
            alignment = TextAnchor.MiddleLeft
        };
        entryStyle.normal.textColor = Color.white;
    }

    /// <summary>GLSL-style smoothstep on an already-normalised t.</summary>
    private static float Smooth01(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3f - 2f * t);
    }

    private static Texture2D BuildSolidTexture()
    {
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return tex;
    }
}
