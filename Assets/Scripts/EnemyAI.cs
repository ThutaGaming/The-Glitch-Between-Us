using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Per-enemy squad-combat state machine for Room 1's encounter (Spawn -> FindCover -> Move ->
/// Aim -> BurstFire -> ChangeCover -> repeat), driven entirely by direct-line movement (this
/// project has no baked NavMesh) and by calling Animator.Play() directly on the reused
/// SciFiWarrior controller's existing states (that controller ships with zero parameters/
/// transitions, so Play() is the only way to switch states without editing the shared asset).
/// Reused across all 5 enemies via <see cref="behaviorType"/>, which only changes which cover
/// points get picked and how often - the state machine itself is identical for every enemy.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    public enum BehaviorType { HoldAndBurst, Reposition, Flank }
    private enum State { Spawning, MovingToCover, Aiming, Firing, Cooldown, Dead }

    [SerializeField] private BehaviorType behaviorType = BehaviorType.HoldAndBurst;
    [SerializeField] private float moveSpeed = 3.2f;
    [SerializeField] private int hitsToKill = 3;
    [SerializeField] private float baseAccuracy = 0.72f;
    [SerializeField] private float movingPlayerAccuracyMultiplier = 0.55f;
    [SerializeField] private float playerMovingSpeedThreshold = 1.0f;
    [Tooltip("Enemies never settle into a firing position closer than this to the player - they engage from range, not point-blank.")]
    [SerializeField] private float minEngageRange = 7f;

    [Tooltip("Hard leash so a wiring mistake or a chased player can never walk this enemy into another room.")]
    [SerializeField] private Vector2 roomBoundsX = new Vector2(-24f, 24f);
    [SerializeField] private Vector2 roomBoundsZ = new Vector2(1f, 39f);

    private CombatEncounterManager manager;
    private Transform player;
    private Rigidbody playerRigidbody;
    private List<Transform> coverPool;
    private int coverCycleIndex;

    private Animator animator;
    private Collider hitCollider;
    private Light muzzleLight;
    private State state = State.Spawning;
    private int hitsTaken;
    private bool holdingFireSlot;

    public bool IsDead => state == State.Dead;

    public void Initialize(CombatEncounterManager owner, Transform playerTransform, List<Transform> assignedCover, BehaviorType behavior, Vector3 spawnPoint)
    {
        manager = owner;
        player = playerTransform;
        playerRigidbody = player.GetComponent<Rigidbody>();
        coverPool = assignedCover;
        behaviorType = behavior;

        animator = GetComponentInChildren<Animator>(true);

        hitCollider = GetComponent<CapsuleCollider>();
        if (hitCollider == null)
        {
            // Measured against the actual HP/PBR/Polyart mesh: feet sit ~0.23 below the root
            // and the head reaches ~2.42 above it (~2.65 total) - a 1.8-tall capsule left the
            // whole head and much of the torso outside the hitbox, so long-range shots aimed at
            // the visible upper body sailed clean over it and never registered.
            var capsule = gameObject.AddComponent<CapsuleCollider>();
            capsule.center = new Vector3(0f, 1.1f, 0f);
            capsule.height = 2.6f;
            capsule.radius = 0.4f;
            hitCollider = capsule;
        }

        var muzzleGo = new GameObject("MuzzleFlash");
        muzzleGo.transform.SetParent(transform);
        muzzleGo.transform.localPosition = new Vector3(0f, 1.35f, 0.3f);
        muzzleLight = muzzleGo.AddComponent<Light>();
        muzzleLight.type = LightType.Point;
        muzzleLight.color = new Color(1f, 0.82f, 0.4f);
        muzzleLight.range = 4f;
        muzzleLight.intensity = 0f;
        muzzleLight.shadows = LightShadows.None;

        transform.position = spawnPoint;
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        PlayAnim("Run_guard_AR");
        yield return MoveTo(ResolveEngagePosition(PickCoverTransform()));

        while (state != State.Dead)
        {
            // Don't fire blind through the barrier - if this spot has lost sight of the player,
            // find a new one instead of standing there uselessly (or shooting through cover).
            if (!HasLineOfSight(transform.position))
            {
                yield return new WaitForSeconds(0.3f);
                if (state == State.Dead) yield break;

                state = State.MovingToCover;
                PlayAnim("Run_guard_AR");
                yield return MoveTo(ResolveEngagePosition(PickCoverTransform()));
                continue;
            }

            state = State.Aiming;
            SnapFaceTarget(player.position);
            PlayAnim("Idle_Shoot_Ar");
            yield return new WaitForSeconds(Random.Range(0.4f, 0.7f));
            if (state == State.Dead) yield break;

            // Squad fire-rate cap: hold aim until the manager has a shooter slot free.
            while (!manager.TryAcquireShooterSlot(this))
            {
                if (state == State.Dead) yield break;
                yield return null;
            }
            holdingFireSlot = true;

            state = State.Firing;
            int rounds = Random.Range(3, 6);
            PlayAnim("Shoot_BurstShot_AR");
            for (int i = 0; i < rounds; i++)
            {
                if (state == State.Dead) break;
                SnapFaceTarget(player.position);
                FireRound();
                yield return new WaitForSeconds(0.12f);
            }

            manager.ReleaseShooterSlot(this);
            holdingFireSlot = false;
            if (state == State.Dead) yield break;

            state = State.Cooldown;
            PlayAnim("Idle_Ducking_AR");
            yield return new WaitForSeconds(Random.Range(1f, 2f));
            if (state == State.Dead) yield break;

            state = State.MovingToCover;
            PlayAnim("Run_guard_AR");
            yield return MoveTo(ResolveEngagePosition(PickCoverTransform()));
        }
    }

    private Transform PickCoverTransform()
    {
        if (coverPool == null || coverPool.Count == 0) return null;

        switch (behaviorType)
        {
            case BehaviorType.HoldAndBurst:
                return coverPool[0];

            case BehaviorType.Reposition:
                coverCycleIndex = (coverCycleIndex + 1) % coverPool.Count;
                return coverPool[coverCycleIndex];

            case BehaviorType.Flank:
            default:
                // Pick whichever assigned point currently sits closest to the player's side,
                // so the two flankers keep drifting toward the player's left/right as they move.
                Transform best = coverPool[0];
                float bestScore = float.MaxValue;
                foreach (var c in coverPool)
                {
                    float score = Vector3.Distance(c.position, player.position);
                    if (score < bestScore) { bestScore = score; best = c; }
                }
                return best;
        }
    }

    /// <summary>
    /// Cover Transforms mark the barrier's own centre, which is inside its collider - standing
    /// there reads as clipping through the cover. This instead picks a spot to its left or right
    /// (whichever side actually has sight of the player), just outside the barrier's footprint.
    /// </summary>
    private Vector3 ResolveEngagePosition(Transform cover)
    {
        if (cover == null) return transform.position;

        var col = cover.GetComponentInChildren<Collider>();
        float halfExtent = col != null ? Mathf.Max(col.bounds.extents.x, col.bounds.extents.z) : 1f;
        float standoff = halfExtent + 1.0f;

        Vector3 toPlayerFlat = player.position - cover.position;
        toPlayerFlat.y = 0f;
        if (toPlayerFlat.sqrMagnitude < 0.01f) toPlayerFlat = transform.forward;
        toPlayerFlat.Normalize();

        Vector3 side = Vector3.Cross(Vector3.up, toPlayerFlat);

        Vector3 rightPos = cover.position + side * standoff + toPlayerFlat * 0.4f;
        Vector3 leftPos = cover.position - side * standoff + toPlayerFlat * 0.4f;

        bool rightClear = HasLineOfSight(rightPos);
        bool leftClear = HasLineOfSight(leftPos);

        Vector3 chosen;
        if (rightClear && !leftClear) chosen = rightPos;
        else if (leftClear && !rightClear) chosen = leftPos;
        else if (rightClear) // both clear - take whichever is closer to the player
            chosen = Vector3.Distance(rightPos, player.position) <= Vector3.Distance(leftPos, player.position) ? rightPos : leftPos;
        else
            chosen = rightPos; // neither sees the player yet - still stand beside cover, not inside it

        return EnforceMinRange(chosen);
    }

    /// <summary>Pushes a candidate position further from the player if it's closer than
    /// <see cref="minEngageRange"/>, so enemies always shoot from range rather than up close.</summary>
    private Vector3 EnforceMinRange(Vector3 point)
    {
        Vector3 fromPlayer = point - player.position;
        fromPlayer.y = 0f;
        float dist = fromPlayer.magnitude;
        if (dist >= minEngageRange) return point;

        Vector3 awayDir = dist > 0.01f ? fromPlayer / dist : -transform.forward;
        Vector3 pushed = player.position + awayDir * minEngageRange;
        pushed.y = point.y;
        return ClampToRoom(pushed);
    }

    private bool HasLineOfSight(Vector3 fromGroundPos)
    {
        Vector3 eye = fromGroundPos + Vector3.up * 1.4f;
        Vector3 targetEye = player.position + Vector3.up * 1.4f;
        if (Physics.Linecast(eye, targetEye, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore))
            return hit.collider.gameObject == player.gameObject || hit.collider.transform.root == player.root;
        return true;
    }

    private IEnumerator MoveTo(Vector3 destination)
    {
        state = State.MovingToCover;
        destination = ClampToRoom(destination);
        const float obstacleRadius = 0.4f;
        const float lookahead = 1.0f;

        while (state != State.Dead && Vector3.Distance(transform.position, destination) > 0.35f)
        {
            Vector3 delta = destination - transform.position;
            delta.y = 0f;
            Vector3 moveDir = delta.normalized;
            Vector3 origin = transform.position + Vector3.up * 1f;

            if (IsBlocked(origin, moveDir, obstacleRadius, lookahead))
            {
                // A wall/pillar is directly ahead - this project has no NavMesh, so steer around
                // it instead of walking straight through (which read as clipping through walls).
                Vector3 side = Vector3.Cross(Vector3.up, moveDir);
                Vector3 rightDir = (moveDir + side).normalized;
                Vector3 leftDir = (moveDir - side).normalized;
                bool rightOpen = !IsBlocked(origin, rightDir, obstacleRadius, lookahead);
                bool leftOpen = !IsBlocked(origin, leftDir, obstacleRadius, lookahead);

                if (rightOpen) moveDir = rightDir;
                else if (leftOpen) moveDir = leftDir;
                else moveDir = side; // boxed in on both diagonals - slide sideways rather than stall
            }

            Vector3 next = transform.position + moveDir * (moveSpeed * Time.deltaTime);
            transform.position = ClampToRoom(next);
            FaceTarget(transform.position + delta);
            yield return null;
        }
    }

    private bool IsBlocked(Vector3 origin, Vector3 direction, float radius, float distance)
    {
        if (!Physics.SphereCast(origin, radius, direction, out RaycastHit hit, distance, ~0, QueryTriggerInteraction.Ignore))
            return false;
        if (hit.collider == hitCollider) return false;
        if (hit.collider.gameObject == player.gameObject || hit.collider.transform.root == player.root) return false;
        if (hit.collider.GetComponentInParent<EnemyAI>() != null) return false;
        return true;
    }

    /// <summary>
    /// Keeps this enemy inside Room 1 no matter what fed it a destination - covers both a bad
    /// cover-point reference and a player who has walked into an adjacent room while being chased.
    /// </summary>
    private Vector3 ClampToRoom(Vector3 point)
    {
        point.x = Mathf.Clamp(point.x, roomBoundsX.x, roomBoundsX.y);
        point.z = Mathf.Clamp(point.z, roomBoundsZ.x, roomBoundsZ.y);
        return point;
    }

    private void FaceTarget(Vector3 worldPoint)
    {
        Vector3 dir = worldPoint - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 12f * Time.deltaTime);
    }

    /// <summary>Instant turn-to-face, used right before aiming/firing so every shot actually
    /// comes from facing the player instead of lagging behind mid-turn from the walk-in.</summary>
    private void SnapFaceTarget(Vector3 worldPoint)
    {
        Vector3 dir = worldPoint - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    private void PlayAnim(string stateName)
    {
        if (animator != null) animator.Play(stateName, 0, 0f);
    }

    private void FireRound()
    {
        Vector3 muzzle = transform.position + Vector3.up * 1.35f + transform.forward * 0.3f;
        Vector3 targetPoint = player.position + Vector3.up * 1.5f;

        StartCoroutine(FlashMuzzle());

        // Don't waste a round shooting through a wall/pillar the AI is still behind.
        if (Physics.Linecast(muzzle, targetPoint, out RaycastHit blockHit, ~0, QueryTriggerInteraction.Ignore))
        {
            if (blockHit.collider.transform.root != player.root && blockHit.collider.gameObject != player.gameObject)
                return;
        }

        float accuracy = baseAccuracy;
        bool playerMoving = playerRigidbody != null && playerRigidbody.linearVelocity.magnitude > playerMovingSpeedThreshold;
        if (playerMoving) accuracy *= movingPlayerAccuracyMultiplier;

        bool isHit = Random.value < accuracy;
        Vector3 aimPoint = targetPoint;
        if (!isHit)
        {
            // Deliberately miss to one side/above so the shot visibly reads as a whiff.
            aimPoint += new Vector3(Random.Range(-1.4f, 1.4f), Random.Range(-0.3f, 1.2f), Random.Range(-1.4f, 1.4f));
        }

        Vector3 fireDir = (aimPoint - muzzle).normalized;
        if (Physics.Raycast(muzzle, fireDir, out RaycastHit hit, 60f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject == player.gameObject || hit.collider.transform.root == player.root)
                manager.OnPlayerHit(hit.point);
        }
    }

    private IEnumerator FlashMuzzle()
    {
        if (muzzleLight == null) yield break;
        muzzleLight.intensity = 7f;
        yield return new WaitForSeconds(0.05f);
        if (muzzleLight != null) muzzleLight.intensity = 0f;
    }

    /// <summary>Called by CombatEncounterManager when the player's weapon hits this enemy.</summary>
    public void RegisterHit()
    {
        if (state == State.Dead) return;
        hitsTaken++;
        if (hitsTaken >= hitsToKill) Die();
        else PlayAnim("Idle_Ducking_AR");
    }

    private void Die()
    {
        state = State.Dead;
        StopAllCoroutines();
        if (holdingFireSlot) { manager.ReleaseShooterSlot(this); holdingFireSlot = false; }
        PlayAnim("Die");
        if (hitCollider != null) hitCollider.enabled = false;
        manager.NotifyDeath(this);
        Destroy(gameObject, 5f);
    }
}
