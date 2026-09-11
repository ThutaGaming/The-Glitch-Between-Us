using UnityEngine;

/// <summary>
/// Procedural "run" for Robot_grey: ping-pongs between two world points while hand-rolling a
/// marching leg rock, alternating arm swing and a stride bob (no animation clips exist on this
/// rigid-part mesh - see RobotIdleAnimator). Faces its direction of travel.
/// </summary>
public class RobotPatrol : MonoBehaviour
{
    public Vector3 pointA, pointB;
    public float speed = 2.5f;

    [SerializeField] private float legSwingDegrees = 22f;
    [SerializeField] private float armSwingDegrees = 30f;
    [SerializeField] private float bobAmplitude = 0.05f;
    [SerializeField] private float strideSpeedMultiplier = 3.2f;
    [SerializeField] private float turnSpeed = 8f;
    [SerializeField] private float arrivalDistance = 0.15f;

    private Transform legs, leftArm, rightArm;
    private Quaternion legsBaseRot, leftArmBaseRot, rightArmBaseRot;
    private bool towardB = true;
    private float floorY;
    private float stridePhase;

    private void Awake()
    {
        legs = transform.Find("legs");
        leftArm = transform.Find("left_arm");
        rightArm = transform.Find("right_arm");

        if (legs != null) legsBaseRot = legs.localRotation;
        if (leftArm != null) leftArmBaseRot = leftArm.localRotation;
        if (rightArm != null) rightArmBaseRot = rightArm.localRotation;

        floorY = transform.position.y;
        stridePhase = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        Vector3 target = towardB ? pointB : pointA;
        Vector3 pos = transform.position;
        Vector3 toTarget = target - pos;
        toTarget.y = 0f;

        if (toTarget.magnitude <= arrivalDistance)
        {
            towardB = !towardB;
        }
        else
        {
            Vector3 dir = toTarget.normalized;
            Vector3 moved = pos + dir * speed * Time.deltaTime;
            moved.y = floorY;
            transform.position = moved;

            Quaternion faceRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, faceRotation, turnSpeed * Time.deltaTime);
        }

        float stride = Time.time * speed * strideSpeedMultiplier + stridePhase;
        float swing = Mathf.Sin(stride);

        Vector3 p = transform.position;
        p.y = floorY + Mathf.Abs(swing) * bobAmplitude;
        transform.position = p;

        if (legs != null) legs.localRotation = legsBaseRot * Quaternion.Euler(swing * legSwingDegrees, 0f, 0f);
        if (leftArm != null) leftArm.localRotation = leftArmBaseRot * Quaternion.Euler(-swing * armSwingDegrees, 0f, 0f);
        if (rightArm != null) rightArm.localRotation = rightArmBaseRot * Quaternion.Euler(swing * armSwingDegrees, 0f, 0f);
    }
}
