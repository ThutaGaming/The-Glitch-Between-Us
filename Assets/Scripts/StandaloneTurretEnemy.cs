using System.Collections;
using UnityEngine;

/// <summary>
/// Self-contained version of TurretEnemy for scenes with no CombatEncounterManager2 squad fight to
/// plug into - same yaw-and-burst-fire behaviour (yaws its pylon to track the player, fires a burst
/// once its barrels are aligned and it has a clear line), but finds the player by tag and applies
/// damage straight to PlayerHealth instead of routing through a manager. Always active once the
/// scene starts; there's no room-clear/Activate() gate here.
/// </summary>
public class StandaloneTurretEnemy : MonoBehaviour
{
    [Header("Rig")]
    [Tooltip("The yawing column. Its whole subtree - head and guns - turns with it.")]
    [SerializeField] private Transform pylon;
    [Tooltip("The gun head; the barrels are parented to it.")]
    [SerializeField] private Transform head;
    [Tooltip("Which way the barrels point in head-local space. Must be horizontal.")]
    [SerializeField] private Vector3 localFiringAxis = Vector3.forward;
    [Tooltip("Muzzle position in head-local space, just past the barrel tips.")]
    [SerializeField] private Vector3 muzzleLocalOffset;

    [Header("Combat")]
    [SerializeField] private int maxHealth = 8;
    [SerializeField] private int damagePerHit = 3;
    [Tooltip("Degrees per second the pylon can traverse - the player's way out is to break its line.")]
    [SerializeField] private float turnSpeed = 70f;
    [SerializeField] private float range = 45f;
    [Tooltip("How close the guns have to be to on-target before it opens up.")]
    [SerializeField] private float aimTolerance = 8f;
    [SerializeField] private int minBurst = 3;
    [SerializeField] private int maxBurst = 5;
    [SerializeField] private float shotInterval = 0.16f;
    [SerializeField] private float burstCooldown = 1.7f;
    [SerializeField] private float nearHitChance = 0.35f;
    [SerializeField] private float farHitChance = 0.12f;
    [SerializeField] private float nearRange = 10f;
    [SerializeField] private float farRange = 40f;
    [Tooltip("How close the player's height has to be to this turret's own base to count as 'the same floor', for the downward case below.")]
    [SerializeField] private float maxHeightDifference = 3.5f;
    [Tooltip("Lets this turret track and fire down at a player on a floor below it, not just level with or above it. Player gunfire can likewise damage this turret from any floor, not just its own.")]
    [SerializeField] private bool crossFloor = false;
    [SerializeField] private AudioClip[] shotClips;

    /// <summary>True if this turret can be shot from any floor - the shot detector uses this to
    /// decide whether to enforce its own same-floor rule for player damage.</summary>
    public bool CrossFloor => crossFloor;
    public float MaxHeightDifference => maxHeightDifference;

    private Transform player;
    private PlayerHealth playerHealth;
    private Collider playerCollider;
    private PlayerHitFeedback hitFeedback;
    private Transform muzzle;
    private Light muzzleLight;
    private LineRenderer tracer;
    private AudioSource audioSource;
    private Material fxMaterial;
    private int health;

    public bool IsDead => health <= 0;
    public int Health => health;
    public int MaxHealth => maxHealth;
    public float LastHitTime { get; private set; } = -99f;

    /// <summary>Where the health bar hangs, a little above the gun head.</summary>
    public Vector3 BarAnchor => (head != null ? head.position : transform.position) + Vector3.up * 0.9f;

    /// <summary>
    /// Aim/hit-test point on the player. Uses the actual collider bounds rather than a fixed
    /// "1.3m above the feet" offset, so it stays correct regardless of the player rig's scale -
    /// a hardcoded offset overshoots a scaled-down player capsule and the turret can fire all day
    /// without ever actually touching it.
    /// </summary>
    private Vector3 PlayerAimPoint => playerCollider != null && playerCollider.enabled
        ? playerCollider.bounds.center
        : (player != null ? player.position + Vector3.up * 1.3f : Vector3.zero);

    private void Awake()
    {
        health = maxHealth;

        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null)
        {
            player = playerGo.transform;
            playerHealth = playerGo.GetComponent<PlayerHealth>();
            playerCollider = playerGo.GetComponentInChildren<Collider>();
            hitFeedback = playerGo.GetComponent<PlayerHitFeedback>();
        }

        muzzle = new GameObject("Muzzle").transform;
        muzzle.SetParent(head, false);
        muzzle.localPosition = muzzleLocalOffset;

