#if UNITY_EDITOR
using UnityEditor;

namespace Akila.FPSFramework.Internal
{
    [CustomEditor(typeof(Firearm))]
    public class FirearmEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            Firearm firearm = (Firearm)target;
            if (!firearm.GetComponent<ItemInput>())
            {
                EditorGUILayout.HelpBox("Firearm must have Firearm Input Manager in order to get input.", MessageType.Info);
            }
        }
    }
}
#endif