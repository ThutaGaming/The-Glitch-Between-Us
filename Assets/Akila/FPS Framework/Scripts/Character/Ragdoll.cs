using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    /// <summary>
    /// Controls the transition between animated and ragdoll states for a player character.
    /// Supports both standard ragdolling and a hybrid mode that preserves external forces.
    /// </summary>
    [AddComponentMenu("Akila/FPS Framework/Player/Ragdoll")]
    public class Ragdoll : MonoBehaviour
    {
        public Animator animator;
        public bool isEnabled;

        protected Rigidbody[] rigidbodies;

        protected virtual void Start()
        {
            if (animator == null)
                animator = transform.SearchFor<Animator>();
            
            rigidbodies = GetComponentsInChildren<Rigidbody>();

            if (isEnabled)
                Enable();
            else
                Disable();
        }

        protected virtual void Update()
        {
            foreach(Rigidbody rb in rigidbodies) rb.isKinematic = !isEnabled;
        }

        public virtual void Enable()
        {
            isEnabled = true;
            animator.enabled = false;
        }

        public virtual void Disable()
        { 
            isEnabled = false;
            animator.enabled = true;
        }
    }
}