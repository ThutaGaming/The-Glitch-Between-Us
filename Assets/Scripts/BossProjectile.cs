using UnityEngine;

/// <summary>
/// A real, travelling plasma bolt (not a hitscan tracer), so the barrage can be read and strafed
/// out of - "run and gun" only works if moving actually saves you. Sphere-casts ahead every frame so
/// fast bolts can't tunnel through the player's thin capsule, then detonates on whatever it meets.
/// </summary>
public class BossProjectile : MonoBehaviour
{
    private static Material trailMaterial;

    private Vector3 direction;
    private float speed;
    private float damage;
    private float radius;
    private float lifeLeft;
    private Transform owner;
    private SpiderMechFxLibrary fx;
    private GameObject visual;
    private TrailRenderer trail;
    private bool finished;

    public static BossProjectile Spawn(Vector3 origin, Vector3 dir, float speed, float damage, float radius,
        Transform owner, SpiderMechFxLibrary fx, float visualScale)
    {
        var go = new GameObject("BossPlasmaBolt");
        go.transform.SetPositionAndRotation(origin, Quaternion.LookRotation(dir));
        var p = go.AddComponent<BossProjectile>();
        p.Init(dir, speed, damage, radius, owner, fx, visualScale);
        return p;
    }

    private void Init(Vector3 dir, float boltSpeed, float boltDamage, float castRadius, Transform shooter,
        SpiderMechFxLibrary library, float visualScale)
    {
        direction = dir.normalized;
        speed = boltSpeed;
        damage = boltDamage;
        radius = castRadius;
        owner = shooter;
        fx = library;
        lifeLeft = 3.5f;

        if (fx != null) visual = BossFx.Loop(fx.plasmaBolt, transform.position, transform.rotation, visualScale, transform);

        if (trailMaterial == null) trailMaterial = new Material(Shader.Find("Sprites/Default"));
        trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = 0.14f;
        trail.startWidth = 0.22f * visualScale / 0.6f;
        trail.endWidth = 0f;
        trail.sharedMaterial = trailMaterial;
        trail.startColor = new Color(1f, 0.75f, 0.2f, 0.95f);
        trail.endColor = new Color(1f, 0.25f, 0.05f, 0f);
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.numCapVertices = 2;

        var glow = gameObject.AddComponent<Light>();
        glow.type = LightType.Point;
        glow.color = new Color(1f, 0.6f, 0.2f);
        glow.range = 4f;
        glow.intensity = 3f;
        glow.shadows = LightShadows.None;
    }

    private void Update()
    {
        if (finished) return;

        float step = speed * Time.deltaTime;
        if (TryHit(step, out RaycastHit hit))
        {
            transform.position = hit.point;
            Impact(hit);
            return;
        }

        transform.position += direction * step;
        lifeLeft -= Time.deltaTime;
        if (lifeLeft <= 0f) Finish();
    }

    private bool TryHit(float step, out RaycastHit best)
    {
        best = default;
        float nearest = float.MaxValue;
        var hits = Physics.SphereCastAll(transform.position, radius, direction, step, ~0, QueryTriggerInteraction.Ignore);
        foreach (var h in hits)
        {
            if (owner != null && h.collider.transform.IsChildOf(owner)) continue;
            if (h.distance < nearest)
            {
                nearest = h.distance;
                best = h;
            }
        }
        if (nearest < float.MaxValue && best.point == Vector3.zero) best.point = transform.position + direction * best.distance;
        return nearest < float.MaxValue;
    }

    private void Impact(RaycastHit hit)
    {
        var playerHealth = hit.collider.GetComponentInParent<PlayerHealth>();
        Vector3 normal = hit.normal.sqrMagnitude > 0.01f ? hit.normal : -direction;

        if (playerHealth != null && !playerHealth.IsDead)
        {
            playerHealth.ApplyDamage(Mathf.RoundToInt(damage));
            var feedback = playerHealth.GetComponent<PlayerHitFeedback>();
            if (feedback != null && owner != null) feedback.Notify(owner.position);
            if (fx != null)
            {
                BossFx.Play(fx.boltImpactPlayer, hit.point, Quaternion.LookRotation(normal), 0.5f, 0.1f);
                BossFx.Sfx(BossFx.Pick(fx.boltImpacts), hit.point, 0.9f, Random.Range(0.8f, 0.95f), 2f, 25f);
            }
            CameraShake.Shake(hit.point, 0.35f, 6f);
        }
        else if (fx != null)
        {
            BossFx.Play(fx.boltImpactWorld, hit.point + normal * 0.05f, Quaternion.LookRotation(normal), 0.35f, 0.12f);
            BossFx.Sfx(BossFx.Pick(fx.boltImpacts), hit.point, 0.7f, Random.Range(0.85f, 1.1f), 3f, 40f);
        }

        BossFx.Flash(hit.point + normal * 0.3f, new Color(1f, 0.55f, 0.15f), 5f, 5f, 0.15f);
        Finish();
    }

    private void Finish()
    {
        if (finished) return;
        finished = true;

        var glow = GetComponent<Light>();
        if (glow != null) glow.enabled = false;
        if (visual != null) BossFx.Release(visual);
        if (trail != null) trail.emitting = false;
        Destroy(gameObject, trail != null ? trail.time + 0.05f : 0f);
    }
}
