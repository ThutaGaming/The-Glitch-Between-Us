using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework.Animation
{
    [AddComponentMenu("Akila/FPS Framework/Animation/Modifiers/Offset Animation Modifier")]
    public class OffsetAnimationModifier : ProceduralAnimationModifier
    {
        [Tooltip("final position result for this modifier")]
        public Vector3 positonOffset;
        [Tooltip("final rotation result for this modifier")]
        public Vector3 rotationOffset;

        private void Update()
        {
            targetPosition = positonOffset;
            targetRotation = rotationOffset;
        }
    }
}