using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Akila.FPSFramework
{
    /// <summary>
    /// JSON-based save system.
    /// - Save/Load objects into their own files.
    /// - Save/Load primitive values grouped in a single JSON file.
    /// - Delete all saves or check if a key exists.
    /// </summary>
    public static class SaveSystem
    {
        public static readonly string BasePath = $"{Application.persistentDataPath}/Akila Documents/FPSFramework";

        private static string GetPath(string fileName)
        {
            if (!Directory.Exists(BasePath))
                Directory.CreateDirectory(BasePath);

            return Path.Combine(BasePath, fileName + ".json");
        }

        // -------- SAVE/LOAD COMPLEX OBJECTS --------
        public static void SaveObject<T>(T data, string fileName)
        {
            string path = GetPath(fileName);
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public static T LoadObject<T>(string fileName)
        {
            string path = GetPath(fileName);
            if (File.Exists(path))
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));

            return default;
        }

        public static bool HasObject(string fileName)
        {
            string path = GetPath(fileName);
            return File.Exists(path);
        }

        // -------- SAVE/LOAD PRIMITIVES (grouped in one file) --------
        [System.Serializable]
        private class PrefsFile
        {
            public Dictionary<string, object> values = new Dictionary<string, object>();
        }

        private static PrefsFile LoadPrefs(string fileName)
        {
            var prefs = LoadObject<PrefsFile>(fileName);
            return prefs ?? new PrefsFile();
        }

        public static void SaveValue<T>(string key, T value, string fileName = "Prefs")
        {
            var prefs = LoadPrefs(fileName);
            prefs.values[key] = value;
            SaveObject(prefs, fileName);
        }

        public static T LoadValue<T>(string key, string fileName = "Prefs")
        {
            var prefs = LoadPrefs(fileName);
            if (prefs.values.TryGetValue(key, out object raw))
            {
                try { return (T)System.Convert.ChangeType(raw, typeof(T)); }
                catch { return default; }
            }
            return default;
        }

        public static T LoadOrSaveValue<T>(string key, string fileName = "Prefs", T defaultValue = default)
        {
            if (HasKey(key, fileName))
            {
                return LoadValue<T>(key, fileName);
            }
            else
            {
                SaveValue(key, defaultValue, fileName);
                return defaultValue;
            }
        }


        public static bool HasKey(string key, string fileName = "Prefs")
        {
            var prefs = LoadPrefs(fileName);
            return prefs.values.ContainsKey(key);
        }

        // -------- DELETE ALL SAVES --------
        public static void DeleteAllSaves()
        {
            if (Directory.Exists(BasePath))
            {
                Directory.Delete(BasePath, true); // recursive
                Directory.CreateDirectory(BasePath); // recreate empty folder
                Debug.Log("[SaveSystem] All saves deleted.");
            }
            else
            {
                Debug.Log("[SaveSystem] No save directory to delete.");
            }
        }
    }
}
