using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;
namespace Akila.FPSFramework.Animation
{
    [AddComponentMenu("Akila/FPS Framework/Animation/Procedural Animation")]
    public class ProceduralAnimation : MonoBehaviour
    {
        public enum AnimationType
        {
            Override,
            Additive
        }

        [System.Flags]
        public enum AnimationOptions
        {
            None = 0,
            Loop = 1 << 0,
            AutoStop = 1 << 1,
            PerModifierConnections = 1 << 2,
            PlayOnAwake = 1 << 3,
            UnidirectionalPlay = 1 << 4,
            ResetOnPlayed = 1 << 5,
            Isolate = 1 << 6
        }

        public string Name = "New Procedural Animation";
        public float length = 0.15f;
        public float weight = 1;

        [FormerlySerializedAs("animationOptions")]
        public AnimationOptions options = AnimationOptions.PerModifierConnections | AnimationOptions.ResetOnPlayed;

        public AnimationType isolationMode = AnimationType.Override;
        public UpdateMode isolationUpdateMode = UpdateMode.Update;

        public TriggerType triggerType;
        public InputAction triggerInputAction = new InputAction();

        public ProceduralAnimationEvents events = new ProceduralAnimationEvents();
        public List<CustomProceduralAnimationEvent> customEvents = new List<CustomProceduralAnimationEvent>();
        public List<ProceduralAnimationConnection> connections = new List<ProceduralAnimationConnection>();

        public MoveAnimationModifier[] moveAnimationModifiers { get; protected set; }
        public SpringAnimationModifier[] springAnimationModifiers { get; protected set; }
        public KickAnimationModifier[] kickAnimationModifiers { get; protected set; }
        public SwayAnimationModifier[] swayAnimationModifiers { get; protected set; }
        public WaveAnimationModifier[] waveAnimationModifiers { get; protected set; }
        public OffsetAnimationModifier[] offsetAnimationModifiers { get; protected set; }

        public bool IsActive { get; set; } = true;

        public bool Isolate
        {
            get
            {
                return options.HasFlag(AnimationOptions.Isolate);
            }
        }

        public bool AutoStop
        {
            get
            {
                return options.HasFlag(AnimationOptions.AutoStop);
            }

            set
            {
                if(value)
                {
                    options |= AnimationOptions.AutoStop; // Add flag
                }
                else
                {
                    options &= ~AnimationOptions.AutoStop; // Remove flag
                }
            }
        }

        public bool PerModifierConnections
        {
            get
            {
                return options.HasFlag(AnimationOptions.PerModifierConnections);
            }

            set
            {
                if (value)
                {
                    options |= AnimationOptions.PerModifierConnections; // Add flag
                }
                else
                {
                    options &= ~AnimationOptions.PerModifierConnections; // Remove flag
                }
            }
        }

        public bool loop
        {
            get
            {
                return options.HasFlag(AnimationOptions.Loop);
            }

            set
            {
                if (value)
                {
                    options |= AnimationOptions.Loop; // Add flag
                }
                else
                {
                    options &= ~AnimationOptions.Loop; // Remove flag
                }
            }
        }

        /// <summary>
        /// final position result for this clip
        /// </summary>
        public Vector3 TargetPosition
        {
            get
            {
                return GetTargetModifiersPosition() * weight * FPSFrameworkSettings.globalAnimationWeight;
            }
        }

        /// <summary>
        /// final rotation result for this clip
        /// </summary>
        public Vector3 TargetRotation
        {
            get
            {
                return GetTargetModifiersRotation() * weight * FPSFrameworkSettings.globalAnimationWeight;
            }
        }

        /// <summary>
        /// current animation progress by value from 0 to 1
        /// </summary>
        public float progress { get; set; }

        private bool internal_isPlaying;

        private bool internal_IsPlaying
        {
            get
            {
                return internal_isPlaying;
            }

            set
            {
                triggerType = TriggerType.None;

                if (HasToAvoid())
                    internal_isPlaying = false;
                else
                {
                    internal_isPlaying = value;
                }
            }
        }

        public bool IsPlaying
        {
            get
            {
                return internal_IsPlaying;
            }

            set
            {
                IsOverridden = true;
                triggerType = TriggerType.None;

                internal_IsPlaying = value;
            }
        }

        public bool IsOverridden { get; protected set; }


        public bool isPaused { get; set; }

        private bool isTrigged;

        //acutal velocity
        private float currentVelocity;

        /// <summary>
        /// current animation movement speed
        /// </summary>
        public float velocity { get => currentVelocity; }

