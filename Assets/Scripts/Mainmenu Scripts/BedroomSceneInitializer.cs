using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BedroomSceneInitializer : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fadeOverlay;       // Fullscreen Black Overlay
    [SerializeField] private Image loadingSplashImage; // Picture / Logo for loading screen
    [SerializeField] private Sprite loadingSprite;    // Picture to display during loading

    [Header("Timings")]
    [SerializeField] private float displayLoadingTime = 1.5f; // Time to show the picture
    [SerializeField] private float fadeDuration = 1.0f;

    private void Start()
    {
        StartCoroutine(PlayBedroomIntro());
    }

    private IEnumerator PlayBedroomIntro()
    {
        // 1. Ensure screen starts fully black & overlay blocks inputs
        fadeOverlay.gameObject.SetActive(true);
        fadeOverlay.raycastTarget = true;
        SetImageAlpha(fadeOverlay, 1f);

        // 2. Set up loading screen image & fade it in over the black screen
        if (loadingSplashImage != null && loadingSprite != null)
        {
            loadingSplashImage.sprite = loadingSprite;
            loadingSplashImage.gameObject.SetActive(true);
            yield return StartCoroutine(FadeImage(loadingSplashImage, 0f, 1f));

            // Keep loading image visible on screen
            yield return new WaitForSeconds(displayLoadingTime);

            // Fade loading image out back to black background
            yield return StartCoroutine(FadeImage(loadingSplashImage, 1f, 0f));
            loadingSplashImage.gameObject.SetActive(false);
        }

        // 3. Fade black overlay out to reveal the Bedroom Scene
        yield return StartCoroutine(FadeImage(fadeOverlay, 1f, 0f));

        // 4. Disable raycast so user can play
        fadeOverlay.raycastTarget = false;
        fadeOverlay.gameObject.SetActive(false);
    }

    private IEnumerator FadeImage(Image img, float startAlpha, float targetAlpha)
    {
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

    private void SetImageAlpha(Image img, float alpha)
    {
        Color col = img.color;
        col.a = alpha;
        img.color = col;
    }
}