using System.Collections;
using UnityEngine;

/// <summary>
/// Stationary auto-turret that fights alongside a CombatEncounterManager2 squad. Sits inert until
/// <see cref="Activate"/>, then yaws its pylon to keep the guns tracking the player and fires
/// bursts whenever it has a clear line. The barrels run along the head's local +X, so "aimed"
/// means that axis is on the player, not the usual forward. Player fire reaches it through the
/// manager's shot registration, which also draws its health bar.
/// </summary>
public class TurretEnemy : MonoBehaviour
{
    [Header("Rig")]
    [Tooltip("The yawing column. Its whole subtree - head and guns - turns with it.")]
    [SerializeField] private Transform pylon;
    [Tooltip("The gun head. Its local +X is the firing axis.")]
    [SerializeField] private Transform head;
    [Tooltip("Muzzle position in head-local space, just past the barrel tips.")]
    [SerializeField] private Vector3 muzzleLocalOffset = new Vector3(6.2f, 0f, 0f);

    [Header("Combat")]
    [SerializeField] private int maxHealth = 8;
    [SerializeField] private int damagePerHit = 7;
    [Tooltip("Degrees per second the pylon can traverse - the player's way out is to break its line.")]
    [SerializeField] private float turnSpeed = 70f;
    [SerializeField] private float range = 45f;
    [Tooltip("How close the guns have to be to on-target before it opens up.")]
    [SerializeField] private float aimTolerance = 8f;
    [SerializeField] private int minBurst = 4;
    [SerializeField] private int maxBurst = 7;
    [SerializeField] private float shotInterval = 0.16f;
    [SerializeField] private float burstCooldown = 1.7f;
    [SerializeField] private float nearHitChance = 0.5f;
    [SerializeField] private float farHitChance = 0.22f;
    [SerializeField] private float nearRange = 10f;
    [SerializeField] private float farRange = 40f;

    private CombatEncounterManager2 manager;
    private Transform player;
    private Transform muzzle;
    private Light muzzleLight;
    private LineRenderer tracer;
    private AudioSource audioSource;
    private AudioClip[] shotClips;
    private int health;
    private bool active;

    public bool IsDead => health <= 0;
    public int Health => health;
    public int MaxHealth => maxHealth;
    public float LastHitTime { get; private set; } = -99f;

    /// <summary>Where the health bar hangs, a little above the gun head.</summary>
    public Vector3 BarAnchor => (head != null ? head.position : transform.position) + Vector3.up * 0.9f;

    public void Initialize(CombatEncounterManager2 owner, Transform playerTransform, AudioClip[] clips, Material fxMaterial)
    {
        manager = owner;
        player = playerTransform;
        shotClips = clips;
        health = maxHealth;

        muzzle = new GameObject("Muzzle").transform;
        muzzle.SetParent(head, false);
        muzzle.localPosition = muzzleLocalOffset;

        muzzleLight = muzzle.gameObject.AddComponent<Light>();
        muzzleLight.type = LightType.Point;
        muzzleLight.color = new Color(1f, 0.75f, 0.35f);
        muzzleLight.range = 7f;
        muzzleLight.intensity = 0f;
        muzzleLight.shadows = LightShadows.None;

        tracer = gameObject.AddComponent<LineRenderer>();
        tracer.useWorldSpace = true;
        tracer.positionCount = 2;
        tracer.startWidth = 0.05f;
        tracer.endWidth = 0.02f;
        tracer.sharedMaterial = fxMaterial;
        tracer.startColor = new Color(1f, 0.85f, 0.45f, 0.95f);
        tracer.endColor = new Color(1f, 0.6f, 0.2f, 0.2f);
        tracer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        tracer.enabled = false;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 5f;
        audioSource.maxDistance = 70f;
        audioSource.volume = 0.9f;
        audioSource.playOnAwake = false;
    }

    /// <summary>Wakes the turret up; called when the encounter starts.</summary>
    public void Activate()
    {
        if (active || IsDead || manager == null) return;
        active = true;
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (!IsDead)
        {
            if (manager.PlayerDead)
            {
                yield return null;
                continue;
            }

            Vector3 chest = manager.PlayerChest;
            AimAt(chest);

            bool inRange = CombatEncounterManager2.FlatDistance(muzzle.position, player.position) <= range;
            if (inRange && Aligned(chest) && HasClearLine(chest))
                yield return Burst();
            else
                yield return null;
        }
    }

    private void AimAt(Vector3 target)
    {
        Vector3 flat = target - pylon.position;
        flat.y = 0f;
        if (flat.sqrMagnitude < 0.0001f) return;

        // Atan2 gives the yaw that puts +Z on the target; the guns sit on +X, a quarter turn over.
        float yaw = Mathf.Atan2(flat.x, flat.z) * Mathf.Rad2Deg - 90f;
        pylon.rotation = Quaternion.RotateTowards(pylon.rotation, Quaternion.Euler(0f, yaw, 0f),
            turnSpeed * Time.deltaTime);
    }

