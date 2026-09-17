using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Test-only shortcut: press F8 to skip straight past Room 1 (first terminal, lab fight, intercom,
/// second terminal) and the Core Room fight, landing just inside the Core Room with both
/// CoreAnalyzerSwitch terminals already armed and ready to interact - so the reactor finale can be
/// tested without replaying the whole level every time.
/// </summary>
public class DebugSkipToCoreRoom : MonoBehaviour
{
    [Tooltip("Ends the opening cutscene first, so the camera is back under the player before the skip - pressing F8 while the intro still holds the camera would otherwise strand it at the intro shot.")]
    [SerializeField] private CutsceneIntro cutsceneIntro;
    [SerializeField] private WallTerminalDoorSwitch firstTerminal;
    [SerializeField] private CombatEncounterManager2 labRoom;
    [SerializeField] private IntercomLightSwitch intercom;
    [SerializeField] private WallTerminalDoorSwitch secondTerminal;
    [SerializeField] private EncounterGroup coreRoomGroup;
    [SerializeField] private CombatEncounterManager2 coreUpper;
    [SerializeField] private CombatEncounterManager2 coreLower;
    [SerializeField] private Transform landingPoint;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.f8Key.wasPressedThisFrame)
            Skip();
    }

    private void Skip()
    {
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo == null) return;

        // Each step reuses the real end-state logic (Arm/Interact/DebugForceClear/DebugSkip), so
        // the doors, HUD and lights all land exactly where they would after actually playing
        // through Room 1 and the Core Room fight - just without the travel time or the fights.
        if (cutsceneIntro != null) cutsceneIntro.EndCutscene();
        if (firstTerminal != null) { firstTerminal.Arm(); firstTerminal.Interact(playerGo); }
        if (labRoom != null) labRoom.DebugForceClear();
        if (intercom != null) intercom.DebugSkip();
        if (secondTerminal != null) secondTerminal.Interact(playerGo);
        if (coreRoomGroup != null) coreRoomGroup.Begin();
        if (coreUpper != null) coreUpper.DebugForceClear();
        if (coreLower != null) coreLower.DebugForceClear();

        if (landingPoint != null)
        {
            var body = playerGo.GetComponent<Rigidbody>();
            playerGo.transform.SetPositionAndRotation(landingPoint.position, landingPoint.rotation);
            if (body != null)
            {
                body.position = landingPoint.position;
                body.rotation = landingPoint.rotation;
                body.linearVelocity = Vector3.zero;
            }
            Physics.SyncTransforms();
        }

        Debug.Log("[DebugSkip] F8: skipped to Core Room, both analyzer terminals armed.");
    }
}
