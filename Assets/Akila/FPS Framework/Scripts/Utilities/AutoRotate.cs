using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    public class AutoRotate : MonoBehaviour
    {
        public Vector3 speed;

        private void Update()
        {
            transform.Rotate(speed * Time.deltaTime * 100, Space.Self);
        }
    }
}