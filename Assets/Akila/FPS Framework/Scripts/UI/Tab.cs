using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Akila.FPSFramework.UI
{
    /// <summary>
    /// Represents a tab UI element that can be highlighted and selected. Provides visual feedback for different states.
    /// </summary>
    public class Tab : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
    {
        // Reference to the Image component that represents the graphical part of the tab
        [Tooltip("The Image component that visually represents the tab.")]
        public Image tabImage;

        // Reference to the TextMeshProUGUI component that holds the text of the tab
        [Tooltip("The TextMeshProUGUI component that represents the tab's text.")]
        public TextMeshProUGUI tabText;

        [Tooltip("The content to be shown or hidden. If the tab is selected, the content will be enbaled.")]
        public RectTransform tabContent;

        public Scrollbar scrollbar;

        // Speed at which the color transitions occur
        [Tooltip("Speed at which the colors transition between states.")]
        public float colorTransitionSpeed = 50f;

        [Header("Tab State Colors")]
        // Colors for the tab's graphic and text in different states
        [Tooltip("The default color for the tab's graphic.")]
        public Color defaultColor = Color.white;

        [Tooltip("The color for the tab's graphic when highlighted.")]
        public Color highlightedColor = Color.gray;

        [Tooltip("The color for the tab's graphic when selected.")]
        public Color selectedColor = Color.gray;

        [Header("Text State Colors")]
        // Colors for the tab's text in different states
        [Tooltip("The default color for the tab's text.")]
        public Color defaultTextColor = Color.black;

        [Tooltip("The color for the tab's text when highlighted.")]
        public Color highlightedTextColor = Color.black;

        [Tooltip("The color for the tab's text when selected.")]
        public Color selectedTextColor = Color.green;

        // Unity events triggered when the tab is highlighted or selected
        public UnityEvent<Tab> onTabHighlighted;
        public UnityEvent<Tab> onTabSelected;

        // Flags indicating whether the tab is highlighted or selected
        public bool isHighlighted { get; private set; }
        public bool isSelected { get; private set; }

        // Internal variables to store the target and current colors for smooth transitions
        private Color targetImageColor;
        private Color targetTextColor;
        private Color currentImageColor;
        private Color currentTextColor;

        private VerticalLayoutGroup verticalLayoutGroup;
        private HorizontalLayoutGroup horizontalLayoutGroup;

        /// <summary>
        /// Initializes the Tab component by ensuring required components are present and set.
        /// </summary>
        private void Awake()
        {
            InitializeTabsManager();
            InitializeGraphics();
            InitializeText();
            InitializeComponents();
        }

        /// <summary>
        /// Ensures the TabsManager is present on the parent object.
        /// </summary>
        private void InitializeTabsManager()
        {
            TabsManager tabsManager = GetComponentInParent<TabsManager>();

            if (tabsManager == null)
            {
                tabsManager = transform.parent.gameObject.AddComponent<TabsManager>();
                Debug.LogError("A 'TabsManager' component is required on the parent object of each Tab. One has been automatically added, but it is strongly recommended to manually attach and configure your own 'TabsManager' for optimal control and performance.", gameObject);
            }
        }

        /// <summary>
        /// Ensures the graphics Image component is assigned. If not, tries to find it.
        /// </summary>
        private void InitializeGraphics()
        {
            if (tabImage == null)
            {
                Debug.LogError($"The 'Graphics' property on {gameObject.name} (Tab) has not been assigned. The system is searching for the Image component on the GameObject, as well as its parent and child objects. Please ensure the property is correctly set for proper functionality.", gameObject);
                
                tabImage = this.SearchFor<Image>();

                if (tabImage == null)
                    Debug.LogError($"The 'Graphics' property on {gameObject.name} (Tab) has not been assigned. The system was unable to find the Image component on the GameObject, its parent, or its child objects. Please ensure the property is correctly assigned for proper functionality.", gameObject);
            }
        }

        /// <summary>
        /// Ensures the TextMeshProUGUI component is assigned. If not, tries to find it.
        /// </summary>
        private void InitializeText()
        {
            if (tabText == null)
            {
                Debug.LogError($"The 'Text' property on {gameObject.name} (Tab) has not been assigned. The system is searching for the TextMeshProUGUI component on the GameObject, as well as its parent and child objects. Please ensure the property is correctly set for proper functionality.", gameObject);
                
                tabText = this.SearchFor<TextMeshProUGUI>();

                if (tabText == null)
                    Debug.LogError($"The 'Text' property on {gameObject.name} (Tab) has not been assigned. The system was unable to find the TextMeshProUGUI component on the GameObject, its parent, or its child objects. Please ensure the property is correctly assigned for proper functionality.", gameObject);
            }
        }

        private void InitializeComponents()
        {
            if(tabContent == null)
            {
                return;
            }

            verticalLayoutGroup = tabContent.GetComponent<VerticalLayoutGroup>();
            horizontalLayoutGroup = tabContent.GetComponent<HorizontalLayoutGroup>();
        }

        /// <summary>
        /// Updates the tab's colors smoothly based on its state (highlighted/selected).
        /// </summary>
        private void Update()
        {
            //Enable and Disable tab content
            if (tabContent == null)
            {
                Debug.LogError("TabContent is not set. Make sure to assgin TabContent(RectTransform) in your tabs.", gameObject);

                return;
            }
            else
            {
                tabContent.gameObject.SetActive(isSelected);
            }

            UpdateColorTargets();

            // Smoothly transition to the target colors
            currentImageColor = Color.Lerp(currentImageColor, targetImageColor, colorTransitionSpeed * Time.unscaledDeltaTime);
            currentTextColor = Color.Lerp(currentTextColor, targetTextColor, colorTransitionSpeed * Time.unscaledDeltaTime);

            // Apply the new colors to the UI elements
            if (tabImage != null)
                tabImage.color = currentImageColor;

            if (tabText != null)
                tabText.color = currentTextColor;
        }

        private void RearrangeLayout()
        {
            verticalLayoutGroup?.SetLayoutVertical();
            verticalLayoutGroup?.SetLayoutHorizontal();

            horizontalLayoutGroup?.SetLayoutHorizontal();
            verticalLayoutGroup?.SetLayoutVertical();
        }

        /// <summary>
        /// Determines the target colors based on the current tab state.
        /// </summary>
        private void UpdateColorTargets()
        {
            if (isSelected)
            {
                targetImageColor = selectedColor;
                targetTextColor = selectedTextColor;
            }
            else
            {
                targetImageColor = isHighlighted ? highlightedColor : defaultColor;
                targetTextColor = isHighlighted ? highlightedTextColor : defaultTextColor;
            }
        }

        /// <summary>
        /// Called when the pointer enters the tab (highlight the tab).
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            SetHighlighted(true);
        }

        /// <summary>
        /// Called when the pointer clicks (select the tab).
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            SetSelected(true);
        }

        /// <summary>
        /// Called when the pointer exits the tab (unhighlight the tab).
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            SetHighlighted(false);
        }

        /// <summary>
        /// Sets the tab's highlighted state and triggers the corresponding event.
        /// </summary>
        /// <param name="highlighted">True to highlight the tab, false to remove highlight.</param>
        private void SetHighlighted(bool highlighted)
        {
            isHighlighted = highlighted;
            onTabHighlighted?.Invoke(this);
        }

        /// <summary>
        /// Sets the tab's selected state and triggers the corresponding event.
        /// </summary>
        /// <param name="selected">True to select the tab, false to deselect.</param>
        private void SetSelected(bool selected)
        {
            isSelected = selected;

            if (selected == false) return;


            onTabSelected?.Invoke(this);

            //Reset scrollbar position on selected.
            if (scrollbar != null)
                scrollbar.value = 1;

            if (selected == true)
            {
                RearrangeLayout();
            }
        }

        /// <summary>
        /// Resets the tab's state to its default (unselected, unhighlighted).
        /// Can be called externally to reset the tab's visual state.
        /// </summary>
        public void ResetTabState()
        {
            SetHighlighted(false);
            SetSelected(false);
        }

        /// <summary>
        /// Highlights the tab externally. Call this to highlight the tab from outside scripts.
        /// </summary>
        public void HighlightTab()
        {
            SetHighlighted(true);
        }

        /// <summary>
        /// Selects the tab externally. Call this to select the tab from outside scripts.
        /// </summary>
        public void SelectTab()
        {
            SetSelected(true);
        }

        /// <summary>
        /// Deselects the tab externally. Call this to deselect the tab from outside scripts.
        /// </summary>
        public void DeselectTab()
        {
            SetSelected(false);
        }

        /// <summary>
        /// Unhighlights the tab externally. Call this to remove highlight from the tab from outside scripts.
        /// </summary>
        public void UnhighlightTab()
        {
            SetHighlighted(false);
        }

        /// <summary>
        /// Sets the tab's graphic (Image) color externally. This method can be used to modify the color programmatically.
        /// </summary>
        /// <param name="color">The color to set for the tab's image.</param>
        public void SetTabImageColor(Color color)
        {
            if (tabImage != null)
            {
                tabImage.color = color;
            }
            else
            {
                Debug.LogError("Tab Image component is not assigned. Please ensure the 'tabImage' property is set.", gameObject);
            }
        }

        /// <summary>
        /// Sets the tab's text (TextMeshPro) color externally. This method can be used to modify the text color programmatically.
        /// </summary>
        /// <param name="color">The color to set for the tab's text.</param>
        public void SetTabTextColor(Color color)
        {
            if (tabText != null)
            {
                tabText.color = color;
            }
            else
            {
                Debug.LogError("Tab TextMeshPro component is not assigned. Please ensure the 'tabText' property is set.", gameObject);
            }
        }

        /// <summary>
        /// Sets both the tab's graphic and text colors externally.
        /// </summary>
        /// <param name="imageColor">The color to set for the tab's image.</param>
        /// <param name="textColor">The color to set for the tab's text.</param>
        public void SetTabColors(Color imageColor, Color textColor)
        {
            SetTabImageColor(imageColor);
            SetTabTextColor(textColor);
        }

        /// <summary>
        /// Sets the tab to a "selected" state and triggers the appropriate events.
        /// </summary>
        public void ForceSelectTab()
        {
            SetSelected(true);
            SetHighlighted(false); // Optionally remove highlight when selected
        }

        /// <summary>
        /// Sets the tab to a "highlighted" state without selecting it.
        /// </summary>
        public void ForceHighlightTab()
        {
            SetHighlighted(true);
            SetSelected(false); // Optionally remove selection when highlighted
        }
    }
}