using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// School Scene on the morning Thuta wakes at his desk after the game world, already late:
/// no arrival small talk and no friends to chat with. He says he has to get to class, "Go to
/// the classroom" lights up BarnDoor_03, and once it's opened "Sit on the chair" lights up
/// (Prb)Chair2 (04) - the same two steps <see cref="GoToClassAndSitQuest"/> runs on the first
/// visit, with this beat's own objective text.
///
/// Triggered like DeskWakeUpSequence: <see cref="QueueForNextLoad"/> before loading the scene
/// (SchoolExitSequence does it on the late trip), then a static sceneLoaded handler switches
/// off the first-visit flow before its Start() and begins this beat instead.
/// Lines are English because Unity's IMGUI/TextMeshPro cannot shape Burmese correctly.
/// </summary>
public class LateSchoolArrivalSequence : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueHUD dialogue;
    [SerializeField] private GoToClassAndSitQuest classQuest;
    [Tooltip("First-visit behaviour switched off for this load: the arrival lines, the friends " +
             "quest and its talk hubs, and Mr. Thiha's lesson that starts when you sit down.")]
    [SerializeField] private Behaviour[] firstVisitOnly;
    [Tooltip("Doors the friends quest keeps locked - unlocked once the objective appears.")]
    [SerializeField] private BarnDoorSlider[] unlockDoors;

    [Header("Arrival")]
    [SerializeField] private float startDelay = 1.2f;
    [SerializeField] private string speaker = "Thuta";
    [SerializeField] private string[] lines = { "I have to go to my classroom now." };
    [SerializeField] private float holdPerLine = 2.2f;

    [Header("Objectives")]
    [SerializeField] private string goToClassObjective = "Go to the classroom";
    [SerializeField] private string sitObjective = "Sit on the chair";

    [Header("Debug")]
    [Tooltip("Play this beat as soon as School Scene starts, without coming from the desk " +
             "wake-up - for testing only.")]
    [SerializeField] private bool debugPlayOnStart;

    /// <summary>Fires once Thuta has sat down in class - hook the next beat to it.</summary>
    public UnityEvent onSeated;

    private static bool pending;

    private bool begun;
    private bool switchedOff;

    /// <summary>Call right before loading School Scene: the next load is the late arrival.</summary>
    public static void QueueForNextLoad()
    {
        if (pending) return;
        pending = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>Static so it survives Bedroom Scene unloading. sceneLoaded runs after
    /// Awake/OnEnable but before Start, so the first-visit intro can still be switched off
    /// before it begins.</summary>
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var sequence = FindFirstObjectByType<LateSchoolArrivalSequence>(FindObjectsInactive.Include);
        if (sequence == null) return;   // not School Scene - keep waiting

        SceneManager.sceneLoaded -= OnSceneLoaded;
        pending = false;
        sequence.Begin();
    }

    private void Awake()
    {
        // Every Start() is still ahead of us here, so the intro never gets to run.
        if (debugPlayOnStart) SwitchOffFirstVisit();
    }

    private void Start()
    {
        if (debugPlayOnStart) Begin();
    }

    /// <summary>Plays the beat once; safe to call again (later calls are ignored).</summary>
    public void Begin()
    {
        if (begun) return;
        begun = true;
        SwitchOffFirstVisit();
        if (classQuest != null) classQuest.onSeated.AddListener(HandleSeated);
        StartCoroutine(ArrivalRoutine());
    }

    private void OnDestroy()
    {
        if (classQuest != null) classQuest.onSeated.RemoveListener(HandleSeated);
    }

    private void SwitchOffFirstVisit()
    {
        if (switchedOff) return;
        switchedOff = true;
        foreach (var b in firstVisitOnly)
        {
            if (b != null) b.enabled = false;
        }
    }

    private IEnumerator ArrivalRoutine()
    {
        // The friends quest locked these in its Awake with "Talk with your friends first" -
        // no prompt at all until the objective appears.
        foreach (var door in unlockDoors)
        {
            if (door != null) door.SetLockedPrompt("");
        }

        if (startDelay > 0f) yield return new WaitForSeconds(startDelay);

        if (dialogue == null) dialogue = DialogueHUD.Instance != null
            ? DialogueHUD.Instance
            : FindFirstObjectByType<DialogueHUD>();

        if (dialogue != null)
        {
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line)) dialogue.Say(speaker, line, holdPerLine);
            }
            yield return null;
            yield return new WaitWhile(() => dialogue.IsPlaying);
        }

        // Unlocked only now, so the door can't be opened before the quest is listening for it.
        foreach (var door in unlockDoors)
        {
            if (door != null) door.SetLocked(false);
        }

        if (classQuest != null) classQuest.BeginWithObjectives(goToClassObjective, sitObjective);
    }

    private void HandleSeated()
    {
        classQuest.onSeated.RemoveListener(HandleSeated);
        onSeated?.Invoke();
    }
}
