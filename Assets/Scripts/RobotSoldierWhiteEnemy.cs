using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stationary combat behaviour for the Robot_Soldier_White instances in Level 4.
/// The soldier never changes its world position: it idles in place, turns only after
/// it has a clear line of sight to the Player, and fires short, readable bursts.
/// </summary>
public sealed class RobotSoldierWhiteEnemy : MonoBehaviour
{
    [Header("Level 4 Combat")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int damagePerShot = 6;
    [SerializeField, Range(0f, 1f)] private float hitChance = 0.62f;
    [SerializeField] private float sightRange = 500f;
    [SerializeField] private float turnSpeed = 150f;
    [SerializeField] private float aimTime = 0.55f;
    [SerializeField] private int burstShots = 3;
    [SerializeField] private float shotInterval = 0.24f;
    [SerializeField] private float coverPause = 1.5f;
    [SerializeField] private AudioClip[] shotClips;

    private Transform player;
    private PlayerHealth playerHealth;
    private Collider playerCollider;
    private PlayerHitFeedback hitFeedback;
    private AudioSource audioSource;
    private Transform torso;
    private Transform head;
    private Transform rifle;
    private Renderer rifleRenderer;
    private Animator animator;
    private Quaternion torsoRest;
    private Quaternion headRest;
    private Quaternion rifleRest;
    private LineRenderer tracer;
    private Light muzzleLight;
    private Material fxMaterial;
    private int health;
    private bool dead;
    private bool firing;
    private float lastHitTime = -99f;
    private float recoil;

    public bool IsDead => dead;
    public int Health => health;
    public int MaxHealth => maxHealth;
    public float LastHitTime => lastHitTime;
    public Vector3 BarAnchor => transform.position + Vector3.up * 2.15f;

    private void Awake()
    {
        health = maxHealth;

        AcquirePlayer();

        FindRigParts();
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.applyRootMotion = false;
            animator.Play("Idle_Shoot_Ar", 0, Random.Range(0f, 1f));
        }
        AddBoundsCollider();
        BuildFireEffects();
    }

    private void Start() => StartCoroutine(CombatLoop());

    private void Update()
    {
        if (dead) return;

        if (player == null)
            AcquirePlayer();

        bool canSeePlayer = HasClearLineOfSight();
        if (canSeePlayer) TurnTowardPlayer();
        if (animator == null || animator.runtimeAnimatorController == null)
            AnimateInPlace(canSeePlayer);
        else if (!firing && !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Shoot_Ar"))
            animator.CrossFade("Idle_Shoot_Ar", 0.12f);
    }

    private void AcquirePlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) return;

