using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Akila.FPSFramework
{
    /// <summary>
    /// Central manager for character-related systems including input, movement, and camera.
    /// Also tracks ground state transitions and triggers jump/land events.
    /// </summary>
    [AddComponentMenu("Akila/FPS Framework/Player/Character Manager")]
    [RequireComponent(typeof(CharacterInput))]
    public class CharacterManager : MonoBehaviour
    {
        [Header("Events")]
        [Tooltip("Invoked when the character leaves the ground.")]
        public UnityEvent onJump = new UnityEvent();

        [Tooltip("Invoked when the character touches the ground.")]
        public UnityEvent onLand = new UnityEvent();

        /// <summary>
        /// The Actor component attached to this character.
        /// </summary>
        public IActor actor { get; protected set; }

        /// <summary>
        /// The CharacterInput component responsible for user input.
        /// </summary>
        public CharacterInput characterInput { get; protected set; }

        /// <summary>
        /// The camera manager controlling the view.
        /// </summary>
        public CameraManager cameraManager { get; protected set; }

        /// <summary>
        /// The character controller implementation handling movement.
        /// </summary>
        public ICharacterController character { get; protected set; }

        /// <summary>
        /// The current velocity of the character.
        /// </summary>
        public Vector3 velocity { get; protected set; }

        /// <summary>
        /// Whether the character is currently grounded.
        /// </summary>
        public bool isGrounded { get; protected set; }

        /// <summary>
        /// Whether the character was grounded during the last frame.
        /// </summary>
        private bool wasGroundedLastFrame;

        /// <summary>
        /// Whether the character is currently aiming.
        /// </summary>
        public bool isAiming { get; set; }

        /// <summary>
        /// Whether the character is attempting to aim.
        /// </summary>
        public bool attemptingToAim { get; set; }

        public float walkSpeed { get; set; }
        public float sprintSpeed { get; set; }

        public float speedMultiplier { get; set; }

        private void Awake()
        {
            speedMultiplier = 1;
        }

        /// <summary>
        /// Initializes required references and components.
        /// </summary>
        private void Start()
        {
            character = GetComponent<ICharacterController>();
            actor = GetComponent<IActor>();
            characterInput = GetComponent<CharacterInput>();
            cameraManager = this.SearchFor<CameraManager>();

            if (character == null) Debug.LogError("CharacterManager: ICharacterController not found.");
            if (actor == null) Debug.LogWarning("CharacterManager: Actor not found.");
            if (cameraManager == null) Debug.LogWarning("CharacterManager: CameraManager not found in hierarchy.");
        }

        private void LateUpdate()
        {
            if (!hasBeenSetup)
            {
                Debug.LogError(
                    $"[CharacterManager] Initialization error on '{gameObject.name}': SetValues() must be called before using CharacterManager. " +
                    $"Please ensure your character movement script correctly calls SetValues() in Update() function.",
                    gameObject
                );
                return;
            }


            if (FPSFrameworkCore.IsPaused && FPSFrameworkCore.IsFreezOnPauseActive)
            {
                return;
            }

            // Check for landing
            if (isGrounded && !wasGroundedLastFrame)
            {
                try
                {
                    onLand?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            // Check for jumping
            if (!isGrounded && wasGroundedLastFrame)
            {
                try
                {
                    onJump?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            wasGroundedLastFrame = isGrounded;
        }

        private bool hasBeenSetup;

        public void SetValues(Vector3 velocityValue, bool groundedState, float walkSpeed, float sprintSpeed)
        {
            hasBeenSetup = true;

            velocity = velocityValue;
            isGrounded = groundedState;
            this.walkSpeed = walkSpeed;
            this.sprintSpeed = sprintSpeed;
        }

        /// <summary>
        /// Applies camera look input to the character.
        /// </summary>
        /// <param name="mouseX">Horizontal look input (usually mouse X)</param>
        /// <param name="mouseY">Vertical look input (usually mouse Y)</param>
        public virtual void AddLookValue(float mouseX, float mouseY)
        {
            characterInput.AddLookValue(new Vector2(mouseX, mouseY));
        }

        public void SetSpeedMultiplier(float value)
        {
            speedMultiplier = value;
        }

        public void ResetSpeedMultiplier()
        {
            speedMultiplier = 1;
        }

        /// <summary>
        /// Checks if the character is almost stopped (very low velocity).
        /// </summary>
        /// <returns>True if velocity magnitude is less than or equal to 0.5.</returns>
        public virtual bool IsAlmostStopped()
        {
            if (character == null) return false;
            return velocity.magnitude <= 0.7f;
        }

        /// <summary>
        /// Checks if the character has completely stopped (zero velocity).
        /// </summary>
        /// <returns>True if velocity magnitude is zero.</returns>
        public virtual bool IsVelocityZero()
        {
            if (character == null) return false;
            return velocity.magnitude <= 0f;
        }
    }
}
