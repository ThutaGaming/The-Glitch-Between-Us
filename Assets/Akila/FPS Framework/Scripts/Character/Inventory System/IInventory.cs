using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Akila.FPSFramework
{
    public interface IInventory
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }

        //Fields
        List<InventoryCollectable> collectables { get; }
        public List<InventoryItem> items { get; set; }
        public InventoryItem currentItem { get; }
        public Transform dropPoint { get; }
        public int currentItemIndex { get; set; }
        public int maxSlots { get; }
        public float dropForce { get; }

        public bool isActive { get; set; }
        public bool isInputActive { get; set; }

        public void Switch(int index, bool immediate = true);
    }
}