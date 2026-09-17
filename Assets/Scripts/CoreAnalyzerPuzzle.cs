using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Tracks the two CoreAnalyzerSwitch terminals in the Core Room as one objective ("Activate both
/// analyzer terminals (1/2)"), the same shared-objective pattern EncounterGroup uses for a
/// multi-part fight. Wire <see cref="Arm"/> to CoreRoom_EncounterGroup.onAllCleared and
/// <see cref="onBothActivated"/> to CoreReactorFinale.Play.
/// </summary>
public class CoreAnalyzerPuzzle : MonoBehaviour
{
    [SerializeField] private MissionHUD mission;
    [SerializeField] private CoreAnalyzerSwitch[] switches;
    [SerializeField] private string objective = "Activate both analyzer terminals";

    public UnityEvent onBothActivated;

    private int activated;

    /// <summary>Called once the Core Room is cleared: shows the objective and arms both terminals.</summary>
    public void Arm()
    {
        if (mission != null && !string.IsNullOrEmpty(objective)) mission.SetObjective(objective, switches.Length);
        foreach (var s in switches) if (s != null) s.Arm();
    }

    /// <summary>Called by each CoreAnalyzerSwitch once the player interacts with it.</summary>
    public void NotifyActivated()
    {
        activated++;
        if (mission != null) mission.SetProgress(activated);
        if (activated < switches.Length) return;

        if (mission != null && mission.HasActiveObjective) mission.CompleteObjective();
        onBothActivated?.Invoke();
    }
}
