using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Test-only shortcut for Level 5: press 8 to kill the Spider-Mech main boss instantly, so the
/// death sequence, barrier drop and victory objective can be checked without playing the fight.
/// If the fight hasn't started yet, it starts it and kills the boss as soon as it has landed, so
/// the death plays on a visible boss and the encounter's victory flow still runs.
/// </summary>
public class DebugKillBoss : MonoBehaviour
{
    private BossAIController boss;
    private BossHealthManager health;
    private BossEncounter encounter;
    private bool pending;

    private void Start()
    {
        boss = FindFirstObjectByType<BossAIController>();
        if (boss != null) health = boss.GetComponent<BossHealthManager>();
        encounter = FindFirstObjectByType<BossEncounter>();
        if (boss == null || health == null)
            Debug.LogWarning("[DebugKillBoss] No Spider-Mech boss found in this scene.");
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.digit8Key.wasPressedThisFrame && !pending)
            StartCoroutine(KillRoutine());
    }

    private IEnumerator KillRoutine()
    {
        if (boss == null || health == null || health.IsDead) yield break;
        pending = true;

        if (boss.State == BossAIController.BossState.Dormant)
        {
            if (encounter != null) encounter.StartNow();
            else boss.BeginEncounter();
            Debug.Log("[DebugKillBoss] 8: fight not started yet - starting it, boss dies on landing.");
            float timeout = Time.time + 10f;
            while (!boss.IsEngaged && Time.time < timeout) yield return null;
        }

        health.Kill();
        Debug.Log("[DebugKillBoss] 8: Spider-Mech killed.");
        pending = false;
    }
}
