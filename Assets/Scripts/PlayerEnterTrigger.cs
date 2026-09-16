using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Fires once, the first time the player walks into this trigger volume. The room-entry
/// counterpart to DoorAutoCloseZone for beats that have nothing to do with a door - starting an
/// ambush, arming an objective, cueing a line.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class PlayerEnterTrigger : MonoBehaviour
{
    public UnityEvent onPlayerEntered;

    private bool fired;

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fired) return;
        if (other.GetComponentInParent<PlayerHealth>() == null) return;

        fired = true;
        onPlayerEntered?.Invoke();
    }
}
