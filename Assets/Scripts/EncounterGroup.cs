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
/// Fill <see cref="members"/> too and the objective shows a live kill counter across all of them
/// (e.g. Level 2's four floors, which start one by one as the player climbs).
/// </summary>
public class EncounterGroup : MonoBehaviour
{
    [SerializeField] private MissionHUD mission;
    [SerializeField] private string objective = "Eliminate all hostiles";
    [Tooltip("How many encounters have to report in before the objective is complete.")]
    [SerializeField] private int encounterCount = 2;
    [Tooltip("Optional: the member encounters, for a kill counter on the objective.")]
    [SerializeField] private CombatEncounterManager2[] members;

    public UnityEvent onAllCleared;

    private bool begun;
    private bool done;
    private int cleared;
    private int shownKills = -1;

    /// <summary>Shows the shared objective; safe to call repeatedly.</summary>
    public void Begin()
    {
        if (begun) return;
        begun = true;
        if (mission == null || string.IsNullOrEmpty(objective)) return;

        int total = 0;
        if (members != null)
            foreach (var m in members) if (m != null) total += m.PlannedTargets;
        mission.SetObjective(objective, total);
    }

    private void Update()
    {
        if (!begun || done || mission == null || members == null || members.Length == 0) return;

        int kills = 0;
        foreach (var m in members) if (m != null) kills += m.EliminatedCount;
        if (kills == shownKills) return;
        shownKills = kills;
        mission.SetProgress(kills);
    }

    /// <summary>Called by each member encounter's onCleared.</summary>
    public void NotifyCleared()
    {
        cleared++;
        if (cleared < encounterCount || done) return;
        done = true;

        if (mission != null && mission.HasActiveObjective) mission.CompleteObjective();
        onAllCleared?.Invoke();
    }
}
