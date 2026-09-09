using UnityEngine;
using UnityEngine.Events;

namespace Akila.FPSFramework.UI
{
    /// <summary>
    /// Represents a menu within the FPS Framework.
    /// Handles visibility, fade transitions, and interaction with the parent <see cref="MenusManager"/>.
    /// </summary>
    [AddComponentMenu("Akila/FPS Framework/UI/Menu")]
    [RequireComponent(typeof(CanvasGroup))]
    public class Menu : MonoBehaviour
    {
        /// <summary>
        /// Name of the menu (for identification purposes).
        /// </summary>
        [Tooltip("Name of the menu for identification.")]
        public string menuName = "Menu";

        /// <summary>
        /// Duration for the menu fade transition in seconds.
        /// </summary>
        [Tooltip("Duration of the fade transition when opening/closing the menu.")]
        public float transitionTime = 0.1f;

        [Space]
        /// <summary>
        /// Event invoked when the menu is opened.
        /// </summary>
        [Tooltip("Event triggered when the menu is opened.")]
        public UnityEvent onOpen;

        /// <summary>
        /// Event invoked when the menu is closed.
        /// </summary>
        [Tooltip("Event triggered when the menu is closed.")]
        public UnityEvent onClose;

        /// <summary>
        /// The CanvasGroup component managing the menu's visibility and interactivity.
        /// </summary>
        public CanvasGroup CanvasGroup { get; protected set; }

        /// <summary>
        /// Reference to the parent MenusManager controlling this menu.
        /// </summary>
        public MenusManager MenusManager { get; protected set; }

        private bool _isOpen;

        /// <summary>
        /// Gets or sets whether the menu is currently open.
        /// When set, triggers the corresponding OnOpen or OnClose event.
        /// </summary>
        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                if (value != _isOpen)
                {
                    if (value)
                        onOpen?.Invoke();
                    else
                        onClose?.Invoke();

                    _isOpen = value;
                }
            }
        }

        /// <summary>
        /// Target alpha value based on whether the menu is open.
        /// </summary>
        public float TargetAlpha => IsOpen ? 1f : 0f;

        /// <summary>
        /// Current alpha value of the CanvasGroup.
        /// </summary>
        public float CurrentAlpha => CanvasGroup.alpha;

        private float _alphaChangeVelocity;

        /// <summary>
        /// Initializes the menu and ensures required components are present.
        /// </summary>
        protected virtual void Start()
        {
            RefreshFields();
        }

        /// <summary>
        /// Smoothly updates the alpha value of the CanvasGroup for fade transitions.
        /// </summary>
        protected virtual void Update()
        {
            if (CanvasGroup != null)
            {
                CanvasGroup.alpha = Mathf.Lerp(CanvasGroup.alpha, TargetAlpha, 1 / transitionTime * Time.unscaledDeltaTime);

                CanvasGroup.interactable = IsOpen;
                CanvasGroup.blocksRaycasts = IsOpen;
            }
        }

        /// <summary>
        /// Get all required components.
        /// </summary>
        public void RefreshFields()
        {
            // Attempt to retrieve the CanvasGroup component
            CanvasGroup = GetComponent<CanvasGroup>();
            if (CanvasGroup == null)
            {
                Debug.LogError("CanvasGroup component is missing. The Menu requires a CanvasGroup to function properly.", gameObject);

                enabled = false; // Disable the script to prevent further issues
                return;
            }

            // Attempt to find a MenusManager in the parent hierarchy
            MenusManager = GetComponentInParent<MenusManager>();

            if (MenusManager == null)
            {
                // Automatically add a MenusManager to the parent object if none exists
                MenusManager = transform.parent?.gameObject.AddComponent<MenusManager>();

                Debug.LogWarning("No MenusManager found in the parent hierarchy. A new MenusManager was added to the parent object.", gameObject);
            }
        }

        /// <summary>
        /// Requests to open this menu through the MenusManager.
        /// </summary>
        public void OpenMenu()
        {
            RefreshFields();

            MenusManager?.OpenMenu(this);
        }

        /// <summary>
        /// Requests to close this menu through the MenusManager.
        /// </summary>
        public void CloseMenu()
        {
            RefreshFields();

            MenusManager?.CloseMenu(this);
        }

        /// <summary>
        /// Toggles the menu's state between open and closed.
        /// </summary>
        public void ToggleMenu()
        {
            if (IsOpen)
                CloseMenu();
            else
                OpenMenu();
        }
    }
}