        player = playerObject.transform;
        playerHealth = playerObject.GetComponent<PlayerHealth>();
        playerCollider = playerObject.GetComponentInChildren<Collider>();
        hitFeedback = playerObject.GetComponent<PlayerHitFeedback>();
    }

    private Vector3 GetPlayerAimPoint()
    {
        if (playerCollider != null && playerCollider.enabled)
            return playerCollider.bounds.center;

        return player != null ? player.position + Vector3.up * 0.5f : Vector3.zero;
    }

    private void FindRigParts()
    {
        torso = FindDescendant("Spine2") ?? FindDescendant("Spine1") ?? FindDescendant("Spine");
        head = FindDescendant("Head");
        rifle = FindDescendant("Robot_Soldier_Rifle");
        rifleRenderer = rifle != null ? rifle.GetComponent<Renderer>() : null;

        if (torso != null) torsoRest = torso.localRotation;
        if (head != null) headRest = head.localRotation;
        if (rifle != null) rifleRest = rifle.localRotation;
    }

    private Transform FindDescendant(string partName)
    {
        foreach (Transform t in GetComponentsInChildren<Transform>(true))
            if (t.name == partName) return t;
        return null;
    }

    private void AddBoundsCollider()
    {
        if (GetComponent<Collider>() != null) return;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);

        BoxCollider box = gameObject.AddComponent<BoxCollider>();
        box.center = transform.InverseTransformPoint(bounds.center);
        Vector3 scale = transform.lossyScale;
        box.size = new Vector3(
            bounds.size.x / Mathf.Max(0.001f, Mathf.Abs(scale.x)),
            bounds.size.y / Mathf.Max(0.001f, Mathf.Abs(scale.y)),
            bounds.size.z / Mathf.Max(0.001f, Mathf.Abs(scale.z)));
    }

    private void BuildFireEffects()
    {
        fxMaterial = new Material(Shader.Find("Sprites/Default"));
        tracer = gameObject.AddComponent<LineRenderer>();
        tracer.useWorldSpace = true;
        tracer.positionCount = 2;
        tracer.startWidth = 0.045f;
        tracer.endWidth = 0.018f;
        tracer.sharedMaterial = fxMaterial;
        tracer.startColor = new Color(1f, 0.22f, 0.08f, 0.95f);
        tracer.endColor = new Color(1f, 0.12f, 0.02f, 0.1f);
        tracer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        tracer.enabled = false;

        GameObject muzzle = new GameObject("EnemyMuzzleFlash");
        muzzle.transform.SetParent(transform, false);
        muzzleLight = muzzle.AddComponent<Light>();
        muzzleLight.type = LightType.Point;
        muzzleLight.color = new Color(1f, 0.3f, 0.08f);
        muzzleLight.range = 6f;
        muzzleLight.intensity = 0f;
        muzzleLight.shadows = LightShadows.None;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 5f;
        audioSource.maxDistance = 70f;
        audioSource.volume = 0.9f;
        audioSource.playOnAwake = false;
    }

    private IEnumerator CombatLoop()
    {
        while (!dead)
        {
            if (!CanAttack())
            {
                yield return new WaitForSeconds(0.12f);
                continue;
            }

            yield return new WaitForSeconds(aimTime);
            if (!CanAttack()) continue;

            if (animator != null && animator.runtimeAnimatorController != null)
                animator.CrossFade("Shoot_BurstShot_AR", 0.08f);
            firing = true;

            for (int shot = 0; shot < burstShots; shot++)
            {
                if (!CanAttack()) break;
                FireOnce();
                yield return new WaitForSeconds(shotInterval);
            }

            firing = false;
            if (!dead && animator != null && animator.runtimeAnimatorController != null)
                animator.CrossFade("Idle_Shoot_Ar", 0.12f);

            yield return new WaitForSeconds(coverPause);
        }
    }

    private bool CanAttack()
    {
        return !dead && player != null && (playerHealth == null || !playerHealth.IsDead)
            && IsFacingPlayer() && HasClearLineOfSight();
    }

    private bool IsFacingPlayer()
    {
        Vector3 flatDirection = player.position - transform.position;
        flatDirection.y = 0f;
        return flatDirection.sqrMagnitude > 0.001f
            && Vector3.Angle(transform.forward, flatDirection) <= 10f;
    }

    private void TurnTowardPlayer()
    {
        Vector3 flatDirection = player.position - transform.position;
        flatDirection.y = 0f;
        if (flatDirection.sqrMagnitude < 0.001f) return;

        Quaternion target = Quaternion.LookRotation(flatDirection.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
    }

    private bool HasClearLineOfSight()
    {
        if (player == null) return false;

        Vector3 target = GetPlayerAimPoint();
        Vector3 source = GetMuzzleWorldPosition(target);
        Vector3 direction = target - source;
        float distance = direction.magnitude;
        if (distance > sightRange || distance < 0.01f) return false;

        RaycastHit[] hits = Physics.RaycastAll(source, direction / distance, distance, ~0, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform)) continue;
            return hit.collider.transform.root == player.root;
        }
        // Reaching the aim point without touching another collider also means the path is clear.
        return true;
    }

    private void FireOnce()
    {
        Vector3 target = GetPlayerAimPoint();
        target += Random.insideUnitSphere * 0.14f;
        Vector3 source = GetMuzzleWorldPosition(target);
        recoil = 1f;

        StartCoroutine(ShowShot(source, target));
        if (shotClips != null && shotClips.Length > 0 && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.94f, 1.04f);
            audioSource.PlayOneShot(shotClips[Random.Range(0, shotClips.Length)]);
        }

        if (playerHealth != null && Random.value <= hitChance)
        {
            playerHealth.ApplyDamage(damagePerShot);
            if (hitFeedback != null) hitFeedback.Notify(transform.position);
        }
    }

    private Vector3 GetMuzzleWorldPosition(Vector3 target)
    {
        if (rifleRenderer == null)
            return rifle != null ? rifle.position : BarAnchor;

        // The rifle is a skinned mesh whose Transform stays at the character root.
        // Use the animated rifle bounds so the tracer and flash originate on the gun itself.
        Bounds gunBounds = rifleRenderer.bounds;
        Vector3 towardTarget = target - gunBounds.center;
        if (towardTarget.sqrMagnitude < 0.0001f)
            towardTarget = transform.forward;

        return gunBounds.ClosestPoint(target) + towardTarget.normalized * 0.035f;
    }

    private IEnumerator ShowShot(Vector3 source, Vector3 target)
    {
        muzzleLight.transform.position = source;
        muzzleLight.intensity = 8f;
        tracer.SetPosition(0, source);
        tracer.SetPosition(1, source);
        tracer.enabled = true;

        float travelDuration = Mathf.Clamp(Vector3.Distance(source, target) / 110f, 0.07f, 0.2f);
        float elapsed = 0f;
        while (elapsed < travelDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / travelDuration);
            float tailProgress = Mathf.Max(0f, progress - 0.14f);
            tracer.SetPosition(0, Vector3.Lerp(source, target, tailProgress));
            tracer.SetPosition(1, Vector3.Lerp(source, target, progress));
            muzzleLight.intensity = Mathf.Lerp(8f, 0f, Mathf.Clamp01(progress * 2.5f));
            yield return null;
        }

        if (tracer != null) tracer.enabled = false;
        if (muzzleLight != null) muzzleLight.intensity = 0f;
    }

    private void AnimateInPlace(bool alert)
    {
        // Subtle procedural idle motion prevents static mannequins while preserving the current world position.
        float wave = Mathf.Sin(Time.time * (alert ? 2.8f : 1.55f) + transform.position.x) ;
        recoil = Mathf.MoveTowards(recoil, 0f, Time.deltaTime * 7f);

        if (torso != null)
            torso.localRotation = torsoRest * Quaternion.Euler(wave * 1.1f, alert ? wave * 1.4f : 0f, 0f);
        if (head != null)
            head.localRotation = headRest * Quaternion.Euler(alert ? -2.5f : wave * 1.4f, wave * 2.2f, 0f);
        if (rifle != null)
            rifle.localRotation = rifleRest * Quaternion.Euler(-recoil * 8f, wave * 0.7f, 0f);
    }

    public bool RegisterHit(Vector3 hitPoint)
    {
        if (dead) return false;

        lastHitTime = Time.time;
        health--;
        if (health > 0) return false;

        StartCoroutine(DeathRoutine());
        return true;
    }

    private IEnumerator DeathRoutine()
    {
        dead = true;
        Collider hitCollider = GetComponent<Collider>();
        if (hitCollider != null) hitCollider.enabled = false;
        if (tracer != null) tracer.enabled = false;
        if (muzzleLight != null) muzzleLight.intensity = 0f;

        if (animator != null && animator.runtimeAnimatorController != null)
            animator.CrossFade("Die", 0.08f);

        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (fxMaterial != null) Destroy(fxMaterial);
    }
}
