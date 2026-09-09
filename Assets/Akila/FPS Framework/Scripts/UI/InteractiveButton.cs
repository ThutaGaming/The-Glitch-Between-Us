using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Akila.FPSFramework
{
    public class InteractiveButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public bool interactable = true;
        public Graphic targetGraphics;
        public TextMeshProUGUI targetText;
        public float fadeDuration = 0.1f;

        [Header("Graphics Colors")]
        public Color normalGraphicsColor = Color.black;
        public Color highlightedGraphicsColor = Color.white;
        public Color selectedGraphicsColor = Color.gray;
        public Color disabledGraphicsColor = Color.red;

        [Header("Text Colors")]
        public Color normalTextColor = Color.white;
        public Color highlightedTextColor = Color.black;
        public Color selectedTextColor = Color.black;
        public Color disabledTextColor = Color.black;

        [Header("Audio")]
        public AudioProfile highlightSound;
        public AudioProfile selectSound;

        [Space]
        public UnityEvent onClick;

        private Color currentGraphicsColor;
        private Color currentTextColor;

        private Audio highlightAudio;
        private Audio selectAudio;

        public bool isHighlighted { get; private set; }
        public bool isPressed { get; private set; }

        private Color targetTextColor;
        private Color targetGraphicsColor;

        private void Awake()
        {
            highlightAudio = new Audio();
            selectAudio = new Audio();

            highlightAudio.Setup(gameObject, highlightSound);
            selectAudio.Setup(gameObject, selectSound);
        }

        protected void Update()
        {
            if (targetGraphics == null) return;

            if (interactable)
            {
                if (isHighlighted && !isPressed)
                {
                    currentGraphicsColor = highlightedGraphicsColor;
                    currentTextColor = highlightedTextColor;
                }

                if (isHighlighted && isPressed)
                {
                    currentGraphicsColor = selectedGraphicsColor;
                    currentTextColor = selectedTextColor;
                }

                if (!isHighlighted && !isPressed)
                {
                    currentGraphicsColor = normalGraphicsColor;
                    currentTextColor = normalTextColor;
                }
            }
            else
            {
                currentGraphicsColor = disabledGraphicsColor;
                currentTextColor = disabledTextColor;
            }

            targetGraphicsColor = Color.Lerp(targetGraphics.color, currentGraphicsColor, Time.unscaledDeltaTime / fadeDuration);
            targetTextColor = Color.Lerp(targetText.color, currentTextColor, Time.unscaledDeltaTime / fadeDuration);

            if (targetGraphics.color != targetGraphicsColor)
                targetGraphics.color = targetGraphicsColor;

            if (targetText.color != targetTextColor)
                targetText.color = targetTextColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (interactable == false) return;

            highlightAudio.Play();

            isHighlighted = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (interactable == false) return;

            selectAudio.Play();

            isPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (interactable == false) return;

            if (isHighlighted)
                onClick?.Invoke();

            isPressed = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (interactable == false) return;

            isHighlighted = false;
        }

    }
}