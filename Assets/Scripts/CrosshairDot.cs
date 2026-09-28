using System.Collections.Generic;
using InfimaGames.LowPolyShooterPack;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The aiming dot in the middle of the screen for the gun levels (Training Ground, Level 1-5):
/// white, and red while a shot would land on a live enemy. It asks the same questions the hit
/// detectors do - StandaloneTurretShotDetector's and BossHitDetector's camera-centre ray against
/// the enemies' own colliders, CombatEncounterManager2's body/head test for EnemyAI2 soldiers -
/// so red means "this shot hits".
///
/// It creates itself at startup, shows only where the player carries the Infima gun rig, and hides
/// while paused, in cutscenes and terminals (Infima cursor unlocked) and once the player is dead.
/// </summary>
public class CrosshairDot : MonoBehaviour
{
    private const float Range = 500f;
    // Dot diameter in pixels at 1080p; it scales with the screen height.
    private const float DotSize = 6f;
    // EnemyAI2 shots are forgiving at range - same numbers as CombatEncounterManager2.
    private const float ForgivenessBase = 0.06f;
    private const float ForgivenessPerMeter = 0.007f;

    private static CrosshairDot instance;

    private Character player;
    private Transform cam;
    private PlayerHealth health;
    private readonly List<EnemyAI2> soldiers = new List<EnemyAI2>();
    private float soldierRefresh;
    private readonly RaycastHit[] hits = new RaycastHit[32];
    private int projectileLayer;
    private bool onEnemy;
    private Texture2D dot;

