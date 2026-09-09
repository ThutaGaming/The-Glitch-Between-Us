using Akila.FPSFramework.Internal;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Akila.FPSFramework
{
    [AddComponentMenu("Akila/FPS Framework/Managers/Spwan Manager")]
    public class SpawnManager : MonoBehaviour
    {
        [FormerlySerializedAs("spwanableObjects")]
        public List<SpwanableObject> spawnableObjects = new List<SpwanableObject>();

        public float spawnRadius = 5;
        public float respawnDelay = 5;

        [Separator]
        public List<SpwanSide> sides;

        public static SpawnManager Instance;

        public bool isActive { get; set; } = true;

        public UnityEvent<GameObject> onPlayerSpwanWithObj { get; set; } = new UnityEvent<GameObject>();
        public UnityEvent<string> onPlayerSpwanWithObjName { get; set; } = new UnityEvent<string>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public async void SpawnActor(IActor actorSelf, string actorObjName, float delay)
        {
            if (Application.isPlaying == false)
                return;

            IActor copyOfActor = Instantiate(actorSelf.gameObject).gameObject.GetComponent<IActor>();

            //copyOfActor.gameObject.hideFlags = HideFlags.HideInHierarchy;

            copyOfActor.gameObject.SetActive(false);

            float time = 0;

            while (time < delay)
            {
                time += Time.deltaTime;

                if (Application.isPlaying == false)
                    return;

                await Task.Yield();
            }

            if (Application.isPlaying == false) return;

            SpawnActor(copyOfActor, actorObjName);

            Destroy(copyOfActor.gameObject);
        }

        public void SpawnActor(IActor actorSelf, string actorObjName)
        {
            GameObject obj = spawnableObjects.Find(x => x.name == actorObjName).obj;

            GameObject newPlayer = SpawnActor( actorSelf, obj);

            Actor newPlayerActorComponent = newPlayer.GetComponent<Actor>();
            Actor actorSelfActorComponent = actorSelf.gameObject.GetComponent<Actor>();

            if (newPlayerActorComponent && actorSelfActorComponent)
            {
                newPlayerActorComponent.kills = actorSelfActorComponent.kills;
                newPlayerActorComponent.deaths = actorSelfActorComponent.deaths;
            }

            onPlayerSpwanWithObjName?.Invoke(actorObjName);
        }

        public GameObject SpawnActor(IActor actorSelf, GameObject actorObj)
        {
            onPlayerSpwanWithObj?.Invoke(actorObj);

            if(!isActive) return null;

            Vector3 actorPosition = GetPlayerPosition(actorSelf.teamId);
            Quaternion actorRotation = GetPlayerRotation(actorSelf.teamId);

            GameObject newActorObject = Instantiate(actorObj, actorPosition, actorRotation);

            Actor newPlayerActorComponent = newActorObject.GetComponent<Actor>();
            Actor actorSelfActorComponent = actorSelf.gameObject.GetComponent<Actor>();


            if (newPlayerActorComponent && actorSelfActorComponent)
            {
                newPlayerActorComponent.kills = actorSelfActorComponent.kills;
                newPlayerActorComponent.deaths = actorSelfActorComponent.deaths;
            }

            Vector3 position = GetPlayerPosition(actorSelf.teamId);
            Quaternion rotation = GetPlayerRotation(actorSelf.teamId);

            newActorObject.transform.SetPositionAndRotation(position, rotation);

            return newActorObject;
        }

        public Transform GetPlayerSpawnPoint(int sideId)
        {
            int pointIndex = Random.Range(0, sides[sideId].points.Length);

            return sides[sideId].points[pointIndex];
        }

        public Vector3 GetPlayerPosition(int sideId)
        {
            Vector3 addedPosition = Random.insideUnitCircle * spawnRadius;

            addedPosition.z = addedPosition.y;

            addedPosition.y = 0;

            return GetPlayerSpawnPoint(sideId).position + addedPosition;
        }

        public Quaternion GetPlayerRotation(int sideId)
        {
            return GetPlayerSpawnPoint(sideId).rotation;
        }

        private void OnDrawGizmos()
        {
            foreach (SpwanSide point in sides)
            {
                foreach (Transform transform in point.points)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(transform.position, spawnRadius * transform.lossyScale.magnitude);
                }
            }
        }

        [ContextMenu("Setup/Network Components")]
        private void SetupNetworkComponents()
        {
            FPSFrameworkCore.InvokeConvertMethod("ConvertSpawnManager", this, new object[] { this });
        }

        [System.Serializable]
        public class SpwanSide
        {
            public Transform[] points;
        }

        [System.Serializable]
        public class SpwanableObject
        {
            public string name;
            public GameObject obj;
        }
    }
}