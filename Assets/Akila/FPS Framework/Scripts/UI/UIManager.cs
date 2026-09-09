using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/UI/UI Mananger")]
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        public DamageableEffectsVisualizer DamagableVisualizer { get; set; }
        public PlayerCard PlayerCard { get; set; }
        public Hitmarker Hitmarker { get; set; }
        public KillFeed KillFeed { get; set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            DamagableVisualizer = GetComponentInChildren<DamageableEffectsVisualizer>();
            PlayerCard = GetComponentInChildren<PlayerCard>();
            Hitmarker = GetComponentInChildren<Hitmarker>();
            KillFeed = GetComponentInChildren<KillFeed>();
        }

        public void LoadGame(string name)
        {
            LoadingScreen.LoadScene(name);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}