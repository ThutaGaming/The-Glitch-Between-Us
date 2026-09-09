using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework.AudioManagement
{
    [AddComponentMenu("Akila/FPS Framework/Audio System/Audio Filter Area")]
    public class AudioFilterArea : MonoBehaviour
    {
        CharacterManager characterManager;

        private void OnTriggerStay(Collider collider)
        {
            if (collider.TryGetComponent<CharacterManager>(out CharacterManager characterManager))
            {
                this.characterManager = characterManager;

                OnAreaStayed(FindFirstObjectByType<AudioFiltersManager>());
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (characterManager) OnAreaExited(FindFirstObjectByType<AudioFiltersManager>());

            characterManager = null;
        }

        protected virtual void OnAreaStayed(AudioFiltersManager audioFiltersManager)
        {

        }

        protected virtual void OnAreaExited(AudioFiltersManager audioFiltersManager)
        {

        }
    }
}