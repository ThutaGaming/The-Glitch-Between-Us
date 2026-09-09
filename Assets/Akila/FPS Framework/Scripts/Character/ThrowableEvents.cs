using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Weapons/Throwable Events")]
    public class ThrowableEvents : MonoBehaviour
    {
        private Throwable throwable;
        private IInventory inventory;

        private void Start()
        {
            inventory = transform.SearchFor<IInventory>();

            if (GetComponent<Throwable>()) throwable = GetComponent<Throwable>();
            if (GetComponentInParent<Throwable>()) throwable = GetComponentInParent<Throwable>();
            if (GetComponentInChildren<Throwable>()) throwable = GetComponentInChildren<Throwable>();
        }
        public void Throw()
        {
            throwable.Throw();
        }

        public void Dispose()
        {
            if (throwable.Ammo.count <= 0)
            {
               inventory.Switch(Mathf.Clamp(inventory.currentItemIndex - 1, 0, inventory.currentItemIndex));

                inventory.items.Remove(throwable);

                Destroy(throwable.gameObject);
            }
        }
    }
}