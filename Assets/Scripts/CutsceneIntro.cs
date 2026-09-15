using System.Collections;
using UnityEngine;
using InfimaGames.LowPolyShooterPack;

/// <summary>
/// Plays once when the scene starts: freezes the player, cuts the player's own camera to a fixed
/// framing of a world object (e.g. a door-control panel) while an EscapeProtocolHologram briefing
/// explains it, then cuts the camera back and hands control back once the hologram is dismissed.
/// Reuses the player's single Camera/AudioListener instead of a second cutscene camera, so there's
/// no listener-duplication or camera-priority juggling - just a temporary local transform override.
/// </summary>
public class CutsceneIntro : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Player root that carries Movement/CameraLook/PlayerInput. Auto-found by tag if left empty.")]
    [SerializeField] private Character player;

    [Header("Cutscene shot")]
    [Tooltip("World position the camera cuts to.")]
    [SerializeField] private Vector3 shotPosition;
    [Tooltip("World point the camera looks at from shotPosition.")]
    [SerializeField] private Vector3 shotLookAt;

    [Header("Briefing")]
    [SerializeField] private EscapeProtocolHologram hologram;

    [Header("Timing")]
    [SerializeField] private float startDelay = 0.5f;

    private Movement movement;
    private CameraLook look;
    private UnityEngine.InputSystem.PlayerInput input;
    private Transform cam;
    private Transform camOriginalParent;
    private Vector3 camOriginalLocalPos;
    private Quaternion camOriginalLocalRot;

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
            cam = player.GetComponentInChildren<Camera>(true).transform;
        }
    }

    private void Start()
    {
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        if (startDelay > 0f) yield return new WaitForSeconds(startDelay);

        if (movement != null) movement.enabled = false;
        if (look != null) look.enabled = false;
        if (input != null) input.enabled = false;

        if (cam != null)
        {
            camOriginalParent = cam.parent;
            camOriginalLocalPos = cam.localPosition;
            camOriginalLocalRot = cam.localRotation;

            cam.SetParent(null, true);
            cam.position = shotPosition;
            cam.rotation = Quaternion.LookRotation(shotLookAt - shotPosition, Vector3.up);
        }

        if (hologram != null) hologram.Begin();
    }

    /// <summary>Wired to EscapeProtocolHologram.onFinished.</summary>
    public void EndCutscene()
    {
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