    /// <summary>True while the dot is red.</summary>
    public static bool IsOnEnemy => instance != null && instance.onEnemy;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null) return;
        var go = new GameObject("~CrosshairDot");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<CrosshairDot>();
    }

    private void Awake()
    {
        dot = BuildDot();
        projectileLayer = LayerMask.NameToLayer("Projectile");
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindPlayer();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (dot != null) Destroy(dot);
        if (instance == this) instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => FindPlayer();

    private void FindPlayer()
    {
        player = null;
        cam = null;
        health = null;
        soldiers.Clear();
        soldierRefresh = 0f;

        var go = GameObject.FindGameObjectWithTag("Player");
        if (go == null) return;
        player = go.GetComponent<Character>();
        if (player == null) return;   // Bedroom / School: no gun, no dot
        var camera = player.GetCameraWorld();
        cam = camera != null ? camera.transform : null;
        health = go.GetComponent<PlayerHealth>();
    }

    private bool Visible =>
        player != null && cam != null && cam.gameObject.activeInHierarchy
        && !PauseMenu.IsPaused && player.IsCursorLocked()
        && (health == null || !health.IsDead);

    private void Update()
    {
        onEnemy = false;
        if (!Visible) return;

        // EnemyAI2 soldiers are spawned by their encounters over time.
        soldierRefresh -= Time.unscaledDeltaTime;
        if (soldierRefresh <= 0f)
        {
            soldierRefresh = 1f;
            soldiers.Clear();
            soldiers.AddRange(FindObjectsByType<EnemyAI2>(FindObjectsSortMode.None));
        }

        Vector3 origin = cam.position;
        Vector3 dir = cam.forward;
        int n = Physics.RaycastNonAlloc(origin, dir, hits, Range, ~0, QueryTriggerInteraction.Ignore);

        Collider first = null;
        float firstDist = Range;
        float wallDist = Range;   // nearest thing that isn't a soldier, for the soldier test
        for (int i = 0; i < n; i++)
        {
            var col = hits[i].collider;
            if (col.transform.root == player.transform.root || col.gameObject.layer == projectileLayer) continue;
            float d = hits[i].distance;
            if (d < firstDist) { firstDist = d; first = col; }
            if (d < wallDist && col.GetComponentInParent<EnemyAI2>() == null) wallDist = d;
        }

        onEnemy = (first != null && IsLiveEnemy(first)) || SoldierOnRay(origin, dir, wallDist);
    }

    /// <summary>The enemies the ray-based detectors damage, while they can still be hurt.</summary>
    private bool IsLiveEnemy(Collider c)
    {
        var soldier = c.GetComponentInParent<EnemyAI2>();
        if (soldier != null) return !soldier.IsDead;

        var turret = c.GetComponentInParent<StandaloneTurretEnemy>();
        if (turret != null)
        {
            // Same rule as StandaloneTurretShotDetector: an ordinary turret only takes damage from
            // its own floor.
            bool sameFloor = Mathf.Abs(player.transform.position.y - turret.transform.position.y) <= turret.MaxHeightDifference;
            return !turret.IsDead && (turret.CrossFloor || sameFloor);
        }

        var oldTurret = c.GetComponentInParent<TurretEnemy>();
        if (oldTurret != null) return !oldTurret.IsDead;
        var mech = c.GetComponentInParent<StandaloneMechEnemy>();
        if (mech != null) return !mech.IsDead;
        var white = c.GetComponentInParent<RobotSoldierWhiteEnemy>();
        if (white != null) return !white.IsDead;
        var blue = c.GetComponentInParent<RobotSoldierBlueEnemy>();
        if (blue != null) return !blue.IsDead;
        var robot = c.GetComponentInParent<BlueRobotEnemy>();
        if (robot != null) return !robot.IsDead;
        var striker = c.GetComponentInParent<MediumMechStrikerBoss>();
        if (striker != null) return !striker.IsDead;
        var spider = c.GetComponentInParent<BossHealthManager>();
        if (spider != null) return !spider.IsDead;
        return false;
    }

    /// <summary>CombatEncounterManager2's shot test: a body segment and a head sphere, a little
    /// wider with distance, and nothing solid in front.</summary>
    private bool SoldierOnRay(Vector3 origin, Vector3 dir, float wallDist)
    {
        foreach (var e in soldiers)
        {
            if (e == null || e.IsDead) continue;
            Vector3 head = e.HeadCenter;
            Vector3 a = e.transform.position + Vector3.up * 0.3f;
            Vector3 b = head - Vector3.up * 0.18f;
            ClosestRaySegment(origin, dir, a, b, out float along, out Vector3 onSegment);

            float headAlong = Vector3.Dot(head - origin, dir);
            float headPerp = Vector3.Distance(origin + dir * headAlong, head);
            float forgiveness = ForgivenessBase + ForgivenessPerMeter * Mathf.Max(along, headAlong);

            bool headHit = headAlong > 0f && headAlong < wallDist && headPerp < 0.15f + forgiveness * 0.6f;
            bool bodyHit = along > 0f && along < wallDist && Vector3.Distance(origin + dir * along, onSegment) < 0.3f + forgiveness;
            if (headHit || bodyHit) return true;
        }
        return false;
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

    private void OnGUI()
    {
        if (Event.current.type != EventType.Repaint || !Visible) return;
        GUI.depth = -400;

        float size = Mathf.Max(4f, DotSize * Screen.height / 1080f);
        float cx = Screen.width * 0.5f, cy = Screen.height * 0.5f;

        // A thin dark rim keeps the white dot readable on bright walls.
        GUI.color = new Color(0f, 0f, 0f, 0.55f);
        GUI.DrawTexture(new Rect(cx - size * 0.5f - 1.5f, cy - size * 0.5f - 1.5f, size + 3f, size + 3f), dot);
        GUI.color = onEnemy ? new Color(1f, 0.15f, 0.12f, 1f) : new Color(1f, 1f, 1f, 0.95f);
        GUI.DrawTexture(new Rect(cx - size * 0.5f, cy - size * 0.5f, size, size), dot);
        GUI.color = Color.white;
    }

    /// <summary>A soft-edged white disc, tinted when drawn.</summary>
    private static Texture2D BuildDot()
    {
        const int res = 32;
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false)
        {
            hideFlags = HideFlags.HideAndDontSave,
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };
        var px = new Color32[res * res];
        float r = res * 0.5f;
        for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, r));
                byte a = (byte)(Mathf.Clamp01(r - d) * 255f);
                px[y * res + x] = new Color32(255, 255, 255, a);
            }
        tex.SetPixels32(px);
        tex.Apply(false);
        return tex;
    }
}
