using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Akila.FPSFramework.UI;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/UI/Get All Screen Resolution")]
    public class GetAllScreenResolution : MonoBehaviour
    {
        private CarouselSelector dropdown;

        private void Awake()
        {
            PopulateDropdown();
        }

        [ContextMenu("Populate Resolutions")]
        private void PopulateDropdown()
        {
            dropdown = GetComponent<CarouselSelector>();

            if (dropdown == null)
            {
                Debug.LogError("No Dropdown component found on this GameObject.");
                return;
            }

            dropdown.ClearOptions();

            Resolution[] resolutions = FPSFrameworkCore.GetResolutions();

            // Format with refresh rate ratio (better than old refreshRate)
            List<string> options = resolutions
                .Select(res => $"{res.width} x {res.height} @ {res.refreshRateRatio.value:F2}Hz")
                .Distinct()
                .ToList();

            dropdown.AddOptions(options.ToArray());

            // Highlight current resolution
            string currentRes = $"{Screen.currentResolution.width} x {Screen.currentResolution.height} @ {Screen.currentResolution.refreshRateRatio.value:F2}Hz";
            int currentIndex = options.IndexOf(currentRes);
            if (currentIndex >= 0)
                dropdown.value = currentIndex;
        }
    }
}
