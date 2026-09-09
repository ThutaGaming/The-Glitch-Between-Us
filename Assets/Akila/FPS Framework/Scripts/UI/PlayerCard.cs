using Akila.FPSFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Akila.FPSFramework
{
    public class PlayerCard : MonoBehaviour
    {
        public TextMeshProUGUI playerNameText;
        public Slider playerHealthBar;
        public Slider playerDamageBar;
        public float damageFollowSpeed = 1;

        private float currentDamage;

        private bool isEnabled;

        private float maxHealth;

        public void Setup(IActor actor)
        {
            if (CheckIfIsPlayer(actor) == false)
                return;

            IDamageable damageable = actor.gameObject.GetComponent<IDamageable>();

            if (damageable == null)
            {
                Debug.LogError("Couldn't find 'IDamagale' on Actor. HealthBar update aborted.");

                return;
            }

            maxHealth = damageable.Health;
        }

        public void UpdateCard(IActor actor)
        {
            if (CheckIfIsPlayer(actor) == false)
                return;

            if (playerHealthBar == null || playerDamageBar == null) return;

            currentDamage = Mathf.Lerp(currentDamage, playerHealthBar.value, Time.deltaTime * 5 * damageFollowSpeed);

            playerDamageBar.value = currentDamage;

            if (playerNameText == null)
            {
                Debug.LogError("'PlayerNameText' is null. Player name update aborted.");
            }
            else
            {
                string actorName = string.IsNullOrEmpty(actor.actorName) ? "UNKNOWN_ACTOR_NAME" : actor.actorName;

                playerNameText.SetText(actorName);
            }

            IDamageable damageable = actor.gameObject.GetComponent<IDamageable>();

            if (damageable == null)
            {
                Debug.LogError("Couldn't find 'IDamagale' on Actor. HealthBar update aborted.");

                return;
            }

            if (playerHealthBar == null)
            {
                Debug.LogError("PlayerHealthBar is null.");

                return;
            }
            else
            {
                playerHealthBar.maxValue = maxHealth;
                playerHealthBar.value = damageable.Health;
            }
        }

        public void Enable(IActor actor)
        {
            if (CheckIfIsPlayer(actor) == false)
                return;

            isEnabled = true;

            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(isEnabled);
            }
        }

        public void Disable(IActor actor)
        {
            if (CheckIfIsPlayer(actor) == false)
                return;

            isEnabled = false;

            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(isEnabled);
            }
        }

        public virtual bool CheckIfIsPlayer(IActor actor)
        {
            if (actor == null)
            {
                Debug.LogError("'Actor' is null. PlayerCard.UpdateCard() will be aborted.");

                return false;
            }

            if (actor.transform.GetComponent<CharacterManager>() != null)
                return true;

            return false;
        }
    }
}