    private bool Aligned(Vector3 target)
    {
        Vector3 aim = head.right;
        aim.y = 0f;
        Vector3 to = target - head.position;
        to.y = 0f;
        return Vector3.Angle(aim, to) <= aimTolerance;
    }

    /// <summary>
    /// Its own parts sit between the muzzle and the world, and it holds fire rather than shooting
    /// through its own squad, so neither counts as a blocker the way a wall does.
    /// </summary>
    private bool HasClearLine(Vector3 target)
    {
        Vector3 d = target - muzzle.position;
        float len = d.magnitude;
        if (len < 0.01f) return true;

        var hits = Physics.RaycastAll(muzzle.position, d / len, len, ~0, QueryTriggerInteraction.Ignore);
        foreach (var h in hits)
        {
            if (h.collider.transform.IsChildOf(transform)) continue;
            if (manager.IsPlayerCollider(h.collider)) continue;
            return false;
        }
        return true;
    }

    private IEnumerator Burst()
    {
        int shots = Random.Range(minBurst, maxBurst + 1);
        for (int i = 0; i < shots; i++)
        {
            if (IsDead || manager.PlayerDead) break;

            Vector3 chest = manager.PlayerChest;
            AimAt(chest);
            if (!HasClearLine(chest)) break;

            FireShot(chest);
            yield return new WaitForSeconds(shotInterval);
        }

        yield return new WaitForSeconds(burstCooldown);
    }

    private void FireShot(Vector3 chest)
    {
        StartCoroutine(MuzzleFlash());
        if (shotClips != null && shotClips.Length > 0)
        {
            audioSource.pitch = Random.Range(0.72f, 0.82f);
            audioSource.PlayOneShot(shotClips[Random.Range(0, shotClips.Length)]);
        }

        Vector3 origin = muzzle.position;
        float dist = Vector3.Distance(origin, chest);
        float chance = Mathf.Lerp(nearHitChance, farHitChance, Mathf.InverseLerp(nearRange, farRange, dist));
        if (manager.PlayerSpeed > 3.5f) chance *= 0.8f;

        Vector3 aim = chest + Random.insideUnitSphere * 0.12f;
        if (Random.value > chance)
        {
            Vector3 side = Vector3.Cross(Vector3.up, (chest - origin).normalized);
            aim = chest + side * (Random.value < 0.5f ? -1f : 1f) * Random.Range(0.7f, 1.3f)
                  + Vector3.up * Random.Range(-0.5f, 0.8f);
        }

        Vector3 dir = (aim - origin).normalized;
        Vector3 end = origin + dir * 70f;
        var hits = Physics.RaycastAll(origin, dir, 70f, ~0, QueryTriggerInteraction.Ignore);
        float nearest = float.MaxValue;
        RaycastHit best = default;
        foreach (var h in hits)
        {
            if (h.collider.transform.IsChildOf(transform)) continue;
            if (h.distance < nearest)
            {
                nearest = h.distance;
                best = h;
            }
        }

        if (nearest < float.MaxValue)
        {
            end = best.point;
            if (manager.IsPlayerCollider(best.collider)) manager.DamagePlayer(damagePerHit, transform.position);
            else if (best.collider.GetComponentInParent<EnemyAI2>() == null) manager.SpawnImpact(best.point);
        }

        StartCoroutine(ShowTracer(origin, end));
    }

    private IEnumerator MuzzleFlash()
    {
        muzzleLight.intensity = 10f;
        yield return new WaitForSeconds(0.05f);
        if (muzzleLight != null) muzzleLight.intensity = 0f;
    }

    private IEnumerator ShowTracer(Vector3 from, Vector3 to)
    {
        tracer.SetPosition(0, from);
        tracer.SetPosition(1, to);
        tracer.enabled = true;
        yield return new WaitForSeconds(0.05f);
        if (tracer != null) tracer.enabled = false;
    }

    /// <summary>Returns true if this hit destroyed the turret.</summary>
    public bool RegisterHit(Vector3 point)
    {
        if (IsDead) return false;

        LastHitTime = Time.time;
        health--;
        manager.SpawnImpact(point);

        if (health <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    private void Die()
    {
        StopAllCoroutines();
        active = false;
        if (muzzleLight != null) muzzleLight.intensity = 0f;
        if (tracer != null) tracer.enabled = false;
        // Guns drop as the mount gives out.
        if (head != null) head.localRotation = Quaternion.Euler(0f, 0f, -28f);
        manager.NotifyTurretDestroyed();
    }
}
