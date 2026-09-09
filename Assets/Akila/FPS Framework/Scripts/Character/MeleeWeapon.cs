using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Weapons/Melee Weapon")]
    public class MeleeWeapon : InventoryItem
    {
        public float range = 2;
        public float damage = 30;
        public float cameraShake = 1;
        public float camersShakeRoughtness = 1;
        public float camersShakeLenght = 0.1f;
        public GameObject defaultDecal;
        public Vector3Direction decalDirection;

        public string[] attacks;

        private int index;

        protected override void Update()
        {
            base.Update();
            
            if(itemInput.SingleFire)
            {

                foreach(string att in attacks)
                {
                    if (GetComponentInChildren<Animator>().IsPlaying(att))
                        return;
                }
                
                index++;

                if (index >= attacks.Length)
                    index = 0;

                GetComponentInChildren<Animator>().Play(attacks[index], 0, 0);

                Camera camera = Camera.main;

                Ray ray = new Ray(camera.transform.position, camera.transform.forward);
                RaycastHit[] hits = Physics.RaycastAll(ray, range);

                for(int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];

                    if (Physics.Raycast(ray, out hit, range))
                    {
                        if (hit.transform == playerObj.transform) return;

                        GameObject decal = defaultDecal;
                        IDamageable damageable = hit.transform.SearchFor<IDamageable>();

                        if (damageable != null)
                        {
                            damageable.Damage(damage, null);
                        }

                        if (hit.transform.TryGetComponent<CustomDecal>(out CustomDecal customDecal))
                        {
                            decal = customDecal.decalVFX;
                        }

                        Vector3 hitPoint = hit.point;
                        Quaternion decalRotation = FPSFrameworkCore.GetFromToRotation(hit, decalDirection);
                        GameObject decalInstance = Instantiate(defaultDecal, hitPoint, decalRotation);

                        decalInstance.transform.SetParent(hit.transform);

                        float decalLifetime = customDecal?.lifeTime ?? 60f;
                        Destroy(decalInstance, decalLifetime);
                    }
                }
                
            }
        }
    }
}