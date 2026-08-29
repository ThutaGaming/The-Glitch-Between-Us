using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implemented by anything the player can interact with via a single key (chairs, doors, ...).
/// Instances self-register so PlayerInteractor can find the nearest one without a per-frame scan.
/// </summary>
public interface IInteractable
{
    Transform InteractTransform { get; }
    string GetPrompt();
    void Interact(GameObject player);
}

public static class InteractableRegistry
{
    public static readonly List<IInteractable> All = new List<IInteractable>();
}
