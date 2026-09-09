using UnityEngine;

namespace Akila.FPSFramework
{
    /// <summary>
    /// A camera that activates when the player dies. 
    /// It positions itself near the player's last position and looks toward the killer.
    /// </summary>
    [AddComponentMenu("Akila/FPS Framework/Player/Death Camera")]
    [RequireComponent(typeof(Camera), typeof(AudioListener))]
    public class DeathCamera : MonoBehaviour
    {
        /// <summary>
        /// The globally active instance of the DeathCamera.
        /// </summary>
        public static DeathCamera Instance;

        [Header("Position Settings")]
        [Tooltip("Maximum radius around the player used to position the camera.")]
        public float lookRadius = 5;

        [Tooltip("Additional position offset applied after placement.")]
        public Vector3 positionOffset;

        /// <summary>
        /// The camera component used by this system.
        /// </summary>
        public Camera Camera { get; private set; }

        /// <summary>
        /// The audio listener used when this camera is active.
        /// </summary>
        public AudioListener AudioListener { get; private set; }

        private void Awake()
        {
            // Ensure a single instance exists
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple DeathCamera instances detected. Destroying duplicate.", this);
                Destroy(gameObject);
                return;
            }

            Instance = this;

            Camera = GetComponent<Camera>();
            AudioListener = GetComponent<AudioListener>();

            if (Camera == null)
                Debug.LogError("Camera component missing on DeathCamera.", this);

            if (AudioListener == null)
                Debug.LogError("AudioListener component missing on DeathCamera.", this);

            Disable();
        }

        /// <summary>
        /// Enables the death camera and positions it near the player while looking at the killer.
        /// </summary>
        /// <param name="player">The player object that died. Used as the anchor position.</param>
        /// <param name="killerPosition">The position to orient the camera toward.</param>
        public void Enable(GameObject player, Vector3 killerPosition)
        {
            if (player == null)
            {
                Debug.LogError("DeathCamera.Enable called with a null player.", this);
                return;
            }

            if (Camera == null || AudioListener == null)
            {
                Debug.LogError("DeathCamera.Enable called but required components are missing.", this);
                return;
            }

            // Random spherical offset on horizontal plane
            Vector3 randomOffset = Random.insideUnitSphere * lookRadius;
            randomOffset.y = 0;

            transform.position = player.transform.position + randomOffset + positionOffset;

            // Look at killer or fallback to player if killer position is invalid
            if (killerPosition != Vector3.zero)
                transform.LookAt(killerPosition);
            else
                transform.LookAt(player.transform.position);

            Camera.enabled = true;
            AudioListener.enabled = true;
        }

        /// <summary>
        /// Disables both the camera view and audio input.
        /// </summary>
        public void Disable()
        {
            if (Camera != null)
                Camera.enabled = false;

            if (AudioListener != null)
                AudioListener.enabled = false;
        }
    }
}
