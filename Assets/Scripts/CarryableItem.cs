using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An object the player can pick up with E and carry in view (see <see cref="HeldItemView"/>).
/// Used for the two schoolbooks. Only one item can be carried at a time.
/// </summary>
public class CarryableItem : MonoBehaviour, IInteractable
{
    [Header("Prompt")]
    [SerializeField] private string itemName = "book";
    [SerializeField] private string pickUpPrompt = "(E) Pick up the book";
    [Tooltip("Shown when hands are already full.")]
    [SerializeField] private string handsFullPrompt = "";

    [Header("Interaction")]
    [Tooltip("Optional. Prompt only appears once this is true - e.g. after the wash-face step.")]
    [SerializeField] private bool available = true;

    public UnityEvent onPickedUp;

    private bool isStored;

    public string ItemName => itemName;
    public bool IsStored => isStored;

    public Transform InteractTransform => transform;

    private void OnEnable() => InteractableRegistry.All.Add(this);
    private void OnDisable() => InteractableRegistry.All.Remove(this);

    /// <summary>Gates the prompt until the quest reaches the packing step.</summary>
    public void SetAvailable(bool value) => available = value;

    /// <summary>Marks the item as gone for good (it is in the bag now).</summary>
    public void MarkStored() => isStored = true;

    public string GetPrompt()
    {
        if (!available || isStored) return "";
        if (HeldItemView.Instance == null) return "";
        if (HeldItemView.Instance.IsHolding)
            return HeldItemView.Instance.HeldItem == transform ? "" : handsFullPrompt;
        return pickUpPrompt;
    }

    public void Interact(GameObject player)
    {
        if (!available || isStored) return;
        if (HeldItemView.Instance == null || HeldItemView.Instance.IsHolding) return;

        if (HeldItemView.Instance.Hold(transform)) onPickedUp?.Invoke();
    }
}
