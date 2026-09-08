using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // The exact name of your starting scene
    [SerializeField] private string newGameSceneName = "bedroom scene";

    public void NewGame()
    {
        // Optional: Reset player data here if needed before starting a fresh run
        
        // Load the bedroom scene
        SceneManager.LoadScene(newGameSceneName);
    }

    public void Continue()
    {
        // Place loading logic here when ready (e.g., loading saved scene/data)
        Debug.Log("Continue button pressed.");
    }

    public void OpenSettings()
    {
        // Place menu panel toggle logic here
        Debug.Log("Settings button pressed.");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}