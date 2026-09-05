using System.Collections;
using UnityEngine;

/// <summary>
/// Drives the morning chain after the face is washed: pack both schoolbooks into the bag,
/// then put the bag on. Listeners are hooked up in code rather than through Inspector events,
/// so the whole sequence reads top-to-bottom in one place.
///
/// All strings are English: Unity's IMGUI/TextMeshPro cannot shape Burmese correctly.
/// </summary>
public class MorningRoutineQuest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WashFaceStation washStation;
    [SerializeField] private CarryableItem[] books;
    [SerializeField] private BackpackStation backpack;
    [SerializeField] private DialogueHUD dialogue;
    [SerializeField] private MissionHUD mission;

    [Header("Speaker")]
    [SerializeField] private string speaker = "Thuta";

    [Header("Step 1 - pack the books")]
    [SerializeField] private float delayAfterWash = 1.2f;
    [SerializeField]
    private string[] afterWashLines =
    {
        "Right - my books.",
        "I'll pack them in my bag and head out."
    };
    [SerializeField] private string packObjective = "Put your books in the bag";

    [Header("Step 2 - wear the bag")]
    [SerializeField] private string wearObjective = "Put on your backpack";
    [SerializeField] private string allPackedLine = "That's both of them. Now my bag.";

    [Header("Finish")]
    [SerializeField] private string finishedLine = "Alright - I'm ready. Off to school.";

    [Header("Objective Markers")]
    [Tooltip("Faint blue marker lights, on for both the packing step and the wear-backpack step.")]
    [SerializeField] private ObjectiveGlow[] bookGlows;
    [SerializeField] private ObjectiveGlow backpackGlow;

    private bool started;

    private void Awake()
    {
        ResolveReferences();

        // Nothing in this chain is interactable until the face is washed.
        SetBooksAvailable(false);
        if (backpack != null) backpack.SetAvailable(false);

        if (washStation != null) washStation.onWashComplete.AddListener(Begin);
        if (backpack != null)
        {
            backpack.onBookStored.AddListener(OnBookStored);
            backpack.onAllBooksStored.AddListener(OnAllBooksStored);
            backpack.onWorn.AddListener(OnBackpackWorn);
        }
    }

    private void OnDestroy()
    {
        if (washStation != null) washStation.onWashComplete.RemoveListener(Begin);
        if (backpack != null)
        {
            backpack.onBookStored.RemoveListener(OnBookStored);
            backpack.onAllBooksStored.RemoveListener(OnAllBooksStored);
            backpack.onWorn.RemoveListener(OnBackpackWorn);
        }
    }

    /// <summary>Starts the packing step. Called by WashFaceStation.onWashComplete.</summary>
    public void Begin()
    {
        if (started) return;
        started = true;
        StartCoroutine(BeginRoutine());
    }

    private IEnumerator BeginRoutine()
    {
        // Let the "that's better" line from the wash finish first.
        yield return new WaitForSeconds(delayAfterWash);

        if (dialogue != null)
            foreach (string line in afterWashLines)
                if (!string.IsNullOrEmpty(line)) dialogue.Say(speaker, line);

        SetBooksAvailable(true);
        if (backpack != null) backpack.SetAvailable(true);

        if (bookGlows != null)
            foreach (var g in bookGlows) if (g != null) g.SetGlowing(true);
        if (backpackGlow != null) backpackGlow.SetGlowing(true);

        if (mission != null)
        {
            mission.SetObjective(packObjective, backpack != null ? backpack.BooksRequired : books.Length);
            mission.SetProgress(0);
        }
    }

    private void OnBookStored(int count)
    {
        if (mission != null) mission.SetProgress(count);
    }

    private void OnAllBooksStored()
    {
        if (mission != null) mission.CompleteObjective();
        if (dialogue != null && !string.IsNullOrEmpty(allPackedLine))
            dialogue.Say(speaker, allPackedLine);
        StartCoroutine(ShowWearObjective());
    }

    private IEnumerator ShowWearObjective()
    {
        // Let the tick-off on the packing objective read before the next one replaces it.
        yield return new WaitForSeconds(1.4f);
        if (mission != null) mission.SetObjective(wearObjective);
    }

    private void OnBackpackWorn()
    {
        if (mission != null) mission.CompleteObjective();
        if (dialogue != null && !string.IsNullOrEmpty(finishedLine))
            dialogue.Say(speaker, finishedLine);
    }

    private void SetBooksAvailable(bool value)
    {
        if (books == null) return;
        foreach (var b in books) if (b != null) b.SetAvailable(value);
    }

    private void ResolveReferences()
    {
        if (washStation == null) washStation = FindFirstObjectByType<WashFaceStation>();
        if (backpack == null) backpack = FindFirstObjectByType<BackpackStation>();
        if (dialogue == null) dialogue = DialogueHUD.Instance != null
            ? DialogueHUD.Instance : FindFirstObjectByType<DialogueHUD>();
        if (mission == null) mission = MissionHUD.Instance != null
            ? MissionHUD.Instance : FindFirstObjectByType<MissionHUD>();
        if (books == null || books.Length == 0) books = FindObjectsByType<CarryableItem>(FindObjectsSortMode.None);
    }
}
