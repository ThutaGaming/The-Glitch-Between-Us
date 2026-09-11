// Copyright 2021, Infima Games. All Rights Reserved.

using System.Linq;
using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class Movement : MovementBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("Audio Clips")]
        
        [Tooltip("The audio clip that is played while walking.")]
        [SerializeField]
        private AudioClip audioClipWalking;

        [Tooltip("The audio clip that is played while running.")]
        [SerializeField]
        private AudioClip audioClipRunning;

        [Header("Speeds")]

        [SerializeField]
        private float speedWalking = 5.0f;

        [Tooltip("How fast the player moves while running."), SerializeField]
        private float speedRunning = 9.0f;

        [Header("Jump")]

        [Tooltip("Upward velocity applied when jumping."), SerializeField]
        private float jumpForce = 6.0f;

        [Header("Step Climbing")]

        [Tooltip("Steps/curbs shorter than this are walked straight up, like real stairs, instead of blocking the capsule.")]
        [SerializeField] private float maxStepHeight = 0.35f;
        [SerializeField] private float stepCheckDistance = 0.5f;
        [SerializeField] private float stepClimbSpeed = 4f;

        #endregion

        #region PROPERTIES

        //Velocity.
        private Vector3 Velocity
        {
            //Getter.
            get => rigidBody.linearVelocity;
            //Setter.
            set => rigidBody.linearVelocity = value;
        }

        #endregion

        #region FIELDS

        /// <summary>
        /// Attached Rigidbody.
        /// </summary>
        private Rigidbody rigidBody;
        /// <summary>
        /// Attached CapsuleCollider.
        /// </summary>
        private CapsuleCollider capsule;
        /// <summary>
        /// Attached AudioSource.
        /// </summary>
        private AudioSource audioSource;
        
        /// <summary>
        /// True if the character is currently grounded.
        /// </summary>
        private bool grounded;

        /// <summary>
        /// Player Character.
        /// </summary>
        private CharacterBehaviour playerCharacter;
        /// <summary>
        /// The player character's equipped weapon.
        /// </summary>
        private WeaponBehaviour equippedWeapon;
        
        /// <summary>
        /// Array of RaycastHits used for ground checking.
        /// </summary>
        private readonly RaycastHit[] groundHits = new RaycastHit[8];

        /// <summary>
        /// True the frame the jump key was pressed while grounded; consumed by the next FixedUpdate.
        /// </summary>
        private bool jumpRequested;

        #endregion

        #region UNITY FUNCTIONS

        /// <summary>
        /// Awake.
        /// </summary>
        protected override void Awake()
        {
            //Get Player Character.
            playerCharacter = ServiceLocator.Current.Get<IGameModeService>().GetPlayerCharacter();
        }

        /// Initializes the FpsController on start.
        protected override  void Start()
        {
            //Rigidbody Setup.
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            //Cache the CapsuleCollider.
            capsule = GetComponent<CapsuleCollider>();

            //Audio Source Setup.
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = audioClipWalking;
            audioSource.loop = true;
        }

        /// Checks if the character is on the ground.
        private void OnCollisionStay()
        {
            //Bounds.
            Bounds bounds = capsule.bounds;
            //Extents.
            Vector3 extents = bounds.extents;
            //Radius.
            float radius = extents.x - 0.01f;
            
            //Cast. This checks whether there is indeed ground, or not.
            Physics.SphereCastNonAlloc(bounds.center, radius, Vector3.down,
                groundHits, extents.y - radius * 0.5f, ~0, QueryTriggerInteraction.Ignore);
            
            //We can ignore the rest if we don't have any proper hits.
            if (!groundHits.Any(hit => hit.collider != null && hit.collider != capsule)) 
                return;
            
            //Store RaycastHits.
            for (var i = 0; i < groundHits.Length; i++)
                groundHits[i] = new RaycastHit();

            //Set grounded. Now we know for sure that we're grounded.
            grounded = true;
        }
			
        protected override void FixedUpdate()
        {
            //Move.
            MoveCharacter();
            
            //Unground.
            grounded = false;
        }

        /// Moves the camera to the character, processes jumping and plays sounds every frame.
        protected override  void Update()
        {
            //Get the equipped weapon!
            equippedWeapon = playerCharacter.GetInventory().GetEquipped();

            //Play Sounds!
            PlayFootstepSounds();

            //Jump. Queued here and consumed in FixedUpdate, since GetKeyDown's one-frame pulse
            //can't be reliably polled from inside FixedUpdate.
            if (grounded && Input.GetKeyDown(KeyCode.Space))
                jumpRequested = true;
        }

        #endregion

        #region METHODS

        private void MoveCharacter()
        {
            #region Calculate Movement Velocity

            //Get Movement Input!
            Vector2 frameInput = playerCharacter.GetInputMovement();
            //Calculate local-space direction by using the player's input.
            var movement = new Vector3(frameInput.x, 0.0f, frameInput.y);
            
            //Running speed calculation.
            if(playerCharacter.IsRunning())
                movement *= speedRunning;
            else
            {
                //Multiply by the normal walking speed.
                movement *= speedWalking;
            }

            //World space velocity calculation. This allows us to add it to the rigidbody's velocity properly.
            movement = transform.TransformDirection(movement);

            #endregion

            //Climb steps/curbs shorter than maxStepHeight instead of walking straight into them -
            //a plain Rigidbody+CapsuleCollider has no built-in step offset like CharacterController.
            Vector3 horizontalDir = new Vector3(movement.x, 0.0f, movement.z);
            if (grounded && horizontalDir.sqrMagnitude > 0.0001f)
                TryStepUp(horizontalDir.normalized);

            //Preserve the current vertical velocity (gravity, jump arc) instead of zeroing it -
            //overwriting it every FixedUpdate was cancelling gravity's acceleration each step,
            //which made falling crawl instead of speeding up like normal free-fall.
            float verticalVelocity = rigidBody.linearVelocity.y;

            //Jump. Consume the request queued in Update().
            if (jumpRequested)
            {
                verticalVelocity = jumpForce;
                jumpRequested = false;
            }

            //Update Velocity.
            Velocity = new Vector3(movement.x, verticalVelocity, movement.z);
        }

        /// <summary>
        /// If something shorter than <see cref="maxStepHeight"/> blocks the way ahead but the
        /// space above it is clear, nudges the rigidbody up so walking into it climbs the step
        /// instead of stopping dead against its riser (real stairs, curbs, ...).
        /// </summary>
        private void TryStepUp(Vector3 direction)
        {
            float radius = Mathf.Max(0.05f, capsule.radius * 0.5f);

            //Lower check: is there actually an obstacle at foot level in front of us?
            Vector3 lowerOrigin = transform.position + Vector3.up * (radius + 0.05f);
            if (!Physics.SphereCast(lowerOrigin, radius, direction, out _, stepCheckDistance, ~0, QueryTriggerInteraction.Ignore))
                return;

            //Upper check: is it clear just above the maximum step height? If something is still
            //in the way up there, this is a real wall, not a climbable step.
            Vector3 upperOrigin = transform.position + Vector3.up * maxStepHeight;
            if (Physics.SphereCast(upperOrigin, radius, direction, out _, stepCheckDistance, ~0, QueryTriggerInteraction.Ignore))
                return;

            //Path is clear above the obstacle - lift the character onto it.
            rigidBody.position += Vector3.up * (stepClimbSpeed * Time.fixedDeltaTime);
        }

        /// <summary>
        /// Plays Footstep Sounds. This code is slightly old, so may not be great, but it functions alright-y!
        /// </summary>
        private void PlayFootstepSounds()
        {
            //Check if we're moving on the ground. We don't need footsteps in the air.
            if (grounded && rigidBody.linearVelocity.sqrMagnitude > 0.1f)
            {
                //Select the correct audio clip to play.
                audioSource.clip = playerCharacter.IsRunning() ? audioClipRunning : audioClipWalking;
                //Play it!
                if (!audioSource.isPlaying)
                    audioSource.Play();
            }
            //Pause it if we're doing something like flying, or not moving!
            else if (audioSource.isPlaying)
                audioSource.Pause();
        }

        #endregion
    }
}