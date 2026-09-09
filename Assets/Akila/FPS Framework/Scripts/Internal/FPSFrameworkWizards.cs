#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEngine.UI;
using Akila.FPSFramework.UI;
using TMPro;
using Akila.FPSFramework.Animation;
using UnityEditor.Events;

namespace Akila.FPSFramework.Internal
{

    public static class FPSFrameworkWizards
    {
        private const string playerPrefabGUID = "33456be00e2110e45b173f528571344e";

        #region Player

        [MenuItem(MenuPaths.CreateFPSController, false, 2)]
        public static void CreateFPSController()
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(playerPrefabGUID);

            if (string.IsNullOrEmpty(prefabPath))
            {
                Debug.LogError("Could not find prefab with provided GUID.");
                return;
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogError("Prefab is null after loading. Check if the asset at the path is a valid prefab.");
                return;
            }

            GameObject player = GameObject.Instantiate(prefab);

            if (player == null)
            {
                Debug.LogError("Failed to instantiate prefab.");
                return;
            }

            player.name = "First Person Controller";

            // Set position in front of Scene view camera
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null && sceneView.camera != null)
            {
                Camera cam = sceneView.camera;
                player.transform.position = cam.transform.position + cam.transform.forward * 2.5f;
            }
            else
            {
                player.transform.position = Vector3.zero;
            }

            // Parent to currently selected object (optional)
            if (Selection.activeTransform != null)
            {
                player.transform.SetParent(Selection.activeTransform);
            }

            Selection.activeGameObject = player;

