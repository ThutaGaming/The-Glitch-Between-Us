using Akila.FPSFramework.Animation;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Weapone/Firearm")]
    [RequireComponent(typeof(FirearmAttachmentsManager))]
    public class Firearm : InventoryItem
    {
        [Tooltip("The firearm preset that defines all values for this firearm. This preset is a ScriptableObject.")]
        public FirearmPreset preset;

        [Tooltip("The Transform from which the shots are fired.")]
        public Transform muzzle;

        [Tooltip("The Transform from which shell casings are ejected.")]
        public Transform casingEjectionPort;

        [Tooltip("Particle system effects that play when chambering a bullet.")]
        public ParticleSystem chamberingEffects;

        [Tooltip("Events related to this firearm."), Space]
        public FirearmEvents events;

        public InventoryCollectable ammoProfile { get; set; }

        private Crosshair crosshair;
        /// <summary>
        /// Pattern for bullet spread when hip firing.
        /// </summary>
        private SprayPattern hipFireSprayPattern;

        /// <summary>
        /// Pattern for bullet spread when aiming down sights.
        /// </summary>
        private SprayPattern aimDownSightsSprayPattern;

        /// <summary>
        /// The sound that plays when firing the firearm.
        /// </summary>
        public Audio fireAudio;

        /// <summary>
        /// The currently extrenally assigned audio.
        /// </summary>
        public Audio currentFireAudio;

        /// <summary>
        /// The sound that plays after the shot (the tail sound).
        /// </summary>
        public Audio fireTailAudio;

        /// <summary>
        /// The sound that plays when reloading the firearm.
        /// </summary>
        public Audio reloadAudio;

        /// <summary>
        /// The sound that plays when reloading an empty magazine.
        /// </summary>
        public Audio reloadEmptyAudio;

        /// <summary>
        /// The multiplier for tactical sprint speed.
        /// </summary>
        private float tacticalSprintMultiplier;

        /// <summary>
        /// The HUD elements related to this firearm.
        /// </summary>
        public FirearmHUD firearmHUD { get; set; }

        /// <summary>
        /// Manages attachments for this firearm.
        /// </summary>
        public FirearmAttachmentsManager firearmAttachmentsManager { get; protected set; }

        /// <summary>
        /// Array of particle systems related to this firearm.
        /// </summary>
        public ParticleSystem[] firearmParticleEffects { get; set; }

        /// <summary>
        /// The current fire mode setting of the firearm.
        /// </summary>
        public FireMode currentFireMode { get; set; }

        /// <summary>
        /// The number of shots fired in the current session.
        /// </summary>
        public int shotsFired { get; protected set; }

        /// <summary>
        /// Represents the current spray progression as a normalized value between 0 and 1, 
        /// where 0 indicates no spray and 1 indicates full spray.
        /// </summary>
        public float currentSprayAmount { get; protected set; }

        // The current multiplier for the spray pattern, affecting the overall intensity of the spread.
        private float currentSprayMultiplier;

        /// <summary>
        /// The current amount of ammo remaining in the magazine.
        /// </summary>
        public int remainingAmmoCount { get; set; }

        /// <summary>
        /// The current count of ammo for the specific ammo type.
        /// </summary>
        public int remainingAmmoTypeCount { get; set; }

        /// <summary>
        /// The maximum capacity of the magazine.
        /// </summary>
        public int magazineCapacity { get; set; }

        /// <summary>
        /// Whether the firearm is currently reloading.
        /// </summary>
        public bool isReloading { get; set; }

        /// <summary>
        /// Whether the firearm is currently firing.
        /// </summary>
        public bool isFiring { get; set; }

        /// <summary>
        /// Whether the firearm is out of ammo.
        /// </summary>
        public bool isOutOfAmmo { get; set; }

        /// <summary>
        /// Whether the player is attempting to fire the firearm.
        /// </summary>
        public bool attemptingToFire { get; protected set; }

        /// <summary>
        /// Whether the firearm is ready to fire.
        /// </summary>
        public bool readyToFire { get; protected set; }

        /// <summary>
        /// A flag that determines whether the firing action is prevented. 
        /// This is useful for disabling the ability to fire, such as when the player is interacting with an inventory UI or performing other non-combat actions.
        /// </summary>
        public bool firePrevented { get; set; }

        /// <summary>
        /// Whether the firearm is ready to be reloaded.
        /// </summary>
        public bool readyToReload { get; protected set; }

        /// <summary>
        /// Controls the visibility of the HUD.
        /// </summary>
        public bool isHudActive { get; set; } = true;

        /// <summary>
        /// Controls the audio system's activation state.
        /// </summary>
        public bool isAudioActive { get; set; } = true;

        /// <summary>
        /// Controls the particle system effects's activation state.
        /// </summary>
        public bool isParticleEffectsActive { get; set; } = true;

        /// <summary>
        /// <summary>
        /// Indicates whether throwing casings is enabled. Defaults to true.
        /// </summary>
        public bool isThrowingCasingActive { get; set; } = true;

        /// <summary>
        /// Controls whether player input is enabled.
        /// </summary>
        public bool isInputActive { get; set; } = true;

        /// <summary>
        /// Represents the current progress of the aiming animation, ranging from 0 (not aiming) to 1 (fully aimed).
        /// </summary>
        public float aimProgress
        {
            get
            {
                if(aimingAnimation != null)
                    return aimingAnimation.progress;
                
                return 0;
            }
        }

        /// <summary>
        /// Tracks the delay between shots fired.
        /// </summary>
        private float fireTimer;

        // Action to be invoked when the casing has been thrown.
        public Action onCasingThrown;

        private bool isPreviouslyReloading;

        protected override void Awake()
        {
            base.Awake();

            //Initialize ammo profile from inventory
            foreach (InventoryCollectable collectable in inventory.collectables)
            {
                if (collectable.GetIdentifier() == preset.ammoType)
                    ammoProfile = collectable;
            }

            // Initialize ammo profile if not set
            if (ammoProfile == null)
            {
                Debug.LogError("Ammo profile is not set. Using a default instance.", preset);

                ammoProfile = new InventoryCollectable();

                ammoProfile.identifier = ScriptableObject.CreateInstance<AmmoProfileData>();

                ammoProfile.identifier.displayName = "Unknown Ammo Profile";

                ammoProfile.SetCount(500);
            }


            // Set the initial ammo and magazine capacity based on the preset.
            remainingAmmoCount = preset.reserve;
            magazineCapacity = preset.magazineCapacity;
        }


        protected virtual void Start()
        {
            events.OnFireDone.AddListener(DoFireDone);
            OnDropPerformed.AddListener(ApplyFirearmToPickable);

            // Check if a valid preset is provided
            if (preset == null)
            {
                Debug.LogWarning("Preset is null. No audio setup will be performed.");
                return;
            }

            // Setup fire audio if the preset provides it
            if (preset.fireSound != null)
            {
                fireAudio = new Audio();
                fireAudio.Setup(gameObject, preset.fireSound);

                // Set the current fire audio profile based on the preset.
                currentFireAudio = fireAudio;
            }

            // Setup fire tail audio if the preset provides it
            if (preset.fireTailSound != null)
            {
                fireTailAudio = new Audio();
                fireTailAudio.Setup(gameObject, preset.fireTailSound);
            }

            // Setup reload audio if the preset provides it
            if (preset.reloadSound != null)
            {
                reloadAudio = new Audio();
                reloadAudio.Setup(gameObject, preset.reloadSound);
            }

            // Setup reload empty audio if the preset provides it
            if (preset.reloadEmptySound != null)
            {
                reloadEmptyAudio = new Audio();
                reloadEmptyAudio.Setup(gameObject, preset.reloadEmptySound);
            }

            // Get the required components from the GameObject.
            firearmAttachmentsManager = GetComponent<FirearmAttachmentsManager>();
            itemInput = GetComponent<ItemInput>();

            // Validate the firearm preset.
            if (preset == null)
            {
                Debug.LogError("Firearm preset is not assigned.", gameObject);
                return;
            }

            // Get all particle systems attached to the firearm.
            firearmParticleEffects = GetComponentsInChildren<ParticleSystem>();

            // Instantiate the HUD for the firearm and set its reference to this firearm.
            if (preset.firearmHud == null)
            {
                Debug.LogError("FirearmHUD is not set in the preset. Firearm's HUD won't be initialized.", gameObject);
            }
            else
            {
                firearmHUD = Instantiate(preset.firearmHud, transform);
                firearmHUD.firearm = this;
            }

            if(preset.crosshair == null)
            {
                Debug.LogError("Crosshair is not set in the preset. Fire's crosshair won't be initialized.", gameObject);
            }
            else
            {
                crosshair = Instantiate(preset.crosshair, firearmHUD.transform);
                crosshair.firearm = this;
            }

            // Initialize spray patterns, use a default if none is provided in the preset.
            if (preset.sprayPattern == null)
            {
                Debug.LogError($"Spray pattern is not assigned in the preset. Using a default instance. For better control and accuracy, consider assigning a custom spray pattern.", preset);
                hipFireSprayPattern = ScriptableObject.CreateInstance<SprayPattern>();
            }
            else
            {
                hipFireSprayPattern = preset.sprayPattern;
            }

            if (preset.aimSprayPattern == null)
            {
                Debug.LogError($"Aim spray pattern is not assigned in the preset. Using a default instance. For better control and accuracy, consider assigning a custom spray pattern.", preset);
                hipFireSprayPattern = ScriptableObject.CreateInstance<SprayPattern>();
            }
            else
            {
                aimDownSightsSprayPattern = preset.aimSprayPattern;
            }

            // Validate muzzle and casing ejection port transforms.
            if (!muzzle)
            {
                Debug.LogError("Muzzle transform is not assigned. Defaulting to the firearm's transform.", gameObject);
                muzzle = transform;
            }

            if (!casingEjectionPort)
            {
                Debug.LogError("Casing ejection port transform is not assigned. Defaulting to the firearm's transform.", gameObject);
                casingEjectionPort = transform;
            }

            // Initialize reloading state.
            isReloading = false;

            // If the firearm has an inventory and character manager, reset the character's speed.
            if (characterManager != null)
            {
                characterManager.ResetSpeedMultiplier();
            }

            reloadingAnimation = proceduralAnimator.GetAnimation("Reloading");
        }

        private void ApplyFirearmToPickable(Pickable pickable)
        {
            if(pickable.TryGetComponent<FirearmPickableModule>(out FirearmPickableModule pickableModule))
            {
                pickableModule.storedAmmoCount = remainingAmmoCount;

                pickableModule.firearmAttachmentsManager = pickable.GetComponent<FirearmAttachmentsManager>();

                if(pickableModule.firearmAttachmentsManager && firearmAttachmentsManager != null)
                {
                    pickableModule.firearmAttachmentsManager.activeAttachments = firearmAttachmentsManager.activeAttachments;
                }
            }
        }

        ProceduralAnimation reloadingAnimation;

        /// <summary>
        /// Updates the firearm's state, including handling reloading, ammo, and animation states.
        /// </summary>
        protected override void Update()
        {
            //print(GetSprayPattern(muzzle.forward, muzzle.right, muzzle.up));

            // Call the base class Update method to ensure any inherited actions is performed.
            base.Update();

            if (reloadingAnimation)
            {
                reloadingAnimation.triggerType = ProceduralAnimation.TriggerType.None;
                reloadingAnimation.AutoStop = false;
                reloadingAnimation.IsPlaying = isReloading;
            }

            // Check if preset is assigned
            if (preset == null)
            {
                Debug.LogError($"The preset is not set. All firearm functionality will be disabled. A preset is essential for proper operation of the firearm.", gameObject);

                return;
            }

            // Update input and movement
            UpdateInput();
            AdjustPlayerSpeed();

            // Stop reloading if magazine is full and reload method is scripted
            if (preset.reloadMethod == ReloadType.Scripted && remainingAmmoCount >= magazineCapacity)
            {
                isReloading = false;
            }

            remainingAmmoTypeCount = ammoProfile.count;

            // Handle reloading state based on ammo count
            if (ammoProfile.count <= 0)
            {
                isReloading = false;
            }

            // Clamp remaining ammo count within magazine capacity
            remainingAmmoCount = Mathf.Clamp(remainingAmmoCount, 0, magazineCapacity);

            // Reset shots fired if it exceeds the preset shot count
            if (shotsFired >= preset.shotCount)
            {
                shotsFired = 0;
            }

            // Fire if in firing state
            if (isFiring)
            {
                Fire();

                if (!isAiming)
                    hipFireSprayPattern.RampupMagnitude(ref currentSprayMultiplier, Time.deltaTime);
                else
                    aimDownSightsSprayPattern.RampupMagnitude(ref currentSprayMultiplier, Time.deltaTime); 
            }
            else if (characterManager.IsAlmostStopped() && !isFiring)
            {
                if (!isAiming)
                    hipFireSprayPattern.Recover(ref currentSprayMultiplier, Time.deltaTime, ref currentSprayPointIdex);
                else
                    aimDownSightsSprayPattern.Recover(ref currentSprayMultiplier, Time.deltaTime, ref currentSprayPointIdex);
            }

            if (proceduralAnimator)
            {
                if (aimingAnimation)
                {
                    characterManager.isAiming = aimingAnimation.IsPlaying;
                    characterManager.attemptingToAim = aimingAnimation.velocity != 0;
                }
            }

            foreach (Animator animator in animators)
            {
                animator.SetBool("Is Reloading", isReloading);
                animator.SetInteger("Ammo", remainingAmmoCount);
                animator.SetFloat("ADS Amount", aimProgress);
                animator.SetFloat("Sprint Amount", tacticalSprintMultiplier);
            }

            if (characterManager.IsAlmostStopped() == false && !isAiming && characterManager.isGrounded)
            {
                float multiplier = Mathf.Lerp(0.5f, 1, characterManager.velocity.magnitude / characterManager.sprintSpeed);

                currentSprayAmount = hipFireSprayPattern.maxAmount * multiplier;
            }
            else if(!characterManager.isGrounded && !isAiming)
            {
                currentSprayAmount = hipFireSprayPattern.maxAmount;
            }
            else
            {
                currentSprayAmount = Mathf.Lerp(hipFireSprayPattern.maxAmount * 0.5f, aimDownSightsSprayPattern.maxAmount, aimProgress) * currentSprayMultiplier;
            }
        }
        

        private void LateUpdate()
        {
            if (isReloading && !isPreviouslyReloading)
            {
                events.OnReloadStart?.Invoke();
            }

            if (!isReloading && isPreviouslyReloading)
            {
                events.OnReloadComplete?.Invoke();
            }

            isPreviouslyReloading = isReloading;

            if(currentSprayMultiplier > 1)
            {
                currentSprayMultiplier = 1;
            }
        }


        /// <summary>
        /// Updates the player's movement speed based on the current action (firing, aiming, or idle).
        /// </summary>
        protected virtual void AdjustPlayerSpeed()
        {
            // Check if the preset is assigned
            if (preset == null)
            {
                Debug.LogError("The preset is not assigned. Player speed adjustments will not be applied. Please ensure a valid preset is configured.", gameObject);
                return;
            }

            // Check if the character manager is assigned
            if (characterManager == null)
            {
                Debug.LogError("The character manager is not assigned. Player speed adjustments will not be applied. Please assign a valid character manager.", gameObject);
                return;
            }

            // Adjust player speed based on the current state
            if (isFiring)
            {
                characterManager.SetSpeedMultiplier(preset.fireWalkPlayerSpeed);
            }
            else if (isAiming)
            {
                characterManager.SetSpeedMultiplier(preset.aimWalkPlayerSpeed);
            }
            else
            {
                characterManager.SetSpeedMultiplier(preset.basePlayerSpeed);
            }
        }

        protected virtual void UpdateInput()
        {
            if (FPSFrameworkCore.IsActive == false)
                return;

            // Cancel reloading if firing and allowed
            if (preset.canCancelReloading && isFiring)
            {
                CancelReload();
            }

            // Exit if item input is not active
            if (!itemInput || !isInputActive)
            {
                return;
            }

            // Handle item input actions
            if (itemInput.DropInput)
            {
                Drop();
            }

            if (itemInput.ReloadInput)
            {
                Reload();
            }

            bool isRotationDefault = true;

            // Check if procedural animator's rotation is default
            if (proceduralAnimator)
            {
                isRotationDefault = proceduralAnimator.IsDefaultingInRotation(preset.maxAimDeviation, true, true, false);
            }

            if (preset.isFireActive)
            {
                // Handle fire mode switch
                if (preset.fireMode == FireMode.Selective)
                {
                    if (itemInput.FireModeSwitchInput)
                    {
                        currentFireMode = (currentFireMode == FireMode.Auto) ? FireMode.SemiAuto: FireMode.Auto;

                        events.OnFireModeChange?.Invoke();
                        
                        Debug.Log($"Selective Mode Switched To: {currentFireMode}");
                    }
                }
                else
                {
                    currentFireMode = preset.fireMode;
                }

                // Update firing status based on fire mode
                if (readyToFire)
                {
                    isFiring = (currentFireMode == FireMode.Auto)
                        ? itemInput.HeldFire
                        : itemInput.SingleFire;
                }
                else
                {
                    isFiring = false;
                }

                // Attempt to fire if conditions are met
                bool isFireInputActive = (currentFireMode == FireMode.Auto)
                    ? itemInput.HeldFire
                    : itemInput.SingleFire;

                if (itemInput.SingleFire && remainingAmmoCount == 0 && preset.canAutomaticallyReload)
                {
                    Reload();
                }

                attemptingToFire = isFireInputActive && remainingAmmoCount > 0;

                // Determine readiness to reload and fire
                readyToReload = !(remainingAmmoCount >= magazineCapacity || remainingAmmoTypeCount <= 0 || isReloading);
                readyToFire = (!isReloading || preset.canCancelReloading) && isRotationDefault && remainingAmmoCount > 0
                    && !IsPlayingRestrictedAnimation() && !FPSFrameworkCore.IsPaused;
            }
        }

        /// <summary>
        /// Initiates the firing sequence based on the current firing state and preset settings.
        /// </summary>
        public void Fire()
        {
            // Exit if not ready to fire
            if (!readyToFire || firePrevented)
            {
                return;
            }

            Vector3 firePosition = Vector3.zero;
            Quaternion fireRotation = Quaternion.identity;
            Vector3 fireDirection = Vector3.zero;

            Camera mainCamera = FPSFrameworkUtility.GetMainCamera();

            // Determine the firing position and direction based on preset settings
            switch (preset.shootingDirection)
            {
                case ShootingDirection.MuzzleForward:

                    firePosition = muzzle.position;
                    fireRotation = muzzle.rotation;
                    fireDirection = muzzle.forward;

                    break;

                case ShootingDirection.CameraForward:

                    firePosition = mainCamera.transform.position;
                    fireRotation = mainCamera.transform.rotation;
                    fireDirection = mainCamera.transform.forward;

                    break;

                case ShootingDirection.FromMuzzleToCameraForward:

                    RaycastHit hitInfo;
                    if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hitInfo, preset.hittableLayers))
                    {
                        if (hitInfo.distance > 5f)
                        {
                            firePosition = muzzle.position;
                            fireRotation = muzzle.rotation;
                            fireDirection = (hitInfo.point - muzzle.position).normalized;
                        }
                        else
                        {
                            firePosition = mainCamera.transform.position;
                            fireRotation = mainCamera.transform.rotation;
                            fireDirection = mainCamera.transform.forward;
                        }
                    }
                    else
                    {
                        firePosition = muzzle.position;
                        fireRotation = muzzle.rotation;
                        fireDirection = mainCamera.transform.forward;
                    }

                    break;

                case ShootingDirection.FromCameraToMuzzleForward:

                    firePosition = mainCamera.transform.position;
                    fireRotation = mainCamera.transform.rotation;

                    RaycastHit hitInfo2;
                    if (Physics.Raycast(muzzle.position, muzzle.forward, out hitInfo2) && hitInfo2.distance > 5f)
                    {
                        fireDirection = (hitInfo2.point - mainCamera.transform.position).normalized;
                    }
                    else
                    {
                        fireDirection = muzzle.forward;
                    }

                    break;
            }
            Speedometer speedometer = muzzle.GetOrAddComponent<Speedometer>();

            firePosition += speedometer.PredictPosition(Time.unscaledDeltaTime, false);

            // Execute the firing logic with the calculated position, rotation, and direction
            Fire(firePosition, fireRotation, fireDirection);
        }

        /// <summary>
        /// Executes the firing action with specified position, rotation, and direction.
        /// </summary>
        /// <param name="position">The position from which to fire.</param>
        /// <param name="rotation">The rotation of the firearm during firing.</param>
        /// <param name="direction">The direction in which the projectile or hit scan will be fired.</param>
        public void Fire(Vector3 position, Quaternion rotation, Vector3 _direction)
        {
            // Exit if not ready to fire or fire timer has not elapsed
            if (!readyToFire || Time.time <= fireTimer)
            {
                return;
            }

            Vector3 finalDirection = Vector3.zero;

            float attachmentFireRate = firearmAttachmentsManager ? firearmAttachmentsManager.fireRate : 1;

            // Update fire timer
            fireTimer = Time.time + 60f / (preset.fireRate * attachmentFireRate);

            // Check if currently playing a restricted animation
            if (!IsPlayingRestrictedAnimation())
            {
                shotsFired = 0;
                finalDirection = GetSprayPattern(_direction, muzzle.right, muzzle.up);

                originalFireDirection = _direction;

                FireDone(position, rotation, finalDirection);

                events.OnFireDemand?.Invoke(position, rotation, finalDirection);

                // Apply fire logic if not set to always apply fire
                if (!preset.alwaysApplyFire)
                {
                    ApplyFireOnce();

                    // Handle gamepad vibration if enabled
                    GamepadManager gamepadManager = GamepadManager.Instance;

                    if (gamepadManager)
                    {
                        gamepadManager.BeginVibration(preset.gamepadVibrationAmountRight, preset.gamepadVibrationAmountLeft, preset.gamepadVibrationDuration);
                    }
                }
            }
        }

        private Vector3 currentFirePosition;
        private Quaternion currentFireRotation;
        private Vector3 currentFireDirection;
        private Vector3 originalFireDirection;
        private int currentSprayPointIdex;

        protected virtual void InvokeFireDone()
        {
            FireDone(currentFirePosition, currentFireRotation, currentFireDirection);
        }

        /// <summary>
        /// Handles the final steps of the firing process, including visual effects and projectile/hit scan logic.
        /// </summary>
        /// <param name="position">The origin position of the projectile or hit scan.</param>
        /// <param name="rotation">The firearm's rotation at the moment of firing.</param>
        /// <param name="direction">The direction in which the projectile or hit scan is fired.</param>
        public virtual void FireDone(Vector3 position, Quaternion rotation, Vector3 direction)
        {
            this.currentFirePosition = position;
            this.currentFireRotation = rotation;

            Vector3 sprayDirection = preset.shotCount > 1 ? GetSprayPattern(direction, muzzle.right, muzzle.up) : direction;

            this.currentFireDirection = sprayDirection;

            events.OnFireDone?.Invoke(position, rotation, currentFireDirection);

            // Trigger this event before executing local firing logic to facilitate networking.
            // This allows you to add a listener to the onFireDone event and make it a server command.
            // Since elements like ammo count, camera recoil, and shell casing spawn don't require full networking, 
            // they are handled locally.

            // Apply firing logic regardless of active status if always apply fire is enabled.
            if (preset.alwaysApplyFire)
            {
                ApplyFireOnce();
            }
            
            // Cancel any pending FireDone invocations
            CancelInvoke();

            // Increment shots fired and handle multiple shots if needed
            shotsFired++;

            if (shotsFired < preset.shotCount && remainingAmmoCount > 0)
            {
                if (preset.shotDelay <= 0f)
                {
                    FireDone(position, rotation, currentFireDirection);
                }
                else if (shotsFired >= 1)
                {
                    Invoke(nameof(InvokeFireDone), preset.shotDelay);
                }
            }
        }

        private void DoFireDone(Vector3 position, Quaternion rotation, Vector3 direction)
        {
            // Exit the method if the firearm is not active.
            if (!isActive)
            {
                return;
            }

            // Play particle effects except for chambering effects
            foreach (ParticleSystem particleSystem in firearmParticleEffects)
            {
                if (particleSystem != chamberingEffects)
                {
                    particleSystem.Play();
                }
            }

            // Handle projectile shooting
            if (preset.shootingMechanism == ShootingMechanism.Projectiles)
            {
                if (preset.projectile)
                {
                    SpawnProjectile(position, rotation, currentFireDirection, preset.muzzleVelocity, preset.range);
                }
                else
                {
                    Debug.LogError($"{Name}'s projectile field is null. The firearm will not fire. Assign it and try again.");
                }
            }

            // Handle hitscan shooting
            if (preset.shootingMechanism == ShootingMechanism.Hitscan)
            {
                Ray ray = new Ray(muzzle.position, direction);

                if (Physics.Raycast(ray, out RaycastHit hit, preset.range, preset.hittableLayers))
                {
                    float damage = preset.alwaysApplyFire ? preset.damage : preset.damage / preset.shotCount;
                    Firearm.UpdateHits(this, preset.defaultDecal, ray, hit, damage, preset.decalDirection);
                }
            }
        }

        /// <summary>
        /// Applies the firing effects, including recoil, ammo consumption, animations, and camera shake.
        /// </summary>
        public void ApplyFireOnce()
        {
            // Apply recoil if enabled
            if (preset.isRecoilActive)
            {
                ApplyRecoil();
            }

            // Decrease remaining ammo count and update spray amount
            remainingAmmoCount--;

            // Play firing animation across all animators
            foreach (Animator animator in animators)
            {
                animator?.CrossFade("Fire", preset.fireTransition, 0, 0f);
            }

            foreach (ParticleSystem effect in firearmParticleEffects)
            {
                if (isParticleEffectsActive)
                {
                    if (effect != chamberingEffects)
                        effect.Play();
                }
            }

            // Shake camera if applicable
            CharacterManager characterManager = base.characterManager;
            CameraManager cameraManager = characterManager?.cameraManager;

            cameraManager?.ShakeCameras(preset.cameraShakeAmount, preset.cameraShakeRoughness, preset.cameraShakeDuration);

            // Handle firing audio
            if (isAudioActive)
            {
                if (preset.isAudioActive)
                {
                    currentFireAudio?.Play(true);

                    fireTailAudio?.Stop();
                    fireTailAudio?.Play(true);
                }
            }

            // Throw casing after firing
            ThrowCasing(casingEjectionPort);

            events.OnFireApplied?.Invoke();
        }

        /// <summary>
        /// Spawns a projectile at the specified position and rotation, with a given direction, speed, and range.
        /// </summary>
        /// <param name="position">The world position where the projectile should spawn.</param>
        /// <param name="rotation">The rotation of the projectile when it spawns.</param>
        /// <param name="direction">The direction in which the projectile will travel.</param>
        /// <param name="speed">The speed of the projectile.</param>
        /// <param name="range">The range the projectile can travel before it is destroyed or becomes ineffective.</param>
        /// <returns>Returns the spawned <see cref="Projectile"/> instance.</returns>
        /// <exception cref="System.NullReferenceException">Thrown when the projectile prefab or preset is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when the speed or range is less than or equal to zero.</exception>
        public Projectile SpawnProjectile(Vector3 position, Quaternion rotation, Vector3 direction, float speed, float range)
        {
            // Check if the preset or projectile prefab is null
            if (preset == null)
            {
                Debug.LogError("Firearm preset is not set.", gameObject);

                return null;
            }

            // Check if the preset or projectile prefab is null
            if (preset.projectile == null)
            {
                Debug.LogError("Projectile prefab is not set.", preset);

                return null;
            }

            // Instantiate the projectile prefab at the given position and rotation
            Projectile newProjectile = Instantiate(preset.projectile, position, rotation);

            // Initialize the velocity of the projectile to zero
            Vector3 initialVelocity = Vector3.zero;

            // If a character manager exists, add its velocity to the initial velocity of the projectile
            if (characterManager)
            {
                initialVelocity += characterManager.velocity;
            }

            // Set up the new projectile with the given parameters: owner, direction, initial velocity, speed, and range
            newProjectile.Setup(this, direction, initialVelocity, speed, range);

            // Return the newly spawned projectile
            return newProjectile;
        }

        /// <summary>
        /// Updates the state of objects hit by a projectile, including applying damage, handling decals, and applying forces.
        /// </summary>
        /// <param name="firearm">The firearm that fired the projectile.</param>
        /// <param name="projectile">The projectile that hit the target.</param>
        /// <param name="defaultDecal">The default decal to apply on the hit surface.</param>
        /// <param name="ray">The ray that represents the projectile's path.</param>
        /// <param name="hit">Information about the hit result.</param>
        /// <param name="damage">The amount of damage to apply.</param>
        /// <param name="decalDirection">The direction for orienting the decal.</param>
        public static void UpdateHits(Firearm firearm, GameObject defaultDecal, Ray ray, RaycastHit hit, float damage, Vector3Direction decalDirection)
        {
            // Check if the hit object should be ignored based on IgnoreHitDetection component
            if (hit.transform.TryGetComponent(out Ignore _ignore) && _ignore.ignoreHitDetection)
            {
                return;
            }

            //Exit if firearm is not set.
            if (firearm == null)
            {
                Debug.LogError($"Firearm is not set{new System.Diagnostics.StackTrace()}.");

                return;
            }

            //Exit if firearm preset is not set
            if(firearm.preset == null)
            {
                Debug.Log($"Firearm preset is not set {new System.Diagnostics.StackTrace()}.", firearm);

                return;
            }

            if(firearm.character == null)
            {
                Debug.LogError($"Character (ICharacterController) in the firearm is not set.", firearm);

                return;
            }

            IActor actor = firearm.actor;
            
            FirearmPreset preset = firearm.preset;

            FirearmAttachmentsManager firearmAttachmentsManager = firearm.firearmAttachmentsManager;

            ICharacterController character = firearm.character;

            // Invoke hit callbacks for the firearm
            InvokeHitCallbacks(character.gameObject, ray, hit);

            // Exit if the hit target is the same as the character
            if (hit.transform == character.transform)
            {
                return;
            }

            float damageMultiplier = 1;

            // Handle damageable groups
            if (hit.transform.TryGetComponent(out IDamageablePart damageablePart))
            {
                damageMultiplier = damageablePart.damageMultipler;
            }

            IDamageable damageable = hit.transform.SearchFor<IDamageable>();

            if (firearmAttachmentsManager)
                damageMultiplier *= firearmAttachmentsManager ? firearmAttachmentsManager.damage : 1;

            // Handle damageable objects
            if (damageable != null && damageable.Health > 0)
            {
                float totalDamage = damage * damageMultiplier;

                GameObject actorGO = null;

                if (actor != null) actorGO = actor.gameObject;

                damageable.Damage(totalDamage, actorGO);

                bool shouldHighlight = damageable.Health <= 0;

                if (firearm.character != null)
                {
                    UIManager uiManager = UIManager.Instance;

                    if (uiManager != null)
                    {
                        Hitmarker hitmarker = uiManager.Hitmarker;
                        hitmarker?.Show(shouldHighlight);
                    }
                }
            }

            // Handle custom decals
            if (hit.transform.TryGetComponent(out CustomDecal customDecal))
            {
                defaultDecal = customDecal.decalVFX;
            }

            // Exit if the hit target is the same as the character manager
            if (firearm?.characterManager?.transform == hit.transform)
            {
                return;
            }

            // Apply default or custom decal
            if (defaultDecal != null)
            {
                Vector3 hitPoint = hit.point;
                Quaternion decalRotation = FPSFrameworkCore.GetFromToRotation(hit, decalDirection);
                GameObject decalInstance = Instantiate(defaultDecal, hitPoint, decalRotation);

                if (customDecal && customDecal.parent || customDecal == null)
                {
                    decalInstance.transform.localScale *= firearm.preset.decalSize;
                    decalInstance.transform.SetParent(hit.transform);
                }

                float decalLifetime = customDecal?.lifeTime ?? 60f;
                Destroy(decalInstance, decalLifetime);
            }

            // Apply force to the rigidbody if present
            if (hit.rigidbody != null)
            {
                float impactForce = firearm.preset.shotDelay <= 0f
                    ? (firearm.preset.impactForce / firearm.preset.shotCount)
                    : firearm.preset.impactForce;

                hit.rigidbody.AddForceAtPosition(-hit.normal * impactForce, hit.point, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// Invokes hit-related callbacks on the hit object, as well as its children and parents, if applicable.
        /// </summary>
        /// <param name="sourcePlayer">The player responsible for the hit.</param>
        /// <param name="ray">The ray that caused the hit.</param>
        /// <param name="hit">Information about the hit.</param>
        public static void InvokeHitCallbacks(GameObject sourcePlayer, Ray ray, RaycastHit hit)
        {
            if (hit.transform.TryGetComponent<Ignore>(out Ignore _ignore) && _ignore.ignoreHitDetection) return;

            // Create a HitInfo object to store details about the hit
            HitInfo hitInfo = new HitInfo(sourcePlayer, hit, ray.origin, ray.direction);

            // Retrieve the GameObject that was hit
            GameObject obj = hit.transform.gameObject;

            // Try to get the IOnAnyHit interface implementation on the hit object, its children, and its parent
            IOnAnyHit onAnyHit = obj.transform.GetComponent<IOnAnyHit>();
            IOnAnyHitInChildren onAnyHitInChildren = obj.transform.GetComponentInParent<IOnAnyHitInChildren>();
            IOnAnyHitInParent onAnyHitInParent = obj.transform.GetComponentInChildren<IOnAnyHitInParent>();

            // Try to get the IOnHit interface implementation on the hit object, its children, and its parent
            IOnHit onHit = obj.transform.GetComponent<IOnHit>();
            IOnHitInChildren onHitInChildren = obj.transform.GetComponentInParent<IOnHitInChildren>();
            IOnHitInParent onHitInParent = obj.transform.GetComponentInChildren<IOnHitInParent>();

            // Invoke the OnHit method on the IOnHit interface, if implemented
            onHit?.OnHit(hitInfo);

            // Invoke the OnHitInChildren method on the IOnHitInChildren interface, if implemented
            onHitInChildren?.OnHitInChildren(hitInfo);

            // Invoke the OnHitInParent method on the IOnHitInParent interface, if implemented
            onHitInParent?.OnHitInParent(hitInfo);

            // Invoke the OnAnyHit method on the IOnAnyHit interface, if implemented
            onAnyHit?.OnAnyHit(hitInfo);

            // Invoke the OnAnyHitInChildren method on the IOnAnyHitInChildren interface, if implemented
            onAnyHitInChildren?.OnAnyHitInChildren(hitInfo);

            // Invoke the OnAnyHitInParent method on the IOnAnyHitInParent interface, if implemented
            onAnyHitInParent?.OnAnyHitInParent(hitInfo);
        }

        /// <summary>
        /// Instantiates and ejects a casing from the firearm, applying velocity and optional chambering effects.
        /// </summary>
        public virtual void ThrowCasing(Transform m_location)
        {
            // Ensure the onCasingThrown is invoked before exiting the method.
            // This allows external scripts to invoke the casing throw function, 
            // which can be used for networked casing spawning (e.g., for network code integration).
            onCasingThrown?.Invoke();

            //Exit if throw casing is false
            if (!isThrowingCasingActive) return;

            // Exit if casing prefab or ejection port is not assigned
            if (!preset.casing)
            {
                Debug.LogError("Casing prefab is not assigned in the preset.", preset);
                return;
            }

            if (!casingEjectionPort)
            {
                Debug.LogError("Casing ejection port is not assigned.", gameObject);
                return;
            }

            Transform location = m_location ? m_location : transform;

            // Instantiate the casing at the ejection port's position and rotation
            GameObject newCasing = Instantiate(preset.casing, location.position, location.rotation);
            Rigidbody casingRigidbody = newCasing.GetComponent<Rigidbody>();

            if (casingRigidbody == null)
            {
                Debug.LogError("Failed to get Rigidbody component from instantiated casing.", gameObject);
                return;
            }

            // Get the velocity of the ejection port's Speedometer, if available
            Speedometer speedometer = location.GetOrAddComponent<Speedometer>();
            Vector3 speedometerVelocity = speedometer ? speedometer.velocity : Vector3.zero;

            // Calculate the casing's velocity based on preset direction, speed, and a random factor
            Vector3 casingDirection = transform.GetDirection(preset.casingDirection);

            float randomFactor = UnityEngine.Random.Range(0.6f, 1f);

            Vector3 casingVelocity = (casingDirection * preset.casingVelocity) * randomFactor;

            casingVelocity += speedometerVelocity;

            // Apply the calculated velocity to the casing's Rigidbody
            if(!float.IsNaN(casingVelocity.x) && !float.IsNaN(casingVelocity.y) && !float.IsNaN(casingVelocity.z))
            {
                casingRigidbody.linearVelocity = casingVelocity;
            }

            // Destroy the casing object after 5 seconds
            Destroy(casingRigidbody.gameObject, 5f);

            // Play chambering effects if assigned
            if (chamberingEffects)
            {
                chamberingEffects.Play();
            }
        }

        /// <summary>
        /// Applies recoil effects to the weapon, including playing recoil animations and adjusting camera recoil based on the current settings.
        /// </summary>
        private void ApplyRecoil()
        {
            // Play recoil animations if the procedural animator is assigned
            if (proceduralAnimator)
            {
                recoilAnimation?.Play(0); // Play recoil animation from the beginning
                recoilAimAnimation?.Play(0); // Play recoil aim animation from the beginning
            }

            // Apply camera recoil adjustments if the character manager is assigned
            if (characterManager)
            {
                // Calculate the recoil values based on the preset and firearm attachments, considering aiming state
                float verticalRecoil = preset.verticalRecoil * firearmAttachmentsManager.recoil;
                float horizontalRecoil = preset.horizontalRecoil * firearmAttachmentsManager.recoil;
                float cameraRecoil = preset.cameraRecoil * firearmAttachmentsManager.recoil;

                characterManager.AddLookValue(verticalRecoil, horizontalRecoil);

                if(characterManager.cameraManager)
                {
                    if (characterManager.cameraManager.cameraKickAnimation)
                    {
                        characterManager.cameraManager.cameraKickAnimation.Play(0);

                        characterManager.cameraManager.cameraKickAnimation.weight = cameraRecoil;
                    }
                }
            }
        }
        
        /// <summary>
        /// Initiates the reloading process if the weapon is ready and ammo is available.
        /// </summary>
        public void Reload()
        {
            // Exit early if the weapon is not ready to reload
            if (!readyToReload)
            {
                return;
            }

            reloadAudio?.Stop();
            reloadEmptyAudio?.Stop();

            // Check if there is ammo available to reload
            if (ammoProfile.count > 0)
            {
                // Reset the MagThrown flag if WeaponEvents component exists
                var weaponEvents = GetComponentInChildren<WeaponEvents>();

                if (weaponEvents != null)
                {
                    weaponEvents.MagThrown = false;
                }

                // Set reloading state and trigger reload event
                isReloading = true;
                events.OnReloadStarting?.Invoke();

                // Start the reload process
                StartReload();
            }
            else
            {
                // No ammo available, so set reloading state to false
                isReloading = false;
            }
        }

        /// <summary>
        /// Starts the reload animation or timer based on the reload method specified in the preset.
        /// </summary>
        private void StartReload()
        {
            // Use scripted reload method if specified
            if (preset.reloadMethod == ReloadType.Scripted)
            {
                foreach (var animator in animators)
                {
                    animator.CrossFade(preset.reloadStateName, preset.reloadTransitionTime, 0, 0f);
                }

                isReloading = true;
                return;
            }

            // Use default reload timing based on remaining ammo
            float reloadTime = remainingAmmoCount <= 0 ? preset.emptyReloadTime : preset.reloadTime;

            Invoke(nameof(ApplyReload), reloadTime);

            //Play normal reloading sounds
            if (isAudioActive && remainingAmmoCount > 0)
            {
                currentFireAudio?.DisableEvents();
                reloadAudio?.Play(true);
            }

            //Play empty mag reloading sounds
            if (isAudioActive && remainingAmmoCount <= 0)
            {
                currentFireAudio?.DisableEvents();
                reloadEmptyAudio?.Play(true);
            }
        }

        /// <summary>
        /// Applies the reload by adjusting ammo counts based on the reload method and available ammo.
        /// </summary>
        public void ApplyReload()
        {
            // Exit if there is no ammo available
            if (ammoProfile.count <= 0)
            {
                return;
            }

            // Apply default reload method
            if (preset.reloadMethod == ReloadType.Default)
            {
                int ammoNeeded = magazineCapacity - remainingAmmoCount;
                int ammoToReload = Mathf.Min(ammoProfile.count, ammoNeeded);

                if (ammoProfile.identifier.displayName != "No Ammo Data")
                {
                    ammoProfile.count -= ammoToReload;
                }

                remainingAmmoCount += ammoToReload;
            }

            // Mark reloading as complete
            isReloading = false;
        }

        /// <summary>
        /// Applies a specified amount of reload and updates ammo counts. Adjusts ammoProfile and remainingAmmoCount accordingly.
        /// </summary>
        /// <param name="amount">The amount of ammo to add to the magazine.</param>
        public void ApplyReloadOnce(int amount = 1)
        {
            // Exit if there is no ammo available
            if (ammoProfile.count <= 0)
            {
                isReloading = false;
                return;
            }

            // Apply reload incrementally
            ammoProfile.count -= amount;
            remainingAmmoCount += amount;

            ammoProfile.count = Mathf.Clamp(ammoProfile.count, 0, int.MaxValue);

            //Play normal reloading sounds
            if (isAudioActive && remainingAmmoCount > 0)
            {
                currentFireAudio?.DisableEvents();
                reloadAudio?.Play(true);
            }

            events.OnReloadAppliedOnce?.Invoke(amount);
        }

        /// <summary>
        /// Cancels the current reload process, resetting any ongoing reload animation and state.
        /// </summary>
        public void CancelReload()
        {
            // Exit if not currently reloading
            if (!isReloading)
            {
                return;
            }

            // Cancel any ongoing reload process
            CancelInvoke(nameof(ApplyReload));

            isReloading = false;

            // Reset the reload animation state
            foreach (var animator in animators)
            {
                animator.SetBool("Is Reloading", false);
            }

            reloadAudio?.Stop();
            reloadEmptyAudio?.Stop();

            events.OnReloadCancel?.Invoke();
        }

        /// <summary>
        /// Calculates the spread pattern for the given direction based on the current aiming state.
        /// </summary>
        /// <param name="forward">The direction to calculate the spread for.</param>
        /// <returns>A <see cref="Vector3"/> representing the calculated spread pattern.</returns>
        public Vector3 GetSprayPattern(Vector3 forward, Vector3 right, Vector3 up)
        {
            if (isAiming)
            {
                return aimDownSightsSprayPattern.CalculatePattern(this, forward, right, up, currentSprayMultiplier, ref currentSprayPointIdex,currentSprayAmount);
            }

            return hipFireSprayPattern.CalculatePattern(this, forward, right, up, currentSprayMultiplier, ref currentSprayPointIdex,currentSprayAmount);
        }

        /// <summary>
        /// Checks if any of the animators are currently playing a restricted animation.
        /// </summary>
        /// <returns>
        /// <c>true</c> if any animator is playing a restricted animation; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPlayingRestrictedAnimation()
        {
            // Initialize result as false
            bool isPlayingRestricted = false;

            // Iterate through all animators
            foreach (Animator animator in base.animators)
            {
                // Check if the animator is playing any restricted animation
                foreach (string restrictedAnimation in this.preset.restrictedAnimations)
                {
                    // If the animator is playing a restricted animation, update the result and break out of the loop
                    if (animator.IsPlaying(restrictedAnimation))
                    {
                        isPlayingRestricted = true;
                        break; // Exit the inner loop to avoid redundant checks
                    }
                }

                // If a restricted animation is found, exit the outer loop
                if (isPlayingRestricted)
                {
                    break;
                }
            }

            // Return whether a restricted animation is being played
            return isPlayingRestricted;
        }



        private void OnEnable()
        {
            // Cancel any ongoing reload actions
            CancelReload();
        }


        private void OnDisable()
        {
            // Cancel any ongoing reload actions
            CancelReload();

            // Check if a valid preset is provided
            if (character == null)
            {
                return;
            }

            if(characterManager)
            {
                characterManager.attemptingToAim = false;
                characterManager.isAiming = false;
            }

            characterManager.ResetSpeedMultiplier();
        }

        private void OnDestroy()
        {
            if (characterManager == null)
                return;

            // Reset the character's speed
            characterManager.ResetSpeedMultiplier();

            if (characterManager.cameraManager)
            {
                if (characterManager.cameraManager.cameraKickAnimation)
                {
                    characterManager.cameraManager.cameraKickAnimation.weight = 1;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(muzzle.position + currentFireDirection, 0.1f);
        }

        [ContextMenu("Setup/Network Components")]
        private void SetupNetworkComponents()
        {
            FPSFrameworkCore.InvokeConvertMethod("ConvertFirearm", this, new object[] { this });
        }

        /// <summary>
        /// State of weapon firing mode
        /// </summary>
        public enum FireMode
        {
            Auto = 0,
            SemiAuto = 1,
            Selective = 2
        }

        /// <summary>
        /// What to shot.
        /// </summary>
        public enum ShootingMechanism
        {
            Hitscan,
            Projectiles
        }

        public enum ShootingDirection
        {
            MuzzleForward,
            CameraForward,
            FromMuzzleToCameraForward,
            FromCameraToMuzzleForward
        }

        /// <summary>
        /// Type of reload. Manual needs animation events in order to function properly.
        /// </summary>
        public enum ReloadType
        {
            Default = 0,
            Scripted = 1
        }
    }
}