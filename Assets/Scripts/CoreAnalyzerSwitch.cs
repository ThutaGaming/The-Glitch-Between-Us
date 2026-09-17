using UnityEngine;

/// <summary>
/// IInteractable on one of the two Retro_laboratory_Analyzer_DoubleScreen consoles in the Core
/// Room. Dormant (no prompt, invisible to PlayerInteractor) until <see cref="Arm"/> is called -
/// wire the pair to CoreAnalyzerPuzzle.Arm, which itself is wired to
/// CoreRoom_EncounterGroup.onAllCleared. Once armed it lights up with the same blue ObjectiveGlow
/// rim every other mission marker in this project uses. Interacting reports back to the shared
/// CoreAnalyzerPuzzle instead of tracking its own objective text, since the HUD shows one combined
/// "(1/2)" counter for the pair.
/// </summary>
public class CoreAnalyzerSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private ObjectiveGlow glow;
    [SerializeField] private CoreAnalyzerPuzzle puzzle;
    [SerializeField] private string prompt = "(E) Activate Analyzer Terminal";

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
        if (puzzle != null) puzzle.NotifyActivated();
    }

    /// <summary>Called once the Core Room is cleared: shows the prompt and lights this terminal.</summary>
    public void Arm()
    {
        if (armed) return;
        armed = true;

        if (glow != null) glow.SetGlowing(true);
    }
}
