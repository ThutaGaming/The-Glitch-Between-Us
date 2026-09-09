using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework 
{
    [AddComponentMenu("Akila/FPS Framework/Player/Door")]
    public class Door : MonoBehaviour, IInteractable
    {
        /// <summary>
        /// Text displayed to the player when interacting with the door.
        /// </summary>
        [Tooltip("What is shown to the player when opening or closing the door")]
        public string interactionName = "Open/Close";
        public bool instant = false;

        /// <summary>
        /// The pivot point around which the door rotates.
        /// </summary>
        public Transform pivot;

        /// <summary>
        /// Controls the speed of the door's rotation transition.
        /// Higher values make the door movement faster and rougher.
        /// </summary>
        public float roughness = 10;

        /// <summary>
        /// The rotation of the door when it is fully open.
        /// </summary>
        public Vector3 openRotation = new Vector3(0, -90, 0);

        /// <summary>
        /// The target rotation of the door during the transition.
        /// </summary>
        private Vector3 targetRotation;

        /// <summary>
        /// The default rotation of the door when it is closed.
        /// </summary>
        private Vector3 defaultRotation;

        // Timer for potential future use (currently not used)
        private float timer;

        /// <summary>
        /// Indicates whether the door is open or closed.
        /// </summary>
        public bool isOpen { get; protected set; }

        /// <summary>
        /// The transform of the player who last interacted.
        /// </summary>
        public Transform interactionSource { get; protected set; }

        public bool isInstant => instant;

        /// <summary>
        /// Initializes the door's default rotation.
        /// </summary>
        protected virtual void Start()
        {
            // Store the default rotation of the pivot point
            defaultRotation = pivot.eulerAngles;
        }

        /// <summary>
        /// Smoothly rotates the door towards the target rotation on every frame update.
        /// </summary>
        protected virtual void Update()
        {
            // Smoothly rotate the door towards the target rotation using Lerp for a smooth transition
            pivot.rotation = Quaternion.Lerp(pivot.rotation, Quaternion.Euler(defaultRotation + targetRotation), Time.deltaTime * roughness);
        }

        /// <summary>
        /// Gets the interaction text that the player will see.
        /// </summary>
        /// <returns>The interaction name.</returns>
        public virtual string GetInteractionName()
        {
            return interactionName;
        }

        /// <summary>
        /// Handles the interaction when a player interacts with the door using the InteractionsManager.
        /// Toggles the door state (open/close) based on its current state.
        /// </summary>
        /// <param name="source">The source InteractionsManager triggering the interaction.</param>
        public virtual void Interact(InteractionsManager source)
        {
            // Toggle the door state (open/close)
            isOpen = !isOpen;

            if (isOpen)
            {
                // Calculate the direction from the source to the door and set the target rotation
                Vector3 dir = (source.transform.position - transform.position).normalized;
                targetRotation.x = -Mathf.Sign(Vector3.Dot(transform.right, dir)) * openRotation.x;
                targetRotation.y = -Mathf.Sign(Vector3.Dot(transform.right, dir)) * openRotation.y;
                targetRotation.z = -Mathf.Sign(Vector3.Dot(transform.right, dir)) * openRotation.z;
            }
            else
            {
                // Reset the target rotation to zero when the door is closed
                targetRotation = Vector3.zero;
            }
        }

        /// <summary>
        /// Handles the interaction using a Transform as the source.
        /// Toggles the door state (open/close) based on its current state.
        /// </summary>
        /// <param name="source">The Transform triggering the interaction.</param>
        public virtual void Interact(Transform source)
        {
            interactionSource = source;

            // Toggle the door state (open/close)
            isOpen = !isOpen;

            if (isOpen)
            {
                // Calculate the direction from the source to the door and set the target rotation
                Vector3 dir = (source.position - transform.position).normalized;
                targetRotation.x = -Mathf.Sign(Vector3.Dot(transform.right, dir)) * openRotation.x;
                targetRotation.y = -Mathf.Sign(Vector3.Dot(transform.right, dir)) * openRotation.y;
                targetRotation.z = -Mathf.Sign(Vector3.Dot(transform.right, dir)) * openRotation.z;
            }
            else
            {
                // Reset the target rotation to zero when the door is closed
                targetRotation = Vector3.zero;
            }
        }
    }

}