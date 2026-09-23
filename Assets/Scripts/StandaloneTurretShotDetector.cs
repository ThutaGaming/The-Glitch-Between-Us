using UnityEngine;
using InfimaGames.LowPolyShooterPack;
using UnityEngine.SceneManagement;

/// <summary>
/// Lets the player's gunfire damage StandaloneTurretEnemy and StandaloneMechEnemy targets and draws
/// their health bars - the standalone-scene equivalent of CombatEncounterManager2's player-shot
/// registration and DrawHealthBar, but a plain raycast is enough here since both are solid,
/// collider-bearing geometry (no animated-body line-segment test the way EnemyAI2 needs). Detects a
/// shot the same way CombatEncounterManager2 does - watching the equipped weapon's ammo count drop
/// between frames - so it doesn't care which weapon or fire mode the player is using.
/// </summary>
public class StandaloneTurretShotDetector : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 500f;

    private Character player;
    private InventoryBehaviour inventory;
    private Transform cameraTransform;
    private Camera mainCamera;
    private int lastAmmo = -1;
    private GUIStyle labelStyle;

    private void Awake()
    {
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo == null) return;

        player = playerGo.GetComponent<Character>();
        if (player == null) return;

        inventory = player.GetInventory();
        cameraTransform = player.GetCameraWorld().transform;
        mainCamera = cameraTransform.GetComponent<Camera>();
    }

    private void Start()
    {
        if (!SceneManager.GetActiveScene().path.EndsWith("Level 4.unity")) return;

        // Level 4 keeps the Robot_Soldier_White instances exactly where artists placed them.
        // Combat components are added at runtime so the scene does not need fragile prefab overrides.
        foreach (Transform candidate in FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (candidate.GetComponent<Animator>() == null) continue;

            if (candidate.name.StartsWith("Robot_Soldier_White")
                && candidate.GetComponent<RobotSoldierWhiteEnemy>() == null)
                candidate.gameObject.AddComponent<RobotSoldierWhiteEnemy>();

            bool isBlueRobot = candidate.name.StartsWith("Ball Robot Blue")
                || candidate.name.StartsWith("Hermit Robot Blue")
                || candidate.name.StartsWith("Blast Robot Blue");
            if (isBlueRobot)
            {
                BlueRobotEnemy blueRobot = candidate.GetComponent<BlueRobotEnemy>();
                if (blueRobot == null)
                    blueRobot = candidate.gameObject.AddComponent<BlueRobotEnemy>();
                blueRobot.enabled = true;
            }
        }
    }

    private void Update()
    {
        if (inventory == null) return;

        WeaponBehaviour equipped = inventory.GetEquipped();
        if (equipped == null)
        {
            lastAmmo = -1;
            return;
        }

        int current = equipped.GetAmmunitionCurrent();
        if (lastAmmo >= 0 && current < lastAmmo) TryRegisterShot();
        lastAmmo = current;
    }

    private void TryRegisterShot()
    {
        if (cameraTransform == null) return;
        if (!Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit,
            raycastDistance, ~0, QueryTriggerInteraction.Ignore)) return;

        var turret = hit.collider.GetComponentInParent<StandaloneTurretEnemy>();
        if (turret != null && !turret.IsDead)
        {
            // A crossFloor turret (one that also shoots down at other floors) can be shot back from
            // anywhere; an ordinary one only takes damage once the player is on its own floor.
            bool sameFloor = Mathf.Abs(player.transform.position.y - turret.transform.position.y) <= turret.MaxHeightDifference;
            if (turret.CrossFloor || sameFloor) turret.RegisterHit(hit.point);
            return;
        }

        var mech = hit.collider.GetComponentInParent<StandaloneMechEnemy>();
        if (mech != null && !mech.IsDead) mech.RegisterHit(hit.point);

        var soldier = hit.collider.GetComponentInParent<RobotSoldierWhiteEnemy>();
        if (soldier != null && !soldier.IsDead) soldier.RegisterHit(hit.point);

        var blueSoldier = hit.collider.GetComponentInParent<RobotSoldierBlueEnemy>();
        if (blueSoldier != null && !blueSoldier.IsDead) blueSoldier.RegisterHit(hit.point);

        var blueRobot = hit.collider.GetComponentInParent<BlueRobotEnemy>();
        if (blueRobot != null && !blueRobot.IsDead) blueRobot.RegisterHit(hit.point);
    }

    private bool HasLineOfSight(Vector3 to)
    {
        Vector3 from = cameraTransform.position;
        Vector3 d = to - from;
        float len = d.magnitude;
        if (len < 0.01f) return true;

        var hits = Physics.RaycastAll(from, d / len, len, ~0, QueryTriggerInteraction.Ignore);
        foreach (var h in hits)
        {
            if (h.collider.transform.root == player.transform.root) continue;
            if (h.collider.GetComponentInParent<StandaloneTurretEnemy>() != null) continue;
            if (h.collider.GetComponentInParent<StandaloneMechEnemy>() != null) continue;
            if (h.collider.GetComponentInParent<RobotSoldierWhiteEnemy>() != null) continue;
            if (h.collider.GetComponentInParent<RobotSoldierBlueEnemy>() != null) continue;
            if (h.collider.GetComponentInParent<BlueRobotEnemy>() != null) continue;
            return false;
        }
        return true;
    }

    private void OnGUI()
    {
        if (mainCamera == null) return;

        foreach (var t in FindObjectsByType<StandaloneTurretEnemy>(FindObjectsSortMode.None))
        {
            if (t.IsDead) continue;
            bool recentlyHit = Time.time - t.LastHitTime < 2.5f;
            if (!recentlyHit && !HasLineOfSight(t.BarAnchor)) continue;

            DrawHealthBar(t.BarAnchor, (float)t.Health / Mathf.Max(1, t.MaxHealth));
        }

        foreach (var m in FindObjectsByType<StandaloneMechEnemy>(FindObjectsSortMode.None))
        {
            if (m.IsDead) continue;
            bool recentlyHit = Time.time - m.LastHitTime < 2.5f;
            if (!recentlyHit && !HasLineOfSight(m.BarAnchor)) continue;

            DrawHealthBar(m.BarAnchor, (float)m.Health / Mathf.Max(1, m.MaxHealth));
        }

        foreach (var soldier in FindObjectsByType<RobotSoldierWhiteEnemy>(FindObjectsSortMode.None))
        {
            if (soldier.IsDead) continue;
            bool recentlyHit = Time.time - soldier.LastHitTime < 2.5f;
            if (!recentlyHit && !HasLineOfSight(soldier.BarAnchor)) continue;

            DrawHealthBar(soldier.BarAnchor, (float)soldier.Health / Mathf.Max(1, soldier.MaxHealth));
        }

        foreach (var blueSoldier in FindObjectsByType<RobotSoldierBlueEnemy>(FindObjectsSortMode.None))
        {
            if (blueSoldier.IsDead) continue;
            bool recentlyHit = Time.time - blueSoldier.LastHitTime < 2.5f;
            if (!recentlyHit && !HasLineOfSight(blueSoldier.BarAnchor)) continue;

            DrawHealthBar(blueSoldier.BarAnchor,
                (float)blueSoldier.Health / Mathf.Max(1, blueSoldier.MaxHealth));
        }

        foreach (var blueRobot in FindObjectsByType<BlueRobotEnemy>(FindObjectsSortMode.None))
        {
            if (blueRobot.IsDead) continue;
            bool recentlyHit = Time.time - blueRobot.LastHitTime < 2.5f;
            if (!recentlyHit && !HasLineOfSight(blueRobot.BarAnchor)) continue;

            DrawHealthBar(blueRobot.BarAnchor,
                (float)blueRobot.Health / Mathf.Max(1, blueRobot.MaxHealth));
        }
    }

    private void DrawHealthBar(Vector3 worldPoint, float pct)
    {
        Vector3 sp = mainCamera.WorldToScreenPoint(worldPoint);
        if (sp.z <= 0f) return;

        var tex = Texture2D.whiteTexture;
        const float width = 76f;
        const float h = 7f;
        float x = sp.x - width * 0.5f;
        float y = Screen.height - sp.y - h;
        pct = Mathf.Clamp01(pct);

        GUI.color = new Color(0f, 0f, 0f, 0.65f);
        GUI.DrawTexture(new Rect(x, y, width, h), tex);
        GUI.color = Color.Lerp(new Color(0.95f, 0.2f, 0.2f), new Color(0.3f, 0.9f, 0.4f), pct);
        GUI.DrawTexture(new Rect(x + 1f, y + 1f, (width - 2f) * pct, h - 2f), tex);
        GUI.color = Color.white;
    }
}
