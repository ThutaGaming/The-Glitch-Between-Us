using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using InfimaGames.LowPolyShooterPack;

/// <summary>
/// Replays the door-control camera shot from CutsceneIntro when triggered (wired to
/// IntercomLightSwitch.Interact), this time turning one of the two red indicator lights green -
/// visible progress towards the door the intro hologram described. Reuses the same
/// camera-reparent-and-restore technique as CutsceneIntro instead of a second camera, and looks
/// the player's camera up fresh each time rather than caching it in Awake, since by the time this
/// plays the camera has long since been restored under the player by CutsceneIntro.EndCutscene.
/// </summary>
public class IntercomCutscene : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Player root that carries Movement/CameraLook/PlayerInput. Auto-found by tag if left empty.")]
    [SerializeField] private Character player;

    [Header("Shot")]
    [Tooltip("Same framing CutsceneIntro used for the intro briefing.")]
    [SerializeField] private Vector3 shotPosition;
    [SerializeField] private Vector3 shotLookAt;

    [Header("Light")]
    [Tooltip("The indicator light this beat turns on. Note: object names in this asset don't " +
             "match screen side - verify with the actual camera shot, not the light's name.")]
    [SerializeField] private Light targetLight;
    [SerializeField] private Color onColor = new Color(0.15f, 1f, 0.25f);
    [SerializeField] private float lightFadeDuration = 1f;

    [Header("Timing")]
    [SerializeField] private float delayBeforeLight = 0.5f;
    [SerializeField] private float holdAfterLight = 2f;

    [Tooltip("Fires the instant the light finishes turning green - while the shot is still holding on it, before the camera cuts back.")]
    public UnityEvent onLightOn;

    private Movement movement;
    private CameraLook look;
    private UnityEngine.InputSystem.PlayerInput input;
    private bool playing;

    private void Awake()
    {
        if (player == null)
        {
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo != null) player = playerGo.GetComponent<Character>();
        }
        if (player != null)
        {
            movement = player.GetComponent<Movement>();
            look = player.GetComponentInChildren<CameraLook>();
            input = player.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        }
    }

    /// <summary>Wired to IntercomLightSwitch.Interact; the switch's own "used" guard makes this safe to call once.</summary>
    public void Play()
    {
        if (playing) return;
        playing = true;
        StartCoroutine(Run());
    }

    /// <summary>
    /// Test-only shortcut for the F8 debug skip: applies this beat's end state (light on,
    /// onLightOn fired) instantly, without reparenting the camera or playing the wait/fade timeline.
    /// </summary>
    public void DebugSkip()
    {
        if (playing) return;
        playing = true;

        if (targetLight != null) targetLight.color = onColor;
        onLightOn?.Invoke();
    }

    private IEnumerator Run()
    {
        if (movement != null) movement.enabled = false;
        if (look != null) look.enabled = false;
        if (input != null) input.enabled = false;

        Transform cam = player != null ? player.GetComponentInChildren<Camera>(true)?.transform : null;
        Transform camOriginalParent = null;
        Vector3 camOriginalLocalPos = Vector3.zero;
        Quaternion camOriginalLocalRot = Quaternion.identity;

        if (cam != null)
        {
            camOriginalParent = cam.parent;
            camOriginalLocalPos = cam.localPosition;
            camOriginalLocalRot = cam.localRotation;

            cam.SetParent(null, true);
            cam.position = shotPosition;
            cam.rotation = Quaternion.LookRotation(shotLookAt - shotPosition, Vector3.up);
        }

        if (delayBeforeLight > 0f) yield return new WaitForSeconds(delayBeforeLight);

        if (targetLight != null)
        {
            Color start = targetLight.color;
            for (float t = 0f; t < lightFadeDuration; t += Time.deltaTime)
            {
                targetLight.color = Color.Lerp(start, onColor, t / lightFadeDuration);
                yield return null;
            }
            targetLight.color = onColor;
        }

        onLightOn?.Invoke();

        yield return new WaitForSeconds(holdAfterLight);

        if (cam != null)
        {
            cam.SetParent(camOriginalParent, true);
            cam.localPosition = camOriginalLocalPos;
            cam.localRotation = camOriginalLocalRot;
        }

        if (movement != null) movement.enabled = true;
        if (look != null) look.enabled = true;
        if (input != null) input.enabled = true;
    }
}
