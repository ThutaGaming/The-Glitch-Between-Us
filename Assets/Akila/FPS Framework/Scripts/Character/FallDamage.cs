using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Akila.FPSFramework.Experimental
{
    [AddComponentMenu("Akila/FPS Framework/Player/Fall Damage")]
    [RequireComponent(typeof(CharacterManager), typeof(Damageable))]
    public class FallDamage : MonoBehaviour
    {
        private CharacterManager characterManager;
        private IDamageable damageable;

        public float minFallingSpeed = 35;
        public float damageValue = 10;
        public float damageMultiplier = 1f;
        public UnityEvent<float> onDamaged;

        public bool isDamageActive { get; set; } = true;


        private void Start()
        {
            characterManager = GetComponent<CharacterManager>();
            damageable = GetComponent<IDamageable>();

            characterManager.onLand.AddListener(ApplyFallDamage);
        }

        private List<float> velocityRecord = new List<float>();

        private void Update()
        {
            if(characterManager.isGrounded == false)
            {
                //Record every velocity to avoid inconsitancy
                velocityRecord.Add(Mathf.Abs(characterManager.velocity.y));
            }
        }

        private void ApplyFallDamage()
        {
            if (velocityRecord.Count == 0) return;

            // Convert max falling speed (m/s) to km/h
            float maxSpeedKmh = velocityRecord.Max() * 2.6f;

            // Clear the record next frame
            Invoke(nameof(Clear), Time.fixedDeltaTime);

            if (maxSpeedKmh < minFallingSpeed) return;

            float normalized = maxSpeedKmh / minFallingSpeed;
            float damage = damageValue * normalized * normalized * damageMultiplier;

            onDamaged?.Invoke(damage);

            if (isDamageActive) damageable.Damage(damage, gameObject);
        }

        private void Clear() => velocityRecord.Clear();
    }
}