using Akila.FPSFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pickable)), DisallowMultipleComponent]
public class FirearmPickableModule : MonoBehaviour
{
    public int storedAmmoCount = -1;

    public Pickable pickable { get; set; }
    public FirearmAttachmentsManager firearmAttachmentsManager {  get; set; }

    private void Start()
    {
        pickable = GetComponent<Pickable>();
        firearmAttachmentsManager = GetComponent<FirearmAttachmentsManager>();

        pickable.onPickupPerformed.AddListener(Apply);

        if(storedAmmoCount < 0)
        {
            Firearm firearm = null;

            if(pickable.itemToPickup != null)
            {
                firearm = pickable.itemToPickup.GetComponent<Firearm>();

                storedAmmoCount = firearm.preset.magazineCapacity;
            }
        }
    }

    private void Apply(InventoryItem item)
    {
        item.firearm = item.GetComponent<Firearm>();

        if (item.firearm == null) return;

        item.firearm.remainingAmmoCount = storedAmmoCount;

        if(firearmAttachmentsManager)
        {
            if (item.TryGetComponent<FirearmAttachmentsManager>(out FirearmAttachmentsManager firearmAttachments))
            {
                firearmAttachments.activeAttachments = firearmAttachmentsManager.activeAttachments;
            }
        }
    }
}
