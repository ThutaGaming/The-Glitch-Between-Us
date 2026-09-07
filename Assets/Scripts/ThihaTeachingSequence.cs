using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Plays Mr. Thiha's classroom entrance once the player sits in `triggerChair`: he appears at
/// `entryPoint` (the barn door), walks to `teachingSpot` (in front of the media console) using
/// the imported ThihaWalking clip, then holds a procedural "teaching" gesture loop — there is
/// no mocap clip for that part, so it's hand-built from bone rotations layered on top of the
/// frozen walk pose in LateUpdate (the standard way to add procedural motion on top of Mecanim).
///
/// While he walks in, the player's camera is driven to watch him. That is split into body-yaw
/// plus camera-pitch exactly the way <see cref="MouseLook"/> does it, so when control is handed
/// back the pitch MouseLook re-seeds in OnEnable already matches and the view doesn't snap.
/// </summary>
public class ThihaTeachingSequence : MonoBehaviour
{
    [Header("Trigger")]
    [SerializeField] private SittableChair triggerChair;
    [Tooltip("Only plays once per scene load.")]
    [SerializeField] private bool triggerOnce = true;

    /// <summary>Fires once Thiha has finished walking out and been deactivated - hook a follow-up quest to it.</summary>
    public UnityEvent onLeft;

    [Header("Thiha")]
    [SerializeField] private Animator thihaAnimator;
    [SerializeField] private Transform entryPoint;
    [SerializeField] private Transform teachingSpot;
    [SerializeField] private float walkSpeed = 1.6f;
    [SerializeField] private float turnDegreesPerSecond = 240f;
    [Tooltip("Which point in the (looping) walk clip to freeze on once Thiha arrives, since " +
             "there is no separate idle clip — 0..1. Nudge until it looks like a normal stance.")]
    [Range(0f, 1f)]
    [SerializeField] private float standingPoseNormalizedTime = 0f;

    [Header("Player camera")]
    [Tooltip("Turn the seated player's view to follow Thiha as he walks in.")]
    [SerializeField] private bool watchThihaEnter = true;
    [SerializeField] private MouseLook playerLook;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float cameraTurnDegreesPerSecond = 120f;
    [Tooltip("Seconds to keep watching him after he has arrived, before control returns.")]
    [SerializeField] private float holdOnArrivalSeconds = 1.5f;

    [Header("Tired monologue — plays once teaching starts, same black-overlay caption style as " +
             "SchoolExitSequence's time-skip beat. Player can't stand up until it finishes.")]
    [SerializeField] private float monologueStartDelay = 1.5f;
    [SerializeField]
    private string[] monologueLines =
    {
        "School is from 9 to 4.",
        "I'm getting pretty tired.",
        "School will be over in a bit."
    };
    [Tooltip("Characters revealed per second.")]
    [SerializeField] private float monologueTypeSpeed = 22f;
    [Tooltip("Extra seconds a fully-typed line stays up before the next one starts.")]
    [SerializeField] private float monologueCaptionHold = 2.0f;
    [SerializeField] private float monologueFadeDuration = 0.5f;
    [SerializeField] private int monologueFontSize = 28;
    [Tooltip("1 = fully black. Lower to keep the classroom faintly visible behind the caption.")]
    [Range(0f, 1f)]
    [SerializeField] private float monologueOverlayAlpha = 1f;

    [Header("Teaching gesture — write on the board, then lower the arm and look at the class, repeat")]
    [Tooltip("How far the right arm lifts toward board height at the peak of each cycle. " +
             "Sign/axis depends on the rig; nudge if it lifts the wrong way.")]
    [SerializeField] private float armRaiseDegrees = 85f;
    [Tooltip("Elbow bend accompanying the raise, like reaching up to write.")]
    [SerializeField] private float elbowBendDegrees = 70f;
    [Tooltip("Small fast side-to-side wrist motion simulating pen strokes, active only while the arm is up.")]
    [SerializeField] private float writingStrokeDegrees = 6f;
    [SerializeField] private float writingStrokesPerSecond = 2.2f;
    [Tooltip("The idle (non-writing) arm resting near the body — small sway only.")]
    [SerializeField] private float restingArmSwayDegrees = 3f;
    [Tooltip("Full cycle time: arm raised & writing -> arm lowers while he turns to look at the " +
             "class -> arm raises again to resume writing.")]
    [SerializeField] private float writeLookCycleSeconds = 5f;
    [Tooltip("How far he turns toward the class while the arm is down.")]
    [SerializeField] private float headTurnDegrees = 30f;
    [SerializeField] private float spineTwistDegrees = 14f;

    private bool hasTriggered;
    private bool isTeaching;
    private float gestureClock;

