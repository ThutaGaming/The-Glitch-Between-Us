using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The trip home: once class is over, "Go home" becomes the objective and Door_2m_A lights up.
/// Opening it cuts to a black screen with a caption line and the same street sound as
/// SchoolExitSequence's morning trip, then loads Bedroom Scene with the player placed at a
/// fixed spot. Mirrors SchoolExitSequence's shape and caption style.
///
/// Hook <see cref="Begin"/> to ThihaTeachingSequence.onLeft in the Inspector.
/// Lines are English because Unity's IMGUI/TextMeshPro cannot shape Burmese correctly.
/// </summary>
public class SchoolHomeSequence : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InteractableDoor homeDoor;
    [SerializeField] private ObjectiveGlow doorGlow;
    [SerializeField] private MissionHUD mission;
    [Tooltip("Disabled for the black-screen beat (same as SchoolExitSequence) - MouseLook in " +
             "particular must not keep reading input here, or the player body quietly drifts " +
             "for however long the beat runs before School Scene unloads it.")]
    [SerializeField] private Behaviour[] disableDuringSequence;

    [Header("Objective")]
    [SerializeField] private string objective = "Go home";

    [Header("Audio")]
    [Tooltip("Same clip as SchoolExitSequence's streetSound (Assets/SFX/Street Sound Effect  High-quality.mp3) - can't reference across scenes, so this scene gets its own AudioSource with the same clip.")]
    [SerializeField] private AudioSource streetSound;

    [Header("Caption")]
    [SerializeField] private string headingHomeLine = "Heading home now.";
    [SerializeField] private float captionHold = 2.0f;
    [Tooltip("Characters revealed per second.")]
    [SerializeField] private float typeSpeed = 22f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private int captionFontSize = 28;

    [Header("Scene / Spawn")]
    [SerializeField] private string bedroomSceneName = "Bedroom Scene";
    [SerializeField] private Vector3 spawnPosition = new Vector3(12.493f, 2.71384f, 239.43f);
    [SerializeField] private float spawnYaw = 84.141f;

    [Header("Debug")]
    [Tooltip("Press this key in Play Mode to skip straight to this sequence (Begin()) without " +
             "talking to a friend / sitting through class first - for testing only.")]
    [SerializeField] private bool debugSkipKeyEnabled = true;
    [SerializeField] private KeyCode debugSkipKey = KeyCode.F9;

    private bool started;
    private float overlayAlpha;
    private string captionText = "";
    private int revealedChars;

    private Texture2D solid;
    private GUIStyle captionStyle;

    private static bool hasPendingSpawn;
    private static Vector3 pendingSpawnPosition;
    private static Quaternion pendingSpawnRotation;

    private void Awake()
    {
        solid = BuildSolidTexture();

        if (mission == null) mission = MissionHUD.Instance != null
            ? MissionHUD.Instance
            : FindFirstObjectByType<MissionHUD>();

        if (homeDoor != null)
        {
            homeDoor.SetLocked(true);
            homeDoor.SetSkipOpenAnimation(true);
        }
    }

    private void OnDestroy()
    {
        if (homeDoor != null) homeDoor.onOpened.RemoveListener(OnDoorOpened);
        if (solid != null) Destroy(solid);
    }

    private void Update()
    {
        if (debugSkipKeyEnabled && !started && Input.GetKeyDown(debugSkipKey)) Begin();
    }

    /// <summary>Shows "Go home" and lights up the door. Hook to ThihaTeachingSequence.onLeft.</summary>
    public void Begin()
    {
        if (started) return;
        started = true;

        if (homeDoor != null)
        {
            homeDoor.SetLocked(false);
            homeDoor.onOpened.AddListener(OnDoorOpened);
        }

        if (mission != null && !string.IsNullOrEmpty(objective)) mission.SetObjective(objective);
        if (doorGlow != null) doorGlow.SetGlowing(true);
    }

    private void OnDoorOpened()
    {
        homeDoor.onOpened.RemoveListener(OnDoorOpened);

        if (mission != null) mission.CompleteObjective();
        if (doorGlow != null) doorGlow.SetGlowing(false);

        StartCoroutine(LeaveRoutine());
    }

    private IEnumerator LeaveRoutine()
    {
        SetControlEnabled(false);

        if (streetSound != null) streetSound.Play();

        yield return FadeOverlay(0f, 1f, fadeDuration);

        yield return ShowCaption(headingHomeLine);

        pendingSpawnPosition = spawnPosition;
        pendingSpawnRotation = Quaternion.Euler(0f, spawnYaw, 0f);
        hasPendingSpawn = true;
        SceneManager.sceneLoaded += ApplyPendingSpawn;

        SceneManager.LoadScene(bedroomSceneName);
    }

    /// <summary>
    /// Static so it survives this object being destroyed when School Scene unloads - a plain
    /// instance method subscribed to SceneManager.sceneLoaded would try to run on a MonoBehaviour
    /// whose native object is already gone.
    /// </summary>
    private static void ApplyPendingSpawn(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= ApplyPendingSpawn;
        if (!hasPendingSpawn) return;
        hasPendingSpawn = false;

        var player = GameObject.Find("Player");
        if (player == null) return;

        // Bedroom Scene's own Start()-based morning intro (bed animation, "it's 8 o'clock"
        // dialogue) would otherwise fire right after this and drag the player back to bed -
        // it doesn't know this load is an afternoon homecoming, not a fresh morning. Disabling
        // them here (still before their first Start(), since sceneLoaded fires after
        // Awake/OnEnable but before Start) skips them entirely for this load.
        var wakeUp = player.GetComponent<PlayerWakeUpSequence>();
        if (wakeUp != null) wakeUp.enabled = false;
        var intro = Object.FindFirstObjectByType<GameIntroSequence>(FindObjectsInactive.Include);
        if (intro != null) intro.enabled = false;

        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.SetPositionAndRotation(pendingSpawnPosition, pendingSpawnRotation);
        if (cc != null) cc.enabled = true;

        // MouseLook/PlayerMovement on this freshly-loaded Player are a different instance from
        // whatever School Scene disabled before the load - they start enabled by default and
        // would otherwise immediately start reading input again, drifting the pose set above.
        // A short delayed re-enable (via a throwaway helper, since this static method has no
        // MonoBehaviour to run a coroutine on) gives one clean settled frame before control returns.
        var mouseLook = player.GetComponent<MouseLook>();
        var movement = player.GetComponent<PlayerMovement>();
        if (mouseLook != null) mouseLook.enabled = false;
        if (movement != null) movement.enabled = false;

        var helperGo = new GameObject("~SpawnSettleHelper");
        helperGo.AddComponent<SpawnSettleHelper>().Init(mouseLook, movement);

        // Homecoming beat: Thuta comments on the day, then "sit at the laptop" becomes the
        // objective. GameIntroSequence is disabled above so this is the only dialogue that
        // plays on this particular load.
        var arrival = Object.FindFirstObjectByType<HomeArrivalSequence>(FindObjectsInactive.Include);
        if (arrival != null) arrival.Begin();
    }

    /// <summary>Re-enables the given Behaviours one frame later, then removes itself.</summary>
    private class SpawnSettleHelper : MonoBehaviour
    {
        private Behaviour a, b;

        public void Init(Behaviour a, Behaviour b)
        {
            this.a = a;
            this.b = b;
            StartCoroutine(Reenable());
        }

        private IEnumerator Reenable()
        {
            yield return null;
            if (a != null) a.enabled = true;
            if (b != null) b.enabled = true;
            Destroy(gameObject);
        }
    }

    private void SetControlEnabled(bool enabled)
    {
        if (disableDuringSequence == null) return;
        foreach (var b in disableDuringSequence)
        {
            if (b != null) b.enabled = enabled;
        }
    }

    private IEnumerator ShowCaption(string text)
    {
        captionText = text;
        revealedChars = 0;

        float typed = 0f;
        while (revealedChars < text.Length)
        {
            typed += typeSpeed * Time.deltaTime;
            revealedChars = Mathf.Min(text.Length, Mathf.FloorToInt(typed));
            yield return null;
        }

        yield return new WaitForSeconds(captionHold);
    }

    private IEnumerator FadeOverlay(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            overlayAlpha = Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));
            yield return null;
        }
        overlayAlpha = to;
    }

    private void OnGUI()
    {
        if (overlayAlpha <= 0.001f) return;

        EnsureStyle();

        Color previous = GUI.color;

        GUI.color = new Color(0f, 0f, 0f, overlayAlpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), solid);

        if (!string.IsNullOrEmpty(captionText))
        {
            string shown = captionText.Substring(0, Mathf.Clamp(revealedChars, 0, captionText.Length));
            GUI.color = new Color(1f, 1f, 1f, overlayAlpha);
            GUI.Label(new Rect(0, Screen.height * 0.5f - 40f, Screen.width, 80f), shown, captionStyle);
        }

        GUI.color = previous;
    }

    private void EnsureStyle()
    {
        if (captionStyle != null) return;

        captionStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = captionFontSize,
            alignment = TextAnchor.MiddleCenter
        };
        captionStyle.normal.textColor = Color.white;
    }

    private static Texture2D BuildSolidTexture()
    {
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return tex;
    }
}
