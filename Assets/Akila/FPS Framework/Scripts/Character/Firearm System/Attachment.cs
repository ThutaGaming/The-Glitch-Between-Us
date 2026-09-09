using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    /// <summary>
    /// Represents an attachment for a firearm in the FPS framework.
    /// This class allows configuration of various firearm attributes such as damage, fire rate, recoil, etc.
    /// </summary>
    [AddComponentMenu("Akila/FPS Framework/Weapons/Attachments/Attachment")]
    public class Attachment : MonoBehaviour
    {
        // Base information for the attachment
        [Header("Base")]
        public string type; // Type/category of attachment (e.g., scope, barrel)
        public string Name; // Name of the attachment (e.g., "Red Dot Sight")

        // Multiplier attributes that affect the firearm's behavior
        [Header("Multipliers")]
        [Range(1, 500), SerializeField, Tooltip("Effects firearm damage")]
        float m_damage = 100;

        [Range(1, 500), SerializeField, Tooltip("Effects firearm shooting spread")]
        float m_spread = 100;

        [Range(1, 500), SerializeField, Tooltip("Effects firearm fire rate")]
        float m_fireRate = 100;

        [Range(1, 500), SerializeField, Tooltip("Effects firearm damage range (Damage Only)")]
        float m_range = 100;

        [Range(1, 500), SerializeField, Tooltip("Effects firearm projectile velocity")]
        float m_muzzleVelocity = 100;

        [Range(1, 500), SerializeField, Tooltip("Effects firearm recoil (Camera Movement)")]
        float m_recoil = 100;

        [Range(1, 500), SerializeField, Tooltip("Effects firearm visible model recoil")]
        float m_visualRecoil = 100;

        [Range(1, 500), SerializeField, Tooltip("Effects firearm aim down sight speed")]
        float m_aimSpeed = 100;

        // Reference to the FirearmAttachmentsManager that handles active attachments
        public FirearmAttachmentsManager attachmentsManager { get; set; }

        /// <summary>
        /// Returns the normalized damage multiplier (scaled from 0 to 1).
        /// </summary>
        public float Damage()
        {
            return m_damage / 100;
        }

        /// <summary>
        /// Returns the normalized spread multiplier (scaled from 0 to 1).
        /// </summary>
        public float Spread()
        {
            return m_spread / 100;
        }

        /// <summary>
        /// Returns the normalized fire rate multiplier (scaled from 0 to 1).
        /// </summary>
        public float FireRate()
        {
            return m_fireRate / 100;
        }

        /// <summary>
        /// Returns the normalized range multiplier (scaled from 0 to 1).
        /// </summary>
        public float Range()
        {
            return m_range / 100;
        }

        /// <summary>
        /// Returns the normalized muzzle velocity multiplier (scaled from 0 to 1).
        /// </summary>
        public float MuzzleVelocity()
        {
            return m_muzzleVelocity / 100;
        }

        /// <summary>
        /// Returns the normalized recoil multiplier (scaled from 0 to 1).
        /// </summary>
        public float Recoil()
        {
            return m_recoil / 100;
        }

        /// <summary>
        /// Returns the normalized visual recoil multiplier (scaled from 0 to 1).
        /// </summary>
        public float VisualRecoil()
        {
            return m_visualRecoil / 100;
        }

        /// <summary>
        /// Returns the normalized aim down sight speed multiplier (scaled from 0 to 1).
        /// </summary>
        public float AimSpeed()
        {
            return m_aimSpeed / 100;
        }

        /// <summary>
        /// Called when the script instance is being loaded.
        /// Initializes the attachment manager by locating the parent FirearmAttachmentsManager.
        /// </summary>
        private void Awake()
        {
            attachmentsManager = GetComponentInParent<FirearmAttachmentsManager>();
        }

        private void Start()
        {
            children = transform.GetAllChildren();
        }

        private List<Transform> children;

        /// <summary>
        /// Updates the state of each child object to match whether the attachment is active or not.
        /// </summary>
        private void Update()
        {
            foreach (Transform child in children)
                child.gameObject.SetActive(IsActive()); // Activate or deactivate child objects based on the attachment's active status
        }

        /// <summary>
        /// Determines if the current attachment is active by comparing its type and name 
        /// against the active attachments in the attachments manager.
        /// </summary>
        /// <returns>True if the attachment is active; otherwise, false.</returns>
        public bool IsActive()
        {
            if(attachmentsManager == null)
            {
                Debug.LogError("Couldn't find a FirearmAttachmentManager, please make sure to have on on the parent object.", gameObject);

                return false;
            }

            // Iterate through all active attachments in the FirearmAttachmentsManager
            foreach (FirearmAttachmentsManager.AttachmentData attachment in attachmentsManager.activeAttachments)
            {
                // Check if the attachment type matches
                if (attachment.type == type)
                {
                    // Check if the attachment name matches
                    if (attachment.name == Name)
                    {
                        // The type and name both match, so this attachment is active
                        return true;
                    }
                }
            }

            // No matching active attachment found
            return false;
        }
    }
}
