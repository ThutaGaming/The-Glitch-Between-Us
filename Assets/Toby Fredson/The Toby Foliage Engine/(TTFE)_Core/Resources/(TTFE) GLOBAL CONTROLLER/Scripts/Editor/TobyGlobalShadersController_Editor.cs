using UnityEngine;
using UnityEditor;

namespace TobyFredson
{
	[CustomEditor(typeof(TobyGlobalShadersController))]
	public class TobyGlobalShadersController_Editor : Editor
	{
		private Texture2D HFGUI_GrassifyHeader;
		private SerializedProperty windType;
		private SerializedProperty windStrength;
		private SerializedProperty windSpeed;
		private SerializedProperty windMotion;
		private SerializedProperty season;
		private SerializedProperty _showWindDirectionGizmo;

		void OnEnable()
		{
			HFGUI_GrassifyHeader = AssetDatabase.LoadAssetAtPath<Texture2D>(
				"Assets/Toby Fredson/The Toby Foliage Engine/(TTFE)_Core/Resources/(TTFE) GLOBAL CONTROLLER/Scripts/Editor/TTFE_Graphic Editor.png");

			windType     = serializedObject.FindProperty("windType");
			windStrength = serializedObject.FindProperty("windStrength");
			windSpeed    = serializedObject.FindProperty("windSpeed");
			windMotion   = serializedObject.FindProperty("windMotion");
			season       = serializedObject.FindProperty("season");
			_showWindDirectionGizmo = serializedObject.FindProperty("_showWindDirectionGizmo");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(HFGUI_GrassifyHeader, GUILayout.Width(192), GUILayout.Height(96));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true));
			GUILayout.Space(10);
			EditorGUILayout.LabelField("The Toby Foliage Engine version 2.0.0", EditorStyles.centeredGreyMiniLabel);

			GUILayout.BeginHorizontal();
			GUILayout.Space(7);
			EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));

			Color rectColor = new Color(0.303f, 0.364f, 0.303f, 1f);
			float luminance = 0.2126f * rectColor.r + 0.7152f * rectColor.g + 0.0722f * rectColor.b;
			Color textColor = luminance < 0.5f ? new Color(0.92f, 0.92f, 0.92f, 1f) : new Color(0.1f, 0.1f, 0.1f, 1f);

			GUIStyle textStyle = new GUIStyle(EditorStyles.label)
			{
				alignment = TextAnchor.MiddleCenter,
				fontStyle = FontStyle.Bold,
				fontSize = 12,
				normal = { textColor = textColor }
			};

			// Global Wind Parameters
			Rect windRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 6);
			windRect.width -= 14; windRect.x += 7; windRect.height = EditorGUIUtility.singleLineHeight + 6;
			EditorGUI.DrawRect(windRect, rectColor);
			Rect windTextRect = new Rect(windRect.x + 12, windRect.y + 1, windRect.width - 24, EditorGUIUtility.singleLineHeight + 2);
			EditorGUI.LabelField(windTextRect, "Global Wind Parameters", textStyle);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(windType);
			EditorGUILayout.PropertyField(windStrength);
			EditorGUILayout.PropertyField(windSpeed);
			EditorGUILayout.PropertyField(windMotion);
			EditorGUILayout.Space();
			EditorGUILayout.HelpBox("Choose between 'Gentle Breeze' or 'Wind Off'. When Gentle Breeze is selected, use the sliders above to control intensity.", MessageType.Info);
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUILayout.Space();

			// Global Season Settings
			Rect seasonRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 6);
			seasonRect.width -= 14; seasonRect.x += 7; seasonRect.height = EditorGUIUtility.singleLineHeight + 6;
			EditorGUI.DrawRect(seasonRect, rectColor);
			Rect seasonTextRect = new Rect(seasonRect.x + 12, seasonRect.y + 1, seasonRect.width - 24, EditorGUIUtility.singleLineHeight + 2);
			EditorGUI.LabelField(seasonTextRect, "Global Season Settings", textStyle);

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(season);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUILayout.Space();

			// Debug TTFE
			Rect debugRect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 6);
			debugRect.width -= 14; debugRect.x += 7; debugRect.height = EditorGUIUtility.singleLineHeight + 6;
			EditorGUI.DrawRect(debugRect, rectColor);
			Rect debugTextRect = new Rect(debugRect.x + 12, debugRect.y + 1, debugRect.width - 24, EditorGUIUtility.singleLineHeight + 2);
			EditorGUI.LabelField(debugTextRect, "Debug TTFE", textStyle);

			EditorGUILayout.Space();
			GUILayout.BeginHorizontal();
			GUILayout.Space(5);
			EditorGUILayout.LabelField("Show Wind Direction Gizmo", GUILayout.ExpandWidth(true));
			GUILayout.FlexibleSpace();
			EditorGUILayout.PropertyField(_showWindDirectionGizmo, GUIContent.none, GUILayout.Width(20));
			GUILayout.Space(5);
			GUILayout.EndHorizontal();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUILayout.Space();

			EditorGUILayout.EndVertical();
			GUILayout.Space(7);
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
			EditorGUILayout.EndVertical();

			serializedObject.ApplyModifiedProperties();
		}
	}
}