        muzzleLight = muzzle.gameObject.AddComponent<Light>();
        muzzleLight.type = LightType.Point;
        muzzleLight.color = new Color(1f, 0.75f, 0.35f);
        muzzleLight.range = 7f;
        muzzleLight.intensity = 0f;
        muzzleLight.shadows = LightShadows.None;

        fxMaterial = new Material(Shader.Find("Sprites/Default"));
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

    private void Start()
    {
        if (player != null) StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (!IsDead)
        {
            if ((playerHealth != null && playerHealth.IsDead) || !CanEngageAtHeight())
            {
                yield return null;
                continue;
            }

            Vector3 chest = PlayerAimPoint;
            AimAt(chest);

            bool inRange = FlatDistance(muzzle.position, player.position) <= range;
            if (inRange && Aligned(chest) && HasClearLine(chest))
                yield return Burst();
            else
                yield return null;
        }
    }

    /// <summary>
    /// True once the turret is allowed to engage at the player's current height. Same floor always
    /// works; a player above always works too (every turret can shoot up a stairwell/atrium at
    /// someone on a higher floor); a player below only works for a crossFloor turret - a normal one
    /// stays inert rather than shooting down at a floor it isn't meant to cover.
    /// </summary>
    private bool CanEngageAtHeight()
    {
        float diff = player.position.y - transform.position.y;
        if (Mathf.Abs(diff) <= maxHeightDifference) return true;
        if (diff > 0f) return true;
        return crossFloor;
    }

    private static float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    private void AimAt(Vector3 target)
    {
        Vector3 flat = target - pylon.position;
        flat.y = 0f;
        if (flat.sqrMagnitude < 0.0001f) return;

        // Atan2 gives the yaw that puts +Z on the target; back off by however far the barrels
        // already sit from +Z in head-local space.
        float yaw = Mathf.Atan2(flat.x, flat.z) * Mathf.Rad2Deg - AxisOffsetDegrees;
        pylon.rotation = Quaternion.RotateTowards(pylon.rotation, Quaternion.Euler(0f, yaw, 0f),
            turnSpeed * Time.deltaTime);
    }

    private float AxisOffsetDegrees => Mathf.Atan2(localFiringAxis.x, localFiringAxis.z) * Mathf.Rad2Deg;

    private Vector3 AimDirection
    {
        get
        {
            Vector3 aim = head.TransformDirection(localFiringAxis);
            aim.y = 0f;
            return aim;
        }
    }

    private bool Aligned(Vector3 target)
    {
        Vector3 to = target - head.position;
        to.y = 0f;
        return Vector3.Angle(AimDirection, to) <= aimTolerance;
    }

    private bool HasClearLine(Vector3 target)
    {
        Vector3 d = target - muzzle.position;
        float len = d.magnitude;
        if (len < 0.01f) return true;

        var hits = Physics.RaycastAll(muzzle.position, d / len, len, ~0, QueryTriggerInteraction.Ignore);
        foreach (var h in hits)
        {
            if (h.collider.transform.IsChildOf(transform)) continue;
            if (h.collider.transform.root == player.root) continue;
            return false;
        }
        return true;
    }

    private IEnumerator Burst()
    {
        int shots = Random.Range(minBurst, maxBurst + 1);
        for (int i = 0; i < shots; i++)
        {
            if (IsDead || (playerHealth != null && playerHealth.IsDead) || !CanEngageAtHeight()) break;

            Vector3 chest = PlayerAimPoint;
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
            if (playerHealth != null && best.collider.transform.root == player.root)
            {
                playerHealth.ApplyDamage(damagePerHit);
                if (hitFeedback != null) hitFeedback.Notify(transform.position);
            }
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
        if (muzzleLight != null) muzzleLight.intensity = 0f;
        if (tracer != null) tracer.enabled = false;
        // Guns drop as the mount gives out.
        if (head != null) head.localRotation = Quaternion.Euler(0f, 0f, -28f);
        CombatAudio.EnemyDeath(head != null ? head.position : transform.position, CombatAudio.Death.Explosion);

        // The root's BoxCollider spans the whole turret - left enabled, a destroyed one would
        // permanently block that spot for both movement and gunfire even though it's out of the fight.
        var box = GetComponent<Collider>();
        if (box != null) box.enabled = false;
    }

    private void OnDestroy()
    {
        if (fxMaterial != null) Destroy(fxMaterial);
    }
}
