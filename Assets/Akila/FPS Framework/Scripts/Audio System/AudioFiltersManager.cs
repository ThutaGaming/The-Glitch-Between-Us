using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Audio System/Audio Filters Manager")]
    [RequireComponent(typeof(AudioListener))]
    public class AudioFiltersManager : MonoBehaviour
    {
        public bool HighPass;
        public bool LowPass;
        public bool Reverb;

        public AudioHighPassFilter HighPassFilter { get; set; }
        public AudioLowPassFilter LowPassFilter { get; set; }
        public AudioReverbFilter ReverbFilter { get; set; }

        [HideInInspector] public float highPassCutoffFrequency;
        [HideInInspector] public float hightPassLerpTime;

        [HideInInspector] public float lowPassCutoffFrequency;
        [HideInInspector] public float lowPassLerpTime;

        public void Awake()
        {
            if(HighPass)
            {
                HighPassFilter = GetComponent<AudioHighPassFilter>();

                if(HighPassFilter == null)
                {
                    HighPassFilter = gameObject.AddComponent<AudioHighPassFilter>();
                }

                if (HighPassFilter)
                    HighPassFilter.cutoffFrequency = 10;
            }

            if (LowPass)
            {
                LowPassFilter = GetComponent<AudioLowPassFilter>();

                if (LowPassFilter == null)
                {
                    LowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
                }

                if (LowPassFilter)
                    LowPassFilter.cutoffFrequency = 22000;
            }

            if (Reverb)
            {
                ReverbFilter = GetComponent<AudioReverbFilter>();

                if (ReverbFilter == null)
                {
                    ReverbFilter = gameObject.AddComponent<AudioReverbFilter>();
                }

                if (ReverbFilter)
                    ReverbFilter.reverbPreset = AudioReverbPreset.Off;
            }
        }

        private void Update()
        {
            if (HighPass)
            {
                if (HighPassFilter.cutoffFrequency != highPassCutoffFrequency)
                {
                    HighPassFilter.cutoffFrequency = Mathf.Lerp(HighPassFilter.cutoffFrequency, highPassCutoffFrequency, hightPassLerpTime);
                }
            }

            if (LowPass)
            {
                if (LowPassFilter.cutoffFrequency != lowPassCutoffFrequency)
                {
                    LowPassFilter.cutoffFrequency = Mathf.Lerp(LowPassFilter.cutoffFrequency, lowPassCutoffFrequency, lowPassLerpTime);
                }
            }
        }

        public void SetHightPass(float cutoffFrequency, float time)
        {
            highPassCutoffFrequency = cutoffFrequency;
            hightPassLerpTime = time;
        }

        public void ResetHighPass(float time)
        {
            highPassCutoffFrequency = 10;
            hightPassLerpTime = time;
        }

        public void ResetHighPass(float time, float delay)
        {
            StartCoroutine(ResetHighPassAfterDelay(delay, time));
        }

        public void SetLowPass(float cutoffFrequency, float time)
        {
            lowPassCutoffFrequency = cutoffFrequency;
            lowPassLerpTime = time;
        }

        public void ResetLowPass(float time)
        {
            lowPassCutoffFrequency = 22000;
            lowPassLerpTime = time;
        }

        public void ResetLowPass(float time, float delay)
        {
            StartCoroutine(ResetLowPassAfterDelay(delay, time));
        }

        private IEnumerator ResetHighPassAfterDelay(float delay, float time)
        {
            yield return new WaitForSeconds(delay);
            ResetHighPass(time);
        }

        private IEnumerator ResetLowPassAfterDelay(float delay, float time)
        {
            yield return new WaitForSeconds(delay);
            ResetLowPass(time);
        }

        public void SetReverp(AudioReverbPreset preset)
        {
            ReverbFilter.reverbPreset = preset;
        }

        public void ResetReverp()
        {
            ReverbFilter.reverbPreset = AudioReverbPreset.Off;
        }

        public void EnableAll()
        {
            HighPassFilter.enabled = true;
            LowPassFilter.enabled = true;
            ReverbFilter.enabled = true;
        }

        public void DisableAll()
        {
            HighPassFilter.enabled = false;
            LowPassFilter.enabled = false;
            ReverbFilter.enabled = false;
        }
    }
}