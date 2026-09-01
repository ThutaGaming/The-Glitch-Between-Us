using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Press E at the sink to wash your face: the player bows their head over the basin, the tap
/// runs, water beads up on the camera, then they straighten back up and the objective ticks off.
///
/// There is no visible player body in this game, so the "bow" is the camera pitching down and
/// dipping toward the basin — that reads as leaning over the sink from a first-person view.
/// </summary>
public class WashFaceStation : MonoBehaviour, IInteractable
{
    [Header("Anchors")]
    [Tooltip("The point in the basin the player looks down at. Also where the tap water lands.")]
    [SerializeField] private Transform basinPoint;
    [Tooltip("What the bowed camera actually aims at. Put it between the spout and the drain so " +
             "the running tap stays in frame while washing. Falls back to basinPoint if empty.")]
    [SerializeField] private Transform lookAtPoint;
    [Tooltip("Optional. Where the player is eased to before bowing. Leave empty to wash in place.")]
    [SerializeField] private Transform standPoint;
    [Tooltip("Optional. Overrides where the interact prompt measures from; defaults to basinPoint.")]
    [SerializeField] private Transform interactAnchor;

    [Header("Water")]
    [SerializeField] private FaucetWaterStream faucet;
    [SerializeField] private ScreenWaterDroplets screenDroplets;

    [Header("Prompt")]
    [SerializeField] private string prompt = "(E) Wash your face";
    [Tooltip("Shown once the objective is done. Leave empty to hide the prompt afterwards.")]
    [SerializeField] private string promptAfterUse = "";

    [Header("Timing (seconds)")]
    [SerializeField] private float moveToStandDuration = 0.5f;
    [SerializeField] private float bowDownDuration = 1.0f;
    [SerializeField] private float washDuration = 3.4f;
    [SerializeField] private float standUpDuration = 0.9f;
    [Tooltip("Tap keeps running this long after the last scrub, then shuts off.")]
    [SerializeField] private float tapOverrun = 0.4f;

    [Header("Bow Pose")]
    [Tooltip("Camera local height while bowed over the basin (standing is about 1.6).")]
    [SerializeField] private float bowedCameraHeight = 1.25f;
    [Tooltip("How far the head leans forward over the basin, in metres. This is what makes it " +
             "read as bowing over the sink - do NOT use extraBowPitch for that, since aiming " +
             "past the basin just points the camera at the cabinet below it.")]
    [SerializeField] private float bowedCameraForward = 0.42f;
    [Tooltip("Small nudge on top of aiming straight at the basin. Keep this near zero; " +
             "large values look past the basin entirely.")]
    [SerializeField] private float extraBowPitch = 4f;

    [Header("Scrubbing")]
    [Tooltip("How many times the head bobs while washing.")]
    [SerializeField] private float scrubCycles = 4f;
    [SerializeField] private float scrubPitchAmplitude = 7f;
    [SerializeField] private float scrubRollAmplitude = 3.5f;
    [SerializeField] private float scrubHeightAmplitude = 0.05f;

    [Header("Objective")]
    [SerializeField] private bool completesMissionOnFinish = true;
    [SerializeField] private string finishedLineSpeaker = "Thuta";
    [SerializeField] private string finishedLine = "That's better. Now I can get to school.";

    public UnityEvent onWashComplete;

    private bool isWashing;
    private bool hasWashed;

    public bool HasWashed => hasWashed;

    public Transform InteractTransform =>
        interactAnchor != null ? interactAnchor : (basinPoint != null ? basinPoint : transform);

    private void OnEnable() => InteractableRegistry.All.Add(this);
    private void OnDisable() => InteractableRegistry.All.Remove(this);

    public string GetPrompt()
    {
        if (isWashing) return "";
        return hasWashed ? promptAfterUse : prompt;
    }

    public void Interact(GameObject player)
    {
        if (isWashing || hasWashed || player == null) return;
        StartCoroutine(WashRoutine(player));
    }

