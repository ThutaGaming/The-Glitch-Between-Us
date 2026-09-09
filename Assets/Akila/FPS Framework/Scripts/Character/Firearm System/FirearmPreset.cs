using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace Akila.FPSFramework
{
    [CreateAssetMenu(fileName = "New Firearm", menuName = "Akila/FPS Framework/Weapons/Firearm Data")]
    public class FirearmPreset : ScriptableObject
    {
        //Basics
        public FirearmHUD firearmHud;
        public Crosshair crosshair;

        //Fire Fields
        public Firearm.ShootingMechanism shootingMechanism = Firearm.ShootingMechanism.Hitscan;
        public Firearm.ShootingDirection shootingDirection = Firearm.ShootingDirection.FromMuzzleToCameraForward;
        public Firearm.FireMode fireMode = Firearm.FireMode.Auto;
        public Vector3Direction casingDirection = Vector3Direction.right;

        public LayerMask hittableLayers = -1;

        public Projectile projectile;
        public GameObject casing;
        public GameObject defaultDecal;
        public Vector3Direction decalDirection;
        public float fireRate = 833;
        public float muzzleVelocity = 250;
        public float casingVelocity = 10;
        public float impactForce = 10;
        public float damage = 20;
        public float range = 300;
        public float maxAimDeviation = 1;


        public SprayPattern sprayPattern;
        public SprayPattern aimSprayPattern;

        public float fireTransition = 0.1f;
        public List<string> restrictedAnimations = new List<string>() { "Recharging" };

        public bool tracerRounds = true;
        public float projectileSize = 0.01f;

        public float decalSize = 1;

        public int shotCount = 1;
        public float shotDelay = 0;
        public bool alwaysApplyFire = false;

        public float gamepadVibrationAmountRight = 1;
        public float gamepadVibrationAmountLeft = 1;
        public float gamepadVibrationDuration = 0.1f;

        //Ammunition
        public AmmoProfileData ammoType;
        public int magazineCapacity = 30;
        public int reserve = 0;
        public bool canAutomaticallyReload = true;

        
        public Firearm.ReloadType reloadMethod;
        public float reloadTime = 1.6f;
        public float emptyReloadTime = 2.13f;

        public string reloadStateName = "Reload";
        public float reloadTransitionTime = 0.3f;
        public bool canCancelReloading = true;

        //Recoil
        public float cameraRecoil = 1;
        public float cameraShakeAmount = 0.05f;
        public float cameraShakeRoughness = 5;
        public float cameraShakeStartTime = 0.1f;
        public float cameraShakeDuration = 0.1f;

        public float horizontalRecoil = 0.7f;
        public float verticalRecoil = 0.1f;

        //ADS (Aim Down Sight)
        public float aimSpeed = 10;
        public float aimFieldOfView = 50;
        public float aimWeaponFieldOfview = 40;


        //Movement
        public float basePlayerSpeed = 1;
        public float aimWalkPlayerSpeed = 0.5f;
        public float fireWalkPlayerSpeed = 0.4f;


        //Audio
        [FormerlySerializedAs("fire")]
        public AudioProfile fireSound;
        [FormerlySerializedAs("fireTail")]
        public AudioProfile fireTailSound;
        [FormerlySerializedAs("reloadAudio")]
        public AudioProfile reloadSound;
        [FormerlySerializedAs("reloadEmptyAudio")]
        public AudioProfile reloadEmptySound;

        public bool isFireActive = true;
        public bool isRecoilActive = true;
        public bool isCharacterActive = true;
        public bool isAudioActive = true;
    }
}