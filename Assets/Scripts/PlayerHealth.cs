using System;
using UnityEngine;

public sealed class PlayerHealth : MonoBehaviour
{
    [Min(1)] public int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;
    [Tooltip("Seconds without taking damage before health starts coming back.")]
    [SerializeField] private float regenDelay = 4f;
    [SerializeField] private float regenPerSecond = 12f;

    private float regenAccumulator;
    private float dotAccumulator;
    private float regenBlockedUntil = -999f;

    public int CurrentHealth => currentHealth;
    public float Normalized => maxHealth <= 0 ? 0f : (float)currentHealth / maxHealth;
    public bool IsDead { get; private set; }
    public float LastDamageTime { get; private set; } = -999f;

    /// <summary>True while a hazard (AoE cloud, electrified floor) is holding regeneration off.</summary>
    public bool RegenSuppressed => Time.time < regenBlockedUntil;

    /// <summary>True when health is actually ticking back up - the HUD uses this for its regen pip.</summary>
    public bool IsRegenerating => !IsDead && currentHealth < maxHealth
        && !RegenSuppressed && Time.time - LastDamageTime >= regenDelay;

    public event Action Died;
    /// <summary>Fires on every damage application, with the damage actually dealt.</summary>
    public event Action<int> Damaged;

    private void Awake() => currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

    private void Update()
    {
        if (IsDead || currentHealth >= maxHealth) return;
        if (RegenSuppressed || Time.time - LastDamageTime < regenDelay) return;

        regenAccumulator += regenPerSecond * Time.deltaTime;
        int whole = Mathf.FloorToInt(regenAccumulator);
        if (whole <= 0) return;
        regenAccumulator -= whole;
        Heal(whole);
    }

    /// <summary>
    /// Holds regeneration off for a short window. Hazards call this every frame the player is
    /// standing in them, so the block lifts by itself the moment the player leaves (or the hazard
    /// is destroyed) - no paired "un-suppress" call to leak.
    /// </summary>
    public void SuppressRegen(float seconds)
    {
        float until = Time.time + Mathf.Max(0f, seconds);
        if (until > regenBlockedUntil) regenBlockedUntil = until;
    }

    /// <summary>
    /// Fractional per-second damage for hazards. Accumulates sub-1 amounts instead of rounding
    /// them away, so an 8 HP/s cloud really does 8 HP/s rather than nothing at 60fps.
    /// </summary>
    public void ApplyDamageOverTime(float damagePerSecond)
    {
        if (IsDead || damagePerSecond <= 0f) return;

        dotAccumulator += damagePerSecond * Time.deltaTime;
        int whole = Mathf.FloorToInt(dotAccumulator);
        if (whole <= 0)
        {
            // Still counts as "being hurt" so the regen delay keeps resetting between ticks.
            LastDamageTime = Time.time;
            return;
        }
        dotAccumulator -= whole;
        ApplyDamage(whole);
    }

    public void ApplyDamage(int amount)
    {
        if (IsDead || amount <= 0) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        LastDamageTime = Time.time;
        regenAccumulator = 0f;
        Damaged?.Invoke(amount);

        if (currentHealth == 0)
        {
            IsDead = true;
            Died?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        currentHealth = Mathf.Clamp(currentHealth + Mathf.Max(0, amount), 0, maxHealth);
    }
}
