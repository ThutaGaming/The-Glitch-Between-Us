using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Akila.FPSFramework
{
    /// <summary>
    /// Base class for all InventoryItems and all PickableItems
    /// </summary>
    public class Item : MonoBehaviour
    {
        [Header("Base")]
        public string Name = "Default";

        public bool isActive { get; set; } = true;
    }
}