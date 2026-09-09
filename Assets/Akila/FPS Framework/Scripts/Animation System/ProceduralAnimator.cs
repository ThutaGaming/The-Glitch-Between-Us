using Akila.FPSFramework.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Akila.FPSFramework.Animation
{
    [AddComponentMenu("Akila/FPS Framework/Animation/Procedural Animator"), DisallowMultipleComponent]
    public class ProceduralAnimator : MonoBehaviour
    {
        public GameObject animationsHolder;
        public int frameRate = 165;
        [Range(0, 1)]
        public float weight = 1;
        [Range(0, 1)]
        public float positionWeight = 1;
        [Range(0, 1)]
        public float rotationWeight = 1;


        public bool isActive
        {
            get
            {
                RefreshClips();

                foreach (ProceduralAnimation anim in clips)
                    anim.IsActive = activeState;

                return activeState;
            }

            set
            {
                RefreshClips();

                foreach (ProceduralAnimation anim in clips)
                    anim.IsActive = value;

                activeState = value;
            }
        }

        private bool activeState = true;

        /// <summary>
        /// final position result from all clips
        /// </summary>
        private Vector3 _targetPosition;

        public Vector3 targetPosition
        {
            get
            {
                if (Time.timeScale <= 0)
                {
                    return Vector3.zero;
                }

                Vector3 result = Vector3.zero;

                if (clips.Count <= 0) return result;

                foreach (ProceduralAnimation clip in clips)
                {
                    if (FPSFrameworkUtility.IsVector3Nan(clip.TargetPosition))
                    {
                        Debug.LogError($"{clip.Name} outputs Nan position. Result animation position will be ignored.", clip);
                    }
                    else if (!clip.Isolate)
                    {
                        result += clip.TargetPosition;
                    }
                }

                if (activeState)
                    _targetPosition = Vector3.Lerp(Vector3.zero, result, weight * positionWeight);

                return _targetPosition;
            }
            set
            {
                _targetPosition = value;
            }
        }

        /// <summary>
        /// final rotation result from all clips
        /// </summary>
        private Vector3 _targetRotation;

        public Vector3 targetRotation
        {
            get
            {
                if(Time.timeScale <= 0)
                {
                    return Vector3.zero;
                }

                Vector3 result = Vector3.zero;

                if (clips.Count <= 0) return result;
                foreach (ProceduralAnimation clip in clips)
                {
                    if(FPSFrameworkUtility.IsVector3Nan(clip.TargetPosition))
                    {
                        Debug.LogError($"{clip.Name} outputs Nan rotation. Result animation rotation will be ignored.", clip);
                    }
                    else if(!clip.Isolate)
                    {
                        result += clip.TargetRotation;
                    }
                }

                if (activeState)
                    _targetRotation = Vector3.Lerp(Vector3.zero, result, weight * rotationWeight);

                return _targetRotation;
            }
            set
            {
                _targetRotation = value;
            }
        }

        [HideInInspector]
        public List<ProceduralAnimation> clips = new List<ProceduralAnimation>();

        public Vector3 defaultPosition { get; protected set; }
        public Vector3 defaultRotation {  get; protected set; }
        
        public bool IsDefaultingInPosition(Vector3 tolerance, bool x = true, bool y = true, bool z = true)
        {
            Vector3 positionDifference = targetPosition - defaultPosition;

            if (positionDifference.x > tolerance.x && x || positionDifference.y > tolerance.y && y || positionDifference.z > tolerance.z && z) 
                return false;

            return true;
        }

        public bool IsDefaultingInRotation(float tolerance, bool x = true, bool y = true, bool z = true)
        {
            Vector3 rotationDifference = targetRotation - defaultRotation;

            if (!x) rotationDifference.x = 0;
            if (!y) rotationDifference.y = 0;
            if (!z) rotationDifference.z = 0;

            return rotationDifference.magnitude <= tolerance;
        }

        private void Awake()
        {
            defaultPosition = transform.localPosition;
            defaultRotation = transform.localEulerAngles;

            if(animationsHolder == null)
            {
                animationsHolder = gameObject;

                Debug.LogError($"AnimationHolder on {this} is not set. Setting it automaticly to self.", gameObject);
            }

            RefreshClips();
        }

        private void OnEnable()
        {
            RefreshClips();
        }

        public float elapsedTime { get; set; } = 0;

        int maxFramerate;

        private void Update()
        {
            if (Time.timeScale <= 0) return;

            elapsedTime += Time.unscaledDeltaTime;

            maxFramerate = Mathf.Clamp(frameRate, 0, FPSFrameworkSettings.maxAnimationFramerate);

            if (elapsedTime >= 1f / maxFramerate)
            {
                elapsedTime = 0f;

                UpdateSingleFrame();
            }
        }

        private void UpdateSingleFrame()
        {
            if (Time.timeScale <= 0) return;

            Vector3 position = defaultPosition + targetPosition;

            Quaternion rotation = Quaternion.identity;


            if (FPSFrameworkUtility.IsVector3Nan(position) == false)
                transform.localPosition = position;
            else
                Debug.LogError($"Total outputs Nan position. Result animation position will be ignored.", this);

            if(!FPSFrameworkUtility.IsVector3Nan(defaultRotation) && !FPSFrameworkUtility.IsVector3Nan(targetRotation))
            {
                rotation = Quaternion.Euler(defaultRotation + targetRotation);
            }
            else
            {
                Debug.LogError($"Total outputs Nan rotation. Result animation rotation will be ignored.", this);
            }

            if (FPSFrameworkUtility.IsVector3Nan(defaultRotation + targetRotation) == false)
                transform.localRotation = rotation;
        }

        public void Play(string name)
        {
            ProceduralAnimation animation = GetAnimation(name);

            animation.Play();
        }

        public void Play(string name, float fixedTime)
        {
            ProceduralAnimation animation = GetAnimation(name);

            if (animation)
                animation.Play(fixedTime);
        }

        public void Pause(string name)
        {
            ProceduralAnimation animation = GetAnimation(name);

            if (animation)
                animation.Pause();
        }

        public void Stop(string name)
        {
            ProceduralAnimation animation = GetAnimation(name);

            if (animation)
                animation.Stop();
        }

        /// <summary>
        /// returns all the animations clip for this animator in a List of ProceduralAnimationClip and refreshes the animtor clips 
        /// </summary>
        public List<ProceduralAnimation> RefreshClips()
        {
            clips = animationsHolder.GetComponentsInChildren<ProceduralAnimation>().ToList();

            return clips.ToList();
        }

        public ProceduralAnimation GetAnimation(string name)
        {
            RefreshClips();

            ProceduralAnimation animation = clips.Find(clip => clip.Name == name);

            return animation;
        }
    }
}