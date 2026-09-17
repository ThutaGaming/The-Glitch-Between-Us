using System;
using System.Collections;
using UnityEngine;
using InfimaGames.LowPolyShooterPack;

/// <summary>
/// Plays once both CoreAnalyzerSwitch terminals are activated (wire to
/// CoreAnalyzerPuzzle.onBothActivated): freezes the player where they stand and cuts the camera to
/// a fixed shot looking down the Core Room at the reactor, the same camera-reparent-and-restore
/// technique CutsceneIntro and IntercomCutscene use. A couple of fast current surges
/// chase up the reactor first (each ring flares then fades as the next one catches, like a jolt of
/// electricity racing up a conduit), then a final pass climbs the same path one more time and stays
/// lit, settling into a steady full charge. Two effects run in lockstep throughout: the reactor's
/// own material glows blue (guaranteed visible regardless of distance/angle, since the reactor is
/// solid and a light placed inside it would be trapped by its own geometry), and rings of point
/// lights stacked outside the reactor's surface. Once fully charged it replays the door-control
/// camera shot from CutsceneIntro/IntercomCutscene a second time via <see cref="finaleCutscene"/>,
/// this time lighting the remaining red indicator.
/// </summary>
public class CoreReactorFinale : MonoBehaviour
{
    [Serializable]
    public class FillLevel
    {
        public Light[] lights;
    }

    [Header("Player")]
    [Tooltip("Player root that carries Movement/CameraLook/PlayerInput. Auto-found by tag if left empty.")]
    [SerializeField] private Character player;

    [Header("Cutscene shot")]
    [Tooltip("World position the camera cuts to for the reactor beat - the Core Room walkway looking down at the core.")]
    [SerializeField] private Vector3 shotPosition = new Vector3(-20.692f, -4.577f, -85.635f);
    [Tooltip("World point the camera looks at from shotPosition.")]
    [SerializeField] private Vector3 shotLookAt = new Vector3(-20.336f, -8.668f, -110.295f);
    [Tooltip("Beat of held shot before the reactor starts charging, so the cut reads before anything moves.")]
    [SerializeField] private float delayBeforeCharge = 0.6f;

    [Header("Reactor surface glow")]
    [Tooltip("The reactor's own renderer - its material emission tracks the light rings, so the effect reads even if the rings alone can't be seen from where the player is standing.")]
    [SerializeField] private Renderer reactorRenderer;
    [SerializeField] private Color emissionColor = new Color(0.25f, 0.65f, 1f);
    [SerializeField] private float emissionIntensity = 2.5f;
    [Tooltip("Brief overshoot on the material during each current surge pulse, on top of emissionIntensity.")]
    [SerializeField] private float surgeEmissionBoost = 1.5f;

    [Header("Reactor fill rings")]
    [Tooltip("Rings of point lights stacked bottom-to-top just outside the reactor's surface.")]
    [SerializeField] private FillLevel[] fillLevels;
    [SerializeField] private Color fillColor = new Color(0.25f, 0.65f, 1f);
    [SerializeField] private float fillIntensity = 6f;
    [SerializeField] private float perLevelDelay = 0.35f;
    [SerializeField] private float fadeInDuration = 0.6f;
    [SerializeField] private float holdAfterFill = 1.5f;

    [Header("Current surge (before the final charge)")]
    [Tooltip("How many times a quick pulse chases bottom-to-top and fades out before the final pass that stays lit.")]
    [SerializeField] private int surgePasses = 2;
    [SerializeField] private float surgePulseDuration = 0.22f;
    [Tooltip("Each surge pass climbs a little faster than the last.")]
    [SerializeField] private float surgeSpeedupPerPass = 0.7f;

    [Header("Finale cutscene")]
    [Tooltip("A second IntercomCutscene instance reusing the door-control shot, targeting the other indicator light.")]
    [SerializeField] private IntercomCutscene finaleCutscene;

    private Movement movement;
    private CameraLook look;
    private UnityEngine.InputSystem.PlayerInput input;
    private Material reactorMat;
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

        if (fillLevels != null)
            foreach (var level in fillLevels)
                if (level?.lights != null)
                    foreach (var l in level.lights)
                        if (l != null) { l.color = fillColor; l.intensity = 0f; l.enabled = true; }

