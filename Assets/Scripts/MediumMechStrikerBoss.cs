using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Side boss for Level 4: the MediumMechStrikerMasterPrefab. Tougher than any soldier or turret,
/// well short of the Spider-Mech. Three phases by health:
///   1  stop-and-shoot: a laser burst from the two arm lasers, then one SRM or LRM that bursts on impact.
///   2  (armor breached) smoking, adds a telegraphed LRM barrage - red rings mark where it lands.
///   3  (overdrive) sparking red, runs between firing spots, longer bursts, both launchers every
///      stop, and a ground-stomp shockwave when the player gets close.
/// It only ever moves across a walkable grid scanned from its own hall at start-up (floor under
/// the feet, room for the body, connected to the spawn point), so it can never walk through a
/// wall or off the station into space.
/// </summary>
public class MediumMechStrikerBoss : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string bossName = "MECH STRIKER";
    [SerializeField] private string bossTitle = "SIDE BOSS";

    [Header("Combat - Health")]
    [SerializeField] private int maxHealth = 200;
    [Tooltip("Share of health where phase 2 (armor breached: smoke, missile barrage) begins.")]
    [SerializeField, Range(0f, 1f)] private float phaseTwoBelow = 0.6f;

    [Header("Combat - Laser (burst per stop)")]
    [SerializeField] private int laserDamage = 6;
    [SerializeField, Range(0f, 1f)] private float laserHitChance = 0.5f;
    [SerializeField] private float laserShotInterval = 0.22f;
    [SerializeField] private int laserBurstCount = 4;

    [Header("Combat - SRM (fast, bursts on impact)")]
    [SerializeField] private int srmDamage = 14;
    [SerializeField, Range(0f, 1f)] private float srmHitChance = 0.55f;
    [SerializeField] private float srmSpeed = 42f;

    [Header("Combat - LRM (slower, arcing, bursts on impact)")]
    [SerializeField] private int lrmDamage = 12;
    [SerializeField, Range(0f, 1f)] private float lrmHitChance = 0.5f;
    [SerializeField] private float lrmSpeed = 28f;
    [SerializeField] private float missileBlastRadius = 2.6f;

    [Header("Missile barrage (phase 2+)")]
    [SerializeField] private int barrageMissiles = 5;
    [SerializeField] private int barrageDamage = 12;
    [SerializeField] private float barrageBlastRadius = 3f;
    [Tooltip("Seconds between the warning rings appearing and the missiles landing.")]
    [SerializeField] private float barrageWarning = 1.4f;
    [SerializeField] private float barrageSpread = 6f;
    [SerializeField] private float barrageCooldown = 10f;

    [Header("Stomp shockwave (phase 3, close range)")]
    [SerializeField] private float stompRange = 7f;
    [SerializeField] private int stompDamage = 16;
    [SerializeField] private float stompWindup = 0.7f;
    [SerializeField] private float stompCooldown = 7f;

    [Header("Stop-and-shoot pattern")]
    [Tooltip("Furthest it walks from one firing spot to the next.")]
    [SerializeField] private float strafeDistance = 7f;
    [Tooltip("Preferred distance to the player when picking a firing spot.")]
    [SerializeField] private Vector2 preferredRange = new Vector2(9f, 20f);
    [Tooltip("Pause after a stop's volley before walking to the next spot.")]
    [SerializeField] private float postFireHold = 0.4f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float turnSpeed = 90f;
    [SerializeField] private float patrolRadius = 8f;
    [SerializeField] private float waypointPause = 2f;

    [Header("Arena (keeps the mech inside its hall)")]
    [Tooltip("How far from the spawn point the walkable grid is scanned.")]
    [SerializeField] private float arenaScanRadius = 30f;
    [SerializeField] private float arenaCellSize = 1f;
    [Tooltip("Clearance the body needs from walls and props.")]
    [SerializeField] private float bodyRadius = 1.4f;
    [SerializeField] private float bodyHeight = 3.6f;
    [Tooltip("Optional hard limit: when set, only cells inside this box are walkable.")]
    [SerializeField] private BoxCollider arenaBounds;

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

    [Header("Effects")]
    [SerializeField] private GameObject explosionFx;
    [SerializeField] private GameObject bigExplosionFx;
    [SerializeField] private GameObject sparksFx;
    [SerializeField] private GameObject smokeFx;
    [SerializeField] private GameObject overdriveFx;
    [SerializeField] private GameObject shockwaveFx;
    [SerializeField] private float explosionFxScale = 1f;

    [SerializeField] private float removeDelayAfterDeath = 8f;

    [Header("Overdrive (phase 3)")]
    [Tooltip("Below this share of its health the mech fights harder: longer laser bursts, both missiles every stop, stomps, and it runs between firing spots.")]
    [SerializeField, Range(0f, 1f)] private float enrageBelow = 0.3f;
    [SerializeField] private int enragedExtraLaserShots = 2;
    [SerializeField] private float enragedSpeedMultiplier = 1.4f;

    [Header("On death - next objective")]
    [Tooltip("Same blue ObjectiveGlow rim every other mission marker uses, put on the door up to Level 5.")]
    [SerializeField] private ObjectiveGlow doorGlow;
    [SerializeField] private string nextMissionObjective = "Go up to Level 5";
    [Tooltip("Fires once, right after the objective and door glow switch over - hook the Level 5 exit to it.")]
    public UnityEvent onDefeated;

    private static readonly string[] HitClips = { "b1HitFront1", "b1HitFront2", "b1HitFront3" };
    private string lastPose;

    private Transform player;
    private PlayerHealth playerHealth;
    private Collider playerCollider;
    private PlayerHitFeedback hitFeedback;
    private Animator animator;
    private AudioSource audioSource;

    private Transform[] laserMuzzles;
    private Transform[] srmMuzzles;
    private Transform[] lrmMuzzles;
    private Transform torso;

    private LineRenderer laserCore;
    private LineRenderer laserGlow;
    private Light laserFlash;
    private Light impactFlash;
    private Light overdriveLight;
    private Material fxMaterial;

    private readonly List<GameObject> liveProjectiles = new List<GameObject>();
    private readonly List<GameObject> liveMarkers = new List<GameObject>();
    private GameObject damageSmoke;
    private GameObject overdriveSparks;

    private float lastSeenPlayerTime = -99f;
    private bool combatActive;
    private bool hasEngagedOnce;
    private bool useSrmNext = true;
    private int phase = 1;
    private float nextBarrageTime;
    private float nextStompTime;
    private float lastSparkTime = -99f;

    private string bannerText;
    private float bannerUntil = -99f;
    private float bannerStart;

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
        BuildLaserVisuals();

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
        home = transform.position;
        StartCoroutine(Brain());
    }

    private void AcquirePlayer()
    {
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go == null) return;
        player = go.transform;
        playerHealth = go.GetComponent<PlayerHealth>();
        playerCollider = go.GetComponentInChildren<Collider>();
        hitFeedback = go.GetComponent<PlayerHitFeedback>();

        var character = go.GetComponent<InfimaGames.LowPolyShooterPack.CharacterBehaviour>();
        if (character != null && character.GetCameraWorld() != null)
            CameraShake.Ensure(character.GetCameraWorld().transform);
    }

    private Vector3 PlayerAimPoint => playerCollider != null && playerCollider.enabled
        ? playerCollider.bounds.center
        : (player != null ? player.position + Vector3.up * 1f : Vector3.zero);

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
            else if (t.name == "UpperTorso") torso = t;
        }
        laserMuzzles = laser.ToArray();
        srmMuzzles = srm.ToArray();
        lrmMuzzles = lrm.ToArray();
        if (torso == null) torso = transform;
    }

    private void BuildLaserVisuals()
    {
        fxMaterial = new Material(Shader.Find("Sprites/Default"));

        laserGlow = MakeLine("LaserGlow", 0.34f, 0.12f, new Color(1f, 0.1f, 0.05f, 0.45f), new Color(1f, 0.05f, 0.02f, 0.05f));
        laserCore = MakeLine("LaserCore", 0.09f, 0.04f, new Color(1f, 0.9f, 0.8f, 1f), new Color(1f, 0.4f, 0.3f, 0.4f));

        laserFlash = MakeLight("LaserMuzzleFlash", new Color(1f, 0.2f, 0.15f), 9f);
        impactFlash = MakeLight("ImpactFlash", new Color(1f, 0.55f, 0.2f), 12f);
        overdriveLight = MakeLight("OverdriveGlow", new Color(1f, 0.1f, 0.05f), 10f);
        overdriveLight.transform.SetParent(torso, false);
    }

    private LineRenderer MakeLine(string name, float start, float end, Color a, Color b)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.startWidth = start;
        lr.endWidth = end;
        lr.sharedMaterial = fxMaterial;
        lr.startColor = a;
        lr.endColor = b;
        lr.numCapVertices = 4;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.enabled = false;
        return lr;
    }

    private Light MakeLight(string name, Color color, float range)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var l = go.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = color;
        l.range = range;
        l.intensity = 0f;
        l.shadows = LightShadows.None;
        return l;
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
        }
        else if (Time.time - lastSeenPlayerTime > loseSightGrace)
        {
            combatActive = false;
        }

        if (overdriveLight != null && phase >= 3)
            overdriveLight.intensity = 2.5f + 2f * Mathf.Sin(Time.time * 9f);

        // The hit clips have no way out, so step back into the walk/idle pose once one has played.
        if (animator != null && lastPose != null)
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.normalizedTime >= 0.95f && !animator.IsInTransition(0) && IsHitState(info))
                animator.CrossFade(lastPose, 0.15f);
        }
    }

    private static bool IsHitState(AnimatorStateInfo info)
    {
        foreach (string clip in HitClips)
            if (info.IsName(clip)) return true;
        return false;
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

    /// <summary>First solid surface along a ray, ignoring the mech itself and the player.</summary>
    private bool FirstSolidHit(Vector3 origin, Vector3 target, out RaycastHit best)
    {
        best = default;
        Vector3 d = target - origin;
        float len = d.magnitude;
        if (len < 0.01f) return false;
        float bestDist = float.MaxValue;
        foreach (var h in Physics.RaycastAll(origin, d / len, len, ~0, QueryTriggerInteraction.Ignore))
        {
            if (h.collider.transform.IsChildOf(transform)) continue;
            if (player != null && h.collider.transform.root == player.root) continue;
            if (h.distance < bestDist) { bestDist = h.distance; best = h; }
        }
        return bestDist < float.MaxValue;
    }

    private bool FacePlayer(float speedMultiplier = 1f)
    {
        if (player == null) return true;
        Vector3 flat = Flat(player.position - transform.position);
        if (flat.sqrMagnitude < 0.0001f) return true;
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            Quaternion.LookRotation(flat.normalized), turnSpeed * speedMultiplier * Time.deltaTime);
        return Vector3.Angle(transform.forward, flat) < 5f;
    }

    private void PlayAnimSafe(string state)
    {
        if (animator == null) return;
        lastPose = state;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(state)) return;
        animator.CrossFade(state, 0.15f);
    }

    private static Vector3 Flat(Vector3 v)
    {
        v.y = 0f;
        return v;
    }

    // =====================================================================================
    // Arena: a walkable grid scanned from the hall, so every step stays on the station floor.
    // =====================================================================================

    private Vector3 home;
    private float floorY;
    private Vector3 gridOrigin;
    private int gridSize;
    private bool[,] walkable;
    private readonly List<Vector2Int> walkableCells = new List<Vector2Int>();

    private void BuildArena()
    {
        Physics.SyncTransforms();
        floorY = transform.position.y;
        int r = Mathf.Max(2, Mathf.CeilToInt(arenaScanRadius / arenaCellSize));
        gridSize = 2 * r + 1;
        gridOrigin = new Vector3(home.x - r * arenaCellSize, floorY, home.z - r * arenaCellSize);
        var open = new bool[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
            for (int z = 0; z < gridSize; z++)
                open[x, z] = CellIsOpen(CellCenter(x, z));

        // Keep only what is connected to where the mech stands.
        walkable = new bool[gridSize, gridSize];
        walkableCells.Clear();
        Vector2Int start = new Vector2Int(r, r);
        if (!open[start.x, start.y] && !NearestWhere(open, start, out start))
        {
            Debug.LogWarning("[MechStriker] no walkable floor around the spawn point - it will stand still.");
            return;
        }
        var queue = new Queue<Vector2Int>();
        queue.Enqueue(start);
        walkable[start.x, start.y] = true;
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            walkableCells.Add(c);
            for (int i = 0; i < 4; i++)
            {
                var n = c + Neighbours4[i];
                if (!InGrid(n) || walkable[n.x, n.y] || !open[n.x, n.y]) continue;
                walkable[n.x, n.y] = true;
                queue.Enqueue(n);
            }
        }

        // Stand on the grid from the first frame, in case the spawn itself was in a wall.
        Vector3 snapped = CellCenter(start.x, start.y);
        if (!IsWalkable(transform.position))
            transform.position = new Vector3(snapped.x, floorY, snapped.z);
    }

    private static readonly Vector2Int[] Neighbours4 = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
    private static readonly Vector2Int[] Neighbours8 =
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
        new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)
    };

    private bool CellIsOpen(Vector3 p)
    {
        if (arenaBounds != null && !arenaBounds.bounds.Contains(new Vector3(p.x, arenaBounds.bounds.center.y, p.z)))
            return false;

        // Floor right under the feet, at the hall's height (not a gap, not a lower deck).
        bool floor = false;
        foreach (var h in Physics.RaycastAll(p + Vector3.up * 2.5f, Vector3.down, 4f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (IgnoredForArena(h.collider)) continue;
            if (Mathf.Abs(h.point.y - floorY) < 0.5f) { floor = true; break; }
        }
        if (!floor) return false;

        // Room for the whole body: no wall, door or turret inside the capsule.
        foreach (var col in Physics.OverlapCapsule(p + Vector3.up * (bodyRadius + 0.3f), p + Vector3.up * bodyHeight,
                     bodyRadius, ~0, QueryTriggerInteraction.Ignore))
            if (!IgnoredForArena(col)) return false;
        return true;
    }

    /// <summary>The mech itself, the player and anything animated (other units move around).</summary>
    private bool IgnoredForArena(Collider c)
    {
        if (c.transform.IsChildOf(transform)) return true;
        if (player != null && c.transform.root == player.root) return true;
        if (c.attachedRigidbody != null && !c.attachedRigidbody.isKinematic) return true;
        return c.GetComponentInParent<Animator>() != null;
    }

    private bool NearestWhere(bool[,] grid, Vector2Int from, out Vector2Int found)
    {
        found = from;
        float best = float.MaxValue;
        for (int x = 0; x < gridSize; x++)
            for (int z = 0; z < gridSize; z++)
            {
                if (!grid[x, z]) continue;
                float d = (new Vector2Int(x, z) - from).sqrMagnitude;
                if (d < best) { best = d; found = new Vector2Int(x, z); }
            }
        return best < float.MaxValue;
    }

    private bool InGrid(Vector2Int c) => c.x >= 0 && c.y >= 0 && c.x < gridSize && c.y < gridSize;

    private Vector3 CellCenter(int x, int z) =>
        new Vector3(gridOrigin.x + x * arenaCellSize, floorY, gridOrigin.z + z * arenaCellSize);

    private Vector3 CellCenter(Vector2Int c) => CellCenter(c.x, c.y);

    private Vector2Int CellOf(Vector3 p) => new Vector2Int(
        Mathf.RoundToInt((p.x - gridOrigin.x) / arenaCellSize),
        Mathf.RoundToInt((p.z - gridOrigin.z) / arenaCellSize));

    private bool IsWalkable(Vector3 p)
    {
        if (walkable == null) return false;
        var c = CellOf(p);
        return InGrid(c) && walkable[c.x, c.y];
    }

    private Vector2Int NearestWalkableCell(Vector3 p)
    {
        var c = CellOf(p);
        if (InGrid(c) && walkable[c.x, c.y]) return c;
        Vector2Int best = CellOf(transform.position);
        float bestD = float.MaxValue;
        foreach (var w in walkableCells)
        {
            float d = (CellCenter(w) - p).sqrMagnitude;
            if (d < bestD) { bestD = d; best = w; }
        }
        return best;
    }

    private bool WalkableLine(Vector3 a, Vector3 b)
    {
        float len = Vector3.Distance(Flat(a), Flat(b));
        int steps = Mathf.Max(1, Mathf.CeilToInt(len / (arenaCellSize * 0.4f)));
        for (int i = 0; i <= steps; i++)
            if (!IsWalkable(Vector3.Lerp(a, b, i / (float)steps))) return false;
        return true;
    }

    /// <summary>Breadth-first path over the grid (diagonals only past two open sides), then
    /// string-pulled so it walks in straight lines wherever the floor allows.</summary>
    private List<Vector3> FindPath(Vector3 from, Vector3 to)
    {
        var path = new List<Vector3>();
        if (walkable == null || walkableCells.Count == 0) return path;
        Vector2Int s = NearestWalkableCell(from), g = NearestWalkableCell(to);
        if (s == g) { path.Add(CellCenter(g)); return path; }

        var came = new Dictionary<Vector2Int, Vector2Int>();
        var queue = new Queue<Vector2Int>();
        queue.Enqueue(s);
        came[s] = s;
        bool reached = false;
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c == g) { reached = true; break; }
            foreach (var d in Neighbours8)
            {
                var n = c + d;
                if (!InGrid(n) || !walkable[n.x, n.y] || came.ContainsKey(n)) continue;
                if (d.x != 0 && d.y != 0 && (!walkable[c.x + d.x, c.y] || !walkable[c.x, c.y + d.y])) continue;
                came[n] = c;
                queue.Enqueue(n);
            }
        }
        if (!reached) return path;

        var cells = new List<Vector2Int>();
        for (var c = g; c != s; c = came[c]) cells.Add(c);
        cells.Reverse();

        Vector3 anchor = from;
        for (int i = 0; i < cells.Count; i++)
        {
            bool last = i == cells.Count - 1;
            if (last || !WalkableLine(anchor, CellCenter(cells[i + 1])))
            {
                path.Add(CellCenter(cells[i]));
                anchor = CellCenter(cells[i]);
            }
        }
        return path;
    }

    private IEnumerator WalkPath(List<Vector3> path, bool run, System.Func<bool> keepGoing)
    {
        if (path == null || path.Count == 0) yield break;
        PlayAnimSafe(run ? "a7RunCycle" : "a5WalkCycle");
        float speed = run ? moveSpeed * enragedSpeedMultiplier : moveSpeed;
        foreach (var point in path)
        {
            Vector3 target = new Vector3(point.x, floorY, point.z);
            while (!dead && keepGoing() && Vector3.Distance(Flat(transform.position), Flat(target)) > 0.3f)
            {
                // Never walk into the player; stop and deal with them instead.
                if (player != null && Vector3.Distance(Flat(player.position), Flat(transform.position)) < bodyRadius + 1.2f)
                    yield break;

                Vector3 dir = Flat(target - transform.position);
                if (dir.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation,
                        Quaternion.LookRotation(dir.normalized), turnSpeed * 1.5f * Time.deltaTime);
                Vector3 next = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                next.y = floorY;
                if (!IsWalkable(next)) yield break;   // belt and braces: the grid is the boundary
                transform.position = next;
                yield return null;
            }
            if (dead || !keepGoing()) yield break;
        }
    }

    /// <summary>A firing spot inside the hall: a comfortable range from the player, a clear shot,
    /// and within a short walk.</summary>
    private Vector3 ChooseCombatSpot()
    {
        Vector3 here = transform.position;
        Vector3 best = here;
        float bestScore = float.MinValue;
        for (int i = 0; i < 28 && walkableCells.Count > 0; i++)
        {
            Vector3 c = CellCenter(walkableCells[Random.Range(0, walkableCells.Count)]);
            float walk = Vector3.Distance(Flat(c), Flat(here));
            if (walk > strafeDistance || walk < 2.5f) continue;
            float range = Vector3.Distance(Flat(c), Flat(player.position));
            float score = -Mathf.Abs(range - Mathf.Clamp(range, preferredRange.x, preferredRange.y)) * 2f;
            if (HasClearLine(c + Vector3.up * 3f, PlayerAimPoint)) score += 10f;
            score += Random.value * 3f;
            if (score > bestScore) { bestScore = score; best = c; }
        }
        return best;
    }

    // =====================================================================================
    // Behaviour
    // =====================================================================================

    private int PhaseForHealth()
    {
        float share = health / (float)Mathf.Max(1, maxHealth);
        if (share <= enrageBelow) return 3;
        if (share <= phaseTwoBelow) return 2;
        return 1;
    }

    private IEnumerator Brain()
    {
        yield return null;          // let every other object finish its Start first
        BuildArena();

        while (!dead)
        {
            if (!combatActive)
            {
                yield return Patrol();
                continue;
            }

            if (!introPlayed)
            {
                introPlayed = true;
                yield return Intro();
                continue;
            }

            int wanted = PhaseForHealth();
            if (wanted > phase)
            {
                yield return PhaseShift(wanted);
                continue;
            }

            float toPlayer = player != null ? Vector3.Distance(Flat(player.position), Flat(transform.position)) : 99f;
            bool stompReady = Time.time >= nextStompTime && toPlayer <= stompRange && (phase >= 3 || toPlayer < bodyRadius + 2.5f);
            if (stompReady) yield return Stomp();
            else if (phase >= 2 && Time.time >= nextBarrageTime) yield return Barrage();
            else yield return StopAndFire();

            if (dead) yield break;
            if (!combatActive) continue;

            Vector3 spot = ChooseCombatSpot();
            yield return WalkPath(FindPath(transform.position, spot), phase >= 3, () => combatActive);
        }
    }

    private bool introPlayed;

    private IEnumerator Patrol()
    {
        if (walkableCells.Count == 0) { PlayAnimSafe("a4IdlePose"); yield return null; yield break; }

        // A random spot near home, inside the hall.
        Vector3 target = transform.position;
        for (int i = 0; i < 16; i++)
        {
            Vector3 c = CellCenter(walkableCells[Random.Range(0, walkableCells.Count)]);
            if (Vector3.Distance(Flat(c), Flat(home)) <= patrolRadius) { target = c; break; }
        }
        yield return WalkPath(FindPath(transform.position, target), false, () => !combatActive);
        if (combatActive || dead) yield break;
        PlayAnimSafe("a4IdlePose");
        float until = Time.time + waypointPause;
        while (!combatActive && !dead && Time.time < until) yield return null;
    }

    private IEnumerator Intro()
    {
        PlayAnimSafe("a4IdlePose");
        ShowBanner("WARNING: " + bossName);
        float t = 0f;
        while (t < 0.8f && !dead) { FacePlayer(2f); t += Time.deltaTime; yield return null; }
        StompBlast(false);   // a show of force: shakes the hall, no damage
        nextBarrageTime = Time.time + barrageCooldown * 0.5f;
        yield return new WaitForSeconds(0.6f);
    }

    private IEnumerator PhaseShift(int newPhase)
    {
        phase = newPhase;
        PlayAnimSafe("a4IdlePose");
        if (newPhase == 2)
        {
            ShowBanner("ARMOR BREACHED");
            BurstAt(torso.position, 1.4f);
            if (damageSmoke == null)
                damageSmoke = TimedEffect.Spawn(smokeFx, torso.position + Vector3.up * 0.5f, Quaternion.identity, 0.8f, -1f, 0f, torso);
            nextBarrageTime = Time.time + 1f;
        }
        else
        {
            ShowBanner("OVERDRIVE");
            BurstAt(torso.position, 1.8f);
            if (overdriveSparks == null)
                overdriveSparks = TimedEffect.Spawn(overdriveFx, torso.position, Quaternion.identity, 1f, -1f, 0f, torso);
            nextStompTime = Time.time;
            StompBlast(false);
        }
        yield return new WaitForSeconds(0.8f);
    }

    // ---------- stop-and-shoot ----------

    private IEnumerator StopAndFire()
    {
        PlayAnimSafe("a4IdlePose");

        float elapsed = 0f;
        while (!dead && combatActive && elapsed < 1.5f)
        {
            if (FacePlayer()) break;
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (dead || !combatActive) yield break;

        bool enraged = phase >= 3;
        int laserShots = laserBurstCount + (enraged ? enragedExtraLaserShots : 0);
        for (int i = 0; i < laserShots && !dead && combatActive; i++)
        {
            FacePlayer();
            FireLaser();
            yield return new WaitForSeconds(laserShotInterval);
        }
        if (dead || !combatActive) yield break;

        if (enraged)
        {
            FireMissile(srmMuzzles, srmProjectilePrefab, srmSpeed, srmDamage, srmHitChance, false);
            yield return new WaitForSeconds(0.25f);
            if (dead || !combatActive) yield break;
        }

        useSrmNext = enraged ? false : !useSrmNext;
        if (useSrmNext) FireMissile(srmMuzzles, srmProjectilePrefab, srmSpeed, srmDamage, srmHitChance, false);
        else FireMissile(lrmMuzzles, lrmProjectilePrefab, lrmSpeed, lrmDamage, lrmHitChance, true);

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

        // A miss keeps going until it strikes something behind the player.
        Vector3 end = aim;
        bool struck = false;
        RaycastHit wall;
        if (!hits)
        {
            Vector3 far = muzzle.position + (aim - muzzle.position).normalized * 60f;
            if (FirstSolidHit(muzzle.position, far, out wall)) { end = wall.point; struck = true; }
            else end = far;
        }

        StartCoroutine(ShowLaser(muzzle.position, end));
        if (struck || hits) SparksAt(end, 0.6f);
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
        impactFlash.transform.position = to;
        impactFlash.intensity = 6f;
        laserCore.SetPosition(0, from);
        laserCore.SetPosition(1, to);
        laserGlow.SetPosition(0, from);
        laserGlow.SetPosition(1, to);
        laserCore.enabled = laserGlow.enabled = true;
        float t = 0f;
        const float life = 0.09f;
        while (t < life && laserCore != null)
        {
            float k = 1f - t / life;
            laserCore.widthMultiplier = k;
            laserGlow.widthMultiplier = 0.6f + 0.4f * k;
            t += Time.deltaTime;
            yield return null;
        }
        if (laserCore != null) laserCore.enabled = laserGlow.enabled = false;
        if (laserFlash != null) laserFlash.intensity = 0f;
        if (impactFlash != null) impactFlash.intensity = 0f;
    }

    // ---------- missiles (SRM / LRM): real projectiles that burst where they land ----------

    private void FireMissile(Transform[] muzzles, GameObject prefab, float speed, int damage, float hitChance, bool arc)
    {
        if (muzzles == null || muzzles.Length == 0 || prefab == null || player == null) return;
        Transform muzzle = muzzles[Random.Range(0, muzzles.Length)];
        Vector3 target = PlayerAimPoint;
        // A miss still lands close - the blast is what you dodge. It flies on until it strikes
        // something (floor, wall, prop) instead of bursting in mid-air.
        if (Random.value > hitChance)
        {
            Vector3 off = target + Flat(Random.insideUnitSphere).normalized * Random.Range(2.5f, 4.5f) + Vector3.down * 1.2f;
            Vector3 far = muzzle.position + (off - muzzle.position).normalized * 60f;
            target = FirstSolidHit(muzzle.position, far, out RaycastHit h) ? h.point : GroundPoint(off);
        }
        // The hall's glass roof is only ~4 m up, so arcs stay low.
        LaunchMissile(muzzle.position, target, prefab, speed, damage, missileBlastRadius, arc ? 0.9f : 0.25f, 0f);
    }

    private GameObject LaunchMissile(Vector3 from, Vector3 target, GameObject prefab, float speed, int damage,
        float radius, float arcHeight, float fixedDuration)
    {
        GameObject proj = Instantiate(prefab, from, Quaternion.LookRotation((target - from).normalized));
        var trail = proj.AddComponent<TrailRenderer>();
        trail.time = 0.45f;
        trail.startWidth = 0.18f;
        trail.endWidth = 0.02f;
        trail.material = fxMaterial;
        trail.startColor = new Color(1f, 0.7f, 0.25f, 0.95f);
        trail.endColor = new Color(0.35f, 0.32f, 0.3f, 0f);
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        liveProjectiles.Add(proj);
        StartCoroutine(FlyMissile(proj, from, target, speed, damage, radius, arcHeight, fixedDuration));

        if (missileClips != null && missileClips.Length > 0)
        {
            audioSource.pitch = Random.Range(0.9f, 1.05f);
            audioSource.PlayOneShot(missileClips[Random.Range(0, missileClips.Length)]);
        }
        return proj;
    }

    private IEnumerator FlyMissile(GameObject proj, Vector3 start, Vector3 target, float speed, int damage,
        float radius, float arcHeight, float fixedDuration)
    {
        float distance = Vector3.Distance(start, target);
        float duration = fixedDuration > 0f ? fixedDuration : Mathf.Max(0.05f, distance / speed);
        float elapsed = 0f;
        Vector3 prev = start;
        Vector3 impact = target;
        while (elapsed < duration && proj != null)
        {
            elapsed += Time.deltaTime;
            float k = Mathf.Clamp01(elapsed / duration);
            Vector3 pos = Vector3.Lerp(start, target, k) + Vector3.up * (arcHeight * 4f * k * (1f - k));
            // Walls and props stop it early.
            if (FirstSolidHit(prev, pos, out RaycastHit h) && elapsed > 0.05f) { impact = h.point; break; }
            if (pos != prev) proj.transform.rotation = Quaternion.LookRotation((pos - prev).normalized);
            proj.transform.position = pos;
            prev = pos;
            yield return null;
        }
        if (proj != null) { liveProjectiles.Remove(proj); Destroy(proj); }
        Explode(impact, radius, damage, 1f);
    }

    private void Explode(Vector3 at, float radius, int damage, float fxScale)
    {
        TimedEffect.Spawn(explosionFx, at, Quaternion.identity, explosionFxScale * fxScale, 0.4f, 4f);
        StartCoroutine(FlashLight(impactFlash, at, 10f, 0.25f));
        if (explosionClip != null) AudioSource.PlayClipAtPoint(explosionClip, at, 0.7f);
        CameraShake.Shake(at, 0.45f * fxScale, 18f);
        DamagePlayerInBlast(at, radius, damage);
    }

    private void DamagePlayerInBlast(Vector3 center, float radius, int damage)
    {
        if (player == null || playerHealth == null || playerHealth.IsDead) return;
        Vector3 aim = PlayerAimPoint;
        float d = Vector3.Distance(center, aim);
        if (d > radius) return;
        if (!HasClearLine(center + Vector3.up * 0.4f, aim)) return;   // cover works
        int dealt = Mathf.Max(1, Mathf.RoundToInt(damage * Mathf.Lerp(1f, 0.4f, d / radius)));
        playerHealth.ApplyDamage(dealt);
        if (hitFeedback != null) hitFeedback.Notify(center);
    }

    private IEnumerator FlashLight(Light l, Vector3 at, float intensity, float time)
    {
        if (l == null) yield break;
        l.transform.position = at;
        float t = 0f;
        while (t < time && l != null)
        {
            l.intensity = intensity * (1f - t / time);
            t += Time.deltaTime;
            yield return null;
        }
        if (l != null) l.intensity = 0f;
    }

    // ---------- barrage (phase 2+) ----------

    private IEnumerator Barrage()
    {
        PlayAnimSafe("a4IdlePose");
        float turn = 0f;
        while (!dead && turn < 1f) { if (FacePlayer(1.5f)) break; turn += Time.deltaTime; yield return null; }
        if (dead) yield break;

        ShowBanner("INCOMING MISSILES");
        int count = barrageMissiles + (phase >= 3 ? 2 : 0);
        var targets = new List<Vector3>();
        Vector3 centre = player != null ? player.position : transform.position + transform.forward * 10f;
        for (int i = 0; i < count; i++)
        {
            Vector3 p = i == 0 ? centre : centre + Flat(Random.insideUnitSphere).normalized * Random.Range(1.5f, barrageSpread);
            targets.Add(GroundPoint(p));
        }

        var launchers = lrmMuzzles != null && lrmMuzzles.Length > 0 ? lrmMuzzles : srmMuzzles;
        for (int i = 0; i < targets.Count && !dead; i++)
        {
            var marker = MakeWarningRing(targets[i], barrageBlastRadius, barrageWarning);
            if (launchers != null && launchers.Length > 0 && lrmProjectilePrefab != null)
            {
                Transform m = launchers[i % launchers.Length];
                LaunchMissile(m.position, targets[i], lrmProjectilePrefab, lrmSpeed, barrageDamage, barrageBlastRadius, 1.1f, barrageWarning);
            }
            yield return new WaitForSeconds(0.12f);
        }
        nextBarrageTime = Time.time + barrageCooldown * (phase >= 3 ? 0.7f : 1f);
        yield return new WaitForSeconds(0.5f);
    }

    private Vector3 GroundPoint(Vector3 p)
    {
        foreach (var h in Physics.RaycastAll(p + Vector3.up * 3f, Vector3.down, 10f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (h.collider.transform.IsChildOf(transform)) continue;
            if (player != null && h.collider.transform.root == player.root) continue;
            return h.point;
        }
        return new Vector3(p.x, floorY, p.z);
    }

    /// <summary>A red ring on the floor with an inner ring closing in: the missile lands when they meet.</summary>
    private GameObject MakeWarningRing(Vector3 at, float radius, float time)
    {
        var go = new GameObject("StrikerBarrageWarning");
        go.transform.position = at + Vector3.up * 0.06f;
        var outer = RingLine(go.transform, radius, 0.14f);
        var inner = RingLine(go.transform, radius, 0.1f);
        liveMarkers.Add(go);
        StartCoroutine(AnimateRing(go, outer, inner, radius, time));
        return go;
    }

    private LineRenderer RingLine(Transform parent, float radius, float width)
    {
        var child = new GameObject("Ring");
        child.transform.SetParent(parent, false);
        var lr = child.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.positionCount = 40;
        lr.widthMultiplier = width;
        lr.sharedMaterial = fxMaterial;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        SetRing(lr, radius);
        return lr;
    }

    private static void SetRing(LineRenderer lr, float radius)
    {
        for (int i = 0; i < lr.positionCount; i++)
        {
            float a = i * Mathf.PI * 2f / lr.positionCount;
            lr.SetPosition(i, new Vector3(Mathf.Cos(a) * radius, 0f, Mathf.Sin(a) * radius));
        }
    }

    private IEnumerator AnimateRing(GameObject go, LineRenderer outer, LineRenderer inner, float radius, float time)
    {
        float t = 0f;
        while (t < time && go != null)
        {
            float k = t / time;
            float pulse = 0.55f + 0.45f * Mathf.Sin(Time.time * 18f);
            var c = new Color(1f, 0.12f, 0.05f, pulse);
            outer.startColor = outer.endColor = c;
            inner.startColor = inner.endColor = new Color(1f, 0.5f, 0.1f, 0.9f);
            SetRing(inner, Mathf.Max(0.05f, radius * (1f - k)));
            t += Time.deltaTime;
            yield return null;
        }
        if (go != null) { liveMarkers.Remove(go); Destroy(go); }
    }

    // ---------- stomp (phase 3 / point blank) ----------

    private IEnumerator Stomp()
    {
        PlayAnimSafe("a4IdlePose");
        ShowBanner("SHOCKWAVE");
        MakeWarningRing(GroundPoint(transform.position), stompRange, stompWindup);
        float t = 0f;
        while (t < stompWindup && !dead)
        {
            if (overdriveLight != null) overdriveLight.intensity = 6f * (t / stompWindup);
            t += Time.deltaTime;
            yield return null;
        }
        if (dead) yield break;
        StompBlast(true);
        nextStompTime = Time.time + stompCooldown;
        yield return new WaitForSeconds(0.6f);
    }

    private void StompBlast(bool hurts)
    {
        Vector3 at = GroundPoint(transform.position);
        TimedEffect.Spawn(shockwaveFx, at + Vector3.up * 0.1f, Quaternion.identity, 1.6f, 0.35f, 3f);
        TimedEffect.Spawn(explosionFx, at, Quaternion.identity, explosionFxScale * 0.8f, 0.3f, 3f);
        StartCoroutine(FlashLight(impactFlash, at + Vector3.up, 12f, 0.35f));
        if (explosionClip != null) AudioSource.PlayClipAtPoint(explosionClip, at, 1f);
        CameraShake.Shake(at, hurts ? 0.8f : 0.6f, 35f);
        if (!hurts || player == null || playerHealth == null || playerHealth.IsDead) return;
        float d = Vector3.Distance(Flat(player.position), Flat(transform.position));
        if (d > stompRange || Mathf.Abs(player.position.y - floorY) > 2f) return;
        int dealt = Mathf.Max(1, Mathf.RoundToInt(stompDamage * Mathf.Lerp(1f, 0.5f, d / stompRange)));
        playerHealth.ApplyDamage(dealt);
        if (hitFeedback != null) hitFeedback.Notify(transform.position);
    }

    // ---------- effects helpers ----------

    private void SparksAt(Vector3 at, float scale)
    {
        TimedEffect.Spawn(sparksFx, at, Quaternion.identity, scale, 0.08f, 1.5f);
    }

    private void BurstAt(Vector3 at, float scale)
    {
        TimedEffect.Spawn(explosionFx, at, Quaternion.identity, explosionFxScale * scale * 0.7f, 0.3f, 3f);
        SparksAt(at, scale);
        CameraShake.Shake(at, 0.5f, 30f);
        if (explosionClip != null) AudioSource.PlayClipAtPoint(explosionClip, at, 0.8f);
    }

    private void ShowBanner(string text)
    {
        bannerText = text;
        bannerStart = Time.time;
        bannerUntil = Time.time + 2.2f;
    }

    // ---------- health / death ----------

    /// <summary>Returns true if this hit destroyed the boss.</summary>
    public bool RegisterHit(Vector3 point)
    {
        if (dead) return false;

        // Flinch at most every so often - restarting the hit clip on every round of a full-auto
        // spray kept the mech stuck in its hit pose for the whole fight.
        if (animator != null && Time.time - LastHitTime > 1.2f && health > 1)
            animator.CrossFade(HitClips[Random.Range(0, HitClips.Length)], 0.05f);
        LastHitTime = Time.time;
        if (Time.time - lastSparkTime > 0.09f)
        {
            lastSparkTime = Time.time;
            SparksAt(point, 0.6f);
        }
        health--;
        if (!hasEngagedOnce) { hasEngagedOnce = true; combatActive = true; lastSeenPlayerTime = Time.time; }

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

        foreach (var p in liveProjectiles) if (p != null) Destroy(p);
        liveProjectiles.Clear();
        foreach (var m in liveMarkers) if (m != null) Destroy(m);
        liveMarkers.Clear();
        if (laserCore != null) laserCore.enabled = laserGlow.enabled = false;
        if (laserFlash != null) laserFlash.intensity = 0f;
        if (overdriveLight != null) overdriveLight.intensity = 0f;

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (animator != null) animator.CrossFade("b2DeathFallDown1", 0.1f);
        ShowBanner(bossName + " DESTROYED");

        if (MissionHUD.Instance != null && !string.IsNullOrEmpty(nextMissionObjective))
            MissionHUD.Instance.SetObjective(nextMissionObjective);
        if (doorGlow != null) doorGlow.SetGlowing(true);
        onDefeated?.Invoke();

        StartCoroutine(DeathSequence());
    }

    /// <summary>Chain of blasts across the body while it falls, then one big one, then it burns.</summary>
    private IEnumerator DeathSequence()
    {
        // The body's meshes only - the damage smoke's particle bounds reach far above the roof.
        Bounds b = new Bounds(transform.position + Vector3.up * 2f, Vector3.one * 2f);
        foreach (var r in GetComponentsInChildren<Renderer>())
            if (r is MeshRenderer || r is SkinnedMeshRenderer) b.Encapsulate(r.bounds);

        for (int i = 0; i < 4; i++)
        {
            Vector3 p = new Vector3(Random.Range(b.min.x, b.max.x), Random.Range(b.center.y, b.max.y), Random.Range(b.min.z, b.max.z));
            TimedEffect.Spawn(explosionFx, p, Quaternion.identity, explosionFxScale * 0.8f, 0.3f, 3f);
            SparksAt(p, 1f);
            if (explosionClip != null) AudioSource.PlayClipAtPoint(explosionClip, p, 0.6f);
            CameraShake.Shake(p, 0.35f, 30f);
            yield return new WaitForSeconds(0.35f);
        }

        if (overdriveSparks != null) Destroy(overdriveSparks);
        Vector3 core = b.center;
        TimedEffect.Spawn(bigExplosionFx != null ? bigExplosionFx : explosionFx, core, Quaternion.identity, explosionFxScale * 1.4f, 0.5f, 5f);
        TimedEffect.Spawn(explosionFx, core, Quaternion.identity, explosionFxScale * 1.8f, 0.4f, 5f);
        StartCoroutine(FlashLight(impactFlash, core, 25f, 0.6f));
        if (explosionClip != null)
        {
            audioSource.pitch = 0.8f;
            audioSource.PlayOneShot(explosionClip, 1f);
        }
        CameraShake.Shake(core, 0.9f, 45f);

        if (damageSmoke == null)
            damageSmoke = TimedEffect.Spawn(smokeFx, torso.position, Quaternion.identity, 1f, -1f, 0f, torso);

        yield return new WaitForSeconds(Mathf.Max(0.5f, removeDelayAfterDeath - 1.5f));
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        foreach (var p in liveProjectiles) if (p != null) Destroy(p);
        foreach (var m in liveMarkers) if (m != null) Destroy(m);
        if (fxMaterial != null) Destroy(fxMaterial);
    }

    // ---------- boss HUD ----------

    private void OnGUI()
    {
        if (Event.current.type != EventType.Repaint) return;
        var tex = Texture2D.whiteTexture;
        var prevColor = GUI.color;

        if (hasEngagedOnce && !dead)
        {
            const float barWidth = 460f;
            const float barHeight = 20f;
            float x = Screen.width * 0.5f - barWidth * 0.5f;
            float y = 26f;

            GUI.color = new Color(0f, 0f, 0f, 0.75f);
            GUI.DrawTexture(new Rect(x - 3f, y - 3f, barWidth + 6f, barHeight + 6f), tex);
            GUI.color = new Color(0.12f, 0.12f, 0.12f, 0.9f);
            GUI.DrawTexture(new Rect(x, y, barWidth, barHeight), tex);

            float pct = Mathf.Clamp01((float)health / Mathf.Max(1, maxHealth));
            bool flash = Time.time - LastHitTime < 0.08f;
            GUI.color = flash ? Color.white : Color.Lerp(new Color(0.85f, 0.12f, 0.08f), new Color(1f, 0.65f, 0.1f), pct);
            GUI.DrawTexture(new Rect(x, y, barWidth * pct, barHeight), tex);

            // Phase marks.
            GUI.color = new Color(1f, 1f, 1f, 0.8f);
            GUI.DrawTexture(new Rect(x + barWidth * phaseTwoBelow - 1f, y, 2f, barHeight), tex);
            GUI.DrawTexture(new Rect(x + barWidth * enrageBelow - 1f, y, 2f, barHeight), tex);

            GUI.color = prevColor;
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 15 };
            style.normal.textColor = Color.white;
            GUI.Label(new Rect(x, y - 22f, barWidth, 20f), bossName, style);
            var small = new GUIStyle(style) { fontSize = 11, fontStyle = FontStyle.Normal };
            small.normal.textColor = new Color(1f, 0.7f, 0.3f);
            GUI.Label(new Rect(x, y + barHeight + 2f, barWidth, 16f), bossTitle + (phase >= 3 ? "  ·  OVERDRIVE" : phase == 2 ? "  ·  ARMOR BREACHED" : ""), small);
        }

        if (!string.IsNullOrEmpty(bannerText) && Time.time < bannerUntil)
        {
            float age = Time.time - bannerStart;
            float a = Mathf.Clamp01(age / 0.15f) * Mathf.Clamp01((bannerUntil - Time.time) / 0.5f);
            var big = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 34 };
            Rect r = new Rect(0f, Screen.height * 0.24f, Screen.width, 50f);
            big.normal.textColor = new Color(0f, 0f, 0f, 0.7f * a);
            GUI.Label(new Rect(r.x + 2f, r.y + 2f, r.width, r.height), bannerText, big);
            big.normal.textColor = new Color(1f, 0.25f, 0.15f, a);
            GUI.Label(r, bannerText, big);
        }
        GUI.color = prevColor;
    }
}
