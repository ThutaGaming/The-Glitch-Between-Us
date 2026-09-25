using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The return from the game world: after Level 5's glitch, Bedroom Scene loads with Thuta slumped
/// over Prop_Desk_02 in office_chair, asleep. He blinks awake, lifts his head, looks around, says
/// what happened, realises it's already 9 o'clock, gets up, and "Go to school" becomes the
/// objective with the front door (Door_1m_A_left) lit up and unlocked.
///
/// Triggered like SchoolHomeSequence's homecoming: <see cref="QueueForNextLoad"/> before loading
/// the scene, then a static sceneLoaded handler switches off the normal morning intro (bed
/// wake-up, "it's 8 o'clock" lines) before its Start() and begins this beat instead.
/// Lines are English because Unity's IMGUI/TextMeshPro cannot shape Burmese correctly.
/// </summary>
public class DeskWakeUpSequence : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SittableChair chair;
    [SerializeField] private Renderer desk;
    [SerializeField] private SchoolExitSequence schoolExit;

    [Header("Asleep on the desk")]
    [Tooltip("How far above the desk top the head (camera) rests.")]
    [SerializeField] private float headHeightAboveDesk = 0.13f;
    [Tooltip("How far past the desk's near edge the head rests.")]
    [SerializeField] private float headPastDeskEdge = 0.02f;
    [Tooltip("Camera tilt while the head lies on the desk: looking down at the keyboard, rolled onto its side.")]
    [SerializeField] private Vector3 sleepingCameraEuler = new Vector3(35f, 20f, 80f);

    [Header("Wall clock (shows the time he wakes up)")]
    [SerializeField] private Transform clock;
    [SerializeField] private Transform clockHourHand;
    [SerializeField] private Transform clockMinuteHand;
    [SerializeField] private int wakeHour = 9;
    [SerializeField] private int wakeMinute = 0;

    [Header("Timing (seconds)")]
    [SerializeField] private float blackHold = 1.0f;
    [SerializeField] private float holdAfterBlinking = 0.6f;
    [SerializeField] private float liftHeadDuration = 1.8f;
    [SerializeField] private float lookAroundDuration = 1.6f;

    [Header("Lines")]
    [SerializeField] private string speaker = "Thuta";
    [SerializeField] private string confusedLine = "What... what happened to me?";
    [Tooltip("Said while looking at the wall clock.")]
    [SerializeField] private string clockLine = "Ah! It's already 9 o'clock!";
    [SerializeField] private string hurryLine = "I'm going to be late for school... I have to go right now!";
    [SerializeField] private float holdPerLine = 1.6f;

    [Header("Objective")]
    [SerializeField] private string objective = "Go to school";

    [Header("Debug")]
    [Tooltip("Play this beat as soon as Bedroom Scene starts, without coming from Level 5 - for testing only.")]
    [SerializeField] private bool debugPlayOnStart;

    private static bool pending;

    private float overlayAlpha;
    private Texture2D solid;
    private bool begun;

    /// <summary>Call right before loading Bedroom Scene: the next load wakes Thuta at his desk.</summary>
    public static void QueueForNextLoad()
    {
        if (pending) return;
        pending = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>Static so it survives the old scene (and whoever called QueueForNextLoad) unloading.
    /// sceneLoaded runs after Awake/OnEnable but before Start, so the morning intro can still be
    /// switched off before it begins.</summary>
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var sequence = FindFirstObjectByType<DeskWakeUpSequence>(FindObjectsInactive.Include);
        if (sequence == null) return;   // not Bedroom Scene - keep waiting

        SceneManager.sceneLoaded -= OnSceneLoaded;
        pending = false;
        sequence.Begin();
    }

    private void Awake()
    {
        solid = new Texture2D(1, 1, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        solid.SetPixel(0, 0, Color.white);
        solid.Apply();
    }

    private void Start()
    {
        if (debugPlayOnStart) Begin();
    }

    private void OnDestroy()
    {
        if (solid != null) Destroy(solid);
    }

    /// <summary>Plays the beat once; safe to call again (later calls are ignored).</summary>
    public void Begin()
    {
        if (begun) return;
        begun = true;
        overlayAlpha = 1f;
        SwitchOffMorningRoutine();
        SetClock(wakeHour, wakeMinute);
        StartCoroutine(WakeRoutine());
    }

    /// <summary>On this clock a positive local Z turn moves a hand clockwise as seen from the room,
    /// and both hands point at 12 at zero.</summary>
    private void SetClock(int hour, int minute)
    {
        if (clockHourHand != null) clockHourHand.localRotation = Quaternion.Euler(0f, 0f, (hour % 12) * 30f + minute * 0.5f);
        if (clockMinuteHand != null) clockMinuteHand.localRotation = Quaternion.Euler(0f, 0f, minute * 6f);
    }

    /// <summary>This load isn't a fresh morning: no bed wake-up or 8 o'clock intro, no computer
    /// screen when he's sat in the chair, and nothing that could hand out the wash/pack/backpack
    /// objectives on the way to the door.</summary>
    private void SwitchOffMorningRoutine()
    {
        foreach (var b in FindObjectsByType<PlayerWakeUpSequence>(FindObjectsInactive.Include, FindObjectsSortMode.None)) b.enabled = false;
        foreach (var b in FindObjectsByType<GameIntroSequence>(FindObjectsInactive.Include, FindObjectsSortMode.None)) b.enabled = false;
        foreach (var b in FindObjectsByType<ComputerUseSequence>(FindObjectsInactive.Include, FindObjectsSortMode.None)) b.enabled = false;
        foreach (var b in FindObjectsByType<WashFaceStation>(FindObjectsInactive.Include, FindObjectsSortMode.None)) b.enabled = false;
        foreach (var b in FindObjectsByType<CarryableItem>(FindObjectsInactive.Include, FindObjectsSortMode.None)) b.enabled = false;
        foreach (var b in FindObjectsByType<BackpackStation>(FindObjectsInactive.Include, FindObjectsSortMode.None)) b.enabled = false;
    }

    private IEnumerator WakeRoutine()
    {
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo == null || chair == null) { overlayAlpha = 0f; yield break; }

        var look = playerGo.GetComponent<MouseLook>();
        var interactor = playerGo.GetComponent<PlayerInteractor>();
        var controller = playerGo.GetComponent<CharacterController>();
        var cam = playerGo.GetComponentInChildren<Camera>(true).transform;
        Transform body = playerGo.transform;

        // Standing values to put back once he's up - SittableChair shortens both while seated.
        float standingHeight = controller != null ? controller.height : 1.8f;
        Vector3 standingCenter = controller != null ? controller.center : new Vector3(0f, 0.9f, 0f);
        Vector3 standingCamLocal = cam.localPosition;

        if (look != null) look.enabled = false;
        if (interactor != null) interactor.enabled = false;

        chair.Sit(playerGo);
        Vector3 seatedCamLocal = cam.localPosition;

        // Head down on the desk, cheek on the surface.
        Vector3 headPoint = SleepingHeadPoint(body.position);
        Vector3 sleepLocal = body.InverseTransformPoint(headPoint);
        Quaternion sleepRot = Quaternion.Euler(sleepingCameraEuler);
        cam.localPosition = sleepLocal;
        cam.localRotation = sleepRot;

        yield return new WaitForSeconds(blackHold);

        // Heavy blinks while still lying there.
        yield return Fade(1f, 0.35f, 0.5f);
        yield return new WaitForSeconds(0.25f);
        yield return Fade(0.35f, 0.9f, 0.2f);
        yield return Fade(0.9f, 0.2f, 0.35f);
        yield return new WaitForSeconds(0.35f);
        yield return Fade(0.2f, 0f, 0.6f);
        yield return new WaitForSeconds(holdAfterBlinking);

        // Lift the head off the desk and sit up.
        Quaternion seatedRot = Quaternion.Euler(12f, 0f, 0f);
        for (float t = 0f; t < liftHeadDuration; t += Time.deltaTime)
        {
            float k = Mathf.SmoothStep(0f, 1f, t / liftHeadDuration);
            cam.localPosition = Vector3.Lerp(sleepLocal, seatedCamLocal, k);
            cam.localRotation = Quaternion.Slerp(sleepRot, seatedRot, k);
            yield return null;
        }
        cam.localPosition = seatedCamLocal;

        // Dazed glance around the room.
        for (float t = 0f; t < lookAroundDuration; t += Time.deltaTime)
        {
            float k = t / lookAroundDuration;
            float yaw = Mathf.Sin(k * Mathf.PI * 2f) * 28f;
            float pitch = Mathf.Lerp(12f, 2f, k);
            cam.localRotation = Quaternion.Euler(pitch, yaw, 0f);
            yield return null;
        }
        cam.localRotation = Quaternion.Euler(2f, 0f, 0f);

        var dialogue = DialogueHUD.Instance != null ? DialogueHUD.Instance : FindFirstObjectByType<DialogueHUD>();
        if (dialogue != null)
        {
            dialogue.Say(speaker, confusedLine, holdPerLine);
            yield return null;
            yield return new WaitWhile(() => dialogue.IsPlaying);

            // Glance up at the wall clock - and it's nine.
            if (clock != null)
            {
                Quaternion toClock = Quaternion.LookRotation(body.InverseTransformDirection(clock.position - cam.position));
                yield return TurnCamera(cam, toClock, 0.5f);
            }
            dialogue.Say(speaker, clockLine, holdPerLine);
            yield return null;
            yield return new WaitWhile(() => dialogue.IsPlaying);

            yield return TurnCamera(cam, Quaternion.identity, 0.35f);
            dialogue.Say(speaker, hurryLine, holdPerLine);
            yield return null;
            yield return new WaitWhile(() => dialogue.IsPlaying);
        }

        // Up out of the chair.
        cam.localRotation = Quaternion.identity;
        chair.StandUp();
        yield return new WaitForSeconds(0.5f);
        if (controller != null)
        {
            controller.height = standingHeight;
            controller.center = standingCenter;
        }
        cam.localPosition = standingCamLocal;
        cam.localRotation = Quaternion.identity;
        if (look != null) look.enabled = true;
        if (interactor != null) interactor.enabled = true;

        // Late for school: School Scene skips the friends and sends him straight to class.
        if (schoolExit != null) schoolExit.UnlockNow(objective, LateSchoolArrivalSequence.QueueForNextLoad);
        else if (MissionHUD.Instance != null) MissionHUD.Instance.SetObjective(objective);
    }

    private static IEnumerator TurnCamera(Transform cam, Quaternion target, float duration)
    {
        Quaternion from = cam.localRotation;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            cam.localRotation = Quaternion.Slerp(from, target, Mathf.SmoothStep(0f, 1f, t / duration));
            yield return null;
        }
        cam.localRotation = target;
    }

    private Vector3 SleepingHeadPoint(Vector3 seat)
    {
        if (desk == null) return seat + Vector3.up * 0.9f;
        Bounds b = desk.bounds;
        Vector3 edge = b.ClosestPoint(new Vector3(seat.x, b.center.y, seat.z));
        Vector3 toDesk = new Vector3(b.center.x - seat.x, 0f, b.center.z - seat.z).normalized;
        Vector3 p = edge + toDesk * headPastDeskEdge;
        p.y = b.max.y + headHeightAboveDesk;
        return p;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            overlayAlpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        overlayAlpha = to;
    }

    private void OnGUI()
    {
        if (overlayAlpha <= 0.001f) return;
        GUI.depth = -100;
        GUI.color = new Color(0f, 0f, 0f, overlayAlpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), solid);
        GUI.color = Color.white;
    }
}
