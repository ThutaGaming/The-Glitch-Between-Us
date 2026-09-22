using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Stationary ranged combat for the Ball, Hermit and Blast blue robots in Level 4.
/// Existing Animator controllers are left untouched; this component only handles targeting,
/// weapon-origin projectiles, damage, health and death.
/// </summary>
public sealed class BlueRobotEnemy : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnableLevelFourBlueRobots()
    {
        if (!SceneManager.GetActiveScene().path.EndsWith("Level 4.unity")) return;

        foreach (BlueRobotEnemy enemy in Object.FindObjectsByType<BlueRobotEnemy>(
                     FindObjectsInactive.Include, FindObjectsSortMode.None))
            enemy.enabled = true;
    }

    private enum RobotVariant
    {
        Ball,
        Hermit,
        Blast
    }

    [Header("Level 4 Combat")]
    [SerializeField] private float sightRange = 500f;
    [SerializeField] private float turnSpeed = 135f;
    [SerializeField, Range(0f, 1f)] private float hitChance = 0.64f;

    private RobotVariant variant;
    private Transform player;
    private PlayerHealth playerHealth;
    private Collider playerCollider;
    private Animator animator;
    private Renderer[] bodyRenderers;
    private Transform[] muzzles;
    private Vector3[] fallbackMuzzleLocalPositions;
    private LineRenderer[] tracers;
    private Light[] muzzleLights;
    private Material projectileMaterial;
    private int maxHealth;
    private int currentHealth;
    private int damagePerShot;
    private int burstShots;
    private float aimTime;
    private float shotInterval;
    private float burstPause;
    private float muzzleOffset;
    private int nextMuzzle;
    private bool dead;
    private float lastHitTime = -99f;

    public bool IsDead => dead;
    public int Health => currentHealth;
    public int MaxHealth => maxHealth;
    public float LastHitTime => lastHitTime;

    public Vector3 BarAnchor
    {
        get
        {
            if (bodyRenderers == null || bodyRenderers.Length == 0)
                return transform.position + Vector3.up * 1.5f;

            Bounds bounds = bodyRenderers[0].bounds;
            for (int i = 1; i < bodyRenderers.Length; i++)
                bounds.Encapsulate(bodyRenderers[i].bounds);
            return new Vector3(bounds.center.x, bounds.max.y + 0.22f, bounds.center.z);
        }
    }

    private void Awake()
    {
        ConfigureVariant();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        bodyRenderers = GetComponentsInChildren<Renderer>(true);
        AcquirePlayer();
        FindMuzzles();
        AddBoundsCollider();
        BuildFireEffects();
    }

    private void Start()
    {
        StartCoroutine(CombatLoop());
    }

    private void Update()
    {
        if (dead) return;
        if (player == null) AcquirePlayer();
        if (HasClearLineOfSight()) TurnTowardPlayer();
    }

    private void ConfigureVariant()
    {
        if (name.StartsWith("Blast Robot Blue"))
        {
            variant = RobotVariant.Blast;
            maxHealth = 10;
            damagePerShot = 7;
            burstShots = 2;
            aimTime = 0.6f;
            shotInterval = 0.34f;
            burstPause = 1.65f;
            muzzleOffset = 0.2f;
            fallbackMuzzleLocalPositions = new[]
            {
                new Vector3(-0.61f, 0.86f, 0.75f),
                new Vector3(0.62f, 0.86f, 0.75f)
            };
        }
        else if (name.StartsWith("Hermit Robot"))
        {
            variant = RobotVariant.Hermit;
            maxHealth = 8;
            damagePerShot = 5;
            burstShots = 4;
            aimTime = 0.5f;
            shotInterval = 0.2f;
            burstPause = 1.5f;
            muzzleOffset = 0.14f;
            fallbackMuzzleLocalPositions = new[]
            {
                new Vector3(-0.1f, 0.43f, 0.38f),
                new Vector3(0.1f, 0.43f, 0.38f)
            };
        }
        else
        {
            variant = RobotVariant.Ball;
            maxHealth = 6;
            damagePerShot = 4;
            burstShots = 3;
            aimTime = 0.4f;
            shotInterval = 0.18f;
            burstPause = 1.25f;
            muzzleOffset = 0.12f;
            fallbackMuzzleLocalPositions = new[]
            {
                new Vector3(0f, 0.77f, 0.14f)
            };
        }
    }

    private void AcquirePlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) return;

        player = playerObject.transform;
        playerHealth = playerObject.GetComponent<PlayerHealth>();
        playerCollider = playerObject.GetComponentInChildren<Collider>();
    }

    private void FindMuzzles()
    {
        string[] names;
        switch (variant)
        {
            case RobotVariant.Hermit:
                names = new[] { "RigLGun", "RigRGun" };
                break;
            case RobotVariant.Blast:
                names = new[] { "RigLBarrel", "RigRBarrel" };
                break;
            default:
                names = new[] { "RigBarrel01", "RigBarrel02" };
                break;
        }

        var found = new List<Transform>();
        foreach (string muzzleName in names)
        {
            Transform muzzle = FindDescendant(muzzleName);
            if (muzzle != null) found.Add(muzzle);
        }

        muzzles = found.ToArray();
    }

    // Keep the intended weapon count even when Unity's optimized Animator strips
    // one or more named gun bones from the runtime hierarchy.
    private int MuzzleCount => fallbackMuzzleLocalPositions != null
        ? fallbackMuzzleLocalPositions.Length
        : (muzzles != null ? muzzles.Length : 0);

    private Transform FindDescendant(string partName)
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
            if (child.name == partName) return child;
        return null;
    }

    private void AddBoundsCollider()
    {
        if (GetComponent<Collider>() != null || bodyRenderers.Length == 0) return;

        Bounds bounds = bodyRenderers[0].bounds;
        for (int i = 1; i < bodyRenderers.Length; i++)
            bounds.Encapsulate(bodyRenderers[i].bounds);

        BoxCollider box = gameObject.AddComponent<BoxCollider>();
        box.center = transform.InverseTransformPoint(bounds.center);
        Vector3 scale = transform.lossyScale;
        box.size = new Vector3(
            bounds.size.x / Mathf.Max(0.001f, Mathf.Abs(scale.x)),
            bounds.size.y / Mathf.Max(0.001f, Mathf.Abs(scale.y)),
            bounds.size.z / Mathf.Max(0.001f, Mathf.Abs(scale.z)));
    }

    private void BuildFireEffects()
    {
        projectileMaterial = new Material(Shader.Find("Sprites/Default"));
        tracers = new LineRenderer[MuzzleCount];
        muzzleLights = new Light[MuzzleCount];

        for (int i = 0; i < MuzzleCount; i++)
        {
            // Renderer components cannot reliably be stacked on the animated root.
            // Give each gun its own projectile emitter so dual-gun robots always
            // render both firing lanes.
            GameObject projectileEmitter = new GameObject("BlueRobotProjectile_" + i);
            projectileEmitter.transform.SetParent(transform, false);
            LineRenderer tracer = projectileEmitter.AddComponent<LineRenderer>();
            tracer.useWorldSpace = true;
            tracer.positionCount = 2;
            tracer.startWidth = variant == RobotVariant.Blast ? 0.075f : 0.045f;
            tracer.endWidth = variant == RobotVariant.Blast ? 0.04f : 0.018f;
            tracer.numCornerVertices = 2;
            tracer.sharedMaterial = projectileMaterial;
            tracer.startColor = variant == RobotVariant.Blast
                ? new Color(0.2f, 0.65f, 1f, 1f)
                : new Color(0.1f, 0.9f, 1f, 1f);
            tracer.endColor = new Color(0.05f, 0.35f, 1f, 0.08f);
            tracer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tracer.enabled = false;
            tracers[i] = tracer;

            GameObject flash = new GameObject("BlueRobotMuzzleFlash_" + i);
            if (muzzles != null && i < muzzles.Length)
            {
                flash.transform.SetParent(muzzles[i], false);
            }
            else
            {
                flash.transform.SetParent(transform, false);
                flash.transform.localPosition = fallbackMuzzleLocalPositions[i];
            }
            Light light = flash.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0.05f, 0.65f, 1f);
            light.range = variant == RobotVariant.Blast ? 7f : 4.5f;
            light.intensity = 0f;
            light.shadows = LightShadows.None;
            muzzleLights[i] = light;
        }
    }

    private IEnumerator CombatLoop()
    {
        while (!dead)
        {
            if (!CanAttack())
            {
                yield return new WaitForSeconds(0.12f);
                continue;
            }

            yield return new WaitForSeconds(aimTime);
            if (!CanAttack()) continue;

            for (int shot = 0; shot < burstShots; shot++)
            {
                if (!CanAttack()) break;
                FireOnce();
                yield return new WaitForSeconds(shotInterval);
            }

            yield return new WaitForSeconds(burstPause);
        }
    }

    private bool CanAttack()
    {
        return !dead && player != null && (playerHealth == null || !playerHealth.IsDead)
            && IsFacingPlayer() && HasClearLineOfSight();
    }

    private bool IsFacingPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        return direction.sqrMagnitude > 0.001f
            && Vector3.Angle(transform.forward, direction) <= 12f;
    }

    private void TurnTowardPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    private Vector3 GetPlayerAimPoint()
    {
        if (playerCollider != null && playerCollider.enabled)
            return playerCollider.bounds.center;
        return player != null ? player.position + Vector3.up * 0.5f : Vector3.zero;
    }

    private Vector3 GetMuzzlePosition(int index, Vector3 target)
    {
        int clampedIndex = Mathf.Clamp(index, 0, MuzzleCount - 1);
        Vector3 basePosition = muzzles != null && clampedIndex < muzzles.Length
            ? muzzles[clampedIndex].position
            : transform.TransformPoint(fallbackMuzzleLocalPositions[clampedIndex]);
        Vector3 direction = target - basePosition;
        if (direction.sqrMagnitude < 0.0001f) direction = transform.forward;
        return basePosition + direction.normalized * muzzleOffset;
    }

    private bool HasClearLineOfSight()
    {
        if (player == null || MuzzleCount == 0) return false;

        Vector3 target = GetPlayerAimPoint();
        Vector3 source = GetMuzzlePosition(nextMuzzle % MuzzleCount, target);
        Vector3 direction = target - source;
        float distance = direction.magnitude;
        if (distance > sightRange || distance < 0.01f) return false;

        RaycastHit[] hits = Physics.RaycastAll(
            source, direction / distance, distance, ~0, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform)) continue;
            return hit.collider.transform.root == player.root;
        }

        return true;
    }

    private void FireOnce()
    {
        int muzzleIndex = nextMuzzle++ % MuzzleCount;
        Vector3 playerPoint = GetPlayerAimPoint();
        bool hitsPlayer = Random.value <= hitChance;
        Vector3 target = playerPoint;

        if (!hitsPlayer)
        {
            float missRadius = Random.Range(0.45f, 1.05f);
            target += Random.onUnitSphere * missRadius;
        }

        Vector3 source = GetMuzzlePosition(muzzleIndex, target);
        StartCoroutine(ShowProjectile(muzzleIndex, source, target));

        if (hitsPlayer && playerHealth != null)
            playerHealth.ApplyDamage(damagePerShot);
    }

    private IEnumerator ShowProjectile(int index, Vector3 source, Vector3 target)
    {
        LineRenderer tracer = tracers[index];
        Light muzzleLight = muzzleLights[index];
        muzzleLight.transform.position = source;
        muzzleLight.intensity = variant == RobotVariant.Blast ? 11f : 7f;
        tracer.SetPosition(0, source);
        tracer.SetPosition(1, source);
        tracer.enabled = true;

        float speed = variant == RobotVariant.Blast ? 80f : 105f;
        float duration = Mathf.Clamp(Vector3.Distance(source, target) / speed, 0.07f, 0.24f);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float tailProgress = Mathf.Max(0f, progress - 0.14f);
            tracer.SetPosition(0, Vector3.Lerp(source, target, tailProgress));
            tracer.SetPosition(1, Vector3.Lerp(source, target, progress));
            muzzleLight.intensity = Mathf.Lerp(muzzleLight.intensity, 0f, progress);
            yield return null;
        }

        if (tracer != null) tracer.enabled = false;
        if (muzzleLight != null) muzzleLight.intensity = 0f;
    }

    public bool RegisterHit(Vector3 hitPoint)
    {
        if (dead) return false;

        lastHitTime = Time.time;
        currentHealth--;
        if (currentHealth > 0) return false;

        StartCoroutine(DeathRoutine());
        return true;
    }

    private IEnumerator DeathRoutine()
    {
        dead = true;
        foreach (Collider hitCollider in GetComponentsInChildren<Collider>())
            hitCollider.enabled = false;
        foreach (LineRenderer tracer in tracers)
            if (tracer != null) tracer.enabled = false;
        foreach (Light muzzleLight in muzzleLights)
            if (muzzleLight != null) muzzleLight.intensity = 0f;
        if (animator != null) animator.enabled = false;

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(72f, 0f, 18f);
        float elapsed = 0f;
        const float collapseDuration = 0.65f;
        while (elapsed < collapseDuration)
        {
            elapsed += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(
                startRotation, endRotation, Mathf.Clamp01(elapsed / collapseDuration));
            yield return null;
        }

        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (projectileMaterial != null) Destroy(projectileMaterial);
    }
}
