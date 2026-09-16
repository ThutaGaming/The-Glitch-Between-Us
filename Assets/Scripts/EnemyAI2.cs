using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cover-shooter enemy for CombatEncounterManager2. Waits behind the door, runs (grid A*) to its
/// route's first cover, then loops: hide -> strafe out past the barrier end (right-hand side
/// preferred) -> aim tell -> short burst -> strafe back. After a few peeks it pushes to its
/// route's other cover; when hurt it falls back; when the player flanks its cover it relocates.
/// </summary>
public class EnemyAI2 : MonoBehaviour
{
    private enum State { Waiting, Moving, InCover, Peeking, Dead }

    [SerializeField] private int maxHealth = 2;
    [SerializeField] private float runSpeed = 4.2f;
    [SerializeField] private float strafeSpeed = 2.6f;
    [SerializeField] private float turnSpeed = 540f;
    [SerializeField] private int damagePerHit = 8;
    [SerializeField] private float shotInterval = 0.42f;
    [SerializeField] private int minBurst = 2;
    [SerializeField] private int maxBurst = 4;
    [SerializeField] private float nearHitChance = 0.55f;
    [SerializeField] private float farHitChance = 0.15f;
    [SerializeField] private float nearRange = 8f;
    [SerializeField] private float farRange = 30f;

    private CombatEncounterManager2 manager;
    private Transform player;
    private EnemyRoute route;
    private float laneSign;
    private Animator animator;
    private CapsuleCollider bodyCollider;
    private SphereCollider headCollider;
    private Transform muzzle;
    private Light muzzleLight;
    private Light presenceLight;
    private LineRenderer tracer;
    private AudioSource audioSource;
    private AudioClip[] shotClips;

    private readonly List<Vector3> path = new List<Vector3>();
    private State state = State.Waiting;
    private int health;
    private bool released;
    private bool wounded;
    private bool hitDuringPeek;
    private int currentCover = -1;
    private int burstsSinceReload;
    private int peeksAtCover;
    private int peeksBeforeMove;
    private int blindPeeks;
    private string currentAnim = "";
    private int roamIndex;
    private float flinchUntil;

    public bool IsDead => state == State.Dead;
    public int Health => health;
    public int MaxHealth => maxHealth;
    public float LastHitTime { get; private set; } = -99f;
    public Vector3 HeadCenter => headCollider != null ? headCollider.transform.position : transform.position + Vector3.up * 1.6f;
    public Vector3 HeadPosition => headCollider != null ? headCollider.transform.position + Vector3.up * 0.3f : transform.position + Vector3.up * 2f;

