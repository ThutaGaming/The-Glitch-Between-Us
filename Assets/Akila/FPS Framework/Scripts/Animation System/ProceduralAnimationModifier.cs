using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework.Animation
{
    [RequireComponent(typeof(ProceduralAnimation)), AddComponentMenu("")]
    public class ProceduralAnimationModifier : MonoBehaviour
    {
        /// <summary>
        /// The target animation that this modifer is modifying
        /// </summary>
        public ProceduralAnimation targetAnimation { get; set; }

        /// <summary>
        /// final position result for this modifier
        /// </summary>
        public Vector3 targetPosition { get; set; }

        /// <summary>
        /// final rotation result for this modifier
        /// </summary>
        public Vector3 targetRotation { get; set; }

        public float globalSpeed
        {
            get
            {
                return FPSFrameworkSettings.globalAnimationSpeed;
            }
        }

        public int maxFramerate
        {
            get
            {
                return FPSFrameworkSettings.maxAnimationFramerate;
            }
        }
    }
}