using UnityEngine;

public class SceneAutoSaver : MonoBehaviour
{
    private void Start()
    {
        // Automatically save the scene as soon as it loads
        MainMenu.SaveCurrentScene();
    }
}