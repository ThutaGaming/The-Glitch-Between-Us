using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The homecoming beat: right after the player lands back in Bedroom Scene from school, Thuta
/// says he's worn out from class and wants to play some games, then "Sit in the chair in front
/// of the laptop" becomes the objective and office_chair lights up. Same shape as
/// GameIntroSequence's morning beat (dialogue -> objective + glow), just triggered by
/// SchoolHomeSequence's scene-load handoff instead of playOnStart.
///
/// Lines are English because Unity's IMGUI/TextMeshPro cannot shape Burmese correctly.
/// </summary>
public class HomeArrivalSequence : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueHUD dialogue;
    [SerializeField] private MissionHUD mission;

    [Header("Speaker")]
    [SerializeField] private string speaker = "Thuta";

    [Header("Lines")]
    [SerializeField]
    private string[] lines =
    {
        "All that schoolwork wore me out.",
        "Now that I'm home, I'll play some games for a bit."
    };
    [SerializeField] private float holdPerLine = 1.8f;
    [SerializeField] private float startDelay = 0.5f;

    [Header("Objective")]
    [SerializeField] private string objective = "Sit in the chair in front of the laptop";
    [Tooltip("Objective appears this long after the last line starts typing.")]
    [SerializeField] private float objectiveDelay = 1.0f;
    [SerializeField] private ObjectiveGlow objectiveGlow;
    [Tooltip("Unblocked alongside the objective - the chair starts SittableChair.isSitBlocked " +
             "true so the player can't use it during the morning routine/school day, only once " +
             "they're actually back home.")]
    [SerializeField] private SittableChair chair;

    [Tooltip("Fires once this beat has finished - hook a follow-up quest to it.")]
    public UnityEvent onFinished;

    private bool hasPlayed;

    /// <summary>Plays the beat once; safe to call again (later calls are ignored).</summary>
    public void Begin()
    {
        if (hasPlayed) return;
        hasPlayed = true;
        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        ResolveReferences();

        if (startDelay > 0f) yield return new WaitForSeconds(startDelay);

        if (dialogue != null)
        {
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line)) dialogue.Say(speaker, line, holdPerLine);
            }
        }

        if (objectiveDelay > 0f) yield return new WaitForSeconds(objectiveDelay);

        if (mission != null && !string.IsNullOrEmpty(objective)) mission.SetObjective(objective);
        if (objectiveGlow != null) objectiveGlow.SetGlowing(true);
        if (chair != null)
        {
            chair.SetSitBlocked(false);
            chair.onSat += HandleSat;
        }

        onFinished?.Invoke();
    }

    /// <summary>Ticks the "sit in the chair" objective off and kills its glow the moment the
    /// player actually sits - nothing else was doing this, so the mission panel used to sit
    /// there forever showing "[ ]" even after the player was already seated and using the
    /// computer.</summary>
    private void HandleSat()
    {
        if (chair != null) chair.onSat -= HandleSat;
        if (mission != null) mission.CompleteObjective();
        if (objectiveGlow != null) objectiveGlow.SetGlowing(false);
    }

    private void OnDestroy()
    {
        if (chair != null) chair.onSat -= HandleSat;
    }

    private void ResolveReferences()
    {
        if (dialogue == null) dialogue = DialogueHUD.Instance != null
            ? DialogueHUD.Instance
            : FindFirstObjectByType<DialogueHUD>();

        if (mission == null) mission = MissionHUD.Instance != null
            ? MissionHUD.Instance
            : FindFirstObjectByType<MissionHUD>();
    }
}
