using UnityEngine.Events;
using System;

namespace Akila.FPSFramework.Animation
{
    [Serializable]
    public class ProceduralAnimationEvents
    {
        public UnityEvent OnPlay = new UnityEvent();
        public UnityEvent OnPlayed = new UnityEvent();
        public UnityEvent OnStoped = new UnityEvent();
    }
}