        /// <summary>
        /// List of all modifieres applied to this animation
        /// </summary>
        public List<ProceduralAnimationModifier> modifiers { get; set; } = new List<ProceduralAnimationModifier>();
        
        internal bool internalAlwaysStayIdle { get; set; }

        public bool ForceIdle { get; set; }

        private Vector3 defaultPosition;
        private Quaternion defaultRotation;

        private void Awake()
        {
            RefreshModifiers();
            triggerInputAction.Enable();

            foreach (ProceduralAnimationModifier modifier in modifiers)
            {
                modifier.targetAnimation = this;
            }

            moveAnimationModifiers = GetComponents<MoveAnimationModifier>();
            springAnimationModifiers = GetComponents<SpringAnimationModifier>();
            kickAnimationModifiers = GetComponents<KickAnimationModifier>();
            swayAnimationModifiers = GetComponents<SwayAnimationModifier>();
            waveAnimationModifiers = GetComponents<WaveAnimationModifier>();
            offsetAnimationModifiers = GetComponents<OffsetAnimationModifier>();

            defaultPosition = transform.localPosition;
            defaultRotation = transform.localRotation;
        }

        private void OnEnable()
        {
            if(options.HasFlag(AnimationOptions.PlayOnAwake))
            {
                Play(0);
            }
        }

        private void Start()
        {
            GetComponentInParent<ProceduralAnimator>()?.RefreshClips();
        }

        bool isTriggred;
        float lastTriggerTime;

        private void Update()
        {
            if (isolationUpdateMode == UpdateMode.Update)
                Tick();
        }

        private void FixedUpdate()
        {
            if (isolationUpdateMode == UpdateMode.FixedUpdate)
                Tick();
        }

        private void LateUpdate()
        {
            if(isolationUpdateMode == UpdateMode.LateUpdate)
                Tick();
        }

        private Vector3 isolatedPosition;
        private Quaternion isolatedRotation;

        public void Tick()
        {
            if (IsActive == false) return;

            //Handles the custom events and progress for this animation.
            HandleEvents();

            foreach(ProceduralAnimationConnection connection in connections)
            {
                if(connection.type == ProceduralAnimationConnectionType.PauseInIdle)
                {
                    if (!connection.target.internal_IsPlaying)
                        Pause();
                    else
                        Unpause();
                }

                if(connection.type == ProceduralAnimationConnectionType.PauseInTrigger)
                {
                    if (connection.target.internal_IsPlaying)
                        Pause();
                    else
                        Unpause();
                }
            }

            if (triggerType == TriggerType.Hold)
            {
                if (triggerInputAction.IsPressed() && progress < 0.9f) Play();
                else Stop();
            }

            if (triggerType == TriggerType.Tab)
            {
                if (triggerInputAction.triggered) isTrigged = !isTrigged;

                if (isTrigged) Play();
                else Stop();
            }

            if (triggerType == TriggerType.DoubleTab)
            {
                triggerInputAction.HasDoupleClicked(ref isTrigged, ref lastTriggerTime, 0.3f);

                if (isTrigged) Play();
                else Stop();
            }

            if (triggerType == TriggerType.Trigger)
            {
                if (triggerInputAction.triggered)
                {
                    Play(0);
                }
            }

            if (!isPaused)
                UpdateProgress();

            if (options.HasFlag(AnimationOptions.Loop) && progress >= 0.999f)
            {
                progress = 0;
            }

            if (options.HasFlag(AnimationOptions.AutoStop) && progress >= 0.999f || HasToAvoid())
            {
                Stop();
            }

            if (options.HasFlag(AnimationOptions.Isolate))
            {
                isolatedPosition = TargetPosition;
                isolatedRotation = Quaternion.Euler(TargetRotation);

                if(isolationMode == AnimationType.Additive)
                {
                    isolatedPosition += transform.localPosition;
                    isolatedRotation *= transform.localRotation;
                }
                else
                {
                    isolatedPosition += defaultPosition;
                    isolatedRotation *= defaultRotation;
                }

                transform.localPosition = TargetPosition + defaultPosition;
                transform.localRotation = isolatedRotation;
            }
        }

        public void Play(float fixedTime = -1)
        {
            if (IsOverridden) return;

            if (options.HasFlag(AnimationOptions.UnidirectionalPlay) && progress > 0.1f)
                return;

            foreach(ProceduralAnimationConnection connection in connections)
            {
                if(connection.target == null)
                {
                    Debug.LogError($"[Procedural Animation] Connection's target reference is null or missing on {gameObject.name}. This instance will be ignored.", gameObject);
                }
            }

            if (!HasToAvoid())
            {
                isPaused = false;
                internal_isPlaying = true;
            }

            if (options.HasFlag(AnimationOptions.ResetOnPlayed))
            {
                progress = 0;
            }
            else if (fixedTime >= 0)
            {
                progress = fixedTime;
            }

            events.OnPlay?.Invoke();
        }

