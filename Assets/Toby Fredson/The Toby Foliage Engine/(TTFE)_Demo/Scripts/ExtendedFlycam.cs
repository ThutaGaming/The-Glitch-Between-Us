using UnityEngine;
using UnityEngine.InputSystem;

namespace TobyFredson.SceneScripts
{
	[AddComponentMenu("Toby Fredson/The Toby Foliage Engine/Scene/Extended Flycam")]
	public class ExtendedFlycam : MonoBehaviour
	{
		/*
            EXTENDED FLYCAM (Unity 6 + Input System)
            - Minimal changes from original
            - Fixed Input System error
            - Stable rotation (no random inversion)
            - Y axis now correctly NOT inverted
            - Mouse speed increased
        */

		public float cameraSensitivity = 5f;
		public float climbSpeed = 4f;
		public float normalMoveSpeed = 10f;
		public float slowMoveFactor = 0.25f;
		public float fastMoveFactor = 3f;

		private float rotationX = 0f;   // yaw (left/right)
		private float rotationY = 0f;   // pitch (up/down)

		void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		void Update()
		{
			// === Mouse Look ===
			Vector2 mouseDelta = Mouse.current.delta.ReadValue();

			rotationX += mouseDelta.x * cameraSensitivity * 0.036f;   // increased for faster feel
			rotationY -= mouseDelta.y * cameraSensitivity * 0.036f;   // minus sign fixes inversion

			rotationY = Mathf.Clamp(rotationY, -90f, 90f);

			if (mouseDelta.sqrMagnitude > 0.01f)
			{
				// Stable & reliable rotation (yaw on world Y, pitch on local X)
				transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);
			}

			// === Movement Speed ===
			float moveSpeed = normalMoveSpeed;
			if (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed)
			{
				moveSpeed *= fastMoveFactor;
			}
			else if (Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed)
			{
				moveSpeed *= slowMoveFactor;
			}

			// === WASD / Arrow Movement ===
			float vertical = 0f;
			float horizontal = 0f;

			if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)    vertical += 1f;
			if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)  vertical -= 1f;
			if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)  horizontal -= 1f;
			if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontal += 1f;

			transform.position += transform.forward * moveSpeed * vertical * Time.deltaTime;
			transform.position += transform.right   * moveSpeed * horizontal * Time.deltaTime;

			// === Climb / Drop (Q / E) ===
			if (Keyboard.current.qKey.isPressed)
				transform.position += transform.up * climbSpeed * Time.deltaTime;

			if (Keyboard.current.eKey.isPressed)
				transform.position -= transform.up * climbSpeed * Time.deltaTime;

			// Toggle cursor lock with End key
			if (Keyboard.current.endKey.wasPressedThisFrame)
			{
				Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked)
					? CursorLockMode.None
					: CursorLockMode.Locked;
			}
		}
	}
}