using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string newGameSceneName = "bedroom scene";

    [Header("UI References")]
    [SerializeField] private Button continueButton;

    private const string SaveKey = "SavedScene";

    private void Start()
    {
        // Check if save data exists when opening the Main Menu
        if (PlayerPrefs.HasKey(SaveKey))
        {
            if (continueButton != null)
                continueButton.interactable = true;
        }
        else
        {
            // Disable Continue button if no save exists
            if (continueButton != null)
                continueButton.interactable = false;
        }
    }

    public void NewGame()
    {
        // Save the initial starting scene
        PlayerPrefs.SetString(SaveKey, newGameSceneName);
        PlayerPrefs.Save();
        
        // Load the bedroom scene
        SceneManager.LoadScene(newGameSceneName);
    }

    public void Continue()
    {
        // Load whatever scene name is stored in PlayerPrefs
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string savedScene = PlayerPrefs.GetString(SaveKey);
            SceneManager.LoadScene(savedScene);
        }
        else
        {
            Debug.LogWarning("No save data found!");
        }
    }

    public void OpenSettings()
    {
        Debug.Log("Settings button pressed.");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    // Static helper method to auto-save any scene currently active
    public static void SaveCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString(SaveKey, currentScene);
        PlayerPrefs.Save();
        Debug.Log("Auto-saved scene: " + currentScene);
    }
}