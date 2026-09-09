using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    [System.Obsolete("Use IDamageablePart instead.")]
    public interface IDamageableGroup
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }

        public IDamageable GetDamageable();
        public HumanBodyBones GetBone();
        public float GetDamageMultipler();
    }
}