using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Level 4 blue soldiers advance between assigned Cover props, peek to fire, then retreat.
/// A walkability grid keeps their routes on the floor and clear of the room's colliders.
/// </summary>
public sealed class RobotSoldierBlueEnemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 8;
    [SerializeField] private int damagePerShot = 7;
    [SerializeField, Range(0f, 1f)] private float hitChance = 0.62f;
    [SerializeField] private float runSpeed = 3.4f;
    [SerializeField] private float sightRange = 65f;
    [SerializeField] private float activationRange = 36f;

    private static CoverNavGrid navGrid;
    private static int navSceneHandle;

    private Transform player;
    private PlayerHealth playerHealth;
    private Collider playerCollider;
    private Animator animator;
    private Transform muzzle;
    private LineRenderer tracer;
    private Light muzzleFlash;
    private Material shotMaterial;
    private Collider bodyCollider;
    private Transform[] covers;
    private Collider[] coverColliders;
    private readonly List<Vector3> path = new List<Vector3>();
    private int health;
    private bool dead;
    private float lastHitTime = -99f;

    public bool IsDead => dead;
    public int Health => health;
    public int MaxHealth => maxHealth;
    public float LastHitTime => lastHitTime;
    public Vector3 BarAnchor => transform.position + Vector3.up * 2.18f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetNavigation()
    {
        navGrid = null;
        navSceneHandle = 0;
    }

    private void Awake()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
        if (animator != null) animator.applyRootMotion = false;
        AcquirePlayer();
        AddHitCollider();
        BuildMuzzleAndProjectile();
    }

    private void Start()
    {
        if (!SceneManager.GetActiveScene().path.EndsWith("Level 4.unity")) return;

        ResolveRoute();
        if (covers == null || covers.Length != 2 || covers[0] == null || covers[1] == null)
        {
            Debug.LogWarning(name + ": Cover route could not be resolved.", this);
            return;
        }

        EnsureNavigationGrid();
        PlayAnimation("Idle");
        StartCoroutine(CombatRoute());
    }

    private void AcquirePlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) return;
        player = playerObject.transform;
        playerHealth = playerObject.GetComponent<PlayerHealth>();
        playerCollider = playerObject.GetComponentInChildren<Collider>();
    }

    private Vector3 PlayerAimPoint => playerCollider != null && playerCollider.enabled
        ? playerCollider.bounds.center
        : player != null ? player.position + Vector3.up * 1f : Vector3.zero;

    private void AddHitCollider()
    {
        bodyCollider = GetComponent<Collider>();
        if (bodyCollider != null) return;

        // The body, arms and rifle are skinned meshes without hit colliders in this prefab.
        // This box covers the complete silhouette without changing the imported meshes.
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
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
        bodyCollider = box;
    }

    private void BuildMuzzleAndProjectile()
    {
        // Measured from the low end of Robot_Soldier_Rifle's baked skinned mesh. Since the
        // rifle is weighted to RightHand, this child follows its barrel during Run_Aim/Shoot.
        Transform hand = animator != null && animator.isHuman
            ? animator.GetBoneTransform(HumanBodyBones.RightHand) : null;
        GameObject muzzleObject = new GameObject("BlueSoldierRifleMuzzle");
        muzzle = muzzleObject.transform;
        muzzle.SetParent(hand != null ? hand : transform, false);
        muzzle.localPosition = hand != null
            ? new Vector3(0.79f, -0.04f, 0.24f)
            : new Vector3(1.32f, 0.5f, 0.21f);

        GameObject projectileObject = new GameObject("BlueSoldierBullet");
        projectileObject.transform.SetParent(transform, false);
        tracer = projectileObject.AddComponent<LineRenderer>();
        shotMaterial = new Material(Shader.Find("Sprites/Default"));
        tracer.sharedMaterial = shotMaterial;
        tracer.useWorldSpace = true;
        tracer.positionCount = 2;
        tracer.startWidth = 0.055f;
        tracer.endWidth = 0.02f;
        tracer.startColor = new Color(0.14f, 0.85f, 1f, 1f);
        tracer.endColor = new Color(0.08f, 0.37f, 1f, 0.15f);
        tracer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        tracer.enabled = false;

        muzzleFlash = muzzleObject.AddComponent<Light>();
        muzzleFlash.type = LightType.Point;
        muzzleFlash.color = new Color(0.12f, 0.75f, 1f);
        muzzleFlash.range = 4.5f;
        muzzleFlash.intensity = 0f;
        muzzleFlash.shadows = LightShadows.None;
    }

    private void ResolveRoute()
    {
        Vector2 first;
        Vector2 second;
        switch (name)
        {
            case "Robot_Soldier_Blue_1":
                first = new Vector2(101.46f, -1.86f);
                second = new Vector2(99.69f, 6.78f);
                break;
            case "Robot_Soldier_Blue_2":
                first = new Vector2(99.01f, 0.94f);
                second = new Vector2(98.16f, 10.99f);
                break;
            case "Robot_Soldier_Blue_3":
                first = new Vector2(93.85f, 0.27f);
                second = new Vector2(94.87f, 8.06f);
                break;
            default:
                first = new Vector2(92.04f, -4.02f);
                second = new Vector2(91.25f, 4.95f);
                break;
        }

        Transform[] markers = Object.FindObjectsByType<Transform>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        covers = new[] { FindCoverMarker(markers, first), FindCoverMarker(markers, second) };
        coverColliders = new[] { FindBarrier(covers[0]), FindBarrier(covers[1]) };
    }

    private static Transform FindCoverMarker(Transform[] all, Vector2 approximate)
    {
        Transform best = null;
        float bestDistance = 1.5f * 1.5f;
        foreach (Transform candidate in all)
        {
            if (candidate.name != "Cover") continue;
            Vector2 at = new Vector2(candidate.position.x, candidate.position.z);
            float distance = (at - approximate).sqrMagnitude;
            if (distance >= bestDistance) continue;
            bestDistance = distance;
            best = candidate;
        }
        return best;
    }

    private static Collider FindBarrier(Transform marker)
    {
        if (marker == null) return null;
        Collider best = null;
        float bestDistance = float.MaxValue;
        foreach (Collider candidate in Physics.OverlapSphere(
                     marker.position, 2.5f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (candidate.name != "Cover" && candidate.name != "Wall_Cube_01_Long"
                && candidate.name != "cover1") continue;
            float distance = (candidate.bounds.center - marker.position).sqrMagnitude;
            if (distance >= bestDistance) continue;
            best = candidate;
            bestDistance = distance;
        }
        return best;
    }

    private void EnsureNavigationGrid()
    {
        int handle = gameObject.scene.handle;
        if (navGrid != null && navSceneHandle == handle) return;

        Vector3 min = transform.position;
        Vector3 max = transform.position;
        foreach (Transform other in Object.FindObjectsByType<Transform>(
                     FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (other.name != "Cover" && !other.name.StartsWith("Robot_Soldier_Blue")) continue;
            if (Mathf.Abs(other.position.y - transform.position.y) > 2f) continue;
            min = Vector3.Min(min, other.position);
            max = Vector3.Max(max, other.position);
        }
        min -= new Vector3(4f, 0f, 4f);
        max += new Vector3(4f, 0f, 4f);
        navGrid = new CoverNavGrid(min, max, transform.position.y, 0.5f,
            0.34f, 1.8f, ~0, IgnoreForNavigation);
        navSceneHandle = handle;
    }

    private static bool IgnoreForNavigation(Collider obstacle)
    {
        return obstacle.transform.root.CompareTag("Player")
            || obstacle.GetComponentInParent<RobotSoldierBlueEnemy>() != null
            || obstacle.GetComponentInParent<RobotSoldierWhiteEnemy>() != null
            || obstacle.GetComponentInParent<BlueRobotEnemy>() != null;
    }

    private bool PlayerInEncounter()
    {
        if (player == null) AcquirePlayer();
        if (player == null || (playerHealth != null && playerHealth.IsDead)) return false;
        if (Mathf.Abs(player.position.y - transform.position.y) > 2.8f) return false;
        Vector3 delta = player.position - transform.position;
        delta.y = 0f;
        return delta.sqrMagnitude <= activationRange * activationRange;
    }

    private IEnumerator CombatRoute()
    {
        while (!dead && !PlayerInEncounter()) yield return new WaitForSeconds(0.2f);
        int coverIndex = 0;
        while (!dead)
        {
            if (!PlayerInEncounter())
            {
                PlayAnimation("Idle");
                yield return new WaitForSeconds(0.25f);
                continue;
            }

            Vector3 hide = GetHideSpot(coverIndex);
            yield return MoveTo(hide);
            if (dead) yield break;

            PlayAnimation("Idle");
            FacePlayer();
            yield return new WaitForSeconds(Random.Range(0.55f, 1.05f));

            if (TryGetPeekSpot(coverIndex, out Vector3 peek))
            {
                yield return MoveTo(peek);
                if (dead) yield break;
                FacePlayer();
                PlayAnimation("Idle");
                yield return new WaitForSeconds(Random.Range(0.4f, 0.7f));

                if (HasClearShot(PlayerAimPoint))
                {
                    PlayAnimation("Shoot");
                    int rounds = Random.Range(3, 6);
                    for (int i = 0; i < rounds && !dead; i++)
                    {
                        FacePlayer();
                        if (!HasClearShot(PlayerAimPoint)) break;
                        FireOnce();
                        yield return new WaitForSeconds(0.2f);
                    }
                }

                yield return MoveTo(GetHideSpot(coverIndex));
            }

            PlayAnimation("Idle");
            yield return new WaitForSeconds(Random.Range(1f, 1.8f));
            coverIndex = 1 - coverIndex;
        }
    }

    private Vector3 GetHideSpot(int index)
    {
        Collider barrier = coverColliders[index];
        if (barrier == null) return covers[index].position;

        Bounds b = barrier.bounds;
        Vector3 away = b.center - (player != null ? player.position : transform.position - Vector3.right);
        away.y = 0f;
        if (away.sqrMagnitude < 0.01f) away = Vector3.right;
        away.Normalize();
        float toEdge = Mathf.Min(
            Mathf.Abs(away.x) > 0.01f ? b.extents.x / Mathf.Abs(away.x) : float.MaxValue,
            Mathf.Abs(away.z) > 0.01f ? b.extents.z / Mathf.Abs(away.z) : float.MaxValue);
        Vector3 desired = b.center + away * (toEdge + 0.7f);
        desired.y = navGrid.FloorY;
        return navGrid.TryGetNearestWalkable(desired, 6, out Vector3 walkable)
            ? walkable : desired;
    }

    private bool TryGetPeekSpot(int index, out Vector3 peek)
    {
        peek = transform.position;
        Collider barrier = coverColliders[index];
        if (barrier == null || player == null) return false;

        Bounds b = barrier.bounds;
        float[] xs = { b.min.x - 0.85f, b.max.x + 0.85f };
        float[] zs = { b.min.z - 0.85f, b.max.z + 0.85f };
        float bestDistance = float.MaxValue;
        for (int xi = 0; xi < 2; xi++)
        for (int zi = 0; zi < 2; zi++)
        {
            Vector3 desired = new Vector3(xs[xi], navGrid.FloorY, zs[zi]);
            if (!navGrid.TryGetNearestWalkable(desired, 3, out Vector3 candidate)) continue;
            if ((candidate - desired).sqrMagnitude > 0.8f * 0.8f) continue;
            if (!navGrid.FindPath(transform.position, candidate, path)) continue;
            if (!HasClearShotFrom(candidate + Vector3.up * 0.55f, PlayerAimPoint)) continue;
            float distance = (candidate - transform.position).sqrMagnitude;
            if (distance >= bestDistance) continue;
            peek = candidate;
            bestDistance = distance;
        }
        return bestDistance < float.MaxValue;
    }

    private IEnumerator MoveTo(Vector3 destination)
    {
        if (navGrid == null || !navGrid.FindPath(transform.position, destination, path))
        {
            PlayAnimation("Idle");
            yield break;
        }

        // Copy the path: other queries can reuse the component's scratch list.
        Vector3[] waypoints = path.ToArray();
        PlayAnimation("Run_Aim");
        foreach (Vector3 waypoint in waypoints)
        {
            float elapsed = 0f;
            while (!dead && elapsed < 10f)
            {
                Vector3 target = new Vector3(waypoint.x, transform.position.y, waypoint.z);
                Vector3 delta = target - transform.position;
                delta.y = 0f;
                if (delta.sqrMagnitude < 0.1f * 0.1f) break;
                Vector3 direction = delta.normalized;
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(direction, Vector3.up), 480f * Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, target,
                    runSpeed * Time.deltaTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (dead) yield break;
        }
        PlayAnimation("Idle");
    }

    private void FacePlayer()
    {
        if (player == null) return;
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
    }

    private bool HasClearShot(Vector3 target)
    {
        return PlayerInEncounter() && HasClearShotFrom(muzzle.position, target);
    }

    private bool HasClearShotFrom(Vector3 source, Vector3 target)
    {
        Vector3 direction = target - source;
        float distance = direction.magnitude;
        if (distance < 0.01f || distance > sightRange) return false;
        RaycastHit[] hits = Physics.RaycastAll(source, direction / distance,
            distance + 0.1f, ~0, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform)) continue;
            return player != null && hit.collider.transform.root == player.root;
        }
        return false;
    }

    private void FireOnce()
    {
        Vector3 start = muzzle.position;
        Vector3 aim = PlayerAimPoint;
        bool accurate = Random.value <= hitChance;
        if (!accurate)
        {
            Vector3 sideways = Vector3.Cross((aim - start).normalized, Vector3.up).normalized;
            aim += sideways * Random.Range(0.75f, 1.3f) * (Random.value < 0.5f ? -1f : 1f);
        }

        Vector3 direction = (aim - start).normalized;
        float distance = Vector3.Distance(start, aim);
        Vector3 impact = aim;
        bool playerHit = false;
        RaycastHit[] hits = Physics.RaycastAll(start, direction, distance + 0.2f,
            ~0, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.IsChildOf(transform)) continue;
            impact = hit.point;
            playerHit = accurate && hit.collider.transform.root == player.root;
            break;
        }
        StartCoroutine(ShowBullet(start, impact, playerHit));
    }

    private IEnumerator ShowBullet(Vector3 start, Vector3 end, bool playerHit)
    {
        muzzleFlash.intensity = 8f;
        tracer.SetPosition(0, start);
        tracer.SetPosition(1, start);
        tracer.enabled = true;
        float duration = Mathf.Clamp(Vector3.Distance(start, end) / 100f, 0.06f, 0.28f);
        float elapsed = 0f;
        while (elapsed < duration && !dead)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            tracer.SetPosition(0, Vector3.Lerp(start, end, Mathf.Max(0f, progress - 0.18f)));
            tracer.SetPosition(1, Vector3.Lerp(start, end, progress));
            muzzleFlash.intensity = Mathf.Lerp(8f, 0f, Mathf.Clamp01(progress * 3f));
            yield return null;
        }
        tracer.enabled = false;
        muzzleFlash.intensity = 0f;
        if (!dead && playerHit && playerHealth != null && !playerHealth.IsDead)
            playerHealth.ApplyDamage(damagePerShot);
    }

    private void PlayAnimation(string state)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(state)) return;
        animator.CrossFade(state, 0.1f);
    }

    public bool RegisterHit(Vector3 hitPoint)
    {
        if (dead) return false;
        lastHitTime = Time.time;
        health--;
        if (health > 0) return false;
        StartCoroutine(Die());
        return true;
    }

    private IEnumerator Die()
    {
        dead = true;
        if (bodyCollider != null) bodyCollider.enabled = false;
        if (tracer != null) tracer.enabled = false;
        if (muzzleFlash != null) muzzleFlash.intensity = 0f;
        PlayAnimation("Die");
        yield return new WaitForSeconds(2.1f);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (shotMaterial != null) Destroy(shotMaterial);
    }
}
