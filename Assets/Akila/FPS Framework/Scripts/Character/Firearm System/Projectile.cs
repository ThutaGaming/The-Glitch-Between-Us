using Akila.FPSFramework.Internal;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Weapons/Projectile")]
    public class Projectile : MonoBehaviour
    {
        [Header("Base Settings")]
        public LayerMask hittableLayers = -1;
        public Vector3Direction decalDirection = Vector3Direction.forward;
        public float speed = 50;
        public float gravity = 1;
        public float force = 10;
        public int lifeTime = 5;
        public GameObject defaultDecal;
        public float hitRadius = 0.03f;

        [Header("Additional Settings")]
        public bool destroyOnImpact = false;
        public bool useSourceVelocity = true;
        public bool useAutoScaling = true;
        public float scaleMultipler = 45;

        [Header("Range Control")]
        public float range = 300;
        public AnimationCurve damageRangeCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0.3f) });


        public Firearm source { get; set; }
        public Vector3 direction { get; set; }
        public Vector3 initialVelocity { get; set; }
        public float maxVelocity { get; protected set; }
        private Vector3 velocity;
        private TrailRenderer trail;
        private Explosive explosive;

        private Rigidbody rb;

        private Vector3 previousPosition;

        private Transform Effects;

        private Vector3 startPosition;

        public UnityEvent<GameObject, Ray, RaycastHit> onHit { get; set; } = new UnityEvent<GameObject, Ray, RaycastHit>();

        public GameObject sourcePlayer { get; set; }

        protected virtual void DestroySelf()
        {
            if (isActive)
                Destroy(gameObject, lifeTime);
        }

        /// <summary>
        /// Setup all this projectile's fields.
        /// </summary>
        /// <param name="source">Source firearm which this projectile will copy things from</param>
        /// <param name="direction">The direction of movement for this projectile</param>
        /// <param name="initialVelocity">The initial velocity for this projectile. By default this field is used for shooter velocity</param>
        /// <param name="speed">The maximum speed for this projectiles</param>
        /// <param name="range">The maximum distance from the initial firing location, if this is in half of the distance and max damage is 10, damage will be 5.</param>
        public void Setup(Firearm source, Vector3 direction, Vector3 initialVelocity, float speed, float range)
        {
            float muzzleModifier = source?.firearmAttachmentsManager != null ? source.firearmAttachmentsManager.muzzleVelocity : 1;

            //Player and source firearm
            this.source = source;

            if (source && source.actor != null)
                this.sourcePlayer = source.actor.gameObject;

            //Direction and speed
            this.direction = direction;
            this.initialVelocity = initialVelocity;
            this.range = range * source.firearmAttachmentsManager.range;
            this.speed = speed * muzzleModifier;

            //Scale
            if (source)
            {
                useAutoScaling = source.preset.tracerRounds;
                scaleMultipler = source.preset.projectileSize;

                //Final setup
                source.Projectiles?.Add(this);
            }
        }

        private void Awake()
        {
            previousPosition = transform.position;
            startPosition = transform.position;
        }

        protected virtual void Start()
        {
            explosive = GetComponent<Explosive>();
            trail = GetComponentInChildren<TrailRenderer>();
            rb = GetComponent<Rigidbody>();

            
            Vector3 sorceVelocity = useSourceVelocity ? initialVelocity : Vector3.zero;

            velocity = (direction) * (speed) + sorceVelocity;

            if (isActive)
                rb.AddForce(velocity, ForceMode.VelocityChange);

            if (source)
                maxVelocity = source.preset.muzzleVelocity;

            if (transform.Find("Effects"))
            {
                Effects = transform.Find("Effects");
                Effects.parent = null;
                Destroy(gameObject, lifeTime + 1);
            }

            if (explosive && source && source.actor != null) explosive.DamageSource = source.actor.gameObject;

            transform.localScale = useAutoScaling ? Vector3.zero : Vector3.one * scaleMultipler;

            if (trail) trail.widthMultiplier = useAutoScaling ? 0 : scaleMultipler;

            if(isActive)
                Destroy(gameObject, lifeTime);
        }

        public float CalculateDamage()
        {
            float distanceFromStartPos = Vector3.Distance(transform.position, startPosition);

            float countFactor =  source.preset.shotCount;

            if (source != null)
            {
                distanceFromStartPos = Mathf.Clamp(distanceFromStartPos, 1, float.MaxValue);

                float posToRange = distanceFromStartPos / range;

                posToRange = Mathf.Clamp01(posToRange);

                float damageToRange = damageRangeCurve.Evaluate(posToRange);

                float finalDamage = damageToRange * source.preset.damage / countFactor;

                return finalDamage;
            }

            Debug.LogError("Couldn't calculate damage due to null source firearm field. Damage will be default to 30.", gameObject);

            return 30;
        }

        private void Update()
        {
            Ray ray = new Ray(previousPosition, -(previousPosition - transform.position));
            float distance = Vector3.Distance(transform.position, previousPosition);

            RaycastHit[] hits = Physics.SphereCastAll(ray, hitRadius, distance, hittableLayers);

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];

                if (hit.point != Vector3.zero && distance != 0)
                {
                    UpdateHits(ray, hit);
                }
            }

            if (useAutoScaling)
            {
                float distanceFromMainCamera = 1;
                float scale = 1;

                Camera mainCamera = FPSFrameworkUtility.GetMainCamera();

                if (mainCamera != null)
                {
                    distanceFromMainCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
                    scale = (distanceFromMainCamera * scaleMultipler) * (mainCamera.fieldOfView / 360);
                }

                transform.localScale = Vector3.one * scale;
                if (trail) trail.widthMultiplier = scale;
            }

            if (!useAutoScaling)
            {
                transform.localScale = Vector3.one * scaleMultipler;
            }

            if (Effects)
            {
                Effects.position = transform.position;
            }
        }

        private void FixedUpdate()
        {
            rb.AddForce(Physics.gravity * gravity, ForceMode.Acceleration);
        }

        private void LateUpdate()
        {
            previousPosition = transform.position;
        }

        protected virtual void UpdateHits(Ray ray, RaycastHit hit)
        {
            if (source == null) return;

            //stop if object has ignore component
            if (hit.transform == null) return;

            if (hit.transform.TryGetComponent(out Ignore _ignore) && _ignore.ignoreHitDetection || sourcePlayer && hit.transform == sourcePlayer.transform) return;
            onHit?.Invoke(hit.transform.gameObject, ray, hit);
            OnHit(hit);

            if (!isActive) return;

            if (explosive)
            {
                explosive.Explode();
                Destroy(gameObject);
                return;
            }

            Firearm.UpdateHits(source.firearm, defaultDecal, ray, hit, CalculateDamage(), decalDirection);
        }

        public bool isActive { get; set; } = true;

        public virtual void OnHit(RaycastHit hit)
        {

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, hitRadius);
        }

        [ContextMenu("Setup/Network Components")]
        public void Convert()
        {
            FPSFrameworkCore.InvokeConvertMethod("ConvertProjectile", this, new object[] { this });
        }
    }
}