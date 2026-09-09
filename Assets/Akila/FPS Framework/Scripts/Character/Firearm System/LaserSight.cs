using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Akila.FPSFramework
{
    /// <summary>
    /// A component for managing a laser sight attachment in an FPS game.
    /// Toggles the laser on/off and manages its behavior, including raycasting
    /// to determine hit points and drawing the laser beam.
    /// </summary>
    [AddComponentMenu("Akila/FPS Framework/Weapons/Attachments/Laser Sight")]
    public class LaserSight : MonoBehaviour
    {
        /// <summary>
        /// Input action to toggle the laser.
        /// </summary>
        [Tooltip("Input action to toggle the laser on/off.")]
        public InputAction toggleLaserInputAction = new InputAction("ToggleLaser", InputActionType.Button, "<Keyboard>/l");

        /// <summary>
        /// Layers the laser can interact with. Defaults to 'Everything'.
        /// </summary>
        [Tooltip("Layers the laser can hit."), Space]
        public LayerMask interactableLayers = ~0; // Defaults to "Everything".

        /// <summary>
        /// Transform representing the laser's source.
        /// </summary>
        [Tooltip("Transform representing the source of the laser.")]
        public Transform laserSource;

        /// <summary>
        /// Transform representing the laser dot.
        /// </summary>
        [Tooltip("Transform representing the laser dot at the hit point.")]
        public Transform laserDot;

        /// <summary>
        /// Transform used to define the laser's maximum range.
        /// </summary>
        [Tooltip("Transform to define the maximum range of the laser.")]
        public Transform laserRange;

        /// <summary>
        /// LineRenderer used to draw the laser beam.
        /// </summary>
        [Tooltip("LineRenderer used to draw the laser beam.")]
        public LineRenderer laserLine;

        /// <summary>
        /// Indicates if the laser is currently active.
        /// </summary>
        [Tooltip("Whether the laser is currently active.")]
        public bool isLaserOn = true;

        public bool automaticToggling = true;

        private Vector3 hitNormal; // Stores the hit surface's normal vector.
        private Vector3 hitPoint;  // Stores the point where the laser hits.
        private RaycastHit hitInfo; // Stores raycast hit data.
        private Firearm firearm; //Stores the parent firearm, if it exists.

        private bool currentState;

        private bool isStopped;

        private void Awake()
        {
            // Validate required components and log warnings/errors
            if (laserSource == null)
            {
                Debug.LogError("[LaserSight] Laser Source is not assigned. Please assign it in the Inspector.", this);
            }

            if (laserDot == null)
            {
                Debug.LogWarning("[LaserSight] Laser Dot is not assigned. The laser dot will not be displayed.", this);
            }

            if (laserRange == null)
            {
                Debug.LogWarning("[LaserSight] Laser Range is not assigned. Using default range behavior.", this);
            }

            if (laserLine == null)
            {
                Debug.LogError("[LaserSight] LineRenderer is not assigned. The laser beam will not be rendered.", this);
            }
            else
            {
                laserLine.useWorldSpace = true;
            }

            // Initialize input action
            toggleLaserInputAction.Enable();
            toggleLaserInputAction.performed += context => isLaserOn = !isLaserOn;

            firearm = GetComponentInParent<Firearm>();
        }

        public void Stop()
        {
            isStopped = true;
        }

        private void LateUpdate()
        {
            if (isStopped)
            {
                laserDot?.gameObject.SetActive(false);
                laserLine?.gameObject.SetActive(false);

                return;
            }

            if (laserLine)
            {
                // Check if the laser should be active
                if(firearm == null)
                {
                    laserLine.enabled = isLaserOn;
                }
                else
                {
                    if (isLaserOn)
                    {
                        currentState = true;

                        laserLine.enabled = currentState;
                    }
                    else
                        laserLine.enabled = false;
                }
                
                // Update the start position of the laser beam
                laserLine.SetPosition(0, laserSource.position);
            }

            // Perform raycast to determine hit point
            if (Physics.Raycast(laserSource.position, transform.forward, out hitInfo, Mathf.Infinity, interactableLayers))
            {
                if (hitInfo.transform.TryGetComponent(out Ignore _ignore) && !_ignore.ignoreLaserDetection)
                {
                    // Laser hits a valid target
                    EnableLaser(hitInfo);
                }
                else
                {
                    // Laser hits an ignored object
                    DisableLaser();
                }
            }
            else
            {
                // Laser does not hit anything
                DisableLaser();
            }
        }

        /// <summary>
        /// Enables the laser effects at the specified hit point.
        /// </summary>
        /// <param name="hit">Raycast hit data.</param>
        private void EnableLaser(RaycastHit hit)
        {

            if(isLaserOn == false)
            {
                laserLine.enabled = false;
                laserDot.gameObject.SetActive(false);

                return;
            }

            if (laserLine) laserLine.SetPosition(1, hit.point);

            hitNormal = hit.normal;
            hitPoint = hit.point;

            // Show and position the laser dot
            if (laserDot)
            {
                laserDot.gameObject.SetActive(isLaserOn);
                laserDot.position = hitPoint;
                laserDot.rotation = Quaternion.FromToRotation(Vector3.up, hitNormal);
            }

            if(currentState == false)
            {
                laserDot.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Disables the laser effects.
        /// </summary>
        private void DisableLaser()
        {
            if (laserDot) laserDot.gameObject.SetActive(false);
            if (laserLine && laserRange) laserLine.SetPosition(1, laserRange.position);
        }

        private void OnDrawGizmos()
        {
            // Draw debug gizmos to visualize the hit point
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hitPoint, 0.4f);
        }

        private void OnDestroy()
        {
            // Unsubscribe from input action to avoid memory leaks
            toggleLaserInputAction.performed -= context => isLaserOn = !isLaserOn;
        }
    }
}
