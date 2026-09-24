using UnityEngine;

/// <summary>
/// Mechanic 2's delivery: an incendiary shell lobbed in a high arc onto the spot the player has been
/// camping. The landing zone is telegraphed the whole flight - a red ring at the exact blast radius
/// with an inner ring closing in like a countdown - so it is always fair to dodge, never fair to ignore.
/// On impact: a blast (falloff damage) and a lingering <see cref="HazardZone"/> fire patch.
/// </summary>
public class BossMortar : MonoBehaviour
{
    private static int incoming;
    private static Material ringMaterial;

    /// <summary>Number of shells currently in the air (drives the HUD "INCOMING" warning).</summary>
    public static int Incoming => incoming;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => incoming = 0;

    private Vector3 start;
    private Vector3 target;
    private float flightTime;
    private float apexHeight;
    private float elapsed;
    private float radius;
    private float impactDamage;
    private float fireDps;
    private float fireDuration;
    private SpiderMechFxLibrary fx;
    private GameObject visual;
    private GameObject telegraphFx;
    private LineRenderer outerRing;
    private LineRenderer innerRing;
    private bool detonated;

    public static void Launch(Vector3 from, Vector3 groundTarget, float flightTime, float apexHeight, float radius,
        float impactDamage, float fireDps, float fireDuration, SpiderMechFxLibrary fx)
    {
        var go = new GameObject("BossIncendiaryShell");
        go.transform.position = from;
        var m = go.AddComponent<BossMortar>();
        m.start = from;
        m.target = groundTarget;
        m.flightTime = Mathf.Max(0.3f, flightTime);
        m.apexHeight = apexHeight;
        m.radius = radius;
        m.impactDamage = impactDamage;
        m.fireDps = fireDps;
        m.fireDuration = fireDuration;
        m.fx = fx;
        m.Begin();
    }

    private void Begin()
    {
        incoming++;

        if (fx != null)
        {
            visual = BossFx.Loop(fx.mortarShell, transform.position, Quaternion.identity, 0.9f, transform);
            telegraphFx = BossFx.Loop(fx.mortarTelegraph, target + Vector3.up * 0.05f, Quaternion.identity, radius * 0.5f);
        }

        if (ringMaterial == null) ringMaterial = new Material(Shader.Find("Sprites/Default"));
        outerRing = BuildRing("TelegraphOuter", radius, 0.12f);
        innerRing = BuildRing("TelegraphInner", radius, 0.08f);

        var trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = 0.45f;
        trail.startWidth = 0.45f;
        trail.endWidth = 0f;
        trail.sharedMaterial = ringMaterial;
        trail.startColor = new Color(1f, 0.55f, 0.1f, 0.9f);
        trail.endColor = new Color(0.3f, 0.3f, 0.3f, 0f);

        var glow = gameObject.AddComponent<Light>();
        glow.type = LightType.Point;
        glow.color = new Color(1f, 0.5f, 0.15f);
        glow.range = 6f;
        glow.intensity = 4f;
        glow.shadows = LightShadows.None;
    }

    private LineRenderer BuildRing(string name, float r, float width)
    {
        var go = new GameObject(name);
        go.transform.position = target + Vector3.up * 0.08f;
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.positionCount = 48;
        lr.startWidth = lr.endWidth = width;
        lr.sharedMaterial = ringMaterial;
        lr.startColor = lr.endColor = new Color(1f, 0.15f, 0.05f, 0.9f);
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        SetRingRadius(lr, r);
        return lr;
    }

    private static void SetRingRadius(LineRenderer lr, float r)
    {
        for (int i = 0; i < lr.positionCount; i++)
        {
            float a = i * Mathf.PI * 2f / lr.positionCount;
            lr.SetPosition(i, new Vector3(Mathf.Cos(a) * r, 0f, Mathf.Sin(a) * r));
        }
    }

    private void Update()
    {
        if (detonated) return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / flightTime);

        Vector3 flat = Vector3.Lerp(start, target, t);
        Vector3 pos = flat + Vector3.up * (apexHeight * 4f * t * (1f - t));
        Vector3 vel = pos - transform.position;
        transform.position = pos;
        if (vel.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(vel.normalized);

        float pulse = 0.55f + 0.45f * Mathf.Sin(Time.time * (10f + 20f * t));
        if (outerRing != null) outerRing.startColor = outerRing.endColor = new Color(1f, 0.15f, 0.05f, 0.5f + 0.5f * pulse);
        if (innerRing != null) SetRingRadius(innerRing, Mathf.Lerp(radius, 0.2f, t));

        if (t >= 1f) Detonate();
    }

    private void Detonate()
    {
        detonated = true;
        incoming = Mathf.Max(0, incoming - 1);

        if (fx != null)
        {
            BossFx.Play(fx.mortarExplosion, target + Vector3.up * 0.3f, Quaternion.identity, 1.1f, 0.35f);
            BossFx.Play(fx.mortarGroundBlast, target + Vector3.up * 0.05f, Quaternion.identity, radius * 0.22f, 0.3f);
            BossFx.Sfx(BossFx.Pick(fx.explosions), target, 1f, Random.Range(0.85f, 1f), 8f, 110f);
            BossFx.Sfx(fx.heavyImpact, target, 0.8f, 0.7f, 6f, 70f);
        }
        BossFx.Flash(target + Vector3.up * 1.2f, new Color(1f, 0.5f, 0.15f), 9f, radius * 3f, 0.4f);
        CameraShake.Shake(target, 0.7f, 22f);

        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null)
        {
            var ph = playerGo.GetComponent<PlayerHealth>();
            Vector3 d = playerGo.transform.position - target;
            float flatDistance = new Vector2(d.x, d.z).magnitude;
            if (ph != null && !ph.IsDead && flatDistance <= radius && Mathf.Abs(d.y) < 2.5f)
            {
                float falloff = Mathf.Lerp(1f, 0.5f, flatDistance / radius);
                ph.ApplyDamage(Mathf.RoundToInt(impactDamage * falloff));
                var feedback = playerGo.GetComponent<PlayerHitFeedback>();
                if (feedback != null) feedback.Notify(target);
            }
        }

        HazardZone.Spawn(target, radius, fireDps, fireDuration, fx);

        if (telegraphFx != null) BossFx.Release(telegraphFx);
        if (visual != null) BossFx.Release(visual);
        if (outerRing != null) Destroy(outerRing.gameObject);
        if (innerRing != null) Destroy(innerRing.gameObject);
        var trail = GetComponent<TrailRenderer>();
        if (trail != null) trail.emitting = false;
        var glow = GetComponent<Light>();
        if (glow != null) glow.enabled = false;
        Destroy(gameObject, 0.5f);
    }

    private void OnDestroy()
    {
        if (!detonated) incoming = Mathf.Max(0, incoming - 1);
        if (outerRing != null) Destroy(outerRing.gameObject);
        if (innerRing != null) Destroy(innerRing.gameObject);
        if (telegraphFx != null) Destroy(telegraphFx);
    }
}
