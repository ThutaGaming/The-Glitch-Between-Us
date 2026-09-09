using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Akila.FPSFramework.Animation
{
    [AddComponentMenu("Akila/FPS Framework/Animation/Modifiers/Wave Modifier"), RequireComponent(typeof(ProceduralAnimation))]
    public class WaveAnimationModifier : ProceduralAnimationModifier
    {
        public float speed = 1;
        public float amount = 1;
        public WaveProfile position = new WaveProfile();
        public WaveProfile rotation = new WaveProfile();
        public bool syncWithAnimation;
        public float syncSpeed = 5;

        private float inSyncAmount = 1;

        private void Start()
        {
            position.Resume();
            rotation.Resume();
        }

        private void Update()
        {
            targetPosition = position.result;
            targetRotation = rotation.result;

            if (targetAnimation.isPaused) return;

            if (syncWithAnimation)
            {
                inSyncAmount = targetAnimation.progress;

                if (targetAnimation.IsPlaying)
                {
                    position.Resume();
                    rotation.Resume();
                }
                else
                {
                    position.Pause(syncSpeed);
                    rotation.Pause(syncSpeed);
                }
            }
            else
            {
                inSyncAmount = 1;
            }

            position.Update(speed * globalSpeed, amount * inSyncAmount);
            rotation.Update(speed * globalSpeed, amount * inSyncAmount);
        }

        [Serializable]
        public class WaveProfile
        {
            public float smoothTime = 0;

            [Space]
            public Vector3 amount;
            public Vector3 speed = new Vector3(1, 1, 1);

            [HideInInspector]
            public Vector3 result;
            private Vector3 time;

            private bool isPaused = false;

            float xV;
            float yV;
            float zV;

            public void Update(float globalSpeed, float globalAmount)
            {
                if (isPaused == true) return;

                time.x += Time.deltaTime * speed.x * globalSpeed;
                time.y += Time.deltaTime * speed.y * globalSpeed;
                time.z += Time.deltaTime * speed.z * globalSpeed;


                result.x = UnityEngine.Mathf.SmoothDamp(result.x, amount.x * speed.x * Mathf.Sin(time.x) * globalAmount, ref xV, smoothTime);
                result.y = UnityEngine.Mathf.SmoothDamp(result.y, amount.y * speed.y * Mathf.Sin(time.y) * globalAmount, ref yV, smoothTime);
                result.z = UnityEngine.Mathf.SmoothDamp(result.z, amount.z * speed.z * Mathf.Sin(time.z) * globalAmount, ref zV, smoothTime);
            }

            public void Pause(float pauseSpeed)
            {
                result = Vector3.Lerp(result, Vector3.zero, Time.deltaTime * pauseSpeed);
                time = Vector3.zero;

                isPaused = true;
            }

            public void Resume()
            {
                isPaused = false;
            }
        }
    }
}