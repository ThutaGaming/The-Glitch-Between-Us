using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using InfimaGames.LowPolyShooterPack;
using Random = UnityEngine.Random;

[Serializable]
public class EnemyRoute
{
    public string label;
    [Tooltip("Patrols roamPoints and trades shots from each one instead of running the cover " +
             "routine - for open ground with no barrier worth hiding behind. " +
             "firstCover/pushCover are unused.")]
    [FormerlySerializedAs("standAndShoot")]
    public bool roam;
    [Tooltip("World positions a roaming enemy walks between, in order, looping. Each must be on " +
             "walkable floor inside the arena.")]
    public Vector3[] roamPoints;
    [Tooltip("Index into covers[] the enemy runs to first (the far/holding cover).")]
    public int firstCover;
    [Tooltip("Index into covers[] the enemy pushes up to, then falls back from.")]
    public int pushCover;
}

/// <summary>
/// Per-encounter difficulty, copied onto every EnemyAI2 this manager sets up. The defaults are
/// EnemyAI2's own, so an encounter that never touches this plays exactly as it did before.
/// </summary>
[Serializable]
public class EnemyTuning
{
    [Tooltip("Body shots to kill. A headshot always kills regardless.")]
    public int maxHealth = 2;
    public int damagePerHit = 8;
    public float shotInterval = 0.42f;
    public int minBurst = 2;
    public int maxBurst = 4;
    [Tooltip("Chance a shot is aimed true at nearRange; it falls off towards farHitChance.")]
    public float nearHitChance = 0.55f;
    public float farHitChance = 0.15f;
    public float runSpeed = 4.2f;
}

public class CoverPoint
{
    public Transform Barrier;
    public Vector3 Center;
    public Vector3 AxisLong;
    public Vector3 AxisThin;
    public float HalfLong;
    public float HalfThin;
    public EnemyAI2 ClaimedBy;
}

