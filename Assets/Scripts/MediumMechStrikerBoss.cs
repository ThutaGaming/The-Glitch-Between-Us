using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Side-boss behaviour for the MediumMechStrikerMasterPrefab (Level 4). Patrols a small loop around
/// its spawn point using the asset's own walk cycle, and once it spots the player, plants itself and
/// cycles through its three mounted weapons - a sustained laser poke from the two
/// ScifiMechMediumStandardLaser barrels, a quick SRM burst from the ScifiMechSRMRackX2 mounts, and a
/// heavier periodic LRM volley from the ScifiMechLRMRackX5 mounts - rather than firing everything at
/// once. Health reads as a fixed bar across the top of the screen instead of a floating world-space
/// one, since a boss this size doesn't need to be looked at directly to track the fight.
/// </summary>
public class MediumMechStrikerBoss : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string bossName = "MECH STRIKER";

    [Header("Combat - Health")]
    [SerializeField] private int maxHealth = 45;

    [Header("Combat - Laser (short burst per stop)")]
    [SerializeField] private int laserDamage = 4;
    [SerializeField, Range(0f, 1f)] private float laserHitChance = 0.4f;
    [SerializeField] private float laserShotInterval = 0.2f;
    [SerializeField] private int laserBurstCount = 3;

    [Header("Combat - SRM (one shot per stop)")]
    [SerializeField] private int srmDamage = 8;
    [SerializeField, Range(0f, 1f)] private float srmHitChance = 0.5f;
    [SerializeField] private float srmSpeed = 45f;

    [Header("Combat - LRM (one shot per stop)")]
    [SerializeField] private int lrmDamage = 6;
    [SerializeField, Range(0f, 1f)] private float lrmHitChance = 0.45f;
    [SerializeField] private float lrmSpeed = 30f;

    [Header("Stop-and-shoot pattern")]
    [Tooltip("How far it walks off to each side before planting and firing again - fires once in place, sidesteps right, plants and fires, sidesteps left, plants and fires, repeating.")]
    [SerializeField] private float strafeDistance = 7f;
    [Tooltip("Pause after a stop's volley before walking to the next side.")]
    [SerializeField] private float postFireHold = 0.5f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.2f;
    [SerializeField] private float turnSpeed = 60f;
    [SerializeField] private float patrolRadius = 8f;
    [SerializeField] private int patrolPointCount = 3;
    [SerializeField] private float waypointPause = 2f;

    [Header("Engagement")]
    [SerializeField] private float engageRange = 55f;
    [SerializeField] private float heightTolerance = 4f;
    [SerializeField] private float loseSightGrace = 4f;

    [Header("Assets (from MediumMechStriker / ScifiMechAmmunition)")]
    [SerializeField] private GameObject srmProjectilePrefab;
    [SerializeField] private GameObject lrmProjectilePrefab;
    [SerializeField] private AudioClip[] laserClips;
    [SerializeField] private AudioClip[] missileClips;
    [SerializeField] private AudioClip explosionClip;

    [SerializeField] private float removeDelayAfterDeath = 8f;

    [Header("On death - next objective")]
    [Tooltip("Same blue ObjectiveGlow rim every other mission marker uses, put on the door up to Level 5.")]
    [SerializeField] private ObjectiveGlow doorGlow;
    [SerializeField] private string nextMissionObjective = "Go up to Level 5";

    private static readonly string[] HitClips = { "b1HitFront1", "b1HitFront2", "b1HitFront3" };

    private Transform player;
    private PlayerHealth playerHealth;
    private Collider playerCollider;
    private PlayerHitFeedback hitFeedback;
    private Animator animator;
    private AudioSource audioSource;

    private Transform[] laserMuzzles;
    private Transform[] srmMuzzles;
    private Transform[] lrmMuzzles;

    private LineRenderer laserTracer;
    private Light laserFlash;
    private Material fxMaterial;

    private Vector3[] patrolPointsWorld;
    private int currentPatrolIndex;
    private float lastSeenPlayerTime = -99f;
    private bool combatActive;
    private bool hasEngagedOnce;
    private bool useSrmNext = true;

    private int health;
    private bool dead;

    public bool IsDead => dead;
    public int Health => health;
    public int MaxHealth => maxHealth;
    public float LastHitTime { get; private set; } = -99f;

    /// <summary>Where the (unused-by-default) floating anchor would sit, kept for parity with the
    /// other Standalone enemies in case something wants to point at the boss.</summary>
    public Vector3 BarAnchor => transform.position + Vector3.up * 5.5f;

    private void Awake()
    {
        health = maxHealth;

        AcquirePlayer();
        FindMuzzles();
        BuildLaserTracer();
        BuildPatrolRoute();

        animator = GetComponent<Animator>();
        if (animator != null) animator.applyRootMotion = false;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 10f;
        audioSource.maxDistance = 120f;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        StartCoroutine(PatrolLoop());
        StartCoroutine(CombatCycle());
    }

    private void AcquirePlayer()
    {
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go == null) return;
        player = go.transform;
        playerHealth = go.GetComponent<PlayerHealth>();
        playerCollider = go.GetComponentInChildren<Collider>();
        hitFeedback = go.GetComponent<PlayerHitFeedback>();
    }

    private Vector3 PlayerAimPoint => playerCollider != null && playerCollider.enabled
        ? playerCollider.bounds.center
        : (player != null ? player.position + Vector3.up * 1f : Vector3.zero);

    private Transform FindDescendant(string exactName)
    {
        foreach (var t in GetComponentsInChildren<Transform>(true))
            if (t.name == exactName) return t;
        return null;
    }

    private void FindMuzzles()
    {
        var laser = new List<Transform>();
        var srm = new List<Transform>();
        var lrm = new List<Transform>();
        foreach (var t in GetComponentsInChildren<Transform>(true))
        {
            if (t.name.StartsWith("ScifiMechMediumStandardLaser")) laser.Add(t);
            else if (t.name.StartsWith("ScifiMechSRMRackX2")) srm.Add(t);
            else if (t.name.StartsWith("ScifiMechLRMRackX5")) lrm.Add(t);
        }
        laserMuzzles = laser.ToArray();
        srmMuzzles = srm.ToArray();
        lrmMuzzles = lrm.ToArray();
    }

    /// <summary>Same proven tracer technique as StandaloneTurretEnemy/StandaloneMechEnemy.</summary>
    private void BuildLaserTracer()
    {
        fxMaterial = new Material(Shader.Find("Sprites/Default"));

        laserTracer = gameObject.AddComponent<LineRenderer>();
        laserTracer.useWorldSpace = true;
        laserTracer.positionCount = 2;
        laserTracer.startWidth = 0.09f;
        laserTracer.endWidth = 0.04f;
        laserTracer.sharedMaterial = fxMaterial;
        laserTracer.startColor = new Color(1f, 0.15f, 0.1f, 0.95f);
        laserTracer.endColor = new Color(1f, 0.05f, 0.02f, 0.15f);
        laserTracer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        laserTracer.enabled = false;

        var flashGo = new GameObject("LaserMuzzleFlash");
        flashGo.transform.SetParent(transform, false);
        laserFlash = flashGo.AddComponent<Light>();
        laserFlash.type = LightType.Point;
        laserFlash.color = new Color(1f, 0.2f, 0.15f);
        laserFlash.range = 9f;
        laserFlash.intensity = 0f;
        laserFlash.shadows = LightShadows.None;
    }

    private void BuildPatrolRoute()
    {
        var pts = new List<Vector3>();
        Vector3 center = transform.position;
        for (int i = 0; i < patrolPointCount; i++)
        {
            float angle = i * Mathf.PI * 2f / patrolPointCount;
            pts.Add(center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * patrolRadius);
        }
        patrolPointsWorld = pts.ToArray();
    }

    private void Update()
    {
        if (dead) return;

        if (player == null) AcquirePlayer();
        if (CanSeePlayer())
        {
            lastSeenPlayerTime = Time.time;
            combatActive = true;
            hasEngagedOnce = true;
            FacePlayer();
        }
        else if (Time.time - lastSeenPlayerTime > loseSightGrace)
        {
            combatActive = false;
        }
    }

    private bool CanSeePlayer()
    {
        if (player == null || (playerHealth != null && playerHealth.IsDead)) return false;

        Vector3 flat = player.position - transform.position;
        flat.y = 0f;
        if (flat.magnitude > engageRange) return false;
        if (Mathf.Abs(player.position.y - transform.position.y) > heightTolerance) return false;

        return HasClearLine(transform.position + Vector3.up * 3f, PlayerAimPoint);
    }

    private bool HasClearLine(Vector3 origin, Vector3 target)
    {
        Vector3 d = target - origin;
        float len = d.magnitude;
        if (len < 0.01f) return true;

        var hits = Physics.RaycastAll(origin, d / len, len, ~0, QueryTriggerInteraction.Ignore);
        foreach (var h in hits)
        {
            if (h.collider.transform.IsChildOf(transform)) continue;
            if (player != null && h.collider.transform.root == player.root) continue;
            return false;
        }
        return true;
    }

    private void FacePlayer()
    {
        if (player == null) return;
        Vector3 flat = player.position - transform.position;
        flat.y = 0f;
        if (flat.sqrMagnitude < 0.0001f) return;
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            Quaternion.LookRotation(flat.normalized), turnSpeed * Time.deltaTime);
    }

    private void PlayAnimSafe(string state)
    {
        if (animator == null) return;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(state)) return;
        animator.CrossFade(state, 0.15f);
    }

    // ---------- movement ----------

    private IEnumerator PatrolLoop()
    {
        while (!dead)
        {
            if (combatActive || patrolPointsWorld.Length == 0)
            {
                PlayAnimSafe("a4IdlePose");
                yield return null;
                continue;
            }

            Vector3 target = patrolPointsWorld[currentPatrolIndex];
            PlayAnimSafe("a5WalkCycle");
            while (!dead && !combatActive &&
                   Vector3.Distance(Flat(transform.position), Flat(target)) > 0.5f)
            {
                Vector3 dir = target - transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation,
                        Quaternion.LookRotation(dir.normalized), turnSpeed * Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position,
                    new Vector3(target.x, transform.position.y, target.z), moveSpeed * Time.deltaTime);
                yield return null;
            }

            if (dead || combatActive) continue;

            PlayAnimSafe("a4IdlePose");
            yield return new WaitForSeconds(waypointPause);
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPointsWorld.Length;
        }
    }

    private static Vector3 Flat(Vector3 v)
    {
        v.y = 0f;
        return v;
    }

    // ---------- stop-and-shoot combat cycle ----------

    /// <summary>
    /// While the player is spotted: fire a volley right where it's standing, sidestep to the right,
    /// plant and fire again, sidestep to the left, plant and fire again, repeating right/left for as
    /// long as combat stays active. Movement and firing never overlap - it always stops to shoot.
    /// </summary>
    private IEnumerator CombatCycle()
    {
        Vector3 anchor = transform.position;
        int nextSide = 0;
        bool justEnteredCombat = true;

        while (!dead)
        {
            if (!combatActive)
            {
                justEnteredCombat = true;
                yield return null;
                continue;
            }

            if (justEnteredCombat)
            {
                anchor = transform.position;
                nextSide = 0;
                justEnteredCombat = false;
            }

            if (nextSide != 0)
            {
                Vector3 toPlayer = Flat(player.position - transform.position);
                Vector3 right = toPlayer.sqrMagnitude > 0.0001f
                    ? Vector3.Cross(Vector3.up, toPlayer.normalized)
                    : transform.right;
                Vector3 destination = anchor + right * (nextSide * strafeDistance);
                yield return MoveToCombatSpot(destination);
            }

            if (dead) yield break;
            if (!combatActive) continue;

            yield return StopAndFire();

            nextSide = nextSide <= 0 ? 1 : -1;
        }
    }

    private IEnumerator MoveToCombatSpot(Vector3 destination)
    {
        PlayAnimSafe("a5WalkCycle");
        while (!dead && combatActive &&
               Vector3.Distance(Flat(transform.position), Flat(destination)) > 0.4f)
        {
            Vector3 dir = destination - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(dir.normalized), turnSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(destination.x, transform.position.y, destination.z), moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator StopAndFire()
    {
        PlayAnimSafe("a4IdlePose");

        // Square up to the player before opening fire.
        float turnTimeout = 1.5f;
        float elapsed = 0f;
        while (!dead && combatActive && elapsed < turnTimeout)
        {
            FacePlayer();
            Vector3 flat = Flat(player.position - transform.position);
            if (flat.sqrMagnitude > 0.0001f && Vector3.Angle(transform.forward, flat) < 5f) break;
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (dead || !combatActive) yield break;

        for (int i = 0; i < laserBurstCount && !dead && combatActive; i++)
        {
            FireLaser();
            yield return new WaitForSeconds(laserShotInterval);
        }

        if (dead || !combatActive) yield break;

        // One missile per stop, alternating launcher types.
        useSrmNext = !useSrmNext;
        if (useSrmNext) FireMissile(srmMuzzles, srmProjectilePrefab, srmSpeed, srmDamage, srmHitChance);
        else FireMissile(lrmMuzzles, lrmProjectilePrefab, lrmSpeed, lrmDamage, lrmHitChance);

        yield return new WaitForSeconds(postFireHold);
    }

    private void FireLaser()
    {
        if (laserMuzzles == null || laserMuzzles.Length == 0 || player == null) return;
        Transform muzzle = laserMuzzles[Random.Range(0, laserMuzzles.Length)];
        Vector3 target = PlayerAimPoint;
        if (!HasClearLine(muzzle.position, target)) return;

        bool hits = Random.value <= laserHitChance;
        Vector3 aim = hits ? target : target + Random.insideUnitSphere * Random.Range(1f, 2f);

        StartCoroutine(ShowLaser(muzzle.position, aim));
        if (laserClips != null && laserClips.Length > 0)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(laserClips[Random.Range(0, laserClips.Length)]);
        }

        if (hits && playerHealth != null)
        {
            playerHealth.ApplyDamage(laserDamage);
            if (hitFeedback != null) hitFeedback.Notify(transform.position);
        }
    }

    private IEnumerator ShowLaser(Vector3 from, Vector3 to)
    {
        laserFlash.transform.position = from;
        laserFlash.intensity = 14f;
        laserTracer.SetPosition(0, from);
        laserTracer.SetPosition(1, to);
        laserTracer.enabled = true;
        yield return new WaitForSeconds(0.06f);
        if (laserTracer != null) laserTracer.enabled = false;
        if (laserFlash != null) laserFlash.intensity = 0f;
    }

    // ---------- missiles (SRM / LRM) ----------

    private void FireMissile(Transform[] muzzles, GameObject prefab, float speed, int damage, float hitChance)
    {
        if (muzzles == null || muzzles.Length == 0 || prefab == null || player == null) return;
        Transform muzzle = muzzles[Random.Range(0, muzzles.Length)];
        Vector3 target = PlayerAimPoint;
        bool hits = Random.value <= hitChance;
        if (!hits) target += Random.insideUnitSphere * Random.Range(1.2f, 2.4f);

        GameObject proj = Instantiate(prefab, muzzle.position,
            Quaternion.LookRotation((target - muzzle.position).normalized));

        var trail = proj.AddComponent<TrailRenderer>();
        trail.time = 0.35f;
        trail.startWidth = 0.14f;
        trail.endWidth = 0.02f;
        trail.material = fxMaterial;
        trail.startColor = new Color(1f, 0.6f, 0.15f, 0.9f);
        trail.endColor = new Color(0.4f, 0.3f, 0.2f, 0f);
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        StartCoroutine(FlyMissile(proj, target, speed, damage, hits));

        if (missileClips != null && missileClips.Length > 0)
        {
            audioSource.pitch = Random.Range(0.9f, 1.05f);
            audioSource.PlayOneShot(missileClips[Random.Range(0, missileClips.Length)]);
        }
    }

    private IEnumerator FlyMissile(GameObject proj, Vector3 target, float speed, int damage, bool hits)
    {
        Vector3 start = proj.transform.position;
        float distance = Vector3.Distance(start, target);
        float duration = Mathf.Max(0.05f, distance / speed);
        float elapsed = 0f;
        while (elapsed < duration && proj != null)
        {
            elapsed += Time.deltaTime;
            proj.transform.position = Vector3.Lerp(start, target, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }
        if (proj != null) Destroy(proj);

        if (hits && playerHealth != null && !playerHealth.IsDead)
        {
            playerHealth.ApplyDamage(damage);
            if (hitFeedback != null) hitFeedback.Notify(transform.position);
        }
    }

    // ---------- health / death ----------

    /// <summary>Returns true if this hit destroyed the boss.</summary>
    public bool RegisterHit(Vector3 point)
    {
        if (dead) return false;

        LastHitTime = Time.time;
        health--;
        if (animator != null) animator.CrossFade(HitClips[Random.Range(0, HitClips.Length)], 0.05f);

        if (health <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    private void Die()
    {
        dead = true;
        combatActive = false;
        StopAllCoroutines();

        if (laserTracer != null) laserTracer.enabled = false;
        if (laserFlash != null) laserFlash.intensity = 0f;

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (animator != null) animator.CrossFade("b2DeathFallDown1", 0.1f);

        if (explosionClip != null)
        {
            audioSource.clip = explosionClip;
            audioSource.Play();
        }

        if (MissionHUD.Instance != null && !string.IsNullOrEmpty(nextMissionObjective))
            MissionHUD.Instance.SetObjective(nextMissionObjective);
        if (doorGlow != null) doorGlow.SetGlowing(true);

        Destroy(gameObject, removeDelayAfterDeath);
    }

    private void OnDestroy()
    {
        if (fxMaterial != null) Destroy(fxMaterial);
    }

    // ---------- boss HUD ----------

    private void OnGUI()
    {
        if (dead || !hasEngagedOnce) return;

        const float barWidth = 380f;
        const float barHeight = 22f;
        float x = Screen.width * 0.5f - barWidth * 0.5f;
        float y = 20f;

        var tex = Texture2D.whiteTexture;
        var prevColor = GUI.color;

        GUI.color = new Color(0f, 0f, 0f, 0.75f);
        GUI.DrawTexture(new Rect(x - 3f, y - 3f, barWidth + 6f, barHeight + 6f), tex);

        GUI.color = new Color(0.12f, 0.12f, 0.12f, 0.9f);
        GUI.DrawTexture(new Rect(x, y, barWidth, barHeight), tex);

        float pct = Mathf.Clamp01((float)health / Mathf.Max(1, maxHealth));
        GUI.color = Color.Lerp(new Color(0.85f, 0.12f, 0.08f), new Color(1f, 0.65f, 0.1f), pct);
        GUI.DrawTexture(new Rect(x, y, barWidth * pct, barHeight), tex);

        GUI.color = prevColor;
        var style = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(x, y - 20f, barWidth, 18f), bossName, style);
    }
}
