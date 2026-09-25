using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A character a cutscene script drives (see NewStudentCutscene): plays states on its humanoid
/// Animator, walks a path at the pace its walk cycle actually covers, and turns its head or
/// reaches its right hand toward things through humanoid IK.
///
/// The Animator's controller needs "IK Pass" on its base layer for the look-at and reach to work
/// (ClassroomCutsceneBaker sets it on AC_TrHtetHtet / AC_MoneMone).
/// </summary>
[RequireComponent(typeof(Animator))]
public class CutsceneActor : MonoBehaviour
{
    [Header("Walking")]
    [Tooltip("The walk clip's forward speed at human scale 1 (Mr. Thiha's Mixamo walk: 1.68).")]
    [SerializeField] private float walkClipSpeed = 1.68f;
    [Tooltip("Must match the Walk state's Speed in the Animator Controller.")]
    [SerializeField] private float walkStateSpeed = 0.8f;
    [SerializeField] private float turnDegreesPerSecond = 240f;

    [Header("Look / reach")]
    [Tooltip("How fast the head turn and the hand reach blend in and out (weight per second).")]
    [SerializeField] private float ikBlendSpeed = 2.5f;
    [Tooltip("How much of the way the reaching hand is pulled onto the target line.")]
    [Range(0f, 1f)]
    [SerializeField] private float reachStrength = 0.75f;

    private Animator anim;

    private Transform lookTarget;
    private Vector3 lookPoint;
    private float lookWeight, lookWeightGoal, lookBodyWeight = 0.2f;

    private Transform reachTarget;
    private Vector3 reachPoint;
    private float reachWeight, reachWeightGoal;

    public Animator Animator => anim != null ? anim : (anim = GetComponent<Animator>());
    public Transform Head => Animator.GetBoneTransform(HumanBodyBones.Head);
    public Transform Chest => Animator.GetBoneTransform(HumanBodyBones.Chest);

    /// <summary>World speed that keeps the feet from sliding: the clip's stride scaled to this
    /// character's size (humanScale is in model units, so the transform scale goes on top).</summary>
    private float WalkSpeed => walkClipSpeed * walkStateSpeed * Animator.humanScale * transform.lossyScale.y;

    public void Play(string state, float fade = 0.25f) => Animator.CrossFadeInFixedTime(state, fade);

    public void Place(Vector3 position, float yaw) =>
        transform.SetPositionAndRotation(position, Quaternion.Euler(0f, yaw, 0f));

    public void LookAt(Transform target, float bodyWeight = 0.2f)
    {
        lookTarget = target;
        lookBodyWeight = bodyWeight;
        lookWeightGoal = 1f;
    }

    public void LookAt(Vector3 point, float bodyWeight = 0.2f)
    {
        lookTarget = null;
        lookPoint = point;
        lookBodyWeight = bodyWeight;
        lookWeightGoal = 1f;
    }

    public void StopLooking() => lookWeightGoal = 0f;

    /// <summary>Stretches the right hand out along the line from the shoulder to the target.</summary>
    public void Reach(Transform target)
    {
        reachTarget = target;
        reachWeightGoal = 1f;
    }

    public void Reach(Vector3 point)
    {
        reachTarget = null;
        reachPoint = point;
        reachWeightGoal = 1f;
    }

    public void StopReaching() => reachWeightGoal = 0f;

    public IEnumerator WalkPath(IList<Vector3> points, string arriveState = "Idle")
    {
        Play("Walk", 0.25f);
        foreach (var p in points)
        {
            Vector3 target = new Vector3(p.x, transform.position.y, p.z);
            while (true)
            {
                Vector3 to = target - transform.position;
                to.y = 0f;
                float dist = to.magnitude;
                if (dist < 0.03f) break;

                Vector3 dir = to / dist;
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(dir, Vector3.up), turnDegreesPerSecond * Time.deltaTime);
                // Ease off while still turning into a corner, so the feet don't stride sideways.
                float align = Mathf.Clamp01(Vector3.Dot(transform.forward, dir));
                transform.position = Vector3.MoveTowards(transform.position, target,
                    WalkSpeed * Mathf.Lerp(0.35f, 1f, align) * Time.deltaTime);
                yield return null;
            }
        }
        if (!string.IsNullOrEmpty(arriveState)) Play(arriveState, 0.3f);
    }

    public IEnumerator TurnTo(float yaw)
    {
        Quaternion goal = Quaternion.Euler(0f, yaw, 0f);
        while (Quaternion.Angle(transform.rotation, goal) > 0.5f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, goal, turnDegreesPerSecond * 0.6f * Time.deltaTime);
            yield return null;
        }
        transform.rotation = goal;
    }

    /// <summary>Eases the root to <paramref name="to"/> - e.g. sliding in onto a chair while the
    /// sit pose cross-fades in.</summary>
    public IEnumerator Slide(Vector3 to, float duration)
    {
        Vector3 from = transform.position;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t / duration));
            yield return null;
        }
        transform.position = to;
    }

    private void Update()
    {
        lookWeight = Mathf.MoveTowards(lookWeight, lookWeightGoal, ikBlendSpeed * Time.deltaTime);
        reachWeight = Mathf.MoveTowards(reachWeight, reachWeightGoal, ikBlendSpeed * Time.deltaTime);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        var a = Animator;

        a.SetLookAtWeight(lookWeight, lookBodyWeight, 1f, 0f, 0.5f);
        if (lookWeight > 0f) a.SetLookAtPosition(lookTarget != null ? lookTarget.position : lookPoint);

        a.SetIKPositionWeight(AvatarIKGoal.RightHand, reachWeight * reachStrength);
        if (reachWeight > 0f)
        {
            Transform shoulder = a.GetBoneTransform(HumanBodyBones.RightUpperArm);
            Transform elbow = a.GetBoneTransform(HumanBodyBones.RightLowerArm);
            Transform hand = a.GetBoneTransform(HumanBodyBones.RightHand);
            float armLength = Vector3.Distance(shoulder.position, elbow.position) + Vector3.Distance(elbow.position, hand.position);
            Vector3 target = reachTarget != null ? reachTarget.position : reachPoint;
            Vector3 dir = (target - shoulder.position).normalized;
            a.SetIKPosition(AvatarIKGoal.RightHand, shoulder.position + dir * armLength * 0.95f);
        }
    }
}
