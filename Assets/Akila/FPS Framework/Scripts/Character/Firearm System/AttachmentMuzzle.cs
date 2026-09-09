using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Weapons/Attachments/Muzzle")]
    public class AttachmentMuzzle : FirearmAttachment
    {
        public AudioProfile fireSFX;
        public ParticleSystem[] muzzleEffects;

        public Audio fireAudio;

        private void Start()
        {
            fireAudio = new Audio();
            fireAudio.Setup(gameObject, fireSFX);
        }

        private void Update()
        {
            if (firearm && attachment.IsActive())
                firearm.currentFireAudio = fireAudio;
        }

        private void OnDisable()
        {
            if (firearm)
                firearm.currentFireAudio = firearm.fireAudio;
        }
    }
}