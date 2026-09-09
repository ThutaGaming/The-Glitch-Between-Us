using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    [System.Obsolete, ExecuteAlways, AddComponentMenu("")]
    public class IgnoreFallDamage : MonoBehaviour
    {
        private void Start()
        {
            if(GetComponent<Ignore>() == null)
            {
                Ignore new_ignore = gameObject.AddComponent<Ignore>();

                new_ignore.ignoreFallDamage = true;
            }
            else
            {
                GetComponent<Ignore>().ignoreHitDetection = true;
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(gameObject);
            UnityEditor.AssetDatabase.SaveAssets();
#endif

            DestroyImmediate(this);
        }
    }
}