    private IEnumerator WashRoutine(GameObject player)
    {
        isWashing = true;

        Transform body = player.transform;
        var camera = player.GetComponentInChildren<Camera>(true);
        Transform cam = camera != null ? camera.transform : null;

        var mouseLook = player.GetComponentInChildren<MouseLook>(true);
        var movement = player.GetComponent<PlayerMovement>();
        var interactor = player.GetComponent<PlayerInteractor>();
        var controller = player.GetComponent<CharacterController>();

        // PlayerMovement rewrites the camera height every frame and MouseLook rewrites its
        // rotation, so both have to be off or they fight this animation.
        bool mouseLookWas = mouseLook != null && mouseLook.enabled;
        bool movementWas = movement != null && movement.enabled;
        bool interactorWas = interactor != null && interactor.enabled;
        if (mouseLook != null) mouseLook.enabled = false;
        if (movement != null) movement.enabled = false;
        if (interactor != null) interactor.enabled = false;

        Vector3 startBodyPos = body.position;
        Quaternion startBodyRot = body.rotation;
        Vector3 startCamLocalPos = cam != null ? cam.localPosition : new Vector3(0f, 1.6f, 0f);
        float startCamHeight = startCamLocalPos.y;
        Quaternion startCamRot = cam != null ? cam.localRotation : Quaternion.identity;

        // Step 1 - ease into place in front of the sink and turn to face the basin.
        if (standPoint != null && moveToStandDuration > 0f)
        {
            bool controllerWas = controller != null && controller.enabled;
            if (controller != null) controller.enabled = false;

            Vector3 target = new Vector3(standPoint.position.x, body.position.y, standPoint.position.z);
            float t = 0f;
            while (t < moveToStandDuration)
            {
                t += Time.deltaTime;
                float p = Smooth01(t / moveToStandDuration);
                body.position = Vector3.Lerp(startBodyPos, target, p);
                body.rotation = Quaternion.Slerp(startBodyRot, FacingBasinYaw(body), p);
                yield return null;
            }
            body.position = target;

            if (controller != null) controller.enabled = controllerWas;
        }

        // Step 2 - bow the head down over the basin.
        float t2 = 0f;
        while (t2 < bowDownDuration)
        {
            t2 += Time.deltaTime;
            float p = Smooth01(t2 / bowDownDuration);

            body.rotation = Quaternion.Slerp(body.rotation, FacingBasinYaw(body), p * 0.5f);

            if (cam != null)
            {
                // Lower AND lean forward - the lean is what puts the head over the basin.
                Vector3 lp = cam.localPosition;
                lp.y = Mathf.Lerp(startCamHeight, bowedCameraHeight, p);
                lp.z = Mathf.Lerp(startCamLocalPos.z, startCamLocalPos.z + bowedCameraForward, p);
                cam.localPosition = lp;
                // Recomputed each frame so the aim stays true as the head moves.
                cam.localRotation = Quaternion.Slerp(startCamRot, BowedCameraRotation(cam, body), p);
            }
            yield return null;
        }

        // Step 3 - water on, scrub, droplets build up on the lens.
        if (faucet != null) faucet.SetRunning(true);
        if (screenDroplets != null) screenDroplets.Splash(10);

        float t3 = 0f;
        int lastScrub = -1;
        while (t3 < washDuration)
        {
            t3 += Time.deltaTime;
            float p = Mathf.Clamp01(t3 / washDuration);
            float phase = p * scrubCycles * Mathf.PI * 2f;

            if (screenDroplets != null)
            {
                // Ramp up fast, then hold; the fade-out happens on the way back up.
                screenDroplets.SetWetness(Smooth01(t3 / 0.6f));
            }

            // A burst of droplets at the bottom of each scrub, when hands hit the face.
            int scrubIndex = Mathf.FloorToInt(p * scrubCycles);
            if (scrubIndex != lastScrub)
            {
                lastScrub = scrubIndex;
                if (screenDroplets != null) screenDroplets.Splash(Random.Range(4, 8));
            }

            if (cam != null)
            {
                Quaternion aim = BowedCameraRotation(cam, body);
                Vector3 e = aim.eulerAngles;
                cam.localRotation = Quaternion.Euler(
                    e.x + Mathf.Sin(phase) * scrubPitchAmplitude,
                    e.y,
                    Mathf.Sin(phase * 0.5f) * scrubRollAmplitude);

                Vector3 lp = cam.localPosition;
                lp.y = bowedCameraHeight + Mathf.Sin(phase) * scrubHeightAmplitude;
                lp.z = startCamLocalPos.z + bowedCameraForward;
                cam.localPosition = lp;
            }
            yield return null;
        }

        if (tapOverrun > 0f) yield return new WaitForSeconds(tapOverrun);
        if (faucet != null) faucet.SetRunning(false);

        // Step 4 - straighten back up; the lens dries off on the way.
        Vector3 bowPos = cam != null ? cam.localPosition : Vector3.zero;
        Quaternion bowRot = cam != null ? cam.localRotation : Quaternion.identity;
        float t4 = 0f;
        while (t4 < standUpDuration)
        {
            t4 += Time.deltaTime;
            float p = Smooth01(t4 / standUpDuration);

            if (cam != null)
            {
                Vector3 lp = cam.localPosition;
                lp.y = Mathf.Lerp(bowPos.y, startCamHeight, p);
                lp.z = Mathf.Lerp(bowPos.z, startCamLocalPos.z, p);
                cam.localPosition = lp;
                cam.localRotation = Quaternion.Slerp(bowRot, Quaternion.identity, p);
            }

            if (screenDroplets != null) screenDroplets.SetWetness(1f - p);
            yield return null;
        }

        if (cam != null)
        {
            // Back to exactly where the camera started, so MouseLook and PlayerMovement
            // pick up a clean pose when they re-enable.
            cam.localPosition = startCamLocalPos;
            cam.localRotation = Quaternion.identity;
        }
        if (screenDroplets != null)
        {
            screenDroplets.SetWetness(0f);
            screenDroplets.ClearDroplets();
        }

        hasWashed = true;
        isWashing = false;

        if (mouseLook != null) mouseLook.enabled = mouseLookWas;
        if (movement != null) movement.enabled = movementWas;
        if (interactor != null) interactor.enabled = interactorWas;

        if (completesMissionOnFinish && MissionHUD.Instance != null)
            MissionHUD.Instance.CompleteObjective();

        if (!string.IsNullOrEmpty(finishedLine) && DialogueHUD.Instance != null)
            DialogueHUD.Instance.Say(finishedLineSpeaker, finishedLine);

        onWashComplete?.Invoke();
    }

