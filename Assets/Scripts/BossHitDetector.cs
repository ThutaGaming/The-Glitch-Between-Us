using System.Collections.Generic;
using UnityEngine;
using InfimaGames.LowPolyShooterPack;

/// <summary>
/// Player-side half of the Spider-Mech fight:
///
///  * Registers the player's shots on the boss (same "equipped weapon's ammo dropped" detection as the
///    other enemies here) and routes them to weak points first - a joint/core sphere sits inside the
///    hull/leg colliders, so the ray is allowed to reach a weak point just behind the first boss surface.
///  * Normalises damage to the design's 75 DPS: the AR fires 610 RPM, so each bullet is worth
///    75 * 60 / 610 = 7.4; a semi-auto (click-limited to ~5/s) is worth the design's 15 per shot.
///  * Impact sparks / sounds per surface (shield, hull, weak point), hitmarkers and crit numbers.
///  * The boss HUD: health + shield bars, phase, the Ultimate charge meter and hazard call-outs.
/// </summary>
public class BossHitDetector : MonoBehaviour
{
    [Header("Damage model")]
    [Tooltip("Design burst DPS of the player's gun (15 dmg x 5 shots/s).")]
    [SerializeField] private float targetBurstDps = 75f;
    [Tooltip("Semi-auto weapons are treated as firing at most this fast (click-limited).")]
    [SerializeField] private float semiAutoMaxRpm = 300f;
    [SerializeField] private float raycastDistance = 400f;
    [Tooltip("How far past the first boss surface a weak point may still be credited.")]
    [SerializeField] private float weakPointReach = 0.7f;

    [SerializeField] private SpiderMechFxLibrary fx;

    private Character player;
    private InventoryBehaviour inventory;
    private Transform cameraTransform;
    private Camera mainCamera;
    private int lastAmmo = -1;

    private BossHealthManager boss;
    private BossAIController bossAI;
    private ArenaHazardManager arena;

    private float lastImpactSound;
    private float hitMarkerTime = -9f;
    private Color hitMarkerColor = Color.white;
    private float displayedHealth01 = 1f;
    private float ghostHealth01 = 1f;
    private float ghostHoldUntil;

    private struct DamageNumber { public Vector3 pos; public string text; public Color color; public float born; public float size; }
    private readonly List<DamageNumber> numbers = new List<DamageNumber>();

    private GUIStyle titleStyle, smallStyle, bannerStyle, numberStyle;
    private int stylesForHeight;

    private void Awake()
    {
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null)
        {
            player = playerGo.GetComponent<Character>();
            if (player != null)
            {
                inventory = player.GetInventory();
                cameraTransform = player.GetCameraWorld().transform;
                mainCamera = cameraTransform.GetComponent<Camera>();
            }
        }

