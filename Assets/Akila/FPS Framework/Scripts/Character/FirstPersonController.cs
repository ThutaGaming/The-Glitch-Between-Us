using Akila.FPSFramework.Animation;
using Akila.FPSFramework.Internal;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Serialization;

namespace Akila.FPSFramework
{
    [RequireComponent(typeof(CharacterManager))]
    [RequireComponent(typeof(CharacterController), typeof(CharacterInput))]
    [AddComponentMenu("Akila/FPS Framework/Player/First Person Controller")]
    public class FirstPersonController : MonoBehaviour, ICharacterController
    {
        [Header("Movement")]
        [Tooltip("How quickly the player accelerates to the target movement speed."), FormerlySerializedAs("acceleration")]
        public float acceleration = 0.1f;

        [Tooltip("Default walking speed."), FormerlySerializedAs("walkSpeed")]
        public float walkSpeed = 5;

        [Tooltip("Movement speed while crouching."), FormerlySerializedAs("crouchSpeed")]
        public float crouchSpeed = 3;

        [Tooltip("Movement speed while sprinting."), FormerlySerializedAs("sprintSpeed")]
        public float sprintSpeed = 10;

        [Tooltip("Movement speed during tactical sprinting (faster than normal sprint).")]
        public float tacticalSprintSpeed = 11;

        [Tooltip("How high the player can jump.")]
        public float jumpHeight = 6;

        [Tooltip("Player’s height when crouched.")]
        public float crouchHeight = 1.5f;

        public float crouchTime = 0.1f;

        [Tooltip("Distance between footstep sounds (lower = more frequent).")]
        public float stepInterval = 7;

        [Tooltip("Automatically detects and follows moving platforms.")]
        public bool autoDetectMovingPlatforms = true;

        [Tooltip("If true, maintains horizontal momentum when jumping or falling.")]
        public bool preserveMomentum = true;

        [Range(0f, 1f)]
        [Tooltip("Fraction of momentum preserved when jumping or falling. For example, 0.2 means 20% is lost and 80% is carried over.")]
        public float momentumLoss = 0.2f;

        //[Range(0f, 1f)]
        //TODO: Fix air control additive bug where speed increases if you move in the direction as your momentum

        private const float airControl = 0;

        [Header("Slopes")]
        [Tooltip("If true, the player will slide down steep slopes automatically.")]
        public bool slideDownSlopes = true;

        [Tooltip("Speed at which the player slides down slopes.")]
        public float slopeSlideSpeed = 1;

        [Space]
        [Tooltip("Strength of gravity applied to the player.")]
        public float gravity = 1;

        [Tooltip("Maximum speed the player can fall.")]
        public float maxFallSpeed = 350;

        [Tooltip("Extra downward force applied to keep the player grounded on slopes or uneven terrain.")]
        public float stickToGroundForce = 0.5f;
        public float leaveGroundForce = 3;

        [Header("Camera")]
        [FormerlySerializedAs("_Camera")]
        [Tooltip("Reference to the player’s camera transform.")]
        public Transform cameraTransform;

        [Tooltip("Maximum upward camera rotation in degrees.")]
        public float maximumX = 90f;

        [Tooltip("Maximum downward camera rotation in degrees.")]
        public float minimumX = -90f;

        [Tooltip("Camera position offset relative to the player.")]
        public Vector3 offset = new Vector3(0, -0.05f, 0);

        [Tooltip("Locks and hides the cursor when the game starts.")]
        public bool lockCursor = true;

        [Tooltip("If true, the player rotation uses a global orientation rather than being camera-relative.")]
        public bool globalOrientation = false;

        [Header("Sensitivity")]
        [Tooltip("Mouse look sensitivity multiplier.")]
        public float sensitivityOnMouse = 1;

        [Tooltip("Gamepad look sensitivity multiplier.")]
        public float sensitivityOnGamepad = 1;

