using UnityEngine;

/// <summary>
/// Procedural idle pose for Robot_grey: it's built from separate rigid mesh parts (head,
/// left_arm, right_arm, legs, ...) with no Animator or animation clips, so this hand-rolls a
/// gentle breathing bob plus a head look side-to-side and a faint arm sway instead.
/// </summary>
public class RobotIdleAnimator : MonoBehaviour
{
    [SerializeField] private float bobAmplitude = 0.03f;
    [SerializeField] private float bobSpeed = 1.4f;
    [SerializeField] private float headSwaySpeed = 0.6f;
    [SerializeField] private float headSwayDegrees = 12f;
    [SerializeField] private float armSwayDegrees = 5f;

    private Transform root, head, leftArm, rightArm;
    private Vector3 rootBasePos;
    private Quaternion headBaseRot, leftArmBaseRot, rightArmBaseRot;
    private float phase;

    private void Awake()
    {
        root = transform;
        head = transform.Find("head");
        leftArm = transform.Find("left_arm");
        rightArm = transform.Find("right_arm");

        rootBasePos = root.localPosition;
        if (head != null) headBaseRot = head.localRotation;
        if (leftArm != null) leftArmBaseRot = leftArm.localRotation;
        if (rightArm != null) rightArmBaseRot = rightArm.localRotation;

        // Desync multiple idle robots spawned in the same wave.
        phase = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        float t = Time.time * bobSpeed + phase;
        root.localPosition = rootBasePos + Vector3.up * (Mathf.Sin(t) * bobAmplitude);

        if (head != null)
        {
            float sway = Mathf.Sin(Time.time * headSwaySpeed + phase) * headSwayDegrees;
            head.localRotation = headBaseRot * Quaternion.Euler(0f, sway, 0f);
        }

        float armSwing = Mathf.Sin(Time.time * bobSpeed * 0.8f + phase) * armSwayDegrees;
        if (leftArm != null) leftArm.localRotation = leftArmBaseRot * Quaternion.Euler(armSwing, 0f, 0f);
        if (rightArm != null) rightArm.localRotation = rightArmBaseRot * Quaternion.Euler(-armSwing, 0f, 0f);
    }
}
