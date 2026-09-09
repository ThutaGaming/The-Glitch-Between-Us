using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Akila.FPSFramework
{
    /// <summary>
    /// Represents an item or collectable that a player can interact with in the world.
    /// Can be added to the inventory or counted as a collectable.
    /// </summary>
    [AddComponentMenu("Akila/FPS Framework/Player/Pickable")]
    public class Pickable : Item, IInteractable
    {
        [Tooltip("Display name used when showing the interaction prompt.")]
        public string interactionName = "Take";

        [Tooltip("The type of this pickable (Item or Collectable).")]
        public PickableType type;

        [Tooltip("The inventory item prefab to be added when this is picked up (used if type is 'Item')."), FormerlySerializedAs("item")]
        public InventoryItem itemToPickup;

        public bool includeCollectable = false;

        [Tooltip("Unique identifier for this collectable (used if type is 'Collectable').")]
        public InventoryCollectableIdentifier collectableIdentifier;

        [Tooltip("Optional sound clip played when this item is interacted with.")]
        public AudioClip interactSound;

        [Tooltip("The amount of collectables granted when picked up (used if type is 'Collectable').")]
        public int collectableCount = 1;

        [HideInInspector] public UnityEvent<GameObject> onInteractAttemptedWithItem;
        [HideInInspector] public UnityEvent<GameObject> onInteractAttemptedWithAmmo;

        public UnityEvent<InventoryItem> onPickupPerformed;

        public bool isInstant => true;

        /// <summary>
        /// Called when the player interacts with this object.
        /// Routes interaction based on the pickable type.
        /// </summary>
        /// <param name="source">The InteractionsManager initiating the interaction.</param>
        public void Interact(InteractionsManager source)
        {
            if (source == null)
            {
                Debug.LogError("InteractionsManager is null during interaction.", gameObject);

                return;
            }

            if (isActive)
            {
                if (interactSound != null)
                    source.interactionAudio?.Play(true, interactSound);
                else if (source.defaultInteractionAudio)
                    source.interactionAudio?.Play(true, source.defaultInteractionAudio.audioClip);
            }

            switch (type)
            {
                case PickableType.Item:
                    InteractWithItem(source);
                    break;
                case PickableType.Collectable:
                    InteractWithCollectable(source);
                    break;
                default:
                    Debug.LogWarning($"Unhandled pickable type: {type}", gameObject);
                    break;
            }
        }

        /// <summary>
        /// Returns a string for display when interacting with the object.
        /// </summary>
        public string GetInteractionName()
        {
            string info = $"{Name} - {type}";
            return $"{interactionName} {info}";
        }

        /// <summary>
        /// Handles interaction logic for item pickables.
        /// Instantiates the item, updates inventory, plays animations, and destroys the pickable.
        /// </summary>
        /// <param name="source">The InteractionsManager performing the interaction.</param>
        public virtual void InteractWithItem(InteractionsManager source)
        {
            if (source == null || source.Inventory == null)
            {
                Debug.LogError("Missing InteractionsManager or Inventory reference during item interaction.", gameObject);
                return;
            }


            GameObject player = source.transform.SearchFor<ICharacterController>()?.gameObject;
            if (player != null)
                onInteractAttemptedWithItem?.Invoke(player);

            if (itemToPickup == null)
            {
                Debug.LogWarning("Pickable is inactive or item is null during InteractWithItem.", gameObject);
                return;
            }

            if (!isActive)
                return;

            IInventory inventory = source.Inventory;

            InventoryItem itemToFind = inventory.items.Find(i => i.Name == itemToPickup.Name);

            if (includeCollectable && itemToFind == null)
            {
                DoItemPickup(inventory);
            }
            else
            {
                int index = inventory.items.IndexOf(itemToFind);

                index = Mathf.Clamp(index, 0, inventory.maxSlots - 1);

                inventory.Switch(index);
            }

            if(includeCollectable == false)
            {
                DoItemPickup(inventory);
            }

            if (includeCollectable)
                InteractWithCollectable(source);

            Destroy(gameObject);
        }

        protected virtual void DoItemPickup(IInventory inventory)
        {
            // Refresh inventory current item UI etc.
            if (itemToPickup == null)
            {
                Debug.LogWarning("Pickable item is null.");
                return;
            }

            InventoryItem newItem = Instantiate(itemToPickup, inventory.transform);


            // Update inventory items list
            inventory.items = inventory.transform.GetComponentsInChildren<InventoryItem>(true).ToList();

            // Switch inventory to the newly picked up item
            int index = inventory.items.IndexOf(newItem);

            if (index != -1)
                inventory.Switch(index);

            if(inventory.items.Count > inventory.maxSlots)
            {
                inventory.currentItem.Drop(true);
            }

            onPickupPerformed?.Invoke(newItem);
        }

        /// <summary>
        /// Handles interaction logic for collectable pickables.
        /// Adds count to existing collectable or creates a new one, and triggers pickup animation.
        /// </summary>
        /// <param name="source">The InteractionsManager performing the interaction.</param>
        public virtual void InteractWithCollectable(InteractionsManager source)
        {
            if (source == null || source.Inventory == null)
            {
                Debug.LogError("Missing source or inventory reference during collectable interaction.", gameObject);
                return;
            }

            GameObject player = source.transform.SearchFor<ICharacterController>()?.gameObject;

            if (player != null)
                onInteractAttemptedWithAmmo?.Invoke(player);

            IInventory inventory = source.Inventory;

            // Try to find an existing collectable
            InventoryCollectable collectable = inventory.collectables
                .FirstOrDefault(m => m.identifier == collectableIdentifier);

            if (collectable == null)
            {
                // Add new collectable
                collectable = new InventoryCollectable
                {
                    identifier = collectableIdentifier,
                    count = collectableCount
                };

                inventory.collectables.Add(collectable);
            }
            else
            {
                collectable.count += collectableCount;
            }

            // Trigger pickup animation if available
            if (inventory.items.Count > 0)
            {
                InventoryItem currentItem = inventory.currentItem;
                if (currentItem?.animators != null)
                {
                    foreach (Animator animator in currentItem.animators)
                    {
                        animator?.CrossFade("Pickup", 0.1f, 0, 0);
                    }
                }
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// Editor-only context menu option to set up network components.
        /// </summary>
        [ContextMenu("Setup/Network Components")]
        private void SetupNetworkComponents()
        {
            FPSFrameworkCore.InvokeConvertMethod("ConvertPickable", this, new object[] { this });
        }
    }

    /// <summary>
    /// Enum representing the type of a Pickable object.
    /// </summary>
    public enum PickableType
    {
        /// <summary>
        /// Represents a standard inventory item.
        /// </summary>
        Item = 0,

        /// <summary>
        /// Represents a collectable that increases a resource count.
        /// </summary>
        Collectable = 1,
    }
}
