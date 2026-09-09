using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    public class Ignore : MonoBehaviour
    {
        public bool ignoreHitDetection = false;
        public bool ignoreLaserDetection = false;
        public bool ignoreWallAvoidance = false;
        public bool ignoreFallDamage = false;
        public bool ignoreMovingPlatform = false;

        private void Start()
        {

        }
    }
}