using UnityEditor;
using UnityEngine;
using static Akila.FPSFramework.SprayPattern;

namespace Akila.FPSFramework.Internal
{
    #if UNITY_EDITOR
    [CustomEditor(typeof(SprayPattern))]
    public class SprayPatternEditor : Editor
    {
        private int selectedPointIndex = -1;
        private int draggedPointIndex = -1;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SprayPattern pattern = (SprayPattern)target;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Spray Settings", EditorStyles.boldLabel);

            pattern.maxAmount = EditorGUILayout.FloatField(
                new GUIContent("Max Amount", "Maximum spread amount applied to the weapon."),
                pattern.maxAmount
            );

            pattern.verticalMultiplier = EditorGUILayout.FloatField(
               new GUIContent("Vertical Multiplier", "How much vertical (Up and down) spread is produced relative to Max Amount."),
               pattern.verticalMultiplier
            );

            pattern.horizontalMultiplier = EditorGUILayout.FloatField(
               new GUIContent("Horizontal Multiplier", "How much horizontal (Left and right) spread is produced relative to Max Amount."),
               pattern.horizontalMultiplier
            );

            pattern.passiveMultiplier = EditorGUILayout.Slider(
                new GUIContent("Passive Multiplier", "Controls spray intensity when the player is idle or not moving. Higher values increase spread."),
                pattern.passiveMultiplier, 0, 1
            );

            pattern.rampUpTime = EditorGUILayout.Slider(
                new GUIContent("Ramp Up Time", "Time it takes to reach full spread while shooting."),
                pattern.rampUpTime, 0, 10
            );

            pattern.recoveryTime = EditorGUILayout.Slider(
                new GUIContent("Recovery Time", "Time it takes for the spread to return to passive value after stopping fire."),
                pattern.recoveryTime, 0, 10
            );

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Recoil Settings", EditorStyles.boldLabel);

            pattern.isRandomized = EditorGUILayout.Toggle(
                new GUIContent("Is Randomized", "Enables random spray behavior instead of following a fixed pattern."),
                pattern.isRandomized
            );

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(pattern.isRandomized);

            pattern.fixedPatternSource = (SprayPattern.FixedPatternSource)EditorGUILayout.EnumPopup(
            new GUIContent(
                "Fixed Pattern Source",
                "Controls how the fixed spray pattern is sourced:\n" +
                "- Independent: Uses the internal fixed pattern defined in this asset.\n" +
                "- Shared: Uses a fixed pattern copied from another asset."), pattern.fixedPatternSource);

            // Conditional display: external pattern reference
            if (pattern.fixedPatternSource == FixedPatternSource.External)
            {
                pattern.externalSprayPatternToCopy = (SprayPattern)EditorGUILayout.ObjectField(
                    new GUIContent(
                        "External Spray Pattern",
                        "Reference to the external spray pattern asset to use."
                    ),
                    pattern.externalSprayPatternToCopy,
                    typeof(SprayPattern),
                    false
                );
            }

            if (pattern.externalSprayPatternToCopy)
            {
                if (pattern.externalSprayPatternToCopy.isRandomized)
                {
                    pattern.externalSprayPatternToCopy = null;

                    Debug.LogError("Invalid assignment: cannot use a randomized spray pattern as an external fixed reference. Please assign a non-randomized (fixed) spray pattern asset instead.", pattern);
                }
            }

            pattern.autoFill = EditorGUILayout.Toggle(
                new GUIContent("Auto Fill", "If enabled, the spray pattern will automatically generate intermediate points between spray dots, creating a smoother and more continuous pattern with less manual effort. (Only used in fixed patterns)"),
                pattern.autoFill
            );

            pattern.loop = EditorGUILayout.Toggle(
                new GUIContent("Loop", "If enabled, the spray pattern will repeat from the beginning after the last point. (Only used in fixed patterns)"),
                pattern.loop
            );

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Spray Pattern", EditorStyles.boldLabel);


            if(pattern.fixedPatternSource == FixedPatternSource.External && pattern.externalSprayPatternToCopy)
            {
                EditorGUILayout.HelpBox(
                    "External spray pattern is active. This asset's local fixed pattern will be ignored, and the external asset will be used instead.",
                    MessageType.Info
                );
            }
            
            EditorGUI.BeginDisabledGroup(pattern.fixedPatternSource == FixedPatternSource.External && pattern.externalSprayPatternToCopy);

            DrawSprayPatternPreview(pattern);

            GUILayout.BeginHorizontal();


            if (GUILayout.Button("Add Point"))
            {
                GenericMenu menu = new GenericMenu();
                Vector2 pos = Random.insideUnitCircle;

                menu.AddItem(new GUIContent("Add In Random Position"), false, () =>
                {
                    Undo.RecordObject(pattern, "Add Spray Point (Random)");

                    ArrayUtility.Add(ref pattern.points, new SprayPattern.SprayPatternPoint
                    {
                        upDown = pos.y,
                        rightLeft = pos.x
                    });

                    selectedPointIndex = pattern.points.Length - 1;
                    EditorUtility.SetDirty(pattern);
                });

                menu.AddItem(new GUIContent("Add In Center"), false, () =>
                {
                    Undo.RecordObject(pattern, "Add Spray Point (Center)");

                    ArrayUtility.Add(ref pattern.points, new SprayPattern.SprayPatternPoint
                    {
                        upDown = 0f,
                        rightLeft = 0f
                    });

                    selectedPointIndex = pattern.points.Length - 1;
                    EditorUtility.SetDirty(pattern);
                });

                menu.ShowAsContext();
            }

