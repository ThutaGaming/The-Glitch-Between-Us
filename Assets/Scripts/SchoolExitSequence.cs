using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gates the bedroom's front door: locked until the backpack is worn (morning routine done).
/// Opening it then skips the door's own swing animation and instead cuts to a black screen
/// with a few caption lines (a time-skip beat) before loading School Scene.
/// Lines are English because Unity's IMGUI/TextMeshPro cannot shape Burmese correctly.
/// </summary>
public class SchoolExitSequence : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InteractableDoor exitDoor;
    [SerializeField] private BackpackStation backpack;
    [Tooltip("Disabled while the black-screen beat plays, re-enabled if the scene load ever fails.")]
    [SerializeField] private Behaviour[] disableDuringSequence;

    [Header("Objective Marker")]
    [SerializeField] private MissionHUD mission;
    [SerializeField] private string doorObjective = "Head out the door";
    [Tooltip("Lets the backpack-worn tick-off read before this objective replaces it.")]
    [SerializeField] private float objectiveDelayAfterBackpack = 1.6f;
    [Tooltip("Faint blue marker on the door; on once this becomes the objective, off once opened.")]
    [SerializeField] private ObjectiveGlow doorGlow;

    [Header("Audio")]
    [Tooltip("Starts partway in (streetSoundStartTime) the moment the player steps out; cut off when School Scene loads.")]
    [SerializeField] private AudioSource streetSound;
    [SerializeField] private float streetSoundStartTime = 3f;
    [Tooltip("Volume the street sound starts at.")]
    [SerializeField] private float streetSoundStartVolume = 0.3f;
    [Tooltip("Volume it gradually swells up to before being cut off.")]
    [SerializeField] private float streetSoundPeakVolume = 1f;
    [Tooltip("How long the swell from start to peak volume takes.")]
    [SerializeField] private float streetSoundFadeUpDuration = 6f;

    [Header("Captions")]
    [SerializeField] private string leavingLine = "Heading to school.";
    [SerializeField] private string timeSkipLine = "30 minutes later...";
    [SerializeField] private string arrivedLine = "Arrived at school.";
    [Tooltip("Extra seconds a fully-typed caption stays up before the next one starts.")]
    [SerializeField] private float captionHold = 2.0f;
    [Tooltip("Characters revealed per second.")]
    [SerializeField] private float typeSpeed = 22f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private int captionFontSize = 28;

    [Header("Scene")]
    [SerializeField] private string schoolSceneName = "School Scene";

    private bool started;
    private float overlayAlpha;
    private string captionText = "";
    private int revealedChars;

    private Texture2D solid;
    private GUIStyle captionStyle;

    private void Awake()
    {
        solid = BuildSolidTexture();

        if (mission == null) mission = MissionHUD.Instance != null
            ? MissionHUD.Instance
            : FindFirstObjectByType<MissionHUD>();

        if (exitDoor != null) exitDoor.SetLocked(true);
        if (backpack != null) backpack.onWorn.AddListener(Unlock);
    }

    private void OnDestroy()
    {
        if (backpack != null) backpack.onWorn.RemoveListener(Unlock);
        if (exitDoor != null) exitDoor.onOpened.RemoveListener(BeginLeaving);
        if (solid != null) Destroy(solid);
    }

    private void Unlock()
    {
        if (exitDoor == null) return;
        exitDoor.SetLocked(false);
        exitDoor.SetSkipOpenAnimation(true);
        exitDoor.onOpened.AddListener(BeginLeaving);
        StartCoroutine(ShowDoorObjective());
    }

    private IEnumerator ShowDoorObjective()
    {
        // Let the "backpack worn" tick-off read before this objective replaces it - same
        // pattern as MorningRoutineQuest.ShowWearObjective.
        if (objectiveDelayAfterBackpack > 0f) yield return new WaitForSeconds(objectiveDelayAfterBackpack);

        if (mission != null && !string.IsNullOrEmpty(doorObjective)) mission.SetObjective(doorObjective);
        if (doorGlow != null) doorGlow.SetGlowing(true);
    }

    private void BeginLeaving()
    {
        if (started) return;
        started = true;
        exitDoor.onOpened.RemoveListener(BeginLeaving);

        if (mission != null) mission.CompleteObjective();
        if (doorGlow != null) doorGlow.SetGlowing(false);

        StartCoroutine(LeaveRoutine());
    }

    private IEnumerator LeaveRoutine()
    {
        SetControlEnabled(false);

        if (streetSound != null)
        {
            streetSound.time = Mathf.Min(streetSoundStartTime, Mathf.Max(0f, streetSound.clip.length - 0.1f));
            streetSound.volume = streetSoundStartVolume;
            streetSound.Play();
            StartCoroutine(SwellStreetSound());
        }

        yield return FadeOverlay(0f, 1f, fadeDuration);

        yield return ShowCaption(leavingLine);
        yield return ShowCaption(timeSkipLine);
        yield return ShowCaption(arrivedLine);

        if (streetSound != null) streetSound.Stop();
        SceneManager.LoadScene(schoolSceneName);
    }

    private IEnumerator SwellStreetSound()
    {
        float t = 0f;
        while (t < streetSoundFadeUpDuration && streetSound.isPlaying)
        {
            t += Time.deltaTime;
            streetSound.volume = Mathf.Lerp(streetSoundStartVolume, streetSoundPeakVolume,
                Mathf.Clamp01(t / streetSoundFadeUpDuration));
            yield return null;
        }
    }

    private IEnumerator ShowCaption(string text)
    {
        captionText = text;
        revealedChars = 0;

        float typed = 0f;
        while (revealedChars < text.Length)
        {
            typed += typeSpeed * Time.deltaTime;
            revealedChars = Mathf.Min(text.Length, Mathf.FloorToInt(typed));
            yield return null;
        }

        yield return new WaitForSeconds(captionHold);
    }

    private IEnumerator FadeOverlay(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            overlayAlpha = Mathf.Lerp(from, to, Mathf.Clamp01(t / duration));
            yield return null;
        }
        overlayAlpha = to;
    }

    private void SetControlEnabled(bool enabled)
    {
        if (disableDuringSequence == null) return;
        foreach (var b in disableDuringSequence)
        {
            if (b != null) b.enabled = enabled;
        }
    }

    private void OnGUI()
    {
        if (overlayAlpha <= 0.001f) return;

        EnsureStyle();

        Color previous = GUI.color;

        GUI.color = new Color(0f, 0f, 0f, overlayAlpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), solid);

        if (!string.IsNullOrEmpty(captionText))
        {
            string shown = captionText.Substring(0, Mathf.Clamp(revealedChars, 0, captionText.Length));
            GUI.color = new Color(1f, 1f, 1f, overlayAlpha);
            GUI.Label(new Rect(0, Screen.height * 0.5f - 40f, Screen.width, 80f), shown, captionStyle);
        }

        GUI.color = previous;
    }

    private void EnsureStyle()
    {
        if (captionStyle != null) return;

        captionStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = captionFontSize,
            alignment = TextAnchor.MiddleCenter
        };
        captionStyle.normal.textColor = Color.white;
    }

    private static Texture2D BuildSolidTexture()
    {
        var tex = new Texture2D(1, 1, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return tex;
    }
}
