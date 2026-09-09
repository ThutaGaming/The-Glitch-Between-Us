using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Akila.FPSFramework
{
    public static class ExtensionMethods
    {
        #region Component
        public static T CopyComponent<T>(this GameObject destination, T original) where T : Component
        {
            // Reuse existing component if present
            T copy = destination.GetComponent<T>();
            if (copy == null)
                copy = destination.AddComponent<T>();

            System.Type type = typeof(T);

            // Copy all fields
            foreach (var field in type.GetFields(System.Reflection.BindingFlags.Public
                                                | System.Reflection.BindingFlags.NonPublic
                                                | System.Reflection.BindingFlags.Instance))
            {
                field.SetValue(copy, field.GetValue(original));
            }

            // Copy all writable properties
            foreach (var prop in type.GetProperties(System.Reflection.BindingFlags.Public
                                                   | System.Reflection.BindingFlags.NonPublic
                                                   | System.Reflection.BindingFlags.Instance))
            {
                if (prop.CanWrite && prop.GetSetMethod(true) != null)
                {
                    try { prop.SetValue(copy, prop.GetValue(original, null), null); }
                    catch { /* ignore unsupported ones */ }
                }
            }

            return copy;
        }
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            if (component == null)
                throw new System.ArgumentNullException(nameof(component));

            var comp = component.GetComponent<T>();
            if (comp == null)
                comp = component.gameObject.AddComponent<T>();

            return comp;
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (gameObject == null)
                throw new System.ArgumentNullException(nameof(gameObject));

            var comp = gameObject.GetComponent<T>();
            if (comp == null)
                comp = gameObject.AddComponent<T>();

            return comp;
        }


        /// <summary>
        /// Tries to find a component of type <typeparamref name="T"/> on the current GameObject,
        /// then its children, and then its parents.
        /// Returns the first component found or null if none is found.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="component">The Component whose GameObject hierarchy to search.</param>
        /// <param name="includeInactive">Whether to include inactive GameObjects when searching children and parents.</param>
        /// <returns>The first found component of type T or null if none found.</returns>
        public static T SearchFor<T>(this Component component, bool includeInactive = false, bool skipParent = false)
        {
            if (component == null) return default(T);

            var comp = component.GetComponent<T>();

            if (comp != null) return comp;

            var childComp = component.GetComponentInChildren<T>(includeInactive);

            if (childComp != null) return childComp;

            var parentComp = component.GetComponentInParent<T>(includeInactive);

            if(parentComp != null && !skipParent) return parentComp;

            return comp;
        }

        /// <summary>
        /// Searches for all components of type <typeparamref name="T"/> on the current GameObject,
        /// all its children, and all its parents.
        /// Returns a list of unique components found.
        /// </summary>
        /// <typeparam name="T">The type of components to search for.</typeparam>
        /// <param name="component">The Component whose GameObject hierarchy to search.</param>
        /// <param name="includeInactive">Whether to include inactive GameObjects when searching children and parents.</param>
        /// <returns>A list of unique components of type T found in self, children, and parents.</returns>
        public static List<T> SearchForAll<T>(this Component component, bool includeInactive = false)
        {
            var results = new List<T>();

            // Get all components on self
            results.AddRange(component.GetComponents<T>());

            // Get all components in children
            results.AddRange(component.GetComponentsInChildren<T>(includeInactive));

            // Get all components in parents
            results.AddRange(component.GetComponentsInParent<T>(includeInactive));

            // Remove duplicates because self, children, and parent searches may overlap
            var uniqueResults = new HashSet<T>(results);

            return new List<T>(uniqueResults);
        }

        /// <summary>
        /// Performs a deep search for a component of type <typeparamref name="T"/> starting
        /// from the current GameObject and its children, then traverses up the parent hierarchy.
        /// For each parent, checks the parent itself and then all of its children.
        /// Returns the first component found or null if none is found.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="component">The Component whose GameObject hierarchy to search.</param>
        /// <param name="includeInactive">Whether to include inactive GameObjects when searching children and parents.</param>
        /// <returns>The first found component of type T or null if none found.</returns>
        public static T DeepSearch<T>(this Component component, bool includeInactive = false)
        {
            // 1. Search on this component's GameObject and all children
            var found = component.gameObject.GetComponentInChildren<T>(includeInactive);

            if (found != null)
                return found;

            // 2. Traverse parents up to root
            Transform currentParent = component.transform.parent;

            while (currentParent != null)
            {
                // 3a. Check if the parent itself has the component
                found = currentParent.GetComponent<T>();
                if (found != null)
                    return found;

                // 3b. Search all children of this parent (deep), including inactive if specified
                found = currentParent.GetComponentInChildren<T>(includeInactive);
                if (found != null)
                    return found;

                currentParent = currentParent.parent;
            }

            // Not found anywhere
            return default;
        }

        /// <summary>
        /// Performs a deep search for all components of type <typeparamref name="T"/> starting
        /// from the current GameObject and its children, then traverses up the parent hierarchy.
        /// For each parent, adds components found on the parent itself and all its children.
        /// Returns a list of unique components found.
        /// </summary>
        /// <typeparam name="T">The type of components to search for.</typeparam>
        /// <param name="component">The Component whose GameObject hierarchy to search.</param>
        /// <param name="includeInactive">Whether to include inactive GameObjects when searching children and parents.</param>
        /// <returns>A list of unique components of type T found in the deep search.</returns>
        public static List<T> DeepSearchAll<T>(this Component component, bool includeInactive = false)
        {
            var results = new HashSet<T>();

            // 1. Search this component's GameObject and its entire children
            var foundOnSelfAndChildren = component.gameObject.GetComponentsInChildren<T>(includeInactive);
            foreach (var item in foundOnSelfAndChildren)
                results.Add(item);

            // 2. Traverse parents up to root
            Transform currentParent = component.transform.parent;

            while (currentParent != null)
            {
                // a) Add components on the parent GameObject itself
                var foundOnParent = currentParent.GetComponents<T>();
                foreach (var item in foundOnParent)
                    results.Add(item);

                // b) Add components in all children of the parent GameObject (deep)
                var foundOnParentsChildren = currentParent.GetComponentsInChildren<T>(includeInactive);
                foreach (var item in foundOnParentsChildren)
                    results.Add(item);

                currentParent = currentParent.parent;
            }

            return new List<T>(results);
        }
        #endregion

        #region Transform
        /// <summary>
        /// Sets transform position to given position. 
        /// If local is set to true, the function will set local position instead.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="position">Target position</param>
        /// <param name="local">If true position is going to chnage in local space insted of global space</param>
        public static void SetPosition(this Transform transform, Vector3 position, bool local = false)
        {
            if (local) transform.localPosition = position;
            else transform.position = position;
        }

        /// <summary>
        /// Sets transform rotation to given rotation. 
        /// If local is set to true, the function will set local rotation instead.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="rotation">Target rotation</param>
        /// <param name="local">If true rotation is going to chnage in local space insted of global space</param>
        public static void SetRotation(this Transform transform, Quaternion rotation, bool local = false)
        {
            if (local) transform.localRotation = rotation;

            else transform.rotation = rotation;
        }

        /// <summary>
        /// Resets transform position, rotation & scale
        /// </summary>
        /// <param name="transform"></param>
        public static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        ///Adds a new game object as a child of the transform, with the default name "GameObject".
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Transform CreateChild(this Transform transform)
        {
            Transform children = new GameObject("GameObject").transform;

            children.parent = transform;
            children.Reset();

            return children;
        }

        /// <summary>
        /// Adds a new game object as a child of the transform, with the given name.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="name">Name of the child</param>
        /// <returns></returns>
        public static Transform CreateChild(this Transform transform, string name)
        {
            Transform children = new GameObject(name).transform;

            children.parent = transform;
            children.Reset();

            return children;
        }


        /// <summary>
        /// Adds a list of new game object as children of the transform, with the names given in the parameter "names". 
        /// If parentAll is true, each new child will be parented it the child before it.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="names">Target names the more names you have the more childern you get</param>
        /// <param name="parentAll">If true all childern will be child if each other</param>
        /// <returns></returns>
        public static Transform[] CreateChildren(this Transform transform, string[] names, bool parentAll = false)
        {
            List<Transform> transforms = new List<Transform>();

            for (int i = 0; i < names.Length; i++)
            {
                Transform child = CreateChild(transform, names[i]);
                transforms.Add(child);

                if (parentAll)
                {
                    if (i > 1)
                    {
                        transforms[1].SetParent(transforms[0]);

                        child.SetParent(transforms[transforms.Count - 2]);
                    }
                }
            }

            return transforms.ToArray();
        }

        /// <summary>
        /// Recursively searches for a child Transform by name.
        /// </summary>
        /// <param name="transform">The root Transform to start searching from.</param>
        /// <param name="childName">The name of the child to search for.</param>
        /// <returns>The Transform of the child if found, otherwise null.</returns>
        public static Transform FindDeepChild(this Transform transform, string childName)
        {
            foreach (Transform child in transform)
            {
                if (child.name == childName)
                    return child;

                Transform result = FindDeepChild(child, childName);
                if (result != null)
                    return result;
            }
            return null;
        }

        /// <summary>
        /// Recursively gets all child Transforms of the given parent Transform.
        /// </summary>
        /// <param name="transform">The root Transform to search under.</param>
        /// <returns>A list of all child Transforms (including nested ones).</returns>
        public static List<Transform> GetAllChildren(this Transform transform)
        {
            List<Transform> allChildren = new List<Transform>();

            GetChildrenRecursive(transform, allChildren);
            return allChildren;
        }

        private static void GetChildrenRecursive(this Transform transform, List<Transform> list)
        {
            foreach (Transform child in transform)
            {
                list.Add(child);
                GetChildrenRecursive(child, list);
            }
        }

        /// <summary>
        /// Destroys all children in transform.
        /// </summary>
        /// <param name="transform"></param>
        public static void ClearChildren(this Transform transform, bool includeInactive = true, string[] exceptions = null)
        {
            for (int i = transform.childCount - 1; i >= 0; i--) // backwards is correct
            {
                Transform child = transform.GetChild(i);

                // Skip if this child's name is in exceptions
                if (exceptions != null && exceptions.Contains(child.name))
                    continue;

                // Only destroy if includeInactive is true OR the child is active
                if (includeInactive || child.gameObject.activeSelf)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }



        /// <summary>
        /// Sets transform position and rotation to given position and rotation. 
        /// If local is set to true, the function will set local position & rotation instead.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="local"></param>
        public static void SetPositionAndRotation(this Transform transform, Vector3 position, Quaternion rotation, bool local = false)
        {
            if (!local)
            {
                transform.position = position;
                transform.rotation = rotation;
            }
            else
            {
                transform.localPosition = position;
                transform.localRotation = rotation;
            }
        }

        /// <summary>
        /// Sets transform position and euler angles to given position and rotation. 
        /// If local is set to true, the function will set local position & rotation instead.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="position"></param>
        /// <param name="eulerAngles"></param>
        /// <param name="local"></param>
        public static void SetPositionAndRotation(this Transform transform, Vector3 position, Vector3 eulerAngles, bool local = false)
        {
            if (!local)
            {
                transform.position = position;
                transform.eulerAngles = eulerAngles;
            }
            else
            {
                transform.localPosition = position;
                transform.localEulerAngles = eulerAngles;
            }
        }

        /// <summary>
        /// Returns a vector for the given direction. If direction is set to Vector3Direction.Up it will return, transform.up.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector3 GetDirection(this Transform transform, Vector3Direction direction)
        {
            Vector3 dir = Vector3.zero;

            switch (direction)
            {
                case Vector3Direction.forward:
                    dir = transform.forward;
                    break;

                case Vector3Direction.back:
                    dir = -transform.forward;

                    break;

                case Vector3Direction.right:
                    dir = transform.right;

                    break;

                case Vector3Direction.left:
                    dir = -transform.right;

                    break;

                case Vector3Direction.up:
                    dir = transform.up;

                    break;

                case Vector3Direction.down:
                    dir = -transform.up;

                    break;
            }

            return dir;
        }
        #endregion

        #region Character Controller
        /// <summary>
        /// Returns true of the character controller isn't moving.
        /// </summary>
        /// <param name="characterController"></param>
        /// <returns></returns>
        public static bool IsVelocityZero(this CharacterController characterController)
        {
            //check if player is standing still if yes set to true else set to false
            return characterController.velocity.magnitude <= 0;
        }
        #endregion

        #region Rigidbody
        /// <summary>
        /// Returns true of the rigidbody isn't moving.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <returns></returns>
        public static bool IsVelocityZero(this Rigidbody rigidbody)
        {
            //check if rigidbody is not moving if yes set to true else set to false
            return rigidbody.linearVelocity.magnitude <= 0;
        }
        #endregion

        #region Dropdown
        /// <summary>
        /// Adds a new option to the dropdown without having to create an OptionData. 
        /// The "option" is the text that is going to be added as a new option
        /// </summary>
        /// <param name="dropdown"></param>
        /// <param name="option"></param>
        public static void AddOption(this Dropdown dropdown, string option)
        {
            dropdown.AddOptions(new List<Dropdown.OptionData>() { new Dropdown.OptionData() { text = option } });
        }
        #endregion

        #region Input Action
        /// <summary>
        /// Checks for douple clicks and sets targetValue to true if the user has douple clicked
        /// </summary>
        /// <param name="inputAction"></param>
        /// <param name="targetValue"></param>
        /// <param name="lastClickTime"></param>
        /// <returns></returns>
        public static void HasDoupleClicked(this InputAction inputAction, ref bool targetValue, ref float lastClickTime, float maxClickTime = 0.5f)
        {
            if (inputAction.triggered)
            {
                float timeSinceLastSprintClick = Time.time - lastClickTime;

                if (timeSinceLastSprintClick < maxClickTime)
                {
                    targetValue = true;
                }

                lastClickTime = Time.time;
            }

            if (inputAction.IsPressed() == false) targetValue = false;
        }
        #endregion

        #region Resolution
        /// <summary>
        /// Returns a string with the resolution and refresh rate, given like this: 1920x1080 165Hz
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static string GetDetails(this Resolution resolution)
        {
            return $"{resolution.height}x{resolution.width} {resolution.refreshRateRatio.value}Hz";
        }
        #endregion

        #region Animator
        /// <summary>
        /// Checks if the specified animation state is currently playing in the animator.
        /// </summary>
        /// <param name="animationStateName">The name of the animation state to check if it's playing.</param>
        /// <returns>True if the specified animation state is currently playing; otherwise, false.</returns>
        public static bool IsPlaying(this Animator animator, string animationStateName)
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName(animationStateName);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor Only - Adds a parameter to the animator's controller with the specified name and type.
        /// If the parameter already exists and overwrite is false, a message is logged and the parameter is not added.
        /// </summary>
        /// <param name="name">The name of the parameter to add.</param>
        /// <param name="type">The type of the parameter (e.g., Float, Int, Bool).</param>
        /// <param name="overwrite">Whether to overwrite the parameter if it already exists.</param>
        public static void AddParameter(this Animator animator, string name, AnimatorControllerParameterType type, bool overwrite)
        {
            // Check if the animator controller already has a parameter with the given name
            if (!overwrite && HasParameter(animator, name))
            {
                // If overwrite is false and the parameter already exists, log a message and exit
                Debug.Log($"Animator on {animator.gameObject.name} already has Parameter with the name ({name}).");
                return;
            }

            // Get the AnimatorController from the runtimeAnimatorController
            UnityEditor.Animations.AnimatorController animatorController = (UnityEditor.Animations.AnimatorController)animator.runtimeAnimatorController;

            // Create and configure a new AnimatorControllerParameter
            AnimatorControllerParameter parameter = new AnimatorControllerParameter
            {
                type = type,
                name = name
            };

            // Add the new parameter to the animator controller
            animatorController.AddParameter(parameter);
        }


        /// <summary>
        /// Editor Only - Checks if the specified parameter exists in the animator's parameter list.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to check for.</param>
        /// <returns>True if the parameter exists; otherwise, false.</returns>
        public static bool HasParameter(this Animator animator, string parameterName)
        {
            // Loop through each parameter in the animator's parameters list
            foreach (AnimatorControllerParameter param in animator.parameters)
            {
                // Check if the parameter's name matches the given name
                if (param.name == parameterName)
                    return true; // Parameter found
            }
            return false; // Parameter not found
        }
#endif

        #endregion

        #region List
        /// <summary>
        /// Gets the maximum valid index for the given list (i.e., Count - 1), clamped to a minimum of 0.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to get the length from.</param>
        /// <returns>The maximum valid index of the list, or 0 if the list is empty.</returns>
        public static int GetLength<T>(this List<T> list)
        {
            int length = list.Count - 1;

            length = Mathf.Clamp(length, 0, list.Count - 1);

            return length;
        }

        /// <summary>
        /// Moves an element from one index to another within the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list containing the element to move.</param>
        /// <param name="oldIndex">The current index of the element.</param>
        /// <param name="newIndex">The target index to move the element to.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when either index is outside the valid range.</exception>
        public static void MoveElement<T>(this List<T> list, int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= list.Count ||
                newIndex < 0 || newIndex >= list.Count)
            {
                throw new ArgumentOutOfRangeException("Indices are out of range.");
            }

            T element = list[oldIndex];
            list.RemoveAt(oldIndex);

            // Adjust targetIndex if necessary after removal
            if (newIndex > oldIndex)
            {
                newIndex--;
            }

            list.Insert(newIndex, element);
        }

        /// <summary>
        /// Moves the element at the specified index one position up (towards the start of the list).
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list containing the element.</param>
        /// <param name="oldIndex">The index of the element to move up.</param>
        public static void MoveElementUp<T>(this List<T> list, int oldIndex)
        {
            if (oldIndex > 0 && oldIndex < list.Count)
            {
                (list[oldIndex - 1], list[oldIndex]) = (list[oldIndex], list[oldIndex - 1]);
            }
        }

        /// <summary>
        /// Moves the element at the specified index one position down (towards the end of the list).
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list containing the element.</param>
        /// <param name="oldIndex">The index of the element to move down.</param>
        public static void MoveElementDown<T>(this List<T> list, int oldIndex)
        {
            if (oldIndex >= 0 && oldIndex < list.Count - 1)
            {
                (list[oldIndex], list[oldIndex + 1]) = (list[oldIndex + 1], list[oldIndex]);
            }
        }

        #endregion

        #region Button

#if UNITY_EDITOR
        [MenuItem("CONTEXT/Button/Upgrade")]
        public static void Upgrade()
        {
            GameObject obj = Selection.activeGameObject;

            Button button = obj.GetComponent<Button>();

            InteractiveButton interactiveButton = (InteractiveButton)Undo.AddComponent(obj, typeof(InteractiveButton));


            interactiveButton.interactable = button.interactable;
            interactiveButton.targetGraphics = button.targetGraphic;
            interactiveButton.targetText = obj.GetComponentInChildren<TextMeshProUGUI>();

            interactiveButton.normalGraphicsColor = button.colors.normalColor;
            interactiveButton.highlightedGraphicsColor = button.colors.highlightedColor;
            interactiveButton.selectedGraphicsColor = button.colors.selectedColor;
            interactiveButton.disabledGraphicsColor = button.colors.disabledColor;

            interactiveButton.fadeDuration = button.colors.fadeDuration;

            interactiveButton.onClick = button.onClick;

            Undo.DestroyObjectImmediate(button);
        }

#endif
#endregion
    }
}