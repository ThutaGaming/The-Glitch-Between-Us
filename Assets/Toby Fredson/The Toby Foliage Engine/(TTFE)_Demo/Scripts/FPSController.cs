using UnityEngine;
using UnityEngine.InputSystem;

namespace TobyFredson.SceneScriptsMDC
{
	[AddComponentMenu("Toby Fredson/MDC/Scene/FPS Controller")]
	[RequireComponent(typeof(CharacterController))]
	public class FPSController : MonoBehaviour
	{
		[Header("Movement")]
		[SerializeField] private float walkSpeed = 6f;
		[SerializeField] private float runSpeed = 12f;
		[SerializeField] private float jumpPower = 7f;
		[SerializeField] private float gravity = 10f;

		[Header("Mouse Look")]
		[SerializeField] private Camera playerCamera;
		[SerializeField] private float lookSpeed = 0.1f;
		[SerializeField] private float lookXLimit = 45f;

		private Vector3 moveDirection = Vector3.zero;
		private float rotationX = 0f;
		private bool canMove = true;
		private CharacterController characterController;

		void Start()
		{
			characterController = GetComponent<CharacterController>();
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		void Update()
		{
			// === Movement ===
			Vector3 forward = transform.TransformDirection(Vector3.forward);
			Vector3 right = transform.TransformDirection(Vector3.right);

			bool isRunning = Keyboard.current.leftShiftKey.isPressed;

			float vertical = 0f;
			if (Keyboard.current.wKey.isPressed) vertical += 1f;
			if (Keyboard.current.sKey.isPressed) vertical -= 1f;

			float horizontal = 0f;
			if (Keyboard.current.dKey.isPressed) horizontal += 1f;
			if (Keyboard.current.aKey.isPressed) horizontal -= 1f;

			float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * vertical : 0f;
			float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * horizontal : 0f;

			float movementDirectionY = moveDirection.y;
			moveDirection = (forward * curSpeedX) + (right * curSpeedY);

			// Jumping
			if (Keyboard.current.spaceKey.wasPressedThisFrame && canMove && characterController.isGrounded)
			{
				moveDirection.y = jumpPower;
			}
			else
			{
				moveDirection.y = movementDirectionY;
			}

			if (!characterController.isGrounded)
			{
				moveDirection.y -= gravity * Time.deltaTime;
			}

			characterController.Move(moveDirection * Time.deltaTime);

			// === Mouse Look ===
			if (canMove && playerCamera != null)
			{
				Vector2 mouseDelta = Mouse.current.delta.ReadValue();

				rotationX += mouseDelta.y * lookSpeed * -1f;
				rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

				playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
				transform.rotation *= Quaternion.Euler(0f, mouseDelta.x * lookSpeed, 0f);
			}
		}
	}
}