        if (reactorRenderer != null)
        {
            // .material (not sharedMaterial) instances a private copy, so this never touches the
            // shared "Lit" asset other props in the scene use.
            reactorMat = reactorRenderer.material;
            reactorMat.EnableKeyword("_EMISSION");
            reactorMat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            reactorMat.SetColor("_EmissionColor", Color.black);
        }
    }

    /// <summary>Wired to CoreAnalyzerPuzzle.onBothActivated.</summary>
    public void Play()
    {
        if (playing) return;
        playing = true;
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        if (movement != null) movement.enabled = false;
        if (look != null) look.enabled = false;
        if (input != null) input.enabled = false;

        // Character.GetCameraWorld() rather than a GetComponentInChildren search: the earlier
        // cutscenes unparent the camera from the player while they hold it, and a hierarchy search
        // finds nothing in that window - the Character's own reference always resolves.
        Camera worldCamera = player != null ? player.GetCameraWorld() : null;
        Transform cam = worldCamera != null ? worldCamera.transform : null;
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

        if (delayBeforeCharge > 0f) yield return new WaitForSeconds(delayBeforeCharge);

        int levelCount = fillLevels != null ? fillLevels.Length : 0;

        // Current surges: a quick pulse chases up the reactor and fades out behind it, like a jolt
        // of electricity racing up a conduit before the reactor actually catches and holds.
        float pulseDuration = surgePulseDuration;
        for (int pass = 0; pass < surgePasses; pass++)
        {
            if (levelCount > 0)
                for (int i = 0; i < levelCount; i++)
                    yield return PulseLevel(fillLevels[i], pulseDuration);
            else if (reactorRenderer != null)
                yield return PulseLevel(null, pulseDuration);

            pulseDuration *= surgeSpeedupPerPass;
        }

        // Final charge: climbs the same path one more time, this time staying lit.
        if (levelCount > 0)
        {
            for (int i = 0; i < levelCount; i++)
            {
                float from = (float)i / levelCount;
                float to = (float)(i + 1) / levelCount;
                yield return FadeLevel(fillLevels[i], from, to, fadeInDuration);
                yield return new WaitForSeconds(perLevelDelay);
            }
        }
        else if (reactorRenderer != null)
        {
            yield return FadeLevel(null, 0f, 1f, fadeInDuration);
        }

        yield return new WaitForSeconds(holdAfterFill);

        if (cam != null)
        {
            cam.SetParent(camOriginalParent, true);
            cam.localPosition = camOriginalLocalPos;
            cam.localRotation = camOriginalLocalRot;
        }

        if (movement != null) movement.enabled = true;
        if (look != null) look.enabled = true;
        if (input != null) input.enabled = true;

        // IntercomCutscene freezes/restores control and re-cuts the camera on its own, so it's safe
        // to hand both back just before handing straight to the next beat.
        if (finaleCutscene != null) finaleCutscene.Play();
    }

    /// <summary>Flares one ring up then back down to zero inside `duration` - a current pulse
    /// passing through on its way further up the conduit, rather than a level that stays lit.</summary>
    private IEnumerator PulseLevel(FillLevel level, float duration)
    {
        float half = duration * 0.5f;
        float peak = fillIntensity * 1.5f;

        for (float t = 0f; t < half; t += Time.deltaTime)
        {
            SetLevelIntensity(level, Mathf.Lerp(0f, peak, t / half));
            if (reactorMat != null) reactorMat.SetColor("_EmissionColor", emissionColor * (emissionIntensity + surgeEmissionBoost) * (t / half));
            yield return null;
        }
        for (float t = 0f; t < half; t += Time.deltaTime)
        {
            SetLevelIntensity(level, Mathf.Lerp(peak, 0f, t / half));
            if (reactorMat != null) reactorMat.SetColor("_EmissionColor", emissionColor * (emissionIntensity + surgeEmissionBoost) * (1f - t / half));
            yield return null;
        }

        SetLevelIntensity(level, 0f);
        if (reactorMat != null) reactorMat.SetColor("_EmissionColor", Color.black);
    }

    private void SetLevelIntensity(FillLevel level, float intensity)
    {
        if (level?.lights == null) return;
        foreach (var l in level.lights)
            if (l != null) l.intensity = intensity;
    }

    /// <summary>Fades one ring of lights on while ramping the reactor's own emission across the
    /// matching slice of the 0-1 fill range, so the two effects stay in lockstep.</summary>
    private IEnumerator FadeLevel(FillLevel level, float progressFrom, float progressTo, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float p = t / duration;
            if (level?.lights != null)
                foreach (var l in level.lights)
                    if (l != null) l.intensity = Mathf.Lerp(0f, fillIntensity, p);

            if (reactorMat != null)
            {
                float progress = Mathf.Lerp(progressFrom, progressTo, p);
                reactorMat.SetColor("_EmissionColor", emissionColor * (emissionIntensity * progress));
            }
            yield return null;
        }

        if (level?.lights != null)
            foreach (var l in level.lights)
                if (l != null) l.intensity = fillIntensity;
        if (reactorMat != null)
            reactorMat.SetColor("_EmissionColor", emissionColor * (emissionIntensity * progressTo));
    }
}
