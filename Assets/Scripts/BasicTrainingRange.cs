using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using InfimaGames.LowPolyShooterPack;

/// <summary>
/// Six-stage Robot_grey shooting range, started by <see cref="Begin"/> (TrainingTerminal.Interact).
/// Waypoints are resolved by name (Floor_Normalb1, Floor_Squareb21/22, ...) so no per-object
/// Inspector wiring is needed; a tile's centre is its Renderer.bounds.center, not its raw
/// transform.position - these tiles are pivoted at a corner, not their middle (confirmed by
/// probing every waypoint before writing this).
///
/// Hit detection: Infima's WeaponBehaviour has no hit/damage event, so this watches the equipped
/// weapon's ammo count and fires its own raycast the instant it drops by one - the same frame
/// Weapon.Fire() actually shoots - then resolves whichever RobotTarget that ray hit.
/// </summary>
public class BasicTrainingRange : MonoBehaviour
{
    [SerializeField] private GameObject robotPrefab;
    [SerializeField] private MissionHUD mission;
    [SerializeField] private int hitsToKill = 3;

    [Header("Stage 1 spawn spread (either side of the tile centre)")]
    [SerializeField] private float stage1Offset = 3f;

    [Header("Patrol speeds")]
    [SerializeField] private float stage4Speed = 2.5f;
    [SerializeField] private float stage5Speed = 3.5f;
    [SerializeField] private float stage6Speed = 4.5f;

    [SerializeField] private float raycastDistance = 200f;

    [Tooltip("Fires once, right after the last stage is cleared and the objective is completed.")]
    public UnityEvent onComplete;

    private bool started;
    private readonly List<RobotTarget> alive = new List<RobotTarget>();

    private IGameModeService gameModeService;
    private CharacterBehaviour playerCharacter;
    private InventoryBehaviour playerCharacterInventory;
    private Transform cameraTransform;
    private int lastAmmo = -1;

    private void Awake()
    {
        gameModeService = ServiceLocator.Current.Get<IGameModeService>();
        playerCharacter = gameModeService != null ? gameModeService.GetPlayerCharacter() : null;
        playerCharacterInventory = playerCharacter != null ? playerCharacter.GetInventory() : null;
        cameraTransform = playerCharacter != null ? playerCharacter.GetCameraWorld().transform : null;
    }

    private void Update()
    {
        if (!started || playerCharacterInventory == null) return;

        WeaponBehaviour equipped = playerCharacterInventory.GetEquipped();
        if (equipped == null) { lastAmmo = -1; return; }

        int current = equipped.GetAmmunitionCurrent();
        if (lastAmmo >= 0 && current < lastAmmo) TryRegisterShot();
        lastAmmo = current;
    }

    private void TryRegisterShot()
    {
        if (cameraTransform == null) return;
        if (!Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, raycastDistance)) return;

        var target = hit.collider.GetComponentInParent<RobotTarget>();
        if (target != null) target.RegisterHit();
    }

    /// <summary>Starts the range once; later calls are ignored.</summary>
    public void Begin()
    {
        if (started) return;
        started = true;
        StartCoroutine(RunStages());
    }

    private IEnumerator RunStages()
    {
        Vector3 b1 = TileCenter("Floor_Normalb1");
        Vector3 sq21 = TileCenter("Floor_Squareb21");
        Vector3 sq22 = TileCenter("Floor_Squareb22");
        Vector3 b3 = TileCenter("Floor_Normalb3");
        Vector3 b41 = TileCenter("Floor_Normalb41");
        Vector3 b42 = TileCenter("Floor_Normalb42");
        Vector3 h51 = TileCenter("Floor_Halfb51");
        Vector3 h52 = TileCenter("Floor_Halfb52");
        Vector3 b61 = TileCenter("Floor_Normalb61");
        Vector3 b62 = TileCenter("Floor_Normalb62");

        Vector3 side = Vector3.right * stage1Offset;
        yield return SpawnIdleWaveAndWait(new[] { b1 - side, b1 + side });

        yield return SpawnIdleWaveAndWait(new[] { sq21, sq22 });

        yield return SpawnIdleWaveAndWait(new[] { b3 });

        yield return SpawnPatrolWaveAndWait(b41, b42, stage4Speed);

        yield return SpawnPatrolWaveAndWait(h51, h52, stage5Speed);

        yield return SpawnPatrolWaveAndWait(b61, b62, stage6Speed);

        if (mission != null) mission.CompleteObjective();

        onComplete?.Invoke();
    }

    private Vector3 TileCenter(string objectName)
    {
        var go = GameObject.Find(objectName);
        if (go == null)
        {
            Debug.LogWarning("BasicTrainingRange: waypoint not found: " + objectName);
            return transform.position;
        }

        var rend = go.GetComponent<Renderer>();
        return rend != null ? rend.bounds.center : go.transform.position;
    }

    private IEnumerator SpawnIdleWaveAndWait(Vector3[] points)
    {
        alive.Clear();
        foreach (var p in points)
        {
            alive.Add(SpawnRobot(p, Quaternion.identity, false, Vector3.zero, Vector3.zero, 0f));
        }
        yield return WaitUntilAllDead();
    }

    private IEnumerator SpawnPatrolWaveAndWait(Vector3 a, Vector3 b, float speed)
    {
        alive.Clear();
        alive.Add(SpawnRobot(a, FaceTowards(a, b), true, a, b, speed));
        alive.Add(SpawnRobot(b, FaceTowards(b, a), true, b, a, speed));
        yield return WaitUntilAllDead();
    }

    private static Quaternion FaceTowards(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;
        dir.y = 0f;
        return dir.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(dir) : Quaternion.identity;
    }

    private RobotTarget SpawnRobot(Vector3 position, Quaternion rotation, bool patrol, Vector3 pointA, Vector3 pointB, float speed)
    {
        var go = Instantiate(robotPrefab, position, rotation);

        var target = go.GetComponent<RobotTarget>();
        if (target == null) target = go.AddComponent<RobotTarget>();
        target.hitsToKill = hitsToKill;

        if (go.GetComponent<Collider>() == null)
        {
            var collider = go.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0f, 0.88f, 0f);
            collider.height = 1.76f;
            collider.radius = 0.35f;
        }

        if (patrol)
        {
            var p = go.AddComponent<RobotPatrol>();
            p.pointA = pointA;
            p.pointB = pointB;
            p.speed = speed;
        }
        else
        {
            go.AddComponent<RobotIdleAnimator>();
        }

        return target;
    }

    private IEnumerator WaitUntilAllDead()
    {
        while (alive.Exists(r => r != null)) yield return null;
    }
}
