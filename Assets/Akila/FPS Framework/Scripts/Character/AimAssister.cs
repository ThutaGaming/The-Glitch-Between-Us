using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Akila.FPSFramework
{
    /// <summary>
    /// Provides aim assist by adjusting the crosshair to target enemies within a defined radius.
    /// </summary>
    public class AimAssister : MonoBehaviour
    {
        [Header("Aim Assist Settings")]
        /// <summary>Maximum distance for aim assist to activate.</summary>
        public float maxDistance = 50f;

        /// <summary>Radius around the crosshair for aim assist.</summary>
        public float assistRadius = 50f;

        /// <summary>Dead zone radius where aim assist is disabled.</summary>
        public float deadZone = 5f;

        /// <summary>Intensity of aim assist adjustment.</summary>
        public float assistStrength = 50f;

        /// <summary>Smoothing factor for aim adjustments to prevent jitter.</summary>
        public float smoothing = 15f;

        [Header("Input Triggers")]
        [Tooltip("Enable aim assist under all conditions.")]
        public bool alwaysOn = false;

        [Tooltip("Enable aim assist when moving.")]
        public bool assistWhenMoving = true;

        [Tooltip("Enable aim assist during aim initiation.")]
        public bool assistOnStartAim = true;

        [Tooltip("Enable aim assist while aiming.")]
        public bool assistWhileAiming = true;

        [Tooltip("Enable aim assist for keyboard and mouse.")]
        public bool assistForKeyboardMouse = false;

        [Tooltip("Enable aim assist for gamepad controllers.")]
        public bool assistForGamepad = true;

        [Tooltip("Enable aim assist for mobile devices.")]
        public bool assistForMobile = false;

        /// <summary>List of detected aim assist targets.</summary>
        public List<AimAssistTarget> Targets { get; private set; }

        private CharacterInput input;
        private CharacterManager manager;
        private Vector2 smoothedAdjustment; // Stores the smoothed aim adjustment

        private void Start()
        {
            input = GetComponent<CharacterInput>();
            manager = GetComponent<CharacterManager>();
        }

        private void Update()
        {
            if (alwaysOn)
            {
                AdjustAim();
                return;
            }

            if (assistWhenMoving && !manager.IsAlmostStopped())
            {
                AdjustAim();
                return;
            }

            if (assistOnStartAim && manager.attemptingToAim)
            {
                AdjustAim();
                return;
            }

            if (assistWhileAiming && manager.isAiming)
            {
                AdjustAim();
            }
        }

        /// <summary>
        /// Adjusts aim towards the closest valid target within the assist radius.
        /// </summary>
        private void AdjustAim()
        {
            if (!enabled) return;

            var controlScheme = FPSFrameworkCore.GetActiveControlScheme();

            if (!assistForKeyboardMouse && (controlScheme == ControlScheme.Mouse || controlScheme == ControlScheme.Keyboard))
                return;

            if (!assistForGamepad && controlScheme == ControlScheme.Gamepad)
                return;

            if (!assistForMobile && controlScheme == ControlScheme.TouchScreen)
                return;

            Targets = FindObjectsByType<AimAssistTarget>(FindObjectsSortMode.None).ToList();

            var closest = FindClosestTarget(Targets);

            if (closest != null)
            {
                var adjustment = CalculateAdjustment(closest.transform);

                // Smooth the adjustment for better user experience
                smoothedAdjustment = Vector2.Lerp(smoothedAdjustment, adjustment, Time.deltaTime * smoothing);

                // Apply adjustment if target is within assist radius
                if (Vector2.Distance(GetScreenCenter(), Camera.main.WorldToScreenPoint(closest.transform.position)) <= assistRadius)
                {
                    input.AddLookValue(smoothedAdjustment);
                }
            }
        }

        /// <summary>
        /// Calculates aim adjustment based on target position.
        /// </summary>
        /// <param name="target">The target's transform.</param>
        /// <returns>Adjustment vector for aim.</returns>
        private Vector2 CalculateAdjustment(Transform target)
        {
            var offset = GetScreenOffset(target);

            // Ignore adjustment if within dead zone
            if (offset.magnitude < deadZone)
                return Vector2.zero;

            // Normalize offset and scale by assist strength and distance
            return offset.normalized * Mathf.Lerp(assistStrength, 0, Vector3.Distance(transform.position, target.position) / maxDistance) * Time.deltaTime;
        }

        /// <summary>
        /// Finds the closest valid target within the assist radius.
        /// </summary>
        /// <param name="targets">List of potential targets.</param>
        /// <returns>The closest valid target or null if none found.</returns>
        private AimAssistTarget FindClosestTarget(List<AimAssistTarget> targets)
        {
            AimAssistTarget closest = null;
            float closestDistance = float.MaxValue;

            foreach (var target in targets)
            {
                if (target == null || (target.healthSystem != null && target.healthSystem.health <= 0))
                    continue;

                var offset = GetScreenOffset(target.transform);

                if (offset.magnitude > assistRadius || !IsOnScreen(target.transform.position))
                    continue;

                float distance = Vector3.Distance(transform.position, target.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = target;
                }
            }

            return closest;
        }

        /// <summary>
        /// Gets the offset of a target from the screen center.
        /// </summary>
        /// <param name="target">The target's transform.</param>
        /// <returns>Offset vector in screen space.</returns>
        private Vector2 GetScreenOffset(Transform target)
        {
            var screenCenter = GetScreenCenter();
            var targetPos = Camera.main.WorldToScreenPoint(target.position);
            return new Vector2(targetPos.x - screenCenter.x, targetPos.y - screenCenter.y);
        }

        /// <summary>
        /// Returns the center of the screen in screen coordinates.
        /// </summary>
        /// <returns>Screen center position.</returns>
        private Vector3 GetScreenCenter()
        {
            return new Vector3(Screen.width / 2, Screen.height / 2, 0);
        }

        /// <summary>
        /// Checks if a world position is visible on screen.
        /// </summary>
        /// <param name="position">World position to check.</param>
        /// <returns>True if visible, otherwise false.</returns>
        private bool IsOnScreen(Vector3 position)
        {
            var screenPos = Camera.main.WorldToScreenPoint(position);
            return screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height;
        }
    }
}
