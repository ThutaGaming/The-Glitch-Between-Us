using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Weapons/Firearm HUD")]
    public class FirearmHUD : MonoBehaviour
    {
        [Header("Text")]
        public TextMeshProUGUI firearmNameText;
        public TextMeshProUGUI ammoTypeNameText;
        public TextMeshProUGUI remainingAmmoText;
        public TextMeshProUGUI remainingAmmoTypeText;
        public GameObject outOfAmmoAlert;
        public GameObject lowAmmoAlert;

        [Header("Colors")]
        public Color normalColor = Color.white;
        public Color alertColor = Color.red;

        public Firearm firearm { get; set; }

        private void Update()
        {
            if (!firearm)
            {
                return;
            }

            gameObject.SetActive(firearm.isHudActive);

            firearmNameText.SetText(firearm.Name);
            ammoTypeNameText.SetText(firearm.ammoProfile.identifier.displayName);
            remainingAmmoText.SetText(firearm.remainingAmmoCount.ToString());
            remainingAmmoTypeText.SetText(firearm.remainingAmmoTypeCount.ToString());

            outOfAmmoAlert.SetActive(firearm.remainingAmmoCount <= 0);
            lowAmmoAlert.SetActive(firearm.remainingAmmoCount <= firearm.preset.magazineCapacity / 3 && firearm.remainingAmmoCount > 0);

            remainingAmmoText.color = firearm.remainingAmmoCount <= firearm.preset.magazineCapacity / 3 ? alertColor : normalColor;
            remainingAmmoTypeText.color = firearm.remainingAmmoTypeCount <= 0 ? alertColor : normalColor;
        }

        private void LateUpdate()
        {
            if(firearm == null)
            {
                Destroy(gameObject);
            }
        }
    }
}