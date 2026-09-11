using UnityEngine;

/// <summary>
/// IInteractable on Retro_laboratory_Wall_Terminal_Thin. Sits dormant (no prompt, invisible to
/// PlayerInteractor) until <see cref="Arm"/> is called - wire EscapeProtocolHologram.onFinished
/// to it. Once armed it shows the objective in MissionHUD and lights up with the same blue
/// ObjectiveGlow rim used by every other mission marker in this project. Interacting starts
/// BasicTrainingRange once.
/// </summary>
public class TrainingTerminal : MonoBehaviour, IInteractable
{
    [SerializeField] private ObjectiveGlow glow;
    [SerializeField] private MissionHUD mission;
    [SerializeField] private BasicTrainingRange range;
    [SerializeField] private string objectiveText = "Basic Training";
    [SerializeField] private string prompt = "(E) Begin Basic Training";

    private bool armed;
    private bool used;

    // Null while dormant, so PlayerInteractor's nearest-search skips this entirely instead of
    // winning the "nearest" slot with an empty prompt.
    public Transform InteractTransform => armed && !used ? transform : null;

    private void OnEnable() => InteractableRegistry.All.Add(this);
    private void OnDisable() => InteractableRegistry.All.Remove(this);

    public string GetPrompt() => armed && !used ? prompt : null;

    public void Interact(GameObject player)
    {
        if (!armed || used) return;
        used = true;

        if (glow != null) glow.SetGlowing(false);
        if (range != null) range.Begin();
    }

    /// <summary>Called once the escape hologram is dismissed: shows the objective and lights the terminal.</summary>
    public void Arm()
    {
        if (armed) return;
        armed = true;

        if (mission != null && !string.IsNullOrEmpty(objectiveText)) mission.SetObjective(objectiveText);
        if (glow != null) glow.SetGlowing(true);
    }
}
