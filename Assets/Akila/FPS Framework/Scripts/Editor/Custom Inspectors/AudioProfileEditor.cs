#if UNITY_EDITOR
using Akila.FPSFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Akila.FPSFramework.Internal
{
    [CustomEditor(typeof(AudioProfile))]
    public class AudioProfileEditor : Editor
    {
        private static bool showBasic;
        private static bool show3D;
        private static bool showDynamicPitch;
        private static bool show6D;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            AudioProfile profile = (AudioProfile)target;

            Undo.RecordObject(profile, $"Modified {profile}");

            EditorGUI.BeginChangeCheck();

            // Audio Source

            profile.audioClip = (AudioClip)EditorGUILayout.ObjectField("Audio Clip", profile.audioClip, typeof(AudioClip), true);
            profile.output = (AudioMixerGroup)EditorGUILayout.ObjectField("Output", profile.output, typeof(AudioMixerGroup), true);

            GUILayout.Space(8);

            EditorGUI.indentLevel++;

            // BASIC SETTINGS

            EditorGUILayout.BeginVertical("box");

            showBasic = EditorGUILayout.Foldout(showBasic, "Basic Audio Settings", true);

            EditorGUILayout.EndVertical();

            if (showBasic)
            {
                EditorGUILayout.BeginVertical("box");

                profile.mute = EditorGUILayout.ToggleLeft("Mute", profile.mute);
                profile.bypassEffects = EditorGUILayout.ToggleLeft("Bypass Effects", profile.bypassEffects);
                profile.bypassListenerEffects = EditorGUILayout.ToggleLeft("Bypass Listener Effects", profile.bypassListenerEffects);
                profile.bypassReverbZones = EditorGUILayout.ToggleLeft("Bypass Reverb Zones", profile.bypassReverbZones);
                profile.playOnAwake = EditorGUILayout.ToggleLeft("Play On Awake", profile.playOnAwake);
                profile.loop = EditorGUILayout.ToggleLeft("Loop", profile.loop);

                GUILayout.Space(5);

                profile.priority = EditorGUILayout.IntSlider("Priority", profile.priority, 0, 256);
                profile.volume = EditorGUILayout.Slider("Priority", profile.volume, 0, 1);
                profile.pitch = EditorGUILayout.Slider("Priority", profile.pitch, -3, 3);
                profile.stereoPan = EditorGUILayout.Slider("Stereo Pan", profile.stereoPan, -1, 3);
                profile.spatialBlend = EditorGUILayout.Slider("Spatial Blend", profile.spatialBlend, 0, 1);
                profile.reverbZoneMix = EditorGUILayout.Slider("Reverb Zone Mix", profile.reverbZoneMix, 0, 1.1f);

                EditorGUILayout.EndVertical();
            }

            // DYNAMIC PITCH SETTINGS

            EditorGUILayout.BeginVertical("box");

            showDynamicPitch = EditorGUILayout.Foldout(showDynamicPitch, "Dynamic Pitch Settings", true);

            EditorGUILayout.EndVertical();

            if (showDynamicPitch)
            {
                EditorGUILayout.BeginVertical("box");

                profile.syncPitchWithTimeScale = EditorGUILayout.ToggleLeft("Sync Pitch With Time Scale", profile.syncPitchWithTimeScale);
                profile.useRandomPitchOffset = EditorGUILayout.ToggleLeft("Dynamic Pitch Enabled", profile.useRandomPitchOffset);
                profile.randomPitchOffset = EditorGUILayout.Slider("pitchOffset", profile.randomPitchOffset, -3, 3);

                EditorGUILayout.EndVertical();
            }

            // 3D SOUND SETTINGS
            EditorGUILayout.BeginVertical("box");


            show3D = EditorGUILayout.Foldout(show3D, "3D Sound Settings", true);

            EditorGUILayout.EndVertical();

            if (show3D)
            {
                EditorGUILayout.BeginVertical("box");

                profile.dopplerLevel = EditorGUILayout.Slider("Doppler Level", profile.dopplerLevel, 0, 5);
                profile.spread = EditorGUILayout.Slider("Spread", profile.spread, 0, 360);
                profile.maxDistance = EditorGUILayout.FloatField("Max Distance", profile.maxDistance);
                profile.minDistance = EditorGUILayout.FloatField("Min Distance", profile.minDistance);
                profile.simulateAcousticLatency = EditorGUILayout.Toggle("Simulate Acoustic Latency", profile.simulateAcousticLatency);

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginVertical("box");

            // 6D SOUND SETTINGS
            show6D = EditorGUILayout.Foldout(show6D, "6D Sound Settings", true);

            EditorGUILayout.EndVertical();


            if (show6D)
            {
                EditorGUILayout.BeginVertical("box");

                profile.forwardFactor = EditorGUILayout.Slider("Forward Factor", profile.forwardFactor, -1, 1);
                profile.backwardFactor = EditorGUILayout.Slider("Backward Factor", profile.backwardFactor, -1, 1);
                profile.rightFactor = EditorGUILayout.Slider("Right Factor", profile.rightFactor, -1, 1);
                profile.leftFactor = EditorGUILayout.Slider("Left Factor", profile.leftFactor, -1, 1);
                profile.upFactor = EditorGUILayout.Slider("Above Factor", profile.upFactor, -1, 1);
                profile.downFactor = EditorGUILayout.Slider("Delow Factor", profile.downFactor, -1, 1);
                profile._6DSoundCurve = EditorGUILayout.CurveField("6D Sound Curve", profile._6DSoundCurve);

                EditorGUILayout.EndVertical();
            }




            EditorGUI.indentLevel--;

            if (profile.spread == 0)
            {
                EditorGUILayout.HelpBox("6D Sounds require 'Spread' to be greater than 1. 6D sounds won't be used.", MessageType.Info);
            }

            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Event Timeline changes during playmode will not apply until next time entering play mode.", MessageType.Warning);
            }

            GUILayout.Space(10);

            DrawTimeline();

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(profile);
        }


        #region Timeline
        private const float timelineHeight = 25;
        private const float markerSize = 8f;
        private const float tickHeightMajor = 10f;
        private const float tickHeightMinor = 5f;
        private const float timeLabelHeight = 14f;

        private Color timelineColor = new Color(0.3f, 0.3f, 0.3f);
        private Color markerColor = Color.gray;
        private Color selectedColor = Color.white;

        private int selectedMarkerIndex = -1;

        private float cursorTime = 0f;
        private bool isDragging;

        private void DrawTimeline()
        {
            AudioProfile track = (AudioProfile)target;
            serializedObject.Update();

            track.audioLayersDuration = Mathf.Clamp(track.audioLayersDuration, 0, float.MaxValue);

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Event Timeline", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            float buttonSize = timelineHeight;
            float spacing = 5f;

            Rect fullRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(timelineHeight));

            Rect buttonRect = new Rect(fullRect.x, fullRect.y, buttonSize, buttonSize);
            Rect timelineRect = new Rect(buttonRect.xMax + spacing, fullRect.y, fullRect.width - buttonSize - spacing, timelineHeight);

            EditorGUI.BeginDisabledGroup(track.audioLayersDuration == 0);

            if (GUI.Button(buttonRect, "+"))
            {
                track.audioLayers.Add(new AudioProfile.CustomAudioLayer { time = cursorTime });
                EditorUtility.SetDirty(track);
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.DrawRect(timelineRect, timelineColor);
            DrawTimeLabels(timelineRect, track.audioLayersDuration);
            DrawMarkers(timelineRect, track);
            DrawTimeCursor(timelineRect, track);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(14);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Duration:", GUILayout.Width(60));
            track.audioLayersDuration = EditorGUILayout.FloatField(track.audioLayersDuration, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            // Keyframe Inspector Panel
            if (track.audioLayersDuration > 0f && selectedMarkerIndex >= 0 && selectedMarkerIndex < track.audioLayers.Count)
            {
                AudioProfile.CustomAudioLayer selectedEvent = track.audioLayers[selectedMarkerIndex];

                EditorGUILayout.LabelField("Keyframe", EditorStyles.boldLabel);

                EditorGUI.BeginChangeCheck();

                // Audio Clip Field
                selectedEvent.audioClip = (AudioClip)EditorGUILayout.ObjectField(
                    "Audio Clip",
                    selectedEvent.audioClip,
                    typeof(AudioClip),
                    false
                );

                // Time Slider
                selectedEvent.time = EditorGUILayout.Slider("Time", selectedEvent.time, 0f, Mathf.Max(0.01f, track.audioLayersDuration));

                if (EditorGUI.EndChangeCheck())
                {
                    selectedEvent.time = Mathf.Clamp(selectedEvent.time, 0f, track.audioLayersDuration);
                    EditorUtility.SetDirty(track);
                }
            }
            else if (track.audioLayersDuration > 0f && track.audioLayers.Count > 0)
            {
                EditorGUILayout.HelpBox("No keyframe selected. Click a keyframe on the timeline to view and edit its properties.", MessageType.Info);
            }

            if (track.audioLayersDuration > 0f && track.audioLayers.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "There are no keyframes. To add one, drag the red playhead to a position on the timeline, then click the '+' button.",
                    MessageType.Info
                );
            }


            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTimeLabels(Rect rect, float duration)
        {
            if (duration <= 0f || rect.width <= 0f)
            {
                GUIStyle centered = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                GUI.Label(rect, "Set duration > 0 to view timeline", centered);
                return;
            }

            Handles.color = Color.gray;
            GUIStyle labelStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.UpperCenter
            };

            float totalMilliseconds = duration * 1000f;
            float minLabelSpacing = 40f;
            float pixelsPerSecond = rect.width / duration;
            if (pixelsPerSecond <= 0f) return;

            int secondsStep = Mathf.CeilToInt(minLabelSpacing / pixelsPerSecond);

            for (int ms = 0; ms <= totalMilliseconds; ms += 100)
            {
                float t = ms / 1000f;
                float normalized = Mathf.Clamp01(t / duration);
                float x = Mathf.Lerp(rect.x, rect.xMax, normalized);

                bool isMajor = ms % 1000 == 0;
                float tickHeight = isMajor ? tickHeightMajor : tickHeightMinor;

                Handles.DrawLine(
                    new Vector3(x, rect.yMax - tickHeight),
                    new Vector3(x, rect.yMax)
                );

                if (isMajor && ((int)t % secondsStep == 0))
                {
                    GUI.Label(new Rect(x - 15, rect.yMax, 30, timeLabelHeight), $"{t:0.#}s", labelStyle);
                }
            }
        }
        private void DrawMarkers(Rect rect, AudioProfile track)
        {
            Event evt = Event.current;
            float yCenter = rect.y + rect.height / 2f;

            for (int i = 0; i < track.audioLayers.Count; i++)
            {
                var e = track.audioLayers[i];
                float normalizedTime = Mathf.Clamp01(e.time / track.audioLayersDuration);
                float x = rect.x + normalizedTime * rect.width;

                Vector2 center = new Vector2(x, yCenter);
                float halfSize = markerSize / 2f;

                Vector3[] diamondPoints = new Vector3[4];
                diamondPoints[0] = new Vector3(center.x, center.y - halfSize);
                diamondPoints[1] = new Vector3(center.x + halfSize, center.y);
                diamondPoints[2] = new Vector3(center.x, center.y + halfSize);
                diamondPoints[3] = new Vector3(center.x - halfSize, center.y);

                Color fill = (selectedMarkerIndex == i) ? selectedColor : markerColor;
                Handles.DrawSolidRectangleWithOutline(diamondPoints, fill, Color.black);

                Rect clickRect = new Rect(center.x - 8, center.y - 8, 16, 16);

                if (evt.type == EventType.MouseDown && evt.button == 0 && clickRect.Contains(evt.mousePosition))
                {
                    selectedMarkerIndex = i;
                    isDragging = true;
                    evt.Use();
                }

                if (evt.type == EventType.ContextClick && clickRect.Contains(evt.mousePosition))
                {
                    int index = i;
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, () =>
                    {
                        track.audioLayers.RemoveAt(index);
                        selectedMarkerIndex = -1;
                        EditorUtility.SetDirty(track);
                    });
                    menu.ShowAsContext();
                    evt.Use();
                }
            }

            if (isDragging && selectedMarkerIndex >= 0 && evt.type == EventType.MouseDrag)
            {
                float newNormalized = Mathf.InverseLerp(rect.x, rect.xMax, evt.mousePosition.x);
                float newTime = Mathf.Clamp01(newNormalized) * track.audioLayersDuration;

                track.audioLayers[selectedMarkerIndex].time = newTime;
                EditorUtility.SetDirty(track);
                evt.Use();
            }

            if (evt.type == EventType.MouseUp && isDragging)
            {
                isDragging = false;
                evt.Use();
            }
        }


        private void DrawTimeCursor(Rect rect, AudioProfile track)
        {
            Event evt = Event.current;

            float normalized = track.audioLayersDuration > 0 ? Mathf.Clamp01(cursorTime / track.audioLayersDuration) : 0f;
            float x = rect.x + normalized * rect.width;

            Handles.color = Color.red;
            Handles.DrawLine(new Vector3(x, rect.y), new Vector3(x, rect.y + rect.height));

            if (evt.type == EventType.MouseDown && evt.button == 0 && rect.Contains(evt.mousePosition))
            {
                float clickedNormalized = Mathf.InverseLerp(rect.x, rect.xMax, evt.mousePosition.x);
                cursorTime = Mathf.Clamp(clickedNormalized * track.audioLayersDuration, 0f, track.audioLayersDuration);
                evt.Use();
            }

            if (evt.type == EventType.MouseDrag && rect.Contains(evt.mousePosition))
            {
                float draggedNormalized = Mathf.InverseLerp(rect.x, rect.xMax, evt.mousePosition.x);
                cursorTime = Mathf.Clamp(draggedNormalized * track.audioLayersDuration, 0f, track.audioLayersDuration);
                evt.Use();
            }
        }
        #endregion
    }
}
#endif