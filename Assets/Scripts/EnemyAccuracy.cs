using UnityEngine;

/// <summary>
/// Shared hit-chance rule for the enemies that roll one flat accuracy value (Level 2's mechs and
/// Level 4's robots): full accuracy up close, falling off with distance, and cut again while the
/// player is moving fast. Range and movement are the player's defence, the same way EnemyAI2 and
/// the turrets already work, so long-range robots can no longer land most shots from across a hall.
/// </summary>
public static class EnemyAccuracy
{
    /// <summary>Distance up to which an enemy keeps its full hit chance.</summary>
    public const float FullAccuracyRange = 12f;
    /// <summary>Distance at which the hit chance bottoms out at <see cref="FarScale"/>.</summary>
    public const float MinAccuracyRange = 45f;
    public const float FarScale = 0.45f;
    /// <summary>Same sprint threshold and penalty EnemyAI2 uses.</summary>
    public const float MovingSpeed = 3.5f;
    public const float MovingScale = 0.75f;

    public static float Scale(float baseChance, Vector3 shooter, Transform player)
    {
        if (player == null) return baseChance;

        float distance = Vector3.Distance(shooter, player.position);
        float chance = baseChance * Mathf.Lerp(1f, FarScale,
            Mathf.InverseLerp(FullAccuracyRange, MinAccuracyRange, distance));

        var body = player.GetComponent<Rigidbody>();
        if (body != null)
        {
            Vector3 velocity = body.linearVelocity;
            velocity.y = 0f;
            if (velocity.magnitude > MovingSpeed) chance *= MovingScale;
        }
        return chance;
    }

    /// <summary>Where a missed shot's tracer should end: beside the player rather than on them.</summary>
    public static Vector3 MissPoint(Vector3 source, Vector3 target)
    {
        Vector3 side = Vector3.Cross(Vector3.up, (target - source).normalized);
        return target + side * (Random.value < 0.5f ? -1f : 1f) * Random.Range(0.7f, 1.3f)
               + Vector3.up * Random.Range(-0.5f, 0.8f);
    }
}
