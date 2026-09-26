using System.Collections;
using InfimaGames.LowPolyShooterPack;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// A level's way out. Once the exit is open (hook <see cref="Arm"/> to whatever opens it - Level
/// 1's Room3BlastGateController.onOpened, Training Ground's hologram, Level 2's intro), standing
/// in the doorway shows an [E] prompt. Pressing it ticks off the door objective, freezes the
/// player, holds a black screen for a moment and loads the next level, where that level's own
/// Player takes over.
///
/// The prompt matches TerminalInteractionBridge's so the levels read as one set of UI.
/// Text is English because Unity's IMGUI cannot shape Burmese correctly.
/// </summary>
public class LevelExitDoor : MonoBehaviour
{
    [Header("Doorway")]
    [Tooltip("Centre of the doorway on the floor; its forward points through the door.")]
    [SerializeField] private Transform zone;
    [Tooltip("Width (across the door) and depth (through it) of the area where E works, in metres.")]
    [SerializeField] private Vector2 zoneSize = new Vector2(15f, 6f);
    [Tooltip("How far above the zone's floor the player's eyes may be. Keeps a player on a lower " +
             "floor of a stacked level from using a door that is right above them.")]
    [SerializeField] private float zoneHeight = 3.5f;

    [Header("Objective")]
    [SerializeField] private MissionHUD mission;
    [SerializeField] private ObjectiveGlow doorGlow;
    [SerializeField] private string prompt = "[E]  ENTER LEVEL 2";

    [Header("Transition")]
    [SerializeField] private float fadeDuration = 0.6f;
    [Tooltip("How long the screen stays black before Level 2 loads.")]
    [SerializeField] private float blackHold = 1.5f;
    [SerializeField] private string nextSceneName = "Level 2";

    private bool armed;
    private bool leaving;
    private float overlayAlpha;
    private Texture2D solid;
    private GUIStyle promptStyle;
    private Camera playerCamera;

    /// <summary>Makes the doorway usable. Hook to Room3BlastGateController.onOpened.</summary>
    public void Arm() => armed = true;

    private void Awake()
    {
        solid = new Texture2D(1, 1, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        solid.SetPixel(0, 0, Color.white);
        solid.Apply();
        if (mission == null) mission = MissionHUD.Instance != null ? MissionHUD.Instance : FindFirstObjectByType<MissionHUD>();
    }

    private void OnDestroy()
    {
        if (solid != null) Destroy(solid);
    }

    private void Update()
    {
        if (!armed || leaving || !PlayerInZone()) return;
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) StartCoroutine(Leave());
    }

    private IEnumerator Leave()
    {
        leaving = true;
        if (mission != null) mission.CompleteObjective();
        if (doorGlow != null) doorGlow.SetGlowing(false);

        // Same freeze NetworkPuzzleTerminal uses: no walking, looking or firing during the fade.
        var player = playerCamera != null ? playerCamera.transform.root.gameObject : null;
        if (player != null)
        {
            var movement = player.GetComponent<Movement>();
            var look = player.GetComponentInChildren<CameraLook>();
            var input = player.GetComponent<PlayerInput>();
            if (movement != null) movement.enabled = false;
            if (look != null) look.enabled = false;
            if (input != null) input.enabled = false;
            var body = player.GetComponent<Rigidbody>();
            if (body != null && !body.isKinematic) body.linearVelocity = Vector3.zero;
        }

        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            overlayAlpha = t / fadeDuration;
            yield return null;
        }
        overlayAlpha = 1f;
        yield return new WaitForSeconds(blackHold);

        SceneManager.LoadScene(nextSceneName);
    }

    private bool PlayerInZone()
    {
        var cam = FindPlayerCamera();
        if (cam == null || zone == null) return false;
        // Measured along the zone's axes in metres, so a zone parented under a scaled door mesh
        // keeps the size typed into the Inspector.
        Vector3 offset = cam.transform.position - zone.position;
        float across = Vector3.Dot(offset, zone.right);
        float through = Vector3.Dot(offset, zone.forward);
        float up = Vector3.Dot(offset, zone.up);
        return Mathf.Abs(across) <= zoneSize.x * 0.5f && Mathf.Abs(through) <= zoneSize.y * 0.5f
            && up >= 0f && up <= zoneHeight;
    }

    private Camera FindPlayerCamera()
    {
        if (playerCamera != null && playerCamera.isActiveAndEnabled) return playerCamera;
        foreach (var camera in Camera.allCameras)
        {
            if (camera.isActiveAndEnabled && camera.cameraType == CameraType.Game && camera.CompareTag("MainCamera"))
            {
                playerCamera = camera;
                return camera;
            }
        }
        return null;
    }

    private void OnGUI()
    {
        if (overlayAlpha > 0.001f)
        {
            GUI.depth = -100;
            Color previous = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, overlayAlpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), solid);
            GUI.color = previous;
            return;
        }

        if (!armed || leaving || !PlayerInZone()) return;
        if (promptStyle == null)
        {
            promptStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 20, fontStyle = FontStyle.Bold };
            promptStyle.normal.textColor = new Color(0.2f, 0.95f, 1f, 1f);
        }
        GUI.Label(new Rect(Screen.width * 0.5f - 190f, Screen.height * 0.64f, 380f, 34f), prompt, promptStyle);
    }

    private void OnDrawGizmosSelected()
    {
        if (zone == null) return;
        Gizmos.matrix = Matrix4x4.TRS(zone.position, zone.rotation, Vector3.one);
        Gizmos.color = new Color(0.2f, 0.95f, 1f, 0.6f);
        Gizmos.DrawWireCube(Vector3.up * zoneHeight * 0.5f, new Vector3(zoneSize.x, zoneHeight, zoneSize.y));
    }
}
