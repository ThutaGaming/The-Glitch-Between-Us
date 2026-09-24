using UnityEngine;

/// <summary>
/// Turns a looping showcase effect into a one-shot: stops emission after a short window (so the
/// existing particles finish their lifetime naturally instead of popping) and then destroys it.
/// Most Effect 1 / Effect 2 prefabs ship with "Looping" on because they were built for a demo reel.
/// </summary>
public class FxAutoStop : MonoBehaviour
{
    private float stopAt;
    private float destroyAt;
    private bool stopped;

    public void Init(float stopAfter, float destroyAfter)
    {
        stopAt = Time.time + Mathf.Max(0f, stopAfter);
        destroyAt = Time.time + Mathf.Max(stopAfter, destroyAfter);
        stopped = false;
    }

    private void Update()
    {
        if (!stopped && Time.time >= stopAt)
        {
            stopped = true;
            foreach (var ps in GetComponentsInChildren<ParticleSystem>())
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            foreach (var l in GetComponentsInChildren<Light>())
                l.enabled = false;
        }

        if (Time.time >= destroyAt) Destroy(gameObject);
    }
}
