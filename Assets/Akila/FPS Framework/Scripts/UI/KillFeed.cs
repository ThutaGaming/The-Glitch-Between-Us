using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/UI/Kill Feed")]
    public class KillFeed : MonoBehaviour
    {
        public KillTag counter;
        public KillTag Tag;
        public Transform tagsHolder;
        public bool useSFX;
        public AudioClip killSFX;

        public AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            Tag.gameObject.SetActive(false);
        }

        public void Show(string killerName, int killerKilsCount, string victimName)
        {
            counter.Show(killerKilsCount, victimName);

            KillTag newTag = Instantiate(Tag, tagsHolder);

            newTag.gameObject.SetActive(true);

            newTag.Show(killerKilsCount, victimName);

            audioSource.Stop();

            if (killSFX)
                audioSource.PlayOneShot(killSFX);
        }
    }
}