            Undo.RegisterCreatedObjectUndo(player, "Create FPS Player");
        }

        #endregion

        #region Animation

        [MenuItem(MenuPaths.CreateProceduralAnimation)]
        public static ProceduralAnimation CreateAnimation()
        {
            ProceduralAnimation newAnimation = new GameObject("New Procedural Animation").AddComponent<ProceduralAnimation>();
            newAnimation.transform.SetParent(Selection.activeTransform);
            newAnimation.transform.Reset();

            // Select the newly created GameObject in the hierarchy
            Selection.activeTransform = newAnimation.transform;

            FPSFrameworkEditor.EnterRenameMode();

            return newAnimation;
        }

        #endregion

        #region UI

        [MenuItem(MenuPaths.CreateCarouselSelector)]
        public static void CreateCarouselSelector()
        {
            Canvas canvas = FPSFrameworkEditor.FindOrCreateCanvas();
            GameObject holder = new GameObject("Carousel Selector");

            Debug.Log(canvas);

            holder.transform.SetParent(canvas.transform);

            RectTransform holderRectTransform = holder.gameObject.AddComponent<RectTransform>();

            holder.transform.SetParent(Selection.activeTransform);

            holderRectTransform.Reset();
            holderRectTransform.localScale = Vector3.one;

            holderRectTransform.sizeDelta = new Vector2(160, 45);



            Image holderBackground = holder.AddComponent<Image>();
            holderBackground.color = new Color(0, 0, 0, 0.39f);


            CarouselSelector dropdownSelector = holder.AddComponent<CarouselSelector>();
            //dropdownSelector.normalSelectionColor = new Color(0, 0, 0, 0.6f);
            //dropdownSelector.selectedSelectionColor = Color.white;
            dropdownSelector.AddOptions(new string[] { "Option A", "Option B" });


            RectTransform selections = new GameObject("Selections").AddComponent<RectTransform>();
            selections.SetParent(holder.transform);
            selections.Reset();

            HorizontalLayoutGroup selectionsHorizontalLayoutGroup = selections.gameObject.AddComponent<HorizontalLayoutGroup>();
            selectionsHorizontalLayoutGroup.spacing = 5;
            selectionsHorizontalLayoutGroup.childAlignment = TextAnchor.UpperRight;
            selectionsHorizontalLayoutGroup.childControlWidth = true;

            selections.anchorMin = new Vector2(0, 0);
            selections.anchorMax = new Vector2(1, 0);

            selections.anchoredPosition = new Vector3(0, 1.392f, 0);
            selections.sizeDelta = new Vector2(0, 2.784f);

            Image selected = new GameObject("Option").AddComponent<Image>();
            selected.rectTransform.SetParent(selections);
            selected.rectTransform.sizeDelta = new Vector2(160, 2.378f);

            selected.rectTransform.localScale = Vector2.one;


            TextMeshProUGUI label = new GameObject("Label").AddComponent<TextMeshProUGUI>();
            label.rectTransform.SetParent(holder.transform);
            label.rectTransform.Reset();
            label.rectTransform.localScale = Vector3.one;

            label.rectTransform.anchoredPosition = new Vector2(44.123f, 1.25f);

            label.rectTransform.sizeDelta = new Vector2(88.245f, 42.5f);

            label.rectTransform.anchorMax = new Vector2(0, 0.5f);
            label.rectTransform.anchorMin = new Vector2(0, 0.5f);

            label.text = "Option A";
            label.fontSize = 18;
            label.alignment = TextAlignmentOptions.Right;


            Button rightButton = new GameObject("Right Button").AddComponent<Button>();
            RectTransform rBRect = rightButton.gameObject.AddComponent<RectTransform>();
            rBRect.SetParent(holder.transform);
            rBRect.Reset();
            rBRect.localScale = Vector3.one;

            ColorBlock rbCB = new ColorBlock();


            rbCB.normalColor = new Color(0.56f, 0.56f, 0.56f, 1);
            rbCB.highlightedColor = new Color(0.18f, 0.18f, 0.18f, 1);
            rbCB.pressedColor = Color.gray;
            rbCB.selectedColor = Color.white;
            rbCB.colorMultiplier = 1;

            rbCB.fadeDuration = 0;

            rightButton.colors = rbCB;

            rBRect.anchoredPosition = new Vector2(-20.00002f, 1.525879e-05f);
            rBRect.sizeDelta = new Vector2(20, 20);

            rBRect.anchorMax = new Vector2(1, 0.5f);
            rBRect.anchorMin = new Vector2(1, 0.5f);
            rightButton.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));

            Image rBImage = rightButton.gameObject.AddComponent<Image>();
            rightButton.targetGraphic = rBImage;



            Button leftButton = new GameObject("Left Button").AddComponent<Button>();
            RectTransform lBRect = leftButton.gameObject.AddComponent<RectTransform>();
            lBRect.SetParent(holder.transform);
            lBRect.Reset();
            lBRect.localScale = Vector3.one;

            ColorBlock lbCB = new ColorBlock();


            lbCB.normalColor = new Color(0.56f, 0.56f, 0.56f, 1);
            lbCB.highlightedColor = new Color(0.18f, 0.18f, 0.18f, 1);
            lbCB.pressedColor = Color.gray;
            lbCB.selectedColor = Color.white;
            lbCB.colorMultiplier = 1;

            lbCB.fadeDuration = 0;

            leftButton.colors = lbCB;

            lBRect.anchoredPosition = new Vector2(-48.6f, 1.525879e-05f);
            lBRect.sizeDelta = new Vector2(20, 20);

            lBRect.anchorMax = new Vector2(1, 0.5f);
            lBRect.anchorMin = new Vector2(1, 0.5f);
            leftButton.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));

            Image lBImage = leftButton.gameObject.AddComponent<Image>();
            leftButton.targetGraphic = lBImage;


            dropdownSelector.label = label;
            dropdownSelector.rightButton = rightButton;
            dropdownSelector.leftButton = leftButton;
            //dropdownSelector.selectionGraphics = selected;
            //dropdownSelector.selectionHolder = selections;

            Undo.RegisterCreatedObjectUndo(holder, "Created CarouselSelector");

            Selection.activeTransform = holderRectTransform;
        }

        [MenuItem(MenuPaths.CreateSlider)]
        public static void CreateSlider()
        {
            Canvas canvas = FPSFrameworkEditor.FindOrCreateCanvas();
            GameObject holder = new GameObject("Slider");
            holder.transform.SetParent(canvas.transform);
            holder.layer = 5;

            RectTransform sliderRectTransform = holder.AddComponent<RectTransform>();
            sliderRectTransform.Reset();

            sliderRectTransform.sizeDelta = new Vector2(160, 20);

            Slider slider = holder.AddComponent<Slider>();

            Image background = new GameObject("Background").AddComponent<Image>();
            background.rectTransform.SetParent(holder.transform);
            background.rectTransform.Reset();

            background.color = new Color(0.18f, 0.18f, 0.18f, 1);
            background.rectTransform.anchorMax = new Vector2(1, 1);
            background.rectTransform.anchorMin = new Vector2(0, 0);

            background.rectTransform.sizeDelta = Vector2.one;

            RectTransform fillArea = new GameObject("Fill Area").AddComponent<RectTransform>();
            fillArea.SetParent(holder.transform);
            fillArea.Reset();

            fillArea.anchorMax = new Vector2(1, 1);
            fillArea.anchorMin = new Vector2(0, 0);

            fillArea.sizeDelta = Vector2.one;

            Image fill = new GameObject("Fill").AddComponent<Image>();
            fill.rectTransform.SetParent(fillArea.transform);
            fill.rectTransform.Reset();

            fill.color = Color.white;
            fill.rectTransform.anchorMax = new Vector2(0.5f, 1);
            fill.rectTransform.anchorMin = new Vector2(0, 0);

            fill.rectTransform.sizeDelta = Vector2.one;

            slider.fillRect = fill.rectTransform;
            slider.value = 0.5f;

            TextMeshProUGUI valueText = new GameObject("Value Text").AddComponent<TextMeshProUGUI>();
            valueText.text = "0.5";

            valueText.rectTransform.SetParent(holder.transform);
            valueText.rectTransform.Reset();

            valueText.rectTransform.anchoredPosition = new Vector2(-125f, 0);
            valueText.rectTransform.sizeDelta = new Vector2(67.587f, 20f);

            valueText.rectTransform.anchorMin = Vector2.one * 0.5f;
            valueText.rectTransform.anchorMax = Vector2.one * 0.5f;

            valueText.fontSize = 14;
            valueText.alignment = TextAlignmentOptions.Right;
            valueText.alignment = TextAlignmentOptions.Center;

            SliderToTMPro sliderToTMPro = holder.AddComponent<SliderToTMPro>();

            Selection.activeGameObject = holder;
        }
        #endregion
    }
}
   #endif