    public void Initialize(CombatEncounterManager2 owner, Transform playerTransform, EnemyRoute enemyRoute, float lane, GameObject gunPrefab, Vector3 gunPos, Vector3 gunEuler, RuntimeAnimatorController controller, AudioClip[] clips, Material fxMaterial, EnemyTuning tuning)
    {
        manager = owner;
        player = playerTransform;
        route = enemyRoute;
        laneSign = lane;
        shotClips = clips;

        if (tuning != null)
        {
            maxHealth = tuning.maxHealth;
            runSpeed = tuning.runSpeed;
            damagePerHit = tuning.damagePerHit;
            shotInterval = tuning.shotInterval;
            minBurst = tuning.minBurst;
            maxBurst = tuning.maxBurst;
            nearHitChance = tuning.nearHitChance;
            farHitChance = tuning.farHitChance;
        }
        health = maxHealth;

        animator = GetComponentInChildren<Animator>(true);
        if (animator != null)
        {
            if (controller != null) animator.runtimeAnimatorController = controller;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }

        var rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        bodyCollider = gameObject.AddComponent<CapsuleCollider>();
        bodyCollider.center = new Vector3(0f, 0.75f, 0f);
        bodyCollider.height = 1.45f;
        bodyCollider.radius = 0.32f;

        Transform hand = animator != null ? animator.GetBoneTransform(HumanBodyBones.RightHand) : null;
        Transform headBone = animator != null ? animator.GetBoneTransform(HumanBodyBones.Head) : null;

        if (headBone != null)
        {
            var headGo = new GameObject("HeadHitbox");
            headGo.transform.SetParent(headBone, false);
            headGo.transform.localPosition = headBone.InverseTransformVector(Vector3.up * 0.08f);
            headCollider = headGo.AddComponent<SphereCollider>();
            headCollider.radius = 0.17f / Mathf.Max(0.001f, headBone.lossyScale.x);
        }

        if (gunPrefab != null && hand != null)
        {
            var gun = Instantiate(gunPrefab, hand);
            gun.transform.localPosition = gunPos;
            gun.transform.localRotation = Quaternion.Euler(gunEuler);
            // Keep the pistol real-size even on a scaled-up character.
            gun.transform.localScale = Vector3.one / Mathf.Max(0.001f, hand.lossyScale.x);
            muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(gun.transform, false);
            muzzle.localPosition = new Vector3(0f, 0.049f, 0.175f);
        }
        else
        {
            muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(transform, false);
            muzzle.localPosition = new Vector3(0.25f, 1.45f, 0.45f);
        }

        muzzleLight = muzzle.gameObject.AddComponent<Light>();
        muzzleLight.type = LightType.Point;
        muzzleLight.color = new Color(1f, 0.8f, 0.4f);
        muzzleLight.range = 5f;
        muzzleLight.intensity = 0f;
        muzzleLight.shadows = LightShadows.None;

        // Always-on fill light carried by the enemy itself, so it reads clearly against dark
        // corners, cover shadows or columns regardless of where it ends up standing in the room.
        var presenceGo = new GameObject("PresenceLight");
        presenceGo.transform.SetParent(transform, false);
        presenceGo.transform.localPosition = new Vector3(0f, 1.55f, 0f);
        presenceLight = presenceGo.AddComponent<Light>();
        presenceLight.type = LightType.Point;
        presenceLight.color = new Color(0.85f, 0.92f, 1f);
        presenceLight.range = 4.5f;
        presenceLight.intensity = 3.5f;
        presenceLight.shadows = LightShadows.None;

        tracer = gameObject.AddComponent<LineRenderer>();
        tracer.useWorldSpace = true;
        tracer.positionCount = 2;
        tracer.startWidth = 0.03f;
        tracer.endWidth = 0.012f;
        tracer.sharedMaterial = fxMaterial;
        tracer.startColor = new Color(1f, 0.9f, 0.55f, 0.95f);
        tracer.endColor = new Color(1f, 0.75f, 0.3f, 0.2f);
        tracer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        tracer.enabled = false;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 3f;
        audioSource.maxDistance = 60f;
        audioSource.volume = 0.8f;
        audioSource.playOnAwake = false;

        PlayAnim("CoverIdle");
        StartCoroutine(Run());
    }

    public void Release() => released = true;

    // ---------- behaviour ----------

    private IEnumerator Run()
    {
        while (!released) yield return null;

        manager.Log(name + " leaving door");

        // If the player is already visible, open the encounter with a burst at any distance.
        // The enemy then follows its cover/patrol route instead of standing in the open.
        yield return FireIfVisible(1.25f);
        if (IsDead) yield break;

        if (route.roam)
        {
            roamIndex = route.roamPoints != null && route.roamPoints.Length > 1 ? 1 : 0;
            PlayAnim("Run");
            while (!IsDead) yield return RoamCycle();
            yield break;
        }

        yield return GoToCover(PickRouteCover(route.firstCover));

        while (!IsDead)
            yield return CoverCycle();
    }

