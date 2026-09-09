using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace Akila.FPSFramework.UI
{
    [AddComponentMenu("Akila/FPS Framework/UI/Carousel Selector")]
    public class CarouselSelector : MonoBehaviour
    {
        public List<string> options = new List<string>();
        public int value = 0;

        public TextMeshProUGUI label;
        public Button rightButton;
        public Button leftButton;
        public UnityEvent<int> onValueChange;


        private void Start()
        {
            
        }

        private void OnEnable()
        {
            rightButton?.onClick.AddListener(GoRight);
            leftButton?.onClick.AddListener(GoLeft);
        }

        private void OnDisable()
        {
            rightButton?.onClick.RemoveAllListeners();
            leftButton?.onClick.RemoveAllListeners();
        }

        private void GoRight()
        {
            value += 1;

            if (value > options.Count - 1) value = 0;

            onValueChange?.Invoke(value);
        }

        private void GoLeft()
        {
            value -= 1;

            if (value < 0) value = options.Count - 1;

            onValueChange?.Invoke(value);
        }

        private void Update()
        {
            if (value < 0) value = options.Count - 1;
            if (value > options.Count - 1) value = 0;

            value = Mathf.Clamp(value, 0, options.Count - 1);

            if(label == null)
            {
                Debug.LogError("Label is not set.", gameObject);
            }
            else
            {
                int value = this.value;

                if (value < 0) value = 0;
                if (options.Count >= 1 && value > options.Count - 1) value = options.Count - 1;

                if(options.Count >= value)
                {
                    label.text = options[value];
                }
                else
                {
                    Debug.LogError($"{gameObject} Label can't show current option. Value is not in range of the added Options.", gameObject);
                }
            }
        }

        public void AddOptions(string[] options)
        {
            this.options.AddRange(options);
        }

        public void ClearOptions()
        {
            options.Clear();
        }
    }
}
