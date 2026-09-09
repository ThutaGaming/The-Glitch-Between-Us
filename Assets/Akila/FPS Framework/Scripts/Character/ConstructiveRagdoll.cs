using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    public class ConstructiveRagdoll : Ragdoll
    {
        protected Transform hips;
        protected Transform virtualHips;

        protected override void Start()
        {
            Rigidbody hipsRb = GetComponentInChildren<Rigidbody>();

            if (hipsRb != null) hips = hipsRb.transform;

            if(hips != null)
            {
                virtualHips = Instantiate(hips, hips.parent);
                
                virtualHips.hideFlags = HideFlags.HideInHierarchy;
                
                virtualHips.name = hips.name;

                foreach(Collider collider in virtualHips.GetComponentsInChildren<Collider>())
                    Destroy(collider);
            }

            foreach(Rigidbody rb in virtualHips.GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = true;
            }
            
            base.Start();
        }

        protected override void Update()
        {
            
        }

        public override void Enable()
        {
            if (animator != null)
                animator.enabled = false;

            if(hips == null)
            {
                Debug.LogError("Couldn't find Rigidbody on Hips", gameObject);

                return;
            }

            // Build map between virtual and real transforms
            Dictionary<Transform, Transform> map = new Dictionary<Transform, Transform>();

            foreach (Transform v in virtualHips.GetComponentsInChildren<Transform>())
            {
                Transform r = hips.FindDeepChild(v.name);
                if (r != null)
                    map[v] = r;
            }

            // Make sure hips has a Rigidbody
            Rigidbody hipsRb = hips.GetComponent<Rigidbody>();

            if (hipsRb == null)
            {
                hipsRb = hips.gameObject.AddComponent<Rigidbody>();
                hipsRb.mass = 1f;
                hipsRb.isKinematic = false;
            }

            // Copy rigidbodies
            foreach (Rigidbody vrb in virtualHips.GetComponentsInChildren<Rigidbody>())
            {
                if (!map.TryGetValue(vrb.transform, out Transform realRb))
                    continue;

                if (realRb == hips)
                    continue;

                Rigidbody newRb = realRb.gameObject.CopyComponent<Rigidbody>(vrb);
                
                newRb.isKinematic = false;
                newRb.automaticCenterOfMass = true;
                newRb.automaticInertiaTensor = true;
            }

            // Copy joints
            foreach (CharacterJoint vJoint in virtualHips.GetComponentsInChildren<CharacterJoint>())
            {
                if (!map.TryGetValue(vJoint.transform, out Transform realJoint))
                    continue;

                if (realJoint == hips)
                    continue;

                CharacterJoint newJoint = realJoint.gameObject.CopyComponent<CharacterJoint>(vJoint);

                if (vJoint.connectedBody != null && map.TryGetValue(vJoint.connectedBody.transform, out Transform realConnected))
                {
                    newJoint.connectedBody = realConnected.GetComponent<Rigidbody>();
                }
                else
                {
                    newJoint.connectedBody = hipsRb;
                }

                // Copy joint properties that affect motion realism
                newJoint.axis = vJoint.axis;
                newJoint.swingAxis = vJoint.swingAxis;

                newJoint.lowTwistLimit = vJoint.lowTwistLimit;
                newJoint.highTwistLimit = vJoint.highTwistLimit;
                newJoint.swing1Limit = vJoint.swing1Limit;
                newJoint.swing2Limit = vJoint.swing2Limit;

                newJoint.anchor = vJoint.anchor;
                newJoint.connectedAnchor = vJoint.connectedAnchor;

                newJoint.enablePreprocessing = true;
                newJoint.enableCollision = true;
            }

            base.rigidbodies = GetComponentsInChildren<Rigidbody>();
        }

        public override void Disable()
        {
            animator.enabled = true;

            CharacterJoint[] joins = hips.GetComponentsInChildren<CharacterJoint>();
            Rigidbody[] rigidbodies = hips.GetComponentsInChildren<Rigidbody>();

            foreach(CharacterJoint joint in joins) 
                Destroy(joint);

            foreach(Rigidbody rb in rigidbodies)
                Destroy(rb);
        }
    }
}