    /// <summary>
    /// Walk a unique loop through the room, pause to scan, and only fire when the player is visible.
    /// The shared attack-token cap prevents all roaming enemies from firing at the same time.
    /// </summary>
    private IEnumerator RoamCycle()
    {
        if (route.roamPoints == null || route.roamPoints.Length == 0)
        {
            PlayAnim("CoverIdle");
            yield return new WaitForSeconds(0.5f);
            yield break;
        }

        if (!manager.TryResolveWalkablePoint(route.roamPoints[roamIndex], out Vector3 target))
        {
            PlayAnim("CoverIdle");
            roamIndex = (roamIndex + 1) % route.roamPoints.Length;
            yield return new WaitForSeconds(0.25f);
            yield break;
        }

        yield return MoveAlongPath(target);
        if (IsDead) yield break;

        state = State.InCover;
        PlayAnim("AimIdle");
        float scanTime = Random.Range(0.4f, 0.85f);
        for (float t = 0f; t < scanTime && !IsDead; t += Time.deltaTime)
        {
            if (!manager.PlayerDead) TurnTowards(player.position - transform.position);
            yield return null;
        }

        bool sawPlayer = !IsDead && !manager.PlayerDead &&
                         manager.HasLineOfSight(muzzle.position, manager.PlayerChest);
        if (sawPlayer)
        {
            yield return FireIfVisible(1.25f);
            if (IsDead) yield break;

            // Roaming routes use real room covers whenever one is available: shoot, hide,
            // then lean out for the next burst before resuming the patrol loop.
            yield return TakeTemporaryCoverAndReturnFire();
            if (IsDead) yield break;
        }

        roamIndex = (roamIndex + 1) % route.roamPoints.Length;
        PlayAnim("CoverIdle");
        yield return new WaitForSeconds(Random.Range(0.3f, 0.7f));
    }

    private int PickRouteCover(int preferred)
    {
        if (manager.IsCoverUsable(preferred, this)) return preferred;
        return manager.FindBestCover(this, transform.position, route.firstCover, route.pushCover, currentCover, laneSign);
    }

    private IEnumerator GoToCover(int index)
    {
        if (index < 0)
        {
            yield return OpenFire();
            manager.ReleaseMoveToken(this);
            yield break;
        }

        manager.Log(name + " cover " + currentCover + " -> " + index + " (peeks " + peeksAtCover + "/" + peeksBeforeMove + ", wounded " + wounded + ", dist to player " + CombatEncounterManager2.FlatDistance(transform.position, player.position).ToString("F1") + ")");
        manager.ReleaseCover(currentCover, this);
        currentCover = index;
        manager.Claim(index, this);

        Vector3 hide = manager.HideSpot(index, player.position, out _, out _);
        yield return MoveAlongPath(hide);

        peeksAtCover = 0;
        peeksBeforeMove = Random.Range(2, 4);
        manager.ReleaseMoveToken(this);
    }

