using UnityEngine;

/// <summary>
/// IInteractable sci-fi double door: two SlidingPanel leaves that slide apart on interact and
/// don't close again (matches the one-way entrance beats used elsewhere in this project).
/// </summary>
public class DoubleSlidingDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private SlidingPanel leftPanel;
    [SerializeField] private SlidingPanel rightPanel;
    [SerializeField] private string prompt = "(E) Open Door";

    private bool opened;

    public Transform InteractTransform => opened ? null : transform;

    private void OnEnable() => InteractableRegistry.All.Add(this);
    private void OnDisable() => InteractableRegistry.All.Remove(this);

    public string GetPrompt() => opened ? null : prompt;

    public void Interact(GameObject player)
    {
        if (opened) return;
        opened = true;

        if (leftPanel != null) leftPanel.Open();
        if (rightPanel != null) rightPanel.Open();
    }
}
