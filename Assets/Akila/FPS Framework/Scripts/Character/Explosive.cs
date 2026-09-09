using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Weapons/Explosive")]
    public class Explosive : MonoBehaviour, IDamageable, IOnHit
    {
        [Header("Base")]
        public ExplosionType type = ExplosionType.RayTracking;
        public LayerMask layerMask = -1;

        [FormerlySerializedAs("deathRadius")]
        public float deathZone = 10;
        [FormerlySerializedAs("damageRadius")]
        public float damageZone = 20;
        [FormerlySerializedAs("damage")]
        public float maxDamage = 150;
        public float force = 7;
        public float delay = 5;

        [Header("Extras")]
        public bool ignoreGlobalScale = false;
        public bool damageable = false;
        public float health = 25;
        public bool exlopeAfterDelay;
        public bool destroyOnExplode = true;
        public float clearDelay = 60;

        [Header("VFX")]
        public GameObject explosion;
        public GameObject craterDecal;
        public GameObject explosionEffect;
        public float explosionEffectForce = 1;
        public Vector3 explosionEffactOffcet;
        public Vector3 explosionEffactRotationOffset;

        [Space]
        public float explosionSize = 1;
        public float craterSize = 1;
        public float cameraShake = 1;

        [Header("Audio")]
        public bool audioLowPassFilter;
        public float lowPassCutoffFrequency = 1500;
        public float lowPassTime = 2f;
        public float lowPassSmoothness = 0.1f;

        [Header("Debug")]
        public bool debug;
        public bool ranges;
        public bool rays;


        private Rigidbody rb;

        public bool exploded { get; set; }
        public bool DeadConfirmed { get; set; }
        public Vector3 damageDirection { get; set; }
        public float maxHealth { get; set; }
        public float scale { get { return ignoreGlobalScale ? 1 : transform.lossyScale.magnitude; } }

        float IDamageable.Health { get => health; set => health = value; }
        public GameObject DamageSource { get; set; }

        public bool isActive { get; set; } = true;

        public UnityEvent OnDeath { get => onExplode; }
        public bool isDamagableDisabled { get; set; }
        public bool allowDamageableEffects { get; set; }

        public DamagableType damagableType => DamagableType.Other;

        public bool isTryingToHeal { get; set; }

        public UnityEvent onExplode;
        public Action<Transform, Vector3, Vector3, bool> onExplosionApplied;

        public GameObject sourcePlayer;

        private void Start()
        {
            maxHealth = health;
            if (exlopeAfterDelay) Explode(delay);
            
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (health <= 0) Explode();
        }

        public void Explode(float delay)
        {
            Invoke(nameof(DoExplode), delay);
        }

        //just to use invoke
        private void DoExplode() => Explode();

        public void AddEffects()
        {
            if (explosion)
            {
                Vector3 pos = Vector3.zero;
                Quaternion rot = Quaternion.identity;

                pos = transform.position + explosionEffactOffcet;
                rot = transform.rotation * Quaternion.Euler(explosionEffactRotationOffset);

                GameObject newExplosion = Instantiate(explosion, pos, rot);
                newExplosion.transform.localScale *= explosionSize;
                Destroy(newExplosion, clearDelay);
            }

            if (craterDecal)
            {
                RaycastHit ray;
                if (Physics.Raycast(transform.position, Vector3.down, out ray, deathZone * scale))
                {
                    GameObject newDecal = Instantiate(craterDecal, ray.point + Vector3.up * 0.01f, Quaternion.FromToRotation(Vector3.up, ray.normal));
                    newDecal.transform.localScale *= craterSize;
                    Destroy(newDecal, clearDelay);
                }
            }

            if (explosionEffect)
            {
                GameObject effect = Instantiate(explosionEffect, transform.position, transform.rotation);
                effect.SetActive(true);

                Destroy(effect, clearDelay);
            }
        }

        /// <summary>
        /// Tries to explode this explosive
        /// </summary>
        public void Explode()
        {
            if (exploded) return;
            
            onExplode?.Invoke();
            
            exploded = true;

            Collider[] nearColliders = Physics.OverlapSphere(transform.position, deathZone * scale, layerMask);
            Collider[] farColliders = Physics.OverlapSphere(transform.position, damageZone * scale, layerMask);

            if (isActive)
                AddEffects();

            foreach (Collider collider in nearColliders)
            {
                var dir = -(transform.position - collider.transform.position);

                if (type == ExplosionType.RayTracking)
                {
                    if (Physics.Raycast(transform.position, dir, out RaycastHit hit))
                        ApplyExplosion(hit.transform, transform.position, dir, true);
                }

                if (type == ExplosionType.Standard)
                {
                    ApplyExplosion(collider.transform, transform.position, dir, true);
                }
            }

            foreach (Collider collider in farColliders)
            {
                var dir = -(transform.position - collider.transform.position);

                if (type == ExplosionType.RayTracking)
                {
                    if (Physics.Raycast(transform.position, dir, out RaycastHit hit))
                        ApplyExplosion(hit.transform, transform.position, dir);
                }

                if (type == ExplosionType.Standard)
                {
                    ApplyExplosion(collider.transform, transform.position, dir);
                    InvokeCallbacks(sourcePlayer, new RaycastHit(), transform.position, dir, collider.transform);
                }

                if (collider.transform.TryGetComponent(out CharacterManager controller))
                {
                    float distance = Vector3.Distance(transform.position, controller.transform.position);

                    // Clamp to the damage zone range (avoid weird negatives or >1)
                    float t = Mathf.InverseLerp(deathZone, damageZone, distance);

                    // Interpolate from max to min intensity (1 -> 0)
                    float shakeIntensity = Mathf.Lerp(1, 0, t);

                    controller.cameraManager.ShakeCameras(cameraShake * 10 * shakeIntensity);

                    if (GamepadManager.Instance) GamepadManager.Instance.BeginVibration(1, 1, 0.8f);
                    
                    if (audioLowPassFilter)
                    {
                        AudioFiltersManager audioFiltersManager = null;

                        audioFiltersManager = controller.DeepSearch<AudioFiltersManager>();

                        if(audioFiltersManager != null)
                        {
                            audioFiltersManager.SetLowPass(lowPassCutoffFrequency * shakeIntensity * scale, 1000 * UnityEngine.Time.deltaTime);
                            audioFiltersManager.ResetLowPass(lowPassSmoothness * UnityEngine.Time.deltaTime, lowPassTime);
                        }
                    }
                }
            }

            if (destroyOnExplode && isActive) Destroy(gameObject);
        }

        public void InvokeCallbacks(GameObject sourcePlayer, RaycastHit hit,Vector3 origin, Vector3 direction, Transform transform)
        {
            HitInfo hitInfo = new HitInfo(sourcePlayer, hit, origin, direction);

            //Get on any hit interface
            IOnAnyHit onAnyHit = transform.GetComponent<IOnAnyHit>();
            IOnAnyHitInChildren onAnyHitInChildren = transform.GetComponentInParent<IOnAnyHitInChildren>();
            IOnAnyHitInParent onAnyHitInParent = transform.GetComponentInChildren<IOnAnyHitInParent>();

            //calls OnHit() for any object with IHitable interface
            IOnExplosionHit onHit = transform.GetComponent<IOnExplosionHit>();
            IOnExplosionHitInChildren onHitInChildren = transform.GetComponentInParent<IOnExplosionHitInChildren>();
            IOnExplosionHitInParent onHitInParent = transform.GetComponentInChildren<IOnExplosionHitInParent>();

            //calls OnHit() for any object with IHitable interface
            IOnRayTrackingExplosionHit rayTrackingOnHit = transform.GetComponent<IOnRayTrackingExplosionHit>();
            IOnRayTrackingExplosionHitInChildren rayTrackingOnHitInChildren = transform.GetComponentInParent<IOnRayTrackingExplosionHitInChildren>();
            IOnRayTrackingExplosionHitInParent rayTrackingOnHitInParent = transform.GetComponentInChildren<IOnRayTrackingExplosionHitInParent>();

            //Call on any hits
            if (onAnyHit != null) onAnyHit.OnAnyHit(hitInfo);
            if (onAnyHitInChildren != null && onAnyHit == null) onAnyHitInChildren.OnAnyHitInChildren(hitInfo);
            if (onAnyHitInParent != null && onAnyHit == null) onAnyHitInParent.OnAnyHitInParent(hitInfo);

            //Call on hit
            if (onHit != null) onHit.OnExplosionHit(hitInfo);
            if (onHitInChildren != null && onHit == null) onHitInChildren.OnExplosionHitInChildren(hitInfo);
            if (onHitInParent != null && onHit == null) onHitInParent.OnExplosionHitInParent(hitInfo);

            //Call on ray tracking hit
            if (rayTrackingOnHit != null) rayTrackingOnHit.OnRayTrackingExplosionHit(hitInfo);
            if (rayTrackingOnHitInChildren != null && rayTrackingOnHit == null) rayTrackingOnHitInChildren.OnRayTrackingExplosionHitInChildren(hitInfo);
            if (rayTrackingOnHitInParent != null && rayTrackingOnHit == null) rayTrackingOnHitInParent.OnRayTrackingExplosionHitInParent(hitInfo);
        }

        public void ApplyExplosion(Transform obj, Vector3 origin, Vector3 dir, bool kill = false)
        {
            onExplosionApplied?.Invoke(obj, origin, dir, kill);

            if (!isActive)
                return;

            if (obj != transform)
            {
                float finalDamage = maxDamage;

                if (obj.SearchFor<IDamageable>() != null)
                {
                    IDamageable damageable = obj.SearchFor<IDamageable>();

                    float distanceFromDamageable = Vector3.Distance(origin, obj.position);
                    float damageMultiplier = Mathf.Lerp(1, 0, distanceFromDamageable / damageZone);

                    finalDamage *= damageMultiplier;

                    if (kill) finalDamage = float.MaxValue;
                    
                    damageable.Damage(finalDamage, DamageSource);
                }
            }

            if(obj.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddExplosionForce(force, transform.position, damageZone, 1, ForceMode.VelocityChange);
            }
        }

        private void OnDrawGizmos()
        {
            if (!debug) return;

            if (ranges)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, deathZone * scale);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, damageZone * scale);
            }

            if (rays && type == ExplosionType.RayTracking)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, damageZone * scale, layerMask);
                foreach (Collider collider in colliders)
                {
                    RaycastHit hit;
                    var dir = -(transform.position - collider.transform.position);
                    if (Physics.Raycast(transform.position, dir, out hit))
                    {
                        if (hit.transform == transform) return;
                        bool hasDamageble = hit.transform.TryGetComponent(out IDamageable damageable) && collider.transform.GetComponentInChildren<IDamageablePart>() == null;
                        bool hadDamageableParent = hit.transform.TryGetComponent(out IDamageablePart damageableParent);
                        bool hasRigidbody = hit.transform.TryGetComponent(out Rigidbody rb);

                        if (hasDamageble || hadDamageableParent || hasRigidbody)
                        {
                            Gizmos.color = hit.distance >= deathZone ? Color.white : Color.red;
                            Gizmos.DrawLine(transform.position, hit.point);
                            Gizmos.DrawSphere(hit.point, 0.1f * transform.lossyScale.magnitude);
                        }
                    }
                }

                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, new Vector3(0, -deathZone * scale, 0) * transform.lossyScale.magnitude);

            }

            if (rays && type == ExplosionType.Standard)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, damageZone * scale, layerMask);
                foreach (Collider collider in colliders)
                {
                    bool hasDamageble = collider.transform.TryGetComponent(out IDamageable damageable) && collider.transform.GetComponentInChildren<IDamageablePart>() == null;
                    bool hadDamageablePart = collider.transform.TryGetComponent(out IDamageablePart damageablePart);
                    bool hasRigidbody = collider.transform.TryGetComponent(out Rigidbody rb);

                    if (hasDamageble || hadDamageablePart || hasRigidbody)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawLine(transform.position, collider.transform.position);
                        Gizmos.DrawSphere(collider.transform.position, 0.1f * transform.lossyScale.magnitude);
                    }
                }

                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, new Vector3(0, -damageZone * scale, 0) * transform.lossyScale.magnitude);

            }
        }
        
        public void Damage(float amount, GameObject damageSource)
        {
            if(!damageable) return;

            health -= amount;
            this.DamageSource = damageSource;
        }

        public void OnHit(HitInfo hitInfo)
        {
            sourcePlayer = hitInfo.sourcePlayer;
        }

        [ContextMenu("Setup/Network Components")]
        private void SetupNetworkComponents()
        {
            FPSFrameworkCore.InvokeConvertMethod("ConvertExplosive", this, new object[] { this });
        }
    }

    public enum ExplosionType
    {
        Standard,
        RayTracking
    }
}