using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("")]
    internal class AudioObject : MonoBehaviour
    {
        public GameObject audioTarget;
        public AudioSource audioSource;
        public List<CustomAudioEvent> events = new List<CustomAudioEvent>();

        private void Start()
        {
            audioSource = gameObject.GetComponent<AudioSource>();

            hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        }

        private void Update()
        {
            if(audioTarget == null)
            {
                Destroy(gameObject);

                return;
            }
            
            transform.SetParent(audioTarget.transform);

            transform.position = audioTarget.transform.position;
            transform.rotation = audioTarget.transform.rotation;
        }
    }
}