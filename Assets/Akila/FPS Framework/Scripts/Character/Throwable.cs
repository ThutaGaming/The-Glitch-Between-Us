using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Threading.Tasks;
using System;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Weapons/Throwable")]
    [RequireComponent(typeof(ItemInput))]
    public class Throwable : InventoryItem
    {
        public Rigidbody throwableItem;
        public Transform throwPoint;
        public Pickable pickableItem;
        public AmmoProfileData ammoProfile;
        public float minimumTriggerTime = 0.75f;
        public float throwForce = 25;
        public string triggerStateName = "Trigger";

        [Space]
        [Header("Cooking (For Explosives Only)")]
        public bool canCook = true;

        private float cookingTimer = 0;
        private float maxCookingTime = 0;

        public InventoryCollectable Ammo { get; set; }

        protected override void Awake()
        {
            base.Awake();

            Setup(pickableItem);
        }

        private void OnEnable()
        {
            itemInput?.ThrowAction?.Enable();
        }

        private void OnDisable()
        {
            itemInput?.ThrowAction?.Disable();
        }

        protected virtual void Start()
        {
            if (inventory != null && ammoProfile != null)
            {
                Ammo = inventory.collectables.Find(i => i.identifier == ammoProfile);
            }

            if (Ammo == null)
            {
                Ammo = new InventoryCollectable();

                Ammo.count = 3;

                Debug.LogWarning($"Couldn't find ammo profile with the identifer '{ammoProfile}' in the IInventory.", gameObject);
            }

            itemInput.ThrowAction.performed += ctx =>
            {
                if (FPSFrameworkCore.IsPaused == false && Ammo.count > 0 && proceduralAnimator.IsDefaultingInRotation(5, true, true, false))
                {
                    if (throwableItem.TryGetComponent<Explosive>(out Explosive explosive))
                    {
                        maxCookingTime = explosive.delay;
                    }

                    cookingTimer = 0;

                    foreach (Animator animator in animators)
                        animator.SetTrigger("Trigger");
                }
            };

            itemInput.ThrowAction.canceled += ctx =>
            {
                if (FPSFrameworkCore.IsPaused == false)
                    PerformThrow();
            };

            if(replacement != null)
            {
                if(replacement.includeCollectable)
                {
                    OnDropPerformed.AddListener(ApplyDrop);
                }
            }
        }

        private void ApplyDrop(Pickable pickable)
        {
            pickable.collectableCount = Ammo.count;

            //Reset player throwable count of this type to avoid ammo adding up
            Ammo.count = 0;
        }

        protected override void Update()
        {
            base.Update();

            if (FPSFrameworkCore.IsPaused) return;

            if (itemInput.DropInput) Drop();

            if (itemInput.ThrowAction.IsPressed() && canCook)
            {
                cookingTimer += Time.deltaTime;
            }

            if (cookingTimer >= maxCookingTime) PerformThrow();
        }

        private void FixedUpdate()
        {
            foreach (Animator animator in animators)
            {
                //Reset all bool, to stops animation looping.
                animator?.SetBool("Trigger", false);
                animator?.SetBool("Throw", false);
            }
        }

        public void Throw()
        {
            //Do throw logic
            Rigidbody newThrowable = Instantiate(throwableItem, throwPoint.position, throwPoint.rotation);

            newThrowable.AddForce(throwPoint.forward * throwForce, ForceMode.VelocityChange);

            if (newThrowable.TryGetComponent<Explosive>(out Explosive explosive))
            {
                explosive.DamageSource = actor.gameObject;
                explosive.delay = maxCookingTime - cookingTimer;
            }

            Ammo.count--;
        }

        public async void PerformThrow()
        {
            //Stops animation overlapping.
            foreach (Animator animator in animators)
            {
                if (!animator.IsPlaying(triggerStateName)) return;
            }

            float currentTime = 0;

            //Wait until the triggering time is reached and then apply throw.
            while (currentTime < minimumTriggerTime && cookingTimer < minimumTriggerTime)
            {
                currentTime += Time.deltaTime;

                await Task.Yield();
            }

            foreach (Animator animator in animators)
                animator?.SetTrigger("Throw");
        }
    }
}