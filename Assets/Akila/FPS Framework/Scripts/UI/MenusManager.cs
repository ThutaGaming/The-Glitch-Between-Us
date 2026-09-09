using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Akila.FPSFramework.UI
{
    /// <summary>
    /// Manages multiple menus in the FPS Framework, providing functionality to open, close, and manage menu states.
    /// </summary>
    [AddComponentMenu("Akila/FPS Framework/UI/Menus Manager")]
    public class MenusManager : MonoBehaviour
    {
        /// <summary>
        /// Name of the default menu to open at startup.
        /// </summary>
        [Tooltip("The name of the default menu to open when the game starts.")]
        public string DefaultMenu;
        public InputAction previousMenuInputAction = new InputAction("Escape [Keyboard]", InputActionType.Button, "<Keyboard>/escape");

        /// <summary>
        /// List of all menus managed by this MenusManager.
        /// </summary>
        [Tooltip("List of menus controlled by this MenusManager.")]
        public List<Menu> Menus { get; protected set; } = new List<Menu>();

        public Menu activeMenu;
        public Menu previousMenu;

        /// <summary>
        /// Finds a menu by its name.
        /// </summary>
        /// <param name="menuName">The name of the menu to find.</param>
        /// <returns>The matching <see cref="Menu"/> if found, otherwise <c>null</c>.</returns>
        public Menu FindMenu(string menuName)
        {
            if (string.IsNullOrEmpty(menuName))
            {
                Debug.LogError("Menu name cannot be null or empty.");
                return null;
            }

            return Menus.Find(menu => menu.menuName == menuName);
        }

        /// <summary>
        /// Opens the specified menu and closes all others except specified exceptions.
        /// </summary>
        /// <param name="menu">The menu to open.</param>
        public void OpenMenu(Menu menu)
        {
            if (menu == null)
            {
                Debug.LogError("Cannot open a null menu.");
                return;
            }

            activeMenu = menu;
            previousMenu = Menus.Find(m => m.IsOpen = true);

            menu.IsOpen = true;

            CloseAllMenus(new Menu[] { menu });
        }

        /// <summary>
        /// Closes the specified menu.
        /// </summary>
        /// <param name="menu">The menu to close.</param>
        public void CloseMenu(Menu menu)
        {
            if (menu == null)
            {
                Debug.LogError("Cannot close a null menu.");
                return;
            }

            menu.IsOpen = false;
        }

        /// <summary>
        /// Opens a menu by its name and closes all others.
        /// </summary>
        /// <param name="menuName">The name of the menu to open.</param>
        public void OpenMenu(string menuName)
        {
            var menu = FindMenu(menuName);
            if (menu == null)
            {
                Debug.LogError($"Menu with name '{menuName}' not found.");
                return;
            }

            OpenMenu(menu);
        }

        /// <summary>
        /// Closes a menu by its name.
        /// </summary>
        /// <param name="menuName">The name of the menu to close.</param>
        public void CloseMenu(string menuName)
        {
            var menu = FindMenu(menuName);
            if (menu == null)
            {
                Debug.LogError($"Menu with name '{menuName}' not found.");
                return;
            }

            CloseMenu(menu);
        }

        /// <summary>
        /// Opens all menus except the specified exceptions.
        /// </summary>
        /// <param name="exceptions">An optional array of menus to exclude from being opened.</param>
        public void OpenAllMenus(Menu[] exceptions = null)
        {
            foreach (var menu in Menus)
            {
                if (exceptions != null && exceptions.Contains(menu))
                    continue;

                menu.IsOpen = true;
            }
        }

        /// <summary>
        /// Closes all menus except the specified exceptions.
        /// </summary>
        /// <param name="exceptions">An optional array of menus to exclude from being closed.</param>
        public void CloseAllMenus(Menu[] exceptions = null)
        {
            foreach (var menu in Menus)
            {
                if (exceptions != null && exceptions.Contains(menu))
                    continue;

                menu.IsOpen = false;
            }
        }

        /// <summary>
        /// Automatically initializes and opens the default menu at startup.
        /// </summary>
        protected virtual void Start()
        {
            // Populate the list of menus with all Menu components in the children
            Menus = GetComponentsInChildren<Menu>(true).ToList();

            foreach(Menu menu in Menus)
            {
                menu.RefreshFields();

                menu.CanvasGroup.alpha = 0;
                menu.IsOpen = false;
            }

            // Open the default menu if specified
            if (!string.IsNullOrEmpty(DefaultMenu))
            {
                var defaultMenu = FindMenu(DefaultMenu);

                if (defaultMenu != null)
                {
                    OpenMenu(defaultMenu);
                }
                else
                {
                    Debug.LogWarning($"Default menu '{DefaultMenu}' not found in MenusManager.", gameObject);
                }
            }

            previousMenuInputAction.Enable();
        }

        private void LateUpdate()
        {
            if(previousMenuInputAction.triggered)
            {
                CloseAllMenus(new Menu[] { previousMenu });

                if(previousMenu) OpenMenu(previousMenu);
            }
        }
    }
}
