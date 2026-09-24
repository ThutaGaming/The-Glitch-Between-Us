using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mechanic 2's payload: the burning ground the incendiary mortar leaves behind. While the player
/// stands in it they take damage-over-time AND their health regen is held off (with a short tail
/// after leaving), so the cover they were camping becomes the worst place to stand.
///
/// Distance-based rather than trigger-based on purpose: it doesn't depend on layer-collision
/// settings or the player's Rigidbody, and the radius is exactly what the telegraph showed.
/// </summary>
public class HazardZone : MonoBehaviour
{
    private static int playersInside;

    /// <summary>True while the player is standing in any fire zone (drives the HUD warning).</summary>
    public static bool PlayerInAnyZone => playersInside > 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => playersInside = 0;

    private float radius;
    private float damagePerSecond;
    private float remaining;
    private float totalDuration;
    private float regenSuppressTail = 2f;
    // Feet must be within this height of the burning ground - a player on a raised platform next to
    // (or above) a ground fire is safe, so the fire can never trap them against the electrified floor.
    private float heightTolerance = 0.5f;
    private Transform player;
    private PlayerHealth playerHealth;
    private AudioSource loopSource;
    private readonly List<GameObject> effects = new List<GameObject>();
    private bool playerInside;

    public static HazardZone Spawn(Vector3 groundPosition, float radius, float damagePerSecond, float duration, SpiderMechFxLibrary fx)
    {
        var go = new GameObject("IncendiaryFireZone");
        go.transform.position = groundPosition;
        var zone = go.AddComponent<HazardZone>();
        zone.Init(radius, damagePerSecond, duration, fx);
        return zone;
    }

    private void Init(float zoneRadius, float dps, float duration, SpiderMechFxLibrary fx)
    {
        radius = zoneRadius;
        damagePerSecond = dps;
        remaining = duration;
        totalDuration = duration;

        var go = GameObject.FindGameObjectWithTag("Player");
        if (go != null)
        {
            player = go.transform;
            playerHealth = go.GetComponent<PlayerHealth>();
        }

        if (fx == null) return;

        // A ring of flame patches plus one in the middle, so the whole telegraphed circle reads as on fire.
        effects.Add(BossFx.Loop(fx.fireZoneFlames, transform.position, Quaternion.identity, 0.9f, transform));
        int ring = 7;
        for (int i = 0; i < ring; i++)
        {
            float a = i * Mathf.PI * 2f / ring + Random.Range(-0.2f, 0.2f);
            float r = radius * Random.Range(0.45f, 0.85f);
            Vector3 p = transform.position + new Vector3(Mathf.Cos(a) * r, 0f, Mathf.Sin(a) * r);
            effects.Add(BossFx.Loop(fx.fireZoneFlames, p, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), Random.Range(0.55f, 0.8f), transform));
        }
        effects.Add(BossFx.Loop(fx.fireZoneSmoke, transform.position + Vector3.up * 0.5f, Quaternion.identity, radius * 0.28f, transform));

        if (fx.fireLoop != null)
        {
            loopSource = gameObject.AddComponent<AudioSource>();
            loopSource.clip = fx.fireLoop;
            loopSource.loop = true;
            loopSource.spatialBlend = 1f;
            loopSource.rolloffMode = AudioRolloffMode.Linear;
            loopSource.minDistance = 3f;
            loopSource.maxDistance = 35f;
            loopSource.volume = 0.8f;
            loopSource.Play();
        }

        var light = gameObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.45f, 0.1f);
        light.range = radius * 2.2f;
        light.intensity = 4f;
        light.shadows = LightShadows.None;
    }

    private void Update()
    {
        remaining -= Time.deltaTime;

        var light = GetComponent<Light>();
        if (light != null) light.intensity = 3.2f + Mathf.PerlinNoise(Time.time * 6f, 0.3f) * 2.2f * Mathf.Clamp01(remaining);
        if (loopSource != null) loopSource.volume = 0.8f * Mathf.Clamp01(remaining);

        bool inside = false;
        if (player != null && playerHealth != null && !playerHealth.IsDead && remaining > 0f)
        {
            Vector3 d = player.position - transform.position;
            float flatDistance = new Vector2(d.x, d.z).magnitude;
            inside = flatDistance <= radius && Mathf.Abs(d.y) <= heightTolerance;
            if (inside)
            {
                playerHealth.ApplyDamageOverTime(damagePerSecond);
                playerHealth.SuppressRegen(regenSuppressTail);
            }
        }
        SetInside(inside);

        if (remaining <= 0f) Expire();
    }

    private void SetInside(bool inside)
    {
        if (inside == playerInside) return;
        playerInside = inside;
        playersInside += inside ? 1 : -1;
    }

    private void Expire()
    {
        SetInside(false);
        foreach (var e in effects) BossFx.Release(e);
        effects.Clear();
        Destroy(gameObject);
    }

    private void OnDestroy() => SetInside(false);

    public float Radius => radius;
    public float Remaining01 => totalDuration <= 0f ? 0f : Mathf.Clamp01(remaining / totalDuration);
}
