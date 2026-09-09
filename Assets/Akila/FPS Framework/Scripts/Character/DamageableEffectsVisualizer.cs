using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Akila.FPSFramework
{
    public class DamageableEffectsVisualizer : MonoBehaviour
    {
        public CanvasGroup effectCanvas;
        public RawImage effectImage;
        public Color damageColor = Color.red;
        public Color healColor = Color.blue;
        public float fadeSpeed = 1;

        public static DamageableEffectsVisualizer instance;

        private void Start()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            if (effectCanvas == null)
            {
                Debug.LogError("'Effect Canvas' is not assgined. Trying to find it.", gameObject);

                effectCanvas = this.SearchFor<CanvasGroup>(false, true);
            }
            else
            {
                effectCanvas.alpha = 0;
            }
        }

        private void Update()
        {
            effectCanvas.alpha = Mathf.Lerp(effectCanvas.alpha, 0, Time.deltaTime * fadeSpeed * 2);
        }

        public void TriggerDamageEffect()
        {
            if (effectImage)
                effectImage.color = damageColor;

            TriggerLastState();
        }

        public void TriggerHealingEffect()
        {
            if (effectImage)
                effectImage.color = healColor;

            TriggerLastState();
        }

        public void TriggerLastState()
        {
            if (effectCanvas == null)
            {
                Debug.LogError("'Effect Canvas' is null. Can't trigger effects.");

                return;
            }

            effectCanvas.alpha = 1;
        }
    }
}