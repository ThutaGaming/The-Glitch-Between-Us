using UnityEngine;

/// <summary>
/// One asset that holds every VFX prefab (Effect 1 / Effect 2 packs) and audio clip the Spider-Mech
/// fight uses, so the boss, its projectiles, the arena and the player-side hit detector all share a
/// single place to retune the look and sound of the encounter.
/// </summary>
[CreateAssetMenu(menuName = "Boss/Spider-Mech FX Library", fileName = "SpiderMechFxLibrary")]
public class SpiderMechFxLibrary : ScriptableObject
{
    [Header("Plasma barrage")]
    public GameObject muzzleFlash;
    public GameObject plasmaBolt;
    public GameObject boltImpactWorld;
    public GameObject boltImpactPlayer;

    [Header("Incendiary mortar")]
    public GameObject mortarShell;
    public GameObject mortarTelegraph;
    public GameObject mortarExplosion;
    public GameObject mortarGroundBlast;
    public GameObject fireZoneFlames;
    public GameObject fireZoneSmoke;

    [Header("Ultimate / stun / shockwave")]
    public GameObject chargeAura;
    public GameObject chargeImplosion;
    public GameObject interruptBurst;
    public GameObject stunSparks;
    public GameObject stunSmoke;
    public GameObject shockwaveCore;
    public GameObject shockwaveRingFx;

    [Header("Body & hits")]
    public GameObject shieldBubble;
    public GameObject shieldHit;
    public GameObject shieldBreak;
    public GameObject bodyHit;
    public GameObject weakPointHit;
    public GameObject jointBreak;
    public GameObject footstepDust;
    public GameObject landingDust;
    public GameObject landingBlast;
    public GameObject phaseTwoBurst;
    public GameObject deathExplosionSmall;
    public GameObject deathExplosionBig;
    public GameObject wreckFire;
    public GameObject wreckSmoke;

    [Header("Arena")]
    public GameObject lightningStrike;
    public GameObject electroHit;
    public GameObject platformDust;

    [Header("Audio")]
    public AudioClip[] footsteps;
    public AudioClip servoLoop;
    public AudioClip[] plasmaFire;
    public AudioClip[] boltImpacts;
    public AudioClip mortarLaunch;
    public AudioClip[] explosions;
    public AudioClip bigExplosion;
    public AudioClip fireLoop;
    public AudioClip chargeLoop;
    public AudioClip powerDown;
    public AudioClip powerUp;
    public AudioClip heavyImpact;
    public AudioClip[] metalDebris;
    public AudioClip shieldHitClip;
    public AudioClip weakPointHitClip;
    public AudioClip electricLoop;
    public AudioClip warningClip;
    public AudioClip barrierClip;
}
