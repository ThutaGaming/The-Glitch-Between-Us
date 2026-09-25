using System;
using UnityEngine;

/// <summary>
/// Mechanic 1 - Health and a separate regenerating Energy Shield for the Spider-Mech.
///
/// The shield is the boss-side answer to the player's own auto-regen: if the boss goes
/// <see cref="shieldRegenDelay"/> seconds without being hurt, the shield climbs back at
/// <see cref="shieldRegenPercentPerSecond"/>% of its max per second, so hiding to heal costs the
/// player real progress. Body and leg-joint hits are absorbed by the shield first; the Ultimate's
/// exposed core is unshielded (it vents the shield to charge), which is what makes the DPS check
/// the fastest way through the fight.
///
/// Default values (see the balancing notes in the setup guide):
///   Health 4500 + Shield 1500 = 6000 effective HP  ->  ~3-4.5 min for an average player.
///   Shield regen 8%/s after 5s  ->  a fully drained shield is back in 5 + 12.5 = 17.5s of stalling.
/// </summary>
public class BossHealthManager : MonoBehaviour
{
    public enum HitKind { Body, WeakPoint, Core, Environment }

    [Header("Health")]
    [SerializeField] private int maxHealth = 4500;

    [Header("Energy shield (anti-stalling)")]
    [SerializeField] private int maxShield = 1500;
    [Tooltip("Seconds without taking ANY damage before the shield starts regenerating.")]
    [SerializeField] private float shieldRegenDelay = 5f;
    [Tooltip("Percent of max shield restored per second once regen kicks in.")]
    [SerializeField, Range(0f, 50f)] private float shieldRegenPercentPerSecond = 8f;

    private float currentHealth;
    private float currentShield;
    private float lastDamageTime = -999f;
    private bool regenerating;

    public int MaxHealth => maxHealth;
    public int MaxShield => maxShield;
    public float CurrentHealth => currentHealth;
    public float CurrentShield => currentShield;
    public float HealthNormalized => maxHealth <= 0 ? 0f : currentHealth / maxHealth;
    public float ShieldNormalized => maxShield <= 0 ? 0f : currentShield / maxShield;
    public bool ShieldUp => currentShield > 0.5f;
    public bool IsDead { get; private set; }
    public bool IsShieldRegenerating => regenerating;
    public float ShieldRegenDelay => shieldRegenDelay;
    public float TimeSinceLastDamage => Time.time - lastDamageTime;

    /// <summary>Set by the AI during intro/phase-shift beats so nothing can be chipped mid-cutscene.</summary>
    public bool Invulnerable { get; set; }

    /// <summary>Set above 1 by the AI during its Stunned (vulnerable) window.</summary>
    public float IncomingDamageMultiplier { get; set; } = 1f;

    /// <summary>(final damage, world point, kind, absorbed by shield)</summary>
    public event Action<float, Vector3, HitKind, bool> Damaged;
    public event Action ShieldBroken;
    public event Action ShieldRegenStarted;
    public event Action Died;

    private void Awake()
    {
        currentHealth = maxHealth;
        currentShield = maxShield;
    }

    private void Update()
    {
        if (IsDead || maxShield <= 0) return;

        bool shouldRegen = currentShield < maxShield && Time.time - lastDamageTime >= shieldRegenDelay;
        if (shouldRegen && !regenerating) ShieldRegenStarted?.Invoke();
        regenerating = shouldRegen;

        if (regenerating)
            currentShield = Mathf.Min(maxShield, currentShield + maxShield * shieldRegenPercentPerSecond * 0.01f * Time.deltaTime);
    }

    /// <summary>Returns the damage actually dealt (after multipliers), 0 if ignored.</summary>
    public float ApplyDamage(float amount, Vector3 point, HitKind kind)
    {
        if (IsDead || Invulnerable || amount <= 0f) return 0f;

        amount *= Mathf.Max(0f, IncomingDamageMultiplier);
        lastDamageTime = Time.time;
        regenerating = false;

        bool absorbed = false;
        if (kind != HitKind.Core && currentShield > 0f)
        {
            absorbed = true;
            float soak = Mathf.Min(currentShield, amount);
            currentShield -= soak;
            float spill = amount - soak;
            if (currentShield <= 0.01f)
            {
                currentShield = 0f;
                ShieldBroken?.Invoke();
            }
            currentHealth = Mathf.Max(0f, currentHealth - spill);
        }
        else
        {
            currentHealth = Mathf.Max(0f, currentHealth - amount);
        }

        Damaged?.Invoke(amount, point, kind, absorbed);

        if (currentHealth <= 0f && !IsDead)
        {
            IsDead = true;
            Died?.Invoke();
        }
        return amount;
    }

    /// <summary>Instant kill that ignores shield, invulnerability and multipliers (debug shortcut).</summary>
    public void Kill()
    {
        if (IsDead) return;
        currentShield = 0f;
        currentHealth = 0f;
        regenerating = false;
        IsDead = true;
        Died?.Invoke();
    }
}
