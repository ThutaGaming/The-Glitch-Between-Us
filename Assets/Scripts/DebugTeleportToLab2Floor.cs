using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Test-only shortcut: press 8 to drop the player standing on top of CR_Lab_2_Floor_199, the small
/// upper-floor platform the turrets/boss share in Level 4 - useful for testing that fight without
/// walking the whole level every time. The landing spot is computed from the floor's own renderer
/// bounds rather than a hand-placed marker, so it still lands correctly if the floor piece moves.
/// </summary>
public class DebugTeleportToLab2Floor : MonoBehaviour
{
    [SerializeField] private string floorObjectName = "CR_Lab_2_Floor_199";
    [SerializeField] private float standClearance = 0.1f;

    private Transform floor;
    private Vector3 landingPoint;
    private bool landingPointReady;

    private void Awake()
    {
        floor = FindFloor();
        if (floor == null)
        {
            Debug.LogWarning("[DebugTeleport] Could not find '" + floorObjectName + "' in the scene.");
            return;
        }

        var renderers = floor.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            Debug.LogWarning("[DebugTeleport] '" + floorObjectName + "' has no renderers to measure.");
            return;
        }

        Bounds bounds = renderers[0].bounds;
        foreach (var r in renderers) bounds.Encapsulate(r.bounds);
        landingPoint = new Vector3(bounds.center.x, bounds.max.y + standClearance, bounds.center.z);
        landingPointReady = true;
    }

    private Transform FindFloor()
    {
        foreach (var t in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (t.name == floorObjectName) return t;
        return null;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.digit8Key.wasPressedThisFrame)
            Teleport();
    }

    private void Teleport()
    {
        if (!landingPointReady)
        {
            Debug.LogWarning("[DebugTeleport] No landing point resolved for '" + floorObjectName + "'.");
            return;
        }

        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo == null) return;

        playerGo.transform.SetPositionAndRotation(landingPoint, playerGo.transform.rotation);
        var body = playerGo.GetComponent<Rigidbody>();
        if (body != null)
        {
            body.position = landingPoint;
            body.linearVelocity = Vector3.zero;
        }
        Physics.SyncTransforms();

        Debug.Log("[DebugTeleport] 8: teleported player onto " + floorObjectName + ".");
    }
}
