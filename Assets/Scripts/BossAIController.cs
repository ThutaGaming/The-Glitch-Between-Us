using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Finite-state brain for the Spider-Mech final boss (P_ISO_Mech, Level 5).
///
///  Dormant ──(player enters arena)──► Intro (drops in from the sky) ──► Combat
///
///  Combat      Walks its OWN patrol loop around the pit - it never chases the player and never
///              backs away - while its body keeps turning to face them (the 2D locomotion blend tree
///              strafes the legs so it can walk sideways/backwards with its guns on target).
///              Fires plasma volleys, stomps anyone who hugs it, and watches the player's position
///              history for camping (Mechanic 2 -> incendiary mortar).
///  WindUp      1.5s telegraph before the Ultimate (fair warning: reload, find the core).
///  Charging    Mechanic 3. 6s charge; the core dome opens. Enough damage on it -> Stunned.
///  Stunned     5s powered-down, x2 damage taken (the interrupt reward).
///  Shockwave   The punishment: an expanding ground ring. Jumping over it or standing on
///              high ground reduces it to 9 HP; otherwise it takes 60% of the player's max HP.
///  PhaseShift  At 50% health: roars, raises the arena platforms and starts the electrified
///              floor (Mechanic 4, owned by ArenaHazardManager), then fights faster and meaner.
///  Stomp / Staggered  short reactive beats (point-blank stomp, broken leg joint).
///  Dead        Chain explosions, collapse, burning wreck.
///
/// Mechanic 1 (shield regen) lives in BossHealthManager.
/// </summary>
public class BossAIController : MonoBehaviour
{
    public enum BossState { Dormant, Intro, Combat, WindUp, Charging, Stunned, Shockwave, PhaseShift, Stomp, Staggered, Dead }

    [Header("Identity")]
    [SerializeField] private string bossName = "ARACHNE-X  //  SPIDER-MECH";

    [Header("References")]
    [SerializeField] private BossHealthManager health;
    [SerializeField] private Animator animator;
    [SerializeField] private WeakPoint coreWeakPoint;
    [SerializeField] private Light coreGlow;
    [SerializeField] private ArenaHazardManager arenaHazards;
    [SerializeField] private SpiderMechFxLibrary fx;

    [Header("Route - its own patrol loop (no chasing, no retreating)")]
    [SerializeField] private Vector3 routeCenter = new Vector3(358.5f, 2.42f, 248.5f);
    [SerializeField] private Vector2 routeRadii = new Vector2(4.5f, 2.0f);
    [SerializeField] private int routePointCount = 12;
    [Tooltip("Optional hand-placed waypoints. When set they replace the generated ellipse.")]
    [SerializeField] private Transform[] routeWaypoints;
    [SerializeField] private float moveSpeed = 1.8f;
    [SerializeField] private float phaseTwoMoveSpeed = 2.4f;
    [SerializeField] private float acceleration = 3f;
    [SerializeField] private float turnSpeed = 75f;
    [SerializeField, Range(0f, 1f)] private float pauseChanceAtWaypoint = 0.2f;
    [Tooltip("Ground speed the walk cycle covers at animation speed 1 and model scale 1. Used to keep the feet planted.")]
    [SerializeField] private float strideSpeedAtScaleOne = 2.6f;

    // Balanced for a 100 HP player (regen 20 HP/s after 3 s): a player standing still in the open
    // takes ~5 DPS in phase 1 / ~8 DPS in phase 2; one who strafes or uses cover takes far less.
    [Header("Plasma barrage (basic attack)")]
    [SerializeField] private float boltDamage = 5f;
    [SerializeField] private float boltSpeed = 30f;
    [SerializeField] private int boltsPerVolley = 3;
    [SerializeField] private int phaseTwoBoltsPerVolley = 4;
    [SerializeField] private float volleyInterval = 2.6f;
    [SerializeField] private float phaseTwoVolleyInterval = 2.1f;
    [SerializeField] private float shotSpacing = 0.09f;
    [SerializeField] private float spreadDegrees = 3.2f;
    [SerializeField, Range(0f, 1f)] private float leadFactor = 0.3f;
    [SerializeField, Range(0f, 1f)] private float phaseTwoLeadFactor = 0.5f;
    [SerializeField] private float maxEngageRange = 60f;

    [Header("Point-blank stomp (punishes hugging the legs)")]
    [SerializeField] private float stompRange = 3.6f;
    [SerializeField] private float stompDamage = 10f;
    [SerializeField] private float stompCooldown = 4f;
    [SerializeField] private float stompLaunchSpeed = 5f;

    [Header("Mechanic 2 - anti-camping incendiary mortar")]
    [SerializeField] private float campRadius = 5f;
    [SerializeField] private float campTimeThreshold = 4f;
    [SerializeField] private float positionSampleInterval = 0.25f;
    [SerializeField] private float mortarCooldown = 9f;
    [SerializeField] private float mortarFlightTime = 1.5f;
    [SerializeField] private float mortarApexHeight = 9f;
    [SerializeField] private float mortarRadius = 4f;
    [SerializeField] private float mortarImpactDamage = 12f;
    [SerializeField] private float fireZoneDamagePerSecond = 6f;
    [SerializeField] private float fireZoneDuration = 6f;
    [Tooltip("Phase 2 also lobs a mortar at the player on this timer, camping or not.")]
    [SerializeField] private float phaseTwoMortarInterval = 15f;

    [Header("Mechanic 3 - Ultimate (DPS check)")]
    [SerializeField] private float firstUltimateDelay = 30f;
    [SerializeField] private float ultimateInterval = 40f;
    [SerializeField] private float windUpDuration = 1.5f;
    [SerializeField] private float chargeDuration = 6f;
    [Tooltip("Damage that must land on the exposed core during the charge. 450 = ~85% of what a player " +
             "with a full AR magazine and 80% accuracy lands in the window (see setup guide).")]
    [SerializeField] private float interruptThreshold = 450f;
    [SerializeField] private float stunDuration = 5f;
    [SerializeField] private float vulnerableDamageMultiplier = 2f;
    [SerializeField, Range(0f, 1f)] private float shockwaveMaxHealthPercent = 0.6f;
    [SerializeField, Range(0f, 1f)] private float shockwaveDodgedMultiplier = 0.15f;
    [SerializeField] private float shockwaveSpeed = 24f;
    [SerializeField] private float shockwaveMaxRadius = 32f;

    [Header("Mechanic 4 - Phase 2")]
    [SerializeField, Range(0f, 1f)] private float phaseTwoHealthThreshold = 0.5f;

    [Header("Intro")]
    [SerializeField] private bool startDormant = true;
    [Tooltip("Fallback trigger if no BossEncounter drives the fight: engage when the player gets this close.")]
    [SerializeField] private float autoEngageRadius = 11f;
    [SerializeField] private float introDropHeight = 26f;
    [SerializeField] private float introFallTime = 0.75f;

