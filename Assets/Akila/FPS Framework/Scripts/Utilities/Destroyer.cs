using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    /// <summary>
    /// Component that destroys the GameObject it's attached to after a specified delay.
    /// </summary>
    [AddComponentMenu("Akila/FPS Framework/Utility/Destroyer")]
    public class Destroyer : MonoBehaviour
    {
        /// <summary>
        /// If true, the GameObject will be destroyed on Awake.
        /// </summary>
        public bool destroyOnAwake = true;
        public bool destroyOnEnabled = false;

        /// <summary>
        /// The delay in seconds before the GameObject is destroyed.
        /// </summary>
        public float destroyDelay = 1;

        private void Awake()
        {
            if (destroyOnAwake) Destroy(gameObject, destroyDelay);
        }

        private void OnEnable()
        {
            if(destroyOnEnabled) Destroy(gameObject, destroyDelay);
        }

        public void DestroySelf(float delay)
        {
            Destroy(gameObject, delay);
        }
    }
}
