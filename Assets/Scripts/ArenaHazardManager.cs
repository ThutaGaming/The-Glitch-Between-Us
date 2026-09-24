using System.Collections;
using UnityEngine;

/// <summary>
/// Mechanic 4 - Phase 2 arena hazard. When the boss drops below its phase-2 health line,
/// BossAIController calls <see cref="Activate"/>: the high-ground platforms grind up out of the floor,
/// then the whole plaza floor starts cycling
///
///     WARNING (pulsing blue, hiss)  ->  LIVE (arcing cyan, lightning, heavy damage)  ->  COOLDOWN
///
/// While LIVE, standing on the plaza floor deals <see cref="damagePerSecond"/> and blocks regen.
/// Standing on a platform, or being airborne, is safe - so the fight becomes "read the warning,
/// get up high, keep shooting", not "tank it and heal behind cover".
///
/// The player can only jump ~1.06 m (jumpForce 6, gravity -17), so the platforms are 0.7 m tall -
/// the existing 1.1-1.8 m columns/walls in this plaza are not reachable.
/// </summary>
public class ArenaHazardManager : MonoBehaviour
{
    public enum FloorState { Dormant, Warning, Live, Cooldown }

    [Header("Plaza floor")]
    [SerializeField] private float floorY = 2.42f;
    [SerializeField] private Vector3 arenaCenter = new Vector3(358.5f, 2.42f, 247.5f);
    [SerializeField] private float arenaRadius = 11.5f;
    [Tooltip("Feet this far above the plaza floor count as 'on high ground' (safe).")]
    [SerializeField] private float safeHeightAboveFloor = 0.4f;

    [Header("Cycle (seconds)")]
    [SerializeField] private float warningDuration = 2.5f;
    [SerializeField] private float liveDuration = 3.5f;
    [SerializeField] private float cooldownDuration = 6f;
    [SerializeField] private float damagePerSecond = 12f;

    [Header("Rising high-ground platforms")]
    [SerializeField] private Transform[] platforms;
    [SerializeField] private float platformBuryDepth = 0.75f;
    [SerializeField] private float platformRiseTime = 2.2f;

    [Header("FX / audio")]
    [SerializeField] private SpiderMechFxLibrary fx;

    private Transform player;
    private PlayerHealth playerHealth;
    private Vector3[] platformRaised;
    private MeshRenderer overlay;
    private Material overlayMaterial;
    private AudioSource humSource;
    private FloorState state = FloorState.Dormant;
    private float stateStarted;

    public bool IsActive => state != FloorState.Dormant;
    public FloorState State => state;
    public float StateProgress01 => Mathf.Clamp01((Time.time - stateStarted) / CurrentStateDuration);
    public bool PlayerEndangered { get; private set; }

    private float CurrentStateDuration => state switch
    {
        FloorState.Warning => warningDuration,
        FloorState.Live => liveDuration,
        FloorState.Cooldown => cooldownDuration,
        _ => 1f
    };

    private void Awake()
    {
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go != null)
        {
            player = go.transform;
            playerHealth = go.GetComponent<PlayerHealth>();
        }

        if (platforms != null)
        {
            platformRaised = new Vector3[platforms.Length];
            for (int i = 0; i < platforms.Length; i++)
            {
                if (platforms[i] == null) continue;
                platformRaised[i] = platforms[i].position;
                platforms[i].position = platformRaised[i] - Vector3.up * platformBuryDepth;
            }
        }

        BuildOverlay();

