using System.Collections;
using UnityEngine;

/// <summary>
/// Sits just past a DoubleSlidingDoor, on the far side. Once the player crosses into it, waits a
/// short delay then closes the door behind them - unlike the project's usual one-way doors, this
/// one is meant to be walked through and shut again (and stays interactable afterwards, since
/// DoubleSlidingDoor.ScriptedClose resets its "opened" flag).
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class DoorAutoCloseZone : MonoBehaviour
{
    [SerializeField] private DoubleSlidingDoor door;
    [SerializeField] private float closeDelay = 2f;
    [Tooltip("Optional - notified once when the player passes through, before the close delay.")]
    [SerializeField] private CombatEncounterManager2 encounter;
    [Tooltip("Optional - the next room's encounter, started the first time the player walks in.")]
    [SerializeField] private CombatEncounterManager2 encounterToStart;

    private bool pending;

    private void Reset()
    {
        var col = GetComponent<BoxCollider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pending || door == null) return;
        if (other.GetComponentInParent<PlayerHealth>() == null) return;

        pending = true;
        if (encounter != null) encounter.NotifyPlayerThroughDoor();
        if (encounterToStart != null) encounterToStart.Begin();
        StartCoroutine(CloseAfterDelay());
    }

    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(closeDelay);
        door.ScriptedClose();
        pending = false;
    }
}