        [Tooltip("Adjusts look sensitivity based on camera FOV using this curve.")]
        public AnimationCurve fovToSensitivityCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0) });

        [Tooltip("If enabled, sensitivity dynamically changes depending on FOV.")]
        public bool isDynamicSensitivityEnabled = true;

        [Header("Audio")]
        [Tooltip("Footstep sounds played based on surface type.")]
        public AudioProfile[] footstepsSFX;

        [Tooltip("Sound played when the player jumps.")]
        public AudioProfile jumpSFX;

        [Tooltip("Sound played when the player lands.")]
        public AudioProfile landSFX;

        public CollisionFlags CollisionFlags { get; set; }
        public CharacterController controller { get; set; }
        public CharacterManager characterManager { get; set; }
        public CharacterInput CharacterInput { get; private set; }

        //input velocity
        private Vector3 desiredVelocityRef;
        private Vector3 desiredVelocity;
        private Vector3 slideVelocity;

        //out put velocity
        private Vector3 velocity;

        public Transform Orientation { get; set; }
        public float tacticalSprintAmount { get; set; }
        public bool canTacticalSprint { get; set; }


        private Vector3 slopeDirection;

        private float yRotation = 0;
        private float xRotation = 0;

        private float speed;

        private float defaultHeight;
        private float defaultstepOffset;

        private float stepCycle;
        private float nextStep;

        public List<Audio> footStepsAudio = new List<Audio>();
        public Audio jumpAudio;
        public Audio landAudio;

        [Space]
        public UnityEvent<int> onStep = new UnityEvent<int>();
        public UnityEvent onJump = new UnityEvent();
        public UnityEvent onLand = new UnityEvent();

        public bool isCrouching { get; set; }

        public bool isActive { get; protected set; } = true;

        public float currentGravityForce { get; protected set; }

        private Quaternion cameraRotation;
        private Quaternion playerRotation;

        public ProceduralAnimator proceduralAnimator { get; protected set; }
        public ProceduralAnimation leanRightAnimation { get; protected set; }
        public ProceduralAnimation leanLeftAnimation { get; protected set; }

        private Vector3 onJumpVelcoity;

        private Speedometer speedometer;

        private bool onMovingPlatform;

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
            CharacterInput = GetComponent<CharacterInput>();
            controller = GetComponent<CharacterController>();
            proceduralAnimator = transform.SearchFor<ProceduralAnimator>();

            speedometer = gameObject.GetOrAddComponent<Speedometer>();
            speedometer.updateMode = UpdateMode.FixedUpdate;

            characterManager.onJump.AddListener(() =>
            {
                if(attemptedToJump == false)
                {
                    currentGravityForce = -leaveGroundForce;
                }

                Vector3 vel = speedometer.velocity;

                onJumpVelcoity = vel.magnitude * (vel.normalized * (1 - momentumLoss));
            });

            characterManager.onLand.AddListener(() =>
            {
                attemptedToJump = false;
            });

            controller.center = Vector3.up * controller.height * 0.5f;

            if(proceduralAnimator)
            {
                leanRightAnimation = proceduralAnimator.GetAnimation("Lean Right");
                leanLeftAnimation = proceduralAnimator.GetAnimation("Lean Left");
            }

            characterManager.SetValues(Vector3.zero, controller.isGrounded, walkSpeed, sprintSpeed);
        }

        float GetSignedAngle(float angle)
        {
            if (angle > 180)
                angle -= 360;
            return angle;
        }


        private void OnEnable()
        {
            footStepsAudio.Clear();

            foreach (AudioProfile profile in footstepsSFX)
            {
                if (profile != null)
                {
                    Audio newAudio = new Audio();
                    newAudio.Setup(gameObject, profile);

                    footStepsAudio.Add(newAudio);
                }
            }

            if (jumpSFX)
            {
                jumpAudio = new Audio();
                jumpAudio.Setup(gameObject, jumpSFX);
            }

            if (landSFX)
            {
                landAudio = new Audio();
                landAudio.Setup(gameObject, landSFX);
            }
        }

        protected virtual void Start()
        {
            if (!cameraTransform) cameraTransform = GetComponentInChildren<Camera>().transform;

            if (cameraTransform != null)
            {
                float xRot = GetSignedAngle(cameraTransform.eulerAngles.x);

                xRotation = xRot;
            }

            if (transform.Find("Orientation") != null)
            {
                Orientation = transform.Find("Orientation");
            }
            else
            {
                Orientation = new GameObject("Orientation").transform;

                Orientation.parent = transform;
                Orientation.localRotation = transform.rotation;
            }

            ResetSpeedMultiplier();

            //get defaults
            defaultHeight = controller.height;
            defaultstepOffset = controller.stepOffset;
            controller.skinWidth = controller.radius / 10;
            controller.enableOverlapRecovery = true;

            //hide and lock cursor if there is no pause menu in the scene
            if (lockCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            characterManager.onLand.AddListener(PlayLandSFX);

            controller.center = Vector3.up * controller.height * 0.5f;

            cameraTransform.position = transform.position + ((Vector3.up * (controller.height - 1) + offset + controller.center));

            transform.position -= controller.center;

            Orientation.localPosition = controller.center;
        }

        private bool attemptedToJump;

        protected virtual void Update()
        {            
            if (!isActive) return;

            if(leanRightAnimation)
            {
                leanRightAnimation.IsPlaying = CharacterInput.LeanRightInput;
                leanLeftAnimation.IsPlaying = CharacterInput.LeanLeftInput;
            }

            //slide down slope if on maxed angle slope
            if (slideDownSlopes && OnMaxedAngleSlope())
                slideVelocity += new Vector3(slopeDirection.x, -slopeDirection.y, slopeDirection.z) * slopeSlideSpeed * Time.deltaTime;
            else
                //reset velocity if not on slope
                slideVelocity = Vector3.zero;

            Vector3 targetVelocity = (SlopeDirection() * CharacterInput.MoveInput.y + Orientation.right * CharacterInput.MoveInput.x).normalized * speed;

            //update desiredVelocity in order to normlize it and smooth the movement
            desiredVelocity = slideVelocity + Vector3.SmoothDamp(desiredVelocity, targetVelocity * CharacterInput.MoveInput.magnitude, ref desiredVelocityRef, acceleration);

            if (!controller.isGrounded || OnSlope())
            {
                controller.stepOffset = 0;
            }
            else
            {
                controller.stepOffset = defaultstepOffset;
            }

            //copy desiredVelocity x, z with normlized values
            velocity.x = (desiredVelocity.x);
            velocity.z = (desiredVelocity.z);

            //update speed according to if player is holding sprint

            if (SlopeAngle() < controller.slopeLimit)
            {
                if (CharacterInput.SprintInput && !CharacterInput.TacticalSprintInput) speed = isCrouching ? crouchSpeed * speedMultiplier : sprintSpeed * speedMultiplier;
                else if (!CharacterInput.TacticalSprintInput) speed = speed = isCrouching ? crouchSpeed * speedMultiplier : walkSpeed * speedMultiplier;

                if (CharacterInput.TacticalSprintInput) speed = speed = isCrouching ? crouchSpeed * speedMultiplier : tacticalSprintSpeed * speedMultiplier;
            }
            else
            {
                speed = 0;
            }

            //Do crouching
            isCrouching = CharacterInput.CrouchInput;
            ApplyCrouching();

            //update gravity and jumping
            if (controller.isGrounded)
            {
                //set small force when grounded in order to staplize the controller
                currentGravityForce = Physics.gravity.y * stickToGroundForce;

                //check jumping input
                if (CharacterInput.JumpInput)
                {
                    attemptedToJump = true;

                    onJump?.Invoke();

                    //update velocity in order to jump
                    currentGravityForce += jumpHeight - currentGravityForce;

                    //play jump sound
                    if (jumpSFX)
                        jumpAudio.Play(true);
                }
                
                velocity.y = currentGravityForce;
            }
            else if (velocity.magnitude * 3.5f < maxFallSpeed)
            {
                //add gravity
                currentGravityForce += Physics.gravity.y * gravity * Time.deltaTime;

                velocity.y = currentGravityForce;
            }

            if (controller.isGrounded)
            {
                Vector3 input = CharacterInput.MoveInput;

                float orientToForward = Vector3.Dot(onJumpVelcoity.normalized, Orientation.forward);
                float orientToBackward = Vector3.Dot(onJumpVelcoity.normalized, -Orientation.forward);
                float orientToRight = Vector3.Dot(onJumpVelcoity.normalized, Orientation.right);
                float orientToLeft = Vector3.Dot(onJumpVelcoity.normalized, -Orientation.right);

                if (onMovingPlatform || orientToForward >= 0.7f && input.y > 0 || orientToBackward >= 0.7f && input.y < 0 || orientToRight >= 0.7f && input.x > 0 || orientToLeft >= 0.7f && input.x < 0)
                {
                    onJumpVelcoity = Vector3.zero;
                }
                else
                {
                    onJumpVelcoity = Vector3.SmoothDamp(onJumpVelcoity, Vector3.zero, ref currentVel, acceleration);
                }
            }

            velocityF.x = velocity.x;
                velocityF.z = velocity.z;

            if (preserveMomentum)
            {
                velocityF.x *= (controller.isGrounded ? 1 : airControl);
                velocityF.z *= (controller.isGrounded ? 1 : airControl);
            }

            velocityF.y = velocity.y;

            onJumpVelcoity.y = 0;

            Vector3 jumpVel = onJumpVelcoity;

            if (preserveMomentum == false)
                jumpVel = Vector3.zero;
            
            Vector3 totalVel = velocityF + jumpVel;

            Vector3 clampedVel = Vector3.ClampMagnitude(new Vector3(totalVel.x, 0, totalVel.z), tacticalSprintSpeed);

            totalVel.x = clampedVel.x;
            totalVel.z = clampedVel.z;
            
            //move and update CollisionFlags in order to check if collition is coming from above ot center or bottom
            CollisionFlags = controller.Move(totalVel * Time.deltaTime);

            //rotate camera
            UpdateCameraRotation();

            tacticalSprintAmount = CharacterInput.TacticalSprintInput ? 1 : 0;

            MoveWithMovingPlatforms();
        }

        protected virtual void LateUpdate()
        {
            UpdateCharacterManager();
        }

        //Use this function to understand how to integrate your own character controller
        protected virtual void UpdateCharacterManager()
        {
            //Feed character manager with the info it needs
            //The character manager uses these info to invoke OnJump or OnLand events
            //Other components use the walkSpeed and sprintSpeed values to calculate certain things
            characterManager.SetValues(controller.velocity, controller.isGrounded, walkSpeed, sprintSpeed);

            //Get the info we need for this movement script from character manager
            //Other components change this value e.g when aiming, speedMultiplier changes
            speedMultiplier = characterManager.speedMultiplier;
        }

        Vector3 velocityF;

        public void ApplyCrouching()
        {
            //set controller height according to if player is crouching
            controller.height = isCrouching ?
            Mathf.SmoothDamp(controller.height, crouchHeight, ref currentCrouchVel, crouchTime) :
            Mathf.SmoothDamp(controller.height, defaultHeight, ref currentCrouchVel, crouchTime);

            controller.center = Vector3.up * controller.height * 0.5f;

            cameraTransform.position = transform.position + ((Vector3.up * (controller.height - 1) + offset + controller.center));
        }

        public virtual void PlayLandSFX()
        {
            onLand?.Invoke();

            if (landSFX)
                landAudio.Play(true);
        }

        public virtual void FixedUpdate()
        {
            //update step sounds
            ProgressStepCycle();
        }

        protected virtual void ProgressStepCycle()
        {
            //stop if not grounded
            if (!controller.isGrounded || footstepsSFX.Length <= 0) return;

            //check if taking input and input
            if (controller.velocity.sqrMagnitude > 0 && (CharacterInput.MoveInput.x != 0 || CharacterInput.MoveInput.y != 0))
            {
                //update step cycle
                stepCycle += (controller.velocity.magnitude + (controller.velocity.magnitude * (!characterManager.IsVelocityZero() ? 1f : 1))) * Time.fixedDeltaTime;
            }

            //check step cycle not equal to next step in order to update right
            if (!(stepCycle > nextStep))
            {
                return;
            }

            //update
            nextStep = stepCycle + stepInterval;
           
            int currentFootStepIndex = Random.Range(0, footStepsAudio.GetLength());

            onStep?.Invoke(currentFootStepIndex);

            if (footstepsSFX != null)
            {
                Audio currentFootStepAudio = footStepsAudio[currentFootStepIndex];

                currentFootStepAudio.Play(true);
            }
        }

        protected virtual void UpdateCameraRotation()
        {
            if (prevCamRotation != cameraTransform.rotation) OnCameraRotationUpdated();

            yRotation += CharacterInput.LookInput.x;
            xRotation -= CharacterInput.LookInput.y;


            xRotation = Mathf.Clamp(xRotation, minimumX, maximumX);

            //Avoid Nan for x rot
            if(float.IsNaN(xRotation))
            {
                xRotation = 0;
            }

            //Avoid Nan for y rot
            if (float.IsNaN(yRotation))
            {
                yRotation = 0;
            }

            cameraRotation = Quaternion.Slerp(cameraRotation, Quaternion.Euler(xRotation, yRotation, 0), Time.deltaTime * 100);
            playerRotation = Quaternion.Slerp(playerRotation, Quaternion.Euler(0, yRotation, 0), Time.deltaTime * 100);

            Orientation.SetRotation(playerRotation, !globalOrientation);
            cameraTransform.SetRotation(cameraRotation, !globalOrientation);

            prevCamRotation = cameraTransform.rotation;
        }

        private Quaternion prevCamRotation;

        protected virtual void OnCameraRotationUpdated() { }

        public virtual bool OnSlope()
        {
            //check if slope angle is more than 0
            if (SlopeAngle() > 0)
            {
                return true;
            }

            return false;
        }

        public virtual bool OnMaxedAngleSlope()
        {
            if (controller.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, controller.height))
            {
                slopeDirection = hit.normal;
                return Vector3.Angle(slopeDirection, Vector3.up) > controller.slopeLimit;
            }

            return false;
        }

        public virtual Vector3 SlopeDirection()
        {
            //setup a raycast from position to down at the bottom of the collider
            RaycastHit slopeHit;

            if (Physics.Raycast(Orientation.position, Vector3.down, out slopeHit, (controller.height / 2) + 0.1f) && SlopeAngle() < controller.slopeLimit)
            {
                //get the direction result according to slope normal
                return Vector3.ProjectOnPlane(Orientation.forward, slopeHit.normal);
            }

            //if not on slope then slope is forward ;)
            return Orientation.forward;
        }

        public virtual float SlopeAngle()
        {
            //setup a raycast from position to down at the bottom of the collider
            RaycastHit slopeHit;
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit))
            {
                //get the direction result according to slope normal
                return (Vector3.Angle(Vector3.down, slopeHit.normal) - 180) * -1;
            }

            //if not on slope then slope is forward ;)
            return 0;
        }
        private float speedMultiplier = 1;

        public virtual void SetSpeedMultiplier(float speedMultiplier)
        {
            this.speedMultiplier = speedMultiplier;
        }

        public virtual void ResetSpeedMultiplier()
        {
            speedMultiplier = 1;
        }

        [System.Obsolete("Use SetSpeedMultiplier instead.")]
        public virtual void SetSpeed(float speedMultiplier)
        {
            this.speedMultiplier = speedMultiplier;
        }

        [System.Obsolete("Use ResetSpeedMultiplier instead.")]
        public virtual void ResetSpeed()
        {
            speedMultiplier = 1;
        }

        public void SetActive(bool value)
        {
            isActive = value;

            //Set active for the camera
            Camera[] cameras = GetComponentsInChildren<Camera>();

            foreach (Camera cam in cameras)
            {
                cam.enabled = value;
            }

            controller.detectCollisions = value;

            //Set active for the audio listener
            AudioListener[] audioListeners = GetComponentsInChildren<AudioListener>();

            foreach (AudioListener audioListener in audioListeners)
            {
                AudioEchoFilter echoFilter = audioListener.GetComponent<AudioEchoFilter>();
                AudioReverbFilter reverbFilter = audioListener.GetComponent<AudioReverbFilter>();
                AudioHighPassFilter highPassFilter = audioListener.GetComponent<AudioHighPassFilter>();
                AudioLowPassFilter lowPassFilter = audioListener.GetComponent<AudioLowPassFilter>();
                AudioDistortionFilter distortionFilter = audioListener.GetComponent<AudioDistortionFilter>();

                if(echoFilter) echoFilter.enabled = value;
                if(reverbFilter)reverbFilter.enabled = value;
                if(highPassFilter) highPassFilter.enabled = value;
                if(lowPassFilter) lowPassFilter.enabled = value;
                if(distortionFilter) distortionFilter.enabled = value;
                
                audioListener.enabled = value;
            }
        }

        protected virtual void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //if hit something while jumping from the above then go down again
            if (CollisionFlags == CollisionFlags.Above)
            {
                velocity.y = 0;
            }
        }

        private Vector3 feetPosition;
        private Vector3 totalVelocity;
        private Vector3 currentVel;
        private float currentCrouchVel;

        private void MoveWithMovingPlatforms()
        {
            totalVelocity = Vector3.zero;

            // Calculate the position of the feet based on character height
            feetPosition = transform.position - ((transform.up * (controller.height / 2)) - controller.center);

            // Perform a sphere cast to detect surrounding objects
            RaycastHit[] hits = Physics.SphereCastAll(new Ray(feetPosition, Vector3.down), controller.radius, controller.radius / 2);


            // Loop through all hits to calculate total velocity from Speedometer components
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform != transform) // Ensure we're not processing the character itself
                {
                    Vector3 hitVelocity = GetTransformVelocity(hit.transform);

                    totalVelocity += hitVelocity;
                }
            }

            if(!controller.isGrounded)
            {
                //totalVelocity += Physics.gravity * gravity * Time.deltaTime;
            }

            // Move the character controller based on total velocity
            transform.position += totalVelocity;

            onMovingPlatform = totalVelocity.magnitude > 0;
        }

        private Vector3 GetTransformVelocity(Transform hitTransform)
        {
            Speedometer speedometer = null;

            //This is used to not move with anything there is and only move with what's under the player's feets
            //e.g To avoid moving with a bullet casing for example
            if (CollisionFlags != CollisionFlags.Below)
            {
                return Vector3.zero;
            }

            if (autoDetectMovingPlatforms)
                speedometer = hitTransform.GetOrAddComponent<Speedometer>();
            else
                speedometer = hitTransform.GetComponent<Speedometer>();

            if (speedometer == null)
                return Vector3.zero;

            if (speedometer.TryGetComponent<Ignore>(out Ignore ignore))
            {
                if (ignore.ignoreMovingPlatform)
                    return Vector3.zero;
            }

            // If the Speedometer component exists, return its velocity
            if (speedometer != null)
            {
                return speedometer.GetPointVelocity(transform.position) * Time.unscaledDeltaTime; // Apply delta time for frame-rate independent movement
            }


            return Vector3.zero; // Return zero if no Speedometer is found
        }

        [ContextMenu("Setup/Network Components")]
        public void Convert()
        {
            FPSFrameworkCore.InvokeConvertMethod("ConvertPlayer", this, new object[] { this });
        }
    }
}