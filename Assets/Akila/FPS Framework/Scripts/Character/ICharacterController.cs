using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace Akila.FPSFramework
{
    public interface ICharacterController
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }
    }
}