        humSource = gameObject.AddComponent<AudioSource>();
        humSource.loop = true;
        humSource.spatialBlend = 0f;
        humSource.playOnAwake = false;
        humSource.volume = 0f;
        if (fx != null) humSource.clip = fx.electricLoop;
    }

    /// <summary>Idempotent: BossAIController calls this once when phase 2 begins.</summary>
    public void Activate()
    {
        if (state != FloorState.Dormant) return;
        state = FloorState.Cooldown;
        stateStarted = Time.time;
        StartCoroutine(RaisePlatforms());
        StartCoroutine(Cycle());
    }

    public void Deactivate()
    {
        StopAllCoroutines();
        state = FloorState.Dormant;
        PlayerEndangered = false;
        if (overlay != null) overlay.enabled = false;
        if (humSource != null) humSource.Stop();
    }

    private IEnumerator RaisePlatforms()
    {
        if (platforms == null) yield break;

        for (int i = 0; i < platforms.Length; i++)
        {
            if (platforms[i] == null) continue;
            if (fx != null)
            {
                BossFx.Play(fx.platformDust, platformRaised[i], Quaternion.identity, 1.4f, platformRiseTime);
                BossFx.Sfx(fx.heavyImpact, platformRaised[i], 0.9f, 0.55f, 6f, 60f);
            }
        }
        CameraShake.Shake(arenaCenter, 0.45f, 40f);

        float t = 0f;
        while (t < platformRiseTime)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / platformRiseTime);
            for (int i = 0; i < platforms.Length; i++)
                if (platforms[i] != null)
                    platforms[i].position = platformRaised[i] - Vector3.up * (platformBuryDepth * (1f - k));
            yield return null;
        }
        for (int i = 0; i < platforms.Length; i++)
            if (platforms[i] != null) platforms[i].position = platformRaised[i];

        if (fx != null)
            foreach (var p in platformRaised)
                BossFx.Sfx(BossFx.Pick(fx.metalDebris), p, 0.9f, 0.7f, 5f, 50f);
    }

    private IEnumerator Cycle()
    {
        // First cooldown gives the platforms time to rise before the floor ever goes live.
        yield return new WaitForSeconds(Mathf.Max(platformRiseTime + 0.5f, cooldownDuration * 0.5f));

        while (true)
        {
            SetState(FloorState.Warning);
            if (fx != null) BossFx.Sfx(fx.warningClip, arenaCenter, 1f, 1.1f, 0f, 200f, 0f);
            float next = 0f;
            while (Time.time - stateStarted < warningDuration)
            {
                if (Time.time >= next)
                {
                    next = Time.time + 0.18f;
                    if (fx != null) BossFx.Play(fx.electroHit, RandomFloorPoint(), Quaternion.identity, 0.35f, 0.08f);
                }
                yield return null;
            }

            SetState(FloorState.Live);
            if (humSource.clip != null) { humSource.pitch = 1.6f; humSource.Play(); }
            next = 0f;
            while (Time.time - stateStarted < liveDuration)
            {
                if (Time.time >= next)
                {
                    next = Time.time + Random.Range(0.08f, 0.16f);
                    Strike();
                }
                yield return null;
            }

            SetState(FloorState.Cooldown);
            yield return new WaitForSeconds(cooldownDuration);
        }
    }

    private void SetState(FloorState s)
    {
        state = s;
        stateStarted = Time.time;
    }

    private void Strike()
    {
        // Bias some strikes toward the player so the floor visibly "hunts" whoever is still on it.
        Vector3 p = (player != null && Random.value < 0.3f)
            ? player.position + new Vector3(Random.Range(-2.5f, 2.5f), 0f, Random.Range(-2.5f, 2.5f))
            : RandomFloorPoint();
        p.y = floorY + 0.05f;

        if (fx != null)
        {
            BossFx.Play(fx.lightningStrike, p, Quaternion.identity, Random.Range(0.6f, 0.9f), 0.12f);
            BossFx.Play(fx.electroHit, p, Quaternion.identity, 0.5f, 0.1f);
            if (Random.value < 0.35f) BossFx.Sfx(BossFx.Pick(fx.boltImpacts), p, 0.5f, Random.Range(1.5f, 1.9f), 3f, 35f);
        }
        BossFx.Flash(p + Vector3.up * 1.5f, new Color(0.4f, 0.85f, 1f), 6f, 6f, 0.12f);
    }

    private Vector3 RandomFloorPoint()
    {
        Vector2 r = Random.insideUnitCircle * arenaRadius * 0.9f;
        return new Vector3(arenaCenter.x + r.x, floorY + 0.05f, arenaCenter.z + r.y);
    }

    private void Update()
    {
        UpdateOverlay();

        PlayerEndangered = false;
        if (state != FloorState.Live && state != FloorState.Warning) return;
        if (player == null || playerHealth == null || playerHealth.IsDead) return;
        if (!PlayerOnPlazaFloor()) return;

        PlayerEndangered = true;
        if (state != FloorState.Live) return;

        playerHealth.ApplyDamageOverTime(damagePerSecond);
        playerHealth.SuppressRegen(1.5f);
        CameraShake.Shake(player.position, 0.6f * Time.deltaTime, 0f);
    }

    /// <summary>True when the player's feet are on the plaza floor itself - not on a platform and not in the air.</summary>
    public bool PlayerOnPlazaFloor()
    {
        Vector3 feet = player.position;
        Vector2 flat = new Vector2(feet.x - arenaCenter.x, feet.z - arenaCenter.z);
        if (flat.magnitude > arenaRadius) return false;

        float nearest = float.MaxValue;
        float groundY = float.MinValue;
        foreach (var h in Physics.RaycastAll(feet + Vector3.up * 0.25f, Vector3.down, 0.6f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (h.collider.transform.root == player.root) continue;
            if (h.distance < nearest) { nearest = h.distance; groundY = h.point.y; }
        }
        if (nearest == float.MaxValue) return false;
        return groundY < floorY + safeHeightAboveFloor;
    }

    // ---------- floor overlay ----------

    private void BuildOverlay()
    {
        var go = new GameObject("ElectrifiedFloorOverlay");
        go.transform.SetParent(transform, false);
        go.transform.position = new Vector3(arenaCenter.x, floorY + 0.03f, arenaCenter.z);

        const int segments = 64;
        var verts = new Vector3[segments + 1];
        var uvs = new Vector2[segments + 1];
        var tris = new int[segments * 3];
        verts[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);
        for (int i = 0; i < segments; i++)
        {
            float a = i * Mathf.PI * 2f / segments;
            verts[i + 1] = new Vector3(Mathf.Cos(a) * arenaRadius, 0f, Mathf.Sin(a) * arenaRadius);
            uvs[i + 1] = new Vector2(verts[i + 1].x, verts[i + 1].z) * 0.5f;
            tris[i * 3] = 0;
            tris[i * 3 + 1] = i == segments - 1 ? 1 : i + 2;
            tris[i * 3 + 2] = i + 1;
        }
        var mesh = new Mesh { name = "ElectrifiedDisc", vertices = verts, uv = uvs, triangles = tris };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        overlay = go.AddComponent<MeshRenderer>();
        overlayMaterial = new Material(Shader.Find("Sprites/Default")) { mainTexture = BuildGridTexture() };
        overlayMaterial.mainTexture.wrapMode = TextureWrapMode.Repeat;
        overlay.sharedMaterial = overlayMaterial;
        overlay.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        overlay.receiveShadows = false;
        overlay.enabled = false;
    }

    /// <summary>Procedural hex-ish circuit grid with soft glow, so the live floor reads as energised.</summary>
    private static Texture2D BuildGridTexture()
    {
        const int size = 128;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, true);
        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float gx = Mathf.Abs((x % 32) - 16f) / 16f;
            float gy = Mathf.Abs((y % 32) - 16f) / 16f;
            float line = Mathf.Max(Mathf.Pow(gx, 10f), Mathf.Pow(gy, 10f));
            float diag = Mathf.Pow(1f - Mathf.Abs(((x + y) % 64) - 32f) / 32f, 18f) * 0.5f;
            float a = Mathf.Clamp01(0.18f + line * 0.9f + diag);
            tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
        }
        tex.Apply();
        return tex;
    }

    private void UpdateOverlay()
    {
        if (overlay == null) return;

        switch (state)
        {
            case FloorState.Warning:
            {
                overlay.enabled = true;
                float p = 0.5f + 0.5f * Mathf.Sin(Time.time * Mathf.Lerp(6f, 22f, StateProgress01));
                overlayMaterial.color = new Color(0.25f, 0.55f, 1f, Mathf.Lerp(0.08f, 0.35f, p));
                break;
            }
            case FloorState.Live:
            {
                overlay.enabled = true;
                float flicker = 0.55f + Mathf.PerlinNoise(Time.time * 30f, 0.5f) * 0.45f;
                overlayMaterial.color = new Color(0.45f, 0.95f, 1f, flicker);
                overlayMaterial.mainTextureOffset = new Vector2(Time.time * 0.6f, Time.time * 0.35f);
                if (humSource != null) humSource.volume = Mathf.MoveTowards(humSource.volume, PlayerEndangered ? 1f : 0.55f, Time.deltaTime * 4f);
                break;
            }
            default:
                overlay.enabled = false;
                if (humSource != null && humSource.isPlaying)
                {
                    humSource.volume = Mathf.MoveTowards(humSource.volume, 0f, Time.deltaTime * 3f);
                    if (humSource.volume <= 0.01f) humSource.Stop();
                }
                break;
        }
    }

    private void OnDestroy()
    {
        if (overlayMaterial != null)
        {
            if (overlayMaterial.mainTexture != null) Destroy(overlayMaterial.mainTexture);
            Destroy(overlayMaterial);
        }
    }
}