            EditorGUI.BeginDisabledGroup((selectedPointIndex >= 0 && selectedPointIndex < pattern.points.Length) == false);

            if (GUILayout.Button("Remove Selected"))
            {
                Undo.RecordObject(pattern, "Remove Spray Point");

                int newIndex = selectedPointIndex - 1;
                ArrayUtility.RemoveAt(ref pattern.points, selectedPointIndex);

                selectedPointIndex = pattern.points.Length > 0 ? Mathf.Clamp(newIndex, 0, pattern.points.Length - 1) : -1;

                EditorUtility.SetDirty(pattern);
            }

            EditorGUI.EndDisabledGroup();   
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(pattern, "Edit Spray Pattern");
                EditorUtility.SetDirty(pattern);
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUI.EndDisabledGroup();

            EditorGUI.EndDisabledGroup();
        }

        private void DrawSprayPatternPreview(SprayPattern pattern)
        {
            if (pattern.isRandomized)
            {
                selectedPointIndex = -1;
                draggedPointIndex = -1;
            }

            float height = 200f;
            Rect previewRect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.helpBox, GUILayout.Height(height));
            GUI.Box(previewRect, GUIContent.none, EditorStyles.helpBox);

            Vector2 center = previewRect.center;
            float scale = Mathf.Min(previewRect.width, previewRect.height) / 2f;

            Handles.BeginGUI();
            Color originalColor = Handles.color;

            // Draw grid
            Handles.color = new Color(1f, 1f, 1f, 0.05f);
            int gridLines = 10;
            float gridSpacing = previewRect.height / gridLines;

            for (float x = center.x; x < previewRect.xMax; x += gridSpacing)
                Handles.DrawLine(new Vector2(x, previewRect.y), new Vector2(x, previewRect.yMax));
            for (float x = center.x - gridSpacing; x > previewRect.xMin; x -= gridSpacing)
                Handles.DrawLine(new Vector2(x, previewRect.y), new Vector2(x, previewRect.yMax));
            for (float y = center.y; y < previewRect.yMax; y += gridSpacing)
                Handles.DrawLine(new Vector2(previewRect.x, y), new Vector2(previewRect.xMax, y));
            for (float y = center.y - gridSpacing; y > previewRect.yMin; y -= gridSpacing)
                Handles.DrawLine(new Vector2(previewRect.x, y), new Vector2(previewRect.xMax, y));

            // Center axes
            Handles.color = new Color(1f, 1f, 1f, 0.25f);
            Handles.DrawLine(new Vector2(center.x, previewRect.y), new Vector2(center.x, previewRect.yMax));
            Handles.DrawLine(new Vector2(previewRect.x, center.y), new Vector2(previewRect.xMax, center.y));

            // Event handling
            Event e = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            bool mouseDown = e.type == EventType.MouseDown && e.button == 0;
            bool mouseDrag = e.type == EventType.MouseDrag && e.button == 0;
            bool mouseUp = e.type == EventType.MouseUp && e.button == 0;
            Vector2 mousePos = e.mousePosition;

            if (pattern.points.Length > 0)
            {
                for (int i = 0; i < pattern.points.Length; i++)
                {
                    var point = pattern.points[i];
                    Vector2 guiPos = center + new Vector2(point.rightLeft, -point.upDown) * scale;
                    float handleSize = 6f;
                    Rect hitBox = new Rect(guiPos.x - handleSize, guiPos.y - handleSize, handleSize * 2, handleSize * 2);

                    // Select
                    if (mouseDown && hitBox.Contains(mousePos))
                    {
                        selectedPointIndex = i;
                        draggedPointIndex = i;
                        GUIUtility.hotControl = controlID;
                        e.Use();
                    }

                    // Drag
                    if (mouseDrag && draggedPointIndex == i && GUIUtility.hotControl == controlID)
                    {
                        // Clamp the GUI position inside previewRect before converting back to logical space
                        Vector2 clampedGUIPos = mousePos;
                        clampedGUIPos.x = Mathf.Clamp(clampedGUIPos.x, previewRect.xMin, previewRect.xMax);
                        clampedGUIPos.y = Mathf.Clamp(clampedGUIPos.y, previewRect.yMin, previewRect.yMax);

                        // Convert back to logical space
                        Vector2 localPos = (clampedGUIPos - center) / scale;

                        Undo.RecordObject(pattern, "Drag Spray Point");
                        pattern.points[i].rightLeft = localPos.x;
                        pattern.points[i].upDown = -localPos.y;

                        EditorUtility.SetDirty(pattern);
                        e.Use();
                    }

                    // End drag
                    if (mouseUp && draggedPointIndex == i)
                    {
                        draggedPointIndex = -1;
                        GUIUtility.hotControl = 0;
                        e.Use();
                    }

                    // Draw
                    float t = i / Mathf.Max(1f, pattern.points.Length - 1);
                    Color color = Color.Lerp(Color.yellow, Color.red, t);

                    if (pattern.isRandomized || pattern.fixedPatternSource == FixedPatternSource.External && pattern.externalSprayPatternToCopy)
                        color.a = 0.3f;

                    if (i == selectedPointIndex)
                    {
                        Handles.color = EditorGUIUtility.isProSkin ? new Color(0.24f, 0.48f, 0.90f) : new Color(0.24f, 0.49f, 0.90f);
                        Handles.DrawSolidDisc(guiPos, Vector3.forward, 6f);
                    }

                    Handles.color = color;
                    Handles.DrawSolidDisc(guiPos, Vector3.forward, 4f);
                }
            }

            Handles.color = originalColor;
            Handles.EndGUI();
        }
    }
#endif
}
