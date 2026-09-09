using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/UI/Kill Tag")]
    public class KillTag : MonoBehaviour
    {
        public KillFeedTagType type;
        public TextMeshProUGUI message;
        public float time = 2;
        public float fadeSpeed = 10;

        [Header("Message Colors")]
        public Color labelColor = Color.white;
        public Color nameColor = Color.red;

        private float timer;
        private CanvasGroup CanvasGroup;
        private Animator animator;

        private void Start()
        {
            CanvasGroup = gameObject.AddComponent<CanvasGroup>();
            CanvasGroup.alpha = 0;

            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (type == KillFeedTagType.Counter && timer <= 0) CanvasGroup.alpha = Mathf.Lerp(CanvasGroup.alpha, 0, Time.deltaTime * fadeSpeed);
            if ((type == KillFeedTagType.Message || type == KillFeedTagType.Image) && CanvasGroup.alpha <= 0.1f && timer <= 0) Destroy(gameObject, 0.2f);
            if ((type == KillFeedTagType.Message || type == KillFeedTagType.Image) && timer <= 0) CanvasGroup.alpha = Mathf.Lerp(CanvasGroup.alpha, 0, Time.deltaTime * fadeSpeed);
            timer -= Time.deltaTime;

            if (timer > 0) CanvasGroup.alpha = Mathf.Lerp(CanvasGroup.alpha, 1, Time.deltaTime * fadeSpeed);
        }

        public void Show(int killsCount, string victimName)
        {
            int kills = killsCount;

            string s = kills > 1 ? "s" : "";
            if (type == KillFeedTagType.Counter) message.text = $"{kills} Kill{s}";
            timer = time;

            if (type == KillFeedTagType.Message && message)
            {
                timer = time;
                message.text = $"<color=#{ColorUtility.ToHtmlStringRGB(labelColor)}>Eliminated <color=#{ColorUtility.ToHtmlStringRGB(nameColor)}>{victimName}";
            }
            if (animator) animator.Play("Show", 0, 0);
        }


        public enum KillFeedTagType
        {
            Counter,
            Message,
            Image
        }
    }
}