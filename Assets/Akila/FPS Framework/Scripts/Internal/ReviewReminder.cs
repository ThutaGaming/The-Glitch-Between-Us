using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;

namespace Akila.FPSFramework
{
    [InitializeOnLoad]
    public static class ReviewReminder
    {
        // ==== CONFIG ====
        private static readonly TimeSpan ReminderInterval = TimeSpan.FromDays(7); // wait 7 days
        private static string AssetStoreUrl = FPSFrameworkSettings.reviewUrl;

        // ==== PREF KEYS ====
        private const string DisabledKey = "Akila_FPSFramework_ReviewReminder_Disabled";
        private const string NextTriggerKey = "Akila_FPSFramework_ReviewReminder_NextTrigger";
        private const string PendingKey = "Akila_FPSFramework_ReviewReminder_Pending";

        // Track if a dialog is currently open
        private static bool dialogOpen = false;

        static ReviewReminder()
        {
            if (!EditorPrefs.HasKey(NextTriggerKey))
            {
                ScheduleNext(DateTime.UtcNow + ReminderInterval);
            }

            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;

            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnEditorUpdate()
        {
            if (EditorPrefs.GetBool(DisabledKey, false))
                return;

            if (dialogOpen) // don't trigger if already open
                return;

            DateTime now = DateTime.UtcNow;
            DateTime next = GetNextTrigger();

            if (now >= next)
            {
                // Schedule next reminder first
                ScheduleNext(now + ReminderInterval);

                if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying)
                {
                    // Delay showing reminder until play mode exits
                    EditorPrefs.SetBool(PendingKey, true);
                }
                else
                {
                    ShowReminder();
                }
            }
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode && EditorPrefs.GetBool(PendingKey, false))
            {
                EditorPrefs.SetBool(PendingKey, false);
                if (!dialogOpen)
                    ShowReminder();
            }
        }

        private static DateTime GetNextTrigger()
        {
            var raw = EditorPrefs.GetString(NextTriggerKey, string.Empty);
            if (long.TryParse(raw, out long ticks))
                return new DateTime(ticks, DateTimeKind.Utc);

            DateTime fallback = DateTime.UtcNow + ReminderInterval;
            ScheduleNext(fallback);
            return fallback;
        }

        private static void ScheduleNext(DateTime at)
        {
            EditorPrefs.SetString(NextTriggerKey, at.Ticks.ToString());
        }

        private static void ShowReminder()
        {
            if (dialogOpen) return;

            dialogOpen = true;

            const string title = "Enjoying FPS Framework?";
            const string message = "If the asset is helping your project, a quick review on the Asset Store really supports ongoing updates.";

            try
            {
                int choice = EditorUtility.DisplayDialogComplex(
                    title,
                    message,
                    "Review now",
                    "Remind me later",
                    "Don't ask again"
                );

                switch (choice)
                {
                    case 0: // Review now
                        Application.OpenURL(AssetStoreUrl);
                        EditorPrefs.SetBool(DisabledKey, true);
                        break;
                    case 1: // Remind me later
                        ScheduleNext(DateTime.UtcNow + ReminderInterval);
                        break;
                    case 2: // Don't ask again
                        EditorPrefs.SetBool(DisabledKey, true);
                        break;
                }
            }
            finally
            {
                dialogOpen = false;
            }
        }
    }
}
#endif