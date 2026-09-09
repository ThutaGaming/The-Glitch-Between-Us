using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Akila.FPSFramework
{
    /// <summary>
    /// Handles all character-related input for the FPS Framework using Unity's Input System.
    /// Processes movement, looking, jumping, crouching, sprinting, leaning, and camera control.
    /// </summary>
    [AddComponentMenu("Akila/FPS Framework/Player/Character Input")]
    public class CharacterInput : MonoBehaviour
    {
        [Header("Toggles")]
        [Tooltip("Allow player input even when the game is paused. -> if FPSFrameworkCore.IsPaused game is paused.")]
        public bool allowInputWhilePaused;

        [Tooltip("Toggle mode for aiming instead of hold.")]
        public bool toggleAim = false;

        [Tooltip("Toggle mode for crouch instead of hold.")]
        public bool toggleCrouch = true;

        [Tooltip("Toggle mode for leaning instead of hold.")]
        public bool toggleLean = false;

        [Tooltip("If enabled, sprinting is toggled: holding the sprint button will disable sprinting, and releasing it will enable sprinting.")]
        public bool reverseSprintInput = false;

        [Tooltip("Allows tactical sprinting if enabled.")]
        public bool allowTacticalSprining = true;

        [Header("Sensitivity")]
        [Tooltip("Sensitivity of camera movement while using mouse.")]
        public float sensitivityOnMouse = 1;
        [Tooltip("Sensitivity of camera movement while using Gamepad.")]
        public float sensitivityOnGamepad = 1;
        [Tooltip("Defines how camera sensitivity changes based on the field of view (FOV).")]
        public AnimationCurve fovToSensitivityCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0) });
        [Tooltip("Toggle dynamic camera sensitivity adjustment based on the field of view (FOV).")]
        public bool isDynamicSensitivityEnabled = true;

        /// <summary>
        /// Event invoked when lean right input is detected.
        /// </summary>
        public Action onLeanRight;

        /// <summary>
        /// Event invoked when lean left input is detected.
        /// </summary>
        public Action onLeanLeft;

        /// <summary>
        /// Reference to the main input action map.
        /// </summary>
        public Controls controls;

        /// <summary>
        /// The character manager that owns this input handler.
        /// </summary>
        public CharacterManager characterManager { get; protected set; }

        private CameraManager cameraManager;

        /// <summary>
        /// Cached reference to the main camera.
        /// </summary>
        public Camera mainCamera { get; protected set; }

        /// <summary>
        /// Input for movement direction (x = strafe, y = forward/back).
        /// </summary>
        protected Vector2 moveInputRaw { get; set; }
        public virtual Vector2 MoveInput => moveInputRaw.normalized;
        public virtual Vector2 MoveInputRaw => moveInputRaw;

        /// <summary>
        /// Raw input from the mouse/controller for camera rotation.
        /// </summary>
        protected Vector2 rawLookInput { get; set; }
        public virtual Vector2 RawLookInput => rawLookInput;

        /// <summary>
        /// Final calculated camera look input with sensitivity and smoothing applied.
        /// </summary>
        protected Vector2 lookInput { get; set; }
        public virtual Vector2 LookInput => lookInput;

        /// <summary>
        /// Whether sprint input is currently being held.
        /// </summary>
        protected bool sprintInput { get; set; }
        public virtual bool SprintInput => sprintInput;

        /// <summary>
        /// Whether tactical sprint input is currently being held.
        /// </summary>
        protected bool tacticalSprintInput { get; set; }
        public virtual bool TacticalSprintInput => tacticalSprintInput;

        /// <summary>
        /// Raw tactical sprint input used to detect double-clicks.
        /// </summary>
        protected bool rawTacticalSprintInput;
        public virtual bool RawTacticalSprintInput => rawTacticalSprintInput;

        /// <summary>
        /// Whether the jump input was triggered this frame.
        /// </summary>
        protected bool jumpInput { get; set; }
        public virtual bool JumpInput => jumpInput;

        /// <summary>
        /// Whether the crouch input is active.
        /// </summary>
        protected bool crouchInput { get; set; }
        public virtual bool CrouchInput => crouchInput;

        /// <summary>
        /// Whether lean right input is active.
        /// </summary>
        protected bool leanRightInput { get; set; }
        public virtual bool LeanRightInput => leanRightInput;

        /// <summary>
        /// Whether lean left input is active.
        /// </summary>
        protected bool leanLeftInput { get; set; }
        public virtual bool LeanLeftInput => leanLeftInput;

        /// <summary>
        /// Additional look rotation manually added to the input.
        /// </summary>
        protected Vector2 addedLookValue { get; set; }
        public virtual Vector2 AddedLookValue => addedLookValue;

        ///Inventory input
        public virtual bool item1 => IsInputAllowed ? controls.Player.Item1.triggered : false;
        public virtual bool item2 => IsInputAllowed ? controls.Player.Item2.triggered : false;
        public virtual bool item3 => IsInputAllowed ? controls.Player.Item3.triggered : false;
        public virtual bool item4 => IsInputAllowed ? controls.Player.Item4.triggered : false;
        public virtual bool item5 => IsInputAllowed ? controls.Player.Item5.triggered : false;
        public virtual bool item6 => IsInputAllowed ? controls.Player.Item6.triggered : false;
        public virtual bool item7 => IsInputAllowed ? controls.Player.Item7.triggered : false;
        public virtual bool item8 => IsInputAllowed ? controls.Player.Item8.triggered : false;
        public virtual bool item9 => IsInputAllowed ? controls.Player.Item9.triggered : false;

        public virtual bool itemUp => IsInputAllowed ? controls.Player.SwitchItem.ReadValue<float>() > 0 : false;
        public virtual bool itemDown => IsInputAllowed ? controls.Player.SwitchItem.ReadValue<float>() < 0 : false;

        public virtual bool itemNext => IsInputAllowed ? controls.Player.NextItem.triggered : false;
        public virtual bool itemPrevious => IsInputAllowed ? controls.Player.PreviousItem.triggered : false;

        public virtual bool defaultItem => IsInputAllowed ? controls.Player.DefaultItem.triggered : false;

        // Helper property for readability
        protected bool IsInputAllowed => FPSFrameworkCore.IsActive && FPSFrameworkCore.IsInputActive;


        /// <summary>
        /// Raw sprint input, tracked independently for external checks.
        /// </summary>
        [HideInInspector] public bool rawSprintInput;

        private float lastSprintClickTime;

        public bool IsSprintingAtAll()
        {
            if (tacticalSprintInput || sprintInput) return true;

            return false;
        }

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            controls = new Controls();

            FPSFrameworkCore.IsInputActive = true;
            FPSFrameworkCore.IsActive = true;
        }

        /// <summary>
        /// Called before the first frame update.
        /// </summary>
        private void Start()
        {
            characterManager = GetComponent<CharacterManager>();
            cameraManager = this.DeepSearch<CameraManager>();
        }

        /// <summary>
        /// Called every frame to process and update input values.
        /// </summary>
        protected void Update()
        {
            if(!FPSFrameworkCore.IsActive || !FPSFrameworkCore.IsInputActive || !allowInputWhilePaused && FPSFrameworkCore.IsPaused)
            {
                moveInputRaw = Vector2.zero;
                rawLookInput = Vector2.zero;
                lookInput = Vector2.zero;
                sprintInput = false;
                tacticalSprintInput = false;
                rawTacticalSprintInput = false;
                jumpInput = false;
                leanRightInput = false;
                leanLeftInput = false;
                addedLookValue = Vector2.zero;
                rawSprintInput = false;
                return;
            }

            if(controls == null)
            {
                return;
            }

            moveInputRaw = controls.Player.Move.ReadValue<Vector2>();

            Vector2 rawLookUnscaled = controls.Player.Look.ReadValue<Vector2>();
            rawLookInput = 100 * new Vector2(
                rawLookUnscaled.x * FPSFrameworkCore.XSensitivityMultiplier,
                rawLookUnscaled.y * FPSFrameworkCore.YSensitivityMultiplier
            ) * FPSFrameworkCore.SensitivityMultiplier;

            if (MoveInput.y < 0)
            {
                sprintInput = false;
                tacticalSprintInput = false;
            }
            else
            {
                tacticalSprintInput = MoveInput.y > 0 && allowTacticalSprining ? rawTacticalSprintInput : false;
                sprintInput = MoveInput.y > 0 ? rawSprintInput : false;
            }

            rawSprintInput = reverseSprintInput ? !controls.Player.Sprint.IsPressed() : controls.Player.Sprint.IsPressed();

            if (tacticalSprintInput)
                sprintInput = false;

            controls.Player.TacticalSprint.HasDoupleClicked(ref rawTacticalSprintInput, ref lastSprintClickTime);

            jumpInput = controls.Player.Jump.triggered;

            mainCamera = Camera.main;

            float finalSensitivity = 1f;
            float deviceSenstivity = 1f;

            if(FPSFrameworkCore.GetActiveControlScheme() == ControlScheme.Mouse)
            {
                deviceSenstivity = sensitivityOnMouse * 0.1f;
            }

            if(FPSFrameworkCore.GetActiveControlScheme() == ControlScheme.Gamepad)
            {
                deviceSenstivity = sensitivityOnGamepad;
            }

            if (characterManager?.character != null)
            {
                float baseSensitivity = deviceSenstivity;

                if (mainCamera != null && isDynamicSensitivityEnabled)
                {
                    float defaultFov = cameraManager ? cameraManager.defaultMainFOV : 60f;

                    finalSensitivity = Mathf.Lerp(
                        baseSensitivity,
                        0,
                        fovToSensitivityCurve.Evaluate(mainCamera.fieldOfView / defaultFov)
                    );
                }
                else
                {
                    finalSensitivity = baseSensitivity;
                }
            }

            if (FPSFrameworkCore.IsPaused)
                finalSensitivity = 0;

            Vector2 calculatedLook = addedLookValue + (rawLookInput * finalSensitivity);

            lookInput = (calculatedLook / 200f) + addedLookValue;

            addedLookValue = Vector2.zero;

            // Force stand if trying to jump or sprint while crouching
            if ((sprintInput || jumpInput) && crouchInput)
                crouchInput = false;
        }

        /// <summary>
        /// Ensures lean directions are exclusive and cancelled if sprinting.
        /// </summary>
        private void LateUpdate()
        {
            if ((leanRightInput && leanLeftInput) || sprintInput || tacticalSprintInput)
            {
                leanRightInput = false;
                leanLeftInput = false;
            }
        }

        /// <summary>
        /// Adds event listeners to the input action map.
        /// </summary>
        protected void AddInputListner()
        {
            if (controls?.Player == null)
                return;

            controls.Player.Crouch.performed += _ =>
            {
                if (FPSFrameworkCore.IsActive && FPSFrameworkCore.IsInputActive && !toggleCrouch) crouchInput = true;
            };

            controls.Player.Crouch.canceled += _ =>
            {
                if (FPSFrameworkCore.IsActive && FPSFrameworkCore.IsInputActive)
                    crouchInput = toggleCrouch ? !crouchInput : false;
            };

            controls.Player.LeanRight.performed += _ =>
            {
                if (FPSFrameworkCore.IsActive && FPSFrameworkCore.IsInputActive && !toggleLean) 
                    leanRightInput = true;
            };

            controls.Player.LeanRight.canceled += _ =>
            {
                if (FPSFrameworkCore.IsActive && FPSFrameworkCore.IsInputActive)
                {
                    if (!toggleLean)
                        leanRightInput = false;
                    else
                    {
                        leanLeftInput = false;
                        leanRightInput = !leanRightInput;
                    }
                }
            };

            controls.Player.LeanLeft.performed += _ =>
            {
                if (FPSFrameworkCore.IsActive && FPSFrameworkCore.IsInputActive &&!toggleLean) leanLeftInput = true;
            };

            controls.Player.LeanLeft.canceled += _ =>
            {
                if (FPSFrameworkCore.IsActive && FPSFrameworkCore.IsInputActive)
                {
                    if (!toggleLean)
                        leanLeftInput = false;
                    else
                    {
                        leanRightInput = false;
                        leanLeftInput = !leanLeftInput;
                    }
                }
            };
        }

        /// <summary>
        /// Adds a specified amount of look input, useful for scripted rotation.
        /// </summary>
        /// <param name="value">The amount of look input to add.</param>
        public virtual void AddLookValue(Vector2 value)
        {
            if (FPSFrameworkCore.IsActive && FPSFrameworkCore.IsInputActive)
                addedLookValue += value;
        }

        /// <summary>
        /// Enables the input system and registers input listeners.
        /// </summary>
        private void OnEnable()
        {
            controls?.Enable();
            AddInputListner();
        }

        /// <summary>
        /// Disables the input system.
        /// </summary>
        private void OnDisable()
        {
            controls?.Disable();
        }

        /// <summary>
        /// Cleans up the input system on destroy.
        /// </summary>
        private void OnDestroy()
        {
            controls?.Disable();
            controls?.Dispose();
        }
    }
}
