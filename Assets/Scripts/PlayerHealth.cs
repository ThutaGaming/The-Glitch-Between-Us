using UnityEngine;

public sealed class PlayerHealth : MonoBehaviour
{
    [Min(1)] public int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;

    public int CurrentHealth => currentHealth;
    public float Normalized => maxHealth <= 0 ? 0f : (float)currentHealth / maxHealth;

    private void Awake() => currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

    public void ApplyDamage(int amount) => currentHealth = Mathf.Clamp(currentHealth - Mathf.Max(0, amount), 0, maxHealth);
    public void Heal(int amount) => currentHealth = Mathf.Clamp(currentHealth + Mathf.Max(0, amount), 0, maxHealth);
}