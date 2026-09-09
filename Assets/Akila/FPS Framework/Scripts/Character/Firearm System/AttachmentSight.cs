using Akila.FPSFramework.Animation;
using UnityEngine;

namespace Akila.FPSFramework
{
    /// <summary>
    /// Manages a sight attachment for firearms, supporting multiple aim presets (multi-sight functionality).
    /// Handles field of view, position, and rotation changes during aiming and leaning.
    /// </summary>
    [AddComponentMenu("Akila/FPS Framework/Weapons/Attachments/Attachment Sight")]
    public class AttachmentSight : FirearmAttachment
    {
        /// <summary>
        /// Array of sight presets for different aim configurations.
        /// Presets allow multi-sight functionality where different aiming modes can be switched.
        /// </summary>
        [Tooltip("Aim presets. Multiple presets allow multi-sight functionality.")]
        [SerializeField] private SightPreset[] presets;

        /// <summary>
        /// The current index of the active sight preset.
        /// </summary>
        [Tooltip("Index of the currently active sight preset.")]
        [SerializeField] private int index;

        /// <summary>
        /// Gets the currently used sight preset based on the current index.
        /// </summary>
        public SightPreset UsedPreset => presets.Length > 0 ? presets[index] : null;

        [System.Serializable]
        public class SightPreset
        {
            [Header("Zoom Level")]
            [Tooltip("FOV for the main camera when aiming.")]
            public float fieldOfView = 50f;

            [Tooltip("FOV for the weapon camera when aiming.")]
            public float weaponFieldOfView = 40f;

            [Header("Offset")]
            [Tooltip("Position of the firearm when aiming.")]
            public Vector3 position;

            [Tooltip("Rotation of the firearm when aiming.")]
            public Vector3 rotation;

            [Tooltip("Position offset while leaning to the right.")]
            public Vector3 leanRightOffset;

            [Tooltip("Position offset while leaning to the left.")]
            public Vector3 leanLeftOffset;
        }

        private void Update()
        {
            // Validate required components before proceeding
            if (firearm == null)
            {
                return;
            }

            if (attachment == null)
            {
                Debug.LogWarning("Attachment component is missing. Ensure the attachment is properly assigned.", gameObject);
                return;
            }

            if(attachment.IsActive() == false)
            {
                return;
            }

            // Ensure there are presets available and process input for switching modes
            if (presets != null && presets.Length > 0)
            {
                if (itemInput && itemInput.SightModeSwitchInput)
                {
                    // Increment the index and wrap around when necessary
                    index = (index + 1) % presets.Length;
                }

                // Ensure index stays within valid range
                index = Mathf.Clamp(index, 0, presets.Length - 1);
            }
            else
            {
                Debug.LogWarning("No sight presets defined.", gameObject);
                return;
            }

            // Get the current position and rotation for the active preset
            Vector3 targetPosition = UsedPreset.position;
            Vector3 targetRotation = UsedPreset.rotation;

            // Handle animation modifiers for aiming and leaning
            MoveAnimationModifier moveAnimationModifier = firearm?.aimingAnimation?.moveAnimationModifiers?[0];
            MoveAnimationModifier leanRightModifier = firearm?.leanRightAimAnimation?.moveAnimationModifiers?[0];
            MoveAnimationModifier leanLeftModifier = firearm?.leanLeftAimAnimation?.moveAnimationModifiers?[0];

            if (moveAnimationModifier == null)
            {
                //Debug.LogWarning("MoveAnimationModifier not set for aiming animation.", gameObject);
                
                return;
            }

            // Smoothly interpolate to the target position and rotation
            moveAnimationModifier.position = Vector3.Lerp(moveAnimationModifier.position, targetPosition, Time.deltaTime * 10f);
            moveAnimationModifier.rotation = Vector3.Lerp(moveAnimationModifier.rotation, targetRotation, Time.deltaTime * 10f);

            // Apply leaning offsets if available
            if (leanRightModifier != null)
                leanRightModifier.position = UsedPreset.leanRightOffset;

            if (leanLeftModifier != null)
                leanLeftModifier.position = UsedPreset.leanLeftOffset;

            // Update the camera FOV based on the current preset
            if (attachmentsManager != null)
            {
                attachmentsManager.UpdateCameraFOV(UsedPreset.fieldOfView, UsedPreset.weaponFieldOfView, firearm.aimingAnimation.progress);
            }
            else
            {
                Debug.LogWarning("AttachmentsManager is not set.", gameObject);
            }
        }
    }
}
