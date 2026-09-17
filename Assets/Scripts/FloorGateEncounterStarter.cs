using UnityEngine;

/// <summary>
/// Holds a per-floor CombatEncounterManager2 (startOnPlay off) inert until the player's height
/// comes within range of that floor - so an upper-floor squad stays idle rather than opening fire
/// on someone still down on a floor below. Once triggered it calls Begin() once; the manager's own
/// guard makes repeat calls harmless.
/// </summary>
public class FloorGateEncounterStarter : MonoBehaviour
{
    [SerializeField] private CombatEncounterManager2 manager;
    [SerializeField] private float floorY;
    [SerializeField] private float heightTolerance = 3.5f;

    private Transform player;

    private void Awake()
    {
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null) player = playerGo.transform;
    }

    private void Update()
    {
        if (manager == null || player == null) return;
        if (Mathf.Abs(player.position.y - floorY) > heightTolerance) return;

        manager.Begin();
        enabled = false;
    }
}
