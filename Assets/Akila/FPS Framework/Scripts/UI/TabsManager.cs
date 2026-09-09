using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Akila.FPSFramework.UI
{
    /// <summary>
    /// Manages the states of multiple tabs in a tab-based UI system.
    /// Ensures only one tab can be selected at a time and provides external control to manage tab states.
    /// </summary>
    public class TabsManager : MonoBehaviour
    {
        // Array of tabs managed by this TabsManager


        [Tooltip("The selected tab onAwake")]
        public Tab defaultTab;

        /// <summary>
        /// The list of tabs managed by this TabsManager.
        /// </summary>
        public List<Tab> tabs { get; protected set; }
  
        // Internal variable to keep track of the currently selected tab
        public Tab currentSelectedTab { get; protected set; }

        /// <summary>
        /// Initializes the TabsManager by finding all Tab components and setting up listeners.
        /// </summary>
        private void Start()
        {
            tabs = GetComponentsInChildren<Tab>().ToList();

            if (tabs.Count == 0)
            {
                Debug.LogWarning("No Tab components found in the children of this TabsManager. Ensure Tabs are properly assigned.", gameObject);
            }

            // Set up listeners for each tab to handle selection events
            foreach (Tab tab in tabs)
            {
                tab.onTabSelected.AddListener(DisableAllAndKeepSelected);
            }

            // Optionally, auto-select the default or first tab on start
            if (defaultTab) 
                SelectTabAtIndex(tabs.IndexOf(defaultTab));
            else 
                SelectFirstTab();
        }

        /// <summary>
        /// Ensures that only the selected tab remains active (unselects other tabs).
        /// </summary>
        /// <param name="selectedTab">The tab that has been selected.</param>
        private void DisableAllAndKeepSelected(Tab selectedTab)
        {
            foreach (Tab tab in tabs)
            {
                if (tab != selectedTab)
                    tab.DeselectTab();  // Unselect the non-selected tabs
            }

            currentSelectedTab = selectedTab;  // Keep track of the currently selected tab
        }

        /// <summary>
        /// Deselects all tabs and selects the first tab in the list.
        /// This method is typically used for initializing the UI on startup.
        /// </summary>
        public void SelectFirstTab()
        {
            if (tabs.Count > 0)
            {
                tabs[0].SelectTab();  // Force select the first tab
            }
            else
            {
                Debug.LogWarning("Cannot select the first tab because no tabs are available.", gameObject);
            }
        }

        /// <summary>
        /// Deselects all tabs and forces a specific tab to be selected by index.
        /// </summary>
        /// <param name="index">The index of the tab to select.</param>
        public void SelectTabAtIndex(int index)
        {
            if (index >= 0 && index < tabs.Count)
            {
                DeselectAllTabs();
                tabs[index].SelectTab();
            }
            else
            {
                Debug.LogError($"Invalid tab index {index}. The index must be between 0 and {tabs.Count - 1}.", gameObject);
            }
        }

        /// <summary>
        /// Deselects all tabs without selecting any specific tab.
        /// </summary>
        public void DeselectAllTabs()
        {
            foreach (Tab tab in tabs)
            {
                tab.DeselectTab();  // Deselect each tab
            }
        }

        /// <summary>
        /// Highlights a specific tab by index without selecting it.
        /// </summary>
        /// <param name="index">The index of the tab to highlight.</param>
        public void HighlightTabAtIndex(int index)
        {
            if (index >= 0 && index < tabs.Count)
            {
                tabs[index].HighlightTab();  // Use the HighlightTab method from Tab class
            }
            else
            {
                Debug.LogError($"Invalid tab index {index}. The index must be between 0 and {tabs.Count - 1}.", gameObject);
            }
        }

        /// <summary>
        /// Unhighlights a specific tab by index.
        /// </summary>
        /// <param name="index">The index of the tab to unhighlight.</param>
        public void UnhighlightTabAtIndex(int index)
        {
            if (index >= 0 && index < tabs.Count)
            {
                tabs[index].UnhighlightTab();  // Use the UnhighlightTab method from Tab class
            }
            else
            {
                Debug.LogError($"Invalid tab index {index}. The index must be between 0 and {tabs.Count - 1}.", gameObject);
            }
        }

        /// <summary>
        /// Enable a specific tab by index. This will allow the tab to be interactable.
        /// </summary>
        /// <param name="index">The index of the tab to enable.</param>
        public void EnableTabAtIndex(int index)
        {
            if (index >= 0 && index < tabs.Count)
            {
                tabs[index].gameObject.SetActive(true);  // Enable the tab GameObject
            }
            else
            {
                Debug.LogError($"Invalid tab index {index}. The index must be between 0 and {tabs.Count - 1}.", gameObject);
            }
        }

        /// <summary>
        /// Disable a specific tab by index. This will make the tab non-interactable.
        /// </summary>
        /// <param name="index">The index of the tab to disable.</param>
        public void DisableTabAtIndex(int index)
        {
            if (index >= 0 && index < tabs.Count)
            {
                tabs[index].gameObject.SetActive(false);  // Disable the tab GameObject
            }
            else
            {
                Debug.LogError($"Invalid tab index {index}. The index must be between 0 and {tabs.Count - 1}.", gameObject);
            }
        }

        /// <summary>
        /// Enables or disables all tabs in the manager.
        /// </summary>
        /// <param name="enabled">True to enable all tabs, false to disable all.</param>
        public void SetTabsActive(bool enabled)
        {
            foreach (Tab tab in tabs)
            {
                tab.gameObject.SetActive(enabled);  // Enable or disable each tab GameObject
            }
        }

        /// <summary>
        /// Returns the currently selected tab, or null if no tab is selected.
        /// </summary>
        /// <returns>The currently selected tab, or null if none is selected.</returns>
        public Tab GetSelectedTab()
        {
            return currentSelectedTab;
        }

        /// <summary>
        /// Returns the tab at the specified index.
        /// </summary>
        /// <param name="index">The index of the tab to retrieve.</param>
        /// <returns>The tab at the specified index, or null if the index is out of range.</returns>
        public Tab GetTabAtIndex(int index)
        {
            if (index >= 0 && index < tabs.Count)
            {
                return tabs[index];
            }
            else
            {
                Debug.LogError($"Invalid tab index {index}. The index must be between 0 and {tabs.Count - 1}.", gameObject);
                return null;
            }
        }

        /// <summary>
        /// Returns the total number of tabs managed by this TabsManager.
        /// </summary>
        /// <returns>The total number of tabs.</returns>
        public int GetTabCount()
        {
            return tabs.Count;
        }

        #region Additional Functions Based on Tab Class

        /// <summary>
        /// Highlights a specific tab externally by index.
        /// </summary>
        public void ForceHighlightTab(int index)
        {
            if (index >= 0 && index < tabs.Count)
            {
                tabs[index].ForceHighlightTab();
            }
            else
            {
                Debug.LogError($"Invalid tab index {index}. The index must be between 0 and {tabs.Count - 1}.", gameObject);
            }
        }

        /// <summary>
        /// Selects a specific tab externally by index.
        /// </summary>
        public void ForceSelectTab(int index)
        {
            if (index >= 0 && index < tabs.Count)
            {
                tabs[index].ForceSelectTab();
            }
            else
            {
                Debug.LogError($"Invalid tab index {index}. The index must be between 0 and {tabs.Count - 1}.", gameObject);
            }
        }

        #endregion
    }
}
