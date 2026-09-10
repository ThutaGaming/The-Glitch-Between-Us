using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class SciFiHealthHud : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image fill;
    public TextMeshProUGUI label;

    private void Update()
    {
        if (playerHealth == null) playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth == null) return;
        if (fill != null) fill.fillAmount = playerHealth.Normalized;
        if (label != null) label.text = playerHealth.CurrentHealth + " / " + playerHealth.maxHealth;
    }
}