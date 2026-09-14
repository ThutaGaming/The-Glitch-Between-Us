using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// When PlayerHealth hits zero: freezes the player's controls, slows time, shows "YOU DIED"
/// and reloads the current scene. IMGUI text stays English (IMGUI can't shape Burmese).
/// </summary>
[RequireComponent(typeof(PlayerHealth))]
public class PlayerDeathScreen : MonoBehaviour
{
    [SerializeField] private float restartDelay = 3f;

    private PlayerHealth health;
    private float deathTime = -1f;
    private GUIStyle titleStyle, subStyle;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        health.Died += OnDied;
    }

    private void OnDestroy()
    {
        if (health != null) health.Died -= OnDied;
    }

    private void OnDied()
    {
        deathTime = Time.unscaledTime;

        var input = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (input != null) input.DeactivateInput();
        foreach (var mb in GetComponentsInChildren<MonoBehaviour>())
        {
            string ns = mb != null ? mb.GetType().Namespace : null;
            if (ns != null && ns.StartsWith("InfimaGames")) mb.enabled = false;
        }

        Time.timeScale = 0.35f;
        StartCoroutine(RestartAfterDelay());
    }

    private IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSecondsRealtime(restartDelay);
        Time.timeScale = 1f;

        var scene = SceneManager.GetActiveScene();
#if UNITY_EDITOR
        if (scene.buildIndex < 0)
        {
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(scene.path, new LoadSceneParameters(LoadSceneMode.Single));
            yield break;
        }
#endif
        SceneManager.LoadScene(scene.buildIndex);
    }

    private void OnGUI()
    {
        if (deathTime < 0f) return;

        float t = Mathf.Clamp01((Time.unscaledTime - deathTime) / 1.2f);
        var prev = GUI.color;
        GUI.color = new Color(0.25f, 0f, 0f, 0.75f * t);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);

        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 72, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            subStyle = new GUIStyle(GUI.skin.label) { fontSize = 22, alignment = TextAnchor.MiddleCenter };
        }

        GUI.color = new Color(1f, 0.25f, 0.2f, t);
        GUI.Label(new Rect(0, Screen.height * 0.5f - 70f, Screen.width, 100f), "YOU DIED", titleStyle);
        GUI.color = new Color(1f, 1f, 1f, 0.8f * t);
        GUI.Label(new Rect(0, Screen.height * 0.5f + 20f, Screen.width, 40f), "Restarting...", subStyle);
        GUI.color = prev;
    }
}
