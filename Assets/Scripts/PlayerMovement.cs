using UnityEngine;

/// <summary>
/// WASD walk, Shift to sprint, Ctrl to crouch. Ground-relative CharacterController movement
/// with simple gravity. Disabled externally (e.g. by PlayerWakeUpSequence) until control should
/// be handed to the player.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Speeds")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float crouchSpeed = 2f;

    [Header("Crouch Pose")]
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private float standingHeight = 1.8f;
    [SerializeField] private Vector3 standingCenter = new Vector3(0f, 0.9f, 0f);
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private Vector3 crouchCenter = new Vector3(0f, 0.5f, 0f);
    [SerializeField] private float standingCameraY = 1.6f;
    [SerializeField] private float crouchCameraY = 0.9f;
    [SerializeField] private float crouchTransitionSpeed = 8f;

    [Header("Sprint")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Gravity")]
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedStickForce = -2f;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private bool isCrouching;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleCrouch();
        HandleMove();
    }

    private void HandleCrouch()
    {
        isCrouching = Input.GetKey(crouchKey);

        float targetHeight = isCrouching ? crouchHeight : standingHeight;
        Vector3 targetCenter = isCrouching ? crouchCenter : standingCenter;
        float lerpFactor = Time.deltaTime * crouchTransitionSpeed;

        controller.height = Mathf.Lerp(controller.height, targetHeight, lerpFactor);
        controller.center = Vector3.Lerp(controller.center, targetCenter, lerpFactor);

        if (cameraTransform != null)
        {
            float targetCameraY = isCrouching ? crouchCameraY : standingCameraY;
            Vector3 localPos = cameraTransform.localPosition;
            localPos.y = Mathf.Lerp(localPos.y, targetCameraY, lerpFactor);
            cameraTransform.localPosition = localPos;
        }
    }

    private void HandleMove()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        float speed = isCrouching ? crouchSpeed : (Input.GetKey(sprintKey) ? sprintSpeed : walkSpeed);

        if (controller.isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = groundedStickForce;
        }
        verticalVelocity.y += gravity * Time.deltaTime;

        Vector3 motion = moveDirection * speed + Vector3.up * verticalVelocity.y;
        controller.Move(motion * Time.deltaTime);
    }
}