    private Transform rightUpperArm;
    private Transform rightLowerArm;
    private Transform rightHand;
    private Transform leftUpperArm;
    private Transform leftLowerArm;
    private Transform head;
    private Transform spine;

    private float overlayAlpha;
    private string captionText = "";
    private int revealedChars;
    private Texture2D solid;
    private GUIStyle captionStyle;

    private void Awake() => solid = BuildSolidTexture();

    private void OnEnable()
    {
        if (triggerChair != null) triggerChair.onSat += HandleSat;
    }

    private void OnDisable()
    {
        if (triggerChair != null) triggerChair.onSat -= HandleSat;
    }

    private void OnDestroy()
    {
        if (solid != null) Destroy(solid);
    }

    private void HandleSat()
    {
        if (triggerOnce && hasTriggered) return;
        hasTriggered = true;
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        isTeaching = false;
        // Keep the player seated for the whole entrance — standing up while Thiha is mid-walk
        // would let them wander off/clip through him. Released once he starts teaching.
        if (triggerChair != null) triggerChair.SetStandUpBlocked(true);

        thihaAnimator.gameObject.SetActive(true);
        // The walk clip carries root motion; this script drives the position itself, so leaving
        // it on makes him drift past the mark.
        thihaAnimator.applyRootMotion = false;
        thihaAnimator.transform.SetPositionAndRotation(entryPoint.position, entryPoint.rotation);
        thihaAnimator.Play("Walk", 0, 0f);
        thihaAnimator.speed = 1f;

        bool cameraTaken = watchThihaEnter && playerCamera != null && playerBody != null;
        if (cameraTaken && playerLook != null) playerLook.enabled = false;

        yield return WalkTo(teachingSpot, cameraTaken);

        thihaAnimator.Play("Walk", 0, standingPoseNormalizedTime);
        thihaAnimator.Update(0f);
        thihaAnimator.speed = 0f;

        rightUpperArm = thihaAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        rightLowerArm = thihaAnimator.GetBoneTransform(HumanBodyBones.RightLowerArm);
        rightHand = thihaAnimator.GetBoneTransform(HumanBodyBones.RightHand);
        leftUpperArm = thihaAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        leftLowerArm = thihaAnimator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        head = thihaAnimator.GetBoneTransform(HumanBodyBones.Head);
        spine = thihaAnimator.GetBoneTransform(HumanBodyBones.Spine);

        gestureClock = 0f;
        isTeaching = true;

        // He's teaching now, but the player stays seated a moment longer for a tired inner
        // monologue (same black-overlay caption style as SchoolExitSequence's time-skip beat).
        yield return PlayMonologue();

        // Monologue's over - standing up is safe again, whether or not the player watches him leave.
        if (triggerChair != null) triggerChair.SetStandUpBlocked(false);

        float hold = 0f;
        while (cameraTaken && hold < holdOnArrivalSeconds)
        {
            hold += Time.deltaTime;
            AimPlayerViewAt(WatchPoint());
            yield return null;
        }

        // Lesson's over - he walks back out through the same door and leaves the classroom.
        isTeaching = false;
        thihaAnimator.speed = 1f;
        yield return WalkTo(entryPoint, cameraTaken);

        thihaAnimator.speed = 0f;
        thihaAnimator.gameObject.SetActive(false);

        if (cameraTaken && playerLook != null) playerLook.enabled = true;

        onLeft?.Invoke();
    }

    /// <summary>
    /// Walks Thiha's transform toward `target`'s position, then turns to match its rotation.
    /// Shared by the entrance (to teachingSpot) and the exit (back to entryPoint).
    /// </summary>
    private IEnumerator WalkTo(Transform target, bool cameraTaken)
    {
        Transform t = thihaAnimator.transform;
        while (Vector3.Distance(t.position, target.position) > 0.05f)
        {
            Vector3 toTarget = target.position - t.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                Quaternion faceMove = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
                t.rotation = Quaternion.RotateTowards(t.rotation, faceMove, turnDegreesPerSecond * Time.deltaTime);
            }

            t.position = Vector3.MoveTowards(t.position, target.position, walkSpeed * Time.deltaTime);
            if (cameraTaken) AimPlayerViewAt(WatchPoint());
            yield return null;
        }

