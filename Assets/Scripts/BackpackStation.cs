using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The schoolbag. Press E while carrying a book to drop it in; once every book is packed,
/// press E again to put the bag on. Wearing it hides the bag from the world — this is a
/// first-person game with no visible body, so a worn backpack simply isn't in view.
/// </summary>
public class BackpackStation : MonoBehaviour, IInteractable
{
    [Header("Packing")]
    [SerializeField] private int booksRequired = 2;

    [Header("Prompts")]
    [SerializeField] private string storePrompt = "(E) Put the book in the bag";
    [SerializeField] private string needBooksPrompt = "Bring your books here";
    [SerializeField] private string wearPrompt = "(E) Put on the backpack";

    [Header("Interaction")]
    [SerializeField] private bool available = true;

    [Header("Events")]
    public UnityEvent<int> onBookStored;
    public UnityEvent onAllBooksStored;
    public UnityEvent onWorn;

    private int booksStored;
    private bool isWorn;

    public int BooksStored => booksStored;
    public int BooksRequired => booksRequired;
    public bool IsFull => booksStored >= booksRequired;
    public bool IsWorn => isWorn;

    public Transform InteractTransform => transform;

    private void OnEnable() => InteractableRegistry.All.Add(this);
    private void OnDisable() => InteractableRegistry.All.Remove(this);

    public void SetAvailable(bool value) => available = value;

    public string GetPrompt()
    {
        if (!available || isWorn) return "";

        bool holdingBook = HeldItemView.Instance != null
            && HeldItemView.Instance.IsHolding
            && HeldItemView.Instance.HeldItem.GetComponent<CarryableItem>() != null;

        if (holdingBook && !IsFull) return storePrompt;
        if (IsFull) return wearPrompt;
        return needBooksPrompt;
    }

    public void Interact(GameObject player)
    {
        if (!available || isWorn) return;

        var view = HeldItemView.Instance;
        bool holdingBook = view != null && view.IsHolding
            && view.HeldItem.GetComponent<CarryableItem>() != null;

        if (holdingBook && !IsFull)
        {
            var book = view.HeldItem.GetComponent<CarryableItem>();
            view.Consume();
            book.MarkStored();

            booksStored++;
            onBookStored?.Invoke(booksStored);
            if (IsFull) onAllBooksStored?.Invoke();
            return;
        }

        if (IsFull) Wear();
    }

    private void Wear()
    {
        isWorn = true;
        // On the player's back, so out of sight in first person.
        gameObject.SetActive(false);
        onWorn?.Invoke();
    }
}
