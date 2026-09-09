using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;
using Akila.FPSFramework.Internal;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Player/Inventory")]
    public class Inventory : MonoBehaviour, IInventory
    {
        public List<InventoryItem> startItems = new List<InventoryItem>();
        public List<InventoryItem> items = new List<InventoryItem>();
        public List<InventoryCollectable> collectables = new List<InventoryCollectable>();

        [Space]
        public InventoryItem defaultItem;
        public int maxSlots = 3;
        public float dropForce = 1;
        public Transform dropLocation;

        public CharacterManager characterManager { get; set; }

        public CharacterInput characterInput { get; set; }

        public bool isActive { get; set; } = true;


        private int previousItemIndex { get; set; }
        public int currentItemIndex { get; set; }


        List<InventoryItem> IInventory.items { get => items; set => items = value; }
        int IInventory.maxSlots { get => maxSlots; }
        float IInventory.dropForce {  get => dropForce; }
        Transform IInventory.dropPoint { get => dropLocation; }
        public bool isInputActive { get; set; } = true;

        public InventoryItem _defaultItem { get; set; }
        public InventoryItem currentDefaultItem { get => _defaultItem; set => _defaultItem = value; }
        List<InventoryCollectable> IInventory.collectables { get => collectables; }
        public InventoryItem currentItem { get => activeItem; }

        private void Start()
        {
            if (items.Count > 0)
            {
                Debug.LogWarning($"[Warning] Items list has {items.Count} elements, but it should be empty. This may indicate an unexpected state. Emptying the list now.", this);

                items.Clear();
            }

            characterManager = GetComponentInParent<CharacterManager>();

            characterInput = GetComponentInParent<CharacterInput>();

            if (isActive)
            {
                foreach (InventoryItem startItem in startItems)
                {
                    InventoryItem newItem = Instantiate(startItem, transform);
                }

                if (defaultItem)
                {
                    _defaultItem = Instantiate(defaultItem, transform);
                    _defaultItem.isDroppingActive = false;
                }
            }

            Switch(0);
        }

        private void RefreshItemsList()
        {
            List<InventoryItem> _items = GetComponentsInChildren<InventoryItem>(true).ToList();

            if (defaultItem == null)
            {
                items = _items;

                return;
            }

            InventoryItem itemToRemove = _items.Find(e => e == _defaultItem);

            _items.Remove(itemToRemove);
            
            items = _items;
        }

        private void Update()
        {
            GetInput();

            RefreshItemsList();

            if (items.Count == 0) currentItemIndex = -1;

            //Ensure the item index wraps around correctly, staying within the bounds of the list
            if (currentItemIndex > items.ToArray().Length - 1) currentItemIndex = 0;

            if (!_defaultItem)
            {
                if (currentItemIndex < 0) currentItemIndex = items.ToArray().Length - 1;
            }
            else
            {
                if (currentItemIndex < -1) currentItemIndex = items.ToArray().Length - 1;
            }

            //Switch item if item index changes
            if (previousItemIndex != currentItemIndex)
            {
                //Empty
                lastItemIndex = previousItemIndex;
            }

            if(currentItemIndex == -1)
            {
                activeItem = currentDefaultItem;
            }
            else
            {
                activeItem = items[currentItemIndex];
            }

            Switch(currentItemIndex);

            //Update the item index
            previousItemIndex = currentItemIndex;
        }

        private void GetInput()
        {
            if (!FPSFrameworkCore.IsActive || !FPSFrameworkCore.IsInputActive || !isInputActive) return;

            if (characterInput.item1) currentItemIndex = 0;
            if (characterInput.item2) currentItemIndex = 1;
            if (characterInput.item3) currentItemIndex = 2;
            if (characterInput.item4) currentItemIndex = 3;
            if (characterInput.item5) currentItemIndex = 4;
            if (characterInput.item6) currentItemIndex = 5;
            if (characterInput.item7) currentItemIndex = 6;
            if (characterInput.item8) currentItemIndex = 7;
            if (characterInput.item9) currentItemIndex = 8;
            
            if (characterInput.itemUp) 
                currentItemIndex++;

            if (characterInput.itemDown)
            {
                currentItemIndex--;
                
                {
                    if (currentItemIndex < 0) currentItemIndex = items.ToArray().Length - 1;
                }
            }

            if (characterInput.defaultItem)
            {
                currentItemIndex = -1;
            }

            if(characterInput.itemNext) currentItemIndex++;
            if (characterInput.itemDown) currentItemIndex = lastItemIndex;
        }

        private int lastItemIndex;
        private InventoryItem activeItem;

        /// <summary>
        /// Switches the currently active inventory item by index.
        /// Pass -1 to activate the default item instead.
        /// </summary>
        /// <param name="index">Index of the item to activate. Use -1 to activate the default item.</param>
        /// <param name="immediate">Currently unused. Intended for future transition logic.</param>
        public void Switch(int index, bool immediate = true)
        {
            // Refresh the list of inventory items (in case it has changed)
            RefreshItemsList();

            // Store the current index for reference
            currentItemIndex = index;

            // Determine if we should activate the default item
            bool useDefault = (index == -1);

            if (useDefault)
            {
                // If a default item is defined, activate it and deactivate all others
                if (_defaultItem)
                {
                    _defaultItem.gameObject.SetActive(true);
                }

                // Deactivate all inventory items
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].gameObject.SetActive(false);
                }

                return; // Done with default item switching
            }

            // If not using default item, deactivate the default if it exists
            if (_defaultItem)
            {
                _defaultItem.gameObject.SetActive(false);
            }

            // Activate the selected item by index, deactivate the others
            for (int i = 0; i < items.Count; i++)
            {
                items[i].gameObject.SetActive(i == index);
            }
        }


        public void DropAllItems()
        {
            items = transform.GetComponentsInChildren<InventoryItem>(true).ToList();

            foreach (InventoryItem item in items)
            {
                if (item != null)
                {
                    item.Drop(false);
                }
            }

            items.Clear();
        }

        /// <summary>
        /// Editor-only context menu option to set up network components.
        /// </summary>
        [ContextMenu("Setup/Network Components")]
        private void SetupNetworkComponents()
        {
            FPSFrameworkCore.InvokeConvertMethod("ConvertInventory", this, new object[] { this });
        }
    }
}