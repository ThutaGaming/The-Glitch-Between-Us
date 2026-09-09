using Akila.FPSFramework.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Player/Interactions Manager")]
    public class InteractionsManager : MonoBehaviour
    {
        [Tooltip("Maximum distance at which the player can interact with an object.")]
        [FormerlySerializedAs("range")]
        public float interactionRange = 2f;

        [Tooltip("How long the interact key must be held for non-instant interactions (e.g., doors, levers).")]
        [FormerlySerializedAs("interactionDuration")]
        public float holdDuration = 0.2f;

        [Tooltip("Defines the interaction angle as a fraction of 360°. Example: 1 = 360°, 0.5 = 180°.")]
        [FormerlySerializedAs("fieldOfInteractions")]
        public float interactionFieldAngle = 0.5f;

        [Tooltip("Layers that contain interactable objects.")]
        [FormerlySerializedAs("interactableLayers")]
        public LayerMask interactableMask = -1;

        [Tooltip("UI element that displays interaction prompts (e.g., 'Press E to Open').")]
        [FormerlySerializedAs("HUDObject")]
        public GameObject interactionHUD;

        [Tooltip("UI text element that shows the interact key (e.g., 'E').")]
        [FormerlySerializedAs("interactKeyText")]
        public TextMeshProUGUI interactKeyLabel;

        [Tooltip("UI text element that displays the action name (e.g., 'Open', 'Pickup').")]
        [FormerlySerializedAs("interactActionText")]
        public TextMeshProUGUI interactActionLabel;

        [Tooltip("Default sound profile to play when interacting.")]
        [FormerlySerializedAs("defaultInteractAudio")]
        public AudioProfile defaultInteractionAudio;

        [Tooltip("Triggered when the player interacts with an object.")]
        [FormerlySerializedAs("onInteract")]
        public UnityEvent<IInteractable> onInteraction;

        [Tooltip("Audio instance used to play interaction sounds.")]
        [FormerlySerializedAs("interactAudio")]
        public Audio interactionAudio;

        /// <summary> Reference to the player's inventory (if present). </summary>
        public IInventory Inventory { get; private set; }

        /// <summary> Whether interactions are currently enabled. </summary>
        public bool IsActive { get; set; } = true;

        /// <summary> Reference to the player's input handler. </summary>
        public CharacterInput CharacterInput { get; private set; }

        /// <summary> The current audio clip used for interactions. </summary>
        public AudioClip CurrentInteractionClip { get; private set; }

        private float holdTimer;
        private float prevHoldTimer;
        private string cachedBindingDisplayString;
        private IInteractable currentInteractable;

        private readonly List<IInteractable> nearbyInteractables = new List<IInteractable>();
        private IInteractable closestInteractable = null;

        private void Start()
        {
            // Get attached components
            Inventory = GetComponent<IInventory>();
            CharacterInput = this.SearchFor<CharacterInput>();

            // Cache the displayed key (like 'E')
            cachedBindingDisplayString = CharacterInput.controls.Player.Interact.GetBindingDisplayString();
        }

        private void OnEnable()
        {
            interactionAudio = new Audio();

            // Setup default audio
            if (defaultInteractionAudio != null)
                CurrentInteractionClip = defaultInteractionAudio.audioClip;

            interactionAudio.Setup(gameObject, defaultInteractionAudio);
        }

        private void Update()
        {
            // Find the current interactable object in range
            currentInteractable = FindClosestInteractable();

            // Update HUD visibility
            if (interactionHUD)
                interactionHUD.SetActive(IsActive && currentInteractable != null);

            // Skip if not active or no interactable object
            if (currentInteractable == null || !IsActive)
                return;

            // Update UI text
            if (interactKeyLabel)
                interactKeyLabel.text = cachedBindingDisplayString;
            if (interactActionLabel)
                interactActionLabel.text = currentInteractable.GetInteractionName();

            // Handle instant and hold interactions
            if (CharacterInput.controls.Player.Interact.triggered)
            {
                holdTimer = 0f;

                if (currentInteractable.isInstant)
                    ExecuteInteraction();
            }

            if (!currentInteractable.isInstant && CharacterInput.controls.Player.Interact.IsPressed())
            {
                holdTimer += Time.deltaTime;
            }

            // Trigger hold interaction when duration reached
            if (!currentInteractable.isInstant && holdTimer > holdDuration && prevHoldTimer < holdDuration)
            {
                ExecuteInteraction();
            }

            prevHoldTimer = holdTimer;
        }

        /// <summary>
        /// Executes the interaction logic on the current interactable.
        /// </summary>
        private void ExecuteInteraction()
        {
            onInteraction?.Invoke(currentInteractable);
            currentInteractable.Interact(this);

            // Cancel reload for all equipped firearms
            foreach (Firearm firearm in GetComponentsInChildren<Firearm>())
            {
                firearm.CancelReload();
            }
        }

        /// <summary>
        /// Finds and returns the closest valid interactable object within range and angle.
        /// </summary>
        private IInteractable FindClosestInteractable()
        {
            nearbyInteractables.Clear();

            Collider[] colliders = Physics.OverlapSphere(transform.position, interactionRange, interactableMask);

            foreach (Collider col in colliders)
            {
                if (col.TryGetComponent(out IInteractable interactable))
                {
                    nearbyInteractables.Add(interactable);
                }
            }

            closestInteractable = null;

            foreach (IInteractable interactable in nearbyInteractables)
            {
                if (closestInteractable == null ||
                    Vector3.Distance(transform.position, interactable.transform.position) <
                    Vector3.Distance(transform.position, closestInteractable.transform.position))
                {
                    closestInteractable = interactable;
                }

                // Check if within allowed interaction angle
                if (Vector3.Dot(transform.forward, (closestInteractable.transform.position - transform.position).normalized)
                    <= interactionFieldAngle)
                {
                    closestInteractable = null;
                }
            }

            return closestInteractable;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
        }

        /// <summary>
        /// Adds or updates the network components required for this manager.
        /// </summary>
        [ContextMenu("Setup/Network Components")]
        private void SetupNetworkComponents()
        {
            FPSFrameworkCore.InvokeConvertMethod("ConvertInteractionsManager", this, new object[] { this });
        }
    }
}
