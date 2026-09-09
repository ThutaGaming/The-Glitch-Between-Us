using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace Akila.FPSFramework.Animation
{
    [AddComponentMenu("Akila/FPS Framework/Animation/Modifiers/Kick Animation Modifier")]
    public class KickAnimationModifier : ProceduralAnimationModifier
    {
        public UpdateMode UpdateMode = UpdateMode.FixedUpdate;
        [Range(0, 1)] public float positionWeight = 1;
        [Range(0, 1)] public float rotationWeight = 1;
        public float positionRoughness = 10;
        public float rotationRoughness = 10;
        public Vector3 staticPosition;
        public Vector3 staticRotation;
        [FormerlySerializedAs("position")]
        public Vector3 randomPosition;
        [FormerlySerializedAs("rotation")]
        public Vector3 randomRotation;

        [Space]
        public UnityEvent OnTrigger = new UnityEvent();

        private Vector3 currentRotation;
        private Vector3 currentPosition;

        private void Update()
        {
            if (UpdateMode == UpdateMode.Update)
            {
                float deltaTime = Time.deltaTime * globalSpeed; // Scale time

                targetPosition = (Vector3.Slerp(targetPosition, currentPosition, positionRoughness * deltaTime) * positionWeight) * globalSpeed;
                targetRotation = (Vector3.Slerp(targetRotation, currentRotation, rotationRoughness * deltaTime) * rotationWeight) * globalSpeed;
            }
        }

        private void FixedUpdate()
        {
            float fixedDeltaTime = Time.fixedDeltaTime * globalSpeed; // Scale time

            currentPosition = Vector3.Lerp(currentPosition, Vector3.zero, 35 * fixedDeltaTime) * globalSpeed;
            currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, 35 * fixedDeltaTime) * globalSpeed;

            if (UpdateMode == UpdateMode.FixedUpdate)
            {
                targetPosition = Vector3.Slerp(targetPosition, currentPosition, positionRoughness * fixedDeltaTime) * positionWeight * globalSpeed;
                targetRotation = Vector3.Slerp(targetRotation, currentRotation, rotationRoughness * fixedDeltaTime) * rotationWeight * globalSpeed;
            }
        }

        private void LateUpdate()
        {
            if (UpdateMode == UpdateMode.LateUpdate)
            {
                float deltaTime = Time.deltaTime * globalSpeed; // Scale time

                targetPosition = Vector3.Slerp(targetPosition, currentPosition, positionRoughness * deltaTime) * positionWeight * globalSpeed;
                targetRotation = Vector3.Slerp(targetRotation, currentRotation, rotationRoughness * deltaTime) * rotationWeight * globalSpeed;
            }
        }


        public void Trigger()
        {
            currentPosition += staticPosition + new Vector3(Random.Range(randomPosition.x, -randomPosition.x), Random.Range(randomPosition.y, -randomPosition.y), randomPosition.z) * globalSpeed;
            currentRotation += staticRotation + new Vector3(randomRotation.x, Random.Range(randomRotation.y, -randomRotation.y), Random.Range(randomRotation.z, -randomRotation.z)) * globalSpeed;
            OnTrigger?.Invoke();
        }
    }
}