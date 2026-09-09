using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    public interface IDamageablePart
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }

        public IDamageable parentDamageable { get; }
        public HumanBodyBones bone { get; }

        public float damageMultipler { get; }
        public bool isCriticalPart { get; }
    }
}