using UnityEngine;

/// <summary>Self-destroying point light that fades out - owns its own lifetime, so a flash
/// survives even if whatever spawned it (a projectile, a dying boss) is destroyed first.</summary>
[RequireComponent(typeof(Light))]
public class FxLightFade : MonoBehaviour
{
    private Light lightSource;
    private float startIntensity;
    private float duration;
    private float elapsed;

    public void Init(float intensity, float fadeSeconds)
    {
        lightSource = GetComponent<Light>();
        startIntensity = intensity;
        duration = Mathf.Max(0.01f, fadeSeconds);
        lightSource.intensity = intensity;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        if (lightSource != null) lightSource.intensity = Mathf.Lerp(startIntensity, 0f, elapsed / duration);
        if (elapsed >= duration) Destroy(gameObject);
    }
}
