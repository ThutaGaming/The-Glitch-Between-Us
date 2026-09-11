using UnityEngine;

/// <summary>
/// Fired once the "head to Floor 1" hologram is dismissed (wire to EscapeProtocolHologram.onFinished
/// on that second hologram instance): slides Grid_06 open, updates the MissionHUD objective, and
/// lights up the stairwell's EndDoor with the same blue ObjectiveGlow rim every other mission
/// marker in this project uses.
/// </summary>
public class FloorOneUnlock : MonoBehaviour
{
    [SerializeField] private SlidingPanel gridPanel;
    [SerializeField] private MissionHUD mission;
    [SerializeField] private ObjectiveGlow endDoorGlow;
    [SerializeField] private string objectiveText = "Head to Floor 1";

    private bool unlocked;

    /// <summary>Runs the unlock sequence once; safe to call again (later calls are ignored).</summary>
    public void Unlock()
    {
        if (unlocked) return;
        unlocked = true;

        if (gridPanel != null) gridPanel.Open();
        if (mission != null && !string.IsNullOrEmpty(objectiveText)) mission.SetObjective(objectiveText);
        if (endDoorGlow != null) endDoorGlow.SetGlowing(true);
    }
}
