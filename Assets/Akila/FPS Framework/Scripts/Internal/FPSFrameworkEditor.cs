using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Akila.FPSFramework.Internal
{
#if UNITY_EDITOR
    public static class FPSFrameworkEditor
    {
        public static Canvas FindOrCreateCanvas()
        {
            Canvas canvas = null;

            // 1. Try to find a Canvas in the active selection
            if (Selection.activeGameObject != null)
            {
                canvas = Selection.activeGameObject.GetComponentInParent<Canvas>();
            }

            // 2. If no Canvas is found in selection, search the scene
            if (!canvas)
            {
                canvas = GameObject.FindFirstObjectByType<Canvas>();
            }

            // 3. If no Canvas exists in the scene, create one
            if (!canvas)
            {
                canvas = CreateCanvas();
            }

            // Ensure an EventSystem exists
            EnsureEventSystemExists();

            // Set the Canvas render mode to ScreenSpaceOverlay (default)
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Set the found or created Canvas as the active selection in the editor
            Selection.activeGameObject = canvas.gameObject;

            return canvas;
        }

        public static Canvas CreateCanvas()
        {
            // Create the Canvas GameObject
            GameObject canvasObject = new GameObject("Canvas");

            // Add the required components
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            return canvas;
        }

        public static void EnsureEventSystemExists()
        {
            if (!GameObject.FindFirstObjectByType<EventSystem>())
            {
                // Create a new EventSystem GameObject if none exists
                GameObject eventSystemObject = new GameObject("EventSystem");
                eventSystemObject.AddComponent<EventSystem>();
                eventSystemObject.AddComponent<StandaloneInputModule>();
            }
        }


        [MenuItem(MenuPaths.Help, false, 100)]
        public static void OpenHelp()
        {
            Application.OpenURL("https://akila.gitbook.io/fps-framework/");
        }

        public static void EnterRenameMode()
        {
            EditorApplication.delayCall += () =>
            {
                EditorGUIUtility.editingTextField = true;
                EditorApplication.delayCall += () =>
                {
                    var editorWindow = EditorWindow.focusedWindow;
                    if (editorWindow != null)
                    {
                        editorWindow.SendEvent(new Event
                        {
                            keyCode = KeyCode.F2,
                            type = EventType.KeyDown
                        });
                    }
                };
            };
        }
    }
#endif
}