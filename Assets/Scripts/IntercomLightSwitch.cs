using UnityEngine;

/// <summary>
/// IInteractable on Intercom_01. Dormant (no prompt, invisible to PlayerInteractor) until
/// <see cref="Arm"/> is called - wire it to CombatEncounterManager2.onCleared, which fires once
/// every hostile in the room (including the turret) is down. Once armed it shows the objective in
/// MissionHUD and lights up with the same blue ObjectiveGlow rim every other mission marker in
/// this project uses. Interacting replays the door-control camera shot via IntercomCutscene,
/// which turns one of the two indicator lights green.
/// </summary>
public class IntercomLightSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private ObjectiveGlow glow;
    [SerializeField] private MissionHUD mission;
    [SerializeField] private IntercomCutscene cutscene;
    [SerializeField] private string objectiveText = "Head to the intercom to turn on the light";
    [SerializeField] private string prompt = "(E) Activate Intercom";

    private bool armed;
    private bool used;

    public Transform InteractTransform => armed && !used ? transform : null;

    private void OnEnable() => InteractableRegistry.All.Add(this);
    private void OnDisable() => InteractableRegistry.All.Remove(this);

    public string GetPrompt() => armed && !used ? prompt : null;

    public void Interact(GameObject player)
    {
        if (!armed || used) return;
        used = true;

        if (glow != null) glow.SetGlowing(false);
        if (mission != null && mission.HasActiveObjective) mission.CompleteObjective();
        if (cutscene != null) cutscene.Play();
    }

    /// <summary>
    /// Test-only shortcut for the F8 debug skip: applies this switch's end state instantly,
    /// skipping the multi-second cutscene camera cut IntercomCutscene.Play() would otherwise play.
    /// </summary>
    public void DebugSkip()
    {
        if (!armed || used) return;
        used = true;

        if (glow != null) glow.SetGlowing(false);
        if (mission != null && mission.HasActiveObjective) mission.CompleteObjective();
        if (cutscene != null) cutscene.DebugSkip();
    }

    /// <summary>Called once the room is cleared: shows the objective and lights the intercom.</summary>
    public void Arm()
    {
        if (armed) return;
        armed = true;

        if (mission != null && !string.IsNullOrEmpty(objectiveText)) mission.SetObjective(objectiveText);
        if (glow != null) glow.SetGlowing(true);
    }
}
