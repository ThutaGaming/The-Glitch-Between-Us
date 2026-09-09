
using UnityEngine;

namespace Akila.FPSFramework.Animation
{
    public class WallAvoidanceAnimationModifier : ProceduralAnimationModifier
    {
        public float range = 1;
        public LayerMask layerMask = ~0;

        public Vector3 position;
        public Vector3 rotation;

        RaycastHit hit;

        private Vector3 posVel;
        private Vector3 rotVel;

        private Vector3 pos;
        private Vector3 rot;

        private float clippingFactor;

        private void Update()
        {
            if(Camera.main == null) return;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range, layerMask))
            {
                targetAnimation.progress = hit.distance / range;
                targetAnimation.IsPlaying = true;

                pos = Vector3.Lerp(position, Vector3.zero, hit.distance / range);
                rot = Vector3.Lerp(rotation, Vector3.zero, hit.distance / range);
            }
            else
            {
                targetAnimation.progress = 0;
                targetAnimation.IsPlaying = false;

                pos = Vector3.zero;
                rot = Vector3.zero;
            }
            targetPosition = Vector3.SmoothDamp(targetPosition, pos, ref posVel, targetAnimation.length / globalSpeed);
            targetRotation = Vector3.SmoothDamp(targetRotation, rot, ref rotVel, targetAnimation.length / globalSpeed); 
        }
    }
}