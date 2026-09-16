using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Presents several CombatEncounterManager2 encounters as one battle. A room that spans two floors
/// needs an encounter per floor, because CoverNavGrid is a single-height grid - but the player
/// should still see one objective, counting every hostile on both floors, that only ticks off once
/// the last of them is down.
///
/// Wire each member encounter's onCleared to <see cref="NotifyCleared"/>, turn their own
/// drivesMissionHud off, and call <see cref="Begin"/> from the same trigger that starts them.
/// </summary>
public class EncounterGroup : MonoBehaviour
{
    [SerializeField] private MissionHUD mission;
    [SerializeField] private string objective = "Eliminate all hostiles";
    [Tooltip("How many encounters have to report in before the objective is complete.")]
    [SerializeField] private int encounterCount = 2;

    public UnityEvent onAllCleared;

    private bool begun;
    private int cleared;

    /// <summary>Shows the shared objective; safe to call repeatedly.</summary>
    public void Begin()
    {
        if (begun) return;
        begun = true;
        if (mission != null && !string.IsNullOrEmpty(objective)) mission.SetObjective(objective);
    }

    /// <summary>Called by each member encounter's onCleared.</summary>
    public void NotifyCleared()
    {
        cleared++;
        if (cleared < encounterCount) return;

        if (mission != null && mission.HasActiveObjective) mission.CompleteObjective();
        onAllCleared?.Invoke();
    }
}
