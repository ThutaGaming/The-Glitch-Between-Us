using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Test-only shortcut for Level 1: press 8 to skip Rooms 1-2 and the network puzzle, landing in
/// Room 3 facing the blast gate with the gate already open - "Head to the door" showing, the
/// door marker lit and the [E] exit to Level 2 ready - so the level exit can be tested without
/// replaying the level every time.
/// </summary>
public class DebugSkipToBlastGate : MonoBehaviour
{
    [SerializeField] private NetworkPuzzleTerminal terminal;
    [SerializeField] private Room3BlastGateController blastGate;
    [SerializeField] private Transform landingPoint;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.digit8Key.wasPressedThisFrame)
            Skip();
    }

    private void Skip()
    {
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo == null) return;

        // Reuses the gate's real end state and onOpened wiring, so the objective, the door glow
        // and LevelExitDoor all land exactly where they would after solving the puzzle.
        if (terminal != null) terminal.DebugMarkSolved();
        if (blastGate != null) blastGate.OpenInstantly();

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

        Debug.Log("[DebugSkip] 8: skipped to Level 1 Room 3, blast gate open.");
    }
}
