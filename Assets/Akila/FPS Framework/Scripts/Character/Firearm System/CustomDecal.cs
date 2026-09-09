using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Effects/Custom Decal")]
    public class CustomDecal : MonoBehaviour
    {
        public GameObject decalVFX;
        public bool parent = true;
        public float lifeTime = 60;
    }
}