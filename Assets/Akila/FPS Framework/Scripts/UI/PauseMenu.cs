using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Akila.FPSFramework.UI
{
    [AddComponentMenu("Akila/FPS Framework/UI/Pause Menu")]
    public class PauseMenu : Menu
    {
        public InputAction pauseInputAction = new InputAction("[Keyboard] Escape", InputActionType.Button, "<Keyboard>/escape");

        [Space]
        public bool freezeOnPaused;
        public float freezeTransitionTime = 0.2f; // Duration to reach 0 time scale

        private float defaultTimeScale;

        public bool IsPaused => FPSFrameworkCore.IsPaused;

        protected override void Start()
        {
            base.Start();

            pauseInputAction.Enable();
            FPSFrameworkCore.IsPaused = false;
            defaultTimeScale = Time.timeScale;
        }

        protected override void Update()
        {
            base.Update();

            FPSFrameworkCore.IsFreezOnPauseActive = freezeOnPaused;

            if (pauseInputAction.triggered)
            {
                if (IsPaused) Unpause();
                else Pause();
            }

            if (!IsPaused) CloseMenu();

            if (freezeOnPaused)
            {
                float target = IsPaused ? 0f : defaultTimeScale;
                float maxChange = defaultTimeScale / freezeTransitionTime * Time.unscaledDeltaTime;
                Time.timeScale = Mathf.MoveTowards(Time.timeScale, target, maxChange);
            }

        }

        public void Pause()
        {
            FPSFrameworkCore.IsPaused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            OpenMenu();
        }

        public void Unpause()
        {
            if (IsOpen)
            {
                FPSFrameworkCore.IsPaused = false;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(Load(sceneName));
        }

        private IEnumerator Load(string sceneName)
        {
            if(freezeOnPaused)
            {
                Time.timeScale = defaultTimeScale;
            }

            yield return new WaitForFixedUpdate();

            LoadingScreen.LoadScene(sceneName);
        }
    }
}
