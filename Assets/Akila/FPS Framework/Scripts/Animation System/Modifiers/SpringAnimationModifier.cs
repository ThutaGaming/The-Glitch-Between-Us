using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace Akila.FPSFramework.Animation
{
    [AddComponentMenu("Akila/FPS Framework/Animation/Modifiers/Spring Animation Modifier"), RequireComponent(typeof(ProceduralAnimation))]
    public class SpringAnimationModifier : ProceduralAnimationModifier
    {
        public float speed = 1;
        public SpringVector3 position = new SpringVector3();
        public SpringVector3 rotation = new SpringVector3();

        private void Update()
        {
            position.Update(speed * globalSpeed);
            rotation.Update(speed * globalSpeed);

            targetPosition = position.result;
            targetRotation = rotation.result;
        }

        public void Trigger()
        {
            position.Start(position.value);
            rotation.Start(rotation.value);
        }

        public void Trigger(Vector3 position, Vector3 rotation)
        {
            print(position);
            this.position.Start(position);
            this.rotation.Start(rotation);
        }
    }
}