using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Akila.FPSFramework
{
    /// <summary>
    /// Handles input for player item interactions such as firing, aiming, reloading, switching fire modes, and throwing items.
    /// </summary>
    [AddComponentMenu("Akila/FPS Framework/Player/Item Input")]
    public class ItemInput : MonoBehaviour
    {
        [Tooltip("Allow player input even when the game is paused. -> if FPSFrameworkCore.IsPaused game is paused.")]
        public bool allowInputWhilePaused;

        /// <summary>
        /// Gets whether aiming is toggled.
        /// </summary>
        public bool ToggleAim { get; protected set; }

        /// <summary>
        /// Gets the input controls for the player.
        /// </summary>
        public Controls Controls { get; private set; }

        /// <summary>
        /// Gets the inventory item holder for managing equipped items.
        /// </summary>
        public IInventory Inventory { get; private set; }

        /// <summary>
        /// Gets the character input component associated with the player.
        /// </summary>
        public CharacterInput CharacterInput { get; private set; }

        /// <summary>
        /// Gets whether the reload input was triggered this frame.
        /// </summary>
        public virtual bool ReloadInput => reloadInput;

        /// <summary>
        /// Gets whether the fire mode switch input was triggered this frame.
        /// </summary>
        public virtual bool FireModeSwitchInput => fireModeSwitchInput;

        /// <summary>
        /// Gets whether the sight mode switch input was triggered this frame.
        /// </summary>
        public virtual bool SightModeSwitchInput => sightModeSwitchInput;

        /// <summary>
        /// Gets whether the aim input is active.
        /// </summary>
        public virtual bool AimInput => aimInput;

        /// <summary>
        /// Gets whether the drop input was triggered this frame.
        /// </summary>
        public virtual bool DropInput => dropInput;

        /// <summary>
        /// Gets whether the fire button was triggered this frame.
        /// </summary>
        public virtual bool SingleFire => triggeredFire;

        /// <summary>
        /// Gets whether the fire button is being held.
        /// </summary>
        public virtual bool HeldFire => heldFire;

        /// <summary>
        /// Gets the input action used for throwing items.
        /// </summary>
        public InputAction ThrowAction { get; private set; }

        private bool reloadInput;
        private bool fireModeSwitchInput;
        private bool sightModeSwitchInput;
        private bool aimInput;
        private bool dropInput;
        private bool triggeredFire;
        private bool heldFire;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Initializes input controls.
        /// </summary>
        private void Awake()
        {
            Controls = new Controls();

            FPSFrameworkCore.IsInputActive = true;
            FPSFrameworkCore.IsActive = true;
        }

        /// <summary>
        /// Called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            try
            {
                Inventory = GetComponentInParent<IInventory>();
                if (Inventory == null)
                {
                    Debug.LogError("ItemInput: IInventory component not found in parent. Ensure it exists in the hierarchy.");
                    return;
                }

                CharacterInput = GetComponentInParent<CharacterInput>();

                AddInputListeners();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"ItemInput: Initialization failed. Exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            aimInput = false;
            InitializeControls();
        }

        /// <summary>
        /// Called when the object becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            Controls.Disable();
        }

        /// <summary>
        /// Called when the MonoBehaviour will be destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Controls.Disable();
            Controls.Dispose();
        }

        /// <summary>
        /// Called once per frame to process input.
        /// </summary>
        private void Update()
        {
            if (!FPSFrameworkCore.IsActive || !FPSFrameworkCore.IsInputActive || !allowInputWhilePaused && FPSFrameworkCore.IsPaused)
            {
                reloadInput = false;
                fireModeSwitchInput = false;
                sightModeSwitchInput = false;
                aimInput = false;
                dropInput = false;
                triggeredFire = false;
                heldFire = false;

                return;
            }

            // Update all relevant input flags
            reloadInput = Controls.Firearm.Reload.triggered;
            fireModeSwitchInput = Controls.Firearm.FireModeSwich.triggered;
            sightModeSwitchInput = Controls.Firearm.SightModeSwitch.triggered;
            dropInput = Controls.Firearm.Drop.triggered;

            ToggleAim = CharacterInput.toggleAim;

            triggeredFire = Controls.Firearm.Fire.triggered;
            heldFire = Controls.Firearm.Fire.IsPressed();
        }

        /// <summary>
        /// Initializes and enables input actions.
        /// </summary>
        private void InitializeControls()
        {
            Controls.Firearm.Enable();
            Controls.Throwable.Enable();

            ThrowAction = Controls.Throwable.Throw;

            ThrowAction.Enable();
        }

        /// <summary>
        /// Adds listeners for complex input actions like aiming.
        /// </summary>
        private void AddInputListeners()
        {
            Controls.Firearm.Aim.performed += context =>
            {
                if (FPSFrameworkCore.IsActive && FPSFrameworkCore.IsInputActive)
                    aimInput = ToggleAim ? !aimInput : true;
                else
                    aimInput = false;
            };

            Controls.Firearm.Aim.canceled += context =>
            {
                if (FPSFrameworkCore.IsActive && FPSFrameworkCore.IsInputActive && !ToggleAim)
                {
                    aimInput = false;
                }
            };
        }
    }
}
