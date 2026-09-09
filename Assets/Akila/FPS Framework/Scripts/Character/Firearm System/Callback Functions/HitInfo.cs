using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    /// <summary>
    /// This class will have almost all of the data yout need to interact with the hits from projectile or explosion
    /// </summary>
    public class HitInfo
    {
        public Vector3 hitOrigin { get; set; }
        public Vector3 hitDirection { get; set; }
        public RaycastHit raycastHit { get; set; }
        public GameObject sourcePlayer { get; set; }
        public bool isCriticalHit
        {
            get
            {
                if (raycastHit.transform)
                {
                    if (raycastHit.transform.TryGetComponent<IDamageablePart>(out IDamageablePart damageableParent))
                    {
                        return damageableParent.isCriticalPart;
                    }
                }

                return false;
            }
        }

        public HitInfo(GameObject sourcePlayer, RaycastHit hit, Vector3 origin, Vector3 direction)
        {
            this.sourcePlayer = sourcePlayer;

            raycastHit = hit;
            hitOrigin = origin;
            hitDirection = direction;
        }
    }
}