using Akila.FPSFramework.Animation;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

namespace Akila.FPSFramework
{
    /// <summary>
    /// Base class for all item that can be held by the player or the bot.
    /// </summary>
    public abstract class InventoryItem : Item
    {
        [Tooltip("Replacement object for this item when dropped.")]
        public Pickable replacement;

        public bool isTryingToDrop { get; set; }

        public Renderer[] renderers { get; private set; }

        public void PlayHideAnimation()
        {
            animators = transform.GetComponentsInChildren<Animator>();

            if (animators == null || animators.Length == 0)
            {
                Debug.LogWarning($"{name} has no animators assigned.");
                return;
            }

            foreach (Animator animator in animators)
            {
                if (animator != null)
                {
                    animator.Play("Hide", 0, 1f); // Play from beginning
                }
                else
                {
                    Debug.LogWarning($"{name} has a null Animator reference.");
                }
            }
        }

        public bool isPlayingHidingAnimation
        {
            get
            {
                foreach (Animator animator in animators)
                {
                    if (animator.IsPlaying("Hide")) return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Represents the firearm component of the item. If the weapon is not a firearm, this will be null.
        /// </summary>
        public Firearm firearm { get; set; }

        /// <summary>
        /// Represents the throwable component of the item. If the weapon is not throwable, this will be null.
        /// </summary>
        public Throwable throwable { get; set; }

        /// <summary>
        /// List of all active projectiles from this weapon.
        /// </summary>
        public List<Projectile> Projectiles { get; set; } = new List<Projectile>();

        /// <summary>
        /// Reference to the player's inventory.
        /// </summary>
        public IInventory inventory { get; set; }

        /// <summary>
        /// Array of animator components related to this item.
        /// </summary>
        public Animator[] animators { get; set; }

        /// <summary>
        /// The procedural animator handling complex animations for this item.
        /// </summary>
        public ProceduralAnimator proceduralAnimator { get; set; }

        /// <summary>
        /// Reference to the actor (usually the player) using the item.
        /// </summary>
        public IActor actor { get; set; }

        /// <summary>
        /// Character controller associated with the actor.
        /// </summary>
        public ICharacterController character { get; set; }

        /// <summary>
        /// Manages character-specific behaviors and states.
        /// </summary>
        public CharacterManager characterManager { get; set; }

        /// <summary>
        /// Input system for the character.
        /// </summary>
        public CharacterInput characterInput { get; set; }

        public GameObject playerObj { get; set; }

        /// <summary>
        /// Input system for item-related actions (e.g., firing, reloading).
        /// </summary>
        public ItemInput itemInput { get; set; }

        /// <summary>
        /// Animation for breathing.
        /// </summary>
        public ProceduralAnimation breathingAnimation { get; protected set; }

        /// <summary>
        /// Animation for breathing while aiming.
        /// </summary>
        public ProceduralAnimation breathingAimAnimation { get; protected set; }

        /// <summary>
        /// Animation for walking.
        /// </summary>
        public ProceduralAnimation walkingAnimation { get; protected set; }

        /// <summary>
        /// Animation for sprinting.
        /// </summary>
        public ProceduralAnimation sprintingAnimation { get; protected set; }

        /// <summary>
        /// Animation for tactical sprinting.
        /// </summary>
        public ProceduralAnimation tacticalSprintingAnimation { get; protected set; }

        /// <summary>
        /// Animation for aiming.
        /// </summary>
        public ProceduralAnimation aimingAnimation { get; protected set; }

        /// <summary>
        /// Animation for recoil when firing.
        /// </summary>
        public ProceduralAnimation recoilAnimation { get; protected set; }

        /// <summary>
        /// Animation for recoil while aiming.
        /// </summary>
        public ProceduralAnimation recoilAimAnimation { get; protected set; }

        /// <summary>
        /// Animation for jumping.
        /// </summary>
        public ProceduralAnimation jumpAnimation { get; protected set; }

        /// <summary>
        /// Animation for landing.
        /// </summary>
        public ProceduralAnimation landAnimation { get; protected set; }

        /// <summary>
        /// Animation for leaning to the right.
        /// </summary>
        public ProceduralAnimation leanRightAnimation { get; protected set; }

        /// <summary>
        /// Animation for leaning to the left.
        /// </summary>
        public ProceduralAnimation leanLeftAnimation { get; protected set; }

        /// <summary>
        /// Animation for leaning to the right while aiming.
        /// </summary>
        public ProceduralAnimation leanRightAimAnimation { get; protected set; }

        /// <summary>
        /// Animation for leaning to the left while aiming.
        /// </summary>
        public ProceduralAnimation leanLeftAimAnimation { get; protected set; }

        /// <summary>
        /// Animation for when player is crouching.
        /// </summary>
        public ProceduralAnimation crouchAnimation { get; protected set; }

        /// <summary>
        /// Animation for when player is moves his item.
        /// </summary>
        public ProceduralAnimation swayAnimation { get; protected set; }

        /// <summary>
        /// Animation for when player is moves his item while aiming.
        /// </summary>
        public ProceduralAnimation swayAimingAnimation { get;  protected set; }

        /// <summary>
        /// Wave modifier for walking animation.
        /// </summary>
        private WaveAnimationModifier walkingWaveAnimationModifier { get; set; }

        public ProceduralAnimation firingAnimation { get; protected set; }
        public ProceduralAnimation aimFiringAnimation { get; protected set; }
        public ProceduralAnimation currentFiringAnimation
        {
            get
            {
                return isAiming ? aimFiringAnimation : firingAnimation;
            }
        }

        public Speedometer speedometer { get; protected set; }
        public Vector3 velocity { get => speedometer.velocity; }

        /// <summary>
        /// Invoked when the item is dropped.
        /// </summary>
        [HideInInspector] public UnityEvent OnDropAttempted;
        [HideInInspector] public UnityEvent<Pickable> OnDropPerformed;

        /// <summary>
        /// If false, the Drop() function will return after invoking the onDrop event.
        /// </summary>
        public bool isDroppingActive { get; set; } = true;

        /// <summary>
        /// Checks if the aiming animation is currently playing.
        /// </summary>
        public bool isAiming
        {
            get
            {
                // Ensure the aimingAnimation exists before checking if it's playing.
                if (aimingAnimation) return aimingAnimation.IsPlaying;
                return false;
            }
        }

        private float defaultAimingTime;

        /// <summary>
        /// Sets up the item and assigns the replacement item.
        /// </summary>
        /// <param name="replacement">The item that will replace this one.</param>
        protected virtual void Setup(Pickable replacement)
        {
            this.replacement = replacement;
        }

        protected virtual void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            Setup();
        }

        /// <summary>
        /// Initializes necessary components for the item, including animations and references.
        /// </summary>
        protected void Setup()
        {
            // Get necessary components for the script
            itemInput = GetComponent<ItemInput>(); // Get the ItemInput component
            animators = GetComponentsInChildren<Animator>(); // Get all Animator components in the children
            proceduralAnimator = transform.SearchFor<ProceduralAnimator>(); // Get the ProceduralAnimator component

            speedometer = gameObject.AddComponent<Speedometer>(); // Add Speedometer component

            firearm = GetComponent<Firearm>(); // Get the Firearm component
            throwable = GetComponent<Throwable>(); // Get the Throwable component

            // Check for inventory in parent, needed to respawn items in the item holder.
            inventory = GetComponentInParent<IInventory>();

            // Assign references from itemsHolder
            actor = inventory.transform.SearchFor<IActor>();
            character = GetComponentInParent<ICharacterController>();
            characterManager = GetComponentInParent<CharacterManager>();
            characterInput = GetComponentInParent<CharacterInput>();

            playerObj = character.gameObject;

            // Initialize animations if proceduralAnimator exists
            if (proceduralAnimator != null)
            {
                firingAnimation = proceduralAnimator.GetAnimation("Recoil");
                aimFiringAnimation = proceduralAnimator.GetAnimation("Aim Recoil");
                breathingAnimation = proceduralAnimator.GetAnimation("Breathing");
                breathingAimAnimation = proceduralAnimator.GetAnimation("Breathing Aim");
                walkingAnimation = proceduralAnimator.GetAnimation("Walking");
                sprintingAnimation = proceduralAnimator.GetAnimation("Sprinting");
                tacticalSprintingAnimation = proceduralAnimator.GetAnimation("Tactical Sprinting");
                aimingAnimation = proceduralAnimator.GetAnimation("Aiming");
                recoilAnimation = proceduralAnimator.GetAnimation("Recoil");
                recoilAimAnimation = proceduralAnimator.GetAnimation("Recoil Aim");
                jumpAnimation = proceduralAnimator.GetAnimation("Jump");
                landAnimation = proceduralAnimator.GetAnimation("Land");
                leanRightAnimation = proceduralAnimator.GetAnimation("Lean Right");
                leanLeftAnimation = proceduralAnimator.GetAnimation("Lean Left");
                leanRightAimAnimation = proceduralAnimator.GetAnimation("Lean Right Aim");
                leanLeftAimAnimation = proceduralAnimator.GetAnimation("Lean Left Aim");
                crouchAnimation = proceduralAnimator.GetAnimation("Crouch");
                swayAnimation = proceduralAnimator.GetAnimation("Sway");
                swayAimingAnimation = proceduralAnimator.GetAnimation("Sway Aiming");

                // Set default aiming time
                if (aimingAnimation) defaultAimingTime = aimingAnimation.length;
            }

            // Set up listeners for character manager events
            if (characterManager != null)
            {
                if (jumpAnimation)
                    characterManager.onJump.AddListener(() => jumpAnimation.Play(0));

                if (landAnimation)
                    characterManager.onLand.AddListener(() => landAnimation.Play(0));
            }

            if (walkingAnimation)
                walkingWaveAnimationModifier = walkingAnimation.GetComponent<WaveAnimationModifier>();
        }

        /// <summary>
        /// Updates the state of the item and animations each frame.
        /// </summary>
        protected virtual void Update()
        {
            if (walkingWaveAnimationModifier != null)
            {
                // Update walking wave animation based on character velocity
                float characterVelocity = characterManager.velocity.magnitude;
                walkingWaveAnimationModifier.speed = Mathf.Lerp(walkingWaveAnimationModifier.speed, characterVelocity, Time.deltaTime * 5);

                if (characterManager.velocity.magnitude > 1 && characterManager.isGrounded)
                    walkingWaveAnimationModifier.amount = Mathf.Lerp(walkingWaveAnimationModifier.amount, characterVelocity, Time.deltaTime * 5);
                else
                    walkingWaveAnimationModifier.amount = Mathf.Lerp(walkingWaveAnimationModifier.amount, 0, Time.deltaTime * 5);
            }

            if(swayAnimation)
            {
                swayAnimation.swayAnimationModifiers[0].inputX = (characterInput.LookInput.x / Time.deltaTime) * 0.5f;
                swayAnimation.swayAnimationModifiers[0].inputY = (characterInput.LookInput.y / Time.deltaTime) * 0.5f;
            }

            if (swayAimingAnimation)
            {
                swayAimingAnimation.swayAnimationModifiers[0].inputX = (characterInput.LookInput.x / Time.deltaTime) * 0.5f;
                swayAimingAnimation.swayAnimationModifiers[0].inputY = (characterInput.LookInput.y / Time.deltaTime) * 0.5f;
            }

            if (sprintingAnimation != null)
            {
                sprintingAnimation.triggerType = ProceduralAnimation.TriggerType.None;
                sprintingAnimation.IsPlaying = characterInput.SprintInput;
            }

            if (tacticalSprintingAnimation != null)
            {
                tacticalSprintingAnimation.triggerType = ProceduralAnimation.TriggerType.None;
                tacticalSprintingAnimation.IsPlaying = characterInput.TacticalSprintInput;
            }

            if (aimingAnimation != null)
            {
                // Adjust aiming animation speed based on firearm aim speed
                if (firearm && firearm.firearmAttachmentsManager)
                    aimingAnimation.length = defaultAimingTime / firearm.firearmAttachmentsManager.aimSpeed;

                aimingAnimation.triggerType = ProceduralAnimation.TriggerType.None;
                aimingAnimation.IsPlaying = itemInput.AimInput;
            }

            // Update crouch animation state
            if (crouchAnimation)
            {
                crouchAnimation.triggerType = ProceduralAnimation.TriggerType.None;
                crouchAnimation.IsPlaying = characterInput.CrouchInput;
            }

            // Update firearm-related animations based on reload and firing state
            if (firearm)
            {
                if (firearm.isReloading || firearm.attemptingToFire)
                {
                    if (recoilAnimation != null) recoilAnimation.weight = firearm.firearmAttachmentsManager.visualRecoil;
                    if (recoilAimAnimation != null) recoilAimAnimation.weight = firearm.firearmAttachmentsManager.visualRecoil;

                    if (sprintingAnimation != null) sprintingAnimation.internalAlwaysStayIdle = true;
                    if (tacticalSprintingAnimation != null) tacticalSprintingAnimation.internalAlwaysStayIdle = true;
                }
                else
                {
                    if (sprintingAnimation != null) sprintingAnimation.internalAlwaysStayIdle = false;
                    if (tacticalSprintingAnimation != null) tacticalSprintingAnimation.internalAlwaysStayIdle = false;
                }
            }

            // Handle leaning animations
            UpdateLeanAnimations();
        }

        /// <summary>
        /// Updates the state of leaning animations based on input.
        /// </summary>
        private void UpdateLeanAnimations()
        {
            // Update right and left leaning animations
            if (leanRightAnimation != null)
            {
                leanRightAnimation.IsPlaying = characterInput.LeanRightInput;
            }

            if (leanLeftAnimation != null)
            {
                leanLeftAnimation.IsPlaying = characterInput.LeanLeftInput;
            }

            // Update right and left aiming lean animations
            if (leanRightAimAnimation != null)
            {
                leanRightAimAnimation.IsPlaying = characterInput.LeanRightInput;
            }

            if (leanLeftAimAnimation != null)
            {
                leanLeftAimAnimation.IsPlaying = characterInput.LeanLeftInput;
            }
        }

        /// <summary>
        /// Drops the item on the ground.
        /// </summary>
        /// <param name="removeFromList">If true, the item will be removed from the Inventory's Items List.</param>
        public virtual void Drop(bool removeFromList = true)
        {
            isTryingToDrop = true;

            // Invoke the onDropped event if set.
            OnDropAttempted?.Invoke();

            // Check if dropping is active, if not log a warning and return early.
            if (!isDroppingActive)
            {
                return;
            }

            // Search for the CameraManager component to reset field of view.
            CameraManager cameraManager = transform.SearchFor<CameraManager>();

            // Calculate the drop force and torque based on inventory settings.
            Vector3 force = Vector3.down * (inventory != null ? inventory.dropForce * 3 : 10);
            Vector3 torque = transform.right * (inventory != null ? inventory.dropForce * 3 : 10);

            // If CameraManager exists, reset its field of view.
            if (cameraManager)
            {
                cameraManager.ResetFieldOfView();
            }

            // Check if a replacement item exists; if not, switch to the last item in the inventory.
            if (replacement == null)
            {
                if (inventory != null)
                {
                    inventory.Switch(inventory.items.Count - 1);
                }
                else
                {
                    //Debug.LogError("[InventoryItem] Inventory is null or contains no items. Unable to switch to a fallback item.");
                }

                // Check if the item is a Firearm or Throwable and throw an appropriate error message
                if (GetComponent<Firearm>() != null)
                {
                    Debug.LogError($"[InventoryItem] Failed to drop '{name}' because the 'replacement' field is null. " +
                                   "For Firearm items, ensure the replacement field is assigned in the Firearm preset.");
                }
                else if (GetComponent<Throwable>() != null)
                {
                    Debug.LogError($"[InventoryItem] Failed to drop '{name}' because the 'replacement' field is null. " +
                                   "For Throwable items, ensure the replacement field is assigned in the Throwable component.");
                }
                else
                {
                    Debug.LogError($"[InventoryItem] Failed to drop '{name}' because the 'replacement' field is null. " +
                                   "Ensure the replacement field is assigned to your custom public field as intended.");
                }

                Destroy(gameObject);
                return;
            }

            // Instantiate the replacement item and apply physics if it has a Rigidbody.
            Vector3 dropPosition = Vector3.zero;
            Quaternion dropRotation = Quaternion.identity;

            if(inventory != null && inventory.dropPoint)
            {
                dropPosition = inventory.dropPoint.position;
                dropRotation = inventory.dropPoint.rotation;
            }

            Pickable newPickable = Instantiate(replacement, dropPosition, dropRotation);

            OnDropPerformed?.Invoke(newPickable);

            if (newPickable.GetComponent<Rigidbody>())
            {
                newPickable.GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);
                newPickable.GetComponent<Rigidbody>().AddTorque(torque, ForceMode.VelocityChange);
            }
            else
            {
                Debug.LogError("The replacement object doesn't have a Rigidbody component.");
            }

            // Remove the current item from the inventory list if specified.
            if (removeFromList) inventory.items.Remove(this);

            // Switch to the last item in the inventory.
            inventory?.Switch(inventory.items.Count - 1);

            // Destroy the current item after dropping it.
            Destroy(gameObject);
        }


#if UNITY_EDITOR
        [ContextMenu("Setup/Default Animations")]
        public void SetupDefaultAnimation()
        {
            proceduralAnimator = transform.SearchFor<ProceduralAnimator>();

            if(!proceduralAnimator)
            {
                proceduralAnimator = gameObject.AddComponent<ProceduralAnimator>();
            }

            ProceduralAnimation breathingAnim = CreateAnimation("Breathing", proceduralAnimator);
            ProceduralAnimation breathingAimAnim = CreateAnimation("Breathing Aim", proceduralAnimator);

            ProceduralAnimation walkingAnim = CreateAnimation("Walking", proceduralAnimator);
            ProceduralAnimation sprintingAnim = CreateAnimation("Sprinting", proceduralAnimator);
            ProceduralAnimation tacSprintingAnim = CreateAnimation("Tactical Sprinting", proceduralAnimator);

            ProceduralAnimation aimAnim = CreateAnimation("Aiming", proceduralAnimator);
            ProceduralAnimation recoilAnim = CreateAnimation("Recoil", proceduralAnimator);
            ProceduralAnimation recoilAimAnim = CreateAnimation("Recoil Aim", proceduralAnimator);

            ProceduralAnimation jumpAnim = CreateAnimation("Jump", proceduralAnimator);
            ProceduralAnimation landAnim = CreateAnimation("Land", proceduralAnimator);

            ProceduralAnimation leanRAnim = CreateAnimation("Lean Right", proceduralAnimator);
            ProceduralAnimation leanLAnim = CreateAnimation("Lean Left", proceduralAnimator);
            ProceduralAnimation leanRAimAnim = CreateAnimation("Lean Right Aim", proceduralAnimator);
            ProceduralAnimation leanLAimAnim = CreateAnimation("Lean Left Aim", proceduralAnimator);

            if (breathingAnim)
            {
                //Basics
                breathingAnim.weight = 0.5f;
                breathingAnim.PerModifierConnections = true;

                //Connections
                ProceduralAnimationConnection connection = new ProceduralAnimationConnection();

                connection.target = aimAnim;
                connection.type = ProceduralAnimationConnectionType.AvoidInTrigger;

                breathingAnim.connections.Add(connection);

                //Modifiers
                WaveAnimationModifier waveModifier = breathingAnim.gameObject.AddComponent<WaveAnimationModifier>();
                waveModifier.position.amount = new Vector3(0, 0.005f, 0);
                waveModifier.position.speed = Vector3.one;

                waveModifier.rotation.amount = new Vector3(-0.68f, 0, 1);
                waveModifier.rotation.speed = new Vector3(1, 0, 1);

                waveModifier.syncWithAnimation = false;
            }

            if (breathingAimAnim)
            {
                //Basics
                breathingAimAnim.weight = 0.5f;
                breathingAimAnim.PerModifierConnections = true;

                //Connections
                ProceduralAnimationConnection connection = new ProceduralAnimationConnection();

                connection.target = aimAnim;
                connection.type = ProceduralAnimationConnectionType.AvoidInIdle;

                breathingAimAnim.connections.Add(connection);

                //Modifiers
                WaveAnimationModifier waveModifier = breathingAimAnim.gameObject.AddComponent<WaveAnimationModifier>();
                waveModifier.position.amount = new Vector3(0.0005f, 0.005f, 0);
                waveModifier.position.speed = new Vector3(1.21f, 1, 1);

                waveModifier.rotation.amount = new Vector3(-0.78f, 0, 0);
                waveModifier.rotation.speed = new Vector3(1, 0, 0);

                waveModifier.syncWithAnimation = false;
            }

            ///--------------------------

            if (walkingAnim)
            {
                //Basics
                walkingAnim.loop = true;

                //Connections
                ProceduralAnimationConnection connection = new ProceduralAnimationConnection();

                connection.target = aimAnim;
                connection.type = ProceduralAnimationConnectionType.AvoidInTrigger;

                walkingAnim.connections.Add(connection);

                //Modifiers
                WaveAnimationModifier waveModifier = walkingAnim.gameObject.AddComponent<WaveAnimationModifier>();
                waveModifier.position.amount = new Vector3(0.0005f, 0.0005f, 0);
                waveModifier.position.speed = new Vector3(1, 2, 1);

                waveModifier.rotation.amount = new Vector3(0.1f, 0, 0.1f);
                waveModifier.rotation.speed = new Vector3(1, 0, 1.27f);

                waveModifier.syncWithAnimation = false;
            }

            if (sprintingAnim)
            {
                //Basics
                sprintingAnim.length = 0.2f;

                sprintingAnim.triggerType = ProceduralAnimation.TriggerType.Hold;
                sprintingAnim.triggerInputAction = new UnityEngine.InputSystem.InputAction("Shit [Keyboard]", UnityEngine.InputSystem.InputActionType.Button, "<Keyboard>/shift");

                //Connections
                ProceduralAnimationConnection connection1 = new ProceduralAnimationConnection();
                ProceduralAnimationConnection connection2 = new ProceduralAnimationConnection();
                ProceduralAnimationConnection connection3 = new ProceduralAnimationConnection();
                ProceduralAnimationConnection connection4 = new ProceduralAnimationConnection();

                connection1.target = aimAnim;
                connection1.type = ProceduralAnimationConnectionType.AvoidInTrigger;
                
                connection2.target = recoilAnim;
                connection2.type = ProceduralAnimationConnectionType.AvoidInTrigger;

                connection3.target = recoilAimAnim;
                connection3.type = ProceduralAnimationConnectionType.AvoidInTrigger;

                connection4.target = tacSprintingAnim;
                connection4.type = ProceduralAnimationConnectionType.AvoidInTrigger;

                sprintingAnim.connections.Add(connection1);
                sprintingAnim.connections.Add(connection2);
                sprintingAnim.connections.Add(connection3);
                sprintingAnim.connections.Add(connection4);

                //Modifiers
                MoveAnimationModifier moveModifier = sprintingAnim.gameObject.AddComponent<MoveAnimationModifier>();
                SpringAnimationModifier springModifier1 = sprintingAnim.gameObject.AddComponent<SpringAnimationModifier>();
                SpringAnimationModifier springModifier2 = sprintingAnim.gameObject.AddComponent<SpringAnimationModifier>();

                moveModifier.position = new Vector3(-0.05f, -0.06f, -0.05f);
                moveModifier.rotation = new Vector3(11.1f, -16.49f, 28.2f);

                springModifier1.position.value = new Vector3(0, 0.015f, 0.01f);
                springModifier1.position.weight = 0.5f;

                springModifier1.rotation.value = new Vector3(0, 0, 5);


                springModifier2.position.value = new Vector3(0, 0.01f, 0.005f);
                springModifier2.position.fadeOutTime = 0.87f;

                springModifier2.rotation.value = new Vector3(0, 0, 3);

                //Trigger Modifiers
                //UnityEventTools.AddPersistentListener(characterManager.onJump, jumpKickModifier.Trigger);
                UnityEventTools.AddPersistentListener(sprintingAnim.events.OnPlayed, springModifier1.Trigger);
                UnityEventTools.AddPersistentListener(sprintingAnim.events.OnStoped, springModifier2.Trigger);
            }

            if (sprintingAnim)
            {
                //Basics
                sprintingAnim.length = 0.2f;

                sprintingAnim.triggerType = ProceduralAnimation.TriggerType.Hold;
                sprintingAnim.triggerInputAction = new UnityEngine.InputSystem.InputAction("Shit [Keyboard]", UnityEngine.InputSystem.InputActionType.Button, "<Keyboard>/shift");

                //Connections
                ProceduralAnimationConnection connection1 = new ProceduralAnimationConnection();
                ProceduralAnimationConnection connection2 = new ProceduralAnimationConnection();
                ProceduralAnimationConnection connection3 = new ProceduralAnimationConnection();

                connection1.target = aimAnim;
                connection1.type = ProceduralAnimationConnectionType.AvoidInTrigger;

                connection2.target = recoilAnim;
                connection2.type = ProceduralAnimationConnectionType.AvoidInTrigger;

                connection3.target = recoilAimAnim;
                connection3.type = ProceduralAnimationConnectionType.AvoidInTrigger;

                sprintingAnim.connections.Add(connection1);
                sprintingAnim.connections.Add(connection2);
                sprintingAnim.connections.Add(connection3);

                //Modifiers
                MoveAnimationModifier moveModifier = sprintingAnim.gameObject.AddComponent<MoveAnimationModifier>();
                SpringAnimationModifier springModifier1 = sprintingAnim.gameObject.AddComponent<SpringAnimationModifier>();
                SpringAnimationModifier springModifier2 = sprintingAnim.gameObject.AddComponent<SpringAnimationModifier>();

                moveModifier.position = new Vector3(-0.08f, -0.04f, -0.1f);
                moveModifier.rotation = new Vector3(-9.5f, -34.14f, 16.9f);

                springModifier1.position.value = new Vector3(0, 0.015f, 0.01f);
                springModifier1.position.weight = 0.5f;

                springModifier1.rotation.value = new Vector3(0, 0, 5);


                springModifier2.position.value = new Vector3(0, 0.01f, 0.005f);
                springModifier2.position.fadeOutTime = 0.87f;

                springModifier2.rotation.value = new Vector3(0, 0, 3);

                //Trigger Modifiers
                //UnityEventTools.AddPersistentListener(characterManager.onJump, jumpKickModifier.Trigger);
                UnityEventTools.AddPersistentListener(sprintingAnim.events.OnPlayed, springModifier1.Trigger);
                UnityEventTools.AddPersistentListener(sprintingAnim.events.OnStoped, springModifier2.Trigger);
            }

            ///--------------------------

            if (aimAnim)
            {
                //Basics
                aimAnim.length = 0.2f;

                aimAnim.triggerType = ProceduralAnimation.TriggerType.Hold;
                aimAnim.triggerInputAction = new UnityEngine.InputSystem.InputAction("Right Button [Mouse]", UnityEngine.InputSystem.InputActionType.Button, "<Mouse>/rightButton");

                //Modifiers
                MoveAnimationModifier moveModifier = aimAnim.gameObject.AddComponent<MoveAnimationModifier>();
                SpringAnimationModifier springModifier1 = aimAnim.gameObject.AddComponent<SpringAnimationModifier>();
                SpringAnimationModifier springModifier2 = aimAnim.gameObject.AddComponent<SpringAnimationModifier>();

                moveModifier.position = new Vector3(-0.08f, 0.023f, -0.06f);
                moveModifier.rotation = new Vector3(0, 0, 0);

                springModifier1.position.value = new Vector3(0, 0.005f, 0);
                springModifier1.position.weight = 0.5f;

                springModifier1.rotation.value = new Vector3(0, 0, 2);
                springModifier1.rotation.weight = 0.33f;


                springModifier2.position.value = new Vector3(0, 0.005f, 0);

                springModifier2.rotation.value = new Vector3(0, 0, 2);

                //Trigger Modifiers
                UnityEventTools.AddPersistentListener(aimAnim.events.OnPlayed, springModifier1.Trigger);
                UnityEventTools.AddPersistentListener(aimAnim.events.OnStoped, springModifier2.Trigger);
            }

            if (recoilAnim)
            {
                //Basics
                recoilAnim.length = 0.01f;
                recoilAnim.AutoStop = true;
                recoilAnim.PerModifierConnections = true;

                //Connections
                ProceduralAnimationConnection connection1 = new ProceduralAnimationConnection();
                ProceduralAnimationConnection connection2 = new ProceduralAnimationConnection();

                connection1.target = aimAnim;
                connection1.type = ProceduralAnimationConnectionType.AvoidInTrigger;

                connection2.target = sprintingAnim;
                connection2.type = ProceduralAnimationConnectionType.AvoidInTrigger;

                recoilAnim.connections.Add(connection1);
                recoilAnim.connections.Add(connection2);

                //Modifiers
                KickAnimationModifier kickModifier = recoilAnim.gameObject.AddComponent<KickAnimationModifier>();
                SpringAnimationModifier springModifier = recoilAnim.gameObject.AddComponent<SpringAnimationModifier>();

                kickModifier.staticPosition = new Vector3(0, 0, -0.4f);
                kickModifier.randomPosition = new Vector3(0.01f, 0.01f, 0);
                kickModifier.randomRotation = new Vector3(-8, 5, 38);

                springModifier.position.value = new Vector3(0, 0.01f, -0.02f);
                springModifier.rotation.value = new Vector3(-2, 0, 0);

                UnityEventTools.AddPersistentListener(recoilAnim.events.OnPlay, kickModifier.Trigger);
                UnityEventTools.AddPersistentListener(recoilAnim.events.OnPlay, springModifier.Trigger);
            }

            if (recoilAimAnim) 
            {
                //Basics
                recoilAimAnim.length = 0.01f;
                recoilAimAnim.AutoStop = true;
                recoilAimAnim.PerModifierConnections = true;

                //Connections
                ProceduralAnimationConnection connection = new ProceduralAnimationConnection();

                connection.target = aimAnim;
                connection.type = ProceduralAnimationConnectionType.AvoidInIdle;

                recoilAimAnim.connections.Add(connection);

                //Modifiers
                KickAnimationModifier kickModifier = recoilAimAnim.gameObject.AddComponent<KickAnimationModifier>();
                SpringAnimationModifier springModifier = recoilAimAnim.gameObject.AddComponent<SpringAnimationModifier>();

                kickModifier.staticPosition = new Vector3(0, 0, -0.3f);
                kickModifier.randomPosition = new Vector3(0.01f, 0.01f, 0);
                kickModifier.randomRotation = new Vector3(-1, 1, 14);

                springModifier.position.value = new Vector3(0, 0.005f, -0.01f);
                springModifier.rotation.value = new Vector3(-1, 0, 0);

                UnityEventTools.AddPersistentListener(recoilAimAnim.events.OnPlay, kickModifier.Trigger);
                UnityEventTools.AddPersistentListener(recoilAimAnim.events.OnPlay, springModifier.Trigger);
            }


            ///--------------------------

            if (jumpAnim)
            {
                //Basics
                jumpAnim.weight = 0.5f;
                jumpAnim.length = 0.15f;
                jumpAnim.AutoStop = true;
                jumpAnim.PerModifierConnections = true;

                //Modifiers
                KickAnimationModifier kickModifier = jumpAnim.gameObject.AddComponent<KickAnimationModifier>();
                SpringAnimationModifier springModifier = jumpAnim.gameObject.AddComponent<SpringAnimationModifier>();

                kickModifier.UpdateMode = UpdateMode.Update;
                kickModifier.positionRoughness = 5;
                kickModifier.rotationRoughness = 5;

                kickModifier.staticPosition = new Vector3(0, -0.18f, 0);
                kickModifier.staticRotation = new Vector3(7, 0, 0);

                springModifier.position.value = new Vector3(0, -0.02f, -0.01f);
                springModifier.rotation.value = new Vector3(4, 0, 0);

                UnityEventTools.AddPersistentListener(jumpAnim.events.OnPlay, kickModifier.Trigger);
                UnityEventTools.AddPersistentListener(jumpAnim.events.OnPlay, springModifier.Trigger);
            }

            if (landAnim)
            {
                //Basics
                landAnim.length = 0.15f;
                landAnim.AutoStop = true;

                //Modifiers
                KickAnimationModifier kickModifier = landAnim.gameObject.AddComponent<KickAnimationModifier>();
                SpringAnimationModifier springModifier = landAnim.gameObject.AddComponent<SpringAnimationModifier>();

                kickModifier.UpdateMode = UpdateMode.Update;
                kickModifier.positionRoughness = 5;
                kickModifier.rotationRoughness = 5;

                kickModifier.staticPosition = new Vector3(0, -0.2f, 0);
                kickModifier.staticRotation = new Vector3(7, 0, 0);

                springModifier.position.value = new Vector3(0, -0.02f, -0.02f);
                springModifier.position.fadeOutTime = 0.65f;

                springModifier.rotation.value = new Vector3(2, 0, 2);
                springModifier.rotation.fadeOutTime = 0.8f;

                UnityEventTools.AddPersistentListener(landAnim.events.OnPlay, kickModifier.Trigger);
                UnityEventTools.AddPersistentListener(landAnim.events.OnPlay, springModifier.Trigger);
            }

            ///--------------------------

            if (leanRAnim)
            {
                //Basics
                leanRAnim.triggerType = ProceduralAnimation.TriggerType.Hold;
                leanRAnim.triggerInputAction = new UnityEngine.InputSystem.InputAction("E [Keyboard]", UnityEngine.InputSystem.InputActionType.Button, "<Keyboard>/e");

                //Connections
                ProceduralAnimationConnection connection = new ProceduralAnimationConnection();

                connection.target = aimAnim;
                connection.type = ProceduralAnimationConnectionType.AvoidInTrigger;

                leanRAnim.connections.Add(connection);

                //Modifiers
                MoveAnimationModifier moveModifier = leanRAnim.gameObject.AddComponent<MoveAnimationModifier>();
                moveModifier.position = new Vector3(0.01f, -0.02f, 0);
                moveModifier.rotation = new Vector3(0, 0, -15);
            }

            if (leanLAnim)
            {
                //Basics
                leanLAnim.triggerType = ProceduralAnimation.TriggerType.Hold;
                leanLAnim.triggerInputAction = new UnityEngine.InputSystem.InputAction("Q [Keyboard]", UnityEngine.InputSystem.InputActionType.Button, "<Keyboard>/q");

                //Connections
                ProceduralAnimationConnection connection = new ProceduralAnimationConnection();

                connection.target = aimAnim;
                connection.type = ProceduralAnimationConnectionType.AvoidInTrigger;

                leanLAnim.connections.Add(connection);

                //Modifiers
                MoveAnimationModifier moveModifier = leanLAnim.gameObject.AddComponent<MoveAnimationModifier>();
                moveModifier.position = new Vector3(-0.13f, -0.02f, 0);
                moveModifier.rotation = new Vector3(0, 0, 15);
            }

            if (leanRAimAnim)
            {
                //Basics
                leanRAimAnim.triggerType = ProceduralAnimation.TriggerType.Hold;
                leanRAimAnim.triggerInputAction = new UnityEngine.InputSystem.InputAction("E [Keyboard]", UnityEngine.InputSystem.InputActionType.Button, "<Keyboard>/e");

                //Connections
                ProceduralAnimationConnection connection = new ProceduralAnimationConnection();

                connection.target = aimAnim;
                connection.type = ProceduralAnimationConnectionType.AvoidInIdle;

                leanRAimAnim.connections.Add(connection);

                //Modifiers
                MoveAnimationModifier moveModifier = leanRAimAnim.gameObject.AddComponent<MoveAnimationModifier>();
                moveModifier.position = new Vector3(-0.038f, 0.007f, 0);
                moveModifier.rotation = new Vector3(0, 0, -25);
            }

            if (leanLAimAnim)
            {
                //Basics
                leanLAimAnim.triggerType = ProceduralAnimation.TriggerType.Hold;
                leanLAimAnim.triggerInputAction = new UnityEngine.InputSystem.InputAction("Q [Keyboard]", UnityEngine.InputSystem.InputActionType.Button, "<Keyboard>/q");

                //Connections
                ProceduralAnimationConnection connection = new ProceduralAnimationConnection();

                connection.target = aimAnim;
                connection.type = ProceduralAnimationConnectionType.AvoidInIdle;

                leanLAimAnim.connections.Add(connection);

                //Modifiers
                MoveAnimationModifier moveModifier = leanLAimAnim.gameObject.AddComponent<MoveAnimationModifier>();
                moveModifier.position = new Vector3(0.038f, 0.007f, 0);
                moveModifier.rotation = new Vector3(0, 0, 25);
            }

            proceduralAnimator.RefreshClips();
        }

        private ProceduralAnimation CreateAnimation(string name, ProceduralAnimator animator)
        {
            ProceduralAnimation animation = animator.GetAnimation(name);

            if (!animation)
            {
                animation = new GameObject($"{name} Animation").AddComponent<ProceduralAnimation>();
                animation.transform.SetParent(animator.transform);
                animation.transform.Reset();

                animation.Name = name;

                Undo.RegisterCreatedObjectUndo(animation.gameObject, $"Created {name} animation");
            }

            return animation;
        }
#endif
    }
}