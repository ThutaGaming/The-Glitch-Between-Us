using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Akila.FPSFramework.UI
{
    [AddComponentMenu("Akila/FPS Framework/UI/Slider To TMPro")]
    [RequireComponent(typeof(Slider))]
    [ExecuteAlways]
    public class SliderToTMPro : MonoBehaviour
    {
        [Tooltip("Number of decimal places to display (ignored if wholeNumbers is true)")]
        [Range(0, 6)]
        public int decimalPrecision = 2;

        private Slider targetSlider;
        private TextMeshProUGUI targetText;
        private string formatString;

        private void Awake()
        {
            CacheReferences();
            UpdateFormatString();
        }

        private void OnEnable()
        {
            CacheReferences();
            UpdateFormatString();

            if (targetSlider)
                targetSlider.onValueChanged.AddListener(UpdateText);

            UpdateText(targetSlider?.value ?? 0f);
        }

        private void OnDisable()
        {
            if (targetSlider)
                targetSlider.onValueChanged.RemoveListener(UpdateText);
        }

        private void CacheReferences()
        {
            if (!targetSlider)
                targetSlider = GetComponent<Slider>();

            if (!targetText)
                targetText = GetComponentInChildren<TextMeshProUGUI>();

            if (!targetSlider)
                Debug.LogWarning($"{nameof(SliderToTMPro)}: Missing Slider component on {gameObject.name}", this);

            if (!targetText)
                Debug.LogWarning($"{nameof(SliderToTMPro)}: No TextMeshProUGUI found in children of {gameObject.name}", this);
        }

        private void UpdateFormatString()
        {
            formatString = "F" + Mathf.Clamp(decimalPrecision, 0, 6);
        }

        private void UpdateText(float value)
        {
            if (!targetText || !targetSlider)
                return;

            if (targetSlider.wholeNumbers)
                targetText.text = ((int)value).ToString();
            else
                targetText.text = value.ToString(formatString);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Make it update in editor without pressing Play
            CacheReferences();
            UpdateFormatString();
            if (targetSlider)
                UpdateText(targetSlider.value);
        }
#endif
    }
}