    /// <summary>Body yaw that faces the basin, keeping the player upright.</summary>
    private Quaternion FacingBasinYaw(Transform body)
    {
        if (basinPoint == null) return body.rotation;
        Vector3 flat = basinPoint.position - body.position;
        flat.y = 0f;
        return flat.sqrMagnitude < 0.0001f ? body.rotation : Quaternion.LookRotation(flat, Vector3.up);
    }

    /// <summary>Camera-local rotation that aims down at the sink from where the camera is now.</summary>
    private Quaternion BowedCameraRotation(Transform cam, Transform body)
    {
        Transform target = lookAtPoint != null ? lookAtPoint : basinPoint;
        if (target == null) return Quaternion.Euler(35f, 0f, 0f);

        Vector3 toTarget = target.position - cam.position;
        float horizontal = new Vector2(toTarget.x, toTarget.z).magnitude;
        float pitch = Mathf.Atan2(-toTarget.y, Mathf.Max(0.01f, horizontal)) * Mathf.Rad2Deg;
        return Quaternion.Euler(Mathf.Clamp(pitch + extraBowPitch, -85f, 85f), 0f, 0f);
    }

    /// <summary>
    /// GLSL-style smoothstep on an already-normalised t (Mathf.SmoothStep is a smoothed lerp
    /// between two values, which is not what is wanted here).
    /// </summary>
    private static float Smooth01(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3f - 2f * t);
    }

    private void OnDrawGizmosSelected()
    {
        if (basinPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(basinPoint.position, 0.07f);
        }
        if (standPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(standPoint.position, 0.2f);
            if (basinPoint != null) Gizmos.DrawLine(standPoint.position, basinPoint.position);
        }
    }
}