/// <summary>
/// Room 1 firefight: the Room2 door opens, a squad files out and fights from the room's cover
/// barriers along designed routes (far cover -> push cover -> fall back), sharing a cover
/// registry, attack tokens (max shooters at once) and a move token (one planned relocation at a
/// time). Also owns player-hit detection, player damage feedback and enemy health bars.
/// </summary>
public class CombatEncounterManager2 : MonoBehaviour
{
    [Header("Assets")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject gunPrefab;
    [SerializeField] private RuntimeAnimatorController enemyController;
    [SerializeField] private AudioClip[] shotClips;
    [SerializeField] private Vector3 gunLocalPosition = new Vector3(0.0968f, -0.0367f, 0.033f);
    [SerializeField] private Vector3 gunLocalEuler = new Vector3(4.02f, 74.71f, 82.85f);
    [SerializeField] private EnemyTuning tuning = new EnemyTuning();

    [Header("Arena")]
    [SerializeField] private DoubleSlidingDoor door;
    [Tooltip("Same blue X-ray outline every other mission marker in this project uses.")]
    [SerializeField] private ObjectiveGlow doorGlow;
    [SerializeField] private Transform[] covers;
    [SerializeField] private EnemyRoute[] routes;
    [Tooltip("Ambush mode: enemies already standing in the room, one per route, instead of a squad " +
             "spawned behind the door. They arm themselves and idle on scene load, and only start " +
             "fighting on Begin(). Leave empty for the door-spawn flow.")]
    [SerializeField] private Transform[] preplacedEnemies;
    [Tooltip("Optional auto-turrets in the room. They wake with the squad and each counts as one " +
             "more target the player has to destroy before the area is clear.")]
    [SerializeField] private TurretEnemy[] turrets;
    [SerializeField] private Vector3 arenaMin = new Vector3(-25f, 0f, 0f);
    [SerializeField] private Vector3 arenaMax = new Vector3(25f, 0f, 47.5f);
    [SerializeField] private float floorY = 0.1f;

    [Header("Flow")]
    [Tooltip("Start as soon as the scene plays. Off = wait for Begin() (e.g. a trigger when the player enters the room).")]
    [SerializeField] private bool startOnPlay = true;
    [SerializeField] private string eliminateObjective = "Eliminate the hostiles";
    [SerializeField] private string clearedObjective = "Head to the Room2 entrance door";
    [Tooltip("Off for a secondary encounter fought alongside another one (e.g. the other floor of " +
             "the same room), so the two don't overwrite each other's objective panel. Let an " +
             "EncounterGroup show the shared objective instead.")]
    [SerializeField] private bool drivesMissionHud = true;
    [Tooltip("Fires once, the moment every hostile (including the turret) is down.")]
    public UnityEvent onCleared;

    [Header("Pacing")]
    [SerializeField] private float startDelay = 2.5f;
    [SerializeField] private float exitStagger = 0.9f;
    [SerializeField] private int maxAttackers = 2;
    [SerializeField] private float idealRange = 14f;

    [Header("Player shots")]
    [SerializeField] private float raycastDistance = 500f;
    private static int projectileLayer => LayerMask.NameToLayer("Projectile");
    [SerializeField] private bool debugLogs;

    private readonly List<CoverPoint> coverPoints = new List<CoverPoint>();
    private readonly List<EnemyAI2> enemies = new List<EnemyAI2>();
    private readonly List<EnemyAI2> attackers = new List<EnemyAI2>();
    private readonly Dictionary<EnemyAI2, bool> visible = new Dictionary<EnemyAI2, bool>();
    private readonly RaycastHit[] rayBuffer = new RaycastHit[16];

    private CharacterBehaviour playerCharacter;
    private InventoryBehaviour playerInventory;
    private Transform cameraTransform;
    private Transform playerTransform;
    private Rigidbody playerBody;
    private PlayerHealth playerHealth;
    private Camera mainCamera;
    private int lastAmmo = -1;

    private EnemyAI2 mover;
    private float moverSince;
    private float nextAttackTime;
    private int kills;
    private bool begun;
    private Vector3 arenaCenter;
    private Vector3 doorInward;

    private ParticleSystem bloodFx, sparkFx;
    private Material fxMaterial;

    private float damageFlash;
    private Vector3 damageFrom;
    private float damageDirTime = -99f;
    private float hitMarkerTime = -99f;
    private bool hitMarkerKill;

    public CoverNavGrid Grid { get; private set; }
    public bool DoorOpen { get; private set; }
    public float FloorY => floorY;
    public Transform PlayerTransform => playerTransform;
    public Vector3 PlayerEye => playerTransform.position + Vector3.up * 1.6f;
    public Vector3 PlayerChest => playerTransform.position + Vector3.up * 1.3f;
    public float PlayerSpeed => playerBody != null ? new Vector3(playerBody.linearVelocity.x, 0f, playerBody.linearVelocity.z).magnitude : 0f;
    public bool PlayerDead => playerHealth != null && playerHealth.IsDead;
    /// <summary>Begun and not yet cleared - only an active encounter reads the player's shots.</summary>
    public bool IsActive => begun && (TotalTargets == 0 || Eliminated < TotalTargets);

    private int TurretCount
    {
        get
        {
            int n = 0;
            if (turrets != null)
                foreach (var t in turrets) if (t != null) n++;
            return n;
        }
    }

    private int TotalTargets => enemies.Count + TurretCount;

    private int Eliminated
    {
        get
        {
            int n = kills;
            if (turrets != null)
                foreach (var t in turrets) if (t != null && t.IsDead) n++;
            return n;
        }
    }

    private bool OwnsTurret(TurretEnemy t)
    {
        if (turrets == null || t == null) return false;
        foreach (var candidate in turrets) if (candidate == t) return true;
        return false;
    }

    private void Awake()
    {
        var gameModeService = ServiceLocator.Current.Get<IGameModeService>();
        playerCharacter = gameModeService != null ? gameModeService.GetPlayerCharacter() : null;
        playerInventory = playerCharacter != null ? playerCharacter.GetInventory() : null;
        cameraTransform = playerCharacter != null ? playerCharacter.GetCameraWorld().transform : null;
        playerTransform = playerCharacter != null ? playerCharacter.transform : null;
        mainCamera = cameraTransform != null ? cameraTransform.GetComponent<Camera>() : Camera.main;
        if (playerTransform != null)
        {
            playerBody = playerTransform.GetComponent<Rigidbody>();
            playerHealth = playerTransform.GetComponent<PlayerHealth>();
        }
    }

    private void Start()
    {
        bool ambush = preplacedEnemies != null && preplacedEnemies.Length > 0;
        if (playerTransform == null || door == null || (enemyPrefab == null && !ambush))
        {
            Debug.LogError("[Encounter] Missing player, door or enemy prefab - encounter disabled.");
            enabled = false;
            return;
        }

        BuildCovers();
        BuildFx();
        if (turrets != null)
            foreach (var t in turrets)
                if (t != null) t.Initialize(this, playerTransform, shotClips, fxMaterial);

        doorInward = door.transform.position - arenaCenter;
        doorInward.y = 0f;
        doorInward = -doorInward.normalized;

        door.Locked = true;
        // Armed here rather than in RunEncounter so the player walking in finds them already
        // standing around with a weapon in hand, not popping into existence mid-room.
        if (ambush) SetUpPreplaced();
        if (startOnPlay) Begin();
    }

    /// <summary>Which side of the arena an enemy favours when picking a fallback cover - from its
    /// route's cover for the normal flow, or its own spawn side for a roaming route that
    /// has no cover index to read.</summary>
    private float LaneSignFor(EnemyRoute route, Transform spawnPoint)
    {
        if (!route.roam && route.firstCover >= 0 && route.firstCover < coverPoints.Count)
            return Mathf.Sign(coverPoints[route.firstCover].Center.x - arenaCenter.x);
        return Mathf.Sign(spawnPoint.position.x - arenaCenter.x);
    }

    private void SetUpPreplaced()
    {
        for (int i = 0; i < preplacedEnemies.Length && i < routes.Length; i++)
        {
            Transform t = preplacedEnemies[i];
            if (t == null) continue;

            PrepareRoamRoute(routes[i], t.position, i);
            var ai = t.gameObject.AddComponent<EnemyAI2>();
            float lane = LaneSignFor(routes[i], t);
            ai.Initialize(this, playerTransform, routes[i], lane, gunPrefab, gunLocalPosition, gunLocalEuler, enemyController, shotClips, fxMaterial, tuning);
            enemies.Add(ai);
        }
    }

    /// <summary>Starts the fight; safe to call repeatedly (only the first call does anything).</summary>
    public void Begin()
    {
        bool hasRoamingEnemies = routes != null && Array.Exists(routes, r => r != null && r.roam);
        if (begun || !enabled || (coverPoints.Count == 0 && !hasRoamingEnemies)) return;
        begun = true;
        // Built now rather than in Start so it reflects the room as the player finds it.
        Grid = new CoverNavGrid(arenaMin, arenaMax, floorY, 0.5f, 0.36f, 1.8f, ~0, IgnoreForGrid);
        StartCoroutine(RunEncounter());
    }

    /// <summary>
    /// Existing scenes used standAndShoot without patrol points. Give each migrated route a
    /// compact, different loop around its spawn. Authored routes are always left untouched.
    /// </summary>
    private void PrepareRoamRoute(EnemyRoute route, Vector3 spawn, int routeIndex)
    {
        if (route == null || !route.roam || (route.roamPoints != null && route.roamPoints.Length >= 2))
            return;

        float sideSign = routeIndex % 2 == 0 ? 1f : -1f;
        float depthSign = (routeIndex / 2) % 2 == 0 ? 1f : -1f;
        float side = 4.5f + (routeIndex % 3) * 1.25f;
        float depth = 3.5f + ((routeIndex + 1) % 3) * 1.1f;

        route.roamPoints = new[]
        {
            ClampRoamPoint(spawn),
            ClampRoamPoint(spawn + new Vector3(sideSign * side, 0f, depthSign * depth * 0.45f)),
            ClampRoamPoint(spawn + new Vector3(sideSign * side * 0.65f, 0f, -depthSign * depth)),
            ClampRoamPoint(spawn + new Vector3(-sideSign * side * 0.55f, 0f, -depthSign * depth * 0.55f))
        };
    }

    private Vector3 ClampRoamPoint(Vector3 point)
    {
        const float margin = 1.25f;
        float minX = Mathf.Min(arenaMin.x, arenaMax.x) + margin;
        float maxX = Mathf.Max(arenaMin.x, arenaMax.x) - margin;
        float minZ = Mathf.Min(arenaMin.z, arenaMax.z) + margin;
        float maxZ = Mathf.Max(arenaMin.z, arenaMax.z) - margin;
        return new Vector3(Mathf.Clamp(point.x, minX, maxX), floorY, Mathf.Clamp(point.z, minZ, maxZ));
    }

    public bool TryResolveWalkablePoint(Vector3 desired, out Vector3 resolved)
    {
        desired.y = floorY;
        if (Grid != null && Grid.TryGetNearestWalkable(desired, 16, out resolved))
        {
            resolved.y = floorY;
            return true;
        }

        resolved = desired;
        return false;
    }

    private bool IgnoreForGrid(Collider c)
    {
        return c.transform.IsChildOf(door.transform) || c.transform.root == playerTransform.root || c.GetComponentInParent<EnemyAI2>() != null;
    }

    private void BuildCovers()
    {
        Vector3 sum = Vector3.zero;
        foreach (var barrier in covers)
        {
            var body = barrier.Find("Body");
            var box = body != null ? body.GetComponent<BoxCollider>() : null;
            var cp = new CoverPoint { Barrier = barrier };
            if (box != null)
            {
                Vector3 half = Vector3.Scale(box.size, body.lossyScale) * 0.5f;
                cp.Center = body.TransformPoint(box.center);
                cp.AxisLong = Flat(body.right);
                cp.AxisThin = Flat(body.forward);
                cp.HalfLong = Mathf.Abs(half.x);
                cp.HalfThin = Mathf.Abs(half.z);
            }
            else
            {
                cp.Center = barrier.position;
                cp.AxisLong = Flat(barrier.right);
                cp.AxisThin = Flat(barrier.forward);
                cp.HalfLong = 1.5f;
                cp.HalfThin = 0.35f;
            }
            cp.Center.y = floorY;
            coverPoints.Add(cp);
            sum += cp.Center;
        }
        arenaCenter = coverPoints.Count > 0
            ? sum / coverPoints.Count
            : new Vector3((arenaMin.x + arenaMax.x) * 0.5f, floorY, (arenaMin.z + arenaMax.z) * 0.5f);
    }

    private IEnumerator RunEncounter()
    {
        // Wait first so a just-completed objective (e.g. "Head to the door") gets to show its tick.
        yield return new WaitForSeconds(startDelay);
        // routes.Length rather than enemies.Count: in the door-spawn flow the squad doesn't exist yet.
        if (drivesMissionHud && MissionHUD.Instance != null)
            MissionHUD.Instance.SetObjective(eliminateObjective, routes.Length + TurretCount);

        // Ambush enemies were already built in Start and are standing in the room; only the
        // door-spawn flow has a squad to file out first.
        bool spawnedAtDoor = enemies.Count == 0;
        if (spawnedAtDoor)
        {
            Vector3 doorRight = Vector3.Cross(Vector3.up, -doorInward);
            for (int i = 0; i < routes.Length; i++)
            {
                int row = i / 2;
                float col = (i % 2 == 0) ? -0.55f : 0.55f;
                Vector3 spawn = door.transform.position - doorInward * (1.3f + row * 1.0f) + doorRight * col;
                spawn.y = floorY;
                var go = Instantiate(enemyPrefab, spawn, Quaternion.LookRotation(doorInward));
                PrepareRoamRoute(routes[i], spawn, i);
                go.name = "Enemy_" + routes[i].label;
                var ai = go.AddComponent<EnemyAI2>();
                float lane = LaneSignFor(routes[i], go.transform);
                ai.Initialize(this, playerTransform, routes[i], lane, gunPrefab, gunLocalPosition, gunLocalEuler, enemyController, shotClips, fxMaterial, tuning);
                enemies.Add(ai);
            }

            door.ScriptedOpen();
            yield return new WaitForSeconds(0.65f);
            DoorOpen = true;
        }

        if (turrets != null)
            foreach (var t in turrets)
                if (t != null) t.Activate();

        foreach (var e in enemies)
        {
            e.Release();
            yield return new WaitForSeconds(exitStagger);
        }

        if (spawnedAtDoor)
        {
            float waitStart = Time.time;
            while (Time.time - waitStart < 8f && !AllEnemiesInside()) yield return null;
            DoorOpen = false;
            door.ScriptedClose();
        }

        while (Eliminated < TotalTargets) yield return null;

        door.Locked = false;
        if (doorGlow != null) doorGlow.SetGlowing(true);
        if (drivesMissionHud && MissionHUD.Instance != null)
            MissionHUD.Instance.SetObjective(clearedObjective);
        onCleared?.Invoke();
    }

    /// <summary>
    /// Test-only shortcut for the F8 debug skip: resolves this encounter as if the player had
    /// fought and cleared it normally - instantly kills any enemies/turrets already spawned
    /// (through their real RegisterHit path, so kills/HUD progress/death FX all fire normally),
    /// unlocks the door and fires onCleared, without waiting for RunEncounter's multi-second
    /// timeline or spawning a squad that never got to file out.
    /// </summary>
    public void DebugForceClear()
    {
        StopAllCoroutines();
        begun = true;

        foreach (var e in enemies) if (e != null && !e.IsDead) e.RegisterHit(true);
        if (turrets != null)
            foreach (var t in turrets)
                if (t != null)
                    for (int i = 0; i < t.MaxHealth && !t.IsDead; i++) t.RegisterHit(t.BarAnchor);

        DoorOpen = false;
        if (door != null) door.Locked = false;
        if (doorGlow != null) doorGlow.SetGlowing(true);
        if (drivesMissionHud && MissionHUD.Instance != null) MissionHUD.Instance.SetObjective(clearedObjective);
        onCleared?.Invoke();
    }

    /// <summary>Called by DoorAutoCloseZone once the player has walked through the door.</summary>
    public void NotifyPlayerThroughDoor()
    {
        if (MissionHUD.Instance != null && MissionHUD.Instance.HasActiveObjective)
            MissionHUD.Instance.CompleteObjective();
        if (doorGlow != null) doorGlow.SetGlowing(false);
    }

    private bool AllEnemiesInside()
    {
        foreach (var e in enemies)
        {
            if (e == null || e.IsDead) continue;
            if (Vector3.Dot(e.transform.position - door.transform.position, doorInward) < 3f) return false;
        }
        return true;
    }

    // ---------- cover registry ----------

    public int CoverCount => coverPoints.Count;

    public bool IsClaimedByOther(int index, EnemyAI2 e)
    {
        var c = coverPoints[index].ClaimedBy;
        return c != null && c != e && !c.IsDead;
    }

    public void Claim(int index, EnemyAI2 e)
    {
        if (index >= 0 && index < coverPoints.Count) coverPoints[index].ClaimedBy = e;
    }

    public void ReleaseCover(int index, EnemyAI2 e)
    {
        if (index >= 0 && index < coverPoints.Count && coverPoints[index].ClaimedBy == e) coverPoints[index].ClaimedBy = null;
    }

    public Vector3 HideSpot(int index, Vector3 threat, out Vector3 facing, out int rightEnd)
    {
        var c = coverPoints[index];
        float s = Vector3.Dot(threat - c.Center, c.AxisThin) >= 0f ? 1f : -1f;
        facing = c.AxisThin * s;
        rightEnd = Vector3.Dot(c.AxisLong, Vector3.Cross(Vector3.up, facing)) >= 0f ? 1 : -1;
        // With the threat well off to one side, tuck in at the far end so this barrier (not some
        // other prop that happens to be in the way) is what covers the angle.
        float lateral = Vector3.Dot(threat - c.Center, c.AxisLong);
        float shift = Mathf.Abs(lateral) > c.HalfLong ? -Mathf.Sign(lateral) : rightEnd;
        Vector3 p = c.Center - c.AxisThin * s * (c.HalfThin + 0.62f) + c.AxisLong * shift * Mathf.Max(0f, c.HalfLong - 0.8f);
        if (Grid != null && Grid.TryGetNearestWalkable(p, 3, out Vector3 snapped)) p = snapped;
        p.y = floorY;
        return p;
    }

    public Vector3 PeekSpot(int index, Vector3 threat, int end, bool isRightEnd)
    {
        var c = coverPoints[index];
        float s = Vector3.Dot(threat - c.Center, c.AxisThin) >= 0f ? 1f : -1f;
        float along = c.HalfLong + 0.72f + (isRightEnd ? 0f : 0.3f);
        Vector3 p = c.Center - c.AxisThin * s * (c.HalfThin + 0.45f) + c.AxisLong * end * along;
        p.y = floorY;
        return p;
    }

    [SerializeField] private float minCoverDistance = 9f;
    private static readonly float[] ProtectHeights = { 1.2f, 1.65f };

    /// <summary>Hidden from the player's eye at chest and head height, with a little sideways
    /// margin so a half-step by the player doesn't flip the answer.</summary>
    public bool IsSpotProtected(Vector3 spot)
    {
        Vector3 eye = PlayerEye;
        Vector3 side = Vector3.Cross(Vector3.up, Flat(spot - eye)) * 0.3f;
        foreach (float h in ProtectHeights)
            for (int k = -1; k <= 1; k++)
                if (HasLineOfSight(eye, spot + Vector3.up * h + side * k)) return false;
        return true;
    }

    public bool IsCoverUsable(int index, EnemyAI2 e)
    {
        if (index < 0 || index >= coverPoints.Count || IsClaimedByOther(index, e)) return false;
        Vector3 hide = HideSpot(index, playerTransform.position, out _, out _);
        return FlatDistance(hide, playerTransform.position) >= minCoverDistance && IsSpotProtected(hide);
    }

    private EnemyAI2 RouteOwnerOf(int coverIndex)
    {
        for (int i = 0; i < enemies.Count && i < routes.Length; i++)
            if (routes[i].firstCover == coverIndex || routes[i].pushCover == coverIndex) return enemies[i];
        return null;
    }

    /// <summary>True if at least one end of this barrier gives a clear shot at the player.</summary>
    public bool HasPeekLine(int index)
    {
        Vector3 threat = playerTransform.position;
        HideSpot(index, threat, out _, out int rightEnd);
        foreach (int end in new[] { rightEnd, -rightEnd })
        {
            Vector3 p = PeekSpot(index, threat, end, end == rightEnd);
            if (Grid.IsWalkable(p) && HasLineOfSight(p + Vector3.up * 1.5f, PlayerChest)) return true;
        }
        return false;
    }

    public int FindBestCover(EnemyAI2 e, Vector3 from, int preferA, int preferB, int exclude, float laneSign, bool requirePeekLine = false)
    {
        int best = -1;
        float bestScore = float.MaxValue;
        Vector3 player = playerTransform.position;
        for (int i = 0; i < coverPoints.Count; i++)
        {
            if (i == exclude || IsClaimedByOther(i, e)) continue;
            Vector3 hide = HideSpot(i, player, out _, out _);
            float toPlayer = FlatDistance(hide, player);
            if (toPlayer < minCoverDistance || !IsSpotProtected(hide)) continue;
            if (requirePeekLine && !HasPeekLine(i)) continue;

            float score = FlatDistance(from, hide) + Mathf.Abs(toPlayer - idealRange) * 0.6f;
            var owner = RouteOwnerOf(i);
            if (i == preferA || i == preferB) score -= 10f;
            else if (owner != null && owner != e && !owner.IsDead) score += 8f;
            if (Mathf.Sign(hide.x - arenaCenter.x) != laneSign) score += 6f;
            if (score < bestScore)
            {
                bestScore = score;
                best = i;
            }
        }
        return best;
    }

    // ---------- tokens ----------

    public bool TryAcquireAttackToken(EnemyAI2 e)
    {
        if (attackers.Contains(e)) return true;
        if (attackers.Count >= maxAttackers || Time.time < nextAttackTime || PlayerDead) return false;
        attackers.Add(e);
        nextAttackTime = Time.time + 0.35f;
        return true;
    }

    public void ReleaseAttackToken(EnemyAI2 e) => attackers.Remove(e);

    public bool TryAcquireMoveToken(EnemyAI2 e)
    {
        if (mover == e) return true;
        if (mover != null && !mover.IsDead && Time.time - moverSince < 8f) return false;
        mover = e;
        moverSince = Time.time;
        return true;
    }

    public void ReleaseMoveToken(EnemyAI2 e)
    {
        if (mover == e) mover = null;
    }

    // ---------- queries ----------

    public bool HasLineOfSight(Vector3 from, Vector3 to)
    {
        Vector3 d = to - from;
        float len = d.magnitude;
        if (len < 0.01f) return true;
        int n = Physics.RaycastNonAlloc(from, d / len, rayBuffer, len, ~0, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < n; i++)
        {
            var col = rayBuffer[i].collider;
            if (col.transform.root == playerTransform.root || col.GetComponentInParent<EnemyAI2>() != null) continue;
            return false;
        }
        return true;
    }

    public bool IsPlayerCollider(Collider c) => c != null && c.transform.root == playerTransform.root;

    public static float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    private static Vector3 Flat(Vector3 v)
    {
        v.y = 0f;
        return v.sqrMagnitude > 0.0001f ? v.normalized : Vector3.forward;
    }

    public void Log(string msg)
    {
        if (debugLogs) Debug.Log("[Encounter] " + msg);
    }

    // ---------- damage ----------

    public void DamagePlayer(int amount, Vector3 fromPosition)
    {
        if (PlayerDead) return;
        if (playerHealth != null) playerHealth.ApplyDamage(amount);
        damageFlash = 1f;
        damageFrom = fromPosition;
        damageDirTime = Time.time;
    }

    public void NotifyDeath(EnemyAI2 e)
    {
        kills++;
        attackers.Remove(e);
        ReleaseMoveToken(e);
        foreach (var c in coverPoints)
            if (c.ClaimedBy == e) c.ClaimedBy = null;
        if (drivesMissionHud && MissionHUD.Instance != null) MissionHUD.Instance.SetProgress(Eliminated);
        Log(e.name + " died (" + Eliminated + "/" + TotalTargets + ")");
    }

    public void NotifyTurretDestroyed()
    {
        if (drivesMissionHud && MissionHUD.Instance != null) MissionHUD.Instance.SetProgress(Eliminated);
        Log("turret destroyed (" + Eliminated + "/" + TotalTargets + ")");
    }

    private void Update()
    {
        if (damageFlash > 0f) damageFlash -= Time.deltaTime * 1.4f;

        foreach (var e in enemies)
        {
            if (e == null || e.IsDead || mainCamera == null) continue;
            bool v = Vector3.Distance(cameraTransform.position, e.HeadPosition) < 45f && HasLineOfSight(cameraTransform.position, e.HeadPosition - Vector3.up * 0.3f);
            visible[e] = v;
        }

        if (!IsActive) { lastAmmo = -1; return; }
        if (playerInventory == null || PlayerDead) return;
        WeaponBehaviour equipped = playerInventory.GetEquipped();
        if (equipped == null) { lastAmmo = -1; return; }

        int current = equipped.GetAmmunitionCurrent();
        if (lastAmmo >= 0 && current < lastAmmo) TryRegisterPlayerShot();
        lastAmmo = current;
    }

    [Tooltip("Extra hit radius (m) added to enemy body/head for every shot, plus a per-metre growth so distant enemies stay hittable.")]
    [SerializeField] private float aimForgivenessBase = 0.06f;
    [SerializeField] private float aimForgivenessPerMeter = 0.007f;
    private readonly RaycastHit[] shotBuffer = new RaycastHit[64];

    /// <summary>
    /// Tests the crosshair ray against each enemy's body axis and head analytically instead of a
    /// thin physics cast: immune to the player's own projectiles/colliders, forgiving at range,
    /// and still blocked by any wall or barrier in front of the enemy.
    /// </summary>
    private void TryRegisterPlayerShot()
    {
        if (cameraTransform == null) return;
        Vector3 origin = cameraTransform.position;
        Vector3 dir = cameraTransform.forward;

        float wallDist = raycastDistance;
        Collider wallCollider = null;
        int n = Physics.RaycastNonAlloc(origin, dir, shotBuffer, raycastDistance, ~0, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < n; i++)
        {
            var col = shotBuffer[i].collider;
            if (IsPlayerCollider(col) || col.GetComponentInParent<EnemyAI2>() != null || col.gameObject.layer == projectileLayer) continue;
            if (shotBuffer[i].distance < wallDist)
            {
                wallDist = shotBuffer[i].distance;
                wallCollider = col;
            }
        }

        EnemyAI2 best = null;
        float bestAlong = float.MaxValue;
        bool bestHead = false;
        Vector3 bestPoint = Vector3.zero;
        foreach (var e in enemies)
        {
            if (e == null || e.IsDead) continue;
            Vector3 head = e.HeadCenter;
            Vector3 a = e.transform.position + Vector3.up * 0.3f;
            Vector3 b = head - Vector3.up * 0.18f;
            ClosestRaySegment(origin, dir, a, b, out float along, out Vector3 onSegment);

            float headAlong = Vector3.Dot(head - origin, dir);
            float headPerp = Vector3.Distance(origin + dir * headAlong, head);
            float forgiveness = aimForgivenessBase + aimForgivenessPerMeter * Mathf.Max(along, headAlong);

            bool headHit = headAlong > 0f && headAlong < wallDist && headPerp < 0.15f + forgiveness * 0.6f;
            bool bodyHit = along > 0f && along < wallDist && Vector3.Distance(origin + dir * along, onSegment) < 0.3f + forgiveness;
            if (!headHit && !bodyHit) continue;

            float hitAlong = headHit ? headAlong : along;
            if (hitAlong < bestAlong)
            {
                bestAlong = hitAlong;
                best = e;
                bestHead = headHit;
                bestPoint = headHit ? head : onSegment;
            }
        }

        if (best == null)
        {
            if (wallDist >= raycastDistance) return;

            // The turret is solid geometry rather than an EnemyAI2, so it shows up as the nearest
            // "wall" - that's the hit.
            Vector3 point = origin + dir * wallDist;
            // OwnsTurret so two managers running side by side (one per floor) can't both charge
            // the same bullet to the same turret.
            var hitTurret = wallCollider != null ? wallCollider.GetComponentInParent<TurretEnemy>() : null;
            if (hitTurret != null && !hitTurret.IsDead && OwnsTurret(hitTurret))
            {
                hitMarkerKill = hitTurret.RegisterHit(point);
                hitMarkerTime = Time.time;
                return;
            }

            Emit(sparkFx, point, 6);
            return;
        }

        bool killed = best.RegisterHit(bestHead);
        Emit(bloodFx, bestPoint, bestHead ? 26 : 16);
        hitMarkerTime = Time.time;
        hitMarkerKill = killed;
    }

    private static void ClosestRaySegment(Vector3 origin, Vector3 dir, Vector3 a, Vector3 b, out float along, out Vector3 onSegment)
    {
        Vector3 v = b - a;
        float vv = Mathf.Max(0.0001f, Vector3.Dot(v, v));
        float dv = Vector3.Dot(dir, v);
        Vector3 w = origin - a;
        float denom = vv - dv * dv;
        float s = denom > 0.0001f ? (Vector3.Dot(v, w) - dv * Vector3.Dot(dir, w)) / denom : 0f;
        s = Mathf.Clamp01(s);
        along = Vector3.Dot(a + v * s - origin, dir);
        s = Mathf.Clamp01(Vector3.Dot(origin + dir * along - a, v) / vv);
        onSegment = a + v * s;
        along = Vector3.Dot(onSegment - origin, dir);
    }

    // ---------- fx ----------

    private void BuildFx()
    {
        var shader = Shader.Find("Sprites/Default");
        fxMaterial = new Material(shader);
        bloodFx = MakeBurst("BloodFx", new Color(0.55f, 0.02f, 0.02f), 0.07f, 3f, 0.45f, 1.6f);
        sparkFx = MakeBurst("SparkFx", new Color(1f, 0.78f, 0.35f), 0.035f, 5f, 0.18f, 0.4f);
    }

    private ParticleSystem MakeBurst(string name, Color color, float size, float speed, float life, float gravity)
    {
        var go = new GameObject(name);
        go.SetActive(false);
        go.transform.SetParent(transform, false);
        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = true;
        main.playOnAwake = true;
        main.startLifetime = life;
        main.startSpeed = speed;
        main.startSize = size;
        main.startColor = color;
        main.gravityModifier = gravity;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 400;
        var emission = ps.emission;
        emission.rateOverTime = 0f;
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.04f;
        go.GetComponent<ParticleSystemRenderer>().sharedMaterial = fxMaterial;
        go.SetActive(true);
        return ps;
    }

    private static void Emit(ParticleSystem ps, Vector3 point, int count)
    {
        if (ps == null) return;
        var p = new ParticleSystem.EmitParams { position = point, applyShapeToPosition = true };
        ps.Emit(p, count);
    }

    public void SpawnImpact(Vector3 point) => Emit(sparkFx, point, 8);

    // ---------- HUD ----------

    private void OnGUI()
    {
        var prev = GUI.color;
        var tex = Texture2D.whiteTexture;

        if (damageFlash > 0f)
        {
            GUI.color = new Color(0.55f, 0f, 0f, damageFlash * 0.28f);
            float edge = Screen.height * 0.12f;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, edge), tex);
            GUI.DrawTexture(new Rect(0, Screen.height - edge, Screen.width, edge), tex);
            GUI.DrawTexture(new Rect(0, 0, edge, Screen.height), tex);
            GUI.DrawTexture(new Rect(Screen.width - edge, 0, edge, Screen.height), tex);
        }