        boss = FindFirstObjectByType<BossHealthManager>();
        bossAI = FindFirstObjectByType<BossAIController>();
        arena = FindFirstObjectByType<ArenaHazardManager>();
    }

    private void Update()
    {
        if (inventory == null) return;

        WeaponBehaviour equipped = inventory.GetEquipped();
        if (equipped == null) { lastAmmo = -1; return; }

        int current = equipped.GetAmmunitionCurrent();
        if (lastAmmo >= 0 && current < lastAmmo) TryRegisterShot(equipped);
        lastAmmo = current;

        if (boss != null)
        {
            float h = boss.HealthNormalized;
            displayedHealth01 = h;
            if (h < ghostHealth01 && Time.time > ghostHoldUntil) ghostHealth01 = Mathf.MoveTowards(ghostHealth01, h, Time.deltaTime * 0.35f);
            if (h > ghostHealth01) ghostHealth01 = h;
        }
    }

    private float DamagePerShot(WeaponBehaviour weapon)
    {
        float rpm = Mathf.Max(1f, weapon.GetRateOfFire());
        if (!weapon.IsAutomatic()) rpm = Mathf.Min(rpm, semiAutoMaxRpm);
        return targetBurstDps * 60f / rpm;
    }

    private void TryRegisterShot(WeaponBehaviour weapon)
    {
        if (cameraTransform == null || boss == null || boss.IsDead) return;

        Vector3 origin = cameraTransform.position;
        Vector3 dir = cameraTransform.forward;
        var hits = Physics.RaycastAll(origin, dir, raycastDistance, ~0, QueryTriggerInteraction.Ignore);

        float obstacle = raycastDistance;
        float firstBoss = float.MaxValue;
        RaycastHit bossHit = default;
        foreach (var h in hits)
        {
            if (player != null && h.collider.transform.root == player.transform.root) continue;
            bool isBoss = h.collider.GetComponentInParent<BossHealthManager>() == boss;
            if (!isBoss)
            {
                if (h.distance < obstacle) obstacle = h.distance;
                continue;
            }
            if (h.distance < firstBoss) { firstBoss = h.distance; bossHit = h; }
        }
        if (firstBoss >= obstacle) return;

        // Prefer a weak point that sits just behind the first boss surface the ray met.
        WeakPoint weak = null;
        RaycastHit weakHit = default;
        float bestWeak = float.MaxValue;
        foreach (var h in hits)
        {
            var wp = h.collider.GetComponent<WeakPoint>();
            if (wp == null || !h.collider.enabled) continue;
            if (h.distance >= obstacle || h.distance > firstBoss + weakPointReach) continue;
            if (h.distance < bestWeak) { bestWeak = h.distance; weak = wp; weakHit = h; }
        }

        float baseDamage = DamagePerShot(weapon);
        if (weak != null)
        {
            float dealt = weak.RegisterHit(baseDamage, weakHit.point);
            if (dealt > 0f) OnWeakPointHit(weakHit.point, dealt, weak.IsChargeCore);
            return;
        }

        bool shieldBefore = boss.ShieldUp;
        float dealtBody = boss.ApplyDamage(baseDamage, bossHit.point, BossHealthManager.HitKind.Body);
        if (dealtBody <= 0f) return;
        OnBodyHit(bossHit.point, bossHit.normal, shieldBefore);
    }

    private void OnWeakPointHit(Vector3 point, float dealt, bool core)
    {
        ghostHoldUntil = Time.time + 0.5f;
        hitMarkerTime = Time.time;
        hitMarkerColor = core ? new Color(1f, 0.85f, 0.15f) : new Color(1f, 0.6f, 0.15f);
        numbers.Add(new DamageNumber
        {
            pos = point, text = Mathf.RoundToInt(dealt).ToString(), born = Time.time,
            color = hitMarkerColor, size = core ? 1.35f : 1.1f
        });

        if (fx == null) return;
        BossFx.Play(fx.weakPointHit, point, Quaternion.identity, core ? 0.55f : 0.4f, 0.06f);
        if (Time.time - lastImpactSound > 0.05f)
        {
            lastImpactSound = Time.time;
            BossFx.Sfx(fx.weakPointHitClip, point, 0.9f, Random.Range(1.1f, 1.3f), 4f, 60f, 0.5f);
        }
    }

    private void OnBodyHit(Vector3 point, Vector3 normal, bool shielded)
    {
        ghostHoldUntil = Time.time + 0.5f;
        hitMarkerTime = Time.time;
        hitMarkerColor = shielded ? new Color(0.35f, 0.85f, 1f) : Color.white;

        if (fx == null) return;
        var rot = Quaternion.LookRotation(normal.sqrMagnitude > 0.01f ? normal : -cameraTransform.forward);
        BossFx.Play(shielded ? fx.shieldHit : fx.bodyHit, point, rot, shielded ? 0.45f : 0.35f, 0.05f);
        if (Time.time - lastImpactSound > 0.08f)
        {
            lastImpactSound = Time.time;
            if (shielded) BossFx.Sfx(fx.shieldHitClip, point, 0.35f, Random.Range(1.5f, 1.8f), 4f, 60f, 0.5f);
            else BossFx.Sfx(BossFx.Pick(fx.boltImpacts), point, 0.45f, Random.Range(0.9f, 1.1f), 4f, 60f, 0.5f);
        }
    }

    // =====================================================================================
    // HUD
    // =====================================================================================

    private void EnsureStyles()
    {
        if (titleStyle != null && stylesForHeight == Screen.height) return;
        stylesForHeight = Screen.height;
        float s = Screen.height / 1080f;

        titleStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = Mathf.RoundToInt(22 * s) };
        smallStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = Mathf.RoundToInt(15 * s) };
        bannerStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = Mathf.RoundToInt(28 * s) };
        numberStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = Mathf.RoundToInt(20 * s) };
    }

    private void OnGUI()
    {
        if (boss == null || bossAI == null) return;
        if (bossAI.State == BossAIController.BossState.Dormant) return;

        EnsureStyles();
        var tex = Texture2D.whiteTexture;
        var prev = GUI.color;
        float s = Screen.height / 1080f;

        DrawHitMarker(tex, s);
        DrawDamageNumbers(s);

        if (bossAI.IsDead)
        {
            GUI.color = prev;
            return;
        }

        // ---- Boss bars ----
        float barW = 720f * s, healthH = 22f * s, shieldH = 9f * s;
        float x = Screen.width * 0.5f - barW * 0.5f;
        float y = 44f * s;

        string title = bossAI.BossName + (bossAI.InPhaseTwo ? "   -   PHASE II" : "");
        Label(new Rect(x, y - 30f * s, barW, 26f * s), title, titleStyle, bossAI.InPhaseTwo ? new Color(1f, 0.35f, 0.25f) : Color.white);

        // Shield strip.
        Fill(tex, new Rect(x - 3f, y - 3f, barW + 6f, shieldH + 6f), new Color(0f, 0f, 0f, 0.7f));
        Fill(tex, new Rect(x, y, barW * boss.ShieldNormalized, shieldH), new Color(0.3f, 0.85f, 1f, 0.95f));
        if (boss.IsShieldRegenerating)
        {
            float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * 8f);
            Fill(tex, new Rect(x, y, barW * boss.ShieldNormalized, shieldH), new Color(1f, 1f, 1f, 0.35f * pulse));
        }

        // Health bar with a lagging "ghost" chunk that shows the last burst of damage.
        float hy = y + shieldH + 6f * s;
        Fill(tex, new Rect(x - 3f, hy - 3f, barW + 6f, healthH + 6f), new Color(0f, 0f, 0f, 0.75f));
        Fill(tex, new Rect(x, hy, barW, healthH), new Color(0.18f, 0.02f, 0.02f, 0.9f));
        Fill(tex, new Rect(x, hy, barW * ghostHealth01, healthH), new Color(1f, 0.9f, 0.6f, 0.85f));
        Color healthCol = Color.Lerp(new Color(0.9f, 0.08f, 0.05f), new Color(1f, 0.55f, 0.1f), displayedHealth01);
        if (bossAI.IsStunned) healthCol = Color.Lerp(healthCol, Color.white, 0.35f + 0.25f * Mathf.Sin(Time.time * 14f));
        Fill(tex, new Rect(x, hy, barW * displayedHealth01, healthH), healthCol);
        // Phase-2 notch.
        Fill(tex, new Rect(x + barW * 0.5f - 1f, hy - 4f * s, 2f, healthH + 8f * s), new Color(1f, 1f, 1f, 0.6f));

        string status = !boss.ShieldUp ? "SHIELD DOWN" : boss.IsShieldRegenerating ? "SHIELD REGENERATING" : "";
        if (bossAI.BrokenJoints > 0) status += (status.Length > 0 ? "   |   " : "") + "JOINTS BROKEN " + bossAI.BrokenJoints + "/6";
        if (status.Length > 0)
            Label(new Rect(x, hy + healthH + 4f * s, barW, 20f * s), status, smallStyle,
                boss.ShieldUp ? new Color(0.5f, 0.9f, 1f) : new Color(1f, 0.6f, 0.4f));

        // ---- Ultimate call-outs ----
        float by = hy + healthH + 34f * s;
        if (bossAI.IsWindingUp)
        {
            float blink = Mathf.PingPong(Time.time * 4f, 1f);
            Label(new Rect(0f, by, Screen.width, 36f * s), "!!  OVERLOAD IMMINENT - RELOAD & FIND THE CORE  !!", bannerStyle,
                Color.Lerp(new Color(1f, 0.2f, 0.1f), Color.white, blink));
        }
        else if (bossAI.IsCharging)
        {
            Label(new Rect(0f, by, Screen.width, 36f * s), "CORE EXPOSED - SHOOT THE GLOWING DOME!", bannerStyle, new Color(1f, 0.85f, 0.2f));
            float mw = 520f * s, mh = 16f * s, mx = Screen.width * 0.5f - mw * 0.5f, my = by + 40f * s;
            Fill(tex, new Rect(mx - 3f, my - 3f, mw + 6f, mh + 6f), new Color(0f, 0f, 0f, 0.75f));
            Fill(tex, new Rect(mx, my, mw * bossAI.ChargeProgress, mh), new Color(1f, 0.85f, 0.15f));
            Label(new Rect(mx, my - 1f, mw, mh + 2f), Mathf.RoundToInt(bossAI.CoreDamage) + " / " + Mathf.RoundToInt(bossAI.InterruptThreshold),
                smallStyle, Color.black);
            float timeLeft01 = bossAI.ChargeTimeLeft / 6f;
            Fill(tex, new Rect(mx, my + mh + 5f * s, mw * Mathf.Clamp01(timeLeft01), 5f * s), new Color(1f, 0.2f, 0.1f, 0.9f));
        }
        else if (bossAI.IsStunned)
        {
            Label(new Rect(0f, by, Screen.width, 36f * s), "INTERRUPTED!  SYSTEMS DOWN - x2 DAMAGE", bannerStyle, new Color(0.45f, 1f, 0.5f));
        }
        else if (bossAI.State == BossAIController.BossState.Shockwave)
        {
            Label(new Rect(0f, by, Screen.width, 36f * s), "SHOCKWAVE - JUMP OR GET TO HIGH GROUND!", bannerStyle, new Color(1f, 0.3f, 0.15f));
        }
        else if (bossAI.State == BossAIController.BossState.PhaseShift)
        {
            Label(new Rect(0f, by, Screen.width, 36f * s), "WARNING: ARENA DEFENCES ONLINE", bannerStyle, new Color(1f, 0.3f, 0.15f));
        }

        // ---- Hazard call-outs (lower centre, above the crosshair area) ----
        float wy = Screen.height * 0.64f;
        string hazard = null;
        Color hazardCol = new Color(1f, 0.45f, 0.2f);
        if (arena != null && arena.PlayerEndangered && arena.State == ArenaHazardManager.FloorState.Live)
        {
            hazard = "ELECTRIFIED FLOOR - GET TO HIGH GROUND!";
            hazardCol = new Color(0.4f, 0.9f, 1f);
        }
        else if (arena != null && arena.PlayerEndangered && arena.State == ArenaHazardManager.FloorState.Warning)
        {
            hazard = "FLOOR CHARGING - CLIMB A PLATFORM!";
            hazardCol = new Color(0.4f, 0.75f, 1f);
        }
        else if (HazardZone.PlayerInAnyZone)
        {
            hazard = "BURNING - HEALTH REGEN BLOCKED!";
        }
        else if (BossMortar.Incoming > 0)
        {
            hazard = "INCOMING MORTAR - MOVE!";
        }
        else if (arena != null && !arena.PlayerEndangered && arena.State == ArenaHazardManager.FloorState.Live
                 && bossAI.State == BossAIController.BossState.Combat)
        {
            hazard = "MECH IS CHANNELLING - OPEN FIRE!";
            hazardCol = new Color(1f, 0.85f, 0.3f);
        }
        if (hazard != null)
        {
            float blink = 0.6f + 0.4f * Mathf.Sin(Time.time * 10f);
            Label(new Rect(0f, wy, Screen.width, 32f * s), hazard, bannerStyle, new Color(hazardCol.r, hazardCol.g, hazardCol.b, blink));
        }

        GUI.color = prev;
    }

    private void DrawHitMarker(Texture2D tex, float s)
    {
        float age = Time.time - hitMarkerTime;
        if (age > 0.16f) return;

        Vector2 c = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        var matrix = GUI.matrix;
        GUIUtility.RotateAroundPivot(45f, c);
        GUI.color = new Color(hitMarkerColor.r, hitMarkerColor.g, hitMarkerColor.b, 1f - age / 0.16f);
        float len = 9f * s, gap = 5f * s, w = 3f * s;
        GUI.DrawTexture(new Rect(c.x - gap - len, c.y - w * 0.5f, len, w), tex);
        GUI.DrawTexture(new Rect(c.x + gap, c.y - w * 0.5f, len, w), tex);
        GUI.DrawTexture(new Rect(c.x - w * 0.5f, c.y - gap - len, w, len), tex);
        GUI.DrawTexture(new Rect(c.x - w * 0.5f, c.y + gap, w, len), tex);
        GUI.matrix = matrix;
    }

    private void DrawDamageNumbers(float s)
    {
        if (mainCamera == null) return;
        for (int i = numbers.Count - 1; i >= 0; i--)
        {
            var n = numbers[i];
            float age = Time.time - n.born;
            if (age > 0.9f) { numbers.RemoveAt(i); continue; }

            Vector3 sp = mainCamera.WorldToScreenPoint(n.pos + Vector3.up * (age * 1.2f));
            if (sp.z <= 0f) continue;
            float size = 60f * s * n.size;
            var col = new Color(n.color.r, n.color.g, n.color.b, 1f - age / 0.9f);
            numberStyle.fontSize = Mathf.RoundToInt(20f * s * n.size * (age < 0.08f ? 1.3f : 1f));
            Label(new Rect(sp.x - size, Screen.height - sp.y - size * 0.3f, size * 2f, size * 0.6f), n.text, numberStyle, col);
        }
    }

    private static void Fill(Texture2D tex, Rect r, Color c)
    {
        GUI.color = c;
        GUI.DrawTexture(r, tex);
    }

    private static void Label(Rect r, string text, GUIStyle style, Color c)
    {
        GUI.color = new Color(0f, 0f, 0f, c.a * 0.85f);
        GUI.Label(new Rect(r.x + 2f, r.y + 2f, r.width, r.height), text, style);
        GUI.color = c;
        GUI.Label(r, text, style);
    }
}
