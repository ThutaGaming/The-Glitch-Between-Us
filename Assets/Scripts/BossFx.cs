using UnityEngine;

/// <summary>
/// Spawning helpers shared by the Spider-Mech fight: one-shot/looping particle prefabs (scaled
/// through the whole hierarchy - the packs default to Local scaling, which ignores the root's
/// scale), positional sounds with pitch control, and short light flashes for muzzle/explosion punch.
/// </summary>
public static class BossFx
{
    /// <summary>Spawns an effect, lets it emit for <paramref name="emitFor"/> seconds, then lets the
    /// particles finish and cleans itself up.</summary>
    public static GameObject Play(GameObject prefab, Vector3 position, Quaternion rotation, float scale = 1f, float emitFor = 0.2f)
    {
        if (prefab == null) return null;

        var go = Object.Instantiate(prefab, position, rotation);
        Prepare(go, scale);
        go.AddComponent<FxAutoStop>().Init(emitFor, emitFor + MaxLifetime(go) + 0.25f);
        return go;
    }

    /// <summary>Spawns an effect that keeps playing until <see cref="Release"/> is called.</summary>
    public static GameObject Loop(GameObject prefab, Vector3 position, Quaternion rotation, float scale = 1f, Transform parent = null)
    {
        if (prefab == null) return null;

        var go = Object.Instantiate(prefab, position, rotation);
        Prepare(go, scale);
        if (parent != null) go.transform.SetParent(parent, true);
        return go;
    }

    /// <summary>Stops a looping effect's emission and destroys it once its particles have died out.</summary>
    public static void Release(GameObject go)
    {
        if (go == null) return;
        go.transform.SetParent(null, true);
        var stopper = go.GetComponent<FxAutoStop>();
        if (stopper == null) stopper = go.AddComponent<FxAutoStop>();
        stopper.Init(0f, MaxLifetime(go) + 0.25f);
    }

    /// <summary>Some packs build their "gather" parts several metres above the root (e.g. the Red energy
    /// explosion's Core/Trails at y=3). Pulls those children down so the energy converges on the boss.</summary>
    public static void ClampChildHeights(GameObject go, float maxLocalY)
    {
        if (go == null) return;
        foreach (Transform child in go.transform)
        {
            var p = child.localPosition;
            if (p.y > maxLocalY) child.localPosition = new Vector3(p.x, maxLocalY, p.z);
        }
    }

    private static void Prepare(GameObject go, float scale)
    {
        go.transform.localScale = Vector3.one * scale;
        foreach (var ps in go.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = ps.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
        }
    }

    private static float MaxLifetime(GameObject go)
    {
        float life = 0.5f;
        foreach (var ps in go.GetComponentsInChildren<ParticleSystem>(true))
        {
            var main = ps.main;
            life = Mathf.Max(life, Mathf.Max(main.startLifetime.constantMax, main.startLifetimeMultiplier));
        }
        return Mathf.Min(life, 8f);
    }

    public static AudioClip Pick(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }

    /// <summary>Positional one-shot with its own pitch (AudioSource.PlayClipAtPoint can't do pitch).</summary>
    public static AudioSource Sfx(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f,
        float minDistance = 6f, float maxDistance = 90f, float spatial = 1f)
    {
        if (clip == null) return null;

        var go = new GameObject("SFX_" + clip.name);
        go.transform.position = position;
        var src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = volume;
        src.pitch = pitch;
        src.spatialBlend = spatial;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.minDistance = minDistance;
        src.maxDistance = maxDistance;
        src.dopplerLevel = 0f;
        src.Play();
        Object.Destroy(go, clip.length / Mathf.Max(0.05f, Mathf.Abs(pitch)) + 0.1f);
        return src;
    }

    /// <summary>Brief point-light pop that lights up the surroundings (muzzle flashes, blasts).</summary>
    public static void Flash(Vector3 position, Color color, float intensity, float range, float duration)
    {
        var go = new GameObject("FxFlash");
        go.transform.position = position;
        var light = go.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.range = range;
        light.shadows = LightShadows.None;
        go.AddComponent<FxLightFade>().Init(intensity, duration);
    }
}
