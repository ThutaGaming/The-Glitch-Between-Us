using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/UI/Crosshair"), RequireComponent(typeof(CanvasGroup))]

    public class Crosshair : MonoBehaviour
    {
        public float size = 1;
        public float sizeMatchingTime = 0.1f;

        public Color color = Color.white;
        public RectTransform crosshairHolder;

        [ReadOnly] public Firearm firearm;

        private float amount;
        private float sizeMatchingVel;

        private CanvasGroup canvasGroup;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            if(firearm == null)
            {
                return;
            }

            foreach(Image image in crosshairHolder.GetComponentsInChildren<Image>())
            {
                image.color = color;
            }

            canvasGroup.alpha = Mathf.Lerp(1, 0, firearm.aimProgress * 1.3f);

            amount = Mathf.SmoothDamp(amount, firearm.currentSprayAmount, ref sizeMatchingVel, sizeMatchingTime);   

            crosshairHolder.sizeDelta = Vector2.one * size * amount;
        }
    }
}