    [Header("Audio mix")]
    [SerializeField] private float servoVolume = 0.45f;
    [SerializeField] private float weaponVolume = 1f;
    [SerializeField] private float footstepVolume = 0.9f;

    private static readonly string[] ShotgunBones = { "ShotgunTop_L", "ShotgunTop_R", "ShotgunBot_L", "ShotgunBot_R" };
    private static readonly string[] FootBones =
    {
        "FrontLegEnd_L", "FrontLegEnd_R", "MiddleLegEnd_L", "MiddleLegEnd_R", "BackLegEnd_L", "BackLegEnd_R"
    };

    private BossState state = BossState.Dormant;

    private Transform player;
    private PlayerHealth playerHealth;
    private Collider playerCollider;
    private Rigidbody playerBody;
    private PlayerHitFeedback playerFeedback;

    private Transform bodyCenter;
    private Transform[] muzzles;
    private Transform[] feet;
    private float[] footBaseline;
    private bool[] footLifted;
    private float lastFootstepTime;

    private readonly List<Vector3> route = new List<Vector3>();
    private int routeIndex;
    private int routeDirection = 1;
    private float pauseTimer;
    private Vector3 velocity;
    private float turnRate;

    private float nextVolleyTime;
    private float nextStompTime;
    private float nextUltimateTime;
    private float nextPeriodicMortar;
    private float lastMortarTime = -999f;
    private readonly Queue<(float time, Vector3 pos)> positionHistory = new Queue<(float, Vector3)>();
    private float nextPositionSample;

    private float coreDamage;
    private float chargeElapsed;
    private bool phaseTwo;
    private int brokenJoints;

    private AudioSource servoSource;
    private AudioSource chargeSource;
    private GameObject shieldBubble;
    private GameObject channelAura;
    private float lastShieldHitTime = -99f;
    private readonly List<GameObject> activeLoops = new List<GameObject>();
    private Renderer[] renderers;
    private Light[] lights;
    private Collider[] colliders;
    private bool drivenByEncounter;

    public event Action Engaged;
    public event Action PhaseTwoStarted;
    public event Action Defeated;

    public BossState State => state;
    public string BossName => bossName;
    public bool IsEngaged => state != BossState.Dormant && state != BossState.Intro;
    public bool IsWindingUp => state == BossState.WindUp;
    public bool IsCharging => state == BossState.Charging;
    public bool IsStunned => state == BossState.Stunned;
    public bool IsDead => state == BossState.Dead;
    public bool InPhaseTwo => phaseTwo;
    public float CoreDamage => coreDamage;
    public float InterruptThreshold => interruptThreshold;
    public float ChargeProgress => interruptThreshold <= 0f ? 0f : Mathf.Clamp01(coreDamage / interruptThreshold);
    public float ChargeTimeLeft => Mathf.Max(0f, chargeDuration - chargeElapsed);
    public int BrokenJoints => brokenJoints;
    public Transform BodyCenter => bodyCenter;

    private float ModelScale => transform.lossyScale.x;
    private float CurrentMoveSpeed => (phaseTwo ? phaseTwoMoveSpeed : moveSpeed) * Mathf.Max(0.55f, 1f - 0.08f * brokenJoints);

    // =====================================================================================
    // Setup
    // =====================================================================================

    private void Awake()
    {
        if (health == null) health = GetComponent<BossHealthManager>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator != null) animator.applyRootMotion = false;

        health.Died += OnDied;
        health.Damaged += OnDamaged;
        health.ShieldBroken += OnShieldBroken;

        AcquirePlayer();

        bodyCenter = FindBone("Top_M") ?? transform;
        muzzles = FindBones(ShotgunBones);
        feet = FindBones(FootBones);
        footBaseline = new float[feet.Length];
        footLifted = new bool[feet.Length];
        for (int i = 0; i < feet.Length; i++) footBaseline[i] = feet[i].position.y - transform.position.y;

        if (coreWeakPoint == null)
            foreach (var wp in GetComponentsInChildren<WeakPoint>(true))
                if (wp.IsChargeCore) coreWeakPoint = wp;
        if (coreWeakPoint != null) coreWeakPoint.SetExposed(false);

        BuildRoute();
        BuildAudio();

        renderers = GetComponentsInChildren<Renderer>(true);
        lights = GetComponentsInChildren<Light>(true);
        colliders = GetComponentsInChildren<Collider>(true);

        if (fx != null && fx.shieldBubble != null)
        {
            shieldBubble = BossFx.Loop(fx.shieldBubble, bodyCenter.position, Quaternion.identity, 0.55f * ModelScale / 0.4f, bodyCenter);
            shieldBubble.SetActive(false);
        }

