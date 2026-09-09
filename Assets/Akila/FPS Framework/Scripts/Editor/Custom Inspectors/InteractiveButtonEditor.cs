using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Akila.FPSFramework.Internal
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(InteractiveButton))]
    public class InteractiveButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();


            if(EditorGUI.EndChangeCheck())
            {
                UpdateColors();
            }
        }

        private void OnEnable()
        {
            UpdateColors();
        }

        private void UpdateColors()
        {
            InteractiveButton button = (InteractiveButton)target;

            if (button.targetText)
            {
                button.targetText.color = button.normalTextColor;
            }

            if (button.targetGraphics)
            {
                button.targetGraphics.color = button.normalGraphicsColor;
            }
        }
    }
#endif
}