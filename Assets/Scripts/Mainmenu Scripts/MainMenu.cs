using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string newGameSceneName = "bedroom scene";

    [Header("UI References")]
    [SerializeField] private Button continueButton;

    private const string SaveKey = SaveCheckpoint.SceneKey;

    private void Start()
    {
        // Continue only works when there is a save; grey it out otherwise.
        if (continueButton == null) return;
        bool hasSave = PlayerPrefs.HasKey(SaveKey);
        continueButton.interactable = hasSave;
        // The button's own tint only reaches its target graphic, not the label on top of it.
        foreach (var label in continueButton.GetComponentsInChildren<Graphic>(true))
        {
            if (label == continueButton.targetGraphic) continue;
            var c = label.color;
            c.a = hasSave ? 1f : 0.35f;
            label.color = c;
        }
    }

    public void NewGame()
    {
        // Save the initial starting scene (and forget any later visit of it)
        SaveCheckpoint.SaveNewGame(newGameSceneName);

        // Fade to black first when the splash loader is there to do it.
        var loader = FindFirstObjectByType<SplashAndSceneLoader>();
        if (loader != null) loader.OnNewGamePressed();
        else SceneManager.LoadScene(newGameSceneName);
    }

    public void Continue()
    {
        // Load whatever scene name is stored in PlayerPrefs
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string savedScene = PlayerPrefs.GetString(SaveKey);
            // Bedroom/School are visited more than once - resume the right visit, not the morning one.
            SaveCheckpoint.RestoreVariantForNextLoad();
            SceneManager.LoadScene(savedScene);
        }
        else
        {
            Debug.LogWarning("No save data found!");
        }
    }

    public void OpenSettings()
    {
        // The settings page is IMGUI, which uGUI can't see: switch the menu's clicks off while it's
        // open so a slider drag can't also press a button underneath.
        var events = EventSystem.current;
        if (events != null) events.enabled = false;
        PauseMenu.OpenSettings(() => { if (events != null) events.enabled = true; });
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    // Static helper method to auto-save any scene currently active
    public static void SaveCurrentScene() => SaveCheckpoint.SaveCurrentScene();
}