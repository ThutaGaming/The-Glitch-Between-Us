using UnityEngine;
using UnityEditor;

namespace Akila.FPSFramework
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SeparatorAttribute))]
    public class SeparatorDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            SeparatorAttribute separator = (SeparatorAttribute)attribute;

            // Add some space before the separator
            position.y += 10;
            position.height = 1;

            // Draw the separator
            EditorGUI.DrawRect(position, Color.gray);

            // Add some space after the separator
            position.y += 10;
        }

        public override float GetHeight()
        {
            return 20;
        }
    }
#endif
}