using UnityEngine;

/// <summary>
/// The clips <see cref="CombatAudio"/> plays: the player being hit, the tick of a shot landing on
/// an enemy, and enemies going down. The asset lives at Resources/CombatAudioLibrary so every level
/// picks it up without any scene wiring.
/// </summary>
[CreateAssetMenu(menuName = "Glitch/Combat Audio Library", fileName = "CombatAudioLibrary")]
public class CombatAudioLibrary : ScriptableObject
{
    [Header("Player")]
    public AudioClip[] playerHurt;
    [Range(0f, 1f)] public float playerHurtVolume = 0.75f;

    [Header("Player's shots landing")]
    public AudioClip[] hitConfirm;
    [Range(0f, 1f)] public float hitConfirmVolume = 0.35f;

    [Header("Enemy deaths")]
    [Tooltip("Human soldiers dropping.")]
    public AudioClip[] fleshDeath;
    [Tooltip("Robots and mechs breaking apart.")]
    public AudioClip[] metalDeath;
    [Tooltip("Turrets and heavy robots blowing up.")]
    public AudioClip[] explosion;
    [Range(0f, 1f)] public float deathVolume = 0.9f;
}
