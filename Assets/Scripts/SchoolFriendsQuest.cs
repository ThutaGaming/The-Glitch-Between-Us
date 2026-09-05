using UnityEngine;

/// <summary>
/// The school's opening objective: talk to your friends before class. Shows the mission once
/// the arrival intro finishes, marks every talkable friend with a blue objective glow, and
/// keeps the classroom doors shut until the player has spoken to at least one of them.
/// Talking to one is enough to tick the objective off - the rest stay available to chat with.
///
/// All strings are English: Unity's IMGUI/TextMeshPro cannot shape Burmese correctly.
/// </summary>
public class SchoolFriendsQuest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MissionHUD mission;
    [Tooltip("Every talk hub in the scene; any completed conversation finishes the objective.")]
    [SerializeField] private FriendGroupConversation[] friendGroups;
    [Tooltip("Blue markers on the friends, on while the objective is active.")]
    [SerializeField] private ObjectiveGlow[] friendGlows;
    [Tooltip("Doors that stay shut until a friend has been spoken to.")]
    [SerializeField] private BarnDoorSlider[] gatedDoors;

    [Header("Objective")]
    [SerializeField] private string objective = "Talk with your friends";
    [SerializeField] private string lockedDoorPrompt = "Talk with your friends first";

    private bool started;
    private bool completed;

    private void Awake()
    {
        if (mission == null) mission = MissionHUD.Instance != null
            ? MissionHUD.Instance
            : FindFirstObjectByType<MissionHUD>();

        foreach (var door in gatedDoors)
        {
            if (door == null) continue;
            door.SetLocked(true);
            door.SetLockedPrompt(lockedDoorPrompt);
        }

        foreach (var group in friendGroups)
        {
            if (group != null) group.onFriendTalked.AddListener(OnFriendTalked);
        }
    }

    private void OnDestroy()
    {
        foreach (var group in friendGroups)
        {
            if (group != null) group.onFriendTalked.RemoveListener(OnFriendTalked);
        }
    }

    /// <summary>Shows the objective and lights the friends up. Hook to GameIntroSequence.onFinished.</summary>
    public void Begin()
    {
        if (started) return;
        started = true;

        if (mission != null && !string.IsNullOrEmpty(objective)) mission.SetObjective(objective);
        SetGlows(true);
    }

    private void OnFriendTalked()
    {
        if (completed) return;
        completed = true;

        if (mission != null) mission.CompleteObjective();
        SetGlows(false);

        foreach (var door in gatedDoors)
        {
            if (door != null) door.SetLocked(false);
        }
    }

    private void SetGlows(bool glowing)
    {
        foreach (var glow in friendGlows)
        {
            if (glow != null) glow.SetGlowing(glowing);
        }
    }
}
