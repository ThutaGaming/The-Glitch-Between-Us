using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Akila.FPSFramework
{
    public interface IDamageable
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }

        public bool isDamagableDisabled { get; set; }
        public bool allowDamageableEffects { get; set; }

        public float Health { get; set; }

        public void Damage(float amount, GameObject damageSource);

        public bool DeadConfirmed { get; set; }

        public GameObject DamageSource { get; set; }
        public UnityEvent OnDeath { get; }
    }
}