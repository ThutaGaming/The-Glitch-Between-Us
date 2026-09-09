using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Akila.FPSFramework
{
    [CreateAssetMenu(menuName = "Akila/FPS Framework/Inventory Collectable Identifier", fileName = "New Inventory Collectable Identifier")]
    public class InventoryCollectableIdentifier : ScriptableObject
    {
        public string displayName = "Default Collectable";
    }
}