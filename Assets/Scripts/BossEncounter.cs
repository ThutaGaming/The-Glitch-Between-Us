using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Runs the Spider-Mech encounter as a set piece: when the player steps into the plaza, the exits
/// seal behind them, the objective updates and the boss drops in. When the boss dies the barriers
/// drop and the objective completes.
/// </summary>
public class BossEncounter : MonoBehaviour
{
    [SerializeField] private BossAIController boss;
    [SerializeField] private Vector3 arenaCenter = new Vector3(358.5f, 2.42f, 247.5f);
    [Tooltip("The fight starts when the player gets this close (flat distance) to the arena centre.")]
    [SerializeField] private float triggerRadius = 10.5f;
    [SerializeField] private EnergyBarrier[] barriers;
    [SerializeField] private SpiderMechFxLibrary fx;

    [Header("Mission text (English - IMGUI can't shape Burmese)")]
    [SerializeField] private string fightObjective = "Destroy the Spider-Mech";
    [SerializeField] private string victoryObjective = "Spider-Mech destroyed - the way is open";
    [SerializeField] private float barrierDropDelay = 4f;

    private Transform player;
    private bool started;
    private bool finished;

    public bool Started => started;
    public bool Finished => finished;

    /// <summary>Fires once the victory objective has been shown and ticked off - the next story
    /// beat can take over the mission panel from here.</summary>
    public event Action VictoryFlowFinished;

    private void Awake()
    {
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go != null) player = go.transform;

        if (fx != null && barriers != null)
            foreach (var b in barriers)
                if (b != null) b.Configure(fx.electricLoop, fx.barrierClip);
    }

    private void Update()
    {
        if (started || boss == null || player == null) return;

        Vector3 d = player.position - arenaCenter;
        if (new Vector2(d.x, d.z).magnitude < triggerRadius) Begin();
    }

    /// <summary>Starts the fight right away, wherever the player is (debug shortcuts).</summary>
    public void StartNow()
    {
        if (!started && boss != null) Begin();
    }

    private void Begin()
    {
        started = true;
        if (barriers != null)
            foreach (var b in barriers)
                if (b != null) b.Raise();

        if (MissionHUD.Instance != null) MissionHUD.Instance.SetObjective(fightObjective);

        boss.Defeated += OnBossDefeated;
        boss.BeginEncounter();
    }

    private void OnBossDefeated()
    {
        if (finished) return;
        finished = true;
        StartCoroutine(Victory());
    }

    private IEnumerator Victory()
    {
        if (MissionHUD.Instance != null) MissionHUD.Instance.CompleteObjective();
        yield return new WaitForSeconds(barrierDropDelay);

        if (barriers != null)
            foreach (var b in barriers)
                if (b != null) b.Lower();

        if (MissionHUD.Instance != null)
        {
            MissionHUD.Instance.SetObjective(victoryObjective);
            yield return new WaitForSeconds(3f);
            MissionHUD.Instance.CompleteObjective();
        }

        VictoryFlowFinished?.Invoke();
    }

    private void OnDestroy()
    {
        if (boss != null) boss.Defeated -= OnBossDefeated;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.1f, 0.5f);
        Gizmos.DrawWireSphere(arenaCenter, triggerRadius);
    }
}
