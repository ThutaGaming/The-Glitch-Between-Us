using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Health System/Damageable Part")]
    public class DamageablePart : MonoBehaviour, IDamageablePart
    {
        [SerializeField] HumanBodyBones m_bone;
        [SerializeField] float m_damageMultipler = 1;
        [SerializeField] bool m_isCritical;

        private IDamageable m_damageable;

        public bool isCriticalPart
        {
            get
            {
                return m_isCritical;
            }
        }

        public IDamageable parentDamageable
        {
            get
            {
                return m_damageable;
            }
        }
        public HumanBodyBones bone
        {
            get
            {
                return m_bone;
            }
        }

        public float damageMultipler
        {
            get
            {
                return m_damageMultipler;
            }
        }

        private void Start()
        {
            m_damageable = GetComponentInParent<IDamageable>();
        }

        [System.Obsolete("Use parentDamageable instead.")]
        public IDamageable GetDamageable()
        {
            return m_damageable;
        }

        [System.Obsolete("Use bone instead.")]
        public HumanBodyBones GetBone()
        {
            return m_bone;
        }

        [System.Obsolete("Use parentDamageable instead.")]
        public float GetDamageMultipler()
        {
            return m_damageMultipler;
        }
    }
}