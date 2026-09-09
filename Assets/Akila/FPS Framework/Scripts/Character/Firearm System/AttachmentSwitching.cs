using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework.Examples
{
    [AddComponentMenu("Akila/FPS Framework/Examples/Attachments Switching")]
    public class AttachmentSwitching : MonoBehaviour
    {
        public static AttachmentSwitching Instance;

        public static event System.Action<string> OnSwitch;

        private void Awake()
        {
            if (!Instance) Instance = this;
            else Destroy(gameObject);
        }

        public void Switch(string typeAndName)
        {
            OnSwitch?.Invoke(typeAndName);
        }
    }
}