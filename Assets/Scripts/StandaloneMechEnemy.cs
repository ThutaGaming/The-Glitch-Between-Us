using System.Collections;
using UnityEngine;

/// <summary>
/// Turns the "mech"/"mech_1" Assault Mech models (Assets/Assets/Mech) into a stationary turret-like
/// enemy: it never walks (MechWalk/MechShoot/MechHit, the asset's own scripts, are demo-reel-only -
/// they walk forward on a fixed cycle and swing the guns left-right on a timer, never actually
/// looking at anything, and the shoot animator's own ShootBigCanon layer loops forever with no
/// parameter to gate it), it just yaws in place to face the player and fires its own burst-then-pause
/// volleys once aligned - real muzzle tracers plus the asset's own canon audio, not the animator's
/// beam-flash Animation Events, which had no way to go quiet between volleys and rendered invisibly
/// besides.
/// </summary>
public class StandaloneMechEnemy : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int damagePerHit = 2;
    [Tooltip("Chance each shot actually lands, so the mech isn't a guaranteed hit every time it fires.")]
    [SerializeField] private float hitChance = 0.5f;
    [Tooltip("Degrees per second the whole mech can turn to face the player.")]
    [SerializeField] private float turnSpeed = 45f;
    [SerializeField] private float range = 60f;
    [Tooltip("How close to dead-on the mech has to be facing before it opens fire.")]
    [SerializeField] private float aimTolerance = 10f;
    [SerializeField] private int minBurst = 2;
    [SerializeField] private int maxBurst = 4;
    [SerializeField] private float shotInterval = 0.3f;
    [Tooltip("Pause between volleys - the mech stops and goes quiet here, rather than firing continuously.")]
    [SerializeField] private float burstCooldown = 2.5f;

    [Header("Assets (from Assets/Assets/Mech)")]
    [SerializeField] private RuntimeAnimatorController shootController;
    [SerializeField] private RuntimeAnimatorController hitController;
    [SerializeField] private AudioClip bigCanonClip;
    [SerializeField] private AudioClip explosionClip;

    private Transform player;
    private PlayerHealth playerHealth;
    private PlayerHitFeedback hitFeedback;
    private Animator animator;
    private AudioSource audioSource;
    private Transform[] muzzleSockets;
    private LineRenderer tracer;
    private Light muzzleLight;
    private Material fxMaterial;
    private int health;
    private bool dead;

    public bool IsDead => dead;
    public int Health => health;
    public int MaxHealth => maxHealth;
    public float LastHitTime { get; private set; } = -99f;

    /// <summary>Where the health bar hangs, above the cockpit.</summary>
    public Vector3 BarAnchor => transform.position + Vector3.up * 4.5f;

    private void Awake()
    {
        health = maxHealth;

        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null)
        {
            player = playerGo.transform;
            playerHealth = playerGo.GetComponent<PlayerHealth>();
            hitFeedback = playerGo.GetComponent<PlayerHitFeedback>();
        }

        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
        // Left assigned purely for the canon-recoil pose it holds the arms in - actual fire timing
        // is driven by this script's own coroutine below, not the controller's own auto-looping layer.
        if (shootController != null) animator.runtimeAnimatorController = shootController;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 8f;
        audioSource.maxDistance = 100f;
        audioSource.playOnAwake = false;

        FindMuzzleSockets();
        BuildTracer();
        AddBoundsCollider();
    }

    private void FindMuzzleSockets()
    {
        Transform body = transform.Find("Mech/Root/Pelvis/Body");
        if (body == null) { muzzleSockets = new Transform[0]; return; }

        string[] names = { "BigCanon01_L", "BigCanon01_R", "BigCanon02_L", "BigCanon02_R" };
        var list = new System.Collections.Generic.List<Transform>();
        foreach (var n in names)
        {
            var t = body.Find(n);
            if (t != null) list.Add(t);
        }
        muzzleSockets = list.ToArray();
    }

    /// <summary>Small, bright, brief tracer line - the same proven technique StandaloneTurretEnemy
    /// uses, rather than the beam-material-tint approach that ended up invisible here.</summary>
    private void BuildTracer()
    {
        fxMaterial = new Material(Shader.Find("Sprites/Default"));

        tracer = gameObject.AddComponent<LineRenderer>();
        tracer.useWorldSpace = true;
        tracer.positionCount = 2;
        tracer.startWidth = 0.06f;
        tracer.endWidth = 0.02f;
        tracer.sharedMaterial = fxMaterial;
        tracer.startColor = new Color(0.4f, 0.9f, 1f, 0.95f);
        tracer.endColor = new Color(0.3f, 0.7f, 1f, 0.2f);
        tracer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        tracer.enabled = false;

        var muzzleGo = new GameObject("MuzzleFlash");
        muzzleGo.transform.SetParent(transform, false);
        muzzleLight = muzzleGo.AddComponent<Light>();
        muzzleLight.type = LightType.Point;
        muzzleLight.color = new Color(0.5f, 0.85f, 1f);
        muzzleLight.range = 10f;
        muzzleLight.intensity = 0f;
        muzzleLight.shadows = LightShadows.None;
    }

    /// <summary>Bounding-box hit collider for the whole mech, sized from its own render bounds -
    /// the imported model carries no collider at all.</summary>
    private void AddBoundsCollider()
    {
        if (GetComponent<Collider>() != null) return;

        var renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds b = renderers[0].bounds;
        foreach (var r in renderers) b.Encapsulate(r.bounds);

        var box = gameObject.AddComponent<BoxCollider>();
        box.center = transform.InverseTransformPoint(b.center);
        box.size = new Vector3(b.size.x / transform.lossyScale.x, b.size.y / transform.lossyScale.y, b.size.z / transform.lossyScale.z);
    }

    private void Start()
    {
        StartCoroutine(CombatLoop());
    }

    private void Update()
    {
        if (dead || player == null) return;
        if (playerHealth != null && playerHealth.IsDead) return;

        AimAt();
    }

    private void AimAt()
    {
        Vector3 flat = player.position - transform.position;
        flat.y = 0f;
        if (flat.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(flat.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
    }

    private IEnumerator CombatLoop()
    {
        while (!dead)
        {
            if (player == null || (playerHealth != null && playerHealth.IsDead) || !PlayerInRangeAndAligned())
            {
                yield return null;
                continue;
            }

            yield return Burst();
        }
    }

    private IEnumerator Burst()
    {
        int shots = Random.Range(minBurst, maxBurst + 1);
        for (int i = 0; i < shots; i++)
        {
            if (dead || (playerHealth != null && playerHealth.IsDead) || !PlayerInRangeAndAligned()) break;

            FireOnce();
            yield return new WaitForSeconds(shotInterval);
        }

        yield return new WaitForSeconds(burstCooldown);
    }

    private void FireOnce()
    {
        Transform socket = muzzleSockets != null && muzzleSockets.Length > 0
            ? muzzleSockets[Random.Range(0, muzzleSockets.Length)]
            : null;
        Vector3 origin = socket != null ? socket.position : BarAnchor;
        Vector3 target = player.position + Vector3.up * 1.3f;

        audioSource.pitch = Random.Range(0.92f, 1.05f);
        audioSource.clip = bigCanonClip;
        if (audioSource.clip != null) audioSource.Play();

        // Roll first so a miss also looks like one: the tracer goes past the player, not into them.
        bool hits = Random.value <= EnemyAccuracy.Scale(hitChance, transform.position, player);
        StartCoroutine(MuzzleFlashRoutine(origin));
        StartCoroutine(ShowTracer(origin, hits ? target : EnemyAccuracy.MissPoint(origin, target)));

        if (playerHealth != null && hits)
        {
            playerHealth.ApplyDamage(damagePerHit);
            if (hitFeedback != null) hitFeedback.Notify(transform.position);
        }
    }

    private IEnumerator MuzzleFlashRoutine(Vector3 at)
    {
        muzzleLight.transform.position = at;
        muzzleLight.intensity = 12f;
        yield return new WaitForSeconds(0.06f);
        if (muzzleLight != null) muzzleLight.intensity = 0f;
    }

    private IEnumerator ShowTracer(Vector3 from, Vector3 to)
    {
        tracer.SetPosition(0, from);
        tracer.SetPosition(1, to);
        tracer.enabled = true;
        yield return new WaitForSeconds(0.06f);
        if (tracer != null) tracer.enabled = false;
    }

    private bool PlayerInRangeAndAligned()
    {
        if (player == null) return false;
        Vector3 flat = player.position - transform.position;
        flat.y = 0f;
        if (flat.magnitude > range) return false;

        float angle = Vector3.Angle(transform.forward, flat);
        return angle <= aimTolerance && HasClearLine();
    }

    private bool HasClearLine()
    {
        Vector3 from = BarAnchor;
        Vector3 to = player.position + Vector3.up * 1.3f;
        Vector3 d = to - from;
        float len = d.magnitude;
        if (len < 0.01f) return true;

        var hits = Physics.RaycastAll(from, d / len, len, ~0, QueryTriggerInteraction.Ignore);
        foreach (var h in hits)
        {
            if (h.collider.transform.IsChildOf(transform)) continue;
            if (h.collider.transform.root == player.root) continue;
            return false;
        }
        return true;
    }

    // ---- Animation Event receivers ----
    // The asset's animator clips (Mech_Shoot's canon states, Mech_Moving/Mech_Hit's Walk state)
    // carry baked-in Animation Events that SendMessage these names regardless of which script is
    // attached. Real firing/damage is driven by this script's own coroutine above, so these are
    // just silent no-ops that stop Unity logging "no receiver" errors for events we don't use.
    private void ShootBigCanonA() { }
    private void ShootBigCanonB() { }
    private void ShootSmallCanonA() { }
    private void ShootSmallCanonB() { }
    private void FootStep() { }
    private void EndOfWalk() { }
    private void EndOfRun() { }
    private void EndOfRunJump() { }

    /// <summary>Returns true if this hit destroyed the mech.</summary>
    public bool RegisterHit(Vector3 point)
    {
        if (dead) return false;

        LastHitTime = Time.time;
        health--;
        if (health <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    [Tooltip("How long the wreck lingers (playing its hit reaction and explosion sound) before it's removed.")]
    [SerializeField] private float removeDelayAfterDeath = 3f;

    private void Die()
    {
        dead = true;
        StopAllCoroutines();

        if (tracer != null) tracer.enabled = false;
        if (muzzleLight != null) muzzleLight.intensity = 0f;

        // The mech's own collider spans its whole bounding box - left enabled, a dead one would
        // permanently block that spot for both movement and gunfire, which is worse than an enemy
        // that's merely stopped fighting.
        var box = GetComponent<Collider>();
        if (box != null) box.enabled = false;

        if (hitController != null) animator.runtimeAnimatorController = hitController;
        animator.SetTrigger("Hit");

        if (explosionClip != null)
        {
            audioSource.clip = explosionClip;
            audioSource.Play();
        }

        Destroy(gameObject, removeDelayAfterDeath);
    }

    private void OnDestroy()
    {
        if (fxMaterial != null) Destroy(fxMaterial);
    }
}
