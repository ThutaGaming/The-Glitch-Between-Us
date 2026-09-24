using UnityEngine;

/// <summary>
/// Trauma-based positional camera shake for the player's world camera. Position only: CameraLook
/// re-reads the camera's localRotation every frame to accumulate pitch, so a rotational shake would
/// bleed permanently into the aim. The previous frame's offset is removed before the new one is
/// applied, so this never drifts regardless of what else moves the camera.
/// </summary>
public class CameraShake : MonoBehaviour
{
    private static CameraShake instance;

    [SerializeField] private float maxOffset = 0.16f;
    [SerializeField] private float traumaDecayPerSecond = 1.5f;
    [SerializeField] private float frequency = 24f;

    private float trauma;
    private Vector3 lastOffset;

    public static void Ensure(Transform cameraTransform)
    {
        if (instance != null || cameraTransform == null) return;
        instance = cameraTransform.GetComponent<CameraShake>();
        if (instance == null) instance = cameraTransform.gameObject.AddComponent<CameraShake>();
    }

    /// <summary>Adds trauma (0..1) scaled by distance - full strength at the source, none past radius.</summary>
    public static void Shake(Vector3 source, float amount, float radius)
    {
        if (instance == null) return;
        float falloff = radius <= 0f ? 1f
            : Mathf.Clamp01(1f - Vector3.Distance(instance.transform.position, source) / radius);
        instance.trauma = Mathf.Clamp01(instance.trauma + amount * falloff);
    }

    private void LateUpdate()
    {
        transform.localPosition -= lastOffset;

        trauma = Mathf.Max(0f, trauma - traumaDecayPerSecond * Time.deltaTime);
        float strength = trauma * trauma;
        float t = Time.time * frequency;
        lastOffset = new Vector3(
            Mathf.PerlinNoise(t, 0.37f) - 0.5f,
            Mathf.PerlinNoise(0.71f, t) - 0.5f,
            (Mathf.PerlinNoise(t, t) - 0.5f) * 0.5f) * (2f * maxOffset * strength);

        transform.localPosition += lastOffset;
    }

    private void OnDisable()
    {
        transform.localPosition -= lastOffset;
        lastOffset = Vector3.zero;
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