        public void Pause()
        {
            isPaused = true;
        }

        public void Unpause()
        {
            isPaused = false;
        }

        public void Stop()
        {
            internal_isPlaying = false;
        }

        private void UpdateProgress()
        {
            float masterSpeed = 1;

            masterSpeed = FPSFrameworkSettings.globalAnimationSpeed;

            if (internal_isPlaying)
                progress = Mathf.SmoothDamp(progress, 1, ref currentVelocity, length / masterSpeed);

            if (!internal_isPlaying || HasToAvoid())
                progress = Mathf.SmoothDamp(progress, 0, ref currentVelocity, length / masterSpeed);
        }

        private bool prevPlaying;

        private void HandleEvents()
        {
            if(internal_isPlaying && !prevPlaying)
            {
                events.OnPlayed?.Invoke();
            }

            if(!internal_isPlaying && prevPlaying)
            {
                events.OnStoped?.Invoke();
            }

            foreach (CustomProceduralAnimationEvent animationEvent in customEvents) animationEvent.UpdateEvent(this);

            prevPlaying = internal_isPlaying;
        }

        /// <summary>
        /// returns all the clip modifiers for this clip in a List of ProceduralAnimationClip and refreshes the animtor clips 
        /// </summary>
        public List<ProceduralAnimationModifier> RefreshModifiers()
        {
            modifiers = GetComponentsInChildren<ProceduralAnimationModifier>().ToList();

            return modifiers;
        }

        public bool HasToAvoid()
        {
            bool result = false;

            if (internalAlwaysStayIdle || ForceIdle || FPSFrameworkCore.IsActive == false) return true;

            foreach (ProceduralAnimationConnection connection in connections)
            {
                if (connection.target != null)
                {
                    if (connection.type == ProceduralAnimationConnectionType.AvoidInTrigger)
                    {
                        if (connection.target && connection.target.internal_isPlaying) result = true;
                    }

                    if (connection.type == ProceduralAnimationConnectionType.AvoidInIdle)
                    {
                        if (!connection.target.internal_isPlaying) result = true;
                    }
                }
            }

            return result;
        }

        public float GetAvoidanceFactor(ProceduralAnimation animation)
        {
            if(animation == null) return 0f;

            return Mathf.Lerp(1, 0, animation.progress);
        }

        /// <summary>
        /// final position result for this modifier
        /// </summary>
        public Vector3 GetTargetModifiersPosition()
        {
            Vector3 result = Vector3.zero;

            float avoidanceFactor = 1;

            foreach (ProceduralAnimationConnection connection in connections)
            {
                if (connection.target != null)
                {
                    if (connection.type == ProceduralAnimationConnectionType.AvoidInTrigger)
                    {
                        avoidanceFactor *= GetAvoidanceFactor(connection.target);
                    }

                    if (connection.type == ProceduralAnimationConnectionType.AvoidInIdle)
                    {
                        avoidanceFactor *= Mathf.Lerp(1, 0, GetAvoidanceFactor(connection.target));
                    }
                }
            }

            foreach (ProceduralAnimationModifier modifier in modifiers) result += modifier.targetPosition;

            if (options.HasFlag(AnimationOptions.PerModifierConnections))
                result *= avoidanceFactor;

            return result;
        }

        /// <summary>
        /// final rotation result for this modifier
        /// </summary>
        public Vector3 GetTargetModifiersRotation()
        {
            Vector3 result = Vector3.zero;

            float avoidanceFactor = 1;

            foreach (ProceduralAnimationConnection connection in connections)
            {
                if (connection.target != null)
                {
                    if (connection.type == ProceduralAnimationConnectionType.AvoidInTrigger)
                    {
                        avoidanceFactor *= GetAvoidanceFactor(connection.target);
                    }

                    if (connection.type == ProceduralAnimationConnectionType.AvoidInIdle)
                    {
                        avoidanceFactor *= Mathf.Lerp(1, 0, GetAvoidanceFactor(connection.target));
                    }
                }
            }

            foreach (ProceduralAnimationModifier modifier in modifiers) result += modifier.targetRotation;

            if (options.HasFlag(AnimationOptions.PerModifierConnections))
                result *= avoidanceFactor;

            return result;
        }

        public enum TriggerType
        {
            None = 0,
            Tab = 1,
            Hold = 2,
            DoubleTab = 3,
            Trigger = 4
        }
    }
}