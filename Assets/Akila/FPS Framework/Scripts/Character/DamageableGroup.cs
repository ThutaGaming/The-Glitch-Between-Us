using System;
using System.Reflection;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu(""), ExecuteAlways, System.Obsolete]
    public class DamageableGroup : MonoBehaviour
    {
        public HumanBodyBones bone;
        public float damageMultipler = 1;

        private void Start()
        {
            if (GetComponent<DamageablePart>() == null)
            {
                DamageablePart new_part = gameObject.AddComponent<DamageablePart>();

                // Get the type of the object
                Type type = new_part.GetType();

                // Get the FieldInfo for 'myField'
                FieldInfo m_bone = type.GetField("m_bone", BindingFlags.Instance | BindingFlags.NonPublic);
                FieldInfo m_damageMultipler = type.GetField("m_damageMultipler", BindingFlags.Instance | BindingFlags.NonPublic);

                // Set the value
                m_bone.SetValue(new_part, bone);
                m_damageMultipler.SetValue(new_part, damageMultipler);
            }
            else
            {
                GetComponent<DamageablePart>();
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(gameObject);
            UnityEditor.AssetDatabase.SaveAssets();
#endif

            DestroyImmediate(this);

            Debug.Log("IDamageableGroup interface and DamageableGroup have been replaced by IDamageablePart interface and DamageablePart class. Automatic upgrade has been done.", gameObject);
        }

        [System.Obsolete("Use IDamageablePart.parentDamageable instead.")]
        public IDamageable GetDamageable()
        {
            return null;
        }

        [System.Obsolete("Use IDamageablePart.bone instead.")]
        public HumanBodyBones GetBone()
        {
            return bone;
        }

        [System.Obsolete("Use IDamageablePart.parentDamageable instead.")]
        public float GetDamageMultipler()
        {
            return damageMultipler;
        }
    }
}