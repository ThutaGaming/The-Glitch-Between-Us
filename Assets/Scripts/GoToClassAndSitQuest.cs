using UnityEngine;

/// <summary>
/// The two objectives right after talking to friends: walk to class through the barn door,
/// then sit down. Mirrors <see cref="SchoolFriendsQuest"/>'s shape — glow the target, show the
/// objective, wait for the matching event, tick it off, move to the next one.
///
/// Hook <see cref="Begin"/> to <see cref="SchoolFriendsQuest.onCompleted"/> in the Inspector.
/// All strings are English: Unity's IMGUI/TextMeshPro cannot shape Burmese correctly.
/// </summary>
public class GoToClassAndSitQuest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MissionHUD mission;

    [Header("Step 1: go to class")]
    [SerializeField] private string goToClassObjective = "You can go to your classroom now";
    [SerializeField] private BarnDoorSlider classDoor;
    [SerializeField] private ObjectiveGlow classDoorGlow;

    [Header("Step 2: sit down")]
    [SerializeField] private string sitObjective = "Sit on the chair";
    [SerializeField] private SittableChair classChair;
    [SerializeField] private ObjectiveGlow classChairGlow;

    private bool started;
    private bool doorOpened;
    private bool seated;

    private void Awake()
    {
        if (mission == null) mission = MissionHUD.Instance != null
            ? MissionHUD.Instance
            : FindFirstObjectByType<MissionHUD>();

        if (classDoor != null) classDoor.onOpened.AddListener(OnDoorOpened);
        if (classChair != null) classChair.onSat += OnSeated;
    }

    private void OnDestroy()
    {
        if (classDoor != null) classDoor.onOpened.RemoveListener(OnDoorOpened);
        if (classChair != null) classChair.onSat -= OnSeated;
    }

    /// <summary>Shows the "go to class" objective and lights up the door. Hook to SchoolFriendsQuest.onCompleted.</summary>
    public void Begin()
    {
        if (started) return;
        started = true;

        if (mission != null && !string.IsNullOrEmpty(goToClassObjective)) mission.SetObjective(goToClassObjective);
        if (classDoorGlow != null) classDoorGlow.SetGlowing(true);
    }

    private void OnDoorOpened()
    {
        if (!started || doorOpened) return;
        doorOpened = true;

        if (mission != null) mission.CompleteObjective();
        if (classDoorGlow != null) classDoorGlow.SetGlowing(false);

        if (mission != null && !string.IsNullOrEmpty(sitObjective)) mission.SetObjective(sitObjective);
        if (classChairGlow != null) classChairGlow.SetGlowing(true);
    }

    private void OnSeated()
    {
        if (!doorOpened || seated) return;
        seated = true;

        if (mission != null) mission.CompleteObjective();
        if (classChairGlow != null) classChairGlow.SetGlowing(false);
    }
}