        if (IsActive && playerHealth != null && !playerHealth.IsDead && playerHealth.Normalized < 0.35f)
        {
            float pulse = 0.18f + 0.1f * Mathf.Sin(Time.time * 5f);
            GUI.color = new Color(0.5f, 0f, 0f, pulse * (1f - playerHealth.Normalized / 0.35f) + 0.08f);
            float edge = Screen.height * 0.08f;
            GUI.DrawTexture(new Rect(0, 0, edge, Screen.height), tex);
            GUI.DrawTexture(new Rect(Screen.width - edge, 0, edge, Screen.height), tex);
        }

        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        float dirAge = Time.time - damageDirTime;
        if (dirAge < 1.2f && cameraTransform != null)
        {
            Vector3 camFwd = Flat(cameraTransform.forward);
            Vector3 toAttacker = Flat(damageFrom - cameraTransform.position);
            float angle = Vector3.SignedAngle(camFwd, toAttacker, Vector3.up);
            var matrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(angle, center);
            GUI.color = new Color(1f, 0.15f, 0.1f, 0.85f * (1f - dirAge / 1.2f));
            GUI.DrawTexture(new Rect(center.x - 45f, center.y - 170f, 90f, 12f), tex);
            GUI.DrawTexture(new Rect(center.x - 12f, center.y - 182f, 24f, 12f), tex);
            GUI.matrix = matrix;
        }

