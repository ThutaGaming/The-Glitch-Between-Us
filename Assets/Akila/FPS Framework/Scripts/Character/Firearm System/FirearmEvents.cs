using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Akila.FPSFramework
{
    [Serializable]
    public class FirearmEvents
    {
        [Tooltip("Action to be invoked when firing is starting, passing the position, rotation, and direction of the projectile. If 'NumberOfShots' is 2, this event will be invoked once the fire action is triggred once.")]
        public UnityEvent<Vector3, Quaternion, Vector3> OnFireDemand;
        [Tooltip("Action to be invoked when firing is completed once, passing the position, rotation, and direction of the projectile. If 'NumberOfShots' is 2, this event will be invoked twice each time the firearm is fired.")]
        public UnityEvent<Vector3, Quaternion, Vector3> OnFireDone;
        [Tooltip("Action to be invoked when post firing events are done once e.g firing sound and effects")]
        public UnityEvent OnFireApplied;
        [Tooltip("Action to be invoked when reloading is starting."), FormerlySerializedAs("OnReload")]
        public UnityEvent OnReloadStarting;
        [Tooltip("Action to be invoked when reloading is started. This is invoked when IsReloading state changes.")]
        public UnityEvent OnReloadStart;
        [Tooltip("Action to be invoked when reloading is completed.")]
        public UnityEvent OnReloadComplete;
        [Tooltip("Action to be invoked when reloading is applied once."), FormerlySerializedAs("OnReloadApplied")]
        public UnityEvent<int> OnReloadAppliedOnce;
        [Tooltip("Action to be invoked when relpading is cancled.")]
        public UnityEvent OnReloadCancel;
        [Tooltip("Action to be invoked when FireMode state is changed.")]
        public UnityEvent OnFireModeChange;

        ///TODO: Add these events
        ///OnAim
        ///OnDropped
        ///OnItemPickedUp
        ///OnDryFire
        ///OnOutOfAmmo
        ///OnOutOfAmmoType
    }
}