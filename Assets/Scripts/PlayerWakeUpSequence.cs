using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Plays a "wake up from bed" intro on game start: lying down -> sit up -> turn right -> stand up.
/// Drives camera height/pitch and CharacterController height/center directly since this project
/// has no visible player body mesh (FPS, arms/legs intentionally absent).
/// </summary>
public class PlayerWakeUpSequence : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform bed;
    [SerializeField] private CharacterController controller;

    [Header("Timing (seconds)")]
    [SerializeField] private float lieHoldDuration = 0.6f;
    [SerializeField] private float sitUpDuration = 1.2f;
    [SerializeField] private float holdAfterSitUp = 0.4f;
    [SerializeField] private float turnRightDuration = 0.8f;
    [SerializeField] private float holdAfterTurn = 0.3f;
    [SerializeField] private float standUpDuration = 1.0f;

    [Header("Camera Local Height (Y)")]
    [SerializeField] private float lyingCameraHeight = 0.3f;
    [SerializeField] private float sittingCameraHeight = 1.0f;
    [SerializeField] private float standingCameraHeight = 1.6f;

    [Header("Camera Pitch (local X, degrees; negative looks up)")]
    [SerializeField] private float lyingPitch = -75f;
    [SerializeField] private float sittingPitch = 0f;
    [SerializeField] private float standingPitch = 0f;
    [Tooltip("Head turned sideways while lying (degrees, positive = right), so the first thing seen is the lit room rather than a dark ceiling.")]
    [SerializeField] private float lyingYaw = 0f;

    [Header("Turn")]
    [SerializeField] private float turnRightYaw = 90f;

    [Header("CharacterController Pose")]
    [SerializeField] private float lyingCCHeight = 0.6f;
    [SerializeField] private Vector3 lyingCCCenter = new Vector3(0f, 0.3f, 0f);
    [SerializeField] private float sittingCCHeight = 1.2f;
    [SerializeField] private Vector3 sittingCCCenter = new Vector3(0f, 0.6f, 0f);
    [SerializeField] private float standingCCHeight = 1.8f;
    [SerializeField] private Vector3 standingCCCenter = new Vector3(0f, 0.9f, 0f);

    [Header("Step Off Bed")]
    [Tooltip("Extra clearance beyond the bed's edge + controller radius when stepping aside while standing up.")]
    [SerializeField] private float stepOffMargin = 0.3f;

    [Header("Ground Snap")]
    [Tooltip("Layers considered floor when probing for the ground height at the step-off destination.")]
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float groundProbeUpOffset = 2f;
    [SerializeField] private float groundProbeMaxDistance = 10f;

    [System.Serializable]
    public struct EyeStep
    {
        [Tooltip("Where the eyelids end up: 0 shut, 1 fully open.")]
        [Range(0f, 1f)] public float openness;
        public float duration;
        public float hold;

        public EyeStep(float openness, float duration, float hold)
        {
            this.openness = openness;
            this.duration = duration;
            this.hold = hold;
        }
    }

    [Header("Eyes Opening (while still lying down)")]
    [SerializeField] private bool playEyesOpening = true;
    [Tooltip("Black screen before the eyes first crack open.")]
    [SerializeField] private float eyesShutHold = 1.0f;
    [Tooltip("Heavy, sleepy blinks: each step moves the eyelids to 'openness' over 'duration', then holds.")]
    [SerializeField] private EyeStep[] eyeSteps =
    {
        new EyeStep(0.30f, 1.00f, 0.40f),   // eyes crack open
        new EyeStep(0.00f, 0.30f, 0.40f),   // too heavy - shut again
        new EyeStep(0.55f, 0.80f, 0.30f),   // a little wider
        new EyeStep(0.05f, 0.15f, 0.05f),   // blink
        new EyeStep(1.00f, 0.90f, 0.20f),   // awake
    };
    [Tooltip("Slow, drowsy head drift while the eyes open, in degrees.")]
    [SerializeField] private float drowsySway = 1.5f;

    [Header("Control Lockout")]
    [Tooltip("Movement/look scripts to disable while the wake-up sequence plays, re-enabled afterward.")]
    [SerializeField] private Behaviour[] disableDuringSequence;

    public UnityEvent onWakeUpComplete;

    private void Reset()
    {
        playerBody = transform;
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>(true) != null
            ? GetComponentInChildren<Camera>(true).transform
            : null;
    }

    private void Start()
    {
        StartCoroutine(WakeUpRoutine());
    }

    private IEnumerator WakeUpRoutine()
    {
        SetControlEnabled(false);
        bool controllerWasEnabled = controller != null && controller.enabled;
        if (controller != null) controller.enabled = false;

        PoseLyingDown();
        if (playEyesOpening) yield return OpenEyes();
        yield return new WaitForSeconds(lieHoldDuration);

        yield return LerpPose(lyingCameraHeight, sittingCameraHeight, lyingPitch, sittingPitch,
            lyingCCHeight, lyingCCCenter, sittingCCHeight, sittingCCCenter, sitUpDuration);
        yield return new WaitForSeconds(holdAfterSitUp);

        yield return TurnRight(turnRightDuration);
        yield return new WaitForSeconds(holdAfterTurn);

        yield return StandUpAndStepAside(standUpDuration);

        if (controller != null) controller.enabled = controllerWasEnabled;
        SetControlEnabled(true);
        onWakeUpComplete?.Invoke();
    }

    private void PoseLyingDown()
    {
        if (bed != null && playerBody != null)
        {
            Vector3 pos = playerBody.position;
            pos.x = bed.position.x;
            pos.z = bed.position.z;
            playerBody.position = pos;
            playerBody.rotation = bed.rotation;
        }

        if (controller != null)
        {
            controller.height = lyingCCHeight;
            controller.center = lyingCCCenter;
        }

        if (playerCamera != null)
        {
            Vector3 lp = playerCamera.localPosition;
            lp.y = lyingCameraHeight;
            playerCamera.localPosition = lp;
            playerCamera.localRotation = Quaternion.Euler(lyingPitch, lyingYaw, 0f);
        }
    }

    /// <summary>Starts with the eyes shut and blinks them open through <see cref="eyeSteps"/>,
    /// while the head drifts a little, before he sits up.</summary>
    private IEnumerator OpenEyes()
    {
        var eyes = gameObject.AddComponent<EyelidOverlay>();
        eyes.Openness = 0f;

        float total = eyesShutHold;
        foreach (var step in eyeSteps) total += step.duration + step.hold;
        StartCoroutine(DrowsySway(total));

        yield return new WaitForSeconds(eyesShutHold);

        float from = 0f;
        foreach (var step in eyeSteps)
        {
            for (float t = 0f; t < step.duration; t += Time.deltaTime)
            {
                float k = Mathf.Clamp01(t / step.duration);
                eyes.Openness = Mathf.Lerp(from, step.openness, k * k * (3f - 2f * k));
                yield return null;
            }
            eyes.Openness = step.openness;
            from = step.openness;
            yield return new WaitForSeconds(step.hold);
        }

        Destroy(eyes);
    }

    /// <summary>Fades out by the end, so the sit-up starts from the plain lying pitch.</summary>
    private IEnumerator DrowsySway(float duration)
    {
        if (playerCamera == null) yield break;

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float amount = drowsySway * Mathf.Clamp01((duration - t) / 0.8f);
            playerCamera.localRotation = Quaternion.Euler(
                lyingPitch + Mathf.Sin(t * 0.8f) * amount,
                lyingYaw + Mathf.Sin(t * 0.5f) * amount * 1.4f,
                Mathf.Sin(t * 0.37f) * amount * 0.5f);
            yield return null;
        }
        playerCamera.localRotation = Quaternion.Euler(lyingPitch, lyingYaw, 0f);
    }

    private IEnumerator LerpPose(float fromCamY, float toCamY, float fromPitch, float toPitch,
        float fromCCHeight, Vector3 fromCCCenter, float toCCHeight, Vector3 toCCCenter, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));

            if (playerCamera != null)
            {
                Vector3 lp = playerCamera.localPosition;
                lp.y = Mathf.Lerp(fromCamY, toCamY, p);
                playerCamera.localPosition = lp;
                playerCamera.localRotation = Quaternion.Euler(Mathf.Lerp(fromPitch, toPitch, p), Mathf.Lerp(lyingYaw, 0f, p), 0f);
            }

            if (controller != null)
            {
                controller.height = Mathf.Lerp(fromCCHeight, toCCHeight, p);
                controller.center = Vector3.Lerp(fromCCCenter, toCCCenter, p);
            }

            yield return null;
        }
    }

    private IEnumerator StandUpAndStepAside(float duration)
    {
        Vector3 fromPos = playerBody.position;
        Vector3 stepDir = new Vector3(playerBody.forward.x, 0f, playerBody.forward.z).normalized;
        float radius = controller != null ? controller.radius : 0.3f;
        float distance = GetBedExtentAlong(stepDir) + radius + stepOffMargin;
        Vector3 toPos = fromPos + stepDir * distance;
        toPos.y = FindGroundY(toPos, fromPos.y);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));

            if (playerCamera != null)
            {
                Vector3 lp = playerCamera.localPosition;
                lp.y = Mathf.Lerp(sittingCameraHeight, standingCameraHeight, p);
                playerCamera.localPosition = lp;
                playerCamera.localRotation = Quaternion.Euler(Mathf.Lerp(sittingPitch, standingPitch, p), 0f, 0f);
            }

            if (controller != null)
            {
                controller.height = Mathf.Lerp(sittingCCHeight, standingCCHeight, p);
                controller.center = Vector3.Lerp(sittingCCCenter, standingCCCenter, p);
            }

            playerBody.position = Vector3.Lerp(fromPos, toPos, p);

            yield return null;
        }
    }

    private float FindGroundY(Vector3 worldXZ, float fallbackY)
    {
        Vector3 origin = new Vector3(worldXZ.x, fallbackY + groundProbeUpOffset, worldXZ.z);
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit,
            groundProbeUpOffset + groundProbeMaxDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point.y;
        }

        Debug.LogWarning("PlayerWakeUpSequence: no ground found below step-off position, keeping bed-level height.", this);
        return fallbackY;
    }

    private float GetBedExtentAlong(Vector3 worldDirection)
    {
        if (bed == null) return 0.6f;
        var bedRenderer = bed.GetComponentInChildren<Renderer>();
        if (bedRenderer == null) return 0.6f;

        Bounds b = bedRenderer.bounds;
        return Mathf.Abs(worldDirection.x) >= Mathf.Abs(worldDirection.z) ? b.extents.x : b.extents.z;
    }

    private IEnumerator TurnRight(float duration)
    {
        if (playerBody == null) yield break;

        Quaternion from = playerBody.rotation;
        Quaternion to = from * Quaternion.Euler(0f, turnRightYaw, 0f);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));
            playerBody.rotation = Quaternion.Slerp(from, to, p);
            yield return null;
        }

        playerBody.rotation = to;
    }

    private void SetControlEnabled(bool enabled)
    {
        if (disableDuringSequence == null) return;
        foreach (var b in disableDuringSequence)
        {
            if (b != null) b.enabled = enabled;
        }
    }
}
