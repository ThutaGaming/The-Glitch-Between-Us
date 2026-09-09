using UnityEngine;

namespace Akila.FPSFramework
{
    /// <summary>
    /// Calculates both linear and angular velocity of a transform.
    /// Can be used to compute world velocity at any point (e.g. for a player on a rotating platform).
    /// </summary>
    public class Speedometer : MonoBehaviour
    {
        public UpdateMode updateMode = UpdateMode.LateUpdate;

        [ReadOnly] public float speedMagnitude;
        [HideInInspector] public float speedKmPerHour;
        public Vector3 velocity { get; private set; }
        public Vector3 angularVelocity { get; private set; } // in radians/sec

        private Vector3 prevPosition;
        private Quaternion prevRotation;

        private void Start()
        {
            prevPosition = transform.position;
            prevRotation = transform.rotation;
        }

        private void Update()
        {
            if (updateMode == UpdateMode.Update)
                Calculate(Time.unscaledDeltaTime);
        }

        private void FixedUpdate()
        {
            if (updateMode == UpdateMode.FixedUpdate)
                Calculate(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            if (updateMode == UpdateMode.LateUpdate)
                Calculate(Time.unscaledDeltaTime);
        }

        private void Calculate(float deltaTime)
        {
            var currentPos = transform.position;
            var currentRot = transform.rotation;

            // Linear velocity
            velocity = (currentPos - prevPosition) / deltaTime;

            // Angular velocity
            Quaternion deltaRot = currentRot * Quaternion.Inverse(prevRotation);
            deltaRot.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;
            angularVelocity = axis * (angle * Mathf.Deg2Rad / deltaTime);

            // Magnitude and km/h
            speedMagnitude = velocity.magnitude;
            speedKmPerHour = speedMagnitude * 3.6f;

            // Save state
            prevPosition = currentPos;
            prevRotation = currentRot;
        }

        /// <summary>
        /// Gets total velocity at a given world-space point (including rotation).
        /// </summary>
        public Vector3 GetPointVelocity(Vector3 worldPoint)
        {
            Vector3 r = worldPoint - transform.position;
            return velocity + Vector3.Cross(angularVelocity, r);
        }

        /// <summary>
        /// Predicts position after a given time (linear only).
        /// </summary>
        public Vector3 PredictPosition(float time = -1, bool includePosition = true)
        {
            float t = time != -1 ? time : Time.unscaledDeltaTime;
            return (includePosition ? transform.position : Vector3.zero) + velocity * t;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, PredictPosition(0.5f));
        }
    }
}
