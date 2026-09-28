using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// What Continue restores. The story visits Bedroom Scene three times (morning, home from school,
/// waking at the desk after Level 5) and School Scene twice (first day, late morning), so the
/// scene name alone always restarted them from the morning / first-day flow. Alongside the scene
/// this remembers which of those visits it was, and Continue re-queues the same scene-load
/// handoff the story used to get there.
/// </summary>
public static class SaveCheckpoint
{
    public const string SceneKey = "SavedScene";
    private const string VariantKey = "SavedVariant";
    private const string SpawnKey = "SavedHomeSpawn";

    /// <summary>Bedroom Scene, back from school (HomeArrivalSequence).</summary>
    public const string HomeArrival = "HomeArrival";
    /// <summary>Bedroom Scene, waking at the desk after Level 5 (DeskWakeUpSequence).</summary>
    public const string DeskWakeUp = "DeskWakeUp";
    /// <summary>School Scene, the late morning (LateSchoolArrivalSequence).</summary>
    public const string LateSchool = "LateSchool";

    private static string variantThisLoad;

    // Play Mode can start without a domain reload, so don't let a variant leak between runs.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => variantThisLoad = null;

    /// <summary>Called by a handoff's sceneLoaded handler, which runs before SceneAutoSaver.Start.</summary>
    public static void MarkVariant(string variant) => variantThisLoad = variant;

    /// <summary>Called by SceneAutoSaver as each scene starts.</summary>
    public static void SaveCurrentScene()
    {
        string scene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString(SceneKey, scene);
        PlayerPrefs.SetString(VariantKey, variantThisLoad ?? "");
        variantThisLoad = null;
        PlayerPrefs.Save();
        Debug.Log("Auto-saved scene: " + scene + (PlayerPrefs.GetString(VariantKey) != "" ? " (" + PlayerPrefs.GetString(VariantKey) + ")" : ""));
    }

    /// <summary>New Game: start from the plain first scene.</summary>
    public static void SaveNewGame(string scene)
    {
        PlayerPrefs.SetString(SceneKey, scene);
        PlayerPrefs.DeleteKey(VariantKey);
        PlayerPrefs.DeleteKey(SpawnKey);
        PlayerPrefs.Save();
    }

    /// <summary>Where SchoolHomeSequence puts Thuta when he walks in from school.</summary>
    public static void SaveHomeSpawn(Vector3 position, float yaw)
    {
        var c = CultureInfo.InvariantCulture;
        PlayerPrefs.SetString(SpawnKey, string.Join(";", position.x.ToString(c), position.y.ToString(c), position.z.ToString(c), yaw.ToString(c)));
    }

    /// <summary>Continue: call right before loading the saved scene.</summary>
    public static void RestoreVariantForNextLoad()
    {
        switch (PlayerPrefs.GetString(VariantKey, ""))
        {
            case HomeArrival:
                if (TryGetHomeSpawn(out Vector3 position, out float yaw)) SchoolHomeSequence.QueueHomecoming(position, yaw);
                break;
            case DeskWakeUp:
                DeskWakeUpSequence.QueueForNextLoad();
                break;
            case LateSchool:
                LateSchoolArrivalSequence.QueueForNextLoad();
                break;
        }
    }

    private static bool TryGetHomeSpawn(out Vector3 position, out float yaw)
    {
        position = Vector3.zero;
        yaw = 0f;
        string[] parts = PlayerPrefs.GetString(SpawnKey, "").Split(';');
        if (parts.Length != 4) return false;

        var c = CultureInfo.InvariantCulture;
        bool ok = float.TryParse(parts[0], NumberStyles.Float, c, out position.x)
                  & float.TryParse(parts[1], NumberStyles.Float, c, out position.y)
                  & float.TryParse(parts[2], NumberStyles.Float, c, out position.z)
                  & float.TryParse(parts[3], NumberStyles.Float, c, out yaw);
        return ok;
    }
}