        while (Quaternion.Angle(t.rotation, target.rotation) > 0.5f)
        {
            t.rotation = Quaternion.RotateTowards(t.rotation, target.rotation, turnDegreesPerSecond * Time.deltaTime);
            if (cameraTaken) AimPlayerViewAt(WatchPoint());
            yield return null;
        }
        t.rotation = target.rotation;
    }

    /// <summary>Roughly Thiha's head, so the player looks at his face rather than his feet.</summary>
    private Vector3 WatchPoint()
    {
        if (head != null) return head.position;
        var bone = thihaAnimator.GetBoneTransform(HumanBodyBones.Head);
        if (bone != null) return bone.position;
        return thihaAnimator.transform.position + Vector3.up * 1.6f;
    }

    /// <summary>
    /// Yaw goes on the body and pitch on the camera's localRotation — the same split MouseLook
    /// uses, so handing control back mid-look doesn't jump.
    /// </summary>
    private void AimPlayerViewAt(Vector3 worldPoint)
    {
        Vector3 toTarget = worldPoint - playerCamera.position;
        Vector3 flat = new Vector3(toTarget.x, 0f, toTarget.z);
        if (flat.sqrMagnitude < 0.0001f) return;

        float maxStep = cameraTurnDegreesPerSecond * Time.deltaTime;

        Quaternion targetYaw = Quaternion.LookRotation(flat.normalized, Vector3.up);
        playerBody.rotation = Quaternion.RotateTowards(playerBody.rotation, targetYaw, maxStep);

        float targetPitch = -Mathf.Atan2(toTarget.y, flat.magnitude) * Mathf.Rad2Deg;
        float currentPitch = playerCamera.localEulerAngles.x;
        if (currentPitch > 180f) currentPitch -= 360f;
        float newPitch = Mathf.MoveTowards(currentPitch, targetPitch, maxStep);
        playerCamera.localRotation = Quaternion.Euler(newPitch, 0f, 0f);
    }

    private void LateUpdate()
    {
        if (!isTeaching) return;

        gestureClock += Time.deltaTime;

        // One shared cycle drives both the arm and the head/spine, inversely: at the peak
        // (writeAmount=1) the arm is fully raised and he faces the board; at the trough
        // (writeAmount=0) the arm has lowered and he's turned to look at the class instead —
        // "write, lower the arm, look forward, raise the arm again, repeat."
        float cycle = Mathf.PI * 2f / Mathf.Max(0.01f, writeLookCycleSeconds);
        float writeAmount = (Mathf.Sin(gestureClock * cycle - Mathf.PI / 2f) + 1f) * 0.5f; // 0..1, starts at 0 (arm down)

        float armRaise = armRaiseDegrees * writeAmount;
        float elbowBend = elbowBendDegrees * writeAmount;
        float headTurn = headTurnDegrees * (1f - writeAmount);
        float spineTwist = spineTwistDegrees * (1f - writeAmount);

        // Pen strokes only while the arm is actually up near the board.
        float strokePhase = gestureClock * writingStrokesPerSecond * Mathf.PI * 2f;
        float stroke = Mathf.Sin(strokePhase) * writingStrokeDegrees * writeAmount;

        // The idle hand sways gently near the body instead of gesturing.
        float restSway = Mathf.Sin(gestureClock * 0.6f) * restingArmSwayDegrees;

        if (rightUpperArm != null) rightUpperArm.localRotation *= Quaternion.Euler(0f, 0f, armRaise);
        if (rightLowerArm != null) rightLowerArm.localRotation *= Quaternion.Euler(elbowBend, 0f, 0f);
        if (rightHand != null) rightHand.localRotation *= Quaternion.Euler(0f, stroke, 0f);
        if (leftUpperArm != null) leftUpperArm.localRotation *= Quaternion.Euler(0f, 0f, -restSway);
        if (head != null) head.localRotation *= Quaternion.Euler(0f, headTurn, 0f);
        if (spine != null) spine.localRotation *= Quaternion.Euler(0f, spineTwist, 0f);
    }

    private IEnumerator PlayMonologue()
    {
        if (monologueLines == null || monologueLines.Length == 0) yield break;

        if (monologueStartDelay > 0f) yield return new WaitForSeconds(monologueStartDelay);

        yield return FadeOverlay(0f, monologueOverlayAlpha, monologueFadeDuration);

        foreach (string line in monologueLines)
        {
            if (!string.IsNullOrEmpty(line)) yield return ShowCaption(line);
        }

        yield return FadeOverlay(monologueOverlayAlpha, 0f, monologueFadeDuration);
        captionText = "";
    }

    private IEnumerator ShowCaption(string text)
    {
        captionText = text;
        revealedChars = 0;

        float typed = 0f;
        while (revealedChars < text.Length)
        {
            typed += monologueTypeSpeed * Time.deltaTime;
            revealedChars = Mathf.Min(text.Length, Mathf.FloorToInt(typed));
            yield return null;
        }

        yield return new WaitForSeconds(monologueCaptionHold);
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

        if (captionStyle == null)
        {
            captionStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = monologueFontSize,
                alignment = TextAnchor.MiddleCenter
            };
            captionStyle.normal.textColor = Color.white;
        }

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

    private static Texture2D BuildSolidTexture()
    {
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return tex;
    }
}
