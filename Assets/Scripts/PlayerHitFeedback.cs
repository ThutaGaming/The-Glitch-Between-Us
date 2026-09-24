using UnityEngine;

/// <summary>
/// Screen-edge red flash and a directional "where did that come from" arrow when the player takes a
/// hit - the same look CombatEncounterManager2 draws for its squad fights (Level 1), reimplemented
/// as a standalone component so Level 4's manager-less enemies (StandaloneTurretEnemy,
/// RobotSoldierWhiteEnemy, BlueRobotEnemy, RobotSoldierBlueEnemy) can trigger it too.
/// </summary>
public class PlayerHitFeedback : MonoBehaviour
{
    private Camera mainCamera;
    private Transform cameraTransform;
    private float damageFlash;
    private Vector3 damageFrom;
    private float damageDirTime = -99f;

    private void Awake()
    {
        mainCamera = Camera.main;
        cameraTransform = mainCamera != null ? mainCamera.transform : null;
    }

    /// <summary>Call when a hit lands on the player; fromPosition is the attacker's world position.</summary>
    public void Notify(Vector3 fromPosition)
    {
        damageFlash = 1f;
        damageFrom = fromPosition;
        damageDirTime = Time.time;
    }

    private void Update()
    {
        if (damageFlash > 0f) damageFlash -= Time.deltaTime * 1.4f;
    }

    private static Vector3 Flat(Vector3 v)
    {
        v.y = 0f;
        return v.sqrMagnitude > 0.0001f ? v.normalized : Vector3.forward;
    }

    private void OnGUI()
    {
        if (cameraTransform == null)
        {
            if (mainCamera == null) mainCamera = Camera.main;
            cameraTransform = mainCamera != null ? mainCamera.transform : null;
            if (cameraTransform == null) return;
        }

        var prev = GUI.color;
        var tex = Texture2D.whiteTexture;

        if (damageFlash > 0f)
        {
            GUI.color = new Color(0.55f, 0f, 0f, damageFlash * 0.28f);
            float edge = Screen.height * 0.12f;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, edge), tex);
            GUI.DrawTexture(new Rect(0, Screen.height - edge, Screen.width, edge), tex);
            GUI.DrawTexture(new Rect(0, 0, edge, Screen.height), tex);
            GUI.DrawTexture(new Rect(Screen.width - edge, 0, edge, Screen.height), tex);
        }

        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        float dirAge = Time.time - damageDirTime;
        if (dirAge < 1.2f)
        {
            Vector3 camFwd = Flat(cameraTransform.forward);
            Vector3 toAttacker = Flat(damageFrom - cameraTransform.position);
            float angle = Vector3.SignedAngle(camFwd, toAttacker, Vector3.up);
            var matrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(angle, center);
            GUI.color = new Color(1f, 0.15f, 0.1f, 0.85f * (1f - dirAge / 1.2f));
            GUI.DrawTexture(new Rect(center.x - 45f, center.y - 170f, 90f, 12f), tex);
            GUI.DrawTexture(new Rect(center.x - 12f, center.y - 182f, 24f, 12f), tex);
            GUI.matrix = matrix;
        }

        GUI.color = prev;
    }
}
