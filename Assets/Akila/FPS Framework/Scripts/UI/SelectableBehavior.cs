using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using Akila.FPSFramework.UI;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/UI/Selectable Behavior")]
    public class SelectableBehavior : Selectable, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
        }
    }
}