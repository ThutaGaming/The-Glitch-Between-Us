using UnityEngine;

/// <summary>
/// A precision target on the Spider-Mech. Two flavours:
///
///   Leg joint  - always live, x2 damage. Each joint has its own durability; breaking one makes the
///                boss stagger, slows its walk and leaves the joint sparking at x2.5 - a reward for
///                flanking and aiming instead of spraying the hull.
///   Charge core - the glowing yellow dome. Its collider is off except during the Ultimate charge,
///                when hits bypass the shield (x3) and count toward the interrupt DPS check.
/// </summary>
[RequireComponent(typeof(Collider))]
public class WeakPoint : MonoBehaviour
{
    [SerializeField] private float damageMultiplier = 2f;
    [SerializeField] private bool isChargeCore;

    [Header("Leg joint only")]
    [Tooltip("Damage this joint can soak before it breaks. 0 = unbreakable.")]
    [SerializeField] private float jointDurability = 320f;
    [SerializeField] private float brokenDamageMultiplier = 2.5f;

    [Header("Visuals")]
    [SerializeField] private Light glow;
    [SerializeField] private Renderer glowOrb;

    [Header("Owners")]
    [SerializeField] private BossHealthManager bossHealth;
    [SerializeField] private BossAIController bossAI;

    private float jointDamage;
    private bool broken;
    private float glowBase;
    private float pulseOffset;
    private Collider hitCollider;

    public bool IsChargeCore => isChargeCore;
    public bool IsBroken => broken;
    public float CurrentMultiplier => broken ? brokenDamageMultiplier : damageMultiplier;
    public float JointDurability01 => jointDurability <= 0f ? 1f : 1f - Mathf.Clamp01(jointDamage / jointDurability);

    private void Awake()
    {
        if (bossHealth == null) bossHealth = GetComponentInParent<BossHealthManager>();
        if (bossAI == null) bossAI = GetComponentInParent<BossAIController>();
        hitCollider = GetComponent<Collider>();
        glowBase = glow != null ? glow.intensity : 0f;
        pulseOffset = Random.Range(0f, 6.28f);
    }

    private void Update()
    {
        if (isChargeCore || glow == null) return;
        float speed = broken ? 14f : 4f;
        float wave = 0.65f + 0.35f * Mathf.Sin(Time.time * speed + pulseOffset);
        glow.intensity = glowBase * (broken ? 1.6f : 1f) * wave;
    }

    /// <summary>Applies one shot of the player's base damage. Returns the damage actually dealt.</summary>
    public float RegisterHit(float baseDamage, Vector3 point)
    {
        if (bossHealth == null || bossHealth.IsDead) return 0f;

        var kind = isChargeCore ? BossHealthManager.HitKind.Core : BossHealthManager.HitKind.WeakPoint;
        float dealt = bossHealth.ApplyDamage(baseDamage * CurrentMultiplier, point, kind);
        if (dealt <= 0f) return 0f;

        if (isChargeCore)
        {
            if (bossAI != null) bossAI.ReportCoreDamage(dealt);
        }
        else if (!broken && jointDurability > 0f)
        {
            jointDamage += dealt;
            if (jointDamage >= jointDurability) Break();
        }
        return dealt;
    }

    private void Break()
    {
        broken = true;
        if (glow != null) glow.color = new Color(1f, 0.25f, 0.1f);
        if (glowOrb != null) glowOrb.material.color = new Color(1f, 0.2f, 0.05f);
        if (bossAI != null) bossAI.OnJointBroken(this);
    }

    /// <summary>Core only: opens/closes the dome's hit collider for the Ultimate charge window.</summary>
    public void SetExposed(bool exposed)
    {
        if (!isChargeCore) return;
        if (hitCollider == null) hitCollider = GetComponent<Collider>();
        hitCollider.enabled = exposed;
    }

    /// <summary>Used on death so nothing keeps registering hits or glowing.</summary>
    public void Shutdown()
    {
        if (hitCollider == null) hitCollider = GetComponent<Collider>();
        hitCollider.enabled = false;
        enabled = false;
        if (glow != null) glow.enabled = false;
        if (glowOrb != null) glowOrb.enabled = false;
    }
}
