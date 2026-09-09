using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms;

namespace Akila.FPSFramework.Internal
{
    /// <summary>
    /// Checks for the latest version of the FPS Framework from a remote source.
    /// </summary>
    public class FPSFrameworkVersionChecker
    {
        private const string versionURL = "https://drive.google.com/file/d/1MPnsXllnuu0nPhAg6S2wZ5rX9106xZY9/view?usp=sharing";
        private const int TimeoutSeconds = 15;

        /// <summary>
        /// Invoked when the check method is finished.
        /// </summary>
        public Action OnCheckFinished { get; set; }

        /// <summary>Indicates whether a version check is currently in progress.</summary>
        public bool IsChecking { get; private set; }

        /// <summary>Indicates whether the last check was successful.</summary>
        public bool WasSuccessful { get; private set; }

        /// <summary>Indicates whether the last check failed.</summary>
        public bool WasFailure { get; private set; }

        /// <summary>Indicates whether a newer version is available.</summary>
        public bool IsUpdateAvailable { get; private set; }

        /// <summary>The latest version number retrieved from the remote source.</summary>
        public string LatestVersion { get; private set; }

        /// <summary>The changelog of the latest version.</summary>
        public string Changelog { get; private set; }

        /// <summary>Status message describing the result of the latest check.</summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Asynchronously checks for updates by contacting a remote version file.
        /// </summary>
        public async Task CheckForUpdateAsync()
        {
            if (IsChecking)
                return;

            ResetStatus();

            StatusMessage = "Checking for updates...";
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            while (stopwatch.Elapsed.TotalSeconds < TimeoutSeconds)
            {
                string directURL = ConvertToDirectDownloadURL(versionURL);
                if (string.IsNullOrEmpty(directURL))
                {
                    StatusMessage = "Invalid update URL.";
                    WasFailure = true;
                    IsChecking = false;
                    return;
                }

                using (var request = UnityWebRequest.Get(directURL))
                {
                    var operation = request.SendWebRequest();
                    while (!operation.isDone)
                        await Task.Yield();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        try
                        {
                            var json = request.downloadHandler.text;
                            var remote = JsonUtility.FromJson<RemoteVersion>(json);

                            LatestVersion = remote.version;
                            Changelog = remote.changelog;

                            if (remote.version != FPSFrameworkCore.version)
                            {
                                IsUpdateAvailable = true;
                                StatusMessage = $"Update available! New version: {remote.version}";

#if UNITY_EDITOR
                                bool isUserWantToUpdate = EditorUtility.DisplayDialog(
                                    "Update Available",
                                    $"A new version of FPSF is available (v{remote.version}).\n\nWould you like to open the UAS to update now?",
                                    "Update Now",
                                    "Later"
                                );

                                if(isUserWantToUpdate)
                                {
                                    Application.OpenURL(FPSFrameworkSettings.preset.url);
                                }
                                else
                                {
                                    FPSFrameworkSettings.preset.checkForUpdateThiSession = false;
                                }
#endif
                            }
                            else
                            {
                                StatusMessage = "You are up to date.";
                            }

                            WasSuccessful = true;
                            IsChecking = false;
                            return;
                        }
                        catch (Exception ex)
                        {
                            WasFailure = true;
                            StatusMessage = $"Error parsing version info: {ex.Message}";
                            Debug.LogError(ex);
                            IsChecking = false;
                            return;
                        }
                    }
                }

                // Wait before retrying
                await Task.Delay(500);
            }

            StatusMessage = "Failed to check version: No internet or server unreachable.";
            WasFailure = true;
            IsChecking = false;

            OnCheckFinished?.Invoke();
        }

        /// <summary>
        /// Converts a standard Google Drive file URL to a direct download link.
        /// </summary>
        /// <param name="googleDriveLink">The shared Google Drive URL.</param>
        /// <returns>The direct download URL or null if the format is invalid.</returns>
        public static string ConvertToDirectDownloadURL(string googleDriveLink)
        {
            const string prefix = "https://drive.google.com/file/d/";
            const string suffix = "/view";

            if (googleDriveLink.StartsWith(prefix) && googleDriveLink.Contains(suffix))
            {
                int startIndex = prefix.Length;
                int endIndex = googleDriveLink.IndexOf(suffix, startIndex);

                string fileId = googleDriveLink.Substring(startIndex, endIndex - startIndex);
                return $"https://drive.google.com/uc?export=download&id={fileId}";
            }

            Debug.LogWarning("Invalid Google Drive link format.");
            return null;
        }

        /// <summary>
        /// Resets all status flags and values before a new check begins.
        /// </summary>
        private void ResetStatus()
        {
            IsChecking = true;
            WasSuccessful = false;
            WasFailure = false;
            IsUpdateAvailable = false;
            LatestVersion = null;
            Changelog = null;
            StatusMessage = null;
        }

        [Serializable]
        private class RemoteVersion
        {
            public string version;
            public string changelog;
        }
    }
}