    private IEnumerator CoverCycle()
    {
        state = State.InCover;

        // No cover claimed - the room ran out of usable ones (an isolated catwalk, or the player
        // standing on top of every hide spot). Hold the ground and keep shooting rather than
        // indexing coverPoints with -1, and retry for a slot each pass.
        if (currentCover < 0)
        {
            yield return OpenFire();
            yield return Relocate(false);
            yield break;
        }

        Vector3 hide = manager.HideSpot(currentCover, player.position, out Vector3 facing, out _);

        if (!manager.IsSpotProtected(hide) || CombatEncounterManager2.FlatDistance(hide, player.position) < 5f)
        {
            yield return Relocate(true);
            yield break;
        }

        if (CombatEncounterManager2.FlatDistance(transform.position, hide) > 0.6f)
            yield return MoveAlongPath(hide);

        PlayAnim("CoverIdle");
        float hold = Random.Range(0.8f, 1.6f) + (wounded ? 1.2f : 0f);
        for (float t = 0f; t < hold; t += Time.deltaTime)
        {
            if (IsDead) yield break;
            TurnTowards(facing);
            if (manager.PlayerDead) t = 0f;
            yield return null;
        }

        if (!manager.IsSpotProtected(transform.position))
        {
            yield return Relocate(true);
            yield break;
        }

        if (burstsSinceReload >= 2)
        {
            PlayAnim("Reload");
            yield return new WaitForSeconds(2.6f);
            burstsSinceReload = 0;
            yield break;
        }

        if (wounded && currentCover != route.firstCover && manager.TryAcquireMoveToken(this))
        {
            wounded = false;
            yield return GoToCover(PickRouteCover(route.firstCover));
            yield break;
        }
        wounded = false;

        if (peeksAtCover >= peeksBeforeMove && manager.TryAcquireMoveToken(this))
        {
            int next = currentCover == route.pushCover ? route.firstCover : route.pushCover;
            yield return GoToCover(PickRouteCover(next));
            yield break;
        }

        if (manager.TryAcquireAttackToken(this))
        {
            yield return Peek();
            manager.ReleaseAttackToken(this);
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator Relocate(bool urgent)
    {
        if (!urgent && !manager.TryAcquireMoveToken(this)) yield break;
        int best = manager.FindBestCover(this, transform.position, route.firstCover, route.pushCover, currentCover, laneSign);
        yield return GoToCover(best);
    }

    private IEnumerator Peek()
    {
        state = State.Peeking;
        hitDuringPeek = false;
        Vector3 threat = player.position;
        manager.HideSpot(currentCover, threat, out _, out int rightEnd);

        Vector3 peek = Vector3.zero;
        bool found = false;
        foreach (int end in new[] { rightEnd, -rightEnd })
        {
            Vector3 p = manager.PeekSpot(currentCover, threat, end, end == rightEnd);
            if (manager.Grid.IsWalkable(p) && manager.HasLineOfSight(p + Vector3.up * 1.5f, manager.PlayerChest))
            {
                peek = p;
                found = true;
                break;
            }
        }

        peeksAtCover++;
        if (!found)
        {
            // Stuck behind a barrier with no angle on the player: after a couple of tries, move to
            // cover that actually has a firing line instead of idling out of the fight.
            blindPeeks++;
            if (blindPeeks >= 2 && manager.TryAcquireMoveToken(this))
            {
                blindPeeks = 0;
                int better = manager.FindBestCover(this, transform.position, route.firstCover, route.pushCover, currentCover, laneSign, true);
                if (better >= 0)
                {
                    manager.ReleaseAttackToken(this);
                    yield return GoToCover(better);
                    yield break;
                }
                manager.ReleaseMoveToken(this);
            }
            yield return new WaitForSeconds(0.4f);
            yield break;
        }
        blindPeeks = 0;

        Vector3 back = transform.position;
        yield return Strafe(peek);
        if (IsDead) yield break;

        yield return AimAndBurst();
        if (IsDead) yield break;

        yield return Strafe(back);
    }

    private IEnumerator AimAndBurst()
    {
        PlayAnim("AimIdle");
        float react = Random.Range(0.4f, 0.6f);
        for (float t = 0f; t < react; t += Time.deltaTime)
        {
            if (IsDead || hitDuringPeek) yield break;
            TurnTowards(player.position - transform.position);
            yield return null;
        }

        int shots = Random.Range(minBurst, maxBurst + 1);
        for (int i = 0; i < shots; i++)
        {
            if (IsDead || hitDuringPeek || manager.PlayerDead) break;
            SnapFace(player.position);
            if (!manager.HasLineOfSight(muzzle.position, manager.PlayerChest)) break;
            PlayAnim("Shoot", true);
            FireShot();
            yield return new WaitForSeconds(shotInterval);
        }
        burstsSinceReload++;
    }

    private IEnumerator OpenFire()
    {
        yield return FireIfVisible(0.8f);
        PlayAnim("AimIdle");
        yield return new WaitForSeconds(0.8f);
    }

    /// <summary>
    /// Fires as soon as the player is visible, regardless of distance. A short token wait keeps
    /// the squad staggered instead of making every enemy fire on the exact same frame.
    /// </summary>
    private IEnumerator FireIfVisible(float tokenWait)
    {
        if (IsDead || manager.PlayerDead ||
            !manager.HasLineOfSight(muzzle.position, manager.PlayerChest)) yield break;

        float deadline = Time.time + tokenWait;
        while (!IsDead && !manager.PlayerDead && Time.time <= deadline)
        {
            if (!manager.HasLineOfSight(muzzle.position, manager.PlayerChest)) yield break;
            if (manager.TryAcquireAttackToken(this))
            {
                state = State.Peeking;
                hitDuringPeek = false;
                yield return AimAndBurst();
                manager.ReleaseAttackToken(this);
                if (!IsDead) state = State.InCover;
                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Lets a roaming enemy borrow an unclaimed cover after firing, then peek out and return fire.
    /// Encounters with no registered cover keep using their obstacle-aware patrol route.
    /// </summary>
    private IEnumerator TakeTemporaryCoverAndReturnFire()
    {
        if (!manager.TryAcquireMoveToken(this)) yield break;

        int cover = manager.FindBestCover(this, transform.position, -1, -1, currentCover, laneSign, true);
        if (cover < 0)
        {
            manager.ReleaseMoveToken(this);
            yield break;
        }

        yield return GoToCover(cover);
        if (IsDead) yield break;

        PlayAnim("CoverIdle");
        float hideTime = Random.Range(0.7f, 1.2f);
        for (float t = 0f; t < hideTime && !IsDead; t += Time.deltaTime)
        {
            TurnTowards(player.position - transform.position);
            yield return null;
        }

        if (!IsDead && !manager.PlayerDead && manager.TryAcquireAttackToken(this))
        {
            yield return Peek();
            manager.ReleaseAttackToken(this);
        }

        manager.ReleaseCover(currentCover, this);
        currentCover = -1;
    }

    // ---------- movement ----------

    private IEnumerator MoveAlongPath(Vector3 target)
    {
        state = State.Moving;
        if (manager.Grid == null ||
            !manager.TryResolveWalkablePoint(target, out Vector3 walkableTarget) ||
            !manager.Grid.FindPath(transform.position, walkableTarget, path))
        {
            path.Clear();
            PlayAnim("CoverIdle");
            yield return new WaitForSeconds(0.2f);
            yield break;
        }

        PlayAnim("Run");
        for (int i = 0; i < path.Count; i++)
        {
            Vector3 wp = path[i];
            while (!IsDead && CombatEncounterManager2.FlatDistance(transform.position, wp) > 0.1f)
            {
                if (CombatEncounterManager2.FlatDistance(transform.position, player.position) < 2.3f && !manager.PlayerDead)
                {
                    yield return OpenFire();
                    PlayAnim("Run");
                }

                Vector3 next = Vector3.MoveTowards(Flat(transform.position), Flat(wp), runSpeed * Time.deltaTime);
                Vector3 dir = next - Flat(transform.position);
                transform.position = new Vector3(next.x, manager.FloorY, next.z);
                if (dir.sqrMagnitude > 0.000001f) TurnTowards(dir);
                yield return null;
            }
            if (IsDead) yield break;
        }
    }

    private IEnumerator Strafe(Vector3 target)
    {
        while (!IsDead && CombatEncounterManager2.FlatDistance(transform.position, target) > 0.05f)
        {
            if (hitDuringPeek && state == State.Peeking) break;
            Vector3 next = Vector3.MoveTowards(Flat(transform.position), Flat(target), strafeSpeed * Time.deltaTime);
            Vector3 dir = next - Flat(transform.position);
            transform.position = new Vector3(next.x, manager.FloorY, next.z);
            TurnTowards(player.position - transform.position);
            PlayAnim(Vector3.Dot(dir, transform.right) >= 0f ? "StrafeRight" : "StrafeLeft");
            yield return null;
        }
    }

    private void TurnTowards(Vector3 dir)
    {
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
    }

    private void SnapFace(Vector3 point)
    {
        Vector3 dir = point - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f) transform.rotation = Quaternion.LookRotation(dir);
    }

    private static Vector3 Flat(Vector3 v) => new Vector3(v.x, 0f, v.z);

    private void PlayAnim(string stateName, bool restart = false)
    {
        if (animator == null) return;
        if (!restart && currentAnim == stateName) return;
        currentAnim = stateName;
        if (Time.time < flinchUntil) return;
        if (restart) animator.Play(stateName, 0, 0f);
        else animator.CrossFadeInFixedTime(stateName, 0.12f);
    }

    // ---------- combat ----------

    private void FireShot()
    {
        StartCoroutine(MuzzleFlash());
        if (shotClips != null && shotClips.Length > 0)
        {
            audioSource.pitch = Random.Range(1.1f, 1.25f);
            audioSource.PlayOneShot(shotClips[Random.Range(0, shotClips.Length)]);
        }

        Vector3 origin = muzzle.position;
        Vector3 chest = manager.PlayerChest;
        float dist = Vector3.Distance(origin, chest);
        float chance = Mathf.Lerp(nearHitChance, farHitChance, Mathf.InverseLerp(nearRange, farRange, dist));
        if (manager.PlayerSpeed > 3.5f) chance *= 0.75f;

        Vector3 aim = chest + Random.insideUnitSphere * 0.1f;
        if (Random.value > chance)
        {
            Vector3 side = Vector3.Cross(Vector3.up, (chest - origin).normalized);
            aim = chest + side * (Random.value < 0.5f ? -1f : 1f) * Random.Range(0.6f, 1.1f) + Vector3.up * Random.Range(-0.5f, 0.7f);
        }

        Vector3 dir = (aim - origin).normalized;
        float shotRange = Mathf.Max(500f, dist + 10f);
        Vector3 end = origin + dir * shotRange;
        var hits = Physics.RaycastAll(origin, dir, shotRange, ~0, QueryTriggerInteraction.Ignore);
        float nearest = float.MaxValue;
        RaycastHit best = default;
        foreach (var h in hits)
        {
            if (h.collider.transform.IsChildOf(transform)) continue;
            if (h.distance < nearest)
            {
                nearest = h.distance;
                best = h;
            }
        }

        if (nearest < float.MaxValue)
        {
            end = best.point;
            if (manager.IsPlayerCollider(best.collider)) manager.DamagePlayer(damagePerHit, transform.position);
            else if (best.collider.GetComponentInParent<EnemyAI2>() == null) manager.SpawnImpact(best.point);
        }

        StartCoroutine(ShowTracer(origin, end));
    }

    private IEnumerator MuzzleFlash()
    {
        muzzleLight.intensity = 8f;
        yield return new WaitForSeconds(0.05f);
        if (muzzleLight != null) muzzleLight.intensity = 0f;
    }

    private IEnumerator ShowTracer(Vector3 from, Vector3 to)
    {
        tracer.SetPosition(0, from);
        tracer.SetPosition(1, to);
        tracer.enabled = true;
        yield return new WaitForSeconds(0.06f);
        if (tracer != null) tracer.enabled = false;
    }

    /// <summary>Returns true if this hit killed the enemy.</summary>
    public bool RegisterHit(bool head)
    {
        if (IsDead) return false;
        LastHitTime = Time.time;
        health -= head ? maxHealth : 1;
        if (health <= 0)
        {
            Die();
            return true;
        }

        wounded = true;
        hitDuringPeek = state == State.Peeking;
        if (animator != null)
        {
            flinchUntil = Time.time + 0.35f;
            animator.CrossFadeInFixedTime("Hit", 0.05f);
            StartCoroutine(EndFlinch());
        }
        return false;
    }

    private IEnumerator EndFlinch()
    {
        yield return new WaitForSeconds(0.36f);
        if (!IsDead && animator != null && currentAnim != "") animator.CrossFadeInFixedTime(currentAnim, 0.12f);
    }

    private void Die()
    {
        state = State.Dead;
        StopAllCoroutines();
        manager.ReleaseCover(currentCover, this);
        if (bodyCollider != null) bodyCollider.enabled = false;
        if (headCollider != null) headCollider.enabled = false;
        if (muzzleLight != null) muzzleLight.intensity = 0f;
        if (tracer != null) tracer.enabled = false;
        if (animator != null) animator.CrossFadeInFixedTime("Death1", 0.08f);
        manager.NotifyDeath(this);
    }
}