        if (startDormant) SetPresence(false);
        else EnterCombatImmediately();
    }

    private void Start()
    {
        drivenByEncounter = FindFirstObjectByType<BossEncounter>() != null;
    }

    private void AcquirePlayer()
    {
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go == null) return;
        player = go.transform;
        playerHealth = go.GetComponent<PlayerHealth>();
        playerCollider = go.GetComponentInChildren<Collider>();
        playerBody = go.GetComponent<Rigidbody>();
        playerFeedback = go.GetComponent<PlayerHitFeedback>();

        var character = go.GetComponent<InfimaGames.LowPolyShooterPack.CharacterBehaviour>();
        if (character != null && character.GetCameraWorld() != null)
            CameraShake.Ensure(character.GetCameraWorld().transform);
    }

    private Transform FindBone(string boneName)
    {
        foreach (var t in GetComponentsInChildren<Transform>(true))
            if (t.name == boneName) return t;
        return null;
    }

    private Transform[] FindBones(string[] names)
    {
        var list = new List<Transform>();
        foreach (var n in names)
        {
            var t = FindBone(n);
            if (t != null) list.Add(t);
        }
        return list.ToArray();
    }

    private void BuildRoute()
    {
        route.Clear();
        if (routeWaypoints != null && routeWaypoints.Length >= 2)
        {
            foreach (var w in routeWaypoints)
                if (w != null) route.Add(w.position);
        }
        else
        {
            int n = Mathf.Max(4, routePointCount);
            for (int i = 0; i < n; i++)
            {
                float a = i * Mathf.PI * 2f / n;
                route.Add(routeCenter + new Vector3(Mathf.Cos(a) * routeRadii.x, 0f, Mathf.Sin(a) * routeRadii.y));
            }
        }
    }

    private void BuildAudio()
    {
        servoSource = gameObject.AddComponent<AudioSource>();
        ConfigureLoop(servoSource, fx != null ? fx.servoLoop : null, 8f, 70f);

        chargeSource = gameObject.AddComponent<AudioSource>();
        ConfigureLoop(chargeSource, fx != null ? fx.chargeLoop : null, 10f, 120f);
    }

    private static void ConfigureLoop(AudioSource src, AudioClip clip, float minD, float maxD)
    {
        src.clip = clip;
        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 1f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.minDistance = minD;
        src.maxDistance = maxD;
        src.dopplerLevel = 0f;
        src.volume = 0f;
    }

    private void SetPresence(bool present)
    {
        foreach (var r in renderers) if (r != null) r.enabled = present;
        foreach (var l in lights) if (l != null) l.enabled = present;
        foreach (var c in colliders)
            if (c != null && (coreWeakPoint == null || c.gameObject != coreWeakPoint.gameObject))
                c.enabled = present;
        if (animator != null) animator.enabled = present;
    }

    private void EnterCombatImmediately()
    {
        SetPresence(true);
        state = BossState.Combat;
        nextUltimateTime = Time.time + firstUltimateDelay;
        nextVolleyTime = Time.time + 1.5f;
        if (servoSource.clip != null) servoSource.Play();
        Engaged?.Invoke();
    }

    // =====================================================================================
    // Encounter start / intro
    // =====================================================================================

    /// <summary>Called by BossEncounter when the player steps into the arena.</summary>
    public void BeginEncounter()
    {
        if (state != BossState.Dormant) return;
        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        state = BossState.Intro;
        health.Invulnerable = true;

        Vector3 landPos = routeCenter;
        landPos.y = transform.position.y;
        FaceInstant(player != null ? player.position : landPos + Vector3.back);
        transform.position = landPos + Vector3.up * introDropHeight;
        SetPresence(true);
        if (animator != null) animator.Play("Locomotion", 0, 0f);

        if (fx != null) BossFx.Sfx(fx.powerUp, landPos, 1f, 0.55f, 0f, 300f, 0.3f);
        CameraShake.Shake(landPos, 0.25f, 0f);
        yield return new WaitForSeconds(0.6f);

        Vector3 high = transform.position;
        float t = 0f;
        while (t < introFallTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / introFallTime);
            transform.position = Vector3.Lerp(high, landPos, k * k);
            yield return null;
        }
        transform.position = landPos;

        PlayImpactPose();
        if (fx != null)
        {
            BossFx.Play(fx.landingDust, landPos + Vector3.up * 0.1f, Quaternion.identity, 2.6f, 0.4f);
            BossFx.Play(fx.landingBlast, landPos + Vector3.up * 0.2f, Quaternion.identity, 1.4f, 0.35f);
            BossFx.Sfx(fx.bigExplosion, landPos, 1f, 0.8f, 10f, 200f, 0.6f);
            BossFx.Sfx(fx.heavyImpact, landPos, 1f, 0.6f, 10f, 150f);
        }
        BossFx.Flash(landPos + Vector3.up * 2f, new Color(1f, 0.6f, 0.3f), 8f, 25f, 0.5f);
        CameraShake.Shake(landPos, 1f, 45f);
        PopPlayerIfClose(landPos, 6f, 8f);

        yield return new WaitForSeconds(2.1f);

        if (animator != null) animator.CrossFade("Locomotion", 0.25f);
        if (servoSource.clip != null) servoSource.Play();
        health.Invulnerable = false;
        state = BossState.Combat;
        nextUltimateTime = Time.time + firstUltimateDelay;
        nextVolleyTime = Time.time + 0.8f;
        routeIndex = NearestRouteIndex();
        Engaged?.Invoke();
    }

    // =====================================================================================
    // Main loop
    // =====================================================================================

    private void Update()
    {
        if (player == null) AcquirePlayer();

        if (state == BossState.Dormant)
        {
            if (!drivenByEncounter && autoEngageRadius > 0f && player != null && FlatDistance(player.position, routeCenter) < autoEngageRadius)
                BeginEncounter();
            return;
        }
        if (state == BossState.Dead || state == BossState.Intro)
        {
            UpdateFeet(0f);
            return;
        }

        UpdateShieldBubble();

        switch (state)
        {
            case BossState.Combat:
                TickCombat();
                break;
            case BossState.WindUp:
            case BossState.Charging:
                Brake();
                FacePlayer(turnSpeed * 0.6f);
                break;
            default:
                Brake();
                break;
        }

        UpdateChannelAura(state == BossState.Combat && FloorIsLive);
        ApplyLocomotion();
        UpdateFeet(velocity.magnitude);
        UpdateServo();
    }

    private void TickCombat()
    {
        if (playerHealth != null && playerHealth.IsDead)
        {
            Brake();
            return;
        }

        FollowRoute();
        FacePlayer(turnSpeed);

        if (!phaseTwo && health.HealthNormalized <= phaseTwoHealthThreshold)
        {
            StartCoroutine(PhaseShift());
            return;
        }

        if (Time.time >= nextUltimateTime)
        {
            StartCoroutine(UltimateSequence());
            return;
        }

        float distance = FlatDistance(player.position, transform.position);
        if (distance < stompRange * ModelScale / 0.4f && Time.time >= nextStompTime)
        {
            StartCoroutine(Stomp());
            return;
        }

        // While the floor is live the mech pours its power into it and holds fire - the player's
        // reward for reaching high ground is a free damage window.
        if (!FloorIsLive && Time.time >= nextVolleyTime && distance <= maxEngageRange && HasLineOfSight())
        {
            int count = phaseTwo ? phaseTwoBoltsPerVolley : boltsPerVolley;
            nextVolleyTime = Time.time + (phaseTwo ? phaseTwoVolleyInterval : volleyInterval) + count * shotSpacing;
            StartCoroutine(FireVolley(count));
        }

        UpdateCampingDetection();

        // No mortar while the floor pins the player to a platform - that damage would be unavoidable.
        if (phaseTwo && Time.time >= nextPeriodicMortar && !FloorForcesHighGround)
        {
            nextPeriodicMortar = Time.time + phaseTwoMortarInterval;
            LaunchMortar(PredictGroundPoint(mortarFlightTime * 0.6f));
        }
    }

    // =====================================================================================
    // Movement
    // =====================================================================================

    private void FollowRoute()
    {
        if (route.Count == 0) return;

        Vector3 desired = Vector3.zero;
        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
        }
        else
        {
            Vector3 target = route[routeIndex];
            Vector3 to = target - transform.position;
            to.y = 0f;
            if (to.magnitude < 0.3f)
            {
                routeIndex = (routeIndex + routeDirection + route.Count) % route.Count;
                if (UnityEngine.Random.value < pauseChanceAtWaypoint) pauseTimer = UnityEngine.Random.Range(0.6f, 1.2f);
            }
            else
            {
                desired = to.normalized * CurrentMoveSpeed;
            }
        }

        velocity = Vector3.MoveTowards(velocity, desired, acceleration * Time.deltaTime);
        Vector3 next = transform.position + velocity * Time.deltaTime;
        next.y = routeCenter.y;
        transform.position = next;
    }

    /// <summary>
    /// A_MechISO_Landing starts ~40 m up in the sky (it's a drop-in clip) and touches down at ~0.63 s.
    /// Every impact reaction starts it from that contact frame so the mech crouches/recoils in place
    /// instead of popping into the air.
    /// </summary>
    private const float LandingContactTime = 0.63f;

    private void PlayImpactPose()
    {
        if (animator != null) animator.CrossFadeInFixedTime("Landing", 0.1f, 0, LandingContactTime);
    }

    /// <summary>Short physical hop (transform-driven, the clips are in-place) ending in a slam.</summary>
    private IEnumerator Leap(float height, float riseTime, float fallTime)
    {
        Vector3 start = transform.position;
        float t = 0f;
        while (t < riseTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Sin(Mathf.Clamp01(t / riseTime) * Mathf.PI * 0.5f);
            transform.position = new Vector3(transform.position.x, start.y + height * k, transform.position.z);
            yield return null;
        }
        t = 0f;
        while (t < fallTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fallTime);
            transform.position = new Vector3(transform.position.x, start.y + height * (1f - k * k), transform.position.z);
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, start.y, transform.position.z);
        PlayImpactPose();
    }

    private void Brake()
    {
        velocity = Vector3.MoveTowards(velocity, Vector3.zero, acceleration * 2f * Time.deltaTime);
        transform.position += velocity * Time.deltaTime;
    }

    private int NearestRouteIndex()
    {
        int best = 0;
        float bestD = float.MaxValue;
        for (int i = 0; i < route.Count; i++)
        {
            float d = FlatDistance(route[i], transform.position);
            if (d < bestD) { bestD = d; best = i; }
        }
        return best;
    }

    private void FacePlayer(float degreesPerSecond)
    {
        if (player == null) { turnRate = 0f; return; }
        Vector3 flat = player.position - transform.position;
        flat.y = 0f;
        if (flat.sqrMagnitude < 0.01f) { turnRate = 0f; return; }

        Quaternion before = transform.rotation;
        transform.rotation = Quaternion.RotateTowards(before, Quaternion.LookRotation(flat.normalized), degreesPerSecond * Time.deltaTime);
        turnRate = Mathf.DeltaAngle(before.eulerAngles.y, transform.eulerAngles.y) / Mathf.Max(0.0001f, Time.deltaTime);
    }

    private void FaceInstant(Vector3 worldTarget)
    {
        Vector3 flat = worldTarget - transform.position;
        flat.y = 0f;
        if (flat.sqrMagnitude > 0.01f) transform.rotation = Quaternion.LookRotation(flat.normalized);
    }

    /// <summary>Feeds the 2D strafe blend tree from the boss's local velocity, and scales the walk
    /// cycle's playback so the feet cover the same ground the body does (no skating).</summary>
    private void ApplyLocomotion()
    {
        if (animator == null || !animator.enabled) return;

        float speed = velocity.magnitude;
        Vector3 local = transform.InverseTransformDirection(velocity);
        Vector2 dir = speed > 0.05f ? new Vector2(local.x, local.z).normalized : Vector2.zero;
        float animSpeed = 1f;

        if (speed > 0.05f)
        {
            animSpeed = Mathf.Clamp(speed / Mathf.Max(0.01f, strideSpeedAtScaleOne * ModelScale), 0.6f, 2.6f);
        }
        else if (Mathf.Abs(turnRate) > 20f)
        {
            // Turning on the spot: shuffle the legs sideways instead of skating.
            dir = new Vector2(Mathf.Sign(turnRate) * 0.7f, 0f);
            animSpeed = 0.9f;
        }

        if (state == BossState.Stunned) animSpeed = 0.35f;

        animator.SetFloat("MoveX", dir.x, 0.18f, Time.deltaTime);
        animator.SetFloat("MoveZ", dir.y, 0.18f, Time.deltaTime);
        animator.SetFloat("LocoSpeed", animSpeed);
    }

    // =====================================================================================
    // Footsteps (detected from the actual foot bones, so sound + dust land exactly on contact)
    // =====================================================================================

    private void UpdateFeet(float speed)
    {
        if (feet == null || feet.Length == 0) return;

        float lift = 0.08f * ModelScale / 0.4f;
        float contact = 0.03f * ModelScale / 0.4f;
        for (int i = 0; i < feet.Length; i++)
        {
            float rel = feet[i].position.y - transform.position.y;
            footBaseline[i] = Mathf.Min(footBaseline[i] + Time.deltaTime * 0.02f, rel);

            if (!footLifted[i] && rel > footBaseline[i] + lift)
            {
                footLifted[i] = true;
            }
            else if (footLifted[i] && rel < footBaseline[i] + contact)
            {
                footLifted[i] = false;
                if (state != BossState.Intro && state != BossState.Dead) Footfall(feet[i].position, speed);
            }
        }
    }

    private void Footfall(Vector3 position, float speed)
    {
        if (fx == null || Time.time - lastFootstepTime < 0.07f) return;
        lastFootstepTime = Time.time;

        float intensity = Mathf.Clamp01(0.45f + speed / Mathf.Max(0.1f, moveSpeed) * 0.55f);
        BossFx.Sfx(BossFx.Pick(fx.footsteps), position, footstepVolume * intensity, UnityEngine.Random.Range(0.55f, 0.7f), 5f, 60f);
        BossFx.Play(fx.footstepDust, position, Quaternion.identity, 0.35f * ModelScale / 0.4f, 0.05f);
        CameraShake.Shake(position, 0.1f * intensity, 12f);
    }

    private void UpdateServo()
    {
        if (servoSource == null || servoSource.clip == null) return;
        float speed01 = Mathf.Clamp01(velocity.magnitude / Mathf.Max(0.1f, phaseTwoMoveSpeed));
        float turn01 = Mathf.Clamp01(Mathf.Abs(turnRate) / 90f);
        float charge = state == BossState.Charging ? chargeElapsed / chargeDuration : 0f;
        servoSource.pitch = 0.7f + 0.35f * Mathf.Max(speed01, turn01) + charge * 0.6f + (phaseTwo ? 0.1f : 0f);
        servoSource.volume = Mathf.MoveTowards(servoSource.volume, servoVolume * (0.55f + 0.45f * Mathf.Max(speed01, turn01)), Time.deltaTime);
    }

    // =====================================================================================
    // Basic attack - plasma barrage
    // =====================================================================================

    private IEnumerator FireVolley(int count)
    {
        if (muzzles.Length == 0) yield break;
        float lead = phaseTwo ? phaseTwoLeadFactor : leadFactor;

        for (int i = 0; i < count; i++)
        {
            if (state != BossState.Combat || player == null) yield break;

            Transform m = muzzles[i % muzzles.Length];
            Vector3 origin = m.position + transform.forward * 0.3f;
            Vector3 aim = PredictAimPoint(origin, boltSpeed, lead);
            Vector3 dir = (aim - origin).normalized;
            dir = Quaternion.AngleAxis(UnityEngine.Random.Range(-spreadDegrees, spreadDegrees), Vector3.up)
                * Quaternion.AngleAxis(UnityEngine.Random.Range(-spreadDegrees, spreadDegrees) * 0.5f, transform.right) * dir;

            BossProjectile.Spawn(origin, dir, boltSpeed, boltDamage, 0.18f, transform, fx, 0.55f);

            if (fx != null)
            {
                BossFx.Play(fx.muzzleFlash, origin, Quaternion.LookRotation(dir), 0.45f, 0.05f);
                BossFx.Sfx(BossFx.Pick(fx.plasmaFire), origin, weaponVolume, UnityEngine.Random.Range(0.9f, 1.08f), 8f, 120f);
            }
            BossFx.Flash(origin, new Color(1f, 0.65f, 0.25f), 6f, 7f, 0.08f);
            CameraShake.Shake(origin, 0.05f, 25f);

            yield return new WaitForSeconds(shotSpacing);
        }
    }

    private Vector3 PlayerAimPoint => playerCollider != null && playerCollider.enabled
        ? playerCollider.bounds.center
        : player.position + Vector3.up * 0.5f;

    private Vector3 PredictAimPoint(Vector3 origin, float projectileSpeed, float lead)
    {
        Vector3 aim = PlayerAimPoint;
        if (playerBody != null)
        {
            Vector3 v = playerBody.linearVelocity;
            v.y = 0f;
            float time = Vector3.Distance(origin, aim) / Mathf.Max(1f, projectileSpeed);
            aim += v * time * lead;
        }
        return aim;
    }

    private Vector3 PredictGroundPoint(float seconds)
    {
        Vector3 p = player.position;
        if (playerBody != null)
        {
            Vector3 v = playerBody.linearVelocity;
            v.y = 0f;
            p += v * seconds;
        }
        return GroundAt(p);
    }

    private Vector3 GroundAt(Vector3 p)
    {
        Vector3 best = new Vector3(p.x, routeCenter.y, p.z);
        float nearest = float.MaxValue;
        foreach (var h in Physics.RaycastAll(new Vector3(p.x, p.y + 3f, p.z), Vector3.down, 8f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (h.collider.transform.IsChildOf(transform)) continue;
            if (player != null && h.collider.transform.root == player.root) continue;
            if (h.distance < nearest) { nearest = h.distance; best = h.point; }
        }
        return best;
    }

    private bool HasLineOfSight()
    {
        Vector3 from = bodyCenter.position;
        Vector3 to = PlayerAimPoint;
        Vector3 d = to - from;
        float len = d.magnitude;
        if (len < 0.01f) return true;

        foreach (var h in Physics.RaycastAll(from, d / len, len, ~0, QueryTriggerInteraction.Ignore))
        {
            if (h.collider.transform.IsChildOf(transform)) continue;
            if (h.collider.transform.root == player.root) continue;
            return false;
        }
        return true;
    }

    // =====================================================================================
    // Mechanic 2 - camping detection (player position history) + mortar
    // =====================================================================================

    private bool FloorIsLive => arenaHazards != null && arenaHazards.State == ArenaHazardManager.FloorState.Live;

    /// <summary>Crackling aura while the mech is feeding the electrified floor.</summary>
    private void UpdateChannelAura(bool on)
    {
        if (on == (channelAura != null)) return;
        if (on)
        {
            if (fx == null) return;
            channelAura = BossFx.Loop(fx.chargeAura, bodyCenter.position, Quaternion.identity, 1.2f * ModelScale / 0.4f, bodyCenter);
            if (channelAura != null) activeLoops.Add(channelAura);
        }
        else
        {
            ReleaseLoop(channelAura);
            channelAura = null;
        }
    }

    /// <summary>Phase 2: while the floor is charging/live the player is supposed to hold a platform.</summary>
    private bool FloorForcesHighGround => arenaHazards != null && arenaHazards.IsActive &&
        (arenaHazards.State == ArenaHazardManager.FloorState.Warning || arenaHazards.State == ArenaHazardManager.FloorState.Live);

    private void UpdateCampingDetection()
    {
        // Standing on a platform to dodge the electrified floor is the intended play, not camping.
        if (FloorForcesHighGround)
        {
            positionHistory.Clear();
            return;
        }
        if (Time.time < nextPositionSample) return;
        nextPositionSample = Time.time + positionSampleInterval;

        Vector3 flatPos = player.position;
        positionHistory.Enqueue((Time.time, flatPos));
        while (positionHistory.Count > 0 && positionHistory.Peek().time < Time.time - campTimeThreshold - positionSampleInterval)
            positionHistory.Dequeue();

        if (Time.time - lastMortarTime < mortarCooldown) return;
        if (positionHistory.Count == 0 || Time.time - positionHistory.Peek().time < campTimeThreshold) return;

        // Camping = every sample of the last campTimeThreshold seconds sits inside a campRadius circle.
        Vector3 centroid = Vector3.zero;
        foreach (var s in positionHistory) centroid += s.pos;
        centroid /= positionHistory.Count;
        foreach (var s in positionHistory)
            if (FlatDistance(s.pos, centroid) > campRadius) return;

        lastMortarTime = Time.time;
        positionHistory.Clear();
        LaunchMortar(GroundAt(centroid));
    }

    private void LaunchMortar(Vector3 groundTarget)
    {
        Vector3 origin = bodyCenter.position + Vector3.up * (1.2f * ModelScale / 0.4f);
        BossMortar.Launch(origin, groundTarget, mortarFlightTime, mortarApexHeight, mortarRadius,
            mortarImpactDamage, fireZoneDamagePerSecond, fireZoneDuration, fx);

        if (fx != null)
        {
            BossFx.Play(fx.muzzleFlash, origin, Quaternion.LookRotation(Vector3.up), 0.9f, 0.08f);
            BossFx.Sfx(fx.mortarLaunch, origin, weaponVolume, 0.6f, 10f, 140f);
        }
        BossFx.Flash(origin, new Color(1f, 0.5f, 0.2f), 8f, 10f, 0.15f);
        CameraShake.Shake(origin, 0.2f, 30f);
    }

    // =====================================================================================
    // Point-blank stomp
    // =====================================================================================

    private IEnumerator Stomp()
    {
        state = BossState.Stomp;
        nextStompTime = Time.time + stompCooldown;
        if (fx != null) BossFx.Sfx(fx.powerUp, transform.position, 0.7f, 1.3f, 6f, 60f);

        // Brief crouch as the tell, then a short hop and slam.
        yield return new WaitForSeconds(0.2f);
        yield return Leap(0.55f * ModelScale / 0.4f, 0.22f, 0.13f);
        if (state != BossState.Stomp) yield break;

        Vector3 c = transform.position;
        if (fx != null)
        {
            BossFx.Play(fx.landingDust, c + Vector3.up * 0.1f, Quaternion.identity, 1.6f, 0.3f);
            BossFx.Play(fx.landingBlast, c + Vector3.up * 0.2f, Quaternion.identity, 0.8f, 0.25f);
            BossFx.Sfx(fx.heavyImpact, c, 1f, 0.7f, 8f, 90f);
            BossFx.Sfx(BossFx.Pick(fx.explosions), c, 0.7f, 1.2f, 8f, 90f);
        }
        CameraShake.Shake(c, 0.7f, 20f);
        PopPlayerIfClose(c, stompRange * ModelScale / 0.4f + 1f, stompDamage);

        yield return new WaitForSeconds(0.7f);
        if (state != BossState.Stomp) yield break;
        if (animator != null) animator.CrossFade("Locomotion", 0.25f);
        state = BossState.Combat;
    }

    private void PopPlayerIfClose(Vector3 center, float radius, float damage)
    {
        if (player == null || playerHealth == null || playerHealth.IsDead) return;
        if (FlatDistance(player.position, center) > radius) return;

        playerHealth.ApplyDamage(Mathf.RoundToInt(damage));
        if (playerFeedback != null) playerFeedback.Notify(center);
        if (playerBody != null)
        {
            Vector3 v = playerBody.linearVelocity;
            v.y = stompLaunchSpeed;
            playerBody.linearVelocity = v;
        }
    }

    // =====================================================================================
    // Mechanic 3 - Ultimate: WindUp -> Charging -> Stunned | Shockwave
    // =====================================================================================

    private IEnumerator UltimateSequence()
    {
        state = BossState.WindUp;
        pauseTimer = 0f;

        if (fx != null) BossFx.Sfx(fx.powerUp, bodyCenter.position, 1f, 0.7f, 10f, 200f, 0.7f);
        var aura = fx != null ? BossFx.Loop(fx.chargeAura, bodyCenter.position, Quaternion.identity, 2.2f * ModelScale / 0.4f, bodyCenter) : null;
        if (aura != null) activeLoops.Add(aura);
        yield return new WaitForSeconds(windUpDuration);

        // ---- ChargingState ----
        state = BossState.Charging;
        coreDamage = 0f;
        chargeElapsed = 0f;
        if (coreWeakPoint != null) coreWeakPoint.SetExposed(true);

        Vector3 corePos = coreWeakPoint != null ? coreWeakPoint.transform.position : bodyCenter.position;
        var implosion = fx != null ? BossFx.Loop(fx.chargeImplosion, corePos, Quaternion.identity, 0.7f * ModelScale / 0.4f,
            coreWeakPoint != null ? coreWeakPoint.transform : bodyCenter) : null;
        if (implosion != null) activeLoops.Add(implosion);

        if (chargeSource.clip != null) { chargeSource.pitch = 0.6f; chargeSource.volume = 0.9f; chargeSource.Play(); }

        bool interrupted = false;
        while (chargeElapsed < chargeDuration)
        {
            chargeElapsed += Time.deltaTime;
            float k = chargeElapsed / chargeDuration;

            if (coreGlow != null)
            {
                coreGlow.intensity = Mathf.Lerp(4f, 28f, k) * (0.85f + 0.15f * Mathf.Sin(Time.time * 30f));
                coreGlow.range = Mathf.Lerp(3f, 9f, k) / Mathf.Max(0.01f, coreGlow.transform.lossyScale.x);
            }
            chargeSource.pitch = Mathf.Lerp(0.6f, 2.2f, k);
            CameraShake.Shake(bodyCenter.position, (0.15f + 0.5f * k) * Time.deltaTime, 60f);

            if (coreDamage >= interruptThreshold) { interrupted = true; break; }
            yield return null;
        }

        if (coreWeakPoint != null) coreWeakPoint.SetExposed(false);
        chargeSource.Stop();
        ReleaseLoop(implosion);
        ReleaseLoop(aura);

        if (interrupted) yield return StunnedState();
        else yield return ShockwaveState();

        if (state == BossState.Dead) yield break;
        routeDirection = -routeDirection;
        routeIndex = NearestRouteIndex();
        nextUltimateTime = Time.time + ultimateInterval;
        nextVolleyTime = Time.time + 1f;
        if (animator != null) animator.CrossFade("Locomotion", 0.3f);
        state = BossState.Combat;
    }

    /// <summary>Called by the core WeakPoint for every hit while it is exposed.</summary>
    public void ReportCoreDamage(float dealt)
    {
        if (state == BossState.Charging) coreDamage += dealt;
    }

    // ---- StunnedState ----
    private IEnumerator StunnedState()
    {
        state = BossState.Stunned;
        health.IncomingDamageMultiplier = vulnerableDamageMultiplier;

        Vector3 corePos = coreWeakPoint != null ? coreWeakPoint.transform.position : bodyCenter.position;
        if (fx != null)
        {
            BossFx.Play(fx.interruptBurst, corePos, Quaternion.identity, 1.2f, 0.25f);
            BossFx.Play(fx.weakPointHit, corePos, Quaternion.identity, 1.6f, 0.2f);
            BossFx.Sfx(fx.powerDown, corePos, 1f, 0.8f, 10f, 200f, 0.6f);
            BossFx.Sfx(BossFx.Pick(fx.explosions), corePos, 0.9f, 1.1f, 10f, 150f);
        }
        BossFx.Flash(corePos, new Color(1f, 0.9f, 0.4f), 14f, 12f, 0.4f);
        CameraShake.Shake(corePos, 0.6f, 40f);
        if (coreGlow != null) coreGlow.intensity = 0f;
        PlayImpactPose();

        var sparks = fx != null ? BossFx.Loop(fx.stunSparks, bodyCenter.position, Quaternion.identity, 1.4f * ModelScale / 0.4f, bodyCenter) : null;
        var smoke = fx != null ? BossFx.Loop(fx.stunSmoke, bodyCenter.position + Vector3.up * 0.3f, Quaternion.identity, 1.1f * ModelScale / 0.4f, bodyCenter) : null;
        if (sparks != null) activeLoops.Add(sparks);
        if (smoke != null) activeLoops.Add(smoke);

        float t = 0f;
        float nextClank = 0f;
        while (t < stunDuration)
        {
            t += Time.deltaTime;
            if (fx != null && Time.time >= nextClank)
            {
                nextClank = Time.time + UnityEngine.Random.Range(0.4f, 0.9f);
                BossFx.Sfx(BossFx.Pick(fx.metalDebris), bodyCenter.position, 0.6f, UnityEngine.Random.Range(0.7f, 1f), 6f, 60f);
                BossFx.Play(fx.stunSparks, bodyCenter.position + UnityEngine.Random.insideUnitSphere * 0.8f, Quaternion.identity, 0.6f, 0.1f);
            }
            yield return null;
        }

        ReleaseLoop(sparks);
        ReleaseLoop(smoke);
        health.IncomingDamageMultiplier = 1f;
        if (coreGlow != null) coreGlow.intensity = 4f;
        if (fx != null) BossFx.Sfx(fx.powerUp, bodyCenter.position, 0.9f, 0.8f, 10f, 150f);
    }

    // ---- Shockwave (fail condition) ----
    private IEnumerator ShockwaveState()
    {
        state = BossState.Shockwave;
        // Leap and slam - the jump itself is the player's cue to jump too.
        if (fx != null) BossFx.Sfx(fx.powerUp, transform.position, 1f, 0.7f, 10f, 200f, 0.5f);
        yield return Leap(1.3f * ModelScale / 0.4f, 0.3f, 0.14f);

        Vector3 c = transform.position;
        if (fx != null)
        {
            // The pack builds its "gather" core 3 units above the root; pull it into the mech's body.
            var core = BossFx.Play(fx.shockwaveCore, c + Vector3.up * 0.05f, Quaternion.identity, 2f, 0.45f);
            BossFx.ClampChildHeights(core, (bodyCenter.position.y - c.y) / 2f);
            BossFx.Play(fx.shockwaveRingFx, c + Vector3.up * 0.2f, Quaternion.identity, 2.2f, 0.5f);
            BossFx.Play(fx.landingDust, c + Vector3.up * 0.1f, Quaternion.identity, 3f, 0.4f);
            BossFx.Sfx(fx.bigExplosion, c, 1f, 0.75f, 20f, 300f, 0.5f);
            BossFx.Sfx(fx.heavyImpact, c, 1f, 0.5f, 10f, 200f, 0.5f);
        }
        BossFx.Flash(c + Vector3.up * 2f, new Color(1f, 0.25f, 0.1f), 20f, 40f, 0.7f);
        CameraShake.Shake(c, 1f, 0f);

        var ring = BuildShockwaveRing();
        float r = 0.5f;
        bool resolved = false;
        while (r < shockwaveMaxRadius)
        {
            r += shockwaveSpeed * Time.deltaTime;
            float fade = 1f - r / shockwaveMaxRadius;
            UpdateRing(ring, c, r, fade);

            if (!resolved && player != null && FlatDistance(player.position, c) <= r)
            {
                resolved = true;
                HitPlayerWithShockwave(c);
            }
            yield return null;
        }
        Destroy(ring.sharedMaterial);
        Destroy(ring.gameObject);
        yield return new WaitForSeconds(0.6f);
    }

    private void HitPlayerWithShockwave(Vector3 source)
    {
        if (playerHealth == null || playerHealth.IsDead) return;

        bool dodged = arenaHazards != null ? !arenaHazards.PlayerOnPlazaFloor() : !PlayerGrounded();
        float dmg = playerHealth.maxHealth * shockwaveMaxHealthPercent * (dodged ? shockwaveDodgedMultiplier : 1f);
        playerHealth.ApplyDamage(Mathf.RoundToInt(dmg));
        if (playerFeedback != null) playerFeedback.Notify(source);
        CameraShake.Shake(player.position, dodged ? 0.5f : 1f, 0f);
        if (fx != null) BossFx.Sfx(fx.heavyImpact, player.position, 1f, dodged ? 1.2f : 0.8f, 2f, 30f, 0.3f);
    }

    private bool PlayerGrounded()
    {
        foreach (var h in Physics.RaycastAll(player.position + Vector3.up * 0.25f, Vector3.down, 0.6f, ~0, QueryTriggerInteraction.Ignore))
            if (h.collider.transform.root != player.root) return true;
        return false;
    }

    private LineRenderer BuildShockwaveRing()
    {
        var go = new GameObject("ShockwaveRing");
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.loop = true;
        lr.positionCount = 72;
        lr.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.numCornerVertices = 2;
        return lr;
    }

    private void UpdateRing(LineRenderer lr, Vector3 c, float r, float fade)
    {
        lr.startWidth = lr.endWidth = Mathf.Lerp(0.2f, 1.1f, fade);
        var col = new Color(1f, Mathf.Lerp(0.2f, 0.6f, fade), 0.1f, fade);
        lr.startColor = lr.endColor = col;
        for (int i = 0; i < lr.positionCount; i++)
        {
            float a = i * Mathf.PI * 2f / lr.positionCount;
            lr.SetPosition(i, new Vector3(c.x + Mathf.Cos(a) * r, c.y + 0.35f, c.z + Mathf.Sin(a) * r));
        }
    }

    // =====================================================================================
    // Mechanic 4 trigger - Phase 2
    // =====================================================================================

    private IEnumerator PhaseShift()
    {
        state = BossState.PhaseShift;
        phaseTwo = true;
        health.Invulnerable = true;
        velocity = Vector3.zero;

        if (fx != null) BossFx.Sfx(fx.powerUp, transform.position, 1f, 0.5f, 20f, 300f, 0.5f);
        yield return Leap(1.6f * ModelScale / 0.4f, 0.35f, 0.15f);
        if (fx != null)
        {
            Vector3 ground = new Vector3(transform.position.x, routeCenter.y + 0.05f, transform.position.z);
            var burst = BossFx.Play(fx.phaseTwoBurst, ground, Quaternion.identity, 1.8f, 0.5f);
            BossFx.ClampChildHeights(burst, (bodyCenter.position.y - ground.y) / 1.8f);
            BossFx.Sfx(fx.bigExplosion, bodyCenter.position, 1f, 0.6f, 20f, 300f, 0.6f);
            BossFx.Sfx(fx.powerUp, bodyCenter.position, 1f, 0.45f, 20f, 300f, 0.6f);
        }
        BossFx.Flash(bodyCenter.position, new Color(1f, 0.15f, 0.05f), 18f, 30f, 0.8f);
        CameraShake.Shake(bodyCenter.position, 0.9f, 0f);
        if (coreGlow != null) coreGlow.color = new Color(1f, 0.35f, 0.1f);

        if (arenaHazards != null) arenaHazards.Activate();
        PhaseTwoStarted?.Invoke();

        yield return new WaitForSeconds(2.2f);

        health.Invulnerable = false;
        nextPeriodicMortar = Time.time + 4f;
        nextVolleyTime = Time.time + 0.6f;
        if (animator != null) animator.CrossFade("Locomotion", 0.25f);
        state = BossState.Combat;
    }

    // =====================================================================================
    // Reactions
    // =====================================================================================

    /// <summary>Called by a leg-joint WeakPoint when its durability runs out.</summary>
    public void OnJointBroken(WeakPoint joint)
    {
        brokenJoints++;
        Vector3 p = joint.transform.position;
        if (fx != null)
        {
            BossFx.Play(fx.jointBreak, p, Quaternion.identity, 0.9f, 0.3f);
            BossFx.Sfx(BossFx.Pick(fx.explosions), p, 0.9f, 1.25f, 8f, 120f);
            BossFx.Sfx(BossFx.Pick(fx.metalDebris), p, 1f, 0.8f, 6f, 80f);
            var sparks = BossFx.Loop(fx.stunSparks, p, Quaternion.identity, 0.45f, joint.transform);
            if (sparks != null) activeLoops.Add(sparks);
        }
        BossFx.Flash(p, new Color(1f, 0.6f, 0.2f), 10f, 8f, 0.3f);
        CameraShake.Shake(p, 0.5f, 30f);

        if (state == BossState.Combat) StartCoroutine(Stagger(1.1f));
    }

    private IEnumerator Stagger(float seconds)
    {
        state = BossState.Staggered;
        PlayImpactPose();
        yield return new WaitForSeconds(seconds);
        if (state != BossState.Staggered) yield break;
        if (animator != null) animator.CrossFade("Locomotion", 0.25f);
        state = BossState.Combat;
    }

    private void OnDamaged(float amount, Vector3 point, BossHealthManager.HitKind kind, bool absorbed)
    {
        if (absorbed) lastShieldHitTime = Time.time;
    }

    private void OnShieldBroken()
    {
        if (fx == null) return;
        BossFx.Play(fx.shieldBreak, bodyCenter.position, Quaternion.identity, 3.2f * ModelScale / 0.4f, 0.15f);
        BossFx.Sfx(fx.shieldHitClip, bodyCenter.position, 1f, 0.6f, 10f, 150f);
        BossFx.Sfx(BossFx.Pick(fx.metalDebris), bodyCenter.position, 0.9f, 1.3f, 10f, 120f);
        BossFx.Flash(bodyCenter.position, new Color(0.3f, 0.8f, 1f), 12f, 14f, 0.35f);
        CameraShake.Shake(bodyCenter.position, 0.35f, 40f);
    }

    private void UpdateShieldBubble()
    {
        if (shieldBubble == null) return;
        bool show = health.ShieldUp && (Time.time - lastShieldHitTime < 0.18f || health.IsShieldRegenerating);
        if (shieldBubble.activeSelf != show) shieldBubble.SetActive(show);
    }

    // =====================================================================================
    // Death
    // =====================================================================================

    private void OnDied()
    {
        StopAllCoroutines();
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        state = BossState.Dead;
        health.Invulnerable = true;
        health.IncomingDamageMultiplier = 1f;
        velocity = Vector3.zero;
        // StopAllCoroutines may have cut a leap short - put the wreck back on the ground.
        transform.position = new Vector3(transform.position.x, routeCenter.y, transform.position.z);
        channelAura = null;

        if (servoSource != null) servoSource.Stop();
        if (chargeSource != null) chargeSource.Stop();
        foreach (var loop in activeLoops) BossFx.Release(loop);
        activeLoops.Clear();
        if (shieldBubble != null) shieldBubble.SetActive(false);
        foreach (var wp in GetComponentsInChildren<WeakPoint>(true)) wp.Shutdown();
        if (coreGlow != null) coreGlow.enabled = false;
        if (arenaHazards != null) arenaHazards.Deactivate();

        PlayImpactPose();
        if (fx != null) BossFx.Sfx(fx.powerDown, bodyCenter.position, 1f, 0.6f, 20f, 300f, 0.6f);

        var bones = GetComponentsInChildren<Transform>();
        for (int i = 0; i < 9; i++)
        {
            Vector3 p = bones[UnityEngine.Random.Range(0, bones.Length)].position + UnityEngine.Random.insideUnitSphere * 0.3f;
            if (fx != null)
            {
                BossFx.Play(fx.deathExplosionSmall, p, Quaternion.identity, UnityEngine.Random.Range(1.1f, 1.6f), 0.25f);
                BossFx.Play(fx.jointBreak, p, Quaternion.identity, UnityEngine.Random.Range(0.5f, 0.8f), 0.15f);
                BossFx.Sfx(BossFx.Pick(fx.explosions), p, 0.9f, UnityEngine.Random.Range(0.9f, 1.2f), 10f, 160f);
                if (UnityEngine.Random.value < 0.5f) BossFx.Sfx(BossFx.Pick(fx.metalDebris), p, 0.8f, UnityEngine.Random.Range(0.8f, 1.1f), 6f, 80f);
            }
            BossFx.Flash(p, new Color(1f, 0.55f, 0.2f), 10f, 10f, 0.3f);
            CameraShake.Shake(p, 0.35f, 50f);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.18f, 0.32f));
        }

        if (animator != null) animator.CrossFade("Death", 0.15f);
        yield return new WaitForSeconds(1.1f);

        Vector3 c = bodyCenter.position;
        if (fx != null)
        {
            BossFx.Play(fx.deathExplosionBig, c, Quaternion.identity, 2.4f, 0.5f);
            BossFx.Play(fx.shockwaveRingFx, new Vector3(c.x, routeCenter.y + 0.2f, c.z), Quaternion.identity, 2.4f, 0.5f);
            BossFx.Play(fx.landingDust, new Vector3(c.x, routeCenter.y + 0.1f, c.z), Quaternion.identity, 3f, 0.5f);
            BossFx.Sfx(fx.bigExplosion, c, 1f, 0.7f, 20f, 400f, 0.4f);
            BossFx.Sfx(BossFx.Pick(fx.explosions), c, 1f, 0.6f, 20f, 300f, 0.5f);
            BossFx.Loop(fx.wreckFire, c, Quaternion.identity, 1.1f, bodyCenter);
            BossFx.Loop(fx.wreckSmoke, c + Vector3.up * 0.5f, Quaternion.identity, 1.6f, bodyCenter);
        }
        BossFx.Flash(c, new Color(1f, 0.5f, 0.15f), 25f, 45f, 1.2f);
        CameraShake.Shake(c, 1f, 0f);

        Defeated?.Invoke();
    }

    private void ReleaseLoop(GameObject loop)
    {
        if (loop == null) return;
        activeLoops.Remove(loop);
        BossFx.Release(loop);
    }

    // =====================================================================================
    // Helpers / gizmos
    // =====================================================================================

    private static float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.6f, 0.1f);
        if (routeWaypoints != null && routeWaypoints.Length >= 2)
        {
            for (int i = 0; i < routeWaypoints.Length; i++)
                if (routeWaypoints[i] != null && routeWaypoints[(i + 1) % routeWaypoints.Length] != null)
                    Gizmos.DrawLine(routeWaypoints[i].position, routeWaypoints[(i + 1) % routeWaypoints.Length].position);
            return;
        }

        int n = Mathf.Max(4, routePointCount);
        for (int i = 0; i < n; i++)
        {
            float a0 = i * Mathf.PI * 2f / n, a1 = (i + 1) * Mathf.PI * 2f / n;
            Vector3 p0 = routeCenter + new Vector3(Mathf.Cos(a0) * routeRadii.x, 0.1f, Mathf.Sin(a0) * routeRadii.y);
            Vector3 p1 = routeCenter + new Vector3(Mathf.Cos(a1) * routeRadii.x, 0.1f, Mathf.Sin(a1) * routeRadii.y);
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawSphere(p0, 0.2f);
        }
        Gizmos.color = new Color(1f, 0.2f, 0.1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, stompRange * ModelScale / 0.4f);
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.Died -= OnDied;
            health.Damaged -= OnDamaged;
            health.ShieldBroken -= OnShieldBroken;
        }
    }
}