        float markerAge = Time.time - hitMarkerTime;
        if (markerAge < 0.18f)
        {
            var matrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(45f, center);
            GUI.color = hitMarkerKill ? new Color(1f, 0.2f, 0.15f, 1f) : new Color(1f, 1f, 1f, 0.95f);
            GUI.DrawTexture(new Rect(center.x - 14f, center.y - 1.5f, 9f, 3f), tex);
            GUI.DrawTexture(new Rect(center.x + 5f, center.y - 1.5f, 9f, 3f), tex);
            GUI.DrawTexture(new Rect(center.x - 1.5f, center.y - 14f, 3f, 9f), tex);
            GUI.DrawTexture(new Rect(center.x - 1.5f, center.y + 5f, 3f, 9f), tex);
            GUI.matrix = matrix;
        }

        if (mainCamera != null)
        {
            foreach (var e in enemies)
            {
                if (e == null || e.IsDead) continue;
                bool recentlyHit = Time.time - e.LastHitTime < 2.5f;
                if (!recentlyHit && !(visible.TryGetValue(e, out bool v) && v)) continue;

                DrawHealthBar(e.HeadPosition, (float)e.Health / Mathf.Max(1, e.MaxHealth), 54f);
            }

            if (turrets != null)
            {
                foreach (var t in turrets)
                {
                    if (t == null || t.IsDead) continue;
                    bool recentlyHit = Time.time - t.LastHitTime < 2.5f;
                    if (recentlyHit || (cameraTransform != null && HasLineOfSight(cameraTransform.position, t.BarAnchor)))
                        DrawHealthBar(t.BarAnchor, (float)t.Health / Mathf.Max(1, t.MaxHealth), 76f);
                }
            }
        }

        GUI.color = prev;
    }

    private void DrawHealthBar(Vector3 worldPoint, float pct, float width)
    {
        Vector3 sp = mainCamera.WorldToScreenPoint(worldPoint);
        if (sp.z <= 0f) return;

        var tex = Texture2D.whiteTexture;
        const float h = 7f;
        float x = sp.x - width * 0.5f;
        float y = Screen.height - sp.y - h;
        pct = Mathf.Clamp01(pct);

        GUI.color = new Color(0f, 0f, 0f, 0.65f);
        GUI.DrawTexture(new Rect(x, y, width, h), tex);
        GUI.color = Color.Lerp(new Color(0.95f, 0.2f, 0.2f), new Color(0.3f, 0.9f, 0.4f), pct);
        GUI.DrawTexture(new Rect(x + 1f, y + 1f, (width - 2f) * pct, h - 2f), tex);
    }

    private void OnDestroy()
    {
        if (fxMaterial != null) Destroy(fxMaterial);
    }
}
