using UnityEngine;

/// <summary>
/// One-shot combat sounds that no single enemy owns: the player taking a hit, the tick of the
/// player's shot landing, and enemies dying. Clips come from Resources/CombatAudioLibrary; with
/// that asset missing every call is a silent no-op.
/// </summary>
public static class CombatAudio
{
    public enum Death { Flesh, Metal, Explosion }

    private static CombatAudioLibrary library;
    private static bool loaded;
    private static float lastHurtTime = -99f;
    private static float lastHitTime = -99f;

    // Play Mode can start without a domain reload, so clear the throttles explicitly.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        library = null;
        loaded = false;
        lastHurtTime = -99f;
        lastHitTime = -99f;
    }

    private static CombatAudioLibrary Library
    {
        get
        {
            if (!loaded)
            {
                library = Resources.Load<CombatAudioLibrary>("CombatAudioLibrary");
                loaded = true;
            }
            return library;
        }
    }

    /// <summary>Plays on the listener, so it reads as "I'm hit" whichever way the player faces.</summary>
    public static void PlayerHurt()
    {
        var lib = Library;
        if (lib == null || Time.time - lastHurtTime < 0.08f) return;
        lastHurtTime = Time.time;
        Play(Pick(lib.playerHurt), Vector3.zero, lib.playerHurtVolume, Random.Range(0.78f, 0.9f), 0f);
    }

    public static void HitConfirm(Vector3 point)
    {
        var lib = Library;
        if (lib == null || Time.time - lastHitTime < 0.05f) return;
        lastHitTime = Time.time;
        Play(Pick(lib.hitConfirm), point, lib.hitConfirmVolume, Random.Range(1.35f, 1.55f), 0.35f);
    }

    public static void EnemyDeath(Vector3 point, Death kind)
    {
        var lib = Library;
        if (lib == null) return;
        switch (kind)
        {
            case Death.Flesh:
                Play(Pick(lib.fleshDeath), point, lib.deathVolume, Random.Range(0.9f, 1.05f), 1f);
                break;
            case Death.Metal:
                Play(Pick(lib.metalDeath), point, lib.deathVolume, Random.Range(0.75f, 0.9f), 1f);
                break;
            case Death.Explosion:
                Play(Pick(lib.explosion), point, lib.deathVolume, Random.Range(0.95f, 1.1f), 1f);
                Play(Pick(lib.metalDeath), point, lib.deathVolume * 0.7f, Random.Range(0.7f, 0.85f), 1f);
                break;
        }
    }

    private static AudioClip Pick(AudioClip[] clips)
    {
        return clips == null || clips.Length == 0 ? null : clips[Random.Range(0, clips.Length)];
    }

    private static void Play(AudioClip clip, Vector3 point, float volume, float pitch, float spatialBlend)
    {
        if (clip == null) return;

        var go = new GameObject("~CombatSfx");
        go.transform.position = point;
        var source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.spatialBlend = spatialBlend;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = 5f;
        source.maxDistance = 70f;
        source.Play();
        Object.Destroy(go, clip.length / Mathf.Max(0.1f, pitch) + 0.1f);
    }
}
