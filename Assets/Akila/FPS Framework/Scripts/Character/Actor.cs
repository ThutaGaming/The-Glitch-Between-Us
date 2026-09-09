using UnityEngine;
using UnityEngine.Events;

namespace Akila.FPSFramework
{
    /// <summary>
    /// Represents a player or character entity within the FPS framework.
    /// Handles respawning, health, team affiliation, and basic statistics.
    /// </summary>
    [AddComponentMenu("Akila/FPS Framework/Player/Actor")]
    [DisallowMultipleComponent]
    public class Actor : MonoBehaviour, IActor
    {
        [Header("Base")]

        /// <summary>
        /// The name of the actor. Automatically set during respawn.
        /// </summary>
        [Tooltip("The name of the actor. Automatically set during respawn.")]
        public string actorName;

        [Header("Respawning")]

        /// <summary>
        /// The type of actor (e.g., Player, Bot, etc.).
        /// </summary>
        [Tooltip("The type of actor (e.g., Player, Bot, etc.).")]
        public string type = "Player";

        /// <summary>
        /// The team ID this actor belongs to.
        /// </summary>
        [Tooltip("The team ID this actor belongs to.")]
        public int teamId = 0;

        /// <summary>
        /// Whether this actor can respawn after death.
        /// </summary>
        [Tooltip("Whether this actor can respawn after death.")]
        public bool respawnable = true;

        /// <summary>
        /// Determines whether the actor is currently allowed to respawn.
        /// </summary>
        public bool RespawnActive { get; set; } = true;

        /// <summary>
        /// Event called when this actor is respawned.
        /// </summary>
        public UnityEvent OnRespawn { get; set; } = new UnityEvent();
        public UnityEvent OnDeathConfirmed { get; set; } = new UnityEvent();

        [Header("Statistics")]

        [Tooltip("Number of kills this actor has made.")]
        public int kills;

        [Tooltip("Number of times this actor has died.")]
        public int deaths;

        public bool IsPlayer
        {
            get
            {
                return GetComponent<ICharacterController>() != null;
            }
        }

        public bool playerCardActive { get; set; } = true;
        public bool playerUIEnabled { get; set; } = true;

        /// <summary>
        /// Interface used to apply and receive damage.
        /// </summary>
        public IDamageable Damageable { get; private set; }

        public PlayerCard playerCard { get; private set; }

        string IActor.actorName => actorName;

        int IActor.teamId => teamId;

        private UIManager uiManager;

        /// <summary>
        /// Initialize references to inventory, character manager, and damageable components.
        /// </summary>
        protected virtual void Awake()
        {
            Damageable = GetComponent<IDamageable>();
        }

        /// <summary>
        /// Subscribes to damage events and sets up the UI display for health and name.
        /// </summary>
        protected virtual void Start()
        {
            uiManager = UIManager.Instance;

            if (UIManager.Instance)
            {
                playerCard = UIManager.Instance.PlayerCard;

                if (playerCard != null && playerCardActive && playerUIEnabled)
                {
                    playerCard.Enable(this);
                    playerCard.UpdateCard(this);
                    playerCard.Setup(this);
                }
            }

            if (Damageable != null)
            {
                Damageable.OnDeath.AddListener(ConfirmDeath);
            }
        }

        /// <summary>
        /// Updates the UI health display each frame.
        /// </summary>
        protected virtual void Update()
        {
            if (UIManager.Instance && playerUIEnabled)
            {
                playerCard = UIManager.Instance.PlayerCard;

                if (playerCard != null && playerCardActive)
                {
                    playerCard.UpdateCard(this);
                }
            }
        }

        /// <summary>
        /// Confirms the actor's death and updates statistics and UI.
        /// </summary>
        public virtual void ConfirmDeath()
        {
            if (Damageable == null)
            {
                Debug.LogError("Damageable (IDamageable) is not set.", gameObject);

                return;
            }

            if (Damageable.DamageSource == null)
            {
                Debug.LogError("DamageSource in Damageable is not set.", gameObject);

                return;
            }

            if (Damageable.DeadConfirmed)
            {
                return;
            }

            Damageable.DeadConfirmed = true;

            OnDeathConfirmed?.Invoke();

            if(playerCard && playerCardActive && playerUIEnabled)
            {
                playerCard.Disable(this);
            }

            Damageable.DeadConfirmed = true;

            if (Damageable.DamageSource == gameObject)
                return;

            Actor killer = Damageable.DamageSource.GetComponent<Actor>();

            if (killer != null) killer.kills++;

            deaths++;

            if (uiManager != null && playerUIEnabled)
            {
                uiManager.KillFeed?.Show(killer.actorName, killer.kills, actorName);

                uiManager.Hitmarker?.Show(true);
            }
        }

        /// <summary>
        /// Initiates a respawn for the actor after the given delay.
        /// </summary>
        /// <param name="delay">Time in seconds before the actor is respawned.</param>
        public virtual void Respawn(float delay)
        {
            if (respawnable == false)
                return;

            OnRespawn?.Invoke();

            if (!RespawnActive) return;

            var spawnManager = FindFirstObjectByType<SpawnManager>();

            if (spawnManager != null)
            {
                spawnManager.SpawnActor(this, type, delay);
            }
            else
            {
                Debug.LogWarning("SpawnManager not found in the scene.");
            }
        }

        public bool IsTeam(IActor actor)
        {
            if (actor.teamId == teamId) return true;

            else
                return false;
        }
    }
}
