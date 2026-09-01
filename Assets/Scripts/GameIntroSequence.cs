using System.Collections;
using UnityEngine;

/// <summary>
/// Opening beat: Thuta wakes at 8:00, realises school starts at nine, and the "wash your face"
/// objective appears in the corner.
///
/// Call <see cref="Begin"/> from PlayerWakeUpSequence.onWakeUpComplete to run it after the
/// wake-up animation, or leave <see cref="playOnStart"/> on to run it straight away.
/// Lines are English because Unity's IMGUI/TextMeshPro cannot shape Burmese correctly.
/// </summary>
public class GameIntroSequence : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueHUD dialogue;
    [SerializeField] private MissionHUD mission;

    [Header("Start")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private float startDelay = 1.0f;

    [Header("Speaker")]
    [SerializeField] private string speaker = "Thuta";

    [Header("Lines")]
    [SerializeField]
    private string[] lines =
    {
        "Ugh... it's already 8 o'clock.",
        "School starts at nine. I can't be late again.",
        "I should wash my face first."
    };
    [SerializeField] private float holdPerLine = 1.8f;

    [Header("Objective")]
    [SerializeField] private string objective = "Wash your face";
    [Tooltip("Objective appears this long after the last line starts typing.")]
    [SerializeField] private float objectiveDelay = 3.4f;

    private bool hasPlayed;

    private void Start()
    {
        if (playOnStart) Begin();
    }

    /// <summary>Plays the intro once; safe to call again (later calls are ignored).</summary>
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
