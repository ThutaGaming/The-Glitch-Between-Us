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

    public int CurrentHealth => currentHealth;
    public float Normalized => maxHealth <= 0 ? 0f : (float)currentHealth / maxHealth;
    public bool IsDead { get; private set; }
    public float LastDamageTime { get; private set; } = -999f;

    public event Action Died;

    private void Awake() => currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

    private void Update()
    {
        if (IsDead || currentHealth >= maxHealth || Time.time - LastDamageTime < regenDelay) return;

        regenAccumulator += regenPerSecond * Time.deltaTime;
        int whole = Mathf.FloorToInt(regenAccumulator);
        if (whole <= 0) return;
        regenAccumulator -= whole;
        Heal(whole);
    }

    public void ApplyDamage(int amount)
    {
        if (IsDead || amount <= 0) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        LastDamageTime = Time.time;
        regenAccumulator = 0f;

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
