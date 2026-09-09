#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Akila.FPSFramework;

#if FPSFRAMEWORK_PRO

#else

namespace Akila.FPSFramework.Internal
{
    [CustomEditor(typeof(Pickable))]
    public class PickableEditor : Editor
    {

        private SerializedProperty nameProp;
        private SerializedProperty interactionNameProp;
        private SerializedProperty typeProp;
        private SerializedProperty interactSoundProp;
        private SerializedProperty itemProp;
        private SerializedProperty includeCollectableProp;
        private SerializedProperty collectableIdentifierProp;
        private SerializedProperty collectableCountProp;

        private void OnEnable()
        {
            nameProp = serializedObject.FindProperty("Name");
            interactionNameProp = serializedObject.FindProperty("interactionName");
            typeProp = serializedObject.FindProperty("type");
            interactSoundProp = serializedObject.FindProperty("interactSound");
            itemProp = serializedObject.FindProperty("itemToPickup");
            includeCollectableProp = serializedObject.FindProperty("includeCollectable");
            collectableIdentifierProp = serializedObject.FindProperty("collectableIdentifier");
            collectableCountProp = serializedObject.FindProperty("collectableCount");
        }

        public override void OnInspectorGUI()
        {
            Pickable pickable = (Pickable)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(nameProp, new GUIContent("Name", "The display name of the item, used for interaction prompts and UI."));
            EditorGUILayout.PropertyField(interactionNameProp, new GUIContent("Hint Text", "Display name used when showing the interaction prompt."));
            EditorGUILayout.PropertyField(typeProp, new GUIContent("Type", "The type of this pickable (Item or Collectable)."));

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(interactSoundProp, new GUIContent("Interaction Sound", "Optional sound clip played when this item is interacted with."));

            EditorGUILayout.Space();

            PickableType type = (PickableType)typeProp.enumValueIndex;

            if (type == PickableType.Item)
            {
                EditorGUILayout.LabelField("Item Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(itemProp, new GUIContent("Item To Pickup", "The inventory item prefab to be added when this is picked up (used if type is 'Item')."));
                EditorGUILayout.PropertyField(includeCollectableProp, new GUIContent("Include Collectable", "If enabled, pickable will act as both, collectable and item.."));
            }

            if (pickable.type == PickableType.Item)
            {
                EditorGUI.indentLevel++;

                EditorGUI.BeginDisabledGroup(!pickable.includeCollectable);

                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField("Collectable Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(collectableIdentifierProp, new GUIContent("Collectable Identifier", "Unique identifier for this collectable (used if type is 'Collectable')."));
            EditorGUILayout.PropertyField(collectableCountProp, new GUIContent("Amount To Collect", "The amount of collectables granted when picked up (used if type is 'Collectable')."));
            

            if (pickable.type == PickableType.Item)
            {
                EditorGUI.indentLevel--;
            
                EditorGUI.EndDisabledGroup();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
#endif