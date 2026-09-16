using UnityEngine;

/// <summary>
/// IInteractable sci-fi double door: two SlidingPanel leaves that slide apart on interact and
/// don't close again once the player has opened them (matches the one-way entrance beats used
/// elsewhere in this project). Scripted beats can also open/close it without using up that
/// one-way open, and lock it so E does nothing until the beat is over.
/// </summary>
public class DoubleSlidingDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private SlidingPanel leftPanel;
    [SerializeField] private SlidingPanel rightPanel;
    [SerializeField] private string prompt = "(E) Open Door";
    [SerializeField] private string lockedPrompt = "LOCKED - Clear the area";

    private bool opened;

    public bool Locked { get; set; }
    public bool IsOpen => opened;

    public Transform InteractTransform => opened ? null : transform;

    private void OnEnable() => InteractableRegistry.All.Add(this);
    private void OnDisable() => InteractableRegistry.All.Remove(this);

    public string GetPrompt() => opened ? null : (Locked ? lockedPrompt : prompt);

    public void Interact(GameObject player)
    {
        if (opened || Locked) return;
        opened = true;
        SetPanels(true);
    }

    public void ScriptedOpen()
    {
        if (!opened) SetPanels(true);
    }

    public void ScriptedClose()
    {
        SetPanels(false);
        opened = false;
    }

    private void SetPanels(bool open)
    {
        if (leftPanel != null)
        {
            if (open) leftPanel.Open();
            else leftPanel.Close();
        }
        if (rightPanel != null)
        {
            if (open) rightPanel.Open();
            else rightPanel.Close();
        }
    }
}
