using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashAndSceneLoader : MonoBehaviour
{
    [Header("Splash Screen References")]
    [SerializeField] private Image fadeOverlay;            // Full-screen black image
    [SerializeField] private Image splashLogo;             // UI Image for the logos
    [SerializeField] private Sprite logo1;                  // First logo sprite
    [SerializeField] private Sprite logo2;                  // Second logo sprite
    [SerializeField] private CanvasGroup mainMenuCanvasGroup; // CanvasGroup attached to your Main Menu UI panel

    [Header("Timings")]
    [SerializeField] private float fadeDuration = 1.0f;     // Speed of fade transitions
    [SerializeField] private float logoDisplayTime = 1.5f;  // How long each logo stays on screen

    [Header("Scene Settings")]
    [SerializeField] private string bedroomSceneName = "bedroom scene"; // Target scene name

    private void Start()
    {
        StartCoroutine(PlaySplashSequence());
    }

    /// <summary>
    /// Handles the intro sequence: Logo 1 -> Logo 2 -> Main Menu Fade In
    /// </summary>
    private IEnumerator PlaySplashSequence()
    {
        // 1. Block mouse clicks while splash sequence plays
        fadeOverlay.gameObject.SetActive(true);
        fadeOverlay.raycastTarget = true;

        if (splashLogo != null)
        {
            splashLogo.gameObject.SetActive(true);
            splashLogo.raycastTarget = false; // Prevent logo image from blocking raycasts
        }

        // Hide Main Menu initially
        if (mainMenuCanvasGroup != null)
        {
            mainMenuCanvasGroup.alpha = 0f;
            mainMenuCanvasGroup.interactable = false;
            mainMenuCanvasGroup.blocksRaycasts = false;
        }

        // --- PLAY LOGO 1 ---
        if (logo1 != null)
        {
            splashLogo.sprite = logo1;
            yield return StartCoroutine(FadeImage(splashLogo, 0f, 1f));
            yield return new WaitForSeconds(logoDisplayTime);
            yield return StartCoroutine(FadeImage(splashLogo, 1f, 0f));
        }

        // --- PLAY LOGO 2 ---
        if (logo2 != null)
        {
            splashLogo.sprite = logo2;
            yield return StartCoroutine(FadeImage(splashLogo, 0f, 1f));
            yield return new WaitForSeconds(logoDisplayTime);
            yield return StartCoroutine(FadeImage(splashLogo, 1f, 0f));
        }

        // Disable splash logo graphic now that logos are finished
        if (splashLogo != null)
        {
            splashLogo.gameObject.SetActive(false);
        }

        // --- FADE OUT BLACK OVERLAY & FADE IN MAIN MENU ---
        StartCoroutine(FadeImage(fadeOverlay, 1f, 0f));
        yield return StartCoroutine(FadeCanvasGroup(mainMenuCanvasGroup, 0f, 1f));

        // Enable Main Menu interaction
        if (mainMenuCanvasGroup != null)
        {
            mainMenuCanvasGroup.interactable = true;
            mainMenuCanvasGroup.blocksRaycasts = true;
        }

        // UNBLOCK BUTTON CLICKS: Turn off raycasting on the black overlay so buttons can be clicked
        fadeOverlay.raycastTarget = false;
    }

    /// <summary>
    /// Called by the 'New Game' button OnClick event
    /// </summary>
    public void OnNewGamePressed()
    {
        StartCoroutine(TransitionToBedroom());
    }

    /// <summary>
    /// Fades the Main Menu to black, then loads the Bedroom Scene
    /// </summary>
    private IEnumerator TransitionToBedroom()
    {
        // Re-enable raycast overlay to block accidental double-clicks while fading
        fadeOverlay.raycastTarget = true;

        // Disable Main Menu UI interaction
        if (mainMenuCanvasGroup != null)
        {
            mainMenuCanvasGroup.interactable = false;
            mainMenuCanvasGroup.blocksRaycasts = false;
        }

        // Fade screen to solid black
        yield return StartCoroutine(FadeImage(fadeOverlay, 0f, 1f));

        // Load the Bedroom Scene
        SceneManager.LoadScene(bedroomSceneName);
    }

    // --- HELPER FADE COROUTINES ---

    private IEnumerator FadeImage(Image img, float startAlpha, float targetAlpha)
    {
        if (img == null) yield break;

        float timer = 0f;
        Color col = img.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            col.a = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            img.color = col;
            yield return null;
        }

        col.a = targetAlpha;
        img.color = col;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float startAlpha, float targetAlpha)
    {
        if (cg == null) yield break;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }

        cg.alpha = targetAlpha;
    }
}