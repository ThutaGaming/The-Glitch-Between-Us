using UnityEngine;

/// <summary>
/// IInteractable wall terminal. Dormant (no prompt, invisible to PlayerInteractor) until
/// <see cref="Arm"/> is called - wire it to the intro briefing hologram's onFinished event.
/// Once armed it shows the objective in MissionHUD and lights up with the same blue
/// ObjectiveGlow rim used by every other mission marker in this project. Interacting opens
/// the paired DoubleSlidingDoor once (ScriptedOpen, so it isn't affected by the door's own
/// Locked flag).
/// </summary>
public class WallTerminalDoorSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private ObjectiveGlow glow;
    [SerializeField] private MissionHUD mission;
    [SerializeField] private DoubleSlidingDoor door;
    [SerializeField] private string objectiveText = "Head to the terminal room to turn on the first light";
    [SerializeField] private string prompt = "(E) Activate Terminal";

    private bool armed;
    private bool used;

    public Transform InteractTransform => armed && !used ? transform : null;

    // DoubleSlidingDoor.Locked isn't a serialized field (other scripts in this project toggle it
    // at runtime too, e.g. CombatEncounterManager2), so it always resets to false on scene load -
    // lock it here to stop the player opening the door directly before using this terminal.
    private void Awake()
    {
        if (door != null) door.Locked = true;
    }

    private void OnEnable() => InteractableRegistry.All.Add(this);
    private void OnDisable() => InteractableRegistry.All.Remove(this);

    public string GetPrompt() => armed && !used ? prompt : null;

    public void Interact(GameObject player)
    {
        if (!armed || used) return;
        used = true;

        if (glow != null) glow.SetGlowing(false);
        if (mission != null && mission.HasActiveObjective) mission.CompleteObjective();
        if (door != null) door.ScriptedOpen();
    }

    /// <summary>Called once the intro briefing hologram is dismissed: shows the objective and lights the terminal.</summary>
    public void Arm()
    {
        if (armed) return;
        armed = true;

        if (mission != null && !string.IsNullOrEmpty(objectiveText)) mission.SetObjective(objectiveText);
        if (glow != null) glow.SetGlowing(true);
    }
}
