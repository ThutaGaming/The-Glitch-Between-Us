using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/UI/Hitmarker")]
    public class Hitmarker : MonoBehaviour
    {
        public float fadeSpeed = 10;
        public float fadeDelay = 0.1f;
        public CanvasGroup hitmakerObject;
        public Color normal = Color.white;
        public Color highlight = Color.red;
        public AudioProfile hitmarkerSound;

        [Header("Movement")]
        public float maxSize = 20;
        public float minSize = 10;
        public float rotaionAmount = 10;

        private RawImage[] images;
        private RectTransform RectTransform;

        private float fadeTimer;
        private Audio hitmarkAudio;

        private void Awake()
        {
            RectTransform = hitmakerObject.gameObject.GetComponent<RectTransform>();

            images = GetComponentsInChildren<RawImage>();

            hitmakerObject.alpha = 0;

            fadeTimer = 0;


            if (hitmarkerSound)
            {
                hitmarkAudio = new Audio();

                hitmarkAudio.Setup(gameObject, hitmarkerSound);
            }
        }

        private void Update()
        {
            Vector3 scale = new Vector3(minSize, minSize, minSize);
            RectTransform.sizeDelta = Vector3.Lerp(RectTransform.sizeDelta, scale, Time.deltaTime * 30);
            RectTransform.rotation = Quaternion.Slerp(RectTransform.rotation, Quaternion.identity, Time.deltaTime * 8);

            if (fadeTimer > 0)
                fadeTimer -= Time.deltaTime;

            if (fadeTimer <= 0)
                Disable();
        }

        public void Show(bool highlight)
        {
            if (highlight)
            {
                foreach (RawImage image in images)
                {
                    image.color = this.highlight;
                }
            }
            else
            {
                foreach (RawImage image in images)
                {
                    image.color = this.normal;
                }
            }

            hitmakerObject.alpha = 1;
            fadeTimer = fadeDelay;

            hitmarkAudio?.Play(true);

            ApplyMovement();
        }

        public void ApplyMovement()
        {
            float scale = Random.Range(minSize, maxSize);

            Vector3 rotation = new Vector3(0, 0, Random.Range(-rotaionAmount, rotaionAmount));

            RectTransform.sizeDelta = new Vector3(scale, scale, scale);

            transform.Rotate(rotation);
        }

        private void Disable()
        {
            hitmakerObject.alpha = Mathf.Lerp(hitmakerObject.alpha, 0, Time.deltaTime * fadeSpeed);
        }
    }
}