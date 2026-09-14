using System.Collections.Generic;
using UnityEngine;
using InfimaGames.LowPolyShooterPack;

/// <summary>
/// Orchestrates Room 1's two-wave, five-enemy encounter: spawns Wave 1 when the player crosses
/// the combat trigger, spawns Wave 2 once two of Wave 1's three enemies are dead, caps how many
/// enemies may be actively bursting at once (the rest hold in Aim/Cover), and on the fifth death
/// unlocks the exit door and shows "AREA SECURED".
///
/// Also owns player-vs-enemy hit detection, reusing BasicTrainingRange's proven technique:
/// watch the equipped weapon's ammo count and raycast the instant it drops, rather than reaching
/// into Infima's private weapon-fire internals.
/// </summary>
public class CombatEncounterManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs; // HP / PBR / Polyart, for visual variety only

    [Header("Wave 1")]
    [SerializeField] private Transform wave1Spawn_Left;
    [SerializeField] private Transform wave1Spawn_Right;
    [SerializeField] private Transform wave1Spawn_Center;
    [SerializeField] private Transform[] coverPool_Left;
    [SerializeField] private Transform[] coverPool_Right;
    [SerializeField] private Transform[] coverPool_Center;

    [Header("Wave 2")]
    [SerializeField] private Transform wave2Spawn_Left;
    [SerializeField] private Transform wave2Spawn_Right;
    [SerializeField] private Transform[] coverPool_FlankLeft;
    [SerializeField] private Transform[] coverPool_FlankRight;

    [Header("Refs")]
    [SerializeField] private DoubleSlidingDoor exitDoor;
    [SerializeField] private MissionHUD mission;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Tuning")]
    [SerializeField] private int maxActiveShooters = 3;
    [SerializeField] private float raycastDistance = 500f;
    [Tooltip("Small forgiveness radius on the player's shot so a near-perfect long-range aim (where recoil/sway makes a pixel-perfect hit unreliable) still registers.")]
    [SerializeField] private float shotForgivenessRadius = 0.12f;
    [SerializeField] private int damagePerHit = 9;

    private readonly List<EnemyAI> wave1Enemies = new List<EnemyAI>();
    private readonly List<EnemyAI> wave2Enemies = new List<EnemyAI>();
    private readonly HashSet<EnemyAI> activeShooters = new HashSet<EnemyAI>();

    private bool wave1Started, wave2Started, combatComplete;
    private int wave1Deaths, totalDeaths;

    private Transform playerTransform;
    private CharacterBehaviour playerCharacter;
    private InventoryBehaviour playerInventory;
    private Transform cameraTransform;
    private int lastAmmo = -1;

    private readonly List<EnemyAI> allEnemies = new List<EnemyAI>();

    private string overlayMessage;
    private float overlayTimer;

    private void Awake()
    {
        var gameModeService = ServiceLocator.Current.Get<IGameModeService>();
        playerCharacter = gameModeService != null ? gameModeService.GetPlayerCharacter() : null;
        playerInventory = playerCharacter != null ? playerCharacter.GetInventory() : null;
        cameraTransform = playerCharacter != null ? playerCharacter.GetCameraWorld().transform : null;
        playerTransform = playerCharacter != null ? playerCharacter.transform : null;

        // Locked until the last enemy falls.
        if (exitDoor != null) exitDoor.enabled = false;
    }

    private void Update()
    {
        if (overlayTimer > 0f) overlayTimer -= Time.deltaTime;

        if (!wave1Started || playerInventory == null) return;

        WeaponBehaviour equipped = playerInventory.GetEquipped();
        if (equipped == null) { lastAmmo = -1; return; }

        int current = equipped.GetAmmunitionCurrent();
        if (lastAmmo >= 0 && current < lastAmmo) TryRegisterPlayerShot();
        lastAmmo = current;
    }

    private void TryRegisterPlayerShot()
    {
        if (cameraTransform == null) return;

        // A thin sphere instead of a pure ray so a near-perfect long-range shot (where a
        // fraction of a degree of sway/recoil would otherwise skim past the edge of a distant,
        // small-on-screen hitbox) still counts, at any range.
        if (Physics.SphereCast(cameraTransform.position, shotForgivenessRadius, cameraTransform.forward,
                out RaycastHit hit, raycastDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            var enemy = hit.collider.GetComponentInParent<EnemyAI>();
            if (enemy != null) enemy.RegisterHit();
        }
    }

    /// <summary>Wired to the combat trigger's OnTriggerEnter (see CombatTriggerZone).</summary>
    public void StartWave1()
    {
        if (wave1Started) return;
        wave1Started = true;

        var e1 = SpawnEnemy(wave1Spawn_Left.position, ToList(coverPool_Left), EnemyAI.BehaviorType.HoldAndBurst, 0);
        var e2 = SpawnEnemy(wave1Spawn_Right.position, ToList(coverPool_Right), EnemyAI.BehaviorType.HoldAndBurst, 1);
        var e3 = SpawnEnemy(wave1Spawn_Center.position, ToList(coverPool_Center), EnemyAI.BehaviorType.Reposition, 2);
        wave1Enemies.Add(e1); wave1Enemies.Add(e2); wave1Enemies.Add(e3);

        if (mission != null) mission.SetObjective("Clear Room 1 Hostiles", 5);
    }

    private void StartWave2()
    {
        if (wave2Started) return;
        wave2Started = true;

        var e4 = SpawnEnemy(wave2Spawn_Left.position, ToList(coverPool_FlankLeft), EnemyAI.BehaviorType.Flank, 3);
        var e5 = SpawnEnemy(wave2Spawn_Right.position, ToList(coverPool_FlankRight), EnemyAI.BehaviorType.Flank, 4);
        wave2Enemies.Add(e4); wave2Enemies.Add(e5);
    }

    private EnemyAI SpawnEnemy(Vector3 spawnPoint, List<Transform> cover, EnemyAI.BehaviorType behavior, int variantIndex)
    {
        var prefab = enemyPrefabs[variantIndex % enemyPrefabs.Length];
        var go = Instantiate(prefab, spawnPoint, Quaternion.identity);
        var ai = go.GetComponent<EnemyAI>();
        if (ai == null) ai = go.AddComponent<EnemyAI>();
        ai.Initialize(this, playerTransform, cover, behavior, spawnPoint);
        allEnemies.Add(ai);
        return ai;
    }

    public bool TryAcquireShooterSlot(EnemyAI enemy)
    {
        if (activeShooters.Contains(enemy)) return true;
        if (activeShooters.Count >= maxActiveShooters) return false;
        activeShooters.Add(enemy);
        return true;
    }

    public void ReleaseShooterSlot(EnemyAI enemy)
    {
        activeShooters.Remove(enemy);
    }

    public void OnPlayerHit(Vector3 hitPoint)
    {
        if (playerHealth != null) playerHealth.ApplyDamage(damagePerHit);
        SpawnBloodBurst(hitPoint);
        StartCoroutine(FlashDamage());
    }

    private void SpawnBloodBurst(Vector3 point)
    {
        var go = new GameObject("BloodBurst");
        go.transform.position = point;
        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.duration = 0.4f;
        main.loop = false;
        main.startLifetime = 0.4f;
        main.startSpeed = 2.5f;
        main.startSize = 0.08f;
        main.startColor = new Color(0.55f, 0.02f, 0.02f);
        main.gravityModifier = 1.5f;
        main.stopAction = ParticleSystemStopAction.Destroy;
        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 14, 22, 1, 0.01f) });
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.05f;
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        ps.Play();
    }

    private System.Collections.IEnumerator FlashDamage()
    {
        damageFlash = 1f;
        bloodSplatSeed = Random.Range(0, 10000);
        while (damageFlash > 0f)
        {
            damageFlash -= Time.deltaTime * 1.1f;
            yield return null;
        }
        damageFlash = 0f;
    }

    private float damageFlash;
    private int bloodSplatSeed;

    public void NotifyDeath(EnemyAI enemy)
    {
        totalDeaths++;
        if (mission != null) mission.SetProgress(totalDeaths);

        if (wave1Enemies.Contains(enemy))
        {
            wave1Deaths++;
            if (wave1Deaths >= 2 && !wave2Started) StartWave2();
        }

        if (totalDeaths >= 5 && !combatComplete) CombatComplete();
    }

    private void CombatComplete()
    {
        combatComplete = true;
        if (mission != null) mission.CompleteObjective();
        if (exitDoor != null) exitDoor.enabled = true;
        ShowOverlay("AREA SECURED", 2.5f);
    }

    private void ShowOverlay(string text, float duration)
    {
        if (text == null) return;
        overlayMessage = text;
        overlayTimer = duration;
    }

    private static List<Transform> ToList(Transform[] arr)
    {
        var list = new List<Transform>();
        if (arr != null) list.AddRange(arr);
        return list;
    }

    private GUIStyle overlayStyle;

    private void OnGUI()
    {
        if (damageFlash > 0f)
        {
            var prev = GUI.color;

            // Screen-edge vignette, like blood pooling at the corners of vision.
            GUI.color = new Color(0.45f, 0f, 0f, damageFlash * 0.3f);
            float edge = 140f;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, edge), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(0, Screen.height - edge, Screen.width, edge), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(0, 0, edge, Screen.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(Screen.width - edge, 0, edge, Screen.height), Texture2D.whiteTexture);

            // A handful of irregular blood-splat blobs, stable for the duration of this hit
            // (seeded once in FlashDamage) so they read as splatter rather than a flicker.
            var rnd = new System.Random(bloodSplatSeed);
            for (int i = 0; i < 6; i++)
            {
                float sx = (float)rnd.NextDouble() * Screen.width;
                float sy = (float)rnd.NextDouble() * Screen.height;
                float size = 60f + (float)rnd.NextDouble() * 120f;
                GUI.color = new Color(0.5f, 0.02f, 0.02f, damageFlash * 0.5f * (float)rnd.NextDouble());
                GUI.DrawTexture(new Rect(sx - size * 0.5f, sy - size * 0.5f, size, size), Texture2D.whiteTexture);
            }

            GUI.color = prev;
        }

        if (overlayTimer > 0f && !string.IsNullOrEmpty(overlayMessage))
        {
            if (overlayStyle == null)
            {
                overlayStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 36,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }
            var c = new Color(0.4f, 1f, 0.55f, Mathf.Clamp01(overlayTimer));
            overlayStyle.normal.textColor = c;
            GUI.Label(new Rect(0, Screen.height * 0.18f, Screen.width, 60f), overlayMessage, overlayStyle);
        }
    }
}
