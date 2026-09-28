using UnityEngine;

/// <summary>
/// Turns a (possibly looping) particle prefab into a one-off burst: every particle system stops
/// emitting after <see cref="emitFor"/> seconds and the object removes itself after
/// <see cref="lifetime"/>, so the particles already in the air finish naturally.
/// </summary>
public class TimedEffect : MonoBehaviour
{
    public float emitFor = 0.5f;
    public float lifetime = 4f;

    private float born;
    private bool stopped;

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, float scale,
        float emitFor, float lifetime, Transform parent = null)
    {
        if (prefab == null) return null;
        var go = Instantiate(prefab, position, rotation, parent);
        float parentScale = parent != null ? Mathf.Max(0.0001f, parent.lossyScale.x) : 1f;
        go.transform.localScale = prefab.transform.localScale * (scale / parentScale);
        // Hierarchy scaling so every child system shrinks or grows with the root.
        foreach (var ps in go.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = ps.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
        }
        var timed = go.AddComponent<TimedEffect>();
        timed.emitFor = emitFor;
        timed.lifetime = lifetime;
        return go;
    }

    private void Awake() => born = Time.time;

    private void Update()
    {
        float age = Time.time - born;
        if (!stopped && emitFor >= 0f && age >= emitFor)
        {
            stopped = true;
            foreach (var ps in GetComponentsInChildren<ParticleSystem>())
                ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
        if (lifetime > 0f && age >= lifetime) Destroy(gameObject);
    }

    /// <summary>Stop emitting now and clean up once the last particles fade.</summary>
    public void FadeOut(float after = 3f)
    {
        emitFor = 0f;
        lifetime = Time.time - born + after;
    }
}
