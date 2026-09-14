using UnityEngine;

/// <summary>Fires CombatEncounterManager.StartWave1() once, the moment the player enters this trigger volume.</summary>
[RequireComponent(typeof(Collider))]
public class CombatTriggerZone : MonoBehaviour
{
    [SerializeField] private CombatEncounterManager manager;
    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player") && other.GetComponentInParent<InfimaGames.LowPolyShooterPack.CharacterBehaviour>() == null) return;

        triggered = true;
        if (manager != null) manager.StartWave1();
    }
}
