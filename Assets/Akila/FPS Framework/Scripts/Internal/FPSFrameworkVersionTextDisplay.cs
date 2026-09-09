using TMPro;
using UnityEngine;

namespace Akila.FPSFramework
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class FPSFrameworkVersionTextDisplay : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<TextMeshProUGUI>().text = $"FPS FRAMEWORK {FPSFrameworkCore.version}";
        }
    }
}