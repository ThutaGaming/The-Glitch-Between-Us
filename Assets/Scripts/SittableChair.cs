using UnityEngine;

/// <summary>
/// Lets the player sit on this chair via ChairInteraction. Seat pose is a local offset from
/// this transform, since scale/rotation vary per chair asset (office_chair vs modern_chair) —
/// the default is an estimate from the chair's mesh bounds; nudge `seatLocalPosition`/
/// `seatLocalYaw` in the Inspector (see the gizmo drawn at the seat point) if it looks off.
/// </summary>
public class SittableChair : MonoBehaviour, IInteractable
{
    [Header("Seat Pose (local to this chair)")]
    [SerializeField] private Vector3 seatLocalPosition = new Vector3(0f, 0.4f, 0f);
    [Tooltip("Yaw added on top of the chair's own rotation; flip by 180 if the player faces the backrest.")]
    [SerializeField] private float seatLocalYaw = 0f;

    [Header("Seated Camera/Controller")]
    [SerializeField] private float seatCameraHeight = 1.0f;
    [SerializeField] private float seatCCHeight = 1.0f;
    [SerializeField] private Vector3 seatCCCenter = new Vector3(0f, 0.5f, 0f);

    [Header("Stand Up")]
    [SerializeField] private float standUpMargin = 0.5f;
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float groundProbeUpOffset = 2f;
    [SerializeField] private float groundProbeMaxDistance = 10f;

    public bool IsOccupied { get; private set; }

    public Transform InteractTransform => transform;

    public string GetPrompt() => IsOccupied ? "(E) Stand Up" : "(E) Sit on Chair";

    public void Interact(GameObject player)
    {
        if (IsOccupied) StandUp();
        else Sit(player);
    }

    private void OnEnable() => InteractableRegistry.All.Add(this);
    private void OnDisable() => InteractableRegistry.All.Remove(this);

    private Transform occupantBody;
    private Transform occupantCamera;
    private PlayerMovement occupantMovement;
    private CharacterController occupantController;

    public void Sit(GameObject player)
    {
        if (IsOccupied || player == null) return;

        occupantBody = player.transform;
        occupantController = player.GetComponent<CharacterController>();
        occupantMovement = player.GetComponent<PlayerMovement>();
        var cam = player.GetComponentInChildren<Camera>(true);
        occupantCamera = cam != null ? cam.transform : null;

        IsOccupied = true;

        if (occupantMovement != null) occupantMovement.enabled = false;
        if (occupantController != null) occupantController.enabled = false;

        occupantBody.position = transform.TransformPoint(seatLocalPosition);
        occupantBody.rotation = transform.rotation * Quaternion.Euler(0f, seatLocalYaw, 0f);

        if (occupantController != null)
        {
            occupantController.height = seatCCHeight;
            occupantController.center = seatCCCenter;
        }

        if (occupantCamera != null)
        {
            Vector3 lp = occupantCamera.localPosition;
            lp.y = seatCameraHeight;
            occupantCamera.localPosition = lp;
        }
    }

    public void StandUp()
    {
        if (!IsOccupied) return;
        IsOccupied = false;

        float radius = occupantController != null ? occupantController.radius : 0.3f;
        Vector3 stepDir = occupantBody.forward;
        Vector3 toPos = occupantBody.position + stepDir.normalized * (radius + standUpMargin);
        toPos.y = FindGroundY(toPos, occupantBody.position.y);
        occupantBody.position = toPos;

        if (occupantController != null) occupantController.enabled = true;
        if (occupantMovement != null) occupantMovement.enabled = true;

        occupantBody = null;
        occupantCamera = null;
        occupantMovement = null;
        occupantController = null;
    }

    private float FindGroundY(Vector3 worldXZ, float fallbackY)
    {
        Vector3 origin = new Vector3(worldXZ.x, fallbackY + groundProbeUpOffset, worldXZ.z);
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit,
            groundProbeUpOffset + groundProbeMaxDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point.y;
        }

        return fallbackY;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 seatWorld = transform.TransformPoint(seatLocalPosition);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(seatWorld, 0.15f);
        Gizmos.DrawLine(seatWorld, seatWorld + (transform.rotation * Quaternion.Euler(0f, seatLocalYaw, 0f) * Vector3.forward) * 0.5f);
    }
}
