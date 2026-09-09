using Akila.FPSFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    public interface IActor
    {
        public GameObject gameObject { get; }
        public Transform transform { get; }

        public string actorName { get; }
        public int teamId { get; }
        public void Respawn(float delay);
    }
}