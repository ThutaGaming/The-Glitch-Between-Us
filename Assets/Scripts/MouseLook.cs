using UnityEngine;

/// <summary>
/// Mouse-driven look: yaw rotates the player body, pitch rotates only the camera (clamped).
/// Disabled externally (e.g. by PlayerWakeUpSequence) until control should be handed to the player.
/// </summary>
public class MouseLook : MonoBehaviour
{
    // The frame time the sensitivity was tuned at (60 fps).
    private const float ReferenceFrameTime = 1f / 60f;

    [Header("References")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform cameraTransform;

    [Header("Sensitivity")]
    [SerializeField] private float mouseSensitivity = 200f;

    [Header("Pitch Clamp")]
    [SerializeField] private float minPitch = -85f;
    [SerializeField] private float maxPitch = 85f;

    [SerializeField] private bool lockCursor = true;

    private float pitch;

    private void Awake()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnEnable()
    {
        if (cameraTransform != null)
        {
            pitch = NormalizePitch(cameraTransform.localEulerAngles.x);
        }
    }

    private void Update()
    {
        // GameSettings.Sensitivity is the player's multiplier from the pause menu. The mouse axes
        // are already this frame's movement, so they're scaled by a fixed 60 fps step rather than
        // Time.deltaTime - otherwise the view turns faster on a laptop that runs slower.
        float sensitivity = mouseSensitivity * GameSettings.Sensitivity * ReferenceFrameTime;
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        pitch = Mathf.Clamp(pitch - mouseY, minPitch, maxPitch);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    private static float NormalizePitch(float angle)
    {
        return angle > 180f ? angle - 360f